using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SharperCraft.Runtime.Host.Services;
using SharperCraft.Runtime.Platform;

namespace SharperCraft.Runtime.Host
{
    public class GenericHost
    {
        public async Task Start(GenericPlatform platform, [CanBeNull] string[] args = null)
        {
            var builder = new HostBuilder()
                        .UseSerilog() // Logs ASP.NET Information messages from middleware into the console.
                        .ConfigureAppConfiguration(configureDelegate: (context, config) =>
                        {
                            config.AddJsonFile("appsettings.json", optional: true);
                            config.AddEnvironmentVariables();

                            if (args != null)
                                config.AddCommandLine(args);
                        })
                         .ConfigureServices(configureDelegate: (context, services) =>
                         {
                             services.AddOptions();
                             services.Configure<AppConfig>(context
                                                         .Configuration
                                                         .GetSection("AppConfig"));
                             services.AddHostedService<TimingService>();
                             services.Configure<AppConfig>(options =>
                             {
                                 options.Platform = platform;
                             });
                         });
            await builder.RunConsoleAsync();
        }
    }
}
