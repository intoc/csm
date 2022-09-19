using csm.Models;

namespace csm.Logic {

    internal interface IImageSet {

        public bool Loaded { get; }

        public IList<ImageData> Images { get; }

        public IFileSource? Source { get; set; }

        public Task LoadImageListAsync(string fileType, int minDim, string? outFileName, string? coverFileName);

        public void RefreshImageList(int minDim, string? outFileName, string? coverFileName);

    }
}
