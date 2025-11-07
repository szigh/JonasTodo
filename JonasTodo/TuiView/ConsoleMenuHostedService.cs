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

            // Start the async menu directly and let it honor the cancellation token.
            var showTask = _menu.ShowAsync(stoppingToken);

            // Wait for either the menu to finish or for cancellation to be requested.
            var finishedTask = await Task.WhenAny(showTask, Task.Delay(Timeout.Infinite, stoppingToken));

            if (finishedTask == showTask)
            {
                try
                {
                    var exitCode = await showTask;
                    _logger.LogInformation("Console menu finished with exit code {ExitCode}", exitCode);

                    Environment.ExitCode = exitCode;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Console menu threw an unhandled exception.");
                    Environment.ExitCode = -1;
                }

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