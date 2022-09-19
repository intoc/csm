using csm.Models;
using System.Diagnostics;

namespace csm.Logic {
    internal class ImageSet : IImageSet {

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
            if (!(_imageSource?.IsReady ?? false)) {
                return;
            }
            await Task.Run(() => {
                lock (Images) {
                    var sw = Stopwatch.StartNew();

                    var getFilesTask = _imageSource.GetFilesAsync();
                    getFilesTask.Wait();
                    var allFiles = getFilesTask.Result;

                    // Get a list of all the images in the source
                    // Don't include hidden files
                    IEnumerable<string> files =
                        from file in allFiles
                        where file.Extension.ToLower().Contains(fileType.ToLower())
                        where (file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden
                        select file.FullName;

                    // Load Image data into list
                    _images.Clear();

                    IList<Task> tasks = new List<Task>();
                    foreach (string path in files) {
                        ImageData image = new(path);
                        _images.Add(image);
                        tasks.Add(Task.Factory.StartNew(() => LoadImageDataFromStream(image)));
                    }

                    Task.WaitAll(tasks.ToArray());

                    sw.Stop();
                    Debug.WriteLine("ImageSet.LoadImageListAsync took {0}", sw.Elapsed);

                    RefreshImageList(minDim, outFileName, coverFileName);
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
        /// Initialize an <see cref="ImageData"/> instance by retrieving its dimensions from a file stream
        /// </summary>
        /// <param name="image">The <see cref="ImageData"/> to initialize</param>
        private static void LoadImageDataFromStream(ImageData image) {
            using var stream = new FileStream(image.File, FileMode.Open, FileAccess.Read);
            using var fromStream = Image.FromStream(stream, false, false);
            image.InitSize(new Size(fromStream.Width, fromStream.Height));
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
            string pathWithoutSuffix = image.FileName.Replace(@"(_\d*)?\.jpg", ".jpg");
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
