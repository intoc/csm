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

        public static ArchiveFileSource Build(string path) {
            var info = new FileInfo(path);
            if (ZipFileSource.Supports(info.Extension)) {
                return new ZipFileSource(path);
            }
            if (RarFileSource.Supports(info.Extension)) {
                return new RarFileSource(path);
            }
            if (SevenZipFileSource.Supports(info.Extension)) {
                return new SevenZipFileSource(path);
            }
            throw new NotImplementedException($"({info.Extension}) file source not supported.");
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
            Log.Information("{0} - Extracting rar file to {1}", GetType().Name, _tempDir.FullName);
            Extract();
            sw.Stop();
            Log.Information("{0} - Extraction complete. Time: {1}", GetType().Name, sw.Elapsed);
        }
    }
}
