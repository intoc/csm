namespace csm.Logic {
    public interface IFileSource : IDisposable {

        bool IsReady { get; }

        string? FullPath { get; }

        string? Name { get; }

        Task<IEnumerable<FileInfo>> GetFilesAsync(string? pattern = null);

    }
}
