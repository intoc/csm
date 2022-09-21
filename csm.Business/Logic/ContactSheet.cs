using csm.Business.Models;
using Serilog;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Path = System.IO.Path;

namespace csm.Business.Logic;

public delegate void SourceChangedEventHandler(string? path);
public delegate void DrawProgressEventHandler(DrawProgressEventArgs args);
public delegate void SettingsChangedEventHandler(SettingsChangedEventArgs args);
public delegate void ImageListChangedEventHandler();
public delegate void ExceptionEventHandler(string message, Exception? e = null);

/// <summary>
/// Creates contact sheets
/// </summary>
public sealed class ContactSheet : IDisposable {

    #region Private Constants

    private static readonly string coverRegexes = @"(cover|big|max|square|front|folder)\.";
    private static readonly string[] helpStrings = { "--help", "-help", "/?", "-?" };
    private const int DEFAULT_WIDTH = 900;
    private const int DEFAULT_COLUMNS = 6;
    private const int DEFAULT_QUALITY = 90;

    #endregion

    #region Public Properties

    /// <summary>
    /// The parameters used for creating the sheet
    /// </summary>
    public List<Param> Params { get; private set; }

    /// <summary>
    /// The path to the settings file
    /// </summary>
    public string? SettingsFile { get; private set; }

    /// <summary>
    /// The list of images found in the source directory
    /// </summary>
    public IList<ImageData> ImageList => imageSet.Images;

    /// <summary>
    /// Whether the GUI is enabled
    /// </summary>
    public bool GuiEnabled => !noGui.BoolValue;

    /// <summary>
    /// Whether the output directory should be opened after the contact sheet output file is created
    /// </summary>
    public bool OpenOutputDir => openOutputDirectoryOnComplete.BoolValue;

    private bool _pauseActions;
    public bool PauseParamEventHandling { 
        get => _pauseActions; 
        set {
            _pauseActions = value;
            if (_pauseActions) {
                Log.Debug("Paused Param event handling");
            } else {
                Log.Debug("Resumed Param event handling");
                HandlePausedEvents();
            }
        } 
    }

    /// <summary>
    /// The source path
    /// </summary>
    public string? Source {
        get {
            return fileSource?.FullPath;
        }
        set {
            var oldSource = fileSource;
           
            try {
                fileSource = _fileSourceBuilder.Build(value);
            } catch (Exception ex) {
                ErrorOccurred?.Invoke("Can't load source path.", ex);
            }
            
            if (oldSource?.FullPath != fileSource?.FullPath) {
                oldSource?.Dispose();
                fileSource?.Initialize(() => SourceChanged?.Invoke(fileSource?.FullPath));
            } else {
                // It's the same source, we don't need the new one
                fileSource?.Dispose();
                if (oldSource != null) {
                    fileSource = oldSource;
                }
            }
        }
    }

    #endregion

    #region Public Events

    /// <summary>
    /// Fired when the progress of drawing the output contact sheet changes
    /// </summary>
    public event DrawProgressEventHandler DrawProgressChanged = delegate { };

    /// <summary>
    /// Fired when there is a change to the contents of the image list
    /// </summary>
    public event ImageListChangedEventHandler ImageListChanged = delegate { };

    /// <summary>
    /// Fired when the settings file changes and is loaded
    /// </summary>
    public event SettingsChangedEventHandler SettingsChanged = delegate { };

    /// <summary>
    /// Fired when the source directory is changed
    /// </summary>
    public event SourceChangedEventHandler SourceChanged = delegate { };

    /// <summary>
    /// Fired when an exception occurred
    /// </summary>
    public event ExceptionEventHandler ErrorOccurred = delegate { };

    #endregion

    #region Private Fields

    private IFileSource? fileSource;
    private readonly IImageSet imageSet = new ImageSet();
    private readonly IFileSourceBuilder _fileSourceBuilder;

    private readonly BoolParam cover;
    private readonly BoolParam exitOnComplete;
    private readonly BoolParam fillCoverGap;
    private readonly BoolParam header;
    private readonly BoolParam headerBold;
    private readonly BoolParam headerStats;
    private readonly BoolParam interpolate;
    private readonly BoolParam labels;
    private readonly BoolParam noGui;
    private readonly BoolParam openOutputDirectoryOnComplete;
    private readonly BoolParam preview;
    private readonly FileParam coverFile;
    private readonly IntParam borders;
    private readonly IntParam columns;
    private readonly IntParam headerFontSize;
    private readonly IntParam labelFontSize;
    private readonly IntParam minDimThumbnail;
    private readonly IntParam minDimInput;
    private readonly IntParam quality;
    private readonly IntParam sheetWidth;
    private readonly StringParam coverPattern;
    private readonly StringParam fileType;
    private readonly StringParam headerTitle;
    private readonly StringParam outputFilePath;

    private readonly IDictionary<string, Action> PausedActions = new Dictionary<string, Action>();

