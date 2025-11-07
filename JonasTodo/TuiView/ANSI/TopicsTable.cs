using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using static JonasTodoConsole.Extensions;

namespace JonasTodoConsole.TuiView.ANSI
{
    public class TopicsTable
    {
        private readonly IDbContextFactory<ToDoContext> _dbFactory;

        public TopicsTable(IDbContextFactory<ToDoContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task RunAsync()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine(@"[green italic] _____           _          
|_   _|__  _ __ (_) ___ ___ 
  | |/ _ \| '_ \| |/ __/ __|
  | | (_) | |_) | | (__\__ \
  |_|\___/| .__/|_|\___|___/
          |_|               [/]" + Environment.NewLine);

            using ToDoContext dbContext = await _dbFactory.CreateDbContextAsync();
            AnsiConsole.WriteLine();

            await WriteTable(dbContext);

            AnsiConsole.MarkupLine("Press [green]ENTER[/] to return to the main menu");
            System.Console.ReadKey();
        }

        private async Task WriteTable(ToDoContext dbContext)
        {
            var topics = await dbContext.Topics.ToListAsync();
            var table = new Table()
                .ShowRowSeparators()
                .HeavyHeadBorder()
                .AddColumn("ID")
                .AddColumn("Logged")
                .AddColumn("Description")
                .AddColumn("Long Description")
                .AddColumn("Priority");
            foreach (var topic in topics)
            {
                string priorityStars = GetStars(topic.Priority ?? 0);
                table.AddRow(
                    MarkupNullableCell(topic.Id.ToString()),
                    MarkupNullableCell(topic.DateLogged?.ToString("yyyy-MM-dd")),
                    MarkupNullableCell(topic.Description),
                    MarkupNullableCell(topic.LongDescriptions),
                    new Markup(priorityStars, new Style(foreground: Color.Yellow)));
            }
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();

        }
    }
}
