using csm.Logic;
using System;
using System.IO;
using System.Windows.Forms;

namespace csm {

    class Program {

        [STAThread]
        static void Main(string[] args) {

            ContactSheet cs = new ContactSheet();

            if (cs.CheckPrintHelp(args)) {
                return;
            }

            bool noPathGiven = true;

            // Search all arguments for a path, use the first one that shows up
            string path = "./";
            foreach (string s in args) {
                if (Directory.Exists(s)) {
                    path = s;
                    noPathGiven = false;
                    break;
                }
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
                settingsPath = settingsPath.Substring(0, settingsPath.LastIndexOf(@"\"));
                cs.SettingsFile = @settingsPath + @"\default.xml";
                cs.LoadSettings(cs.SettingsFile);
                cs.Load(args);

                // Launch a directory chooser if no path was entered
                if (noPathGiven) {
                    FolderBrowserDialog folder = new FolderBrowserDialog {
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

                // Show a gui for parameter customization
                Application.EnableVisualStyles();
                Controls.CsmGui gui = new Controls.CsmGui(cs);
                gui.ShowDialog();
                gui.Activate();
            } else {
                // Parameters are as they were entered
                // Just go
                if (!cs.Run()) {
                    Application.Exit();
                }
            }
        }
    }
}
