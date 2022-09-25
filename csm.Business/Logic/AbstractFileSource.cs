using csm.Business.Models;
using SixLabors.ImageSharp;
using System.Text.RegularExpressions;

namespace csm.Business.Logic {

    /// <summary>
    /// Implements most methods of <see cref="IFileSource"/>, but leaves <see cref="IFileSource.Initialize(Action?)"/>,
    /// <see cref="IFileSource.GetFilesAsync(string?)"/> up to implementors.
    /// </summary>
    public abstract class AbstractFileSource : IFileSource {

        /// <summary>
        /// The full path of the source
        /// </summary>
        public abstract string? FullPath { get; }

        public string? ParentDirectoryPath {
            get {
                if (FileExists(FullPath)) {
                    return Path.GetDirectoryName(FullPath);
                }
                if (FullPath != null) {
                    return Directory.GetParent(FullPath)?.FullName;
                }
                return null;
            }
            
        }

        /// <summary>
        /// The name of the source
        /// </summary>
        public abstract string? Name { get; }

        /// <summary>
        /// The total size of all files. KiB or MiB.
        /// </summary>
        public string Size { 
            get {
                var kb = Bytes / 1024f;
                if (Bytes < 1024) {
                    return $"{kb:0.}KiB";
                } 
                return $"{kb / 1024f:0.0}MiB";
            }
        }

        public abstract void Initialize(Action? callback = null);

        public abstract Task<IEnumerable<ImageFile>> GetFilesAsync(string? pattern = null);

        protected long Bytes;

        /// <summary>
        /// Gets the files in a directory recursively, optionally filtered by <paramref name="pattern"/>
        /// </summary>
        /// <param name="directory">The directory to query</param>
        /// <param name="pattern">The file name match pattern. Can use * and ? wildcards, but not regular expressions.</param>
        /// <returns>The files</returns>
        protected IEnumerable<ImageFile> GetFiles(DirectoryInfo directory, string? pattern) {
            var files = directory.EnumerateFiles();
            if (pattern != null) {
                files = files.Where(f => Regex.IsMatch(f.Name, pattern));
            }
            return files.Select(f => new ImageFile(f))
                .Concat(directory.GetDirectories().SelectMany(d => GetFiles(d, pattern)));
        }

        /// <summary>
        /// Initialize an <see cref="ImageData"/> instance by retrieving its dimensions from a file stream
        /// </summary>
        /// <param name="image">The <see cref="ImageData"/> to initialize</param>
        public void LoadImageDimensions(ImageData image) {
            using var stream = new FileStream(image.File, FileMode.Open, FileAccess.Read);
            using var fromStream = Image.Load(stream);
            image.InitSize(new Size(fromStream.Width, fromStream.Height));
        }

        /// <summary>
        /// Determines whether a file exists
        /// </summary>
        /// <param name="path">The path of the file</param>
        /// <returns>True if the file exists</returns>
        public bool FileExists(string? path) {
            return File.Exists(path);
        }

        /// <summary>
        /// Gets an <see cref="ImageFile"/> with the given path
        /// </summary>
        /// <param name="path">The path of the file</param>
        /// <returns>The <see cref="ImageFile"/> at the path, if it exists</returns>
        public ImageFile? GetFile(string? path) {
            if (path == null || !FileExists(path)) {
                return null;
            }
            return new ImageFile(new FileInfo(path));
        }

        #region IDisposable

        /// <summary>
        /// Disposes this instance
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This method has to be overridden by implementors
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            // Don't do anything by default
        }

        #endregion
    }
}
