using csm.Logic;
using csm.Models;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace csm.Controls; 
public partial class CsmGui : Form {

    private ContactSheet cs;

    public delegate void UpdateProgressDelegate(DrawProgressEventArgs args);
    public delegate void UpdateSettingsStatusDelegate(SettingsChangedEventArgs args);

    private FileList fileListWindow;

    public ContactSheet Sheet {
        get { return cs; }
        set {
            cs = value;
            foreach (Param p in cs.Params) {
                paramsPanel.AddParamControl(p);
            }
            
            cs.DrawProgressChanged += new DrawProgressEventHandler(DrawProgressChanged);
            cs.SettingsChanged += new SettingsChangedEventHandler(SettingsChanged);
            cs.ExceptionOccurred += new ExceptionEventHandler(ExceptionOccurred);

            settingsLabel.Text = cs.SettingsFile;

            fileListWindow = new FileList(cs);
        }
    }

    /// <summary>
    /// Main Constructor
    /// </summary>
    public CsmGui(ContactSheet cs) {
        InitializeComponent();
        Application.EnableVisualStyles();

        // Set up ESC exit
        Button btnCancel = new();
        CancelButton = btnCancel;
        btnCancel.Click += new EventHandler(Exit);

        // Initialize status elements
        drawStatus.Text = string.Empty;
        settingsFileStatus.Text = string.Empty;
        directoryLabel.Text = cs.SourceDirectory;
        cs.SourceDirectoryChanged += (path) => directoryLabel.Text = path;
        Sheet = cs;
    }

    void Exit(object sender, EventArgs e) {
        Close();
    }

    void ExceptionOccurred(Exception e) {
        MessageBox.Show(e.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        elapsedTime.Text = string.Format("{0:00}:{1:00}", args.Time.Minutes, args.Time.Seconds);
        if (args.Percentage < 100) {
            drawStatus.Text = string.Format("Drawing {0}%", args.Percentage);
        } else {
            drawStatus.Text = string.Format("Drawing Finished!");
            // Open the folder
            if (cs.OpenOutputDir) {
                System.Diagnostics.Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", Path.GetDirectoryName(cs.OutFilePath));
            }
        }
    }

    public void UpdateSettingsStatus(SettingsChangedEventArgs args) {
        if (args.Passed) {
            settingsLabel.Text = args.SettingsFile;
        }
        settingsFileStatus.Text = args.Message;
    }

    private void RunSheet(object sender, EventArgs e) {
        try {
            ThreadPool.QueueUserWorkItem(new WaitCallback((state) => {
                if (!cs.Run()) {
                    Application.Exit();
                }
            }));
        } catch (Exception ex) {
            MessageBox.Show(ex.Message);
        }
    }

    private void ChangeDirectory(object sender, EventArgs e) {
        FolderBrowserDialog folder = new() {
            Description = "Select the folder containing the images:",
            SelectedPath = cs.SourceDirectory
        };
        if (folder.ShowDialog() == DialogResult.OK) {
            cs.SourceDirectory = folder.SelectedPath;
        }
    }

    private void LoadSettings(object sender, EventArgs e) {
        OpenFileDialog ofd = new() {
            InitialDirectory = Application.ExecutablePath
        };

        if (ofd.ShowDialog() == DialogResult.OK) {
            cs.LoadSettings(ofd.FileName);
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
        if (!fileListWindow.Visible) {
            fileListWindow.Show(this);
        }

        fileListWindow.Activate();
    }

    private void Activate(object sender, EventArgs e) {
        // If the user is attempting to open a menu item at
        // activation, oblige them!
        System.Drawing.Point p = PointToClient(Cursor.Position);
        ToolStripMenuItem i = (ToolStripMenuItem)menu.GetItemAt(p);
        i?.ShowDropDown();
    }

}

