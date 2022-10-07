﻿using csm.Business.Models;
using Serilog;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace csm.Business.Logic {

    internal sealed class SheetBuilder : IDisposable {

        private readonly IImageSet ImageSet;

        #region Public Sheet Parameters

        public int SheetWidth { get; set; }
        public int SheetHeight { get; set; }
        public int MaxImagesPerRow { get; set; }
        public int MinThumbDim { get; set; }
        public bool DrawCover { get; set; }
        public int MaxCoverWidthPercent { get; set; }
        public bool FillGap { get; set; }
        public bool DrawHeader { get; set; }
        public string HeaderTitle { get; set; } = string.Empty;
        public bool IsHeaderTitleBold { get; set; }
        public int HeaderTitleFontSize { get; set; }
        public bool DrawHeaderStats { get; set; }
        public int BorderWidth { get; set; }
        public bool DrawLabels { get; set; }
        public int LabelFontSize { get; set; }
        public bool PreviewOnly { get; set; }
        public FontFamily FontFamily { get; set; }

        public float ShiftFactor { get; set; } = 0.85f; // TODO: AppSetting?

        #endregion

        #region Public Output Parameters

        public ImageFile? CoverFile;
        public List<List<ImageData>> RowLayout = new();

        #endregion

        #region Events

        public event Action<string, bool> ErrorOccurred = delegate { };
        public event Action<ProgressEventArgs> DrawProgressChanged = delegate { };

        #endregion

        #region Fields

        private readonly object _graphicsLock = new();
        private readonly ILogger _logger;
        private bool _isDisposed;
        private int _drawnCount;
        private int _progressStep;
        private Image? _headerImage;
        private ImageData? _coverImageData;

        #endregion

        public SheetBuilder(IImageSet images, ILogger logger) {
            ImageSet = images;
            _logger = logger;
        }

        /// <summary>
        /// Analyzes the image set based on all of the parameters and prepares
        /// a layout
        /// </summary>
        /// <returns>If the layout was prepared successfully</returns>
        public bool BuildLayout() {

            // Determine the images that are actually being requested to draw
            var images = ImageSet.Images.Where(i => i.Include);
            var imageCount = images.Count();

            if (imageCount == 0) {
                ErrorOccurred?.Invoke($"No valid/selected Images in {ImageSet.Source.Name}!", true);
                return false;
            }

            int rowIndex = 0;
            int maxRowHeight = 0;
            int rowHeight;
            RowLayout.Clear();
            RowLayout.Add(new List<ImageData>());

            #region Cover Setup

            // Avoid stupidness
            if (DrawCover && CoverFile == null) {
                ErrorOccurred?.Invoke("Can't draw the cover because there is no cover file set.", false);
                DrawCover = false;
                FillGap = false;
            }

            // Analyze the cover
            if (DrawCover && ImageSet.Source != null && CoverFile != null) {
                // Begin image analysis
                _logger.Debug("Analyzing cover...");
                _coverImageData = new ImageData(CoverFile.Path);
                ImageSet.Source.LoadImageDimensions(_coverImageData);
                double maxCoverImageScaleForGap = Math.Round((MaxCoverWidthPercent / 100f) * MaxImagesPerRow) / MaxImagesPerRow;

                if (_coverImageData.Width >= (SheetWidth * maxCoverImageScaleForGap)) {
                    // We want a gap right? Make the cover smaller.
                    _logger.Information("Cover image is too large. Reducing size to {0:0.00}% of sheet width.", maxCoverImageScaleForGap * 100);
                    double scaleFactor = (SheetWidth * maxCoverImageScaleForGap) / _coverImageData.Width;
                    _coverImageData.Scale(scaleFactor);
                }
                if (_coverImageData.Width < SheetWidth && !FillGap) {
                    // Center images smaller than the sheet width
                    _logger.Information("Centering cover image.");
                    _coverImageData.X = (SheetWidth - _coverImageData.Width) / 2;
                }
                _logger.Debug("Cover analysis complete. Fill gap: {0}, cover bounds: {1}", FillGap, _coverImageData.Bounds);
            }

            #endregion

            #region Analysis Pass 1 - Build initial rows scaled to width

            // Begin image analysis
            _logger.Debug("Pass 1: Analyzing {0} images", imageCount);

            var maxWidth = images.Max(i => i.Width);

            // First pass, add the same number of images to each row,
            // scale to width, record maximum row height
            foreach (ImageData data in images) {
                // Add image to row
                RowLayout[rowIndex].Add(data);

                // Check for any images that didn't get dimensions during the initial loading process
                // We'll show a placeholder later if it still fails to load
                if (data.Width == 0) {
                    data.InitSize(new Size(maxWidth, maxWidth));
                }

                if (RowLayout[rowIndex].Count == MaxImagesPerRow) {

                    // Scale the row to fit the sheetwidth
                    rowHeight = ScaleRow(RowLayout[rowIndex], SheetWidth);

                    // Record max row height (scaled)
                    maxRowHeight = Math.Max(maxRowHeight, rowHeight);

                    ++rowIndex;
                    RowLayout.Add(new List<ImageData>());
                }
            }
            if (RowLayout.Count == 1) {
                maxRowHeight = ScaleRow(RowLayout.Single(), SheetWidth);
            }

            _logger.Debug("Added {0} rows, maxRowHeight: {1}", RowLayout.Count, maxRowHeight);

            #endregion

            #region Analysis Pass 2 - Scale and Shift

            _logger.Debug("Pass 2: Analyzing {0} Rows to normalize row height", RowLayout.Count);

            // Second pass tries to make all rows of similar height by
            // shifting images and rescaling rows.
            Size minRowDims;
            int rowWidth;
            Point curPoint = new(0, 0);
            bool inGap = FillGap;



            for (rowIndex = 0; rowIndex < RowLayout.Count; ++rowIndex) {

                if (inGap && _coverImageData != null) {
                    // Row space = cover gap
                    rowWidth = SheetWidth - _coverImageData.Width;
                    curPoint.X = _coverImageData.Width;
                } else {
                    // Row space = sheet width
                    rowWidth = SheetWidth;
                    curPoint.X = 0;
                }

                // Set the first image's location
                // Succeeding row images will follow horizontally
                RowLayout[rowIndex].First().MoveTo(curPoint);

                // Do the scaling/shifting to give the row a similar
                // height to the rest, with each image's dimensions
                // greater than or equal to the minimum dimension param.
                rowHeight = ScaleRow(RowLayout[rowIndex], rowWidth);
                minRowDims = MinDims(RowLayout[rowIndex]);
                while (RowLayout[rowIndex].Count > 1 &&
                        (rowHeight < maxRowHeight * ShiftFactor ||
                         minRowDims.Width < MinThumbDim || minRowDims.Height < MinThumbDim ||
                         RowLayout[rowIndex].Count > MaxImagesPerRow)) {
                    ShiftImage(rowIndex, rowIndex + 1);
                    rowHeight = ScaleRow(RowLayout[rowIndex], rowWidth);
                    minRowDims = MinDims(RowLayout[rowIndex]);
                }

                // Process at the end of the cover gap
                // Or at the end of the imagelist
                int overFlow = curPoint.Y + rowHeight - _coverImageData?.Height ?? 0;
                if (inGap && _coverImageData != null && (overFlow > 0 || rowIndex + 1 == RowLayout.Count || RowLayout[rowIndex + 1].Count == 0)) {
                    if (overFlow > rowHeight / 3) {
                        // This row is too tall to fit in the gap.
                        // Move all images in the row to the next one
                        while (RowLayout[rowIndex].Any()) {
                            ShiftImage(rowIndex, rowIndex + 1);
                        }
                        // Remove this empty row
                        _logger.Debug("Removing row " + rowIndex);
                        RowLayout.Remove(RowLayout[rowIndex]);

                        // Since we removed a row, the next row is now this one.
                        // Make sure to process it
                        --rowIndex;
                    }

                    // If we just moved the first row down, then there's no point in
                    // resizing the gap images (there aren't any).
                    if (rowIndex >= 0) {

                        // Scale the cover and the gap images so they are the same height
                        double h1 = _coverImageData.Height;
                        double w1 = _coverImageData.Width;
                        double h2 = RowLayout[rowIndex].First().Bottom;
                        double w2 = rowWidth;

                        double f1 = h2 * SheetWidth / (h1 * w2 + h2 * w1);

                        _coverImageData.Scale(f1);

                        curPoint.Y = 0;
                        for (int i = 0; i <= rowIndex; ++i) {
                            // Move images to the start of the new gap
                            RowLayout[i].First().X = _coverImageData.Width;
                            RowLayout[i].First().Y = curPoint.Y;
                            // Scale row width to the new gap
                            rowHeight = ScaleRow(RowLayout[i], SheetWidth - _coverImageData.Width);
                            _logger.Debug("In Gap, Final Scaling, Row {0}", i);
                            // Next row
                            curPoint.Y += rowHeight;
                        }
                    } else {
                        // No gap images. Display the cover normally.
                        _coverImageData.X = SheetWidth / 2 - _coverImageData.Width / 2;
                        FillGap = false;
                        ErrorOccurred?.Invoke("Cover gap fill failed, image is too small. Centering.", false);
                    }
                    // We're done with the gap
                    inGap = false;
                } else {
                    curPoint.Y += rowHeight;
                }

                if (rowIndex >= 0) {
                    _logger.Debug("Row {0}: {1} Images. Height: {2}px. Y: {3}", rowIndex, RowLayout[rowIndex].Count, rowHeight, RowLayout[rowIndex].First().Y);
                }
            }

            #endregion

            #region Analysis Pass 3 - Remove empty rows

            // Remove empty rows
            _logger.Debug("Pass 3: Removing empty rows");
            for (int i = 0; i < RowLayout.Count; ++i) {
                if (!RowLayout[i].Any()) {
                    _logger.Debug("Removing row {0}", i);
                    RowLayout.RemoveAt(i);
                }
            }

            #endregion

            #region Analysis Pass 4 - Even out rows in reverse

            // Determine if this is a single row sheet (outside of the cover gap)
            bool isSingleRow = RowLayout.Count(r => !RowInGap(r)) == 1;

            // Adjust the last rows to account for distortion
            if (!isSingleRow) {
                _logger.Debug("Pass 4: Even out row heights in reverse to reduce massive images at the end");
                ShiftFromLastRowRecursive(RowLayout.Last(), maxRowHeight, ShiftFactor);
            } else {
                _logger.Debug("Skipping Pass 4 (Single Row Sheet)");
            }

            #endregion

            #region Pass 5 - Remove row gaps and overlaps

            // Make sure rows don't overlap or have gaps between them
            _logger.Debug("Pass 5: Removing row gaps and overlaps");
            curPoint.Y = RowLayout.First().First().Y;
            foreach (List<ImageData> row in RowLayout) {
                int gap = row.First().Y - curPoint.Y;
                if (gap != 0) {
                    _logger.Debug("Removing {0} at row {1}: {2}px", gap > 0 ? "gap" : "overlap", RowIndex(row), gap);
                    foreach (ImageData im in row) {
                        im.Y = curPoint.Y;
                    }
                }
                curPoint.Y = row.First().Bottom;
            }

            #endregion

            #region Build Header Image

            // Draw the header image first since we can't extend the canvas during drawing

            if (DrawHeader) {
                _logger.Debug("Building the header {0} stats", DrawHeaderStats ? "with" : "without");
                _headerImage = new Image<Rgba32>(SheetWidth, SheetWidth);
                int padding = 5;
                int headerWidth = SheetWidth - (padding * 2);
                _logger.Debug("Title{1}: {0}", HeaderTitle, IsHeaderTitleBold ? " (bold)" : string.Empty);

                // Build title font
                Font titleFont = FontFamily.CreateFont(HeaderTitleFontSize, IsHeaderTitleBold ? FontStyle.Bold : FontStyle.Regular);
                TextOptions titleTextOptions = new(titleFont) {
                    WrappingLength = headerWidth,
                    Origin = new(padding, 0)
                };
                FontRectangle headerFontRect = TextMeasurer.Measure(HeaderTitle, titleTextOptions);
                int headerHeight = (int)Math.Ceiling(headerFontRect.Height + padding * 2);

                _headerImage.Mutate(headerImageContext => {

                    // Draw title text
                    headerImageContext
                        .Fill(Color.Black)
                        .DrawText(titleTextOptions, HeaderTitle, Color.White);

                    // Stats
                    if (DrawHeaderStats) {
                        // Build stats font
                        Font statsFont = FontFamily.CreateFont(14, FontStyle.Regular);
                        TextOptions statsTextOptions = new(statsFont) {
                            WrappingLength = headerWidth,
                            Origin = new(padding, headerFontRect.Height + 10)
                        };

                        // Determine largest image
                        var maxSize = RowLayout
                            .Where(x => x.Count > 0)
                            .MaxBy(row => row.Max(img => img.OriginalSize.Height))?
                            .MaxBy(img => img.OriginalSize.Height)?.OriginalSize ?? default;

                        // Determine how much space the stats will take up in the header
                        string stats = $"{imageCount} images. Maximum dimensions {maxSize.Width}x{maxSize.Height}px.";
                        if (ImageSet.Source != null) {
                            stats += $" {ImageSet.Source.Size}";
                        }
                        var statsFontSize = TextMeasurer.Measure(stats, statsTextOptions);
                        int statsHeight = (int)statsFontSize.Height + 10;
                        headerHeight = (int)Math.Ceiling(headerFontRect.Height + statsHeight + padding);
                        _logger.Debug("Header Stats: {0}", stats);

                        // Pre-draw stats
                        int statsTop = (int)headerFontRect.Height + padding;
                        headerImageContext
                            .DrawLines(new Pen(Color.DarkSlateGray, 1), new PointF(padding, statsTop), new PointF(SheetWidth - padding, statsTop))
                            .DrawText(statsTextOptions, stats, Color.White);
                    }

                    // Get rid of extra height
                    headerImageContext.Crop(SheetWidth, headerHeight);
                    _logger.Debug("Header Height: {0}px", headerHeight);
                });
            }

            #endregion

            #region Adjust Y values

            // Determine where we should start drawing images vertically
            int newTop = _headerImage?.Height ?? 0;
            if (newTop > 0) {
                newTop -= BorderWidth;

                // Adjust cover Y position
                if (_coverImageData != null) {
                    _coverImageData.Y = newTop;
                }
            }

            // Adjust image Y positions
            foreach (var image in images) {
                image.Y += newTop;
                if (!FillGap && _coverImageData != null) {
                    image.Y += _coverImageData.Height;
                }
            }

            #endregion

            #region Pass 6 - Calculate Borders

            _logger.Debug("Pass 6: Calculating borders (Width {0}px) and accounting for rounding error", BorderWidth);

            // Calculate row height scale factor
            ImageData last = RowLayout.Last().First();
            SheetHeight = last.Bottom + BorderWidth;
            double vScale = 1.0;
            int borderSum = BorderWidth * (RowLayout.Count + 1);
            double reduceImageHeight = (double)borderSum / (SheetHeight - RowLayout.First().First().Y);
            vScale = 1.0 - reduceImageHeight;
            int top = RowLayout.First().First().Y;

            foreach (List<ImageData> row in RowLayout) {

                var firstRowImage = row.First();
                var lastRowImage = row.Last();

                // Calculate image with scale factor
                borderSum = BorderWidth * (row.Count + 1);
                double reduceImageWidth = (double)borderSum / (SheetWidth - firstRowImage.X);
                double hScale = 1.0 - reduceImageWidth;
                int leftEdge = firstRowImage.X;
                int rightEdge = firstRowImage.X;

                // Scale and shift images to create borders
                foreach (ImageData image in row) {
                    image.Width = (int)Math.Round(image.Width * hScale);
                    image.Height = (int)Math.Round(image.Height * vScale);
                    image.X = rightEdge + BorderWidth;
                    image.Y = top + BorderWidth;
                    rightEdge = image.Right;
                }

                // Correct rounding error Horizontally
                // Calculate the error, shift all images to center, and adjust the left and right edges to align
                int xError = SheetWidth - (lastRowImage.Right + BorderWidth);
                int xCorrection = (int)Math.Round((double)xError / 2);
                foreach (ImageData image in row) {
                    image.X += xCorrection;
                }
                int shift = BorderWidth - (firstRowImage.X - leftEdge);
                firstRowImage.X = leftEdge + BorderWidth;
                firstRowImage.Width -= shift;
                lastRowImage.Width = SheetWidth - lastRowImage.X - BorderWidth;

                top = firstRowImage.Bottom;
            }

            // Correct cover height after thumbnail borders added
            if (_coverImageData != null) {
                _coverImageData.Pad(BorderWidth);
                if (FillGap) {
                    foreach (var row in RowLayout) {
                        int leftEdge = row.First().X;
                        if (leftEdge == BorderWidth) {
                            _coverImageData.Height = (row.First().Y - BorderWidth) - _coverImageData.Y;
                            break;
                        }
                    }
                }
            }

            // Calculate final sheet height
            SheetHeight = last.Bottom + BorderWidth;

            #endregion

            return true;
        }

        /// <summary>
        /// Get whether <paramref name="row"/> is in the cover gap region
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>Whether the row is in the cover gap region</returns>
        private bool RowInGap(List<ImageData> row) => row.First().Y < (_coverImageData?.Height ?? 0);

        /// <summary>
        /// Get the index of <paramref name="row"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>The row index</returns>
        private int RowIndex(List<ImageData> row) => RowLayout.IndexOf(row);

        /// <summary>
        /// Get the row previous to <paramref name="row"/>
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>The previous row</returns>
        private List<ImageData> PrevRow(List<ImageData> row) {
            while (RowIndex(row) > 0 && !RowLayout[RowIndex(row) - 1].Any()) {
                _logger.Debug("Row {0} exhausted. Removing.", RowIndex(row) - 1);
                RowLayout.RemoveAt(RowIndex(row) - 1);
            }
            if (RowIndex(row) == 0 || RowInGap(RowLayout[RowIndex(row) - 1])) {
                return Enumerable.Empty<ImageData>().ToList();
            }
            return RowLayout[RowIndex(row) - 1];
        }

        /// <summary>
        /// Shifts images from the row previous to <paramref name="row"/> until <paramref name="row"/>
        /// has a satisfactory height
        /// </summary>
        /// <param name="row">The row to shift images into</param>
        /// <param name="maxRowHeight">The initial maximum row height</param>
        /// <param name="shiftFactor">The factor to use when calculating how close to <paramref name="maxRowHeight"/> the row height should be</param>
        private void ShiftFromLastRowRecursive(List<ImageData> row, int maxRowHeight, float shiftFactor) {
            // Be sure to scale the row first, it might be the last one
            int height = ScaleRow(row, SheetWidth);
            if (!PrevRow(row).Any()) {
                return;
            }
            bool scaled = false;
            while (PrevRow(row).Any() && height > (maxRowHeight / shiftFactor) && row.Count < MaxImagesPerRow) {
                _logger.Debug("Row {0} Too Tall ({1} at {2}px) Shifting from row {3} ({4} left)", RowIndex(row), row.Count, height, RowIndex(PrevRow(row)), PrevRow(row).Count);
                ShiftImage(PrevRow(row), row);
                row.First().X = 0;
                height = ScaleRow(row, SheetWidth);
                scaled = true;
            }
            if (scaled) {
                _logger.Debug("Row {0} Rescaled ({1} at {2}px)", RowIndex(row), row.Count, height);
                ShiftFromLastRowRecursive(PrevRow(row), maxRowHeight, shiftFactor);
            }
        }

        public async Task<Image> Draw() {
            // Create the output image
            var sheetImage = new Image<Rgba32>(SheetWidth, SheetHeight);

            // Draw the sheet
            sheetImage.Mutate(sheetContext => {

                // We have to make the background white because the header text looks bad on a black background
                sheetContext.Fill(Color.Black);

                // Draw the the header
                if (_headerImage != null) {
                    // Draw the header on the sheet
                    _logger.Debug("Drawing header. {0}", _headerImage.Bounds());
                    sheetContext.DrawImage(_headerImage, 1);
                }

                // Draw the cover
                if (_coverImageData != null && !_isDisposed) {
                    _logger.Debug("Drawing cover {0}. {1}", _coverImageData.FileName, _coverImageData.Bounds);
                    if (PreviewOnly) {
                        TextOptions options = new(FontFamily.CreateFont(24, FontStyle.Bold)) {
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            TextAlignment = TextAlignment.Center,
                            Origin = new PointF(_coverImageData.X + _coverImageData.Width / 2f, _coverImageData.Y + _coverImageData.Height / 2f)
                        };
                        sheetContext
                            .Fill(Brushes.Solid(Color.White), _coverImageData.Bounds)
                            .DrawText(options, "COVER", Color.Black);
                    } else {
                        using var coverImage = Image.Load(_coverImageData.File);
                        coverImage.Mutate(coverContext => {
                            coverContext.Resize(_coverImageData.Width, _coverImageData.Height);
                        });
                        sheetContext.DrawImage(coverImage, _coverImageData.Origin, 1);
                    }
                }
            });

            // Draw the thumbnail images
            int index = 1;
            _logger.Debug("Drawing sheet. {0}", sheetImage.Bounds());
            _drawnCount = 0;
            _progressStep = 0;
            IList<Task> drawThumbTasks = new List<Task>();
            Font labelFont = FontFamily.CreateFont(DrawLabels ? LabelFontSize : 0, FontStyle.Bold);
            var start = DateTime.Now;

            var images = RowLayout.SelectMany(row => row.Select(image => image));
            foreach (ImageData image in images) {
                if (_isDisposed) {
                    break;
                }
                // Create info for threaded load/draw operation
                ThumbnailData tdata = new(image, sheetImage, labelFont) {
                    Index = index++,
                    ImageTotal = images.Count(),
                    StartTime = start
                };

                drawThumbTasks.Add(Task.Factory.StartNew(() => DrawThumb(tdata)));
            }

            if (_isDisposed) {
                return sheetImage;
            }
            await Task.WhenAll(drawThumbTasks);

            if (_isDisposed) {
                sheetImage.Dispose();
            }

            return sheetImage;
        }

        /// <summary>
        /// Draw a thumbnail image on the contact sheet
        /// </summary>
        /// <param name="data">Data about the image and its position on the sheet</param>
        private void DrawThumb(ThumbnailData data) {
            Image image;
            Size size = new(data.Image.Width, data.Image.Height);
            if (PreviewOnly) {
                image = new Image<Rgba32>(size.Width, size.Height);
            } else {
                try {
                    if (_isDisposed) {
                        _logger.Debug("{0}({1}): Disposed, cancelling...", "DrawThumb", data.Image.FileName);
                        return;
                    }
                    image = Image.Load(data.File);
                } catch (Exception) {
                    // Failed to load the image. Draw a placeholder
                    image = new Image<Rgba32>(size.Width, size.Height);
                    image.Mutate(bad => {
                        var pen = Pens.Solid(Color.Red, 4);
                        var rect = new Rectangle(2, 2, size.Width - 4, size.Height - 4);
                        bad.Fill(Color.Black)
                            .DrawLines(pen,
                                new Point(rect.Left, rect.Top),
                                new Point(rect.Right, rect.Bottom),
                                new Point(rect.Right, rect.Top),
                                new Point(rect.Left, rect.Bottom))
                            .Draw(pen, rect);
                    });
                    _logger.Error("{0} Image load failed, drawing placeholder.", data.Image.FileName);
                }
            }

            image.Mutate(imageContext => {

                if (PreviewOnly) {
                    // Make the preview box
                    imageContext.Fill(Brushes.Solid(Color.White))
                        .Draw(Pens.Solid(Color.LightGreen, 1), new Rectangle(Point.Empty, size));
                } else {
                    // Resize the image to thumbnail size
                    imageContext.Resize(size.Width, size.Height, KnownResamplers.Triangle);
                }

                // Draw image name label
                if (data.LabelFont.Size > 0) {

                    // Set label to file name, no extension
                    string label = data.Image.FileNameNoExtension;
                    TextOptions labelOptions = new(data.LabelFont) {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        TextAlignment = TextAlignment.Center,
                        WordBreaking = WordBreaking.BreakAll,
                        WrappingLength = data.Image.Width - data.LabelFont.Size
                    };

                    // Determine label size
                    var labelSize = TextMeasurer.Measure(label, labelOptions);
                    labelSize = labelSize.Inflate(data.LabelFont.Size / 2, 0);
                    if (size.Width - labelSize.Width < 10) {
                        labelSize = new FontRectangle(labelSize.X, labelSize.Y, size.Width, labelSize.Height);
                    }
                    labelOptions.Origin = new PointF(labelSize.Width / 2, 0);

                    // Make the label image
                    using Image labelImage = new Image<Rgba32>((int)Math.Round(labelSize.Width), (int)Math.Round(labelSize.Height));
                    labelImage.Mutate(labelContext => {
                        labelContext.Fill(Color.Black).DrawText(labelOptions, label, Color.White);

                        // Round the corners
                        Size size = labelContext.GetCurrentSize();
                        IPathCollection corners = BuildCorners(size.Width, data.LabelFont.Size * 2 / 3);
                        labelContext.SetGraphicsOptions(new GraphicsOptions() {
                            Antialias = true,
                            AlphaCompositionMode = PixelAlphaCompositionMode.DestOut // enforces that any part of this shape that has color is punched out of the background
                        });

                        // mutating in here as we already have a cloned original
                        // use any color (not Transparent), so the corners will be clipped
                        foreach (var c in corners) {
                            labelContext = labelContext.Fill(Color.Red, c);
                        }
                    });

                    // Draw the label image on the thumbnail. Center at the bottom.
                    Point labelCoords = new((int)(size.Width - labelSize.Width) / 2, (int)(size.Height - labelSize.Height));
                    imageContext.DrawImage(labelImage, labelCoords, 0.5f);
                }
            });

            lock (_graphicsLock) {
                data.SheetImage.Mutate(context => {
                    // Draw the thumbnail
                    context.DrawImage(image, data.Image.Origin, 1.0f);
                });
            }

            // Clean up
            image.Dispose();

            // Update counters
            int resolution = 10;
            Interlocked.Increment(ref _drawnCount);
            double drawProgress = _drawnCount / (double)data.ImageTotal;
            int step = (int)Math.Floor(drawProgress * resolution);

            // Send a limited number of progress updates to the listeners
            if (step > _progressStep) {
                ++_progressStep;
                // Send progress to listeners
                DrawProgressChanged?.Invoke(new ProgressEventArgs(_drawnCount, data.ImageTotal, DateTime.Now - data.StartTime));
            }
            // Output status to console
            _logger.Debug("({0:P1}) {1} ({2}/{3}) {4}",
                drawProgress, data.Image.FileName, data.Index, data.ImageTotal, data.Image.Bounds);
        }

        /// <summary>
        /// Shift a thumbnail image from an end of one row to another on the contact sheet
        /// </summary>
        /// <param name="fromRow">The index of the source row</param>
        /// <param name="toRow">The index of the target row</param>
        private void ShiftImage(int fromRow, int toRow) {
            if (RowLayout.Count <= toRow) {
                RowLayout.Add(new List<ImageData>());
            }
            ShiftImage(RowLayout[fromRow], RowLayout[toRow]);
        }

        /// <summary>
        /// Shift a thumbnail image from an end of one row to another on the contact sheet
        /// </summary>
        /// <param name="fromRow">The source row</param>
        /// <param name="toRow">The target row</param>
        private static void ShiftImage(List<ImageData> fromRow, List<ImageData> toRow) {
            toRow.Insert(0, fromRow.Last());
            fromRow.Remove(fromRow.Last());
        }

        /// <summary>
        /// Scale the images in a row to fit a new row width
        /// </summary>
        /// <param name="list">The list of images in the row</param>
        /// <param name="width">The new row width</param>
        /// <returns>The newly scaled row height</returns>
        private static int ScaleRow(List<ImageData> list, int width) {

            int rowHeight = 0;
            int rowWidth = 0;
            int maxImageHeight = list.Max(img => img.OriginalSize.Height);
            double factor;
            var first = list.First();

            // Scale all images to the maximum image height and determine the row width
            foreach (ImageData data in list) {
                factor = 1.0;
                if (data.OriginalSize.Height < maxImageHeight) {
                    factor = data.ScaleToHeight(maxImageHeight);
                }
                rowWidth += (int)Math.Round(data.OriginalSize.Width * factor);
            }

            // Calculate the row height based on the factor needed to scale the row to the sheet width
            rowHeight = (int)Math.Round(maxImageHeight * (width / (double)rowWidth));

            // Scale images to calculated height
            list.ForEach(img => img.ScaleToHeight(rowHeight));

            if (list.Count > 1) {
                // Set image locations
                var pairs = list.Skip(1).Select((image, index) => (image, prev: list.ElementAt(index)));
                foreach (var (image, prev) in pairs) {
                    image.X = prev.Right;
                    image.Y = first.Y;
                }
            }

            return rowHeight;
        }

        /// <summary>
        /// Get the minimum dimensions of the images in a row
        /// </summary>
        /// <param name="row">The list of images in a row</param>
        /// <returns>A <see cref="Size"/> containing the calculated minimum Height and Width</returns>
        private static Size MinDims(List<ImageData> row) {
            return row.Any() ? new Size(row.Min(img => img.Width), row.Min(img => img.Height)) : new Size();
        }

        /// <summary>
        /// Build a path collection containing cutouts for rounded label corners
        /// </summary>
        /// <param name="imageWidth">The width of the image</param>
        /// <param name="cornerRadius">The radius of the corners</param>
        /// <returns>The path collection</returns>
        private static IPathCollection BuildCorners(int imageWidth, float cornerRadius) {
            var rect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius);
            IPath cornerTopLeft = rect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));
            float rightPos = imageWidth - cornerTopLeft.Bounds.Width + 1;
            IPath cornerTopRight = cornerTopLeft.RotateDegree(90).Translate(rightPos, 0);
            return new PathCollection(cornerTopLeft, cornerTopRight);
        }

        public void Dispose() {
            _isDisposed = true;
        }
    }
}