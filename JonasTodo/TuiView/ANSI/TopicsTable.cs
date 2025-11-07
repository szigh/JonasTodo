using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

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
                string priorityStars = GetStars(topic);
                table.AddRow(
                    new Markup(topic.Id.ToString()),
                    new Markup(topic.DateLogged?.ToString("yyyy-MM-dd") ?? "N/A"),
                    new Markup(topic.Description ?? "N/A"),
                    new Markup(topic.LongDescriptions ?? "N/A"),
                    new Markup(priorityStars, new Style(foreground: Color.Yellow)))
                ;
            }
            AnsiConsole.Write(table);
            return dbContext;
        }

        private static string GetStars(Topic topic)
        {
            var priorityStars = string.Empty;
            if (topic.Priority != null)
            {
                for (int i = 0; i < topic.Priority; i++)
                {
                    priorityStars += "[yellow]:star:[/]";
                }
            }

            return priorityStars;
        }
    }
}
