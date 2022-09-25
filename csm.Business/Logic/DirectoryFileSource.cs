using csm.Business.Models;

namespace csm.Business.Logic {
    public class DirectoryFileSource : AbstractFileSource {

        private readonly DirectoryInfo? _directory;

        public DirectoryFileSource(string? path = null) : base() {
            if (path != null) {
                _directory = new DirectoryInfo(path);
                // Get total size of all files in the directory and subdirectories
                var files = GetFiles(_directory);
                Bytes = files.Sum(f => f.Bytes);
            }
        }

        public override string? FullPath => _directory?.FullName;

        public override string? Name => _directory?.Name;

        public override void Initialize(Action? callback = null) {
            callback?.Invoke();
        }

        public override async Task<IEnumerable<ImageFile>> GetFilesAsync(string? pattern = null) {
            if (_directory == null) {
                return Enumerable.Empty<ImageFile>();
            }
            return await Task.Run(() => GetFiles(_directory, pattern));
        }
    }
}
