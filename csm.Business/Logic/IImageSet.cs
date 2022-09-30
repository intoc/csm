using csm.Business.Models;

namespace csm.Business.Logic;
internal interface IImageSet : IDisposable {

    event Action<ProgressEventArgs> LoadProgressChanged;
    event Action<IFileSource> LoadCompleted;

    bool Loaded { get; }

    IList<ImageData> Images { get; }

    IFileSource Source { get; }

    Task SetSource(IFileSource source);

    Task<bool> LoadImageListAsync(string fileRegex, int minDim, string? outFileName, string? coverFileName);

    void RefreshImageList(int minDim, string? outFileName, string? coverFileName);

    Task<bool> GuessFile(FileParam param, string listPattern, string pattern, bool force = false);

}