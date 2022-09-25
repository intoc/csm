using csm.Business.Models;
using Serilog;
using System.Diagnostics;

namespace csm.Business.Logic {

    /// <summary>
    /// Abstract class for implementations of archive file sources
    /// </summary>
    public abstract class ArchiveFileSource : AbstractFileSource {

        /// <summary>
        /// The full path of the archive file
        /// </summary>
        public override string? FullPath => Path.GetFullPath(_archiveFilePath);

        /// <summary>
        /// The name of the archive file without its extension
        /// </summary>
        public override string? Name => Path.GetFileNameWithoutExtension(_archiveFilePath);

        protected readonly DirectoryInfo _tempDir;
        protected readonly string _archiveFilePath;
        private readonly object _dirLock = new();
        private bool _extracted = false;
 
        /// <summary>
        /// Creates an instance of this file source with the given archive file path
        /// </summary>
        /// <param name="path">The path to the archive file</param>
        protected ArchiveFileSource(string path) : base() {
            // Create the temp folder for this archive extraction if it doesn't exist
            _tempDir = new DirectoryInfo(Path.Combine(_csmTempFolder.FullName, $"{Guid.NewGuid()}"));
            if (!_tempDir.Exists) {
                _tempDir.Create();
            }
            _archiveFilePath = path;
            if (File.Exists(path)) {
                Bytes = new FileInfo(path).Length;
            }
        }

        /// <summary>
        /// Extracts the archive file to a temp directory so that the files inside it can be accessed
        /// </summary>
        /// <param name="callback">Called when extraction complets</param>
        public override void Initialize(Action? callback = null) {
            Task.Run(() => {
                lock (_dirLock) {
                    if (!_extracted) {
                        ExtractWithStats();
                        _extracted = true;
                    }
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
            IEnumerable<ImageFile> files = new List<ImageFile>();
            await Task.Run(() => {
                lock (_dirLock) {
                    if (!_extracted) {
                        ExtractWithStats();
                        _extracted = true;
                    }
                    files = GetFiles(_tempDir, pattern);
                }
            });
            return files;
        }

        private void ExtractWithStats() {
            Stopwatch sw = Stopwatch.StartNew();
            Log.Information("{0} - Extracting archive {1} to {2}", GetType().Name, Path.GetFileName(_archiveFilePath), _tempDir.FullName);
            Extract();
            sw.Stop();
            Log.Information("{0} - Extraction complete. Time: {1}", GetType().Name, sw.Elapsed);
        }

        protected abstract void Extract();

        /// <summary>
        /// Deletes temp files
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing) {
            lock (_dirLock) {
                if (_tempDir.Exists) {
                    lock (_dirLock) {
                        _tempDir.Delete(true);
                    }
                    Log.Debug("Deleted {0}", _tempDir.FullName);
                }
                base.Dispose(disposing);
            }
        }
    }
}
