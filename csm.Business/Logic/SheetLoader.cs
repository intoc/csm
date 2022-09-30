using csm.Business.Models;
using Serilog;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Path = System.IO.Path;

namespace csm.Business.Logic;

/// <summary>
/// Creates contact sheets
/// </summary>
public sealed class SheetLoader : IDisposable {

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
    public IList<ImageData>? ImageList => _imageSet?.Images;

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
            return _imageSet?.Source.FullPath;
        }
    }

    public async Task SetSourcePath(string path) {
       try {
            var fileSource = _fileSourceBuilder.Build(path);
            if (_imageSet == null) {
                _imageSet = new ImageSet(fileSource);
                _imageSet.LoadProgressChanged += (e) => {
                    LoadProgress = e.Percentage;
                    LoadProgressChanged(this, e);
                };
                _imageSet.LoadCompleted += (e) => LoadCompleted(this, e);
            } else {
                await _imageSet.SetSource(fileSource);
            }
        } catch (Exception ex) {
            ErrorOccurred?.Invoke("Can't load source path.", true, ex);
        }
    }

    /// <summary>
    /// The path to the directory containing the source image files
    /// </summary>
    public string? SourceImageFileDirectoryPath => _imageSet?.Source.ImageFileDirectoryPath;


    #endregion

    #region Public Events

    /// <summary>
    /// Fired when the progress of drawing the output contact sheet changes
    /// </summary>
    public event Action<SheetLoader, ProgressEventArgs> DrawProgressChanged = delegate { };

    /// <summary>
    /// Fired when there is a change to the contents of the image list
    /// </summary>
    public event Action<SheetLoader, bool> ImageListChanged = delegate { };

    /// <summary>
    /// Fired when the settings file changes and is loaded
    /// </summary>
    public event Action<SettingsChangedEventArgs> SettingsChanged = delegate { };

    /// <summary>
    /// Fired when the source directory is changed
    /// </summary>
    public event Action<SheetLoader, IFileSource> LoadCompleted = delegate { };

    /// <summary>
    /// Fired when the file loading progress changes
    /// </summary>
    public event Action<SheetLoader, ProgressEventArgs> LoadProgressChanged = delegate { };


    /// <summary>
    /// Fired when an exception occurred
    /// </summary>
    public event ExceptionEventHandler ErrorOccurred = delegate { };
    public delegate void ExceptionEventHandler(string message, bool isFatal, Exception? e = null);

    #endregion

    #region Private Fields

    private IImageSet? _imageSet;
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
    private readonly IntParam maxCoverWidthPercent;
    private readonly IntParam minDimThumbnail;
    private readonly IntParam minDimInput;
    private readonly IntParam quality;
    private readonly IntParam sheetWidth;
    private readonly StringParam coverPattern;
    private readonly StringParam filePattern;
    private readonly StringParam headerTitle;
    private readonly StringParam outputFilePath;

    private readonly object _logLock = new();
    private bool _firstLoadIncomplete;
    private bool _isDisposed = false;

    #endregion

    /// <summary>
    /// Create a contact sheet instance
    /// </summary>
    /// <param name="fileSourceBuilder">The file source builder</param>
    /// <param name="hasGui">If false, removes the -nogui parameter</param>
    public SheetLoader(IFileSourceBuilder fileSourceBuilder, bool hasGui = true) {

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
        maxCoverWidthPercent = new IntParam("-cmaxw", 75) {
            MinVal = 0, MaxVal = 100, Units = "%", IsSmall = true
        };
        cover.AddSubParam(maxCoverWidthPercent);

        #endregion

        #region Event Subscriptions

        // Setup all instances where a file list reload is required
        filePattern.ParamChanged += async (path) => await LoadFileList(path);
        LoadCompleted += async (sheet, source) => {
            Log.Debug("Source set to {0}", sheet.Source);
            headerTitle.ParseVal(_imageSet?.Source.Name);
            Log.Debug("Directory Name -> Header Title: {0}", headerTitle.ParsedValue);
            await LoadFileList();
            if (cover.BoolValue) {
                await GuessCover(true);
            }
            // First loading has finished, stop blocking DrawAndSave
            _firstLoadIncomplete = false;
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
        if (_imageSet == null || _isDisposed) {
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
        if (!cover.BoolValue || _imageSet == null || _isDisposed) {
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
        if (_imageSet == null) {
            return string.Empty;
        }

        // Use the parent directory
        var outputDirectory = _imageSet?.Source.ParentDirectoryPath;
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
    /// Load parameter values from another <see cref="SheetLoader"/>
    /// </summary>
    /// <param name="other">The other sheet</param>
    public void LoadParamsFromSheet(SheetLoader other) {
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
        if (_isDisposed || _imageSet == null) {
            return false;
        }
        string listRegex = filePattern.Value ?? defaultFilePattern;
        string coverRegex = coverPattern.Value ?? coverRegexes;
        return await _imageSet.GuessFile(coverFile, listRegex, coverRegex, force);
    }

    /// <summary>
    /// Load the file list and image information from the source directory if it's set
    /// </summary>
    /// <param name="p">The <see cref="Param"/> that caused the need</param>
    public async Task LoadFileList(Param? p = null) {
        if (_isDisposed || _imageSet == null) {
            return;
        }
        if (p != null) {
            Log.Information("Reloading file list due to change in {0}", p.CmdParameter);
        }
        bool filesAddedOrRemoved = await _imageSet.LoadImageListAsync(filePattern.ParsedValue ?? defaultFilePattern, minDimInput.IntValue, Path.GetFileName(OutFilePath()), cover.BoolValue ? coverFile.FileName : null);
        ImageListChanged?.Invoke(this, filesAddedOrRemoved);
    }

    /// <summary>
    /// Refresh the image data and whether they should be included in the output based on parameter changes
    /// </summary>
    /// <param name="p">The <see cref="Param"/> that caused the need for the refresh</param>
    public void RefreshImageList(Param? p = null) {
        if (!(_imageSet?.Images.Any() ?? false) || _isDisposed) {
            return;
        }
        if (p != null) {
            Log.Debug("Refreshing image list due to change in {0}", p.Desc);
        }
        _imageSet.RefreshImageList(minDimInput.IntValue, Path.GetFileName(OutFilePath()), cover.BoolValue ? coverFile.FileName : null);
        ImageListChanged?.Invoke(this, false);
    }

    /// <summary>
    /// Run the image analysis and contact sheet creation process
    /// </summary>
    /// <returns>Whether the process is set to exit on complete</returns>
    public async Task<bool> DrawAndSave() {

        Log.Debug("SheetLoader.DrawAndSave starting");

        // Check for blockers
        if (_isDisposed) {
            Log.Debug("DrawAndSave called on disposed sheet, cancelling...");
            return false;
        }
        if (_imageSet == null) {
            ErrorOccurred?.Invoke("No/invalid Source selected!", true);
            return false; // Don't exit the GUI
        }

        // Get fonts
        FontCollection fonts = new();
        fonts.Add("Fonts/OpenSans-Bold.ttf");
        FontFamily fontFamily = fonts.Add("Fonts/OpenSans-Regular.ttf");

        // Wait for the image list to be ready
        bool notLoaded = true;
        while (notLoaded) {
            if (_imageSet.Loaded && !_firstLoadIncomplete) {
                notLoaded = false;
            }
            Log.Debug("DrawAndSave waiting for images to be ready...");
            Thread.Sleep(250);
            if (_isDisposed) {
                return false;
            }
        }

        SheetBuilder sheet = new(_imageSet) {
            BorderWidth = borders.IntValue,
            CoverFile = coverFile.File,
            DrawCover = cover.BoolValue,
            DrawHeader = header.BoolValue,
            DrawHeaderStats = headerStats.BoolValue,
            DrawLabels = labels.BoolValue,
            FillGap = fillCoverGap.BoolValue,
            FontFamily = fontFamily,
            HeaderTitle = headerTitle.ParsedValue,
            HeaderTitleFontSize = headerFontSize.IntValue,
            IsHeaderTitleBold = headerBold.BoolValue,
            LabelFontSize = labelFontSize.IntValue,
            MaxColumns = columns.IntValue,
            MaxCoverWidthPercent = maxCoverWidthPercent.IntValue,
            MinThumbDim = minDimThumbnail.IntValue,
            PreviewOnly = preview.BoolValue,
            SheetWidth = sheetWidth.IntValue
        };

        sheet.DrawProgressChanged += (args) => {
            DrawProgress = args.Percentage;
            DrawProgressChanged?.Invoke(this, args);
        };
        sheet.ErrorOccurred += (message, isFatal) => ErrorOccurred(message, isFatal);

        Stopwatch sw = Stopwatch.StartNew();

        Log.Information("Starting to draw {0}", Source);
        lock (_imageSet.Images) {
            sheet.BuildLayout();
        }

        var sheetImage = await sheet.Draw();

        sw.Stop();

        // Update list watchers so they see the new sizes
        ImageListChanged?.Invoke(this, false);

        lock (_logLock) {
            Log.Information("---------------------------------------------------------------------------");
            Log.Information("Completed {0}! It took {1}", Source, sw.Elapsed);
            Log.Information("Sheet Size: {0} images, {1} rows, {2}x{3}px", sheet.RowLayout.Sum(r => r.Count), sheet.RowLayout.Count, sheetImage.Width, sheetImage.Height);
            Log.Information("Min/Max Images per Row: {0}/{1}", sheet.RowLayout.Max(r => r.Count), sheet.RowLayout.Min(r => r.Count));
            Log.Information("Output Quality: {0}%", quality.IntValue);

            try {
                lock (_imageSet.Images) {
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
                if (!noGui.BoolValue) {
                    Log.Debug("Exit on Complete: {0}", exitOnComplete.BoolValue);
                }
                Log.Information("---------------------------------------------------------------------------");
                // Clean up
                sheetImage.Dispose();
            }
        }
        return exitOnComplete.BoolValue;
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
        _isDisposed = true;
        if (_imageSet == null) {
            return;
        }
        Log.Debug("{0} Disposing", Source);
        _imageSet.Dispose();
        Log.Debug("{0} Disposed", Source);
    }
}
