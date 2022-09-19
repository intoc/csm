namespace csm.Logic {
    public class DirectoryFileSource : AbstractFileSource {

        private readonly DirectoryInfo? _directory;

        public DirectoryFileSource(string? path = null) {
            if (path != null) {
                _directory = new DirectoryInfo(path);
            }
        }

        public override string? FullPath => _directory?.FullName;

        public override string? Name => _directory?.Name;

        public override async Task Initialize(Action callback) {
            await Task.Run(callback);
        }

        public override async Task<IEnumerable<FileInfo>> GetFilesAsync(string? pattern = null) {
            if (_directory == null) {
                return Enumerable.Empty<FileInfo>();
            }
            return await Task.Run(() => GetFiles(_directory, pattern));
        }
    }
}
