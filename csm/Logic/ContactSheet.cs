using csm.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;

namespace csm.Logic {

    public delegate void SourceDirectoryChangedEventHandler(string path);
    public delegate void DrawProgressEventHandler(DrawProgressEventArgs args);
    public delegate void SettingsChangedEventHandler(SettingsChangedEventArgs args);
    public delegate void ImageListChangedEventHandler(ImageListChangedEventArgs args);
    public delegate void ExceptionEventHandler(Exception e);

    public class ContactSheet {

        private static readonly int DEFAULT_WIDTH = 900;
        private static readonly int DEFAULT_COLUMNS = 6;
        private static readonly int DEFAULT_QUALITY = 90;

        private readonly BoolParam noGui;
        private readonly StringParam fileType;
        private readonly IntParam columns;
        private readonly IntParam sheetWidth;
        private readonly IntParam minDim;
        private readonly IntParam minDimInput;
        private readonly IntParam borders;
        private readonly IntParam quality;
        private readonly BoolParam interpolate;
        private readonly BoolParam labels;
        private readonly IntParam labelFontSize;
        private readonly BoolParam header;
        private readonly IntParam headerFontSize;
        private readonly StringParam headerTitle;
        private readonly BoolParam headerBold;
        private readonly BoolParam headerStats;
        private readonly BoolParam cover;
        private readonly StringParam coverPattern;
        private readonly FileParam coverFile;
        private readonly BoolParam fillCoverGap;
        private readonly BoolParam preview;
        private readonly BoolParam exitOnComplete;
        private readonly BoolParam threading;
        private readonly IntParam maxThreads;
        private readonly StringParam outputFilePath;
        private readonly BoolParam openOutputDirectoryOnComplete;

        public List<Param> Params { get; private set; }

        private int imageCount, drawnCount, activeDrawThreads;
        private DirectoryInfo sourceDir;
        public string SettingsFile { get; set; }

        public List<ImageData> ImageList { get; set; }

        private bool LoadingImageList;
        private readonly string[] coverNames = { "cover", "folder", "square", "front" };
        private static readonly string[] helpStrings = { "--help", "-help", "/?", "-?" };

        public event DrawProgressEventHandler DrawProgressChanged;
        public event SettingsChangedEventHandler SettingsChanged;
        public event ImageListChangedEventHandler ImageListChanged;
        public event ExceptionEventHandler ExceptionOccurred;
        public event SourceDirectoryChangedEventHandler SourceDirectoryChanged;

        readonly bool canAnalyzeRam = false;
        private readonly PerformanceCounter workingSetCounter;
        private readonly PerformanceCounter availableRamCounter;
        private float ramUsageAtStart, drawRam;

        private bool drawThreadsRunning;
        private DateTime startTime;

        public string SourceDirectory {
            get {
                if (sourceDir != null) {
                    return sourceDir.FullName;
                } else {
                    return string.Empty;
                }
            }
            set {
                sourceDir = new DirectoryInfo(value);
                Console.WriteLine($"Setting sourcedir to {sourceDir}");
                headerTitle.ParseVal(sourceDir.Name);
                Console.WriteLine($"Directory Name -> Header Title: {headerTitle.Val}");

                if (cover.Val) {
                    GuessCover(true);
                }

                LoadFileList();

                SourceDirectoryChanged?.Invoke(Path.GetFullPath(value));
            }
        }

        public string OutFilePath {
            get {
                if (Path.IsPathRooted(outputFilePath.Val)) {
                    return outputFilePath.Val;
                }
                return Path.GetFullPath(Path.Combine(SourceDirectory, outputFilePath.Val));
            }
        }

        public bool GuiEnabled => !noGui.Val;

        public bool OpenOutputDir => openOutputDirectoryOnComplete.Val;

