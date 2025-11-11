using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using static JonasTodoConsole.Extensions;

namespace JonasTodoConsole.TuiView.Tables
{
    public class TopicsTable : ITopicsTable
    {
        private readonly IDbContextFactory<ToDoContext> _dbFactory;

        public TopicsTable(IDbContextFactory<ToDoContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task RunAsync(CancellationToken ct = default)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine(@"[green italic] _____           _          
|_   _|__  _ __ (_) ___ ___ 
  | |/ _ \| '_ \| |/ __/ __|
  | | (_) | |_) | | (__\__ \
  |_|\___/| .__/|_|\___|___/
          |_|               [/]" + Environment.NewLine);

            using ToDoContext dbContext = await _dbFactory.CreateDbContextAsync(ct);
            AnsiConsole.WriteLine();

            var topics = await dbContext.Topics.ToListAsync(ct);

            do
            {
                DisplayTable(topics);
                AnsiConsole.WriteLine();

                AnsiConsole.MarkupLine("[green italic]Choose what to do next[/]");
                var choice = await AnsiConsole.PromptAsync(new SelectionPrompt<string>()
                    .AddChoices(new List<string>() { "Add topic", "Exit" }), ct);

                if (choice == "Exit")
                    return;
                if (choice == "Add topic")
                {
                    H3("Add new topic", false);
                    Topic? entity = await PromptForTopicDetailsAsync(ct);
                    if (entity != null)
                    {
                        dbContext.Add(entity);
                        await dbContext.SaveChangesAsync(ct);
                    }
                }
            } while (true);
        }

        private static async Task<Topic?> PromptForTopicDetailsAsync(CancellationToken ct = default)
        {
            var description = await AnsiConsole.PromptAsync(new TextPrompt<string>("Description:").Validate(s =>
            {
                return s.Length > 0 && s.Length <= 10
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Description must be between 1 and 10 characters.[/]");
            }), ct);
            var longDescription = await AnsiConsole.PromptAsync(new TextPrompt<string>("Long Description:").AllowEmpty(), ct);
            var priority = await AnsiConsole.PromptAsync(new TextPrompt<int>("Priority (1-5):").Validate(p =>
            {
                return p >= 1 && p <= 5
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Priority must be between 1 and 5.[/]");
            }), ct);
            var topic = new Topic
            {
                Description = description,
                LongDescriptions = longDescription,
                Priority = priority,
                DateLogged = DateOnly.FromDateTime(DateTime.Now)
            };

            DisplayTable([topic]);
            bool confirmation = BooleanPrompt("Confirm addition to table", true);
            if (confirmation)
                return topic;
            else
                return null;
        }

        internal static void DisplayTable(IEnumerable<Topic> topics)
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
