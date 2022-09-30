using csm.Business.Logic;
using csm.Business.Models;
using Serilog;
using System.ComponentModel;
using System.Diagnostics;

namespace csm.WinForms.Controls;

public partial class FileList : Form {

    readonly SheetLoader cs;

    private readonly IDictionary<string, bool> PinnedImages = new Dictionary<string, bool>();
    private readonly FileSystemWatcher fileWatcher = new();

    public FileList(SheetLoader sheet) {
        InitializeComponent();
        cs = sheet;
        if (cs.Source != null) {
            if (Directory.Exists(cs.Source)) {
                fileWatcher.Path = cs.Source;
                fileWatcher.EnableRaisingEvents = true;
                Text = Path.GetDirectoryName(cs.Source);
            } else if (File.Exists(cs.Source)) {
                Text = Path.GetFileName(cs.Source);
            }
        }
        if (cs.ImageList?.Any() ?? false) {
            binder.DataSource = new BindingList<ImageData>(cs.ImageList);
        }
    }

    /// <summary>
    /// Invoked after the window loads
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FileListLoaded(object sender, EventArgs e) {
        cs.ImageListChanged += ImageListChanged;
        Rectangle bounds = Owner.Bounds;
        Bounds = new Rectangle(bounds.X + bounds.Width, bounds.Y, Width, bounds.Height);
        cs.LoadCompleted += SourceChanged;
        fileWatcher.NotifyFilter = NotifyFilters.FileName;
        fileWatcher.Changed += ReloadFiles;
        fileWatcher.Deleted += ReloadFiles;
        fileWatcher.Created += ReloadFiles;
        fileWatcher.Renamed += ReloadFiles;
        fileWatcher.IncludeSubdirectories = true;
        UpdateList(cs, false);
    }

    /// <summary>
    /// Invoked by the back end whenever the source changes
    /// </summary>
    /// <param name="path"></param>
    void SourceChanged(SheetLoader sheet, IFileSource source) {
        Log.Debug("FileList-SourceChanged");
        if (Directory.Exists(source.ImageFileDirectoryPath)) {
            fileWatcher.Path = source.ImageFileDirectoryPath;
            fileWatcher.EnableRaisingEvents = true;
        } else {
            fileWatcher.EnableRaisingEvents = false;
        }
        Text = sheet.Source?.Split('\\').Last() ?? "No Source Selected";
        PinnedImages.Clear();
    }

    /// <summary>
    /// Invoked by the back end whenever the image list is loaded
    /// </summary>
    /// <param name="args"></param>
    void ImageListChanged(SheetLoader source, bool filesAddedOrRemoved) {
        Log.Debug("FileList-ImageListChanged");
        Invoke(() => UpdateList(source, filesAddedOrRemoved));
    }

    /// <summary>
    /// Invoked by cs_ImageListChanged
    /// </summary>
    /// <param name="args"></param>
    void UpdateList(SheetLoader source, bool reset) {
        Log.Debug("FileList-UpdateList");
        if (source.ImageList == null) {
            return;
        }
        if (binder.DataSource == null && source.ImageList.Any()) {
            binder.DataSource = new BindingList<ImageData>(source.ImageList);
        } else {
            foreach (ImageData image in source.ImageList.Where(i => PinnedImages.ContainsKey(i.File))) {
                image.Include = PinnedImages[image.File];
                image.InclusionPinned = true;
            }
            if (reset) {
                binder.ResetBindings(false);
            } else {
                files.Refresh();
            }
        }
        UpdateStatus();
        files.Enabled = true;
    }

    private void UpdateStatus() {
        if (cs.ImageList == null) {
            return;
        }
        int included = cs.ImageList.Count(i => !i.Include);
        int excluded = cs.ImageList.Count - included;
        lblImageCount.Text = $"{cs.ImageList.Count} Images ({included} Excluded, {excluded} Included)";
    }

    private void PinImage(ImageData image) {
        image.InclusionPinned = true;
        PinnedImages[image.File] = image.Include;
    }

    private void RemoveFilesClicked(object sender, EventArgs e) {
        SetIncluded(files.SelectedRows, false);
    }

    private void IncludeFilesClicked(object sender, EventArgs e) {
        SetIncluded(files.SelectedRows, true);
    }

    private void SetIncluded(DataGridViewSelectedRowCollection rows, bool include) {
        foreach (DataGridViewRow row in rows) {
            if (row.DataBoundItem is ImageData image) {
                image.Include = include;
                PinImage(image);
                row.Selected = false;
            }
        }
        UpdateStatus();
    }

    private async void ResetIncluded(object sender, EventArgs e) {
        files.Enabled = false;
        PinnedImages.Clear();
        await cs.LoadFileList();
    }

    private async void ReloadFiles(object sender, EventArgs e) {
        await cs.LoadFileList();
    }

    private void RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) {
        if (e.RowIndex < 0) {
            return;
        }
        var row = files.Rows[e.RowIndex];
        if (row.DataBoundItem is ImageData image) {
            row.DefaultCellStyle.BackColor = RowColor(image);
        }
    }

    private static Color RowColor(ImageData image) => image.Include ? RowColorIncluded(image) : RowColorExcluded(image);
    private static Color RowColorIncluded(ImageData image) => image.InclusionPinned ? (Color.Beige) : (Color.White);
    private static Color RowColorExcluded(ImageData image) => image.InclusionPinned ? (Color.DarkGray) : (Color.LightGray);

    private void UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {
        if (e.Row?.DataBoundItem is ImageData image) {
            image.Include = false;
            PinImage(image);
            e.Cancel = true;
            e.Row.Selected = false;
        }
    }

    private void CloseButtonClicked(object sender, EventArgs e) => Hide();

    private void OpenDirectoryButtonClicked(object sender, EventArgs e) {
        Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", cs.SourceImageFileDirectoryPath ?? string.Empty);
    }

}

