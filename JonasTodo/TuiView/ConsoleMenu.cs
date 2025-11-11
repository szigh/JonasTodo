using DAL;
using DAL.Models;
using JonasTodoConsole.TuiView.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Spectre.Console;
using static DAL.DIRegistrations;

namespace JonasTodoConsole.TuiView
{
    internal class ConsoleMenu
    {
        private readonly IDbContextFactory<ToDoContext> _dbFactory;
        private readonly ISubtopicsTable _subtopicsTable;
        private readonly ITopicsTable _topicsTable;
        private readonly IConfiguration _configuration;
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly IOptions<DALSettings> _dalSettings;

        public ConsoleMenu(IDbContextFactory<ToDoContext> dbFactory,
            ISubtopicsTable subtopicsTable,
            ITopicsTable topicsTable,
            IConfiguration configuration,
            IConnectionStringProvider connectionStringProvider,
            IOptions<DALSettings> dalSettings)
        {
            _dbFactory = dbFactory;
            _subtopicsTable = subtopicsTable;
            _topicsTable = topicsTable;
            _configuration = configuration;
            _connectionStringProvider = connectionStringProvider;
            _dalSettings = dalSettings;
        }

        internal async Task<int> ShowAsync(CancellationToken stoppingToken, IOptions<DALSettings> dalSettings)
        {
            try
            {
                do
                {
                    // Main menu: choose to open a table, change DB, or exit
                    var mainChoice = await AnsiConsole.PromptAsync(new SelectionPrompt<string>()
                        .Title("Main menu - choose an action")
                        .AddChoices(new[] { "Choose table", "Change database connection", "Exit" }), stoppingToken);

                    if (mainChoice == "Exit")
                        return 0;

                    if (mainChoice == "Change database connection")
                    {
                        await ShowConnectionStringChooserAsync(stoppingToken);
                        continue;
                    }

                    // Choose and open a table
                    var t = await TableChooser.ChooseTable(stoppingToken);

                    switch (t)
                    {
                        case TableEnum.Topics:
                            await _topicsTable.RunAsync(stoppingToken);
                            break;
                        case TableEnum.Subtopics:
                            await _subtopicsTable.RunAsync(stoppingToken);
                            break;
                        default:
                            return -1;
                    }
                } while (!stoppingToken.IsCancellationRequested);
                return 0;
            }
            catch (Exception e)
            {
                AnsiConsole.WriteException(e, ExceptionFormats.ShortenPaths | ExceptionFormats.ShowLinks);
                return -1;
            }
        }

        private async Task ShowConnectionStringChooserAsync(CancellationToken ct = default)
        {
            try
            {
                Extensions.H3("Change database connection");

                var connnectionStrings = new[]{ _dalSettings.Value.LocalConnectionString, _dalSettings.Value.RemoteConnectionString };
               
                var selected = await AnsiConsole.PromptAsync(new SelectionPrompt<string>()
                    .Title("Select connection string to use for future DbContext instances:")
                    .AddChoices(connnectionStrings), ct);

                var confirm = await AnsiConsole.PromptAsync(new ConfirmationPrompt($"Switch to connection '{selected}'?"), ct);
                if (!confirm)
                {
                    AnsiConsole.MarkupLine("[yellow]Cancelled.[/]");
                    return;
                }

                // call provider to change the key for future DbContext creations
                _connectionStringProvider.SetConnectionStringKey(selected);

                AnsiConsole.MarkupLine($"[green]Connection string key set to '{selected}'. New DbContext instances will use the selected database.[/]");
                AnsiConsole.WriteLine("Press any key to continue");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                AnsiConsole.WriteLine("Press any key to continue");
                Console.ReadKey();
            }
        }
    }
}
