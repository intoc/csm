using csm.Business.Models;
using Serilog;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace csm.Business.Logic {
    public sealed class ImageSet : IImageSet {

        public IList<ImageData> Images => _images;

        public IFileSource Source {
            get => _imageSource;
        }

        public bool Loaded { get; private set; }

        private IFileSource _imageSource;
        private ILogger _logger;
        private readonly IList<ImageData> _images = new List<ImageData>();

        public ImageSet(IFileSource fileSource, ILogger logger) {
            _imageSource = AbstractFileSource.Empty;
            _logger = logger.ForContext("Context", fileSource.Name);
            Task.Run(async () => await SetSource(fileSource));
        }

        public event Action<ProgressEventArgs> LoadProgressChanged = delegate { };
        public event Action<IFileSource> LoadCompleted = delegate { };

        public async Task SetSource(IFileSource source) {
            if (source == _imageSource || source.FullPath == _imageSource.FullPath) {
                // It's the same source, we don't need the new one
                source.Dispose();
                return;
            }
            _logger = _logger.ForContext("Context", source.Name);
            _imageSource.Dispose();
            _imageSource = source;
            _imageSource.LoadProgressChanged += (e) => {
                LoadProgressChanged.Invoke(e);
            };
            await _imageSource.Initialize(() => LoadCompleted?.Invoke(_imageSource));
        }

        /// <summary>
        /// Load the file list and image information from the source directory if it's set
        /// </summary>
        /// <param name="fileRegex">The regex to filter file names</param>
        /// <param name="minDim">The minimum dimension (height and width) of images to include</param>
        /// <param name="outFileName">Output file name to ignore</param>
        /// <param name="coverFileName">Cover file name to ignore</param>
        public async Task<bool> LoadImageListAsync(string fileRegex, int minDim, string? outFileName, string? coverFileName) {
            var originalCount = Images.Count;
            await Task.Run(async () => {
                if (_imageSource == null) {
                    return;
                }
                var sw = Stopwatch.StartNew();

                try {
                    var getFilesTask = _imageSource.GetFilesAsync(fileRegex);
                    getFilesTask.Wait();
                    var allFiles = getFilesTask.Result;

                    // Get a list of all the images in the source
                    // Don't include hidden files
                    IEnumerable<string> files =
                        from file in allFiles
                        where !file.Hidden
                        select file.Path;
                    // Load Image data into list
                    _images.Clear();
                    IList<ImageData> unsorted = new List<ImageData>();
                    IList<Task> tasks = new List<Task>();
                    foreach (string path in files) {
                        ImageData image = new(path);
                        tasks.Add(Task.Run(() => {
                            try {
                                _imageSource.LoadImageDimensions(image);
                            } catch (Exception ex) {
                                _logger.Error("Failed to load dimensions for {0}: {1}", image.FileName, ex.Message);
                            }
                            unsorted.Add(image);
                        }));
                    }
                    await Task.WhenAll(tasks);
                    foreach (var i in unsorted.OrderBy(i => i.File)) {
                        _images.Add(i);
                    }
                    sw.Stop();
                } catch (RegexParseException ex) {
                    _logger.Error("Error occurred during file name pattern matching: {0}", ex.Message);
                } catch (Exception ex) {
                    _logger.Error(ex, "Error occurred while loading file list.");
                }
                RefreshImageList(minDim, outFileName, coverFileName);
            });
            return Images.Count != originalCount;
        }

        /// <summary>
        /// Refreshes the dimensions and default inclusion of images in the list
        /// </summary>
        /// <param name="minDim">The minimum dimension (height and width) of images to include</param>
        /// <param name="outFileName">Output file name to ignore</param>
        /// <param name="coverFileName">Cover file name to ignore</param>
        public void RefreshImageList(int minDim, string? outFileName, string? coverFileName) {
            lock (Images) {
                foreach (ImageData image in _images) {
                    if (!image.InclusionPinned) {
                        image.Include = !(IsImageTooSmall(image, minDim) || IsOldSheet(image, outFileName) || IsCover(image, coverFileName));
                    }
                }
                Loaded = true;
            }
        }

        /// <summary>
        /// Attempts to match a file in the file source using a regular expression
        /// </summary>
        /// <param name="param">The <see cref="FileParam"/> that is being guessed</param>
        /// <param name="pattern">The regular expression</param>
        /// <returns>If a match was found</returns>
        public async Task<bool> GuessFile(FileParam param, string listPattern, string pattern, bool force = false) {

            if (_imageSource == null) {
                return false;
            }

            if (!force && param.File != null) {
                return false;
            }

            string? origPath = param.Path;
            bool changed;
            _logger.Debug("Guessing {0} (force={1}) using match pattern: {2}", param.Desc, force, pattern);
            try {
                var files = (await _imageSource.GetFilesAsync(pattern)).ToList();
                ImageFile? match = files.FirstOrDefault();
                if (match != null) {
                    changed = origPath != match.Path;
                    if (changed) {
                        param.Path = match.Path;
                        _logger.Information("Matched {0} on {1}", param.Desc, param.FileName);
                    } else {
                        _logger.Debug("Matched on the same cover file as before");
                    }
                    return changed;
                }
                _logger.Information("No match found for {0}, using first file in the directory.", param.Desc);
                var allFiles = await _imageSource.GetFilesAsync(listPattern);
                param.Path = allFiles.FirstOrDefault()?.Path;
            } catch (RegexParseException ex) {
                _logger.Error("Error occurred during cover file pattern matching: {0}", ex.Message);
            } catch (Exception ex) {
                _logger.Error(ex, "Error occurred while guessing cover.");
            }

            return origPath != param.Path;
        }

        /// <summary>
        /// Don't include images smaller than minDim
        /// </summary>
        /// <param name="image">The image</param>
        /// <param name="minDim">The minimum dimension (height and width) of images to include</param>
        /// <returns>True if the image is too small</returns>
        private static bool IsImageTooSmall(ImageData image, int minDim) => image.Width < minDim && image.Height < minDim;

        /// <summary>
        /// Don't include a previously generated contact sheet if we can avoid it
        /// </summary>
        /// <param name="image">The image</param>
        /// <param name="outFileName">Output file name to ignore</param>
        /// <returns>True if the image is an old output file</returns>
        private static bool IsOldSheet(ImageData image, string? outFileName) {
            if (outFileName == null) {
                return false;
            }
            string pathWithoutSuffix = Regex.Replace(image.FileName, @"(_\d*)?\.([^\.]*)$", ".$2");
            return pathWithoutSuffix.Equals(outFileName);
        }

        /// <summary>
        /// Don't include cover file
        /// </summary>
        /// <param name="image">The image</param>
        /// <param name="coverFileName">The cover file name</param>
        /// <returns>True if the image is the cover file</returns>
        private static bool IsCover(ImageData image, string? coverFileName) {
            if (coverFileName == null) {
                return false;
            }
            return image.FileName.Equals(coverFileName);
        }

        public void Dispose() {
            _imageSource.Dispose();
        }
    }
}
