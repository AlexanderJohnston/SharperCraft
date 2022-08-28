using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using SharperCraft.Runtime.Platform;

namespace SharperCraft.Runtime.Host.Services
{
    public class TimingService : IHostedService
    {
        private GenericPlatform _platform;

        private readonly ILogger _log = Log.ForContext<TimingService>();

        private INativeClass _nativeTimers { get; set; }

        public TimingService(IOptions<AppConfig> options)
        {
            _platform = options.Value.Platform;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _log.Information("[Timing Service]: Calling out to the platform for native timers.");
            _nativeTimers = (INativeClass)_platform.GetNativeClass("SharperCraft", "WindowsTimers");
            await _nativeTimers.Initialize(this);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _log.Warning("[Timing Service]: Terminating this service.");
            await _nativeTimers.Terminate("The timing service is being shut down by the host.");
        }
    }
}
