using System.Diagnostics;
using Serilog;

namespace Project1.Processors
{
    public class ProcessorBackgroundWorker : IHostedService, IDisposable
    {
        public ProcessorBackgroundWorker() 
        {
        }

        private Timer? _timer = null;
        private bool _processIsBusy = false;

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(
                _process,
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }

        private async void _process(object? state)
        {
            if (_processIsBusy)
                return;

            _processIsBusy = true;

            try
            {
                await Processor.Process();
            }
            catch(Exception ex)
            {
                Log.Error($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
            finally
            { 
                _processIsBusy = false; 
            }
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
