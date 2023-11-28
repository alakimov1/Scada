using Serilog;

namespace Project1.Services
{
    public class WindowsService : BackgroundService
    {
        Serilog.ILogger _logger;
        public WindowsService()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.File("logserv-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Information("WindowsService is starting.");

            stoppingToken.Register(() => _logger.Information("WindowsService is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.Information("WindowsService is doing background work.");

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            _logger.Information("WindowsService has stopped.");
        }
    }
}
