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
            this.stateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.progressColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.drawProgress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sheetBinder = new System.Windows.Forms.BindingSource(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.maxConcurrentLoadLabel = new System.Windows.Forms.Label();
            this.maxConcurrentLoadSpinner = new System.Windows.Forms.NumericUpDown();
            this.maxConcurrentLabel = new System.Windows.Forms.Label();
            this.maxConcurrentDrawSpinner = new System.Windows.Forms.NumericUpDown();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.chooseDirectoryButton = new System.Windows.Forms.Button();
            this.chooseArchivesButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.runButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.sheetGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetBinder)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxConcurrentLoadSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxConcurrentDrawSpinner)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // sheetGrid
            // 
            this.sheetGrid.AllowUserToAddRows = false;
            this.sheetGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sheetGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.sheetGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sheetGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sourceColumn,
            this.stateColumn,
            this.progressColumn,
            this.drawProgress});
            this.sheetGrid.Location = new System.Drawing.Point(0, 0);
            this.sheetGrid.Name = "sheetGrid";
            this.sheetGrid.ReadOnly = true;
            this.sheetGrid.RowTemplate.Height = 25;
            this.sheetGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.sheetGrid.Size = new System.Drawing.Size(890, 418);
            this.sheetGrid.TabIndex = 2;
            this.sheetGrid.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.RowPrePaint);
            this.sheetGrid.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.UserDeletedRow);
            this.sheetGrid.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.UserDeletingRow);
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
            // stateColumn
            // 
            this.stateColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.stateColumn.DataPropertyName = "State";
            this.stateColumn.FillWeight = 34.425F;
            this.stateColumn.HeaderText = "State";
            this.stateColumn.Name = "stateColumn";
            this.stateColumn.ReadOnly = true;
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
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.statusStrip1);
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 418);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(890, 63);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.maxConcurrentLoadLabel);
            this.panel2.Controls.Add(this.maxConcurrentLoadSpinner);
            this.panel2.Controls.Add(this.maxConcurrentLabel);
            this.panel2.Controls.Add(this.maxConcurrentDrawSpinner);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(491, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.panel2.Size = new System.Drawing.Size(399, 41);
            this.panel2.TabIndex = 9;
            // 
            // maxConcurrentLoadLabel
            // 
            this.maxConcurrentLoadLabel.AutoSize = true;
            this.maxConcurrentLoadLabel.Location = new System.Drawing.Point(5, 11);
            this.maxConcurrentLoadLabel.Name = "maxConcurrentLoadLabel";
            this.maxConcurrentLoadLabel.Size = new System.Drawing.Size(122, 15);
            this.maxConcurrentLoadLabel.TabIndex = 13;
            this.maxConcurrentLoadLabel.Text = "Max Concurrent Load";
            // 
            // maxConcurrentLoadSpinner
            // 
            this.maxConcurrentLoadSpinner.Location = new System.Drawing.Point(133, 9);
            this.maxConcurrentLoadSpinner.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.maxConcurrentLoadSpinner.Name = "maxConcurrentLoadSpinner";
            this.maxConcurrentLoadSpinner.Size = new System.Drawing.Size(61, 23);
            this.maxConcurrentLoadSpinner.TabIndex = 14;
            this.maxConcurrentLoadSpinner.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.maxConcurrentLoadSpinner.ValueChanged += new System.EventHandler(this.MaxConcurrentLoadSpinnerValueChanged);
            // 
            // maxConcurrentLabel
            // 
            this.maxConcurrentLabel.AutoSize = true;
            this.maxConcurrentLabel.Location = new System.Drawing.Point(200, 11);
            this.maxConcurrentLabel.Name = "maxConcurrentLabel";
            this.maxConcurrentLabel.Size = new System.Drawing.Size(123, 15);
            this.maxConcurrentLabel.TabIndex = 11;
            this.maxConcurrentLabel.Text = "Max Concurrent Draw";
            // 
            // maxConcurrentDrawSpinner
            // 
            this.maxConcurrentDrawSpinner.Location = new System.Drawing.Point(329, 9);
            this.maxConcurrentDrawSpinner.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.maxConcurrentDrawSpinner.Name = "maxConcurrentDrawSpinner";
            this.maxConcurrentDrawSpinner.Size = new System.Drawing.Size(61, 23);
            this.maxConcurrentDrawSpinner.TabIndex = 12;
            this.maxConcurrentDrawSpinner.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.maxConcurrentDrawSpinner.ValueChanged += new System.EventHandler(this.MaxConcurrentDrawSpinnerValueChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.statusStrip1.Size = new System.Drawing.Size(890, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // loadingCountLabel
            // 
            this.loadingCountLabel.Name = "loadingCountLabel";
            this.loadingCountLabel.Size = new System.Drawing.Size(53, 17);
            this.loadingCountLabel.Text = "Loading:";
            // 
            // loadingCountValueLabel
            // 
            this.loadingCountValueLabel.AutoSize = false;
            this.loadingCountValueLabel.Name = "loadingCountValueLabel";
            this.loadingCountValueLabel.Size = new System.Drawing.Size(25, 17);
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
            this.drawQueueCountValueLabel.AutoSize = false;
            this.drawQueueCountValueLabel.Name = "drawQueueCountValueLabel";
            this.drawQueueCountValueLabel.Size = new System.Drawing.Size(25, 17);
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
            this.drawingCountValueLabel.AutoSize = false;
            this.drawingCountValueLabel.Name = "drawingCountValueLabel";
            this.drawingCountValueLabel.Size = new System.Drawing.Size(25, 17);
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
            this.completedCountValueLabel.AutoSize = false;
            this.completedCountValueLabel.Name = "completedCountValueLabel";
            this.completedCountValueLabel.Size = new System.Drawing.Size(50, 17);
            this.completedCountValueLabel.Text = "0";
            // 
            // completedProgressBar
            // 
            this.completedProgressBar.AutoSize = false;
            this.completedProgressBar.Name = "completedProgressBar";
            this.completedProgressBar.Size = new System.Drawing.Size(210, 16);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.chooseDirectoryButton);
            this.flowLayoutPanel1.Controls.Add(this.chooseArchivesButton);
            this.flowLayoutPanel1.Controls.Add(this.deleteButton);
            this.flowLayoutPanel1.Controls.Add(this.runButton);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(480, 29);
            this.flowLayoutPanel1.TabIndex = 7;
            // 
            // chooseDirectoryButton
            // 
            this.chooseDirectoryButton.Location = new System.Drawing.Point(3, 3);
            this.chooseDirectoryButton.Name = "chooseDirectoryButton";
            this.chooseDirectoryButton.Size = new System.Drawing.Size(121, 23);
            this.chooseDirectoryButton.TabIndex = 4;
            this.chooseDirectoryButton.Text = "Add Directory Items";
            this.chooseDirectoryButton.UseVisualStyleBackColor = true;
            this.chooseDirectoryButton.Click += new System.EventHandler(this.ChooseDirectoryClicked);
            // 
            // chooseArchivesButton
            // 
            this.chooseArchivesButton.Location = new System.Drawing.Point(130, 3);
            this.chooseArchivesButton.Name = "chooseArchivesButton";
            this.chooseArchivesButton.Size = new System.Drawing.Size(110, 23);
            this.chooseArchivesButton.TabIndex = 0;
            this.chooseArchivesButton.Text = "Add Archives";
            this.chooseArchivesButton.UseVisualStyleBackColor = true;
            this.chooseArchivesButton.Click += new System.EventHandler(this.ChooseArchivesButtonClicked);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(246, 3);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(109, 23);
            this.deleteButton.TabIndex = 3;
            this.deleteButton.Text = "Remove Selected";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.DeleteSelectedRows);
            // 
            // runButton
            // 
            this.runButton.Location = new System.Drawing.Point(361, 3);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(116, 23);
            this.runButton.TabIndex = 1;
            this.runButton.Text = "Run";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.RunButtonClicked);
            // 
            // BatchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 481);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.sheetGrid);
            this.MinimumSize = new System.Drawing.Size(906, 200);
            this.Name = "BatchForm";
            this.Text = "Batch Processing";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BatchFormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.sheetGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetBinder)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxConcurrentLoadSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxConcurrentDrawSpinner)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private DataGridView sheetGrid;
        private BindingSource sheetBinder;
        private DataGridViewTextBoxColumn sourceColumn;
        private DataGridViewTextBoxColumn stateColumn;
        private DataGridViewTextBoxColumn progressColumn;
        private DataGridViewTextBoxColumn drawProgress;
        private Panel panel1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button chooseArchivesButton;
        private Button deleteButton;
        private Button runButton;
        private StatusStrip statusStrip1;
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
        private Panel panel2;
        private Label maxConcurrentLoadLabel;
        private NumericUpDown maxConcurrentLoadSpinner;
        private Label maxConcurrentLabel;
        private NumericUpDown maxConcurrentDrawSpinner;
        private Button chooseDirectoryButton;
    }
}