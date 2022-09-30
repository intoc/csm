using csm.Business.Logic;
using csm.Business.Models;
using csm.WinForms.Controls;
using csm.WinForms.Models.Settings;
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

        var appSettings = config.GetSection("AppSettings").Get<AppSettings>();

        try {
            SheetLoader cs = new(new FileSourceBuilder());
            cs.ErrorOccurred += (msg, isFatal, ex) => Log.Error(ex, msg);

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
                CsmGui gui = new(cs, appSettings);
                gui.FormClosed += async (sender, args) => await Log.CloseAndFlushAsync();
                
                if (path != null) {
                    // Delay setting the source path until after the main GUI has loaded
                    // so the buttons will be set to enabled/disabled correctly
                    gui.Load += async (sender, args) => await cs.SetSourcePath(path);
                }

                // Show a GUI for parameter customization
                Application.EnableVisualStyles();
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                gui.ShowDialog();
                gui.Activate();
            } else {
                if (path != null) {
                    cs.SetSourcePath(path).Wait();
                }
                // Parameters are as they were entered, just go
                cs.DrawAndSave().Wait();
                cs.Dispose();
            }
        } catch (Exception ex) {
            Log.Error(ex, "An unhandled Exception occurred.");
        } finally {
            Log.CloseAndFlush();
        }
    }
}
