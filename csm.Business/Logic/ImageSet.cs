using csm.Business.Models;
using Serilog;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace csm.Business.Logic {
    public class ImageSet : IImageSet {

        public IList<ImageData> Images => _images;

        public IFileSource? Source {
            get => _imageSource;
            set {
                if (_imageSource != null) {
                    _imageSource.Dispose();
                }
                _imageSource = value;
            }
        }

        public bool Loaded { get; private set; }

        private IFileSource? _imageSource;

        private readonly IList<ImageData> _images = new List<ImageData>();

        public ImageSet(IFileSource? fileSource = null) {
            _imageSource = fileSource;
        }

        /// <summary>
        /// Load the file list and image information from the source directory if it's set
        /// </summary>
        /// <param name="fileType">The image file extension to filter on</param>
        /// <param name="minDim">The minimum dimension (height and width) of images to include</param>
        /// <param name="outFileName">Output file name to ignore</param>
        /// <param name="coverFileName">Cover file name to ignore</param>
        public async Task LoadImageListAsync(string fileType, int minDim, string? outFileName, string? coverFileName) {
            await Task.Run(() => {
                lock (Images) {
                    if (_imageSource == null) {
                        return;
                    }
                    var sw = Stopwatch.StartNew();

                    var getFilesTask = _imageSource.GetFilesAsync($"*{fileType}");
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

                    IList<Task> tasks = new List<Task>();
                    foreach (string path in files) {
                        ImageData image = new(path);
                        _images.Add(image);
                        tasks.Add(Task.Run(() => _imageSource.LoadImageDimensions(image)));
                    }

                    Task.WaitAll(tasks.ToArray());

                    sw.Stop();
                    Log.Debug("{0}.{1} took {2}", GetType().Name, "LoadImageListAsync", sw.Elapsed);

                    RefreshImageList(minDim, outFileName, coverFileName);
                    Loaded = true;
                }
            });
        }

        /// <summary>
        /// Refreshes the dimensions and default inclusion of images in the list
        /// </summary>
        /// <param name="minDim">The minimum dimension (height and width) of images to include</param>
        /// <param name="outFileName">Output file name to ignore</param>
        /// <param name="coverFileName">Cover file name to ignore</param>
        public void RefreshImageList(int minDim, string? outFileName, string? coverFileName) {
            foreach (ImageData image in _images) {
                if (!image.InclusionPinned) {
                    image.Include = !(IsImageTooSmall(image, minDim) || IsOldSheet(image, outFileName) || IsCover(image, coverFileName));
                }
            }
        }

        /// <summary>
        /// Attempts to match a file in the file source using a regular expression
        /// </summary>
        /// <param name="param">The <see cref="FileParam"/> that is being guessed</param>
        /// <param name="pattern">The regular expression</param>
        /// <returns>If a match was found</returns>
        public async Task<bool> GuessFile(FileParam param, string? fileType, string pattern, bool force = false) {

            if (_imageSource == null || string.IsNullOrEmpty(fileType)) {
                return false;
            }
            
            // If the command line set the cover file pattern/name,
            // make sure it exists. If not, guess.
            if (!(force || param.File == null)) {
                return false;
            }

            string? origPath = param.Path;
            bool changed = false;
            Log.Information("Guessing {0} using match pattern: {1}", param.Desc, pattern);
            var files = (await _imageSource.GetFilesAsync($"*{fileType}")).ToList();
            try {
                var regex = new Regex(pattern);
                ImageFile? match = files.FirstOrDefault(f => regex.IsMatch(f.Path));
                if (match != null) {
                    changed = origPath != match.Path;
                    if (changed) {
                        param.Path = match.Path;
                        Log.Information("Matched {0} on {1}", param.Desc, param.Path);
                    } else {
                        Log.Information("Matched on the same cover file as before");
                    }
                    return changed;
                }
            } catch (RegexParseException ex) {
                Log.Error("Error occurred during cover file pattern matching: {0}", ex.Message);
            } catch (Exception ex) {
                Log.Error(ex, "Error occurred while guessing cover.");
            }
            if (files.Any()) {
                Log.Information("No match found for {0}, using first file in the directory.", param.Desc);
                param.Path = files.First().Path;
            }
            changed = origPath != param.Path;
            
            return changed;
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
            string pathWithoutSuffix = Regex.Replace(image.FileName, @"(_\d*)?\.jpg", ".jpg");
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
    }
}
