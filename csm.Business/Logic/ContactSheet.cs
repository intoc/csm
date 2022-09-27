using csm.Business.Models;
using Serilog;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Path = System.IO.Path;

namespace csm.Business.Logic;

public delegate void SourceChangedEventHandler(string? path);
public delegate void DrawProgressEventHandler(ContactSheet sender, ProgressEventArgs args);
public delegate void LoadProgressEventHandler(ContactSheet sender, ProgressEventArgs args);
public delegate void SettingsChangedEventHandler(SettingsChangedEventArgs args);
public delegate void ImageListChangedEventHandler();
public delegate void ExceptionEventHandler(string message, bool isFatal, Exception? e = null);

/// <summary>
/// Creates contact sheets
/// </summary>
public sealed class ContactSheet : IDisposable {

    #region Private Constants

    private static readonly string defaultFilePattern = @"\.(jpg|jpeg|png)$";
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

    public bool FirstLoadComplete => !_firstLoadIncomplete;

    public double LoadProgress { get; private set; }

    public double DrawProgress { get; private set; }

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
                fileSource.LoadProgressChanged += (e) => {
                    Log.Debug("Archive extraction progress: {0:P1}", e.Percentage);
                    LoadProgressChanged.Invoke(this, e);
                };
            } catch (Exception ex) {
                ErrorOccurred?.Invoke("Can't load source path.", true, ex);
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

    /// <summary>
    /// The path to the directory containing the source image files
    /// </summary>
    public string? SourceImageFileDirectoryPath => fileSource?.ImageFileDirectoryPath;
    

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
    /// Fired when the file loading progress changes
    /// </summary>
    public event LoadProgressEventHandler LoadProgressChanged = delegate { };

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
    private readonly BoolParam labels;
    private readonly BoolParam noGui;
    private readonly BoolParam openOutputDirectoryOnComplete;
    private readonly BoolParam preview;
    private readonly FileParam coverFile;
    private readonly IntParam borders;
    private readonly IntParam columns;
    private readonly IntParam headerFontSize;
    private readonly IntParam labelFontSize;
    private readonly IntParam maxCoverWithPercent;
    private readonly IntParam minDimThumbnail;
    private readonly IntParam minDimInput;
    private readonly IntParam quality;
    private readonly IntParam sheetWidth;
    private readonly StringParam coverPattern;
    private readonly StringParam filePattern;
    private readonly StringParam headerTitle;
    private readonly StringParam outputFilePath;

    // Draw status variables
    private DateTime startTime;
    private int drawnCount, progressStep;
    private readonly object graphicsLock = new();
    private bool _firstLoadIncomplete;

    #endregion

    /// <summary>
    /// Create a contact sheet instance
    /// </summary>
    /// <param name="fileSourceBuilder">The file source builder</param>
    /// <param name="hasGui">If false, removes the -nogui parameter</param>
    public ContactSheet(IFileSourceBuilder fileSourceBuilder, bool hasGui = true) {

        _fileSourceBuilder = fileSourceBuilder;
        _firstLoadIncomplete = true;

        // Set parameter fields and defaults

        #region General Parameters

        noGui = new BoolParam("-nogui", false) {
            LoadFromSettings = false
        };

        filePattern = new StringParam("-fregx", defaultFilePattern, "Regex") {
            MaxChars = 20
        };

        minDimInput = new IntParam("-mindiminput", 0, "px") {
            MinVal = 0,
            MaxVal = 99999
        };

        columns = new IntParam("-cols", DEFAULT_COLUMNS) {
            MinVal = 1,
            MaxVal = 50,
            IsSmall = true

        };

        sheetWidth = new IntParam("-width", DEFAULT_WIDTH, "px") {
            MinVal = 1,
            MaxVal = 99999
        };

        minDimThumbnail = new IntParam("-mindim", 0, "px") {
            MinVal = 0,
            MaxVal = sheetWidth.MaxVal
        };

        borders = new IntParam("-border", 0, "px") {
            MinVal = 0,
            MaxVal = 50,
            IsSmall = true
        };

        quality = new IntParam("-qual", DEFAULT_QUALITY, "%") {
            MinVal = 0,
            MaxVal = 100,
            IsSmall = true
        };

        preview = new BoolParam("-preview", false);
        exitOnComplete = new BoolParam("-exit", false);

        outputFilePath = new StringParam("-outfile", "ContactSheet.jpg", "File Path") {
            MaxChars = 25
        };
        openOutputDirectoryOnComplete = new BoolParam("-openoutdir", true);

        var generalParams = new NullParam("General");
        generalParams.AddSubParam(filePattern);
        generalParams.AddSubParam(minDimInput);
        generalParams.AddSubParam(sheetWidth);
        generalParams.AddSubParam(columns);
        generalParams.AddSubParam(minDimThumbnail);
        generalParams.AddSubParam(borders);
        generalParams.AddSubParam(quality);
        generalParams.AddSubParam(exitOnComplete);
        generalParams.AddSubParam(openOutputDirectoryOnComplete);
        generalParams.AddSubParam(preview);
        generalParams.AddSubParam(outputFilePath);

        #endregion

        #region Label Parameters

        labels = new BoolParam("-labels", false);
        labelFontSize = new IntParam("-lsize", 8, "pt") {
            MinVal = 0,
            MaxVal = 136,
            IsSmall = true
        };
        labels.AddSubParam(labelFontSize);

        #endregion

        #region Header Parameters

        header = new BoolParam("-header", false);
        headerFontSize = new IntParam("-hsize", 12, "pt") {
            MinVal = 0,
            MaxVal = 180,
            IsSmall = true
        };
        headerBold = new BoolParam("-hbold", false);
        headerTitle = new StringParam("-htitle", "Title", "Words") {
            MaxChars = 20,
            LoadFromSettings = false
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
            LoadFromSettings = false
        };
        cover.AddSubParam(coverPattern);
        cover.AddSubParam(coverFile);
        fillCoverGap = new BoolParam("-cfill", false);
        cover.AddSubParam(fillCoverGap);
        maxCoverWithPercent = new IntParam("-cmaxw", 75) {
            MinVal = 0, MaxVal = 100, Units = "%", IsSmall = true
        };
        cover.AddSubParam(maxCoverWithPercent);

        #endregion

        #region Event Subscriptions

        // Setup all instances where a file list reload is required
        filePattern.ParamChanged += async (path) => await LoadFileList(path);
        SourceChanged += async (path) => {
            Log.Information("Source set to {0}", path);
            imageSet.Source = fileSource;
            headerTitle.ParseVal(fileSource?.Name);
            Log.Information("Directory Name -> Header Title: {0}", headerTitle.ParsedValue);
            await LoadFileList();
            if (cover.BoolValue) {
                await GuessCover(true);
            }
            // First loading has finished, stop blocking DrawAndSave
            _firstLoadIncomplete = false;
        };
        LoadProgressChanged += (source, e) => {
            LoadProgress = e.Percentage;
        };

        // Setup all instances where a image list refresh is required without a full reload
        coverFile.ParamChanged += RefreshImageList;
        minDimInput.ParamChanged += RefreshImageList;
        outputFilePath.ParamChanged += RefreshImageList;

        cover.ParamChanged += async (p) => await HandleShowCoverChanged();
        coverPattern.ParamChanged += async (p) => await HandleCoverPatternChanged();

        #endregion

        // Load top-level params into externaly visible Param list (ordered)
        Params = new List<Param> {
            generalParams,
            header,
            labels,
            cover
        };

        if (hasGui) {
            Params.Insert(0, noGui);
        } else {
            noGui.BoolValue = true;
        }
    }

    private async Task HandleShowCoverChanged() {
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

    private async Task HandleCoverPatternChanged() {
        if (!cover.BoolValue || fileSource == null) {
            return;
        }
        if (await GuessCover(true)) {
            RefreshImageList(cover);
        }
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

        if (suffix > 0) {
            // Insert _suffix before extension
            path = Regex.Replace(path, @"\.([^\.]*)$", $"_{suffix}.$1");
        }

        path = path.Replace("{title}", headerTitle.Value);

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
                ErrorOccurred?.Invoke($"Settings file does not exist ({SettingsFile}).", false);
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
            ErrorOccurred?.Invoke($"Couldn't load {SettingsFile}!", false, e);
            SettingsChanged?.Invoke(new SettingsChangedEventArgs(filename, "Load Failed", false));
        }
        return false;
    }

    /// <summary>
    /// Load Params from command-line arguments
    /// </summary>
    /// <param name="args">Command-line arguments</param>
    public void LoadParamsFromCommandLine(IEnumerable<string> args) {

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
    /// Load parameter values from another <see cref="ContactSheet"/>
    /// </summary>
    /// <param name="other">The other sheet</param>
    public void LoadParamsFromSheet(ContactSheet other) {
        foreach (var param in Params) {
            param.Load(other.Params);
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
        string listRegex = filePattern.Value ?? defaultFilePattern;
        string coverRegex = coverPattern.Value ?? coverRegexes;
        return await imageSet.GuessFile(coverFile, listRegex, coverRegex, force);
    }

    /// <summary>
    /// Load the file list and image information from the source directory if it's set
    /// </summary>
    /// <param name="p">The <see cref="Param"/> that caused the need</param>
    public async Task LoadFileList(Param? p = null) {
        if (p != null) {
            Log.Information("Reloading file list due to change in {0}", p.CmdParameter);
        }
        await imageSet.LoadImageListAsync(filePattern.ParsedValue ?? defaultFilePattern, minDimInput.IntValue, Path.GetFileName(OutFilePath()), cover.BoolValue ? coverFile.FileName : null);
        ImageListChanged?.Invoke();
    }

    /// <summary>
    /// Refresh the image data and whether they should be included in the output based on parameter changes
    /// </summary>
    /// <param name="p">The <see cref="Param"/> that caused the need for the refresh</param>
    public void RefreshImageList(Param? p = null) {
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
    /// Run the image analysis and contact sheet creation process
    /// </summary>
    /// <returns>Whether the process is set to exit on complete</returns>
    public async Task<bool> DrawAndSave(bool waitForLoad = false) {
        if (string.IsNullOrEmpty(Source)) {
            ErrorOccurred?.Invoke("No/invalid Source selected!", true);
            return false; // Don't exit the GUI
        }

        IEnumerable<ImageData> images;
        List<List<ImageData>> analyses = new() {
            new List<ImageData>()
        };
        ImageData? coverImageData = null;
        Image<Rgba32>? headerImage = null;
        Image<Rgba32> sheetImage;
        int sheetHeight = 0;
        int imageCount;
        int rowIndex = 0;
        int maxRowHeight = 0;
        int rowHeight;
        int fileIndex = 1;
        bool drawCover = cover.BoolValue;
        bool fillGap = drawCover && fillCoverGap.BoolValue;
        int borderWidth = borders.IntValue;

        // Get fonts
        FontCollection fonts = new();
        fonts.Add("Fonts/OpenSans-Bold.ttf");
        FontFamily fontFamily = fonts.Add("Fonts/OpenSans-Regular.ttf");

        // Wait for the image list to be ready

        while (waitForLoad) {
            Log.Debug("DrawAndSave waiting for images to be ready...");
            if (imageSet.Loaded && !_firstLoadIncomplete) {
                break;
            }
            Thread.Sleep(250);
        }

        Log.Debug("DrawAndSave starting");

        lock (imageSet.Images) {

            images = imageSet.Images.Where(i => i.Include);
            imageCount = images.Count();

            if (imageCount == 0) {
                ErrorOccurred?.Invoke($"No valid/selected {filePattern.ParsedValue} Images in {Source}!", true);
                return false; // Don't exit the GUI
            }

            // Mark the start time
            startTime = DateTime.Now;

            #region Cover Setup

            // Avoid stupidness
            if (drawCover && coverFile.File == null) {
                ErrorOccurred?.Invoke("Can't draw the cover because there is no cover file set.", false);
                drawCover = false;
                fillGap = false;
            }

            // Analyze the cover
            if (drawCover && fileSource != null && coverFile.File != null) {
                // Begin image analysis
                Log.Information("Analyzing cover...");
                coverImageData = new ImageData(coverFile.File.Path);
                fileSource.LoadImageDimensions(coverImageData);
                double maxCoverImageScaleForGap = Math.Round(((float)maxCoverWithPercent.IntValue / 100) * columns.IntValue) / columns.IntValue;

                if (coverImageData.Width >= (sheetWidth.IntValue * maxCoverImageScaleForGap)) {
                    // We want a gap right? Make the cover smaller.
                    Log.Information("Cover image is too large. Reducing size to {0:0.00}% of sheet width.", maxCoverImageScaleForGap * 100);
                    double scaleFactor = (sheetWidth.IntValue * maxCoverImageScaleForGap) / coverImageData.Width;
                    coverImageData.Scale(scaleFactor);
                }

                if (coverImageData.Width < sheetWidth.IntValue && !fillGap) {
                    // Center images smaller than the sheet width
                    Log.Information("Centering cover image.");
                    coverImageData.X = (sheetWidth.IntValue - coverImageData.Width) / 2;
                }
                if (coverImageData.Width > sheetWidth.IntValue) {
                    // Scale the image down to sheet width
                    Log.Information("Cover image is too large. Reducing size to fit sheet width.");
                    coverImageData.Scale((double)sheetWidth.IntValue / coverImageData.Width);
                }
                Log.Information("Cover analysis complete. Fill gap: {0}, cover bounds: {1}", fillGap, coverImageData.Bounds);
            }

            #endregion

            #region Analysis Pass 1 - Build initial rows scaled to width

            // Begin image analysis
            Log.Information("Pass 1: Analyzing {0} images", imageCount);


            var maxWidth = images.Max(i => i.Width);

            // First pass, add the same number of images to each row,
            // scale to width, record maximum row height
            foreach (ImageData data in images) {
                // Add image to row
                analyses[rowIndex].Add(data);

                // Check for any images that didn't get dimensions during the initial loading process
                // We'll show a placeholder later if it still fails to load
                if (data.Width == 0) {
                    data.InitSize(new Size(maxWidth, maxWidth));
                }

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
            Log.Information("Added {0} rows, maxRowHeight: {1}", analyses.Count, maxRowHeight);

            #endregion

            #region Analysis Pass 2 - Scale and Shift

            Log.Information("Pass 2: Analyzing {0} Rows to normalize row height", analyses.Count);

            // Second pass tries to make all rows of similar height by
            // shifting images and rescaling rows.
            Size minRowDims;
            bool done = false;
            int rowWidth;
            Point curPoint = new(0, 0);
            bool inGap = fillGap;

            for (rowIndex = 0; !done; ++rowIndex) {

                if (inGap && coverImageData != null) {
                    // Row space = cover gap
                    rowWidth = sheetWidth.IntValue - coverImageData.Width;
                    curPoint.X = coverImageData.Width;
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
                int overFlow = curPoint.Y + rowHeight - coverImageData?.Height ?? 0;
                if (inGap && coverImageData != null && (overFlow > 0 || rowIndex + 1 == analyses.Count || analyses[rowIndex + 1].Count == 0)) {
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
                        double h1 = coverImageData.Height;
                        double w1 = coverImageData.Width;
                        double h2 = analyses[rowIndex][0].Y + analyses[rowIndex][0].Height;
                        double w2 = rowWidth;

                        double f1 = h2 * sheetWidth.IntValue / (h1 * w2 + h2 * w1);

                        coverImageData.Scale(f1);

                        curPoint.Y = 0;
                        for (int i = 0; i <= rowIndex; ++i) {
                            // Move images to the start of the new gap
                            analyses[i][0].X = coverImageData.Width;
                            analyses[i][0].Y = curPoint.Y;
                            // Scale row width to the new gap
                            rowHeight = ScaleRow(analyses[i], sheetWidth.IntValue - coverImageData.Width);
                            Log.Information("In Gap, Final Scaling, Row {0}", i);
                            // Next row
                            curPoint.Y += rowHeight;
                        }
                    } else {
                        // No gap images. Display the cover normally.
                        coverImageData.X = sheetWidth.IntValue / 2 - coverImageData.Width / 2;
                        fillGap = false;
                        ErrorOccurred?.Invoke("Cover gap fill failed, image is too small. Centering.", false);
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
                    bool lastRowInGap = rowIndex > 0 && analyses[rowIndex - 1].Last().Y < (coverImageData?.Height ?? 0);
                    while (!lastRowInGap && rowHeight > lastRowHeight * 2 && analyses[rowIndex - 1].Count > 1) {
                        ShiftImage(analyses, rowIndex - 1, rowIndex);
                        lastRowHeight = ScaleRow(analyses[rowIndex - 1], lastRowWidth);
                        analyses[rowIndex][0].X = curPoint.X;
                        analyses[rowIndex][0].Y += lastRowHeight;
                        rowHeight = ScaleRow(analyses[rowIndex], rowWidth);
                        Log.Information("Row {0} Rescaled, {1} Images. Height: {2}px", rowIndex - 1, analyses[rowIndex - 1].Count, lastRowHeight);
                    }
                    done = true;
                }

                if (rowIndex >= 0) {
                    Log.Information("Row {0}: {1} Images. Height: {2}px. Y: {3}", rowIndex, analyses[rowIndex].Count, rowHeight, analyses[rowIndex][0].Y);
                }
            }

            // Remove empty rows
            for (int i = 0; i < analyses.Count; ++i) {
                if (!analyses[i].Any()) {
                    analyses.RemoveAt(i);
                }
            }

            // Make sure rows don't overlap or have gaps between them
            curPoint.Y = analyses.First().First().Y;
            foreach (List<ImageData> row in analyses) {
                foreach (ImageData im in row) {
                    im.Y = curPoint.Y;
                }
                curPoint.Y += row[0].Height;
            }

            #endregion

            #region Build Header Image

            // Draw the header image first since we can't extend the canvas during drawing

            if (header.BoolValue) {
                Log.Information("Building the header {0} stats", headerStats.BoolValue ? "with" : "without");
                headerImage = new Image<Rgba32>(sheetWidth.IntValue, sheetWidth.IntValue);
                string headerText = headerTitle.ParsedValue ?? string.Empty;
                int padding = 5;
                int headerWidth = sheetWidth.IntValue - (padding * 2);
                Log.Information("Title{1}: {0}", headerText, headerBold.BoolValue ? " (bold)" : string.Empty);

                // Build title font
                Font titleFont = fontFamily.CreateFont(headerFontSize.IntValue, headerBold.BoolValue ? FontStyle.Bold : FontStyle.Regular);
                TextOptions titleTextOptions = new(titleFont) {
                    WrappingLength = headerWidth,
                    Origin = new(padding, 0)
                };
                FontRectangle headerFontRect = TextMeasurer.Measure(headerText, titleTextOptions);
                int headerHeight = (int)Math.Ceiling(headerFontRect.Height + padding * 2);

                headerImage.Mutate(headerImageContext => {

                    // Draw title text
                    headerImageContext
                        .Fill(Color.Black)
                        .DrawText(titleTextOptions, headerText, Color.White);

                    // Stats
                    if (headerStats.BoolValue) {
                        // Build stats font
                        Font statsFont = fontFamily.CreateFont(14, FontStyle.Regular);
                        TextOptions statsTextOptions = new(statsFont) {
                            WrappingLength = headerWidth,
                            Origin = new(padding, headerFontRect.Height + 10)
                        };

                        // Determine largest image
                        var maxSize = analyses
                            .Where(x => x.Count > 0)
                            .MaxBy(row => row.Max(img => img.OriginalSize.Height))?
                            .MaxBy(img => img.OriginalSize.Height)?.OriginalSize ?? default;

                        // Determine how much space the stats will take up in the header
                        string stats = $"{imageCount} images. Maximum dimensions {maxSize.Width}x{maxSize.Height}px.";
                        if (fileSource != null) {
                            stats += $" {fileSource.Size}";
                        }
                        var statsFontSize = TextMeasurer.Measure(stats, statsTextOptions);
                        int statsHeight = (int)statsFontSize.Height + 10;
                        headerHeight = (int)Math.Ceiling(headerFontRect.Height + statsHeight + padding);
                        Log.Information("Stats: {0}", stats);

                        // Pre-draw stats
                        int statsTop = (int)headerFontRect.Height + padding;
                        headerImageContext
                            .DrawLines(new Pen(Color.DarkSlateGray, 1), new PointF(padding, statsTop), new PointF(sheetWidth.IntValue - padding, statsTop))
                            .DrawText(statsTextOptions, stats, Color.White);
                    }

                    // Get rid of extra height
                    headerImageContext.Crop(sheetWidth.IntValue, headerHeight);
                    Log.Information("Height: {0}px", headerHeight);
                });
            }

            #endregion

            #region Adjust Y values

            // Determine where we should start drawing images vertically
            int newTop = headerImage?.Height ?? 0;
            if (newTop > 0) {
                newTop -= borderWidth;

                // Adjust cover Y position
                if (coverImageData != null) {
                    coverImageData.Y = newTop;
                }
            }

            // Adjust image Y positions
            foreach (var image in analyses.SelectMany(row => row.Select(image => image))) {
                image.Y += newTop;
                if (!fillGap) {
                    image.Y += coverImageData?.Height ?? 0;
                }
            }

            #endregion

            #region Calculate Borders

            Log.Information("Pass 3: Calculating borders (Width {0}px) and accounting for rounding error", borderWidth);

            // Calculate row height scale factor
            ImageData last = analyses.Last().First();
            sheetHeight = last.Y + last.Height + borderWidth;
            double vScale = 1.0;
            int borderSum = borderWidth * (analyses.Count + 1);
            double reduceImageHeight = (double)borderSum / (sheetHeight - analyses.First().First().Y);
            vScale = 1.0 - reduceImageHeight;
            int top = analyses.First().First().Y;

            foreach (List<ImageData> row in analyses) {

                var firstRowImage = row.First();
                var lastRowImage = row.Last();

                // Calculate image with scale factor
                double hScale = 1.0;
                borderSum = borderWidth * (row.Count + 1);
                double reduceImageWidth = (double)borderSum / (sheetWidth.IntValue - firstRowImage.X);
                hScale = 1.0 - reduceImageWidth;
                int leftEdge = firstRowImage.X;
                int lastImageRightEdge = firstRowImage.X;

                // Scale and shift images to create borders
                foreach (ImageData image in row) {
                    image.Width = (int)Math.Round(image.Width * hScale);
                    image.Height = (int)Math.Round(image.Height * vScale);
                    image.X = lastImageRightEdge + borderWidth;
                    image.Y = top + borderWidth;
                    lastImageRightEdge = image.X + image.Width;
                }

                // Correct rounding error Horizontally
                // Calculate the error, shift all images to center, and adjust the left and right edges to align
                int xError = sheetWidth.IntValue - (lastRowImage.X + lastRowImage.Width + borderWidth);
                int xCorrection = (int)Math.Round((double)xError / 2);
                foreach (ImageData image in row) {
                    image.X += xCorrection;
                }
                int shift = borderWidth - (firstRowImage.X - leftEdge);
                firstRowImage.X = leftEdge + borderWidth;
                firstRowImage.Width -= shift;
                lastRowImage.Width = sheetWidth.IntValue - lastRowImage.X - borderWidth;

                top = firstRowImage.Y + firstRowImage.Height;
            }

            // Correct cover height after thumbnail borders added
            if (coverImageData != null) {
                coverImageData.Pad(borderWidth);
                if (fillGap) {
                    foreach (var row in analyses) {
                        int leftEdge = row.First().X;
                        if (leftEdge == borderWidth) {
                            coverImageData.Height = (row.First().Y - borderWidth) - coverImageData.Y;
                            break;
                        }
                    }
                }
            }

            // Calculate final sheet height
            sheetHeight = last.Y + last.Height + borderWidth;
        }

        #endregion

        #region Drawing

        // Create the output image
        sheetImage = new Image<Rgba32>(sheetWidth.IntValue, sheetHeight);

        // Draw the sheet
        sheetImage.Mutate(sheetContext => {

            // We have to make the background white because the header text looks bad on a black background
            sheetContext.Fill(Color.Black);

            // Draw the the header
            if (headerImage != null) {
                // Draw the header on the sheet
                Log.Information("Drawing header. {0}", headerImage.Bounds());
                sheetContext.DrawImage(headerImage, 1);
            }

            // Draw the cover
            if (coverImageData != null) {
                Log.Information("Drawing cover {0}. {1}", Path.GetFileName(coverFile.Path), coverImageData.Bounds);
                if (preview.BoolValue) {
                    sheetContext.Fill(Brushes.Solid(Color.White), coverImageData.Bounds);
                    sheetContext.DrawText(
                        "COVER", fontFamily.CreateFont(14, FontStyle.Bold), Color.Black,
                        new PointF(coverImageData.X + coverImageData.Width / 2, coverImageData.Y + coverImageData.Height / 2));
                } else {
                    using var coverImage = Image.Load(coverImageData.File);
                    coverImage.Mutate(coverContext => {
                        coverContext.Resize(coverImageData.Width, coverImageData.Height);
                    });
                    sheetContext.DrawImage(coverImage, coverImageData.Origin, 1);
                }
            }
        });


        // Draw the thumbnail images
        int index = 1;
        Log.Information("Drawing sheet. {0}", sheetImage.Bounds());
        drawnCount = 0;
        progressStep = 0;
        IList<Task> drawThumbTasks = new List<Task>();
        Font labelFont = fontFamily.CreateFont(labels.BoolValue ? labelFontSize.IntValue : 0, FontStyle.Bold);

        foreach (ImageData image in analyses.SelectMany(row => row.Select(image => image))) {

            // Create info for threaded load/draw operation
            ThumbnailData tdata = new(image, sheetImage, labelFont) {
                Index = index++,
                ImageTotal = imageCount
            };

            drawThumbTasks.Add(Task.Factory.StartNew(() => DrawThumb(tdata)));
        }

        await Task.WhenAll(drawThumbTasks);

        DateTime endTime = DateTime.Now;
        TimeSpan duration = endTime - startTime;

        #endregion

        #region Finish

        // Update list watchers so they see the new sizes
        ImageListChanged?.Invoke();

        Log.Information("---------------------------------------------------------------------------");
        Log.Information("Completed! It took {0}", duration);
        Log.Information("---------------------------------------------------------------------------");
        Log.Information("Sheet Size: {0} images, {1} rows, {2}x{3}px", imageCount, analyses.Count(r => r.Count > 0), sheetImage.Width, sheetImage.Height);
        Log.Information("Maximum Images per Row: {0}", analyses.Max(r => r.Count));
        Log.Information("Minimum Images per Row: {0}", analyses.Where(r => r.Count > 0).Min(r => r.Count));
        Log.Information("Output Quality: {0}%", quality.IntValue);
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

                string? dir = Path.GetDirectoryName(OutFilePath(suffix));
                if (dir != null && !Directory.Exists(dir)) {
                    Log.Information("Creating Directory: {0}", dir);
                    Directory.CreateDirectory(dir);
                }
                sheetImage.SaveAsJpeg(OutFilePath(suffix), new JpegEncoder {
                    Quality = quality.IntValue
                });
                Log.Information("Saved. Size: {0} KiB", new FileInfo(OutFilePath(suffix)).Length / 1024f);

            }
        } catch (System.Runtime.InteropServices.ExternalException e) {
            ErrorOccurred?.Invoke("Can't Save Sheet", true, e);
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
    private void DrawThumb(ThumbnailData data) {
        Image image;
        Size size = new(data.Image.Width, data.Image.Height);
        if (preview.BoolValue) {
            image = new Image<Rgba32>(size.Width, size.Height);
        } else {
            try {
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
                Log.Error("{0} Image load failed, drawing placeholder.", data.Image.FileName);
            }
        }

        image.Mutate(imageContext => {

            if (preview.BoolValue) {
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
                string label = Path.GetFileNameWithoutExtension(data.Image.FileName);
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
                });

                // Draw the label image on the thumbnail. Center at the bottom.
                Point labelCoords = new((int)(size.Width - labelSize.Width) / 2, (int)(size.Height - labelSize.Height));
                imageContext.DrawImage(labelImage, labelCoords, 0.5f);
            }
        });

        lock (graphicsLock) {
            data.SheetImage.Mutate(context => {
                // Draw the thumbnail
                context.DrawImage(image, data.Image.Origin, 1.0f);
            });
        }

        // Clean up
        image.Dispose();

        // Update counters
        int resolution = 10;
        Interlocked.Increment(ref drawnCount);
        DrawProgress = drawnCount / (double)data.ImageTotal;
        int step = (int)Math.Floor(DrawProgress * resolution);

        // Send a limited number of progress updates to the listeners
        if (step > progressStep) {
            ++progressStep;
            // Send progress to listeners
            DrawProgressChanged?.Invoke(this, new ProgressEventArgs(drawnCount, data.ImageTotal, DateTime.Now - startTime));
        }
        // Output status to console
        Log.Information("({0:P1}) {1} ({2}/{3}) {4}",
            DrawProgress, data.Image.FileName, data.Index, data.ImageTotal, data.Image.Bounds);
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
        int maxImageHeight = 0;
        int rowHeight = 0;
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
            rowHeight = (int)Math.Round(maxImageHeight * (width / (double)rowWidth));

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
            rowHeight = (int)Math.Round(list[0].OriginalSize.Height * (width / (double)rowWidth));

            // Scale images to calculated height
            list[0].ScaleToHeight(rowHeight);
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
