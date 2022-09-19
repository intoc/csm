using System.Diagnostics;
using System.IO.Compression;

namespace csm.Logic {
    internal sealed class ZipFileSource : IFileSource {
        public bool IsReady => File.Exists(_zipFilePath);

        public string? FullPath => Path.GetFullPath(_zipFilePath);

        public string? Name => Path.GetFileNameWithoutExtension(_zipFilePath);

        private readonly string _zipFilePath;
        private readonly DirectoryInfo _tempDir;
        private bool _extracted = false;

        private readonly object _externalLock;
        private readonly object _dirLock = new();

        public ZipFileSource(string path, object lockObject) {
            _zipFilePath = path;
            _tempDir = new DirectoryInfo(Path.Combine(Path.GetTempPath(), $"csm_{Guid.NewGuid()}"));
            if (!_tempDir.Exists) {
                _tempDir.Create();
            }
            _externalLock = lockObject;
        }

        public void Dispose() {
            lock (_dirLock) {
                if (_tempDir.Exists) {
                    Debug.WriteLine("Deleting {0}", _tempDir.FullName);
                    lock (_dirLock) {
                        _tempDir.Delete(true);
                    }
                    Debug.WriteLine("Deleted {0}", _tempDir.FullName);
                }
            }
        }

        public async Task<IEnumerable<FileInfo>> GetFilesAsync(string? pattern = null) {
            await Task.Run(() => {
                lock (_dirLock) {
                    Extract();
                }
            });
            return await GetFilesAsync(_tempDir, pattern);
        }

        private async Task<IEnumerable<FileInfo>> GetFilesAsync(DirectoryInfo directory, string? pattern) {
            IEnumerable<FileInfo> files;
            if (pattern != null) {
                files = await Task.Run(() => directory.EnumerateFiles(pattern));
            } else {
                files = await Task.Run(() => directory.EnumerateFiles());
            }
            foreach (var sub in directory.GetDirectories()) {
                files = files.Concat(await GetFilesAsync(sub, pattern));
            }
            return files;
        }

        private void Extract() {
            if (_extracted) {
                // Extraction may have happened while we were waiting for the lock
                return;
            }
            Console.WriteLine("ZipFileSource - Extracting zip file to {0}", _tempDir.FullName);
            ZipFile.ExtractToDirectory(_zipFilePath, _tempDir.FullName, true);
            _extracted = true;
            Console.WriteLine("ZipFileSource - Extraction complete");
        }
    }
}
