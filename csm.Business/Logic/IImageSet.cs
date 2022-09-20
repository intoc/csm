using csm.Business.Models;

namespace csm.Business.Logic; 
internal interface IImageSet {

    bool Loaded { get; }

    IList<ImageData> Images { get; }

    IFileSource? Source { get; set; }

    Task LoadImageListAsync(string fileType, int minDim, string? outFileName, string? coverFileName);

    void RefreshImageList(int minDim, string? outFileName, string? coverFileName);

    Task<bool> GuessFile(FileParam param, string? fileType, string pattern, bool force = false);

}