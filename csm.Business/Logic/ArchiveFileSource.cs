﻿using csm.Business.Models;
using Serilog;
using System.Diagnostics;
using System.IO;

namespace csm.Business.Logic {

    /// <summary>
    /// Abstract class for implementations of archive file sources
    /// </summary>
    public abstract class ArchiveFileSource : AbstractFileSource {

        /// <summary>
        /// The full path of the archive file
        /// </summary>
        public override string FullPath => Path.GetFullPath(_archiveFilePath);

        /// <summary>
        /// The full path of the temp image file directory
        /// </summary>
        public override string ImageFileDirectoryPath => _tempDir.FullName;


        /// <summary>
        /// The name of the archive file without its extension
        /// </summary>
        public override string Name => Path.GetFileNameWithoutExtension(_archiveFilePath);

        protected readonly DirectoryInfo _tempDir;
        protected readonly IDictionary<string, bool> entryCompletion;
        protected readonly string _archiveFilePath;
        protected Stopwatch _timer = new();

        private readonly object _dirLock = new();
        private bool _extracted = false;

        /// <summary>
        /// Creates an instance of this file source with the given archive file path
        /// </summary>
        /// <param name="path">The path to the archive file</param>
        protected ArchiveFileSource(string path, ILogger logger) : base(logger.ForContext("Context", Path.GetFileNameWithoutExtension(path))) {
            // Create the temp folder for this archive extraction if it doesn't exist
            _tempDir = new DirectoryInfo(Path.Combine(_csmTempFolder.FullName, $"{Guid.NewGuid()}"));
            if (!_tempDir.Exists) {
                _tempDir.Create();
            }
            _archiveFilePath = path;
            if (File.Exists(path)) {
                Bytes = new FileInfo(path).Length;
            }
            entryCompletion = new Dictionary<string, bool>();
        }

        /// <summary>
        /// Extracts the archive file to a temp directory so that the files inside it can be accessed
        /// </summary>
        /// <param name="callback">Called when extraction complets</param>
        public override async Task Initialize(Action? callback = null) {
            if (_isDisposed) {
                return;
            }
            await Task.Run(() => {
                lock (_dirLock) {
                    ExtractWithStats();
                    callback?.Invoke();
                }
            });
        }

        /// <summary>
        /// Gets the files from the archive
        /// </summary>
        /// <param name="pattern">The search pattern for the files</param>
        /// <returns>The files as <see cref="ImageFile"/> instances</returns>
        public override async Task<IEnumerable<ImageFile>> GetFilesAsync(string? pattern = null) {
            if (_isDisposed) {
                return Enumerable.Empty<ImageFile>();
            }
            IEnumerable<ImageFile> files = new List<ImageFile>();
            await Task.Run(() => {
                lock (_dirLock) {
                    ExtractWithStats();
                    files = GetFiles(_tempDir, pattern);
                }
            });
            return files;
        }

        private void ExtractWithStats() {
            if (!_extracted) {
                _timer.Start();
                _logger.Information("{0} - Extracting archive {1} to {2}", GetType().Name, Path.GetFileName(_archiveFilePath), _tempDir.FullName);
                UpdateProgress(new ProgressEventArgs(0, 100, _timer.Elapsed, FullPath));
                Extract();
                _timer.Stop();
                _logger.Information("{0} - Extraction complete. Time: {1}", GetType().Name, _timer.Elapsed);
                UpdateProgress(new ProgressEventArgs(100, 100, _timer.Elapsed, FullPath));
                _extracted = true;
            }
        }

        protected abstract void Extract();

        /// <summary>
        /// Deletes temp files
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing) {
            _isDisposed = true;
            lock (_dirLock) {
                if (_tempDir.Exists) {
                    _tempDir.Delete(true);
                    _logger.Debug("Deleted {0}", _tempDir.FullName);
                }
                base.Dispose(disposing);
            }
        }
    }
}
