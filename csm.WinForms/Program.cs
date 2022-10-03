using csm.Business.Logic;
using csm.Business.Models;
using csm.WinForms.Controls;
using csm.WinForms.Models.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace csm;
static class Program {

    public static IConfigurationRoot ConfigurationRoot { get; }
    public static AppSettings AppSettings { get; }
    public static IServiceProvider Services { get; }

    private const string DEFAULT_SETTINGS_FILE = "default.xml";
    private const string APPSETTINGS_JSON_FILE = "appsettings.json";

    static Program() {
        // Logging configuration
        var configBuilder = new ConfigurationBuilder()
            .AddJsonFile(APPSETTINGS_JSON_FILE, optional: true, reloadOnChange: true);

        ConfigurationRoot = configBuilder.Build();
        AppSettings = ConfigurationRoot.GetSection("AppSettings").Get<AppSettings>();

        var builder = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) => {
                services
                    .AddTransient(provider => new LoggerConfiguration()
                        .ReadFrom.Configuration(ConfigurationRoot)
                        .CreateLogger()
                        .ForContext("Context", "NoContext"))
                    .AddTransient<IFileSourceBuilder, FileSourceBuilder>()
                    .AddTransient<SheetLoader>();
            });
        var host = builder.Build();
        Services = host.Services;
        Log.Logger = Services.GetRequiredService<ILogger>();
    }

    [STAThread]
    static void Main(string[] args) {

        try {
            SheetLoader cs = Services.GetRequiredService<SheetLoader>();
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
                CsmGui gui = new(cs, AppSettings);
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
                    // Parameters are as they were entered, just go
                    cs.SetSourcePath(path).Wait();
                    cs.DrawAndSave().Wait();
                    cs.Dispose();
                } else {
                    Log.Error("No source path specified. Provide a source path as a nameless argument. Exiting.");
                }
            }
        } catch (Exception ex) {
            Log.Error(ex, "An unhandled Exception occurred.");
        } finally {
            Log.CloseAndFlush();
        }
    }
}
