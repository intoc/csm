using csm.Business.Models;
using Serilog;
using System.Diagnostics;

namespace csm.Business.Logic {
    public abstract class ArchiveFileSource : AbstractFileSource {
        public override string? FullPath => Path.GetFullPath(_archiveFilePath);

        public override string? Name => Path.GetFileNameWithoutExtension(_archiveFilePath);

        protected readonly string _archiveFilePath;
        protected readonly DirectoryInfo _tempDir;

        private bool _extracted = false;
        private readonly object _dirLock = new();

        protected ArchiveFileSource(string path) {
            _archiveFilePath = path;
            _tempDir = new DirectoryInfo(Path.Combine(Path.GetTempPath(), $"csm_{Guid.NewGuid()}"));
            if (!_tempDir.Exists) {
                _tempDir.Create();
            }
        }

        protected override void Dispose(bool disposing) {
            lock (_dirLock) {
                if (_tempDir.Exists) {
                    Log.Debug("Deleting {0}", _tempDir.FullName);
                    lock (_dirLock) {
                        _tempDir.Delete(true);
                    }
                    Log.Debug("Deleted {0}", _tempDir.FullName);
                }
                base.Dispose(disposing);
            }
            // TODO:
            // Delete old directories?
            // Use a global static GUID for parent folder and delete parent folders without that GUID
        }

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

        protected abstract void Extract();

        private void ExtractWithStats() {
            Stopwatch sw = Stopwatch.StartNew();
            Log.Debug("{0} - Extracting rar file to {1}", GetType().Name, _tempDir.FullName);
            Extract();
            sw.Stop();
            Log.Debug("{0} - Extraction complete. Time: {1}", GetType().Name, sw.Elapsed);
        }
    }
}
