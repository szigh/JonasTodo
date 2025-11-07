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
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                        TableEnum.Topics,
                        TableEnum.Subtopics,
                        TableEnum.UnfinishedSubtopics,
                        //TableEnum.TopicsAndSubtopics,
                        TableEnum.Exit
                    }));
        }
    }
}