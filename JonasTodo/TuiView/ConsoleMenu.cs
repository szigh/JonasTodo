using Spectre.Console;
using JonasTodoConsole.TuiView.TableViewer;
using Core;
using JonasTodoConsole.TuiView.TableViewer.Spectre.Console;

namespace JonasTodoConsole.TuiView
{
    internal class ConsoleMenu
    {
        private readonly IConsoleTablePresenter _tablePresenter;
        private readonly IAnsiTableChooser ansiTableChooser;
        private readonly ISelectTable _selectTable;

        public ConsoleMenu(IConsoleTablePresenter tablePresenter, IAnsiTableChooser ansiTableChooser, ISelectTable selectTable)
        {
            _tablePresenter = tablePresenter;
            this.ansiTableChooser = ansiTableChooser;
            _selectTable = selectTable;
        }

        internal int Show()
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

                    return 0;
                }
                else
                {
                    Extensions.H3("Raw console table viewer");

                    var prompt = AnsiConsole.Prompt(new SelectionPrompt<TableEnum>()
                .Title("Choose data to view")
                .PageSize(10)
                .AddChoices([TableEnum.SelectTopics, TableEnum.SelectSubtopics, TableEnum.SelectUnfinishedSubtopics,
                    TableEnum.SelectTopicsAndSubtopics, TableEnum.Exit]));

                    switch (prompt)
                    {
                        case TableEnum.SelectTopics:
                            _tablePresenter.PresentTable(_selectTable.SelectAllTopics());
                            break;
                        case TableEnum.SelectSubtopics:
                            _tablePresenter.PresentTable(_selectTable.SelectAllSubtopics());
                            break;
                        case TableEnum.SelectTopicsAndSubtopics:
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
                Console.WriteLine(e);
                return -1;
            }
        }
    }
}
