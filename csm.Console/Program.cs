
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
    using SheetLoader cs = new(new FileSourceBuilder(), Log.Logger, false);
    cs.ErrorOccurred += (msg, isFatal, ex) => Log.Error(ex, "{0} {1}", msg, isFatal ? "[FATAL]" : string.Empty);

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
        await cs.SetSourcePath(path);
    }

    await cs.DrawAndSave();
} catch (Exception ex) {
    Log.Error(ex, "An unhandled Exception occurred.");
} finally {
    Log.CloseAndFlush();
}
