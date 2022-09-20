using csm.Business.Logic;
using csm.Business.Models;
using csm.WinForms.Controls;
using Serilog;

namespace csm;
static class Program {

    private const string DEFAULT_SETTINGS_FILE = "default.xml";

    [STAThread]
    static void Main(string[] args) {

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        try {
            ContactSheet cs = new();
            cs.ErrorOccurred += (msg, ex) => Log.Error(ex, msg);

            // Check for a -help parameter and handle it
            if (cs.Help(args)) {
                return;
            }

            // Search all arguments for a path, use the first one that shows up
            var path = args.FirstOrDefault(a => Directory.Exists(a) || File.Exists(a));
            if (path != null) {
                cs.Source = path;
            }

            // Load a settings file if path is provided
            var sFileArgAndValue = args.FirstOrDefault(a => a.ToLower().StartsWith("-sfile="));
            if (sFileArgAndValue != null) {
                cs.LoadSettingsFromFile(Param.GetValueFromCmdParamAndValue(sFileArgAndValue));
            }

            cs.LoadSettingsFromCommandLine(args);

            // Prompt for arguments graphically
            if (cs.GuiEnabled) {
                // Load default settings file if none was supplied from the CLA
                if (cs.SettingsFile == null && cs.LoadSettingsFromFile(DEFAULT_SETTINGS_FILE)) {
                    // Load parameters from command line arguments again to override the settings file
                    cs.LoadSettingsFromCommandLine(args);
                }

                CsmGui gui = new(cs);
                gui.FormClosed += async (sender, args) => {
                    await Log.CloseAndFlushAsync();
                };

                // Show a GUI for parameter customization
                Application.EnableVisualStyles();
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                gui.ShowDialog();
                gui.Activate();
            } else {
                // Parameters are as they were entered, just go
                cs.DrawAndSave(true).Wait();
                cs.Dispose();
            }
        } catch (Exception ex) {
            Log.Error(ex, "An unhandled Exception occurred.");
        } finally {
            Log.CloseAndFlush();
        }
    }
}
