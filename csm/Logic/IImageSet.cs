using csm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csm.Logic {

    internal interface IImageSet {

        public IList<ImageData> Images { get; }

        public Task LoadImageListAsync(string fileType, int minDim, string? outFileName, string? coverFileName);

        public void RefreshImageList(int minDim, string? outFileName, string? coverFileName);

    }
}
