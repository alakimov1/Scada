using Project1.Processors;
using Serilog;

namespace Project1.Services.Analytics
{
    public class Analytics
    {
        private static Analytics? Instance;
        private Serilog.ILogger? _logger;

        private void Init()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.File("analytics-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        private void Information(string message)
        {
            if (_logger == null)
                Init();

            if (_logger != null)
                _logger.Information(message);
        }

        public static void Write(string message)
        {
            if (Instance == null)
            {
                Instance = new Analytics();
            }

            Instance.Information(message);
        }
    }
}
