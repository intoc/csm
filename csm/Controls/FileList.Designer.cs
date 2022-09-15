
namespace csm.Controls;

partial class FileList
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.lblImageCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlCenter = new System.Windows.Forms.Panel();
            this.files = new System.Windows.Forms.DataGridView();
            this.FileCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WidthCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HeightCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OriginalSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Orientation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.binder = new System.Windows.Forms.BindingSource(this.components);
            this.pnlButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnExclude = new System.Windows.Forms.Button();
            this.btnReload = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
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
            this.WidthCol,
            this.HeightCol,
            this.OriginalSize,
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
            this.files.Size = new System.Drawing.Size(569, 380);
            this.files.TabIndex = 3;
            this.files.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Files_CellDoubleClick);
            this.files.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.Files_RowPrePaint);
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
            // WidthCol
            // 
            this.WidthCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.WidthCol.DataPropertyName = "Width";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.WidthCol.DefaultCellStyle = dataGridViewCellStyle1;
            this.WidthCol.HeaderText = "Width";
            this.WidthCol.Name = "WidthCol";
            this.WidthCol.ReadOnly = true;
            this.WidthCol.Width = 64;
            // 
            // HeightCol
            // 
            this.HeightCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.HeightCol.DataPropertyName = "Height";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.HeightCol.DefaultCellStyle = dataGridViewCellStyle2;
            this.HeightCol.HeaderText = "Height";
            this.HeightCol.Name = "HeightCol";
            this.HeightCol.ReadOnly = true;
            this.HeightCol.Width = 68;
            // 
            // OriginalSize
            // 
            this.OriginalSize.DataPropertyName = "OriginalSize";
            this.OriginalSize.HeaderText = "Original Size";
            this.OriginalSize.Name = "OriginalSize";
            this.OriginalSize.ReadOnly = true;
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
            this.pnlButtons.Controls.Add(this.btnReload);
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
            this.btnExclude.Size = new System.Drawing.Size(88, 27);
            this.btnExclude.TabIndex = 0;
            this.btnExclude.Text = "Exclude";
            this.btnExclude.UseVisualStyleBackColor = true;
            this.btnExclude.Click += new System.EventHandler(this.RemoveFiles);
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(100, 3);
            this.btnReload.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(88, 27);
            this.btnReload.TabIndex = 1;
            this.btnReload.Text = "Reload";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.ReloadFiles);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(196, 3);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(88, 27);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // FileList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(569, 440);
            this.ControlBox = false;
            this.Controls.Add(this.pnlCenter);
            this.Controls.Add(this.statusBar);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "FileList";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "File List";
            this.Load += new System.EventHandler(this.FileList_Load);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.pnlCenter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.files)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.binder)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.StatusStrip statusBar;
    private System.Windows.Forms.ToolStripStatusLabel lblImageCount;
    private System.Windows.Forms.Panel pnlCenter;
    private System.Windows.Forms.DataGridView files;
    private System.Windows.Forms.FlowLayoutPanel pnlButtons;
    private System.Windows.Forms.Button btnExclude;
    private System.Windows.Forms.Button btnReload;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.BindingSource binder;
    private System.Windows.Forms.DataGridViewTextBoxColumn FileCol;
    private System.Windows.Forms.DataGridViewTextBoxColumn WidthCol;
    private System.Windows.Forms.DataGridViewTextBoxColumn HeightCol;
    private System.Windows.Forms.DataGridViewTextBoxColumn OriginalSize;
    private System.Windows.Forms.DataGridViewTextBoxColumn Orientation;
}
