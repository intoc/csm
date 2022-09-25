
using csm.Business.Logic;
using csm.Business.Models;
using Microsoft.Extensions.Configuration;
using Serilog;

// Logging configuration
var builder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
var config = builder.Build();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

try {
    using ContactSheet cs = new(new FileSourceBuilder(), false);
    cs.ErrorOccurred += (msg, ex) => Log.Error(ex, msg);

    // Check for a -help parameter and handle it
    if (cs.Help(args)) {
        return;
    }

    // Load a settings file if path is provided
    var sFileParamAndValue = args.FirstOrDefault(a => a.ToLower().StartsWith("-sfile="));
    if (sFileParamAndValue != null) {
        cs.LoadParamsFromFile(Param.GetValueFromCmdParamAndValue(sFileParamAndValue));
    }

    // Override loaded settings that were passed in from the command line
    cs.LoadParamsFromCommandLine(args);

    // Search all arguments for a path, use the first one that shows up
    var path = args.FirstOrDefault(a => !a.StartsWith("-"));
    if (path != null) {
        cs.Source = path;
    }

    cs.DrawAndSave(true).Wait();
} catch (Exception ex) {
    Log.Error(ex, "An unhandled Exception occurred.");
} finally {
    Log.CloseAndFlush();
}
