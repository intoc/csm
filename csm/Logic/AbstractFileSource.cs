namespace csm.Logic {

    public abstract class AbstractFileSource : IFileSource {
        public abstract string? FullPath { get; }

        public abstract string? Name { get; }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            // Don't do anything by default
        }

        public abstract void Initialize(Action? callback = null);

        public abstract Task<IEnumerable<FileInfo>> GetFilesAsync(string? pattern = null);

        protected IEnumerable<FileInfo> GetFiles(DirectoryInfo directory, string? pattern) {
            IEnumerable<FileInfo> files;
            if (pattern != null) {
                files = directory.EnumerateFiles(pattern);
            } else {
                files = directory.EnumerateFiles();
            }
            foreach (var sub in directory.GetDirectories()) {
                files = files.Concat(GetFiles(sub, pattern));
            }
            return files;
        }
    }
}
