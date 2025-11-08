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
            DisplayTable(topics);
            AnsiConsole.WriteLine();


            do
            {
                AnsiConsole.MarkupLine("[green italic]Choose what to do next[/]");
                var choice = await AnsiConsole.PromptAsync<string>(new SelectionPrompt<string>()
                    .AddChoices(new List<string>() { "Add topic", "Exit" }));

                if (choice == "Exit")
                    return;

                H3("Add new topic", false);
                topics.Add(await PromptForTopicDetails());
            } while (true);
        }

        private async Task<Topic> PromptForTopicDetails()
        {
            var description = await AnsiConsole.PromptAsync<string>(new TextPrompt<string>("Description:").Validate(s =>
            {
                return s.Length > 0 && s.Length <= 10
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Description must be between 1 and 10 characters.[/]");
            }));
            var longDescription = await AnsiConsole.PromptAsync<string>(new TextPrompt<string>("Long Description:").AllowEmpty());
            var priority = await AnsiConsole.PromptAsync<int>(new TextPrompt<int>("Priority (1-5):").Validate(p =>
            {
                return p >= 1 && p <= 5
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Priority must be between 1 and 5.[/]");
            }));
            var topic = new Topic
            {
                Description = description,
                LongDescriptions = longDescription,
                Priority = priority,
                DateLogged = DateOnly.FromDateTime(DateTime.Now)
            };
            return topic;
        }

        private static void DisplayTable(List<Topic> topics)
        {
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
        }
    }
}
