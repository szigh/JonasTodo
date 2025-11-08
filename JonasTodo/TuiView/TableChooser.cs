using Spectre.Console;

namespace JonasTodoConsole.TuiView
{
    /// <summary>
    /// Choose which table you want to perform CRUID operations on using ANSI table presentation.
    /// </summary>
    public static class TableChooser
    {
        public static async Task<TableEnum> ChooseTable(CancellationToken ct = default)
        {
            Extensions.H3("ANSI Table Chooser - Choose a table");

            return await AnsiConsole.PromptAsync(new SelectionPrompt<TableEnum>()
                    .PageSize(10)
                    .AddChoices(
                    [
                        TableEnum.Topics,
                        TableEnum.Subtopics,
                        //TableEnum.UnfinishedSubtopics,
                        //TableEnum.TopicsAndSubtopics,
                        TableEnum.Exit
                    ]), ct);
        }
    }
}