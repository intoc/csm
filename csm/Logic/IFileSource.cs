namespace csm.Logic {
    public interface IFileSource : IDisposable {

        string? FullPath { get; }

        string? Name { get; }

        Task<IEnumerable<FileInfo>> GetFilesAsync(string? pattern = null);

        void Initialize(Action? callback = null);

    }
}
