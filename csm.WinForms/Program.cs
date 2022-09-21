using csm.Business.Logic;
using csm.Business.Models;
using csm.WinForms.Controls;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace csm;
static class Program {

    private const string DEFAULT_SETTINGS_FILE = "default.xml";

    [STAThread]
    static void Main(string[] args) {

        // Logging configuration
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        var config = builder.Build();
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
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

            cs.PauseParamEventHandling = true;

            // Load a settings file if path is provided
            var sFileParamAndValue = args.FirstOrDefault(a => a.ToLower().StartsWith("-sfile="));
            if (sFileParamAndValue != null) {
                cs.LoadParamsFromFile(Param.GetValueFromCmdParamAndValue(sFileParamAndValue));
            }

            cs.LoadParamsFromCommandLine(args);

            // Prompt for arguments graphically
            if (cs.GuiEnabled) {
                // Load default settings file if none was supplied from the CLA
                if (cs.SettingsFile == null && cs.LoadParamsFromFile(DEFAULT_SETTINGS_FILE)) {
                    // Load parameters from command line arguments again to override the settings file
                    cs.LoadParamsFromCommandLine(args);
                }
                cs.PauseParamEventHandling = false;

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
                cs.PauseParamEventHandling = false;
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
