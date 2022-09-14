using csm.Logic;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace csm;
static class Program {

    [STAThread]
    static void Main(string[] args) {

        ContactSheet cs = new();

        if (cs.CheckPrintHelp(args)) {
            return;
        }

        bool noPathGiven = true;

        // Search all arguments for a path, use the first one that shows up
        var path = args.FirstOrDefault(a => Directory.Exists(a));
        if (path != null) {
            noPathGiven = false;
        } else {
            path = "./";
        }

        cs.SourceDirectory = path;

        // Load command line arguments, exit if load failed.
        if (!cs.Load(args)) {
            return;
        }

        // Prompt for arguments graphically
        if (cs.GuiEnabled) {
            // Load default settings
            string settingsPath = Application.ExecutablePath;
            settingsPath = settingsPath[..settingsPath.LastIndexOf(@"\")];
            cs.SettingsFile = Path.Combine(settingsPath, "default.xml");
            cs.LoadSettings(cs.SettingsFile);
            cs.Load(args);

            // Launch a directory chooser if no path was entered
            if (noPathGiven) {
                FolderBrowserDialog folder = new() {
                    Description = "Select the folder containing the images:",
                    SelectedPath = Directory.GetParent("./").ToString()
                };
                if (folder.ShowDialog() == DialogResult.OK) {
                    cs.SourceDirectory = folder.SelectedPath;
                } else {
                    // Canceled
                    return;
                }
            }

            // Show a GUI for parameter customization
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Controls.CsmGui gui = new(cs);
            gui.ShowDialog();
            gui.Activate();
        } else {
            // Parameters are as they were entered, just go
            if (!cs.Run()) {
                Application.Exit();
            }
        }
    }
}
