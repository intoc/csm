namespace csm;
static class Program {

    [STAThread]
    static void Main(string[] args) {

        Logic.ContactSheet cs = new();

        if (cs.CheckPrintHelp(args)) {
            return;
        }

        // Get a settings file if path is provided
        const string sFileArg = "-sfile";
        var settingsPath = args.FirstOrDefault(a => a.ToLower().StartsWith($"{sFileArg}="));
        if (settingsPath != null) {
            settingsPath = Path.GetFullPath(settingsPath[(settingsPath.IndexOf("=") + 1)..]);
            cs.LoadSettings(settingsPath);
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

        // Load command line arguments
        cs.Load(args);

        // Prompt for arguments graphically
        if (cs.GuiEnabled) {
            // Load default settings
            if (settingsPath == null) {
                settingsPath = Application.ExecutablePath;
                settingsPath = Path.Combine(settingsPath[..settingsPath.LastIndexOf(@"\")], "default.xml");
                cs.LoadSettings(settingsPath);
                cs.Load(args);
            }
            Controls.CsmGui gui = new(cs);

            // Launch a directory chooser if no path was entered
            if (noPathGiven && !gui.ChangeDirectory()) {
                return;
            }

            // Show a GUI for parameter customization
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
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
