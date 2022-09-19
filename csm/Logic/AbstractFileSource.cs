using csm.Models;

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

        public abstract Task<IEnumerable<ImageFile>> GetFilesAsync(string? pattern = null);

        protected IEnumerable<ImageFile> GetFiles(DirectoryInfo directory, string? pattern) {
            IEnumerable<FileInfo> fileInfos;
            if (pattern != null) {
                fileInfos = directory.EnumerateFiles(pattern);
            } else {
                fileInfos = directory.EnumerateFiles();
            }
            var files = fileInfos.Select(f => new ImageFile(f.FullName, (f.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden));
            foreach (var sub in directory.GetDirectories()) {
                files = files.Concat(GetFiles(sub, pattern));
            }
            return files;
        }

        /// <summary>
        /// Initialize an <see cref="ImageData"/> instance by retrieving its dimensions from a file stream
        /// </summary>
        /// <param name="image">The <see cref="ImageData"/> to initialize</param>
        public void LoadImageDataFromStream(ImageData image) {
            using var stream = new FileStream(image.File, FileMode.Open, FileAccess.Read);
            using var fromStream = Image.FromStream(stream, false, false);
            image.InitSize(new Size(fromStream.Width, fromStream.Height));
        }
    }
}
