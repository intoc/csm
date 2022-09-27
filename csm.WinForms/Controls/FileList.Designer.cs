
namespace csm.WinForms.Controls;

partial class FileList {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
        if (disposing && (components != null)) {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.lblImageCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlCenter = new System.Windows.Forms.Panel();
            this.files = new System.Windows.Forms.DataGridView();
            this.FileCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sizeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Orientation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.binder = new System.Windows.Forms.BindingSource(this.components);
            this.pnlButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnExclude = new System.Windows.Forms.Button();
            this.btnInclude = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.openDirectoryButton = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.ImageSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusBar.SuspendLayout();
            this.pnlCenter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.files)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.binder)).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblImageCount});
            this.statusBar.Location = new System.Drawing.Point(0, 418);
            this.statusBar.Name = "statusBar";
            this.statusBar.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusBar.Size = new System.Drawing.Size(569, 22);
            this.statusBar.TabIndex = 0;
            this.statusBar.Text = "statusStrip1";
            // 
            // lblImageCount
            // 
            this.lblImageCount.Name = "lblImageCount";
            this.lblImageCount.Size = new System.Drawing.Size(73, 17);
            this.lblImageCount.Text = "ImageCount";
            // 
            // pnlCenter
            // 
            this.pnlCenter.Controls.Add(this.files);
            this.pnlCenter.Controls.Add(this.pnlButtons);
            this.pnlCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCenter.Location = new System.Drawing.Point(0, 0);
            this.pnlCenter.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlCenter.Name = "pnlCenter";
            this.pnlCenter.Size = new System.Drawing.Size(569, 418);
            this.pnlCenter.TabIndex = 1;
            // 
            // files
            // 
            this.files.AllowUserToAddRows = false;
            this.files.AllowUserToResizeRows = false;
            this.files.AutoGenerateColumns = false;
            this.files.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.files.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.files.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.files.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FileCol,
            this.sizeColumn,
            this.Orientation});
            this.files.DataSource = this.binder;
            this.files.Dock = System.Windows.Forms.DockStyle.Fill;
            this.files.Location = new System.Drawing.Point(0, 0);
            this.files.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.files.Name = "files";
            this.files.ReadOnly = true;
            this.files.RowHeadersVisible = false;
            this.files.RowHeadersWidth = 4;
            this.files.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.files.ShowCellErrors = false;
            this.files.ShowCellToolTips = false;
            this.files.ShowEditingIcon = false;
            this.files.ShowRowErrors = false;
            this.files.Size = new System.Drawing.Size(569, 380);
            this.files.TabIndex = 3;
            this.files.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.RowPrePaint);
            this.files.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.UserDeletingRow);
            // 
            // FileCol
            // 
            this.FileCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.FileCol.DataPropertyName = "FileName";
            this.FileCol.HeaderText = "File";
            this.FileCol.Name = "FileCol";
            this.FileCol.ReadOnly = true;
            this.FileCol.Width = 50;
            // 
            // sizeColumn
            // 
            this.sizeColumn.DataPropertyName = "SizeString";
            this.sizeColumn.HeaderText = "Size";
            this.sizeColumn.Name = "sizeColumn";
            this.sizeColumn.ReadOnly = true;
            // 
            // Orientation
            // 
            this.Orientation.DataPropertyName = "Orientation";
            this.Orientation.HeaderText = "Orientation";
            this.Orientation.Name = "Orientation";
            this.Orientation.ReadOnly = true;
            // 
            // binder
            // 
            this.binder.Sort = "";
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnExclude);
            this.pnlButtons.Controls.Add(this.btnInclude);
            this.pnlButtons.Controls.Add(this.btnReset);
            this.pnlButtons.Controls.Add(this.openDirectoryButton);
            this.pnlButtons.Controls.Add(this.btnClose);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(0, 380);
            this.pnlButtons.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(569, 38);
            this.pnlButtons.TabIndex = 2;
            // 
            // btnExclude
            // 
            this.btnExclude.Location = new System.Drawing.Point(4, 3);
            this.btnExclude.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnExclude.Name = "btnExclude";
            this.btnExclude.Size = new System.Drawing.Size(107, 27);
            this.btnExclude.TabIndex = 0;
            this.btnExclude.Text = "Exclude Selected";
            this.btnExclude.UseVisualStyleBackColor = true;
            this.btnExclude.Click += new System.EventHandler(this.RemoveFilesClicked);
            // 
            // btnInclude
            // 
            this.btnInclude.AutoSize = true;
            this.btnInclude.Location = new System.Drawing.Point(119, 3);
            this.btnInclude.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnInclude.Name = "btnInclude";
            this.btnInclude.Size = new System.Drawing.Size(103, 27);
            this.btnInclude.TabIndex = 0;
            this.btnInclude.Text = "Include Selected";
            this.btnInclude.UseVisualStyleBackColor = true;
            this.btnInclude.Click += new System.EventHandler(this.IncludeFilesClicked);
            // 
            // btnReset
            // 
            this.btnReset.AutoSize = true;
            this.btnReset.Location = new System.Drawing.Point(230, 3);
            this.btnReset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(88, 27);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.ResetIncluded);
            // 
            // openDirectoryButton
            // 
            this.openDirectoryButton.Location = new System.Drawing.Point(325, 3);
            this.openDirectoryButton.Name = "openDirectoryButton";
            this.openDirectoryButton.Size = new System.Drawing.Size(107, 27);
            this.openDirectoryButton.TabIndex = 4;
            this.openDirectoryButton.Text = "Open Directory";
            this.openDirectoryButton.UseVisualStyleBackColor = true;
            this.openDirectoryButton.Click += new System.EventHandler(this.OpenDirectoryButtonClicked);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(439, 3);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(88, 27);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.CloseButtonClicked);
            // 
            // ImageSize
            // 
            this.ImageSize.DataPropertyName = "SizeString";
            this.ImageSize.HeaderText = "Size";
            this.ImageSize.Name = "ImageSize";
            this.ImageSize.Width = 258;
            // 
            // FileList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(569, 440);
            this.ControlBox = false;
            this.Controls.Add(this.pnlCenter);
            this.Controls.Add(this.statusBar);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "FileList";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "No Source Selected";
            this.Load += new System.EventHandler(this.FileListLoaded);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.pnlCenter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.files)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.binder)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private StatusStrip statusBar;
    private ToolStripStatusLabel lblImageCount;
    private Panel pnlCenter;
    private DataGridView files;
    private FlowLayoutPanel pnlButtons;
    private Button btnExclude;
    private Button btnInclude;
    private Button btnClose;
    private BindingSource binder;
    private Button btnReset;
    private DataGridViewTextBoxColumn ImageSize;
    private Button openDirectoryButton;
    private DataGridViewTextBoxColumn FileCol;
    private DataGridViewTextBoxColumn sizeColumn;
    private DataGridViewTextBoxColumn Orientation;
}
