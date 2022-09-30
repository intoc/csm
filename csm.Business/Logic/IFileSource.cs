using csm.Business.Models;

namespace csm.Business.Logic {

    public delegate void FileLoadProgressEventHandler(ProgressEventArgs args);

    public interface IFileSource : IDisposable {

        event FileLoadProgressEventHandler LoadProgressChanged;

        string FullPath { get; }

        string ParentDirectoryPath { get; }

        string ImageFileDirectoryPath { get;  }

        string Name { get; }

        string Size { get; }

        Task<IEnumerable<ImageFile>> GetFilesAsync(string? pattern = null);

        Task Initialize(Action? callback = null);

        void LoadImageDimensions(ImageData image);

        bool FileExists(string? path);

        ImageFile? GetFile(string? path);

    }
}
