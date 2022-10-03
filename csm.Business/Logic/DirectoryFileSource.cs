using csm.Business.Models;
using Serilog;

namespace csm.Business.Logic {
    public class DirectoryFileSource : AbstractFileSource {

        private readonly DirectoryInfo _directory;

        public DirectoryFileSource(ILogger logger) : base(logger.ForContext("Context", @".\")) {
            _directory = new DirectoryInfo(@".\");
        }

        public DirectoryFileSource(string path, ILogger logger) : base(logger.ForContext("Context", Path.GetDirectoryName(path))) {
            _directory = new DirectoryInfo(path);
            // Get total size of all files in the directory and subdirectories
            var files = GetFiles(_directory);
            Bytes = files.Sum(f => f.Bytes);
        }

        public override string FullPath => _directory.FullName;

        public override string ImageFileDirectoryPath => _directory.FullName;

        public override string Name => _directory.Name;

        public override async Task Initialize(Action? callback = null) {
            callback?.Invoke();
            await Task.Run(() => UpdateProgress(new ProgressEventArgs(1, 1, TimeSpan.Zero, FullPath)));
        }

        public override async Task<IEnumerable<ImageFile>> GetFilesAsync(string? pattern = null) {
            if (_directory == null) {
                return Enumerable.Empty<ImageFile>();
            }
            return await Task.Run(() => GetFiles(_directory, pattern));
        }
    }
}
