namespace csm.WinForms.Controls;

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
            this.chooseFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chooseArchiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawSheetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.directoryPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.directoryLabelLabel = new System.Windows.Forms.Label();
            this.directoryLabel = new System.Windows.Forms.Label();
            this.paramsPanel = new csm.WinForms.Controls.ParamsPanel();
            this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnFiles = new System.Windows.Forms.Button();
            this.btnFolder = new System.Windows.Forms.Button();
            this.btnArchive = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.settingsLabel = new System.Windows.Forms.Label();
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
            this.statusStrip.Location = new System.Drawing.Point(0, 583);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip.ShowItemToolTips = true;
            this.statusStrip.Size = new System.Drawing.Size(1004, 27);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "status";
            // 
            // drawProgressBar
            // 
            this.drawProgressBar.ForeColor = System.Drawing.SystemColors.ControlText;
            this.drawProgressBar.Name = "drawProgressBar";
            this.drawProgressBar.Size = new System.Drawing.Size(175, 21);
            this.drawProgressBar.ToolTipText = "Drawing Progress";
            // 
            // drawStatus
            // 
            this.drawStatus.Name = "drawStatus";
            this.drawStatus.Size = new System.Drawing.Size(65, 22);
            this.drawStatus.Text = "drawStatus";
            // 
            // elapsedTime
            // 
            this.elapsedTime.Name = "elapsedTime";
            this.elapsedTime.Size = new System.Drawing.Size(47, 22);
            this.elapsedTime.Text = "Elapsed";
            // 
            // settingsFileStatus
            // 
            this.settingsFileStatus.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.settingsFileStatus.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.settingsFileStatus.Name = "settingsFileStatus";
            this.settingsFileStatus.Size = new System.Drawing.Size(102, 22);
            this.settingsFileStatus.Text = "settingsFileStatus";
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menu.Size = new System.Drawing.Size(1004, 24);
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
            this.chooseFolderToolStripMenuItem,
            this.chooseArchiveToolStripMenuItem,
            this.viewFilesToolStripMenuItem,
            this.drawSheetToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // chooseFolderToolStripMenuItem
            // 
            this.chooseFolderToolStripMenuItem.Name = "chooseFolderToolStripMenuItem";
            this.chooseFolderToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.chooseFolderToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.chooseFolderToolStripMenuItem.Text = "Choose Folder";
            this.chooseFolderToolStripMenuItem.Click += new System.EventHandler(this.ChooseFolder);
            // 
            // chooseArchiveToolStripMenuItem
            // 
            this.chooseArchiveToolStripMenuItem.Name = "chooseArchiveToolStripMenuItem";
            this.chooseArchiveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.chooseArchiveToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.chooseArchiveToolStripMenuItem.Text = "Choose Archive";
            this.chooseArchiveToolStripMenuItem.Click += new System.EventHandler(this.ChooseArchive);
            // 
            // viewFilesToolStripMenuItem
            // 
            this.viewFilesToolStripMenuItem.Name = "viewFilesToolStripMenuItem";
            this.viewFilesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.viewFilesToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.viewFilesToolStripMenuItem.Text = "View Files";
            this.viewFilesToolStripMenuItem.Click += new System.EventHandler(this.ViewFiles);
            // 
            // drawSheetToolStripMenuItem
            // 
            this.drawSheetToolStripMenuItem.Name = "drawSheetToolStripMenuItem";
            this.drawSheetToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.drawSheetToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
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
            this.pnlMain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1004, 559);
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
            this.directoryPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.directoryPanel.Name = "directoryPanel";
            this.directoryPanel.Padding = new System.Windows.Forms.Padding(2);
            this.directoryPanel.Size = new System.Drawing.Size(1004, 23);
            this.directoryPanel.TabIndex = 0;
            // 
            // directoryLabelLabel
            // 
            this.directoryLabelLabel.AutoSize = true;
            this.directoryLabelLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.directoryLabelLabel.Location = new System.Drawing.Point(6, 2);
            this.directoryLabelLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.directoryLabelLabel.Name = "directoryLabelLabel";
            this.directoryLabelLabel.Size = new System.Drawing.Size(92, 15);
            this.directoryLabelLabel.TabIndex = 6;
            this.directoryLabelLabel.Text = "Current Source";
            this.directoryLabelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // directoryLabel
            // 
            this.directoryLabel.AutoSize = true;
            this.directoryLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directoryLabel.Location = new System.Drawing.Point(106, 2);
            this.directoryLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.directoryLabel.Name = "directoryLabel";
            this.directoryLabel.Size = new System.Drawing.Size(86, 15);
            this.directoryLabel.TabIndex = 5;
            this.directoryLabel.Text = "Current Source";
            this.directoryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.paramsPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.paramsPanel.Name = "paramsPanel";
            this.paramsPanel.Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
            this.paramsPanel.Size = new System.Drawing.Size(1004, 526);
            this.paramsPanel.TabIndex = 3;
            // 
            // buttonPanel
            // 
            this.buttonPanel.AutoSize = true;
            this.buttonPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonPanel.Controls.Add(this.btnRun);
            this.buttonPanel.Controls.Add(this.btnFiles);
            this.buttonPanel.Controls.Add(this.btnFolder);
            this.buttonPanel.Controls.Add(this.btnArchive);
            this.buttonPanel.Controls.Add(this.btnSave);
            this.buttonPanel.Controls.Add(this.settingsLabel);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 526);
            this.buttonPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(1004, 33);
            this.buttonPanel.TabIndex = 4;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(4, 3);
            this.btnRun.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(44, 27);
            this.btnRun.TabIndex = 2;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.RunSheet);
            // 
            // btnFiles
            // 
            this.btnFiles.Location = new System.Drawing.Point(56, 3);
            this.btnFiles.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnFiles.Name = "btnFiles";
            this.btnFiles.Size = new System.Drawing.Size(80, 27);
            this.btnFiles.TabIndex = 5;
            this.btnFiles.Text = "View Files";
            this.btnFiles.UseVisualStyleBackColor = true;
            this.btnFiles.Click += new System.EventHandler(this.ViewFiles);
            // 
            // btnFolder
            // 
            this.btnFolder.Location = new System.Drawing.Point(144, 3);
            this.btnFolder.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(120, 27);
            this.btnFolder.TabIndex = 7;
            this.btnFolder.Text = "Choose Folder";
            this.btnFolder.UseVisualStyleBackColor = true;
            this.btnFolder.Click += new System.EventHandler(this.ChooseFolder);
            // 
            // btnArchive
            // 
            this.btnArchive.Location = new System.Drawing.Point(272, 3);
            this.btnArchive.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnArchive.Name = "btnArchive";
            this.btnArchive.Size = new System.Drawing.Size(120, 27);
            this.btnArchive.TabIndex = 6;
            this.btnArchive.Text = "Choose Archive";
            this.btnArchive.UseVisualStyleBackColor = true;
            this.btnArchive.Click += new System.EventHandler(this.ChooseArchive);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(400, 3);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(96, 27);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save Settings";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.SaveSettings);
            // 
            // settingsLabel
            // 
            this.settingsLabel.AutoSize = true;
            this.settingsLabel.Location = new System.Drawing.Point(504, 9);
            this.settingsLabel.Margin = new System.Windows.Forms.Padding(4, 9, 4, 0);
            this.settingsLabel.Name = "settingsLabel";
            this.settingsLabel.Size = new System.Drawing.Size(76, 15);
            this.settingsLabel.TabIndex = 4;
            this.settingsLabel.Text = "settingsLabel";
            // 
            // CsmGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 610);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menu);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menu;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "CsmGui";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Intelligent Contact Sheet Maker";
            this.Activated += new System.EventHandler(this.Activate);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CsmGui_FormClosed);
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
    private System.Windows.Forms.ToolStripMenuItem chooseFolderToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem viewFilesToolStripMenuItem;
    private System.Windows.Forms.FlowLayoutPanel buttonPanel;
    private System.Windows.Forms.Button btnRun;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Label settingsLabel;
    private csm.WinForms.Controls.ParamsPanel paramsPanel;
    private System.Windows.Forms.Button btnFiles;
    private System.Windows.Forms.Button btnArchive;
    private System.Windows.Forms.ToolStripMenuItem drawSheetToolStripMenuItem;
    private System.Windows.Forms.ToolStripStatusLabel elapsedTime;
    private System.Windows.Forms.Label directoryLabel;
    private System.Windows.Forms.Panel pnlMain;
    private System.Windows.Forms.Label directoryLabelLabel;
    private System.Windows.Forms.FlowLayoutPanel directoryPanel;
    private Button btnFolder;
    private ToolStripMenuItem chooseArchiveToolStripMenuItem;
}
