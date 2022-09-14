namespace csm.Controls
{
    partial class CsmGui
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CsmGui));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.drawProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.drawStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.elapsedTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.settingsFileStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.menu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSettingsAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawSheetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.directoryPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.directoryLabelLabel = new System.Windows.Forms.Label();
            this.directoryLabel = new System.Windows.Forms.Label();
            this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnFiles = new System.Windows.Forms.Button();
            this.btnDir = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.settingsLabel = new System.Windows.Forms.Label();
            this.paramsPanel = new csm.Controls.ParamsPanel();
            this.statusStrip.SuspendLayout();
            this.menu.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.directoryPanel.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.drawProgressBar,
            this.drawStatus,
            this.elapsedTime,
            this.settingsFileStatus});
            this.statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStrip.Location = new System.Drawing.Point(0, 568);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.ShowItemToolTips = true;
            this.statusStrip.Size = new System.Drawing.Size(943, 24);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "status";
            // 
            // drawProgressBar
            // 
            this.drawProgressBar.ForeColor = System.Drawing.SystemColors.ControlText;
            this.drawProgressBar.Name = "drawProgressBar";
            this.drawProgressBar.Size = new System.Drawing.Size(150, 18);
            this.drawProgressBar.ToolTipText = "Drawing Progress";
            // 
            // drawStatus
            // 
            this.drawStatus.Name = "drawStatus";
            this.drawStatus.Size = new System.Drawing.Size(65, 19);
            this.drawStatus.Text = "drawStatus";
            // 
            // elapsedTime
            // 
            this.elapsedTime.Name = "elapsedTime";
            this.elapsedTime.Size = new System.Drawing.Size(47, 19);
            this.elapsedTime.Text = "Elapsed";
            // 
            // settingsFileStatus
            // 
            this.settingsFileStatus.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.settingsFileStatus.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.settingsFileStatus.Name = "settingsFileStatus";
            this.settingsFileStatus.Size = new System.Drawing.Size(102, 19);
            this.settingsFileStatus.Text = "settingsFileStatus";
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(943, 24);
            this.menu.TabIndex = 4;
            this.menu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadSettingsToolStripMenuItem,
            this.saveSettingsToolStripMenuItem,
            this.saveSettingsAsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadSettingsToolStripMenuItem
            // 
            this.loadSettingsToolStripMenuItem.Name = "loadSettingsToolStripMenuItem";
            this.loadSettingsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.loadSettingsToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.loadSettingsToolStripMenuItem.Text = "Load Settings";
            this.loadSettingsToolStripMenuItem.Click += new System.EventHandler(this.LoadSettings);
            // 
            // saveSettingsToolStripMenuItem
            // 
            this.saveSettingsToolStripMenuItem.Name = "saveSettingsToolStripMenuItem";
            this.saveSettingsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveSettingsToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.saveSettingsToolStripMenuItem.Text = "Save Settings";
            this.saveSettingsToolStripMenuItem.Click += new System.EventHandler(this.SaveSettings);
            // 
            // saveSettingsAsToolStripMenuItem
            // 
            this.saveSettingsAsToolStripMenuItem.Name = "saveSettingsAsToolStripMenuItem";
            this.saveSettingsAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveSettingsAsToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.saveSettingsAsToolStripMenuItem.Text = "Save Settings As...";
            this.saveSettingsAsToolStripMenuItem.Click += new System.EventHandler(this.SaveSettingsAs);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeyDisplayString = "Esc";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.Exit);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeDirectoryToolStripMenuItem,
            this.viewFilesToolStripMenuItem,
            this.drawSheetToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // changeDirectoryToolStripMenuItem
            // 
            this.changeDirectoryToolStripMenuItem.Name = "changeDirectoryToolStripMenuItem";
            this.changeDirectoryToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.changeDirectoryToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.changeDirectoryToolStripMenuItem.Text = "Change Directory";
            this.changeDirectoryToolStripMenuItem.Click += new System.EventHandler(this.ChangeDirectory);
            // 
            // viewFilesToolStripMenuItem
            // 
            this.viewFilesToolStripMenuItem.Name = "viewFilesToolStripMenuItem";
            this.viewFilesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.viewFilesToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.viewFilesToolStripMenuItem.Text = "View Files";
            this.viewFilesToolStripMenuItem.Click += new System.EventHandler(this.ViewFiles);
            // 
            // drawSheetToolStripMenuItem
            // 
            this.drawSheetToolStripMenuItem.Name = "drawSheetToolStripMenuItem";
            this.drawSheetToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.drawSheetToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.drawSheetToolStripMenuItem.Text = "Draw Sheet";
            this.drawSheetToolStripMenuItem.Click += new System.EventHandler(this.RunSheet);
            // 
            // pnlMain
            // 
            this.pnlMain.AutoSize = true;
            this.pnlMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlMain.Controls.Add(this.directoryPanel);
            this.pnlMain.Controls.Add(this.paramsPanel);
            this.pnlMain.Controls.Add(this.buttonPanel);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 24);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(943, 544);
            this.pnlMain.TabIndex = 6;
            // 
            // directoryPanel
            // 
            this.directoryPanel.AutoSize = true;
            this.directoryPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.directoryPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.directoryPanel.Controls.Add(this.directoryLabelLabel);
            this.directoryPanel.Controls.Add(this.directoryLabel);
            this.directoryPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.directoryPanel.Location = new System.Drawing.Point(0, 0);
            this.directoryPanel.Name = "directoryPanel";
            this.directoryPanel.Padding = new System.Windows.Forms.Padding(2);
            this.directoryPanel.Size = new System.Drawing.Size(943, 21);
            this.directoryPanel.TabIndex = 0;
            // 
            // directoryLabelLabel
            // 
            this.directoryLabelLabel.AutoSize = true;
            this.directoryLabelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.directoryLabelLabel.Location = new System.Drawing.Point(5, 2);
            this.directoryLabelLabel.Name = "directoryLabelLabel";
            this.directoryLabelLabel.Size = new System.Drawing.Size(103, 13);
            this.directoryLabelLabel.TabIndex = 6;
            this.directoryLabelLabel.Text = "Current Directory";
            this.directoryLabelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // directoryLabel
            // 
            this.directoryLabel.AutoSize = true;
            this.directoryLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directoryLabel.Location = new System.Drawing.Point(114, 2);
            this.directoryLabel.Name = "directoryLabel";
            this.directoryLabel.Size = new System.Drawing.Size(86, 13);
            this.directoryLabel.TabIndex = 5;
            this.directoryLabel.Text = "Current Directory";
            this.directoryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonPanel
            // 
            this.buttonPanel.AutoSize = true;
            this.buttonPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonPanel.Controls.Add(this.btnRun);
            this.buttonPanel.Controls.Add(this.btnFiles);
            this.buttonPanel.Controls.Add(this.btnDir);
            this.buttonPanel.Controls.Add(this.btnSave);
            this.buttonPanel.Controls.Add(this.settingsLabel);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 515);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(943, 29);
            this.buttonPanel.TabIndex = 4;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(3, 3);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(38, 23);
            this.btnRun.TabIndex = 2;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.RunSheet);
            // 
            // btnFiles
            // 
            this.btnFiles.Location = new System.Drawing.Point(47, 3);
            this.btnFiles.Name = "btnFiles";
            this.btnFiles.Size = new System.Drawing.Size(69, 23);
            this.btnFiles.TabIndex = 5;
            this.btnFiles.Text = "View Files";
            this.btnFiles.UseVisualStyleBackColor = true;
            this.btnFiles.Click += new System.EventHandler(this.ViewFiles);
            // 
            // btnDir
            // 
            this.btnDir.Location = new System.Drawing.Point(122, 3);
            this.btnDir.Name = "btnDir";
            this.btnDir.Size = new System.Drawing.Size(103, 23);
            this.btnDir.TabIndex = 6;
            this.btnDir.Text = "Change Directory";
            this.btnDir.UseVisualStyleBackColor = true;
            this.btnDir.Click += new System.EventHandler(this.ChangeDirectory);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(231, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(82, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save Settings";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.SaveSettings);
            // 
            // settingsLabel
            // 
            this.settingsLabel.AutoSize = true;
            this.settingsLabel.Location = new System.Drawing.Point(319, 8);
            this.settingsLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
            this.settingsLabel.Name = "settingsLabel";
            this.settingsLabel.Size = new System.Drawing.Size(69, 13);
            this.settingsLabel.TabIndex = 4;
            this.settingsLabel.Text = "settingsLabel";
            // 
            // paramsPanel
            // 
            this.paramsPanel.AutoScroll = true;
            this.paramsPanel.AutoSize = true;
            this.paramsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.paramsPanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.paramsPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.paramsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paramsPanel.Location = new System.Drawing.Point(0, 0);
            this.paramsPanel.Name = "paramsPanel";
            this.paramsPanel.Padding = new System.Windows.Forms.Padding(0, 30, 0, 0);
            //this.paramsPanel.Parameters = null;
            this.paramsPanel.Size = new System.Drawing.Size(943, 515);
            this.paramsPanel.TabIndex = 3;
            // 
            // CsmGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(943, 592);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menu);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menu;
            this.MaximizeBox = false;
            this.Name = "CsmGui";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "iCSM";
            this.Activated += new System.EventHandler(this.Activate);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.directoryPanel.ResumeLayout(false);
            this.directoryPanel.PerformLayout();
            this.buttonPanel.ResumeLayout(false);
            this.buttonPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripProgressBar drawProgressBar;
        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSettingsAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel drawStatus;
        private System.Windows.Forms.ToolStripStatusLabel settingsFileStatus;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewFilesToolStripMenuItem;
        private System.Windows.Forms.FlowLayoutPanel buttonPanel;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label settingsLabel;
        private csm.Controls.ParamsPanel paramsPanel;
        private System.Windows.Forms.Button btnFiles;
        private System.Windows.Forms.Button btnDir;
        private System.Windows.Forms.ToolStripMenuItem drawSheetToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel elapsedTime;
        private System.Windows.Forms.Label directoryLabel;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Label directoryLabelLabel;
        private System.Windows.Forms.FlowLayoutPanel directoryPanel;
    }
}
