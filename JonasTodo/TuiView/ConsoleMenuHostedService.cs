using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JonasTodoConsole.TuiView
{
    internal class ConsoleMenuHostedService : BackgroundService
    {
        private readonly ConsoleMenu _menu;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly ILogger<ConsoleMenuHostedService> _logger;

        public ConsoleMenuHostedService(
            ConsoleMenu menu,
            IHostApplicationLifetime appLifetime,
            ILogger<ConsoleMenuHostedService> logger)
        {
            _menu = menu ?? throw new ArgumentNullException(nameof(menu));
            _appLifetime = appLifetime ?? throw new ArgumentNullException(nameof(appLifetime));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Console menu hosted service starting.");

            // Run the blocking menu on a background thread so ExecuteAsync stays responsive to cancellation.
            var showTask = Task.Run(() => _menu.Show(), stoppingToken);

            // Wait for either the menu to finish or for cancellation to be requested.
            var finishedTask = await Task.WhenAny(showTask, Task.Delay(Timeout.Infinite, stoppingToken));

            if (finishedTask == showTask)
            {
                // Menu finished normally
                var exitCode = showTask.Result;
                _logger.LogInformation("Console menu finished with exit code {ExitCode}", exitCode);

                // Tell the host to stop
                _appLifetime.StopApplication();
            }
            else
            {
                // Cancellation requested (Ctrl+C)
                _logger.LogInformation("Console menu cancellation requested.");
            }
        }
    }
}