using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using static JonasTodoConsole.Extensions;

namespace JonasTodoConsole.TuiView.ANSI
{
    public class TopicsTable
    {
        private readonly IDbContextFactory<LearningDbContext> _dbFactory;

        public TopicsTable(IDbContextFactory<LearningDbContext> dbFactory)
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

            using LearningDbContext dbContext = await WriteTable();
            AnsiConsole.WriteLine();
            
            AnsiConsole.Prompt(new TextPrompt<string>("Press [green]ENTER[/] to return to the main menu"));
        }

        private async Task<LearningDbContext> WriteTable()
        {
            var dbContext = _dbFactory.CreateDbContext();
            var topics = await dbContext.Topics.ToListAsync();
            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Logged");
            table.AddColumn("Description");
            table.AddColumn("Long Description");
            table.AddColumn("Priority");
            foreach (var topic in topics)
            {
                string priorityStars = GetStars(topic.Priority ?? 0);
                table.AddRow(
                    MarkupCell(topic.Id.ToString()),
                    MarkupCell(topic.DateLogged?.ToString("yyyy-MM-dd")),
                    MarkupCell(topic.Description),
                    MarkupCell(topic.LongDescriptions),
                    new Markup(priorityStars, new Style(foreground: Color.Yellow)));
            }
            AnsiConsole.Write(table);
            return dbContext;
        }
    }
}
