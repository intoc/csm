using csm.Business.Logic;
using csm.Business.Models;
using System.Diagnostics;

namespace csm.WinForms.Controls;
public partial class CsmGui : Form {

    private readonly ContactSheet cs;

    public delegate void UpdateProgressDelegate(DrawProgressEventArgs args);
    public delegate void UpdateSettingsStatusDelegate(SettingsChangedEventArgs args);

    private readonly FileList fileListWindow;

    /// <summary>
    /// Main Constructor
    /// </summary>
    public CsmGui(ContactSheet sheet) {
        InitializeComponent();
        
        // Initialize status elements
        drawStatus.Text = string.Empty;
        settingsFileStatus.Text = string.Empty;
        var directoryLabelText = (string? path) => string.IsNullOrEmpty(path) ? "None Selected" : path;
        directoryLabel.Text = directoryLabelText(sheet.Source);
        cs = sheet;

        foreach (Param p in cs.Params) {
            paramsPanel.AddParamControl(p);
        }

        cs.DrawProgressChanged += new DrawProgressEventHandler(DrawProgressChanged);
        cs.SettingsChanged += new SettingsChangedEventHandler(SettingsChanged);
        cs.ErrorOccurred += new ExceptionEventHandler(ExceptionOccurred);
        cs.SourceChanged += (path) => Invoke(() => {
            directoryLabel.Text = directoryLabelText(path);
            SetButtonsEnabled(true, true);
        });

        settingsLabel.Text = cs.SettingsFile;

        fileListWindow = new FileList(cs);
        SetButtonsEnabledIfFilesLoaded();
    }

    private void SetButtonsEnabledIfFilesLoaded() {
        SetButtonsEnabled(cs.FirstLoadComplete, cs.Source != null || cs.FirstLoadComplete);
    }

    private void SetButtonsEnabled(bool run, bool choose) {
        btnRun.Enabled = run;
        btnArchive.Enabled = choose;
        btnFolder.Enabled = choose;
        chooseFolderToolStripMenuItem.Enabled = choose;
        chooseArchiveToolStripMenuItem.Enabled = choose;
        drawSheetToolStripMenuItem.Enabled = choose;
    }

    void Exit(object? sender, EventArgs e) {
        Close();
    }

    void ExceptionOccurred(string message, Exception? e) {
        MessageBox.Show($"{message}\n\n{(e?.Message == null ? string.Empty : $"Exception: {e.Message}")}", "Error!",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    void DrawProgressChanged(DrawProgressEventArgs args) {
        if (statusStrip.InvokeRequired) {
            object[] argsArr = { args };
            statusStrip.Invoke(new UpdateProgressDelegate(UpdateDrawProgress), argsArr);
        } else {
            UpdateDrawProgress(args);
        }
    }
    void SettingsChanged(SettingsChangedEventArgs args) {
        if (statusStrip.InvokeRequired) {
            object[] argsArr = { args };
            statusStrip.Invoke(new UpdateSettingsStatusDelegate(UpdateSettingsStatus), argsArr);
        } else {
            UpdateSettingsStatus(args);
        }
    }

    public void UpdateDrawProgress(DrawProgressEventArgs args) {
        drawProgressBar.Value = args.Percentage;
        elapsedTime.Text = $"{args.Time.Minutes:00}:{args.Time.Seconds:00}";
        if (args.Percentage < 100) {
            drawStatus.Text = $"Drawing {args.Percentage}%";
        } else {
            drawStatus.Text = "Drawing Finished!";
            // Open the folder
            if (cs.OpenOutputDir) {
                Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", Path.GetDirectoryName(cs.OutFilePath(0)) ?? string.Empty);
            }
        }
    }

    public void UpdateSettingsStatus(SettingsChangedEventArgs args) {
        if (args.Passed) {
            settingsLabel.Text = args.SettingsFile;
        }
        settingsFileStatus.Text = args.Message;
    }

    private async void RunSheet(object sender, EventArgs e) {
        try {
            bool exit = await cs.DrawAndSave();
            if (exit) {
                Application.Exit();
            }
        } catch (Exception ex) {
            Console.Error.WriteLine(ex);
            ExceptionOccurred("Unhandled", ex);
        }
    }

    private string GetDirectoryFromSource() {
        string? directory = null;
        if (Directory.Exists(cs.Source)) {
            directory = cs.Source;
        } else if (cs.Source != null) {
            directory = Directory.GetParent(cs.Source)?.FullName;
        }
        return directory ?? string.Empty;
    }

    public void ChooseArchive() {
        OpenFileDialog ofd = new() {
            Title = "Select the archive containing the images",
            InitialDirectory = GetDirectoryFromSource(),
            CheckPathExists = true,
            CheckFileExists = true,
            DefaultExt = "zip",
            Filter = "Archive files (*.zip, *.rar, *.7z)|*.zip;*.rar;*.7z"
        };
        if (ofd.ShowDialog() == DialogResult.OK) {
            SetButtonsEnabled(false, false);
            cs.Source = ofd.FileName;
        }
    }

    public void ChooseFolder() {
        FolderBrowserDialog folder = new() {
            Description = "Select the folder containing the images",
            SelectedPath = GetDirectoryFromSource()
        };
        if (folder.ShowDialog() == DialogResult.OK) {
            SetButtonsEnabled(false, false);
            cs.Source = folder.SelectedPath;
        }
    }

    private void ChooseArchive(object sender, EventArgs e) => ChooseArchive();

    private void ChooseFolder(object sender, EventArgs e) => ChooseFolder();


    private void LoadSettings(object sender, EventArgs e) {
        OpenFileDialog ofd = new() {
            InitialDirectory = Application.ExecutablePath
        };

        if (ofd.ShowDialog() == DialogResult.OK) {
            cs.LoadParamsFromFile(ofd.FileName);
        }
    }

    private void SaveSettingsAs(object sender, EventArgs e) {
        SaveFileDialog sfd = new() {
            InitialDirectory = Application.ExecutablePath,
            Filter = "XML files|*.xml",
            AddExtension = true,
            DefaultExt = ".xml"
        };

        if (sfd.ShowDialog() == DialogResult.OK) {
            cs.SaveSettings(sfd.FileName);
        }
    }

    private void SaveSettings(object sender, EventArgs e) {
        cs.SaveSettings();
    }

    private void ViewFiles(object sender, EventArgs e) {
        SetButtonsEnabledIfFilesLoaded();
        if (!fileListWindow.Visible) {
            fileListWindow.Show(this);
        }

        fileListWindow.Activate();
    }

    private void Activate(object sender, EventArgs e) {
        // If the user is attempting to open a menu item at
        // activation, oblige them!
        Point p = PointToClient(Cursor.Position);
        ToolStripMenuItem i = (ToolStripMenuItem)menu.GetItemAt(p);
        i?.ShowDropDown();
    }

    private void CsmGui_FormClosed(object sender, FormClosedEventArgs e) {
        cs.Dispose();
    }

}

