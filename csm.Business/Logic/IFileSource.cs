using csm.Business.Models;

namespace csm.Business.Logic {
    public interface IFileSource : IDisposable {

        string? FullPath { get; }

        string? ParentDirectoryPath { get; }

        string? Name { get; }

        string Size { get; }

        Task<IEnumerable<ImageFile>> GetFilesAsync(string? pattern = null);

        void Initialize(Action? callback = null);

        void LoadImageDimensions(ImageData image);

        bool FileExists(string? path);

        ImageFile? GetFile(string? path);

    }
}