        public ContactSheet() {

            noGui = new BoolParam("-nogui", false);

           

            // Set parameter fields and defaults

            fileType = new StringParam("-filetype", ".jpg", "Extension") {
                MaxChars = 4
            };

            minDimInput = new IntParam("-mindiminput", 0, "px");

            columns = new IntParam("-cols", DEFAULT_COLUMNS) {
                MinVal = 1,
                MaxVal = 99999
            };

            sheetWidth = new IntParam("-width", DEFAULT_WIDTH, "px") {
                MinVal = 1,
                MaxVal = 999999
            };

            minDim = new IntParam("-mindim", 0, "px") {
                MinVal = 0,
                MaxVal = sheetWidth.MaxVal
            };

            borders = new IntParam("-border", 0, "px") {
                MinVal = 0,
                MaxVal = 50
            };

            quality = new IntParam("-qual", DEFAULT_QUALITY, "%") {
                MinVal = 0,
                MaxVal = 100
            };

            interpolate = new BoolParam("-interp", true);

            threading = new BoolParam("-thread", false);
            maxThreads = new IntParam("-tmax", 4) {
                MinVal = 1,
                MaxVal = 16
            };
            threading.AddSubParam(maxThreads);
            preview = new BoolParam("-preview", false);
            exitOnComplete = new BoolParam("-exit", false);

            outputFilePath = new StringParam("-outfile", "ContactSheet.jpg", "File Path") {
                MaxChars = 25
            };
            openOutputDirectoryOnComplete = new BoolParam("-openoutdir", true);

            var generalParams = new NullParam("General");
            generalParams.AddSubParam(fileType);
            generalParams.AddSubParam(columns);
            generalParams.AddSubParam(minDimInput);
            generalParams.AddSubParam(sheetWidth);
            generalParams.AddSubParam(columns);
            generalParams.AddSubParam(minDim);
            generalParams.AddSubParam(borders);
            generalParams.AddSubParam(quality);
            generalParams.AddSubParam(interpolate);
            generalParams.AddSubParam(threading);
            generalParams.AddSubParam(exitOnComplete);
            generalParams.AddSubParam(openOutputDirectoryOnComplete);
            generalParams.AddSubParam(preview);
            generalParams.AddSubParam(outputFilePath);

            labels = new BoolParam("-labels", false);
            labelFontSize = new IntParam("-lsize", 8, "pt") {
                MinVal = 0,
                MaxVal = 136
            };
            labels.AddSubParam(labelFontSize);

            header = new BoolParam("-header", false);
            headerFontSize = new IntParam("-hsize", 12, "pt") {
                MinVal = 0,
                MaxVal = 180
            };
            headerBold = new BoolParam("-hbold", false);
            headerTitle = new StringParam("-htitle", "Title", "Words") {
                MaxChars = 20
            };
            headerStats = new BoolParam("-hstats", false);
            header.AddSubParam(headerFontSize);
            header.AddSubParam(headerBold);
            header.AddSubParam(headerTitle);
            header.AddSubParam(headerStats);

            cover = new BoolParam("-cover", false);
            coverPattern = new StringParam("-cregx", string.Empty, "Regex") {
                MaxChars = 20
            };
            coverFile = new FileParam("-cfile", null);
            cover.AddSubParam(coverPattern);
            cover.AddSubParam(coverFile);
            fillCoverGap = new BoolParam("-cfill", false);
            cover.AddSubParam(fillCoverGap);

            // Setup all instances where a file list reload is required
            var reloadFileListHandler = new ParamChangedEventHandler(LoadFileList);
            fileType.ParamChanged += reloadFileListHandler;
            cover.ParamChanged += reloadFileListHandler;
            coverFile.ParamChanged += reloadFileListHandler;
            minDimInput.ParamChanged += reloadFileListHandler;
            outputFilePath.ParamChanged += reloadFileListHandler;

            coverPattern.ParamChanged += (param) => GuessCover(true);

            // Load top-level params into list (ordered)
            Params = new List<Param> {
                noGui,
                generalParams,
                header,
                labels,
                cover
            };

            ImageList = new List<ImageData>();
            LoadingImageList = false;

            drawnCount = 0;

            try {
                string procName = Process.GetCurrentProcess().ProcessName;
                availableRamCounter = new PerformanceCounter("Memory", "Available Bytes", true);
                workingSetCounter = new PerformanceCounter("Process", "Working Set", procName, true);
                //workingSetPeakCounter = new PerformanceCounter("Process", "Working Set Peak", procName, true);
                canAnalyzeRam = true;
            } catch (InvalidOperationException ex) {
                Console.Error.WriteLine("Encountered an error when seting up memory analysis: {0}", ex.Message);
                Console.WriteLine("Multithreading will be disabled. If this persists, try running 'lodctr /r' (as admin)");
            }
        }

        /// <summary>
        /// Load settings from a settings xml file
        /// </summary>
        /// <param name="filename">The filename/path</param>
        public void LoadSettings(string filename) {
            try {
                XmlTextReader xmlReader = new XmlTextReader(filename);
                XmlSerializer ser = new XmlSerializer(Params.GetType());
                var deserializedList = (List<Param>)ser.Deserialize(xmlReader);
                xmlReader.Close();

                SettingsFile = filename;

                Console.WriteLine("Loading Params from {0}", SettingsFile);

                var headerTitleOrig = headerTitle.Val;
                var coverFileOrig = coverFile.Val;

                foreach (var param in Params) {
                    param.Load(deserializedList);
                }

                headerTitle.Val = headerTitleOrig;
                coverFile.Val = coverFileOrig;

                SettingsChanged?.Invoke(new SettingsChangedEventArgs(SettingsFile, "Loaded", true));

            } catch (Exception) {
                Console.WriteLine("Couldn't load {0}! Using hard-coded defaults.", SettingsFile);
                SettingsChanged?.Invoke(new SettingsChangedEventArgs(filename, "Load Failed", false));
            }
        }

