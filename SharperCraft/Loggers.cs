using Serilog;
using SharperCrafting;

namespace SharperCraft
{
    internal static class SharpLogs
    {
        private const string LogTemplate =
            "{Timestamp:yyyy-MM-dd HH:mm:ss} | {Level:u3}: [{ThreadId}:{SourceContext}] {Message:lj}{NewLine}{Exception}";

        internal static ILogger MetaLogger(string runtimeUri, string template)
        {
            var config = new LoggerConfiguration()
                        .MinimumLevel.Verbose()
                        .Enrich.FromLogContext()
                        .Enrich.WithThreadId()
                        .WriteTo.File(path: $@"{runtimeUri}\meta.log", outputTemplate: template);
            return config.CreateLogger();
        }
        internal static LoggerConfiguration VerboseLogger()
        {
            var config = new LoggerConfiguration()
                        .MinimumLevel.Verbose()
                        .Enrich.FromLogContext()
                        .Enrich.WithThreadId()
                        .WriteTo.Console(outputTemplate: LogTemplate, theme: ConsoleExtensions.BlueConsole);
            return config;
        }
    }
}
