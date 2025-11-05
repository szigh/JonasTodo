using JonasTodoConsole.TuiView.TableViewer.Spectre.Console;
using Microsoft.Identity.Client;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JonasTodoConsole.TuiView.TableViewer
{
    /// <summary>
    /// Choose which table you want to perform CRUID operations on using ANSI table presentation.
    /// </summary>
    public class AnsiTableChooser : IAnsiTableChooser
    {
        public void PresentTable<T>(IEnumerable<T> tableData)
        {
            Extensions.H3("ANSI Table Chooser - Choose a table");
            
            var prompt = AnsiConsole.Prompt<TableEnum>(
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