        public void SaveSettings() {
            SaveSettings(SettingsFile);
        }

        public void SaveSettings(string path) {
            try {
                TextWriter w = new StreamWriter(path);
                new XmlSerializer(Params.GetType()).Serialize(w, Params);
                w.Close();
                SettingsFile = path;
                Console.WriteLine("Saved settings to {0}", SettingsFile);
                SettingsChanged?.Invoke(new SettingsChangedEventArgs(SettingsFile, "Saved", true));
            } catch (Exception e) {
                Console.WriteLine("Save failed! :: {0}", e);
                SettingsChanged?.Invoke(new SettingsChangedEventArgs(path, "Save Failed", false));
            }
        }

        public bool Load(string[] args) {

            // Get any command line arguments
            foreach (string a in args) {
                foreach (Param p in Params) {
                    try {
                        p.Parse(a);
                    } catch (ArgumentOutOfRangeException e) {
                        // Die on any bad params
                        Console.Error.WriteLine("Error Parsing Argument {0}: {1}", a, e.Message);
                        return false;
                    }
                }
            }

            // If the GUI is enabled, remove the No GUI option
            if (GuiEnabled && Params.Contains(noGui)) {
                Params.Remove(noGui);
            }

            return true;
        }

        private bool GuessFile(FileParam fileParam, string[] patterns, bool force) {
            bool changed = false;
            fileParam.Ext = fileType.Val;
            // If the command line set the cover file pattern/name,
            // make sure it exists. If not, guess.
            if (force || fileParam.File == null) {
                fileParam.Directory = sourceDir;
                changed = fileParam.Guess(patterns);
            }
            return changed;
        }

        private bool GuessCover(bool force) => GuessFile(coverFile, !string.IsNullOrEmpty(coverPattern.Val) ? new string[] { coverPattern.Val } : coverNames, force);

        public void LoadFileList(Param p) {
            Debug.WriteLine("Reloading file list due to change in {0}", p.Arg);
            LoadFileList();
        }

