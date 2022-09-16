using csm.Logic;
using csm.Models;
using System.ComponentModel;
using System.Diagnostics;

namespace csm.Controls;

public partial class FileList : Form {
    readonly ContactSheet cs;

    private readonly IDictionary<string, bool> PinnedImages = new Dictionary<string, bool>();
    private FileSystemWatcher fileWatcher = new FileSystemWatcher();

    public FileList(ContactSheet sheet) {
        InitializeComponent();
        cs = sheet;
        if (cs.SourceDirectory != null) {
            fileWatcher.Path = cs.SourceDirectory;
        }
        if (cs.ImageList.Any()) {
            binder.DataSource = new BindingList<ImageData>(cs.ImageList);
        }
    }

    /// <summary>
    /// Invoked after the window loads
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FileListLoaded(object sender, EventArgs e) {
        cs.ImageListChanged += new ImageListChangedEventHandler(ImageListChanged);
        Rectangle bounds = Owner.Bounds;
        Bounds = new Rectangle(bounds.X + bounds.Width, bounds.Y, Width, bounds.Height);
        cs.SourceDirectoryChanged += (path) => {
            fileWatcher.Path = path;
            PinnedImages.Clear();
        };
        fileWatcher.NotifyFilter = NotifyFilters.FileName;
        fileWatcher.Changed += ReloadFiles;
        fileWatcher.Deleted += ReloadFiles;
        fileWatcher.Created += ReloadFiles;
        fileWatcher.Renamed += ReloadFiles;
        fileWatcher.EnableRaisingEvents = true;
        UpdateList();
    }

    /// <summary>
    /// Invoked by the back end whenever the image list is loaded
    /// </summary>
    /// <param name="args"></param>
    void ImageListChanged() {
        Debug.WriteLine("FileList-ImageListChanged");
        Invoke(new MethodInvoker(UpdateList));
    }

    /// <summary>
    /// Invoked by cs_ImageListChanged
    /// </summary>
    /// <param name="args"></param>
    void UpdateList() {
        Debug.WriteLine("FileList-UpdateList");
        if (binder.DataSource == null && cs.ImageList.Any()) {
            binder.DataSource = new BindingList<ImageData>(cs.ImageList);
        } else {
            foreach (ImageData image in cs.ImageList.Where(i => PinnedImages.ContainsKey(i.File))) {
                image.Include = PinnedImages[image.File];
                image.InclusionPinned = true;
            }
            binder.ResetBindings(false);
        }
        UpdateStatus();
    }

    private void UpdateStatus() {
        Text = cs.SourceDirectory?.Split('\\').Last() ?? "No Directory Selected";
        lblImageCount.Text = string.Format("{0} Images ({1} Excluded, {2} Included)", binder.Count, cs.ImageList.Count(i => !i.Include), cs.ImageList.Count(i => i.Include));
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
        PinnedImages.Clear();
        await cs.LoadFileList();
    }

    private async void ReloadFiles(object sender, EventArgs e) => await cs.LoadFileList(true);

    private void RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) {
        if (files.Rows[e.RowIndex].DataBoundItem is ImageData image) {
            files.Rows[e.RowIndex].DefaultCellStyle.BackColor = image.Include ? Color.White : Color.LightGray;
        }
    }

    private void UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {
        if (e.Row?.DataBoundItem is ImageData image) {
            image.Include = false;
            PinImage(image);
            e.Cancel = true;
            e.Row.Selected = false;
        }
    }

    private void CloseButtonClicked(object sender, EventArgs e) => Hide();
}

