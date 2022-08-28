using Serilog;
using SharperCraft.Runtime.Platform;
using SharperCrafting;
using System.Reflection;

namespace SharperCraft.Runtime
{
    internal sealed class GenericRuntime
    {
        static GenericRuntime()
        {
            Platform = new GenericPlatform();
            Assembly = GetAssembly<GenericRuntime>();
            ApplicationUri = Path.GetDirectoryName(Assembly.Location) ?? string.Empty;
        }

        private static Assembly GetAssembly<T>() => typeof(T).GetTypeInfo().Assembly;

        private static readonly Assembly Assembly;
        private static readonly string ApplicationUri;
        private static readonly GenericPlatform Platform;
        private ILogger _log = Log.ForContext<GenericRuntime>();

        private const string LogTemplate =
            "{Timestamp:yyyy-MM-dd HH:mm:ss} | {Level:u3}: [{ThreadId}:{SourceContext}] {Message:lj}{NewLine}{Exception}";

        public void Entry()
        {
            try
            {
                var log = VerboseLogger();
                _log = log.ForContext<GenericRuntime>();
                Log.Logger = log;
                //LoggingServices.DefaultBackend = new SerilogLoggingBackend(log.ForContext("RuntimeContext", "PostSharp"));
            }
            catch (Exception ex)
            {
                Platform.Crash();
            }

            try
            {
                //Post.Cast<Runtime, IFreezable>(this).Freeze();
                //Post.Cast<GenericPlatform, IFreezable>(Platform).Freeze();

                var cleanlyLaunch = Platform.Host.Start(Platform);
                var returned = cleanlyLaunch.ContinueWith((t) =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                        Platform
                           .Crash($"Fatal exception in the common platform at runtime{Environment.NewLine}{t.Exception?.ToString()}");
                    else
                        _log
                           .Information(("Successfully launched the common runtime with a platform."));
                });
                returned.Wait();

                //  Results in sink monitors receiving StopMonitoring calls.
                Log.CloseAndFlush();
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Could not freeze and launch the common platform from the generic runtime.{NewLine}{ex}",
                                         Environment.NewLine, ex));
                Platform.Crash("Unrecognized failure occurred in the runtime.");
            }
        }

        private static ILogger VerboseLogger()
        {
            var config = new LoggerConfiguration()
                        .MinimumLevel.Verbose()
                        .Enrich.FromLogContext()
                        .Enrich.WithThreadId()
                        .WriteTo.Console(outputTemplate: LogTemplate, theme: ConsoleExtensions.BlueConsole);
            return config.CreateLogger();
        }

        public static Assembly GetRuntimeAssembly() => Assembly;
        public static string GetRuntimeUri() => ApplicationUri;
    }
}