        public void LoadFileList() {

            LoadingImageList = true;

            Thread t = new Thread(delegate () {
                var toRemove = new List<string>();
                // Remove the cover/logo from the list if it's being displayed full size
                if (cover.Val) {
                    toRemove.Add(coverFile.Path);
                }

                // Get a list of all the images in the directory, 
                // Don't include hidden files
                IEnumerable<string> files = 
                    from file in sourceDir.GetFiles()
                    //where !toRemove.Contains(file.FullName)
                    where file.Extension.ToLower().Contains(fileType.Val.ToLower())
                    where (file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden
                    select file.FullName;

                // Load Image data into list
                Monitor.Enter(ImageList);
                ImageList.Clear();
                foreach (string path in files) {
                    
                    // Load image info without actually loading the image
                    var imageUri = new Uri(path);
                    var bmpFrame = BitmapFrame.Create(imageUri, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                    var image = new ImageData(new Size(bmpFrame.PixelWidth, bmpFrame.PixelHeight), path);

                    // Don't include images smaller than minDimInput
                    if (bmpFrame.PixelWidth < minDimInput.Val && bmpFrame.PixelHeight < minDimInput.Val) {
                        image.Include = false;
                    } else
                    // Don't include a previously generated contact sheet if we can avoid it
                    if (Path.GetFileName(path).Equals(Path.GetFileName(outputFilePath.Val))) {
                        image.Include = false;
                    } else
                    // Don't include cover or logo files    
                    if (toRemove.Contains(path)) {
                        image.Include = false;
                    }

                    ImageList.Add(image);
                }
                Monitor.Exit(ImageList);
                ImageListChanged?.Invoke(new ImageListChangedEventArgs());
                LoadingImageList = false;
            });
            t.Start();
        }

        public bool Run() {
            while (LoadingImageList) {
                Thread.Sleep(200);
            }

            var images = ImageList.Where(i => i.Include);

            activeDrawThreads = 0;
            if (!images.Any()) {
                ExceptionOccurred?.Invoke(new Exception(string.Format("No valid/selected {0} Images in {1}!", fileType.Val, SourceDirectory)));
                return true; // Don't exit
            }

            // Avoid stupidness
            if (!cover.Val || coverFile.File == null) {
                cover.Val = false;
                fillCoverGap.Val = false;
            }

            // Mark the start time
            startTime = DateTime.Now;

            #region Cover Setup

            // Draw the cover
            Image coverImage = null;
            Rectangle coverBounds = new Rectangle();
            if (cover.Val) {
                coverImage = Image.FromFile(coverFile.File.FullName);

                if (coverImage.Width > sheetWidth.Val) {
                    coverBounds.Width = sheetWidth.Val;
                    coverBounds.Height = (int)Math.Round(coverImage.Height * (double)sheetWidth.Val / coverImage.Width);
                    coverBounds.X = 0;

                    // Since there is no gap, no sense in trying to fill it
                    fillCoverGap.Val = false;
                } else {
                    coverBounds.Width = coverImage.Width;
                    coverBounds.Height = coverImage.Height;

                    if (fillCoverGap.Val && (sheetWidth.Val - coverBounds.Width) >= (sheetWidth.Val / columns.Val)) {
                        // Shift cover to the left to set up the gap to fill
                        coverBounds.X = 0;
                    } else {
                        // Not enough room for gap filling, center the cover
                        if (fillCoverGap.Val) {
                            double scale = 0.7;
                            Console.WriteLine("Reducing cover size by a factor of {0} to create gap.", scale);
                            //fillCoverGap.Val = false;

                            coverBounds.Width = (int)(coverBounds.Width * scale);
                            coverBounds.Height = (int)(coverBounds.Height * scale);
                        } else {
                            coverBounds.X = (sheetWidth.Val / 2) - (coverBounds.Width / 2);
                        }
                    }
                }

            }

            #endregion

            #region Analyses

            // Begin image analysis
            imageCount = images.Count();
            Console.WriteLine("Analyzing {0} files (Pass 1)", imageCount);

            List<List<ImageData>> analyses = new List<List<ImageData>> {
                new List<ImageData>()
            };

            int rowIndex = 0;
            int maxRowHeight = 0;
            int rowHeight;
            int fileIndex = 1;

            // First pass, add the same number of images to each row,
            // scale to width, record maximum row height
            foreach (ImageData data in images) {
                // Add image to row
                analyses[rowIndex].Add(data);

                if (analyses[rowIndex].Count == columns.Val || fileIndex == images.Count()) {
                    // Scale the row to fit the sheetwidth
                    rowHeight = ScaleRow(analyses[rowIndex], sheetWidth.Val);

                    // Record max row height
                    if (analyses[rowIndex].Count == columns.Val) {
                        maxRowHeight = Math.Max(maxRowHeight, rowHeight);
                    }
                    ++rowIndex;
                    analyses.Add(new List<ImageData>());
                }
                ++fileIndex;
            }

            Console.WriteLine("Analyzing {0} Rows (Pass 2), maxRowHeight: {1}", analyses.Count(), maxRowHeight);

            Size minRowDims;

            // Second pass tries to make all rows of similar height by
            // shifting images and rescaling rows.
            bool done = false;
            int rowWidth;
            Point curPoint = new Point(0, 0);
            bool inGap = fillCoverGap.Val;

            for (rowIndex = 0; !done; ++rowIndex) {

                if (inGap) {
                    // Row space = cover gap
                    rowWidth = sheetWidth.Val - coverBounds.Width;
                    curPoint.X = coverBounds.Width;
                } else {
                    // Row space = sheet width
                    rowWidth = sheetWidth.Val;
                    curPoint.X = 0;
                }

                // Set the first image's location
                // Succeeding row images will follow horizontally
                analyses[rowIndex][0].X = curPoint.X;
                analyses[rowIndex][0].Y = curPoint.Y;

                // Do the scaling/shifting to give the row a similar
                // height to the rest, with each image's dimensions
                // greater than or equal to the minimum dimension param.
                rowHeight = ScaleRow(analyses[rowIndex], rowWidth);
                minRowDims = MinDims(analyses[rowIndex]);
                while ((analyses[rowIndex].Count() > 1) &&
                        (rowHeight < (maxRowHeight * 0.85) ||
                         minRowDims.Width < minDim.Val ||
                         minRowDims.Height < minDim.Val ||
                         analyses[rowIndex].Count > columns.Val)) {
                    ShiftImage(analyses, rowIndex, rowIndex + 1);
                    rowHeight = ScaleRow(analyses[rowIndex], rowWidth);
                    minRowDims = MinDims(analyses[rowIndex]);
                }


                // Process at the end of the cover gap
                // Or at the end of the imagelist
                int overFlow = (curPoint.Y + rowHeight) - coverBounds.Height;
                if (inGap && (overFlow > 0 ||
                    (rowIndex + 1 == analyses.Count ||
                        analyses[rowIndex + 1].Count == 0))) {
                    if (overFlow > rowHeight / 3) {
                        // This row is too tall to fit in the gap.
                        // Move all images in the row to the next one
                        while (analyses[rowIndex].Count() > 0) {
                            ShiftImage(analyses, rowIndex, rowIndex + 1);
                        }
                        // Remove this empty row
                        Console.WriteLine("Removing row " + rowIndex);
                        analyses.Remove(analyses[rowIndex]);

                        // Since we removed a row, the next row is now this one.
                        // Make sure to process it
                        --rowIndex;
                    }

                    // If we just moved the first row down, then there's no point in
                    // resizing the gap images (there aren't any).
                    if (rowIndex >= 0) {

                        // Scale the cover and the gap images so they are the same height
                        double h1 = coverBounds.Height;
                        double w1 = coverBounds.Width;
                        double h2 = analyses[rowIndex][0].Y + analyses[rowIndex][0].Height;
                        double w2 = rowWidth;

                        double f1 = (h2 * sheetWidth.Val) / ((h1 * w2) + (h2 * w1));
                        //double f2 = f1 * h1 / h2;

                        coverBounds.Width = (int)Math.Round(coverBounds.Width * f1);
                        coverBounds.Height = (int)Math.Round(coverBounds.Height * f1);

                        curPoint.Y = 0;
                        for (int i = 0; i <= rowIndex; ++i) {
                            // Move images to the start of the new gap
                            analyses[i][0].X = coverBounds.Width;
                            analyses[i][0].Y = curPoint.Y;
                            // Scale row width to the new gap
                            rowHeight = ScaleRow(analyses[i], sheetWidth.Val - coverBounds.Width);
                            Console.WriteLine("In Gap, Final Scaling, Row {0}", i);
                            // Next row
                            curPoint.Y += rowHeight;
                        }
                        // Adjust for scale error
                        coverBounds.Height = curPoint.Y;
                    } else {
                        // No gap images. Display the cover normally.
                        coverBounds.X = sheetWidth.Val / 2 - coverBounds.Width / 2;
                        fillCoverGap.Val = false;
                        Console.WriteLine("GAP FILL FAILED. Cover image is too small.");
                    }
                    // We're done with the gap
                    inGap = false;
                } else {
                    curPoint.Y += rowHeight;
                }

                // Adjust the last rows to account for distortion

                if (rowIndex + 1 == analyses.Count ||
                    analyses[rowIndex + 1].Count == 0) {
                    int lastRowHeight = rowHeight;
                    int lastRowWidth = analyses[rowIndex].Last().X + analyses[rowIndex].Last().Width;
                    // If this is a single row sheet, don't try to get the previous row's height
                    if (rowIndex > 0) {
                        lastRowHeight = analyses[rowIndex - 1][0].Height;
                        lastRowWidth = analyses[rowIndex - 1].Last().X + analyses[rowIndex - 1].Last().Width;
                    }

                    // Attempt to even out the last two rows so there aren't any massive
                    // images at the end
                    // Don't adjust if the last row was in the cover gap
                    bool lastRowInGap = rowIndex > 0 && analyses[rowIndex - 1].Last().Y < coverBounds.Height;
                    while (!lastRowInGap &&
                        rowHeight > lastRowHeight * 2 &&
                        analyses[rowIndex - 1].Count > 1) {
                        ShiftImage(analyses, rowIndex - 1, rowIndex);
                        lastRowHeight = ScaleRow(analyses[rowIndex - 1], lastRowWidth);
                        analyses[rowIndex][0].X = curPoint.X;
                        analyses[rowIndex][0].Y += lastRowHeight;
                        rowHeight = ScaleRow(analyses[rowIndex], rowWidth);
                        Console.WriteLine("Row {0} Rescaled, {1} images. Height: {2}", rowIndex - 1, analyses[rowIndex - 1].Count, lastRowHeight);
                    }
                    done = true;
                }

                if (rowIndex >= 0) {
                    Console.WriteLine("Row {0} finished, {1} images. Height: {2}. Y: {3}", rowIndex, analyses[rowIndex].Count, rowHeight, analyses[rowIndex][0].Y);
                }
            }

            // Final pass, make sure rows don't overlap or have gaps between them
            curPoint.Y = analyses[0][0].Y;
            foreach (List<ImageData> row in analyses) {
                foreach (ImageData im in row) {
                    im.Y = curPoint.Y;
                }
                if (row.Count > 0) {
                    curPoint.Y += row[0].Height;
                }
            }
            // Update list watchers so they see the new sizes
            ImageListChanged?.Invoke(new ImageListChangedEventArgs());

            #endregion

            #region Drawing
            // Detemermine the maximum image dimensions
            Size maxSize =
                (from row in analyses
                 where row.Count > 0
                 select row.OrderBy(im => im.OriginalSize.Height).Last())
                      .OrderBy(im => im.OriginalSize.Height).Last().OriginalSize;

            // Set up the header
            Image headerImage = null;
            int headerHeight = 0;
            double headerWidth = 0;
            if (header.Val) {
                headerImage = new Bitmap(sheetWidth.Val, sheetWidth.Val);
                Graphics headerG = Graphics.FromImage(headerImage);
                SolidBrush br = new SolidBrush(Color.White);
                string headerText = headerTitle.Val;
                if (headerStats.Val) {
                    headerText += string.Format("\n(x{0}) max {1}x{2}px", images.Count(), maxSize.Width, maxSize.Height);
                }

                Font headerFont = new Font("Arial", headerFontSize.Val, headerBold.Val ? FontStyle.Bold : FontStyle.Regular);
                SizeF headerSize = headerG.MeasureString(headerText, headerFont, sheetWidth.Val);
                headerHeight = (int)Math.Ceiling(headerSize.Height);
                headerWidth = headerSize.Width;
                Rectangle headerRegion = new Rectangle(0, 0, sheetWidth.Val, headerHeight);
                headerG.DrawString(headerText, headerFont, br, headerRegion);
                headerG.Dispose();
            }

            // Determine the overall sheet height
            ImageData last = analyses.Last(l => l.Count > 0)[0];
            int sheetHeight = last.Y + last.Height + headerHeight;
            if (!fillCoverGap.Val) {
                sheetHeight += coverBounds.Height;
            }

            // Create the output image
            Image sheet = new Bitmap(sheetWidth.Val, sheetHeight);

            // Set up the Graphics and resampling options
            Graphics g = Graphics.FromImage(sheet);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            // Draw the header
            if (header.Val) {
                Console.WriteLine("Drawing header...");
                g.DrawImage(headerImage, 0, 0);
                headerImage.Dispose();
            }
            // Draw the cover
            if (cover.Val && coverFile.Val != string.Empty) {
                Console.WriteLine("Drawing cover...");
                Rectangle bounds = coverBounds;
                bounds.Y += headerHeight + borders.Val;
                bounds.X += borders.Val;
                bounds.Width -= (borders.Val * 2);
                bounds.Height -= (borders.Val * 2);
                if (preview.Val) {
                    g.FillRectangle(Brushes.White, bounds);
                    g.DrawRectangle(Pens.LightGreen, bounds);
                    g.DrawString(
                        "COVER",
                        new Font("Times New Roman", 14, FontStyle.Bold),
                        Brushes.Black,
                        bounds.X + bounds.Width / 2,
                        bounds.Y + bounds.Height / 2);
                } else {
                    g.DrawImage(coverImage, bounds);
                }
                coverImage.Dispose();
            }

            // Interpolation is being done by the draw function on separate graphics
            // objects, no need to use it here. Same for smoothing.
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

            // Draw the thumbnail images
            int index = 1;
            Console.WriteLine("Drawing sheet...");

            drawnCount = 0;


            if (threading.Val && canAnalyzeRam) {
                drawThreadsRunning = true;
                new Thread(new ThreadStart(MonitorRunningMemory)).Start();
            }

            foreach (List<ImageData> row in analyses.Where(l => l.Count > 0)) {
                foreach (ImageData col in row) {
                    col.Y += headerHeight;
                    if (!fillCoverGap.Val) {
                        col.Y += coverBounds.Height;
                    }

                    // Create info for threaded load/draw operation
                    DrawThreadObj tdata = new DrawThreadObj() {
                        Image = col,
                        Index = index,
                        File = col.File,
                        ImageTotal = images.Count(),
                        G = g,
                        FontSize = labels.Val ? labelFontSize.Val : 0,
                        BorderWidth = borders.Val
                    };

                    if (col == row.Last() && col.X + col.Width != sheetWidth.Val) {
                        tdata.Image.Width = sheetWidth.Val - col.X;
                    }

                    // Add the operation to thread pool if threading is on.
                    // Don't thread the first image so memory data can be gathered.
                    if (threading.Val && canAnalyzeRam) {
                        // Analyze RAM
                        //Console.WriteLine("Required Ram: {0}, Available Ram: {1}",
                        //    Math.Round(drawRam / (1024f * 1024f), 2),
                        //    Math.Round(availableRamCounter.NextValue() / (1024f * 1024f), 2));

                        bool outofMemory() => drawRam > availableRamCounter.NextValue();

                        if (outofMemory()) {
                            Console.WriteLine("Not enough memory! Required: {0}Mb, Available: {1}Mb. Waiting for some threads to finish...",
                                Math.Round(drawRam / (1024f * 1024f), 2),
                                Math.Round(availableRamCounter.NextValue() / (1024f * 1024f), 2));
                        }
                        while ((activeDrawThreads >= maxThreads.Val) || outofMemory()) {
                            Thread.Sleep(100);
                        }

                        Interlocked.Increment(ref activeDrawThreads);

                        // Draw
                        ThreadPool.QueueUserWorkItem(new WaitCallback(DrawThumb), tdata);
                    } else {
                        DrawThumb(tdata);
                    }

                    ++index;
                }
            }

            while (drawnCount < imageCount) {
                Thread.Sleep(100);
            }
            drawThreadsRunning = false;

            // Save the sheet with the given Jpeg quality
            ImageCodecInfo jpgEncoder = ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            myEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality.Val);


            DateTime endTime = DateTime.Now;
            TimeSpan duration = endTime - startTime;

            #endregion

            #region Finish

            Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine("Completed! It took {0}", duration);
            Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine("Sheet Size: {0} images, {1} rows, {2}x{3}px", imageCount, analyses.Count(r => r.Count > 0), sheet.Size.Width, sheet.Size.Height);
            Console.WriteLine("Maximum Images per Row: {0}", analyses.Max(r => r.Count));
            Console.WriteLine("Minimum Images per Row: {0}", analyses.Where(r => r.Count > 0).Min(r => r.Count));
            Console.WriteLine("Output Quality: {0}%{1}", quality.Val, interpolate.Val ? ", Using High-Quality Interpolation." : "");
            Console.WriteLine("---------------------------------------------------------------------------");

            try {
                Console.Write("Saving to {0}... ", OutFilePath);
                sheet.Save(OutFilePath, jpgEncoder, myEncoderParameters);
                Console.WriteLine("Saved. Size: {0:.00}Mb", new FileInfo(OutFilePath).Length / (1024f * 1024f));
            } catch (System.Runtime.InteropServices.ExternalException e) {
                Exception ex = new Exception(string.Format("Can't Save Sheet: {0}", e.Message), e);
                Console.Error.WriteLine(ex.Message);
                ExceptionOccurred?.Invoke(ex);
            } finally {
                Console.WriteLine("---------------------------------------------------------------------------");
                // Clean up
                sheet.Dispose();
            }

            Console.WriteLine("Exit on Complete: {0}", exitOnComplete.Val);
            if (exitOnComplete.Val) {
                return false;
            }

            return true;

            #endregion
        }


        /// <summary>
        /// Monitors the RAM used by the draw threads, recording to drawRam
        /// </summary>
        private void MonitorRunningMemory() {
            // Record the available RAM
            ramUsageAtStart = workingSetCounter.NextValue();
            drawRam = 0;
            float ram;
            DateTime start = DateTime.Now;
            bool stall = false;
            while (drawThreadsRunning) {
                ram = Math.Max(drawRam, (workingSetCounter.NextValue() - ramUsageAtStart));
                drawRam = ram;
                Thread.Sleep(50);

                // If there is no activity for a while, then the max ram
                // value is outdated. Reset it if necessary.
                if (!stall && activeDrawThreads == 0) {
                    stall = true;
                    start = DateTime.Now;
                } else if (stall && activeDrawThreads != 0) {
                    stall = false;
                } else if (stall && (DateTime.Now.Millisecond - start.Millisecond) > 500) {
                    drawRam /= maxThreads.Val;
                    stall = false;
                }
            }
        }

        private void DrawThumb(object state) {
            DrawThreadObj data = (DrawThreadObj)state;

            Image i = null;
            if (!preview.Val) {
                i = new Bitmap(data.File);
            }

            Rectangle thumb = new Rectangle(
                data.BorderWidth,
                data.BorderWidth,
                data.Image.Width - data.BorderWidth * 2,
                data.Image.Height - data.BorderWidth * 2);
            Image bmp = new Bitmap(data.Image.Width, data.Image.Height);

            // Load the image
            Graphics thumbG = Graphics.FromImage(bmp);

            // Only use the high-quality shit if specified
            if (interpolate.Val) {
                thumbG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            } else {
                thumbG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            }

            // Smooth the image. This doesn't hurt much.
            //thumbG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            if (preview.Val) {
                thumbG.FillRectangle(Brushes.White, thumb);
                thumbG.DrawRectangle(Pens.LightGreen, thumb);
                if (data.Image.IsLogo) {
                    thumbG.DrawString(
                        "LOGO", new
                            Font("Times New Roman", 14, FontStyle.Bold),
                            Brushes.Black,
                            thumb.X + thumb.Width / 2,
                            thumb.Y + thumb.Height / 2);
                }
            } else {
                thumbG.DrawImage(i, thumb);
            }
            if (i != null) {
                i.Dispose();
            }

            // Draw image name labels
            if (data.FontSize > 0 && !data.Image.IsLogo) {
                // Set label to file name, no extension
                string label = data.File.Split('\\').Last();
                label = label.Substring(0, label.LastIndexOf("."));
                // Determine label size
                Font font = new Font(FontFamily.GenericSansSerif, data.FontSize);
                SizeF labelSize = thumbG.MeasureString(label, font, data.Image.Width);
                // Center label at the bottom of the thumbnail
                RectangleF labelClip = new RectangleF(
                    (data.Image.Width - labelSize.Width) / 2,
                    data.Image.Height - labelSize.Height,
                    labelSize.Width,
                    labelSize.Height);
                // Align text center
                StringFormat alignCenter = new StringFormat {
                    Alignment = StringAlignment.Center
                };
                // Draw translucent black background
                thumbG.FillRectangle(new SolidBrush(Color.FromArgb(150, Color.Black)), labelClip);
                // Draw label
                thumbG.DrawString(label, font, Brushes.White, labelClip, alignCenter);
            }



            // Have to lock on the Graphics object
            // because two threads can't draw on it at the same time
            Monitor.Enter(data.G);
            data.G.DrawImage(bmp, data.Image.Bounds);
            Monitor.Exit(data.G);

            // Set the RAM used for this draw
            //Console.WriteLine("t{0}: RAM={1}", data.Index, Math.Abs(workingSetPeakCounter.NextValue() - ramUsageAtStart));

            // Clean up
            bmp.Dispose();
            thumbG.Dispose();

            // Output status
            Console.WriteLine("{0}, {1} of {2} ",
                data.File.Split('\\').Last(), data.Index, data.ImageTotal);

            // Update counters
            Interlocked.Decrement(ref activeDrawThreads);
            Interlocked.Increment(ref drawnCount);

            // Send progress to listeners
            DrawProgressChanged?.Invoke(new DrawProgressEventArgs(drawnCount, imageCount, DateTime.Now - startTime));
        }

