using csm.Business.Models;

namespace csm.Business.Logic {
    public interface IFileSource : IDisposable {

        string? FullPath { get; }

        string? ParentDirectoryPath { get; }

        string? Name { get; }

        Task<IEnumerable<ImageFile>> GetFilesAsync(string? pattern = null);

        void Initialize(Action? callback = null);

        void LoadImageDimensions(ImageData image);

        bool FileExists(string? path);

        string CombinePaths(string path1, string path2);

        ImageFile? GetFile(string? path);

    }
}
