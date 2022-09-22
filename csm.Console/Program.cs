// Logging configuration
using csm.Business.Logic;
using csm.Business.Models;
using Microsoft.Extensions.Configuration;
using Serilog;

var builder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
var config = builder.Build();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

try {
    using ContactSheet cs = new(new FileSourceBuilder());
    cs.ErrorOccurred += (msg, ex) => Log.Error(ex, msg);

    // Check for a -help parameter and handle it
    if (cs.Help(args)) {
        return;
    }

    // Search all arguments for a path, use the first one that shows up
    var path = args.FirstOrDefault(a => !a.StartsWith("-"));
    if (path != null) {
        cs.Source = path;
    }

    // Load a settings file if path is provided
    var sFileParamAndValue = args.FirstOrDefault(a => a.ToLower().StartsWith("-sfile="));
    if (sFileParamAndValue != null) {
        cs.LoadParamsFromFile(Param.GetValueFromCmdParamAndValue(sFileParamAndValue));
    }

    // Ignore the nogui parameter because it doesn't matter
    cs.LoadParamsFromCommandLine(args.Where(a => !a.StartsWith("-nogui=")));
    cs.DrawAndSave(true).Wait();
} catch (Exception ex) {
    Log.Error(ex, "An unhandled Exception occurred.");
} finally {
    Log.CloseAndFlush();
}
