using Serilog;
using SharperCraft.Runtime.Host.Services;
using SharperCraft.Runtime.Platform;

namespace SharperCraft.Win32NT
{
    internal class WindowsTimers : INativeClass
    {
        private readonly ILogger _log = Log.ForContext<WindowsTimers>();

        private Timer _timerShort { get; set; }

        private Timer _timerMedium { get; set; }

        private Timer _timerLong { get; set; }

        private TimingService _parent { get; set; }

        public async Task Initialize<T>(T parent)
        {
            Type parentType = typeof(T);
            if (parentType == typeof(TimingService))
                _parent = (TimingService)(object)parent;

            _timerShort = new Timer(
                                     e => WriteTime("short"),
                                     null,
                                     TimeSpan.FromSeconds(1),
                                     TimeSpan.FromSeconds(1));
            _timerMedium = new Timer(
                                      e => WriteTime("medium"),
                                      null,
                                      TimeSpan.FromSeconds(1),
                                      TimeSpan.FromSeconds(3));
            _timerLong = new Timer(
                                    e => WriteTime("long"),
                                    null,
                                    TimeSpan.FromSeconds(1),
                                    TimeSpan.FromSeconds(5));
        }

        public async Task Terminate(string reason)
        {
            _timerShort.Dispose();
            _timerMedium.Dispose();
            _timerLong.Dispose();
            _log.Verbose(string.Format("[Native Timers]: Disposed of the timers after receiving reason: {reason}", reason));
        }

        public async Task WriteTime(string name)
        {
            _log.Information(string.Format("[Native Timers] : The current time is {UtcNow} on the [ {name} ] timer.", DateTime.UtcNow, name));
        }
    }
}
