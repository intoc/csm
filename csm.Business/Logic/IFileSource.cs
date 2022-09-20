﻿using csm.Business.Models;

namespace csm.Business.Logic {
    public interface IFileSource : IDisposable {

        string? FullPath { get; }

        string? Name { get; }

        Task<IEnumerable<ImageFile>> GetFilesAsync(string? pattern = null);

        void Initialize(Action? callback = null);

        void LoadImageDimensions(ImageData image);

    }
}