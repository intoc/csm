namespace csm.WinForms.Controls {
    partial class BatchForm {
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
            this.sheetGrid = new System.Windows.Forms.DataGridView();
            this.sourceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.loadedColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.progressColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.drawProgress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sheetBinder = new System.Windows.Forms.BindingSource(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.maxConcurrentLabel = new System.Windows.Forms.Label();
            this.maxConcurrentSpinner = new System.Windows.Forms.NumericUpDown();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.sheetsCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.sheetsCountValueLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.loadingCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.loadingCountValueLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.loadProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.drawQueueCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.drawQueueCountValueLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.queueProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.drawingCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.drawingCountValueLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.drawingCountBar = new System.Windows.Forms.ToolStripProgressBar();
            this.completedCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.completedCountValueLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.completedProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.runAfterLoadCheckBox = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.chooseArchivesButton = new System.Windows.Forms.Button();
            this.refreshListButton = new System.Windows.Forms.Button();
            this.runButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.sheetGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetBinder)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxConcurrentSpinner)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // sheetGrid
            // 
            this.sheetGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sheetGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.sheetGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sheetGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sourceColumn,
            this.loadedColumn,
            this.progressColumn,
            this.drawProgress});
            this.sheetGrid.Location = new System.Drawing.Point(12, 12);
            this.sheetGrid.Name = "sheetGrid";
            this.sheetGrid.RowTemplate.Height = 25;
            this.sheetGrid.Size = new System.Drawing.Size(795, 415);
            this.sheetGrid.TabIndex = 2;
            this.sheetGrid.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.RowPrePaint);
            // 
            // sourceColumn
            // 
            this.sourceColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.sourceColumn.DataPropertyName = "Source";
            this.sourceColumn.FillWeight = 258.7197F;
            this.sourceColumn.HeaderText = "Source";
            this.sourceColumn.Name = "sourceColumn";
            this.sourceColumn.ReadOnly = true;
            // 
            // loadedColumn
            // 
            this.loadedColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.loadedColumn.DataPropertyName = "FirstLoadComplete";
            this.loadedColumn.FillWeight = 34.425F;
            this.loadedColumn.HeaderText = "Loaded";
            this.loadedColumn.Name = "loadedColumn";
            this.loadedColumn.ReadOnly = true;
            // 
            // progressColumn
            // 
            this.progressColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.progressColumn.DataPropertyName = "LoadProgress";
            this.progressColumn.FillWeight = 45.94163F;
            this.progressColumn.HeaderText = "Load Progress";
            this.progressColumn.Name = "progressColumn";
            this.progressColumn.ReadOnly = true;
            // 
            // drawProgress
            // 
            this.drawProgress.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.drawProgress.DataPropertyName = "DrawProgress";
            this.drawProgress.FillWeight = 60.9137F;
            this.drawProgress.HeaderText = "Draw Progress";
            this.drawProgress.Name = "drawProgress";
            this.drawProgress.ReadOnly = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.maxConcurrentLabel);
            this.panel1.Controls.Add(this.maxConcurrentSpinner);
            this.panel1.Controls.Add(this.statusStrip1);
            this.panel1.Controls.Add(this.runAfterLoadCheckBox);
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 427);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(819, 63);
            this.panel1.TabIndex = 3;
            // 
            // maxConcurrentLabel
            // 
            this.maxConcurrentLabel.AutoSize = true;
            this.maxConcurrentLabel.Location = new System.Drawing.Point(520, 13);
            this.maxConcurrentLabel.Name = "maxConcurrentLabel";
            this.maxConcurrentLabel.Size = new System.Drawing.Size(128, 15);
            this.maxConcurrentLabel.TabIndex = 0;
            this.maxConcurrentLabel.Text = "Max Concurrent Draws";
            // 
            // maxConcurrentSpinner
            // 
            this.maxConcurrentSpinner.Location = new System.Drawing.Point(654, 11);
            this.maxConcurrentSpinner.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.maxConcurrentSpinner.Name = "maxConcurrentSpinner";
            this.maxConcurrentSpinner.Size = new System.Drawing.Size(61, 23);
            this.maxConcurrentSpinner.TabIndex = 1;
            this.maxConcurrentSpinner.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sheetsCountLabel,
            this.sheetsCountValueLabel,
            this.loadingCountLabel,
            this.loadingCountValueLabel,
            this.loadProgressBar,
            this.drawQueueCountLabel,
            this.drawQueueCountValueLabel,
            this.queueProgressBar,
            this.drawingCountLabel,
            this.drawingCountValueLabel,
            this.drawingCountBar,
            this.completedCountLabel,
            this.completedCountValueLabel,
            this.completedProgressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 41);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(819, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // sheetsCountLabel
            // 
            this.sheetsCountLabel.Name = "sheetsCountLabel";
            this.sheetsCountLabel.Size = new System.Drawing.Size(44, 17);
            this.sheetsCountLabel.Text = "Sheets:";
            // 
            // sheetsCountValueLabel
            // 
            this.sheetsCountValueLabel.Name = "sheetsCountValueLabel";
            this.sheetsCountValueLabel.Size = new System.Drawing.Size(13, 17);
            this.sheetsCountValueLabel.Text = "0";
            // 
            // loadingCountLabel
            // 
            this.loadingCountLabel.Name = "loadingCountLabel";
            this.loadingCountLabel.Size = new System.Drawing.Size(53, 17);
            this.loadingCountLabel.Text = "Loading:";
            // 
            // loadingCountValueLabel
            // 
            this.loadingCountValueLabel.Name = "loadingCountValueLabel";
            this.loadingCountValueLabel.Size = new System.Drawing.Size(13, 17);
            this.loadingCountValueLabel.Text = "0";
            // 
            // loadProgressBar
            // 
            this.loadProgressBar.Name = "loadProgressBar";
            this.loadProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // drawQueueCountLabel
            // 
            this.drawQueueCountLabel.Name = "drawQueueCountLabel";
            this.drawQueueCountLabel.Size = new System.Drawing.Size(52, 17);
            this.drawQueueCountLabel.Text = "Queued:";
            // 
            // drawQueueCountValueLabel
            // 
            this.drawQueueCountValueLabel.Name = "drawQueueCountValueLabel";
            this.drawQueueCountValueLabel.Size = new System.Drawing.Size(13, 17);
            this.drawQueueCountValueLabel.Text = "0";
            // 
            // queueProgressBar
            // 
            this.queueProgressBar.Name = "queueProgressBar";
            this.queueProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // drawingCountLabel
            // 
            this.drawingCountLabel.Name = "drawingCountLabel";
            this.drawingCountLabel.Size = new System.Drawing.Size(54, 17);
            this.drawingCountLabel.Text = "Drawing:";
            // 
            // drawingCountValueLabel
            // 
            this.drawingCountValueLabel.Name = "drawingCountValueLabel";
            this.drawingCountValueLabel.Size = new System.Drawing.Size(13, 17);
            this.drawingCountValueLabel.Text = "0";
            // 
            // drawingCountBar
            // 
            this.drawingCountBar.Name = "drawingCountBar";
            this.drawingCountBar.Size = new System.Drawing.Size(100, 16);
            // 
            // completedCountLabel
            // 
            this.completedCountLabel.Name = "completedCountLabel";
            this.completedCountLabel.Size = new System.Drawing.Size(69, 17);
            this.completedCountLabel.Text = "Completed:";
            // 
            // completedCountValueLabel
            // 
            this.completedCountValueLabel.Name = "completedCountValueLabel";
            this.completedCountValueLabel.Size = new System.Drawing.Size(13, 17);
            this.completedCountValueLabel.Text = "0";
            // 
            // completedProgressBar
            // 
            this.completedProgressBar.Name = "completedProgressBar";
            this.completedProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // runAfterLoadCheckBox
            // 
            this.runAfterLoadCheckBox.AutoSize = true;
            this.runAfterLoadCheckBox.Location = new System.Drawing.Point(339, 12);
            this.runAfterLoadCheckBox.Name = "runAfterLoadCheckBox";
            this.runAfterLoadCheckBox.Size = new System.Drawing.Size(175, 19);
            this.runAfterLoadCheckBox.TabIndex = 9;
            this.runAfterLoadCheckBox.Text = "Run after loading completes";
            this.runAfterLoadCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.runAfterLoadCheckBox.UseVisualStyleBackColor = true;
            this.runAfterLoadCheckBox.CheckedChanged += new System.EventHandler(this.RunAfterLoadCheckBox_CheckedChanged);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.flowLayoutPanel1.Controls.Add(this.chooseArchivesButton);
            this.flowLayoutPanel1.Controls.Add(this.refreshListButton);
            this.flowLayoutPanel1.Controls.Add(this.runButton);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(6, 6);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(327, 32);
            this.flowLayoutPanel1.TabIndex = 7;
            // 
            // chooseArchivesButton
            // 
            this.chooseArchivesButton.Location = new System.Drawing.Point(3, 3);
            this.chooseArchivesButton.Name = "chooseArchivesButton";
            this.chooseArchivesButton.Size = new System.Drawing.Size(139, 23);
            this.chooseArchivesButton.TabIndex = 0;
            this.chooseArchivesButton.Text = "Choose Archives";
            this.chooseArchivesButton.UseVisualStyleBackColor = true;
            this.chooseArchivesButton.Click += new System.EventHandler(this.ChooseArchivesButton_Click);
            // 
            // refreshListButton
            // 
            this.refreshListButton.Location = new System.Drawing.Point(148, 3);
            this.refreshListButton.Name = "refreshListButton";
            this.refreshListButton.Size = new System.Drawing.Size(91, 23);
            this.refreshListButton.TabIndex = 3;
            this.refreshListButton.Text = "Refresh List";
            this.refreshListButton.UseVisualStyleBackColor = true;
            this.refreshListButton.Click += new System.EventHandler(this.RefreshListButton_Click);
            // 
            // runButton
            // 
            this.runButton.Location = new System.Drawing.Point(245, 3);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(75, 23);
            this.runButton.TabIndex = 1;
            this.runButton.Text = "Run";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.RunButton_Click);
            // 
            // BatchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(819, 490);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.sheetGrid);
            this.Name = "BatchForm";
            this.Text = "Batch Processing";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BatchForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.sheetGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetBinder)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxConcurrentSpinner)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private DataGridView sheetGrid;
        private BindingSource sheetBinder;
        private DataGridViewTextBoxColumn sourceColumn;
        private DataGridViewCheckBoxColumn loadedColumn;
        private DataGridViewTextBoxColumn progressColumn;
        private DataGridViewTextBoxColumn drawProgress;
        private Panel panel1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button chooseArchivesButton;
        private Button refreshListButton;
        private Button runButton;
        private Label maxConcurrentLabel;
        private NumericUpDown maxConcurrentSpinner;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel sheetsCountLabel;
        private ToolStripStatusLabel sheetsCountValueLabel;
        private ToolStripStatusLabel loadingCountLabel;
        private ToolStripStatusLabel loadingCountValueLabel;
        private ToolStripStatusLabel drawQueueCountLabel;
        private ToolStripStatusLabel drawQueueCountValueLabel;
        private ToolStripStatusLabel drawingCountLabel;
        private ToolStripStatusLabel drawingCountValueLabel;
        private ToolStripStatusLabel completedCountLabel;
        private ToolStripStatusLabel completedCountValueLabel;
        private ToolStripProgressBar completedProgressBar;
        private ToolStripProgressBar loadProgressBar;
        private ToolStripProgressBar queueProgressBar;
        private ToolStripProgressBar drawingCountBar;
        private CheckBox runAfterLoadCheckBox;
    }
}