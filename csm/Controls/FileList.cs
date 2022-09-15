using csm.Logic;
using csm.Models;
using System.ComponentModel;
using System.Diagnostics;

namespace csm.Controls;
public partial class FileList : Form {
    readonly ContactSheet cs;

    public delegate void UpdateListDelegate(ImageListChangedEventArgs args);

    public FileList(ContactSheet sheet) {
        InitializeComponent();
        cs = sheet;
        if (cs.ImageList.Any()) {
            binder.DataSource = new BindingList<ImageData>(cs.ImageList);
        }
    }

    /// <summary>
    /// Invoked by the back end whenever the image list is loaded
    /// </summary>
    /// <param name="args"></param>
    void ImageListChanged(ImageListChangedEventArgs args) {
        Debug.WriteLine("FileList-ImageListChanged");
        if (binder.DataSource == null && cs.ImageList.Any()) {
            binder.DataSource = new BindingList<ImageData>(cs.ImageList);
        }
        object[] argsArr = { args };
        Invoke(new UpdateListDelegate(UpdateList), argsArr);
    }

    /// <summary>
    /// Invoked by cs_ImageListChanged
    /// </summary>
    /// <param name="args"></param>
    void UpdateList(ImageListChangedEventArgs args) {
        Debug.WriteLine("FileList-UpdateList");
        binder.ResetBindings(false);
        UpdateStatus();
    }

    /// <summary>
    /// Invoked when a change is noticed by the BindingSource
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ImagesBindingSource_ListChanged(object sender, ListChangedEventArgs e) {
        UpdateStatus();
    }

    private void UpdateStatus() {
        Text = cs.SourceDirectory?.Split('\\').Last() ?? "No Directory Selected";
        lblImageCount.Text = string.Format("{0} Images ({1} Excluded, {2} Included)", binder.Count, cs.ImageList.Count(i => !i.Include), cs.ImageList.Count(i => i.Include));
    }

    /// <summary>
    /// Invoked after the window loads
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FileList_Load(object sender, EventArgs e) {
        cs.ImageListChanged += new ImageListChangedEventHandler(ImageListChanged);
        Rectangle bounds = Owner.Bounds;
        Bounds = new Rectangle(bounds.X + bounds.Width, bounds.Y, Width, bounds.Height);
        UpdateStatus();
    }


    protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
        if (keyData == Keys.Escape) {
            Hide();
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void RemoveFiles(object sender, EventArgs e) {
        foreach (DataGridViewRow row in files.SelectedRows) {
            if (row.DataBoundItem is ImageData data) {
                data.Include = false;
                data.ManuallyExcluded = true;
            }
        }
        UpdateStatus();
    }

    private async void ReloadFiles(object sender, EventArgs e) =>  await cs.LoadFileList(true);

    private void BtnClose_Click(object sender, EventArgs e)  => Hide();

    private void Files_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
        // Open the image with the user's default image viewer
        string path = ((ImageData)files.Rows[e.RowIndex].DataBoundItem).File;
        System.Diagnostics.Process.Start(@path);
    }

    private void Files_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) {
        if (files.Rows[e.RowIndex].DataBoundItem is ImageData concreteSelectedRowItem && !concreteSelectedRowItem.Include) {
            files.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
        }
    }
}

