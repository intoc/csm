namespace csm.Logic {
    public sealed class DirectoryFileSource : IFileSource {

        private readonly DirectoryInfo? _directory;

        public DirectoryFileSource(string? path = null) {
            if (path != null) {
                _directory = new DirectoryInfo(path);
            }
        }

        public bool IsReady => _directory != null;

        public string? FullPath => _directory?.FullName;

        public string? Name => _directory?.Name;

        public void Dispose() {
            // Nothing to dispose
        }

        public async Task<IEnumerable<FileInfo>> GetFilesAsync(string? pattern = null) {
            if (_directory == null) {
                return Enumerable.Empty<FileInfo>();
            }
            IEnumerable<FileInfo> files;
            if (pattern == null) {
                files = await Task.Run(() => _directory.EnumerateFiles());
            } else {
                files = await Task.Run(() => _directory.EnumerateFiles(pattern));
            }
            return files;
        }
    }
}
