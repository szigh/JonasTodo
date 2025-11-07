using Core;
using DAL.Models;
using JonasTodoConsole.TuiView.ANSI;
using JonasTodoConsole.TuiView.Console;
using JonasTodoConsole.TuiView.TableViewer;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace JonasTodoConsole.TuiView
{
    internal class ConsoleMenu
    {
        private readonly IConsoleTablePresenter _tablePresenter;
        private readonly ISelectTable _selectTable;
        private readonly IDbContextFactory<ToDoContext> _dbFactory;

        public ConsoleMenu(IConsoleTablePresenter tablePresenter,
            ISelectTable selectTable,
            IDbContextFactory<ToDoContext> dbFactory)
        {
            _tablePresenter = tablePresenter;
            _selectTable = selectTable;
            _dbFactory = dbFactory;
        }

        internal async Task<int> ShowAsync(CancellationToken stoppingToken)
        {
            try
            {
                Extensions.H3("Main menu");

                var tableType = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title("Choose table type")
                    .PageSize(10)
                    .AddChoices(new[] { "Ansi Table Viewer", "Simple Console Table Viewer" }));

                
                if (tableType == "Ansi Table Viewer")
                {
                    TableEnum t;
                    do
                    {
                        t = await AnsiTableChooser.ChooseTable();

                        switch (t)
                        {
                            case TableEnum.Topics:
                                await new TopicsTable(_dbFactory).RunAsync();
                                break;
                            case TableEnum.Subtopics:
                                await new SubtopicsTable(_dbFactory).RunAsync();
                                break;
                            case TableEnum.UnfinishedSubtopics:
                                AnsiConsole.WriteException(new Exception("Not implemented yet"));
                                break;
                            case TableEnum.TopicsAndSubtopics:
                                AnsiConsole.WriteException(new Exception("Not implemented yet"));
                                break;
                            default:
                                return -1;
                        }
                    } while (t != TableEnum.Exit); // Loop until user chooses to exit
                    return 0;
                }
                else
                {

                    Extensions.H3("Raw console table viewer");

                    var prompt = AnsiConsole.Prompt(new SelectionPrompt<TableEnum>()
                .Title("Choose data to view")
                .PageSize(10)
                .AddChoices([TableEnum.Topics, TableEnum.Subtopics, TableEnum.UnfinishedSubtopics,
                    TableEnum.TopicsAndSubtopics, TableEnum.Exit]));

                    switch (prompt)
                    {
                        case TableEnum.Topics:
                            _tablePresenter.PresentTable(_selectTable.SelectAllTopics());
                            break;
                        case TableEnum.Subtopics:
                            _tablePresenter.PresentTable(_selectTable.SelectAllSubtopics());
                            break;
                        case TableEnum.TopicsAndSubtopics:
                            _tablePresenter.PresentTable(_selectTable.SelectUnfinishedSubtopics()
                                .OrderByDescending(t => t.Priority)
                                .ThenByDescending(t => t.EstimatedHours));
                            break;
                        case TableEnum.Exit:
                        default:
                            return 0;
                    }
                    return 0;
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                //SUB(11) add logger
                return -1;
            }
        }
    }
}
