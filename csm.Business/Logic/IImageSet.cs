using csm.Business.Models;

namespace csm.Business.Logic; 
internal interface IImageSet {

    bool Loaded { get; }

    IList<ImageData> Images { get; }

    IFileSource Source { get; set; }

    Task<bool> LoadImageListAsync(string fileRegex, int minDim, string? outFileName, string? coverFileName);

    void RefreshImageList(int minDim, string? outFileName, string? coverFileName);

    Task<bool> GuessFile(FileParam param, string listPattern, string pattern, bool force = false);

}