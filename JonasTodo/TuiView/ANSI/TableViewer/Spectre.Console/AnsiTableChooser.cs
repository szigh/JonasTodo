using JonasTodoConsole.TuiView.ANSI.TableViewer.Spectre.Console;
using Spectre.Console;

namespace JonasTodoConsole.TuiView.TableViewer
{
    /// <summary>
    /// Choose which table you want to perform CRUID operations on using ANSI table presentation.
    /// </summary>
    public static class AnsiTableChooser
    {
        public static async Task<TableEnum> ChooseTable()
        {
            Extensions.H3("ANSI Table Chooser - Choose a table");
            
            return await AnsiConsole.PromptAsync(
                new SelectionPrompt<TableEnum>()
                    .Title("Choose data to view")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                        TableEnum.SelectTopics,
                        TableEnum.SelectSubtopics,
                        TableEnum.SelectUnfinishedSubtopics,
                        TableEnum.SelectTopicsAndSubtopics,
                        TableEnum.Exit
                    })
            );

        }
    }
}
