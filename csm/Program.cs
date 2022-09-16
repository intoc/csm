namespace csm;
static class Program {

    private const string DEFAULT_SETTINGS_FILE = "default.xml";

    [STAThread]
    static void Main(string[] args) {

        Logic.ContactSheet cs = new();

        // Check for a -help parameter and handle it
        if (cs.Help(args)) {
            return;
        }

        // Get a settings file if path is provided
        var sFileArgAndValue = args.FirstOrDefault(a => a.ToLower().StartsWith("-sfile="));
        if (sFileArgAndValue != null) {
            var settingsPath = Path.GetFullPath(Models.Param.GetValueFromCmdParamAndValue(sFileArgAndValue));
            cs.LoadSettingsFromFile(settingsPath);
        }

        // Search all arguments for a path, use the first one that shows up
        bool noPathGiven = true;
        var path = args.FirstOrDefault(a => Directory.Exists(a));
        if (path != null) {
            noPathGiven = false;
        } else {
            path = "./";
        }
        cs.SourceDirectory = path;
        cs.LoadSettingsFromCommandLine(args);

        // Prompt for arguments graphically
        if (cs.GuiEnabled) {
            // Load settings
            if (cs.SettingsFile == null) {
                var settingsPath = Application.ExecutablePath;
                settingsPath = Path.Combine(settingsPath[..settingsPath.LastIndexOf(@"\")], DEFAULT_SETTINGS_FILE);
                if (cs.LoadSettingsFromFile(settingsPath)) {
                    // Load parameters from command line arguments again to override the settings file
                    cs.LoadSettingsFromCommandLine(args);
                }
            }

            Controls.CsmGui gui = new(cs);

            // Launch a directory chooser if no path was entered
            if (noPathGiven) {
                gui.ChangeDirectory();
            }

            // Show a GUI for parameter customization
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            gui.ShowDialog();
            gui.Activate();
        } else {
            // Parameters are as they were entered, just go
            cs.DrawAndSave().Wait();
        }
    }

}