        private void ShiftImage(List<List<ImageData>> list, int fromRow, int toRow) {
            if (fromRow < toRow) {
                if (list.Count <= toRow) {
                    list.Add(new List<ImageData>());
                }
                list[toRow].Insert(0, list[fromRow].Last());
                list[fromRow].Remove(list[fromRow].Last());
            } else {
                list[toRow].Add(list[fromRow].First());
                list[fromRow].Remove(list[fromRow].First());
            }
        }

        private int ScaleRow(List<ImageData> list, int width) {
            double maxImageHeight = 0;
            int rowWidth = 0;
            double rowHeight = 0;
            double factor;

            if (list.Count > 1) {
                // Determine the maximum image height
                foreach (ImageData data in list) {
                    maxImageHeight = Math.Max(maxImageHeight, data.OriginalSize.Height);
                }

                // Scale all images to the maximum image height and determine the row width
                foreach (ImageData data in list) {
                    factor = 1.0;
                    if (data.OriginalSize.Height < maxImageHeight) {
                        factor = data.ScaleToHeight(maxImageHeight);
                    }
                    rowWidth += (int)(data.OriginalSize.Width * factor);
                }

                // Calculate the row height based on the factor needed to scale the row to the sheet width
                rowHeight = Math.Round((double)maxImageHeight * (width / (double)rowWidth));

                // Scale images to calculated height
                foreach (ImageData id in list) {
                    id.ScaleToHeight(rowHeight);
                }

                // Set image locations
                for (int i = 1; i < list.Count; ++i) {
                    list[i].X = list[i - 1].X + list[i - 1].Width;
                    list[i].Y = list[0].Y;
                }
            } else if (list.Count > 0) {
                // Calculate the row height based on the factor needed to scale the row to the sheet width
                rowWidth = list[0].OriginalSize.Width;
                rowHeight = Math.Round(list[0].OriginalSize.Height * (width / (double)rowWidth));

                // Scale images to calculated height
                list[0].ScaleToHeight(rowHeight);
            }

            return (int)rowHeight;
        }

        private static Size MinDims(List<ImageData> row) {
            Size s = new Size(row[0].Width, row[0].Height);
            foreach (ImageData img in row) {
                s.Width = Math.Min(s.Width, img.Width);
                s.Height = Math.Min(s.Height, img.Height);
            }
            return s;
        }

        public bool CheckPrintHelp(string[] args) {
            bool markDown = args.Contains("markdown");
            if (args.Any(a => helpStrings.Contains(a))) {
                Console.WriteLine("------ Parameters ------");
                foreach (Param p in Params) {
                    Console.WriteLine(p.GetHelp(markDown));
                }
                string help = markDown ? "`--help`" : "--help:";
                Console.WriteLine("{0} View this help message.", help);
                return true;
            }
            return false;
        }
    }
}
