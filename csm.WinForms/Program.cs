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
            ContactSheet cs = new(new FileSourceBuilder());
            cs.ErrorOccurred += (msg, ex) => Log.Error(ex, msg);

            // Check for a -help parameter and handle it
            if (cs.Help(args)) {
                return;
            }

            // Search all arguments for a path, use the first one that shows up
            var path = args.FirstOrDefault(a => !a.StartsWith("-"));

            // Load a settings file
            var sFileParamAndValue = args.FirstOrDefault(a => a.ToLower().StartsWith("-sfile="));
            if (sFileParamAndValue != null) {
                cs.LoadParamsFromFile(Param.GetValueFromCmdParamAndValue(sFileParamAndValue));
            } else if (cs.GuiEnabled) {
                cs.LoadParamsFromFile(DEFAULT_SETTINGS_FILE);
            }

            // Any parameters from the command line should override the settings file
            cs.LoadParamsFromCommandLine(args);

            // Prompt for arguments graphically
            if (cs.GuiEnabled) {
                if (path != null) {
                    cs.Source = path;
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
                if (path != null) {
                    cs.Source = path;
                }
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