    // Draw status variables
    private DateTime startTime;
    private int drawnCount, progressStep;
    private readonly object graphicsLock = new();
    private readonly object progressLock = new();

    #endregion

    /// <summary>
    /// Create a contact sheet instance
    /// </summary>
    public ContactSheet(IFileSourceBuilder fileSourceBuilder) {

        _fileSourceBuilder = fileSourceBuilder;

        // Set parameter fields and defaults

        #region General Parameters

        noGui = new BoolParam("-nogui", false);

        fileType = new StringParam("-filetype", ".jpg", "Extension") {
            MaxChars = 4
        };

        minDimInput = new IntParam("-mindiminput", 0, "px") {
            MinVal = 0,
            MaxVal = 999999
        };

        columns = new IntParam("-cols", DEFAULT_COLUMNS) {
            MinVal = 1,
            MaxVal = 99999
        };

        sheetWidth = new IntParam("-width", DEFAULT_WIDTH, "px") {
            MinVal = 1,
            MaxVal = 999999
        };

        minDimThumbnail = new IntParam("-mindim", 0, "px") {
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

        preview = new BoolParam("-preview", false);
        exitOnComplete = new BoolParam("-exit", false);

        outputFilePath = new StringParam("-outfile", "ContactSheet.jpg", "File Path") {
            MaxChars = 25
        };
        openOutputDirectoryOnComplete = new BoolParam("-openoutdir", true);

        var generalParams = new NullParam("General");
        generalParams.AddSubParam(fileType);
        generalParams.AddSubParam(minDimInput);
        generalParams.AddSubParam(sheetWidth);
        generalParams.AddSubParam(columns);
        generalParams.AddSubParam(minDimThumbnail);
        generalParams.AddSubParam(borders);
        generalParams.AddSubParam(quality);
        generalParams.AddSubParam(interpolate);
        generalParams.AddSubParam(exitOnComplete);
        generalParams.AddSubParam(openOutputDirectoryOnComplete);
        generalParams.AddSubParam(preview);
        generalParams.AddSubParam(outputFilePath);

        #endregion

        #region Label Parameters

        labels = new BoolParam("-labels", false);
        labelFontSize = new IntParam("-lsize", 8, "pt") {
            MinVal = 0,
            MaxVal = 136
        };
        labels.AddSubParam(labelFontSize);

        #endregion

        #region Header Parameters

        header = new BoolParam("-header", false);
        headerFontSize = new IntParam("-hsize", 12, "pt") {
            MinVal = 0,
            MaxVal = 180
        };
        headerBold = new BoolParam("-hbold", false);
        headerTitle = new StringParam("-htitle", "Title", "Words") {
            MaxChars = 20,
            ExcludeFromLoading = true
        };
        headerStats = new BoolParam("-hstats", false);
        header.AddSubParam(headerFontSize);
        header.AddSubParam(headerBold);
        header.AddSubParam(headerTitle);
        header.AddSubParam(headerStats);

        #endregion

        #region Cover Parameters

        cover = new BoolParam("-cover", false);
        coverPattern = new StringParam("-cregx", @"cover\.", "Regex") {
            MaxChars = 20
        };
        coverFile = new FileParam("-cfile", new DirectoryFileSource()) {
            ExcludeFromLoading = true
        };
        cover.AddSubParam(coverPattern);
        cover.AddSubParam(coverFile);
        fillCoverGap = new BoolParam("-cfill", false);
        cover.AddSubParam(fillCoverGap);

        #endregion

        #region Event Subscriptions

        // Setup all instances where a file list reload is required
        fileType.ParamChanged += async (path) => await LoadFileList(path);
        SourceChanged += async (path) => {
            Log.Information("Source set to {0}", path);
            imageSet.Source = fileSource ?? new DirectoryFileSource();
            headerTitle.ParseVal(fileSource?.Name);
            Log.Information("Directory Name -> Header Title: {0}", headerTitle.ParsedValue);
            if (cover.BoolValue) {
                await GuessCover(true);
            }
            await LoadFileList();
        };

        // Setup all instances where a image list refresh is required without a full reload
        coverFile.ParamChanged += RefreshImageList;
        minDimInput.ParamChanged += RefreshImageList;
        outputFilePath.ParamChanged += RefreshImageList;

        cover.ParamChanged += async (p) => await HandleShowCoverChanged(p);
        coverPattern.ParamChanged += async (p) => await HandleCoverPatternChanged(p);

        #endregion

        // Load top-level params into externaly visible Param list (ordered)
        Params = new List<Param> {
            noGui,
            generalParams,
            header,
            labels,
            cover
        };
    }

    private async Task HandleShowCoverChanged(Param param) {
        if (PauseParamEventHandling) {
            PausedActions["guessCover"] = async () => await HandleShowCoverChanged(param);
            return;
        }
        if (fileSource == null) {
            return;
        }
        if (cover.BoolValue) {
            if (!await GuessCover(false)) {
                // Image list will be refreshed via coverFile.ParamChanged
                // If GuessCover changes its value.
                // If not, it still needs to be refreshed do to the cover bool switch
                RefreshImageList(cover);
            }
        } else {
            RefreshImageList(cover);
        }
    }

    private async Task HandleCoverPatternChanged(Param param) {
        if (PauseParamEventHandling) {
            PausedActions["guessCover"] = async () => await HandleCoverPatternChanged(param);
            return;
        }
        if (!cover.BoolValue || fileSource == null) {
            return;
        }
        if (await GuessCover(true)) {
            RefreshImageList(cover);
        }
    }

    /// <summary>
    /// Handle all events that are in the PausedActions collection and resume normal event handling
    /// </summary>
    private void HandlePausedEvents() {
        foreach (KeyValuePair<string, Action> kvp in PausedActions) {
            kvp.Value();
        }
        PausedActions.Clear();
    }

    /// <summary>
    /// Get the output file path with an optional suffix
    /// </summary>
    /// <param name="suffix">Optional numeric filename suffix (before the extension)</param>
    /// <returns>The output file path</returns>
    public string OutFilePath(int suffix = 0) {
        string? path = outputFilePath.ParsedValue;
        if (path == null) {
            return string.Empty;
        }

        path = path.Replace("{title}", headerTitle.Value);

        if (suffix > 0) {
            path = Regex.Replace(path, @"\.(.*)$", $"_{suffix}.$1");
        }

        if (Path.IsPathRooted(path)) {
            return path;
        }
        if (fileSource == null) {
            return string.Empty;
        }

        // Use the parent directory
        var outputDirectory = fileSource?.ParentDirectoryPath;
        if (outputDirectory == null) {
            return string.Empty;
        }
        return Path.GetFullPath(Path.Combine(outputDirectory, path));
    }

    /// <summary>
    /// Load Params from a settings xml file
    /// </summary>
    /// <param name="filename">The filename/path</param>
    public bool LoadParamsFromFile(string filename) {
        try {
            SettingsFile = Path.GetFullPath(filename);
            if (!File.Exists(SettingsFile)) {
                ErrorOccurred?.Invoke($"Settings file does not exist ({SettingsFile}).");
                return false;
            }

            XmlTextReader xmlReader = new(filename);
            XmlSerializer ser = new(Params.GetType());
            var deserializedList = ser.Deserialize(xmlReader) as List<Param> ?? new List<Param>();
            xmlReader.Close();

            Log.Information("Loading Params from {0}", SettingsFile);

            foreach (var param in Params) {
                param.Load(deserializedList);
            }

            SettingsChanged?.Invoke(new SettingsChangedEventArgs(SettingsFile, "Loaded", true));
            return true;
        } catch (Exception e) {
            ErrorOccurred?.Invoke($"Couldn't load {SettingsFile}!", e);
            SettingsChanged?.Invoke(new SettingsChangedEventArgs(filename, "Load Failed", false));
        }
        return false;
    }

    /// <summary>
    /// Load Params from command-line arguments
    /// </summary>
    /// <param name="args">Command-line arguments</param>
    public void LoadParamsFromCommandLine(string[] args) {

        // Get any command line arguments
        foreach (string a in args) {
            foreach (Param p in Params) {
                p.Parse(a);
            }
        }

        // If the GUI is enabled, remove the No GUI option
        if (GuiEnabled && Params.Contains(noGui)) {
            Params.Remove(noGui);
        }
    }

    /// <summary>
    /// Save settings to a file. If <see cref="SettingsFile"/> is not set,
    /// save to default.xml.
    /// </summary>
    public void SaveSettings() {
        SaveSettings(SettingsFile ?? "default.xml");
    }

    /// <summary>
    /// Save settings to a file with the given path.
    /// </summary>
    /// <param name="path">The settings file path</param>
    public void SaveSettings(string path) {
        try {
            TextWriter w = new StreamWriter(path);
            new XmlSerializer(Params.GetType()).Serialize(w, Params);
            w.Close();
            SettingsFile = path;
            Log.Information("Saved settings to {0}", SettingsFile);
            SettingsChanged?.Invoke(new SettingsChangedEventArgs(SettingsFile, "Saved", true));
        } catch (Exception e) {
            Log.Information("Save failed! :: {0}", e);
            SettingsChanged?.Invoke(new SettingsChangedEventArgs(path, "Save Failed", false));
        }
    }

    /// <summary>
    /// Guess the cover file path
    /// </summary>
    /// <param name="force">Proceed even if the cover file path has already been set</param>
    private async Task<bool> GuessCover(bool force) {
        string pattern = coverPattern.ParsedValue ?? coverRegexes;
        return await imageSet.GuessFile(coverFile, fileType.Value, pattern, force);
    }

    /// <summary>
    /// Load the file list and image information from the source directory if it's set
    /// </summary>
    /// <param name="p">The <see cref="Param"/> that caused the need</param>
    public async Task LoadFileList(Param? p = null) {
        if (p != null) {
            Log.Information("Reloading file list due to change in {0}", p.CmdParameter);
        }
        await imageSet.LoadImageListAsync(fileType.ParsedValue ?? ".jpg", minDimInput.IntValue, Path.GetFileName(OutFilePath()), cover.BoolValue ? coverFile.FileName : null);
        ImageListChanged?.Invoke();
    }

    /// <summary>
    /// Refresh the image data and whether they should be included in the output based on parameter changes
    /// </summary>
    /// <param name="p">The <see cref="Param"/> that caused the need for the refresh</param>
    public void RefreshImageList(Param? p = null) {
        if (PauseParamEventHandling) {
            PausedActions["refreshImageList"] = () => RefreshImageList(p);
            return;
        }
        if (!imageSet.Images.Any()) {
            return;
        }
        if (p != null) {
            Log.Debug("Refreshing image list due to change in {0}", p.Desc);
        }
        imageSet.RefreshImageList(minDimInput.IntValue, Path.GetFileName(OutFilePath()), cover.BoolValue ? coverFile.FileName : null);
        ImageListChanged?.Invoke();
    }

    /// <summary>
    /// Scales a <see cref="Rectangle"/> by the given factor
    /// </summary>
    /// <param name="rect">The <see cref="Rectangle"/></param>
    /// <param name="scale">The scale factor</param>
    private static void ScaleRect(ref Rectangle rect, double scale) {
        rect.Width = (int)(rect.Width * scale);
        rect.Height = (int)(rect.Height * scale);
    }

    /// <summary>
    /// Run the image analysis and contact sheet creation process
    /// </summary>
    /// <returns>Whether the process is set to exit on complete</returns>
    public async Task<bool> DrawAndSave(bool waitForLoad = false) {
        if (string.IsNullOrEmpty(Source)) {
            ErrorOccurred?.Invoke("No/invalid Source selected!");
            return false; // Don't exit the GUI
        }

        IEnumerable<ImageData> images;
        List<List<ImageData>> analyses = new() {
            new List<ImageData>()
        };
        Rectangle coverBounds = new();
        Image? coverImage = null;
        int imageCount;
        int rowIndex = 0;
        int maxRowHeight = 0;
        int rowHeight;
        int fileIndex = 1;
        bool drawCover = cover.BoolValue;
        bool fillGap = drawCover && fillCoverGap.BoolValue;

        // Wait for the image list to be ready
        if (waitForLoad) {
            Log.Debug("DrawAndSave waiting...");
        }
        while (waitForLoad) {
            lock (imageSet.Images) {
                if (imageSet.Loaded) {
                    break;
                }
            }
            Thread.Sleep(250);
        }

        lock (imageSet.Images) {
            Log.Debug("DrawAndSave starting");

            images = imageSet.Images.Where(i => i.Include);
            imageCount = images.Count();

            if (imageCount == 0) {
                ErrorOccurred?.Invoke($"No valid/selected {fileType.ParsedValue} Images in {Source}!");
                return false; // Don't exit the GUI
            }

            // Mark the start time
            startTime = DateTime.Now;

            #region Cover Setup

            // Avoid stupidness
            if (drawCover && coverFile.File == null) {
                ErrorOccurred?.Invoke("Can't draw the cover because there is no cover file set.");
                drawCover = false;
                fillGap = false;
            }

            // Analyze the cover
            if (drawCover && coverFile.File != null) {
                // Begin image analysis
                Log.Information("Analyzing cover...");
                coverImage = Image.FromFile(coverFile.Path);
                coverBounds = new Rectangle { Width = coverImage.Width, Height = coverImage.Height };
                double maxCoverImageScaleForGap = Math.Round(0.75 * columns.IntValue) / columns.IntValue;

                if (fillGap) {
                    if (coverBounds.Width >= (sheetWidth.IntValue * maxCoverImageScaleForGap)) {
                        // We want a gap right? Make the cover smaller.
                        Log.Information("Cover image is too large. Reducing size to {0:0.00}xSheetWidth to create a gap.", maxCoverImageScaleForGap);
                        double scale = (sheetWidth.IntValue * maxCoverImageScaleForGap) / coverBounds.Width;
                        ScaleRect(ref coverBounds, scale);
                    } else {
                        // Otherwise the image is already small enough for gap filling
                        Log.Information("Cover image size allows for a natural gap.");
                    }
                } else if (coverBounds.Width < sheetWidth.IntValue) {
                    // Center images smaller than the sheet width
                    Log.Information("Centering cover image.");
                    coverBounds.X = (sheetWidth.IntValue - coverBounds.Width) / 2;
                } else {
                    // Scale the image down to sheet width
                    Log.Information("Cover image is too large. Reducing size to fit SheetWidth.");
                    ScaleRect(ref coverBounds, (double)sheetWidth.IntValue / coverBounds.Width);
                }
                Log.Information("Cover analysis complete. Fill gap: {0}, cover bounds: {1}", fillGap, coverBounds);
            }

            #endregion

            // Begin image analysis
            Log.Information("Analyzing {0} files (Pass 1)", imageCount);

            // First pass, add the same number of images to each row,
            // scale to width, record maximum row height
            foreach (ImageData data in images) {
                // Add image to row
                analyses[rowIndex].Add(data);

                if (analyses[rowIndex].Count == columns.IntValue || fileIndex == imageCount) {
                    // Scale the row to fit the sheetwidth
                    rowHeight = ScaleRow(analyses[rowIndex], sheetWidth.IntValue);

                    // Record max row height
                    if (analyses[rowIndex].Count == columns.IntValue) {
                        maxRowHeight = Math.Max(maxRowHeight, rowHeight);
                    }
                    ++rowIndex;
                    analyses.Add(new List<ImageData>());
                }
                ++fileIndex;
            }
        }

        Log.Information("Analyzing {0} Rows (Pass 2), maxRowHeight: {1}", analyses.Count, maxRowHeight);

        Size minRowDims;

        // Second pass tries to make all rows of similar height by
        // shifting images and rescaling rows.
        bool done = false;
        int rowWidth;
        Point curPoint = new(0, 0);
        bool inGap = fillGap;

        for (rowIndex = 0; !done; ++rowIndex) {

            if (inGap) {
                // Row space = cover gap
                rowWidth = sheetWidth.IntValue - coverBounds.Width;
                curPoint.X = coverBounds.Width;
            } else {
                // Row space = sheet width
                rowWidth = sheetWidth.IntValue;
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
            while (analyses[rowIndex].Count > 1 &&
                    (rowHeight < maxRowHeight * 0.85 ||
                     minRowDims.Width < minDimThumbnail.IntValue ||
                     minRowDims.Height < minDimThumbnail.IntValue ||
                     analyses[rowIndex].Count > columns.IntValue)) {
                ShiftImage(analyses, rowIndex, rowIndex + 1);
                rowHeight = ScaleRow(analyses[rowIndex], rowWidth);
                minRowDims = MinDims(analyses[rowIndex]);
            }

            // Process at the end of the cover gap
            // Or at the end of the imagelist
            int overFlow = curPoint.Y + rowHeight - coverBounds.Height;
            if (inGap && (overFlow > 0 ||
                rowIndex + 1 == analyses.Count ||
                    analyses[rowIndex + 1].Count == 0)) {
                if (overFlow > rowHeight / 3) {
                    // This row is too tall to fit in the gap.
                    // Move all images in the row to the next one
                    while (analyses[rowIndex].Count > 0) {
                        ShiftImage(analyses, rowIndex, rowIndex + 1);
                    }
                    // Remove this empty row
                    Log.Information("Removing row " + rowIndex);
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

                    double f1 = h2 * sheetWidth.IntValue / (h1 * w2 + h2 * w1);

                    coverBounds.Width = (int)Math.Round(coverBounds.Width * f1);
                    coverBounds.Height = (int)Math.Round(coverBounds.Height * f1);

                    curPoint.Y = 0;
                    for (int i = 0; i <= rowIndex; ++i) {
                        // Move images to the start of the new gap
                        analyses[i][0].X = coverBounds.Width;
                        analyses[i][0].Y = curPoint.Y;
                        // Scale row width to the new gap
                        rowHeight = ScaleRow(analyses[i], sheetWidth.IntValue - coverBounds.Width);
                        Log.Information("In Gap, Final Scaling, Row {0}", i);
                        // Next row
                        curPoint.Y += rowHeight;
                    }
                    // Adjust for scale error
                    coverBounds.Height = curPoint.Y;
                } else {
                    // No gap images. Display the cover normally.
                    coverBounds.X = sheetWidth.IntValue / 2 - coverBounds.Width / 2;
                    fillGap = false;
                    ErrorOccurred?.Invoke("Cover gap fill failed, image is too small. Centering.");
                }
                // We're done with the gap
                inGap = false;
            } else {
                curPoint.Y += rowHeight;
            }

            // Adjust the last rows to account for distortion
            if (rowIndex + 1 == analyses.Count || analyses[rowIndex + 1].Count == 0) {

                // If this is a single row sheet, don't try to get the previous row's dimensions
                bool isSingleRow = rowIndex == 0;
                var lastRow = isSingleRow ? analyses[rowIndex] : analyses[rowIndex - 1];
                int lastRowHeight = lastRow.First().Height;
                int lastRowWidth = lastRow.Last().X + lastRow.Last().Width;

                // Attempt to even out the last two rows so there aren't any massive images at the end
                // Don't adjust if the last row was in the cover gap
                bool lastRowInGap = rowIndex > 0 && analyses[rowIndex - 1].Last().Y < coverBounds.Height;
                while (!lastRowInGap && rowHeight > lastRowHeight * 2 && analyses[rowIndex - 1].Count > 1) {
                    ShiftImage(analyses, rowIndex - 1, rowIndex);
                    lastRowHeight = ScaleRow(analyses[rowIndex - 1], lastRowWidth);
                    analyses[rowIndex][0].X = curPoint.X;
                    analyses[rowIndex][0].Y += lastRowHeight;
                    rowHeight = ScaleRow(analyses[rowIndex], rowWidth);
                    Log.Information("Row {0} Rescaled, {1} images. Height: {2}px", rowIndex - 1, analyses[rowIndex - 1].Count, lastRowHeight);
                }
                done = true;
            }

            if (rowIndex >= 0) {
                Log.Information("Row {0}: {1} images. Height: {2}px. Y: {3}", rowIndex, analyses[rowIndex].Count, rowHeight, analyses[rowIndex][0].Y);
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
        ImageListChanged?.Invoke();

        #region Drawing

        // Set up the header
        Image? headerImage = null;
        int headerHeight = 0;
        if (header.BoolValue) {
            headerImage = new Bitmap(sheetWidth.IntValue, sheetWidth.IntValue);
            using Graphics headerG = Graphics.FromImage(headerImage);
            SolidBrush br = new(Color.White);
            string headerText = headerTitle.ParsedValue ?? string.Empty;
            Font headerFont = new("Arial", headerFontSize.IntValue, headerBold.BoolValue ? FontStyle.Bold : FontStyle.Regular);
            Font statsFont = new("Arial", headerFontSize.IntValue, FontStyle.Regular);
            int padding = 5;
            SizeF headerSize = headerG.MeasureString(headerText, headerFont, sheetWidth.IntValue - (padding * 2));
            SizeF statsSize = new();
            string? stats = null;
            if (headerStats.BoolValue) {
                // Determine largest image
                var maxSize = analyses
                    .Where(x => x.Count > 0)
                    .MaxBy(row => row.Max(img => img.OriginalSize.Height))?
                    .MaxBy(img => img.OriginalSize.Height)?.OriginalSize ?? default;
                // Determine how much space the stats will take up in the header
                stats = $"{imageCount} images. Maximum dimensions {maxSize.Width}x{maxSize.Height}px";
                statsSize = headerG.MeasureString(stats, statsFont, sheetWidth.IntValue - (padding * 2));
                statsSize.Height += padding * 2 + 1;
            }

            // Pre-draw the header
            headerHeight = (int)Math.Ceiling(headerSize.Height + statsSize.Height + padding * 2);
            Rectangle headerRegion = new(padding, padding, sheetWidth.IntValue, headerHeight);
            headerG.DrawString(headerText, headerFont, br, headerRegion);
            if (stats != null) {
                // Pre-draw stats
                int statsTop = (int)headerSize.Height + padding * 2 + 1;
                headerG.DrawLine(new Pen(Color.DarkSlateGray, 1), new Point(padding, statsTop), new Point(sheetWidth.IntValue - padding, statsTop));
                headerG.DrawString(stats, statsFont, br, 
                    new Rectangle(padding, statsTop + padding, sheetWidth.IntValue - padding * 2, (int)statsSize.Height));
            }
        }

        // Determine the overall sheet height
        ImageData last = analyses.Last(l => l.Count > 0)[0];
        int sheetHeight = last.Y + last.Height + headerHeight;
        if (!fillGap) {
            sheetHeight += coverBounds.Height;
        }

        // Create the output image
        using Image sheetImage = new Bitmap(sheetWidth.IntValue, sheetHeight);

        // Set up the Graphics and resampling options
        using Graphics sheetGraphics = Graphics.FromImage(sheetImage);
        sheetGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        sheetGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

        // Draw the header
        if (headerImage != null) {
            Log.Information("Drawing header...");
            sheetGraphics.DrawImage(headerImage, 0, 0);
            headerImage.Dispose();
        }

        // Draw the cover
        if (coverImage != null) {
            Log.Information("Drawing cover {0}...", Path.GetFileName(coverFile.Path));
            coverBounds.Y += headerHeight + borders.IntValue;
            coverBounds.X += borders.IntValue;
            coverBounds.Width -= borders.IntValue * 2;
            coverBounds.Height -= borders.IntValue * 2;
            if (preview.BoolValue) {
                sheetGraphics.FillRectangle(Brushes.White, coverBounds);
                sheetGraphics.DrawRectangle(Pens.LightGreen, coverBounds);
                sheetGraphics.DrawString(
                    "COVER",
                    new Font("Times New Roman", 14, FontStyle.Bold),
                    Brushes.Black,
                    coverBounds.X + coverBounds.Width / 2,
                    coverBounds.Y + coverBounds.Height / 2);
            } else {
                sheetGraphics.DrawImage(coverImage, coverBounds);
            }
            coverImage?.Dispose();
        }

        // Interpolation is being done by the draw function on separate graphics
        // objects, no need to use it here. Same for smoothing.
        sheetGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
        sheetGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

        // Draw the thumbnail images
        int index = 1;
        Log.Information("Drawing sheet...");

        drawnCount = 0;
        progressStep = 0;

        IList<Task> drawThumbTasks = new List<Task>();
        foreach (List<ImageData> row in analyses.Where(l => l.Count > 0)) {
            foreach (ImageData col in row) {
                col.Y += headerHeight;
                if (!fillGap) {
                    col.Y += coverBounds.Height;
                }

                // Create info for threaded load/draw operation
                DrawThreadObj tdata = new(col, sheetGraphics) {
                    Index = index,
                    ImageTotal = imageCount,
                    FontSize = labels.BoolValue ? labelFontSize.IntValue : 0,
                    BorderWidth = borders.IntValue
                };

                if (col == row.Last() && col.X + col.Width != sheetWidth.IntValue) {
                    tdata.Image.Width = sheetWidth.IntValue - col.X;
                }

                drawThumbTasks.Add(Task.Factory.StartNew(() => DrawThumb(tdata)));
                ++index;
            }
        }
        await Task.WhenAll(drawThumbTasks);

        // Save the sheet with the given Jpeg quality
        ImageCodecInfo? jpgEncoder = ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
        EncoderParameters myEncoderParameters = new(1);
        myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality.IntValue);

        DateTime endTime = DateTime.Now;
        TimeSpan duration = endTime - startTime;

        #endregion

        #region Finish

        Log.Information("---------------------------------------------------------------------------");
        Log.Information("Completed! It took {0}", duration);
        Log.Information("---------------------------------------------------------------------------");
        Log.Information("Sheet Size: {0} images, {1} rows, {2}x{3}px", imageCount, analyses.Count(r => r.Count > 0), sheetImage.Size.Width, sheetImage.Size.Height);
        Log.Information("Maximum Images per Row: {0}", analyses.Max(r => r.Count));
        Log.Information("Minimum Images per Row: {0}", analyses.Where(r => r.Count > 0).Min(r => r.Count));
        Log.Information("Output Quality: {0}%{1}", quality.IntValue, interpolate.BoolValue ? ", Using High-Quality Interpolation." : "");
        Log.Information("---------------------------------------------------------------------------");

        try {
            lock (ImageList) {
                int suffix = 0;
                string outPath = OutFilePath(suffix);
                Log.Information("Saving to {0}... ", outPath);
                if (File.Exists(outPath)) {
                    Log.Information("File exists. Attempting to delete... ");
                    try {
                        File.Delete(outPath);
                        Log.Information("Deleted.");
                    } catch (IOException ioEx) {
                        Log.Information("can't delete: {0}", ioEx.Message);
                        while (File.Exists(outPath)) {
                            outPath = OutFilePath(++suffix);
                            Log.Information("Trying a new output file name: {0}", outPath);
                        }
                    }
                }
                if (jpgEncoder != null) {
                    string? dir = Path.GetDirectoryName(OutFilePath(suffix));
                    if (dir != null && !Directory.Exists(dir)) {
                        Log.Information("Creating Directory: {0}", dir);
                        Directory.CreateDirectory(dir);
                    }
                    sheetImage.Save(OutFilePath(suffix), jpgEncoder, myEncoderParameters);
                    Log.Information("Saved. Size: {0} KiB", new FileInfo(OutFilePath(suffix)).Length / 1024f);
                } else {
                    ErrorOccurred?.Invoke("JPEG Encoder not found.");
                }
            }
        } catch (System.Runtime.InteropServices.ExternalException e) {
            ErrorOccurred?.Invoke("Can't Save Sheet", e);
        } finally {
            Log.Information("---------------------------------------------------------------------------");
            // Clean up
            sheetImage.Dispose();
        }
        if (!noGui.BoolValue) {
            Log.Information("Exit on Complete: {0}", exitOnComplete.BoolValue);
        }
        return exitOnComplete.BoolValue;

        #endregion
    }

    /// <summary>
    /// Draw a thumbnail image on the contact sheet
    /// </summary>
    /// <param name="data">Data about the image and its position on the sheet</param>
    private void DrawThumb(DrawThreadObj data) {
        Image image;
        if (!preview.BoolValue) {
            image = new Bitmap(data.File);
        } else {
            image = new Bitmap(data.Image.Width, data.Image.Height);
        }

        Rectangle thumb = new(
            data.BorderWidth,
            data.BorderWidth,
            data.Image.Width - data.BorderWidth * 2,
            data.Image.Height - data.BorderWidth * 2);
        Image bmp = new Bitmap(data.Image.Width, data.Image.Height);

        // Load the image
        Graphics thumbG = Graphics.FromImage(bmp);

        // Only use the high-quality shit if specified
        if (interpolate.BoolValue) {
            thumbG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
        } else {
            thumbG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
        }

        if (preview.BoolValue) {
            thumbG.FillRectangle(Brushes.White, thumb);
            thumbG.DrawRectangle(Pens.LightGreen, thumb);
        } else {
            thumbG.DrawImage(image, thumb);
        }

        image.Dispose();


        // Draw image name labels
        if (data.FontSize > 0) {
            // Set label to file name, no extension
            string label = data.Image.FileName;
            label = label[..label.LastIndexOf(".")];
            // Determine label size
            Font font = new(FontFamily.GenericSansSerif, data.FontSize);
            SizeF labelSize = thumbG.MeasureString(label, font, data.Image.Width);
            // Center label at the bottom of the thumbnail
            RectangleF labelClip = new(
                (data.Image.Width - labelSize.Width) / 2,
                data.Image.Height - labelSize.Height,
                labelSize.Width,
                labelSize.Height);
            // Align text center
            StringFormat alignCenter = new() {
                Alignment = StringAlignment.Center
            };
            // Draw translucent black background
            thumbG.FillRectangle(new SolidBrush(Color.FromArgb(150, Color.Black)), labelClip);
            // Draw label
            thumbG.DrawString(label, font, Brushes.White, labelClip, alignCenter);
        }

        // Have to lock on the Graphics object
        // because two threads can't draw on it at the same time
        lock (graphicsLock) {
            data.Graphics.DrawImage(bmp, data.Image.Bounds);
        }

        // Clean up
        bmp.Dispose();
        thumbG.Dispose();

        // Send a limited number of progress updates to the listeners
        lock (progressLock) {
            // Update counters
            int resolution = 10;
            double progressFraction = ++drawnCount / (double)data.ImageTotal;
            int step = (int)Math.Floor(progressFraction * resolution);
            if (step > progressStep) {
                ++progressStep;
                // Send progress to listeners
                DrawProgressChanged?.Invoke(new DrawProgressEventArgs(drawnCount, data.ImageTotal, DateTime.Now - startTime));
            }
            // Output status to console
            Log.Information("({0:P1}) {1} ({2}/{3}) {4}",
                progressFraction, data.Image.FileName, data.Index, data.ImageTotal, data.Image.Bounds);
        }
    }

    /// <summary>
    /// Shift a thumbnail image from an end of one row to another on the contact sheet
    /// </summary>
    /// <param name="list">The list of rows of images</param>
    /// <param name="fromRow">The index of the source row</param>
    /// <param name="toRow">The index of the target row</param>
    private static void ShiftImage(List<List<ImageData>> list, int fromRow, int toRow) {
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

    /// <summary>
    /// Scale the images in a row to fit a new row width
    /// </summary>
    /// <param name="list">The list of images in the row</param>
    /// <param name="width">The new row width</param>
    /// <returns>The newly scaled row height</returns>
    private static int ScaleRow(List<ImageData> list, int width) {
        double maxImageHeight = 0;
        double rowHeight = 0;
        double factor;
        int rowWidth = 0;

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
            rowHeight = Math.Round(maxImageHeight * (width / (double)rowWidth));

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

    /// <summary>
    /// Get the minimum dimensions of the images in a row
    /// </summary>
    /// <param name="row">The list of images in a row</param>
    /// <returns>A <see cref="Size"/> containing the calculated minimum Height and Width</returns>
    private static Size MinDims(List<ImageData> row) {
        return row.Any() ? new Size(row.Min(img => img.Width), row.Min(img => img.Height)) : new Size();
    }

    /// <summary>
    /// Check if the supplied command-line arguments include a Help argument.
    /// If they do, print the Help message.
    /// </summary>
    /// <param name="args">The command-line arguments</param>
    /// <returns>Whether a Help argument was found and Help message was output.</returns>
    public bool Help(string[] args) {
        bool markDown = args.Contains("markdown");
        if (args.Any(a => helpStrings.Contains(a))) {
            if (markDown) {
                Console.WriteLine("| Parameter | Name | Type | Default | Description |");
                Console.WriteLine("| --------- | ---- | ---- | ------- | ----------- |");
            } else {
                Console.WriteLine("------ Parameters ------");
            }
            foreach (Param p in Params) {
                Console.Write(p.GetHelp(markDown));
                if (!markDown) {
                    Console.WriteLine();
                }
            }

            string sfile = markDown ? "| `-sfile` |" : "-sfile:";
            Console.WriteLine(@"{0} Settings file path {1}", sfile, markDown ? "| File Path | default.aspx | The path to a settings file. Can be absolute or relative. |" : string.Empty);
            string help = markDown ? "| `-help` |" : "-help:";
            Console.WriteLine("{0} View help message (no value required) {1}", help, markDown ? "| None | N/A | Show a help message on the command line with parameter documentation. |" : string.Empty);

            return true;
        }
        return false;
    }

    public void Dispose() {
        fileSource?.Dispose();
    }
}
