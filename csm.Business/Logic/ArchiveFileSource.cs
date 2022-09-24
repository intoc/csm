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

        private const string CSM_TEMP_FOLDER = "csm_e2bd2683";
        protected readonly DirectoryInfo _csmTempFolder;
        protected readonly DirectoryInfo _tempDir;
        protected readonly string _archiveFilePath;
        private readonly object _dirLock = new();
        private bool _extracted = false;
 
        /// <summary>
        /// Creates an instance of this file source with the given archive file path
        /// </summary>
        /// <param name="path">The path to the archive file</param>
        protected ArchiveFileSource(string path) {
            // Create the parent temp directory for csm if it doesn't exist
            _csmTempFolder = new DirectoryInfo(Path.Combine(Path.GetTempPath(), CSM_TEMP_FOLDER));
            if (!_csmTempFolder.Exists) {
                _csmTempFolder.Create();
            }
            // Create the temp folder for this archive extraction if it doesn't exist
            _tempDir = new DirectoryInfo(Path.Combine(_csmTempFolder.FullName, $"{Guid.NewGuid()}"));
            if (!_tempDir.Exists) {
                _tempDir.Create();
            }
            _archiveFilePath = path;
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
            Log.Debug("{0} - Extracting rar file to {1}", GetType().Name, _tempDir.FullName);
            Extract();
            sw.Stop();
            Log.Debug("{0} - Extraction complete. Time: {1}", GetType().Name, sw.Elapsed);
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

            // Look for old temp directories and delete them
            var dirs = _csmTempFolder.GetDirectories();
            foreach (var dir in dirs) {
                if (DateTime.Now - dir.CreationTime > TimeSpan.FromHours(1)) {
                    dir.Delete(true);
                    Log.Debug("Deleted old temp directory {0}", _tempDir.FullName);
                }
            }
        }
    }
}
