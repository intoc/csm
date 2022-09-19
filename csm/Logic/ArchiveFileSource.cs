namespace csm.Logic {
    public abstract class ArchiveFileSource : AbstractFileSource {

        public override bool IsReady => File.Exists(_archiveFilePath);

        public override string? FullPath => Path.GetFullPath(_archiveFilePath);

        public override string? Name => Path.GetFileNameWithoutExtension(_archiveFilePath);

        protected readonly string _archiveFilePath;
        protected readonly DirectoryInfo _tempDir;

        private bool _extracted = false;
        private readonly object _externalLock;
        private readonly object _dirLock = new();

        protected ArchiveFileSource(string path, object lockObject) {
            _archiveFilePath = path;
            _tempDir = new DirectoryInfo(Path.Combine(Path.GetTempPath(), $"csm_{Guid.NewGuid()}"));
            if (!_tempDir.Exists) {
                _tempDir.Create();
            }
            _externalLock = lockObject;
        }

        protected override void Dispose(bool disposing) {
            lock (_dirLock) {
                if (_tempDir.Exists) {
                    Console.WriteLine("Deleting {0}", _tempDir.FullName);
                    lock (_dirLock) {
                        _tempDir.Delete(true);
                    }
                    Console.WriteLine("Deleted {0}", _tempDir.FullName);
                }
                base.Dispose(disposing);
            }
        }

        public override async Task<IEnumerable<FileInfo>> GetFilesAsync(string? pattern = null) {
            IEnumerable<FileInfo> files = new List<FileInfo>();
            await Task.Run(() => {
                lock (_dirLock) {
                    if (!_extracted) {
                        Extract();
                        _extracted = true;
                    }
                    files = GetFiles(_tempDir, pattern);
                }
            });
            return files;
        }

        protected abstract void Extract();
    }
}
