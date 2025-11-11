using DAL.Models;
using JonasTodoConsole.TuiView.Tables;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace JonasTodoConsole.TuiView
{
    internal class ConsoleMenu
    {
        private readonly IDbContextFactory<ToDoContext> _dbFactory;
        private readonly ISubtopicsTable _subtopicsTable;
        private readonly ITopicsTable _topicsTable;

        public ConsoleMenu(IDbContextFactory<ToDoContext> dbFactory,
            ISubtopicsTable subtopicsTable,
            ITopicsTable topicsTable)
        {
            _dbFactory = dbFactory;
            _subtopicsTable = subtopicsTable;
            _topicsTable = topicsTable;
        }

        internal async Task<int> ShowAsync(CancellationToken stoppingToken)
        {
            try
            {
                TableEnum t;
                do
                {
                    t = await TableChooser.ChooseTable(stoppingToken);

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
                } while (t != TableEnum.Exit); // Loop until user chooses to exit
                return 0;
            }
            catch (Exception e)
            {
                AnsiConsole.WriteException(e, ExceptionFormats.ShortenPaths | ExceptionFormats.ShowLinks);
                return -1;
            }
        }
    }
}
