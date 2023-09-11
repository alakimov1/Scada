using System.Diagnostics;

namespace Project1.Processors
{
    public class ProcessorBackgroundWorker : IHostedService, IDisposable
    {
        private Timer? _timer = null;

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(
                _process,
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private async void _process(object? state)
        {
            if (Processor.Instance == null)
                await Processor.Init();

            if (Processor.Instance != null)
                await Processor.Instance.Process();
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
