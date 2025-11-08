using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using static JonasTodoConsole.Extensions;

namespace JonasTodoConsole.TuiView.ANSI
{
    public class SubtopicsTable
    {
        private readonly IDbContextFactory<ToDoContext> _dbFactory;

        public SubtopicsTable(IDbContextFactory<ToDoContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task RunAsync()
        {
            AnsiConsole.Clear();

            //https://www.asciiart.eu/text-to-ascii-art <- would be better to use a library or API
            AnsiConsole.MarkupLine(@"[green italic] ____        _     _              _          
/ ___| _   _| |__ | |_ ___  _ __ (_) ___ ___ 
\___ \| | | | '_ \| __/ _ \| '_ \| |/ __/ __|
 ___) | |_| | |_) | || (_) | |_) | | (__\__ \
|____/ \__,_|_.__/ \__\___/| .__/|_|\___|___/
                           |_|               [/]" + Environment.NewLine);

            using ToDoContext dbContext = await _dbFactory.CreateDbContextAsync();
            AnsiConsole.WriteLine();

            do
            {
                var subtopics = await dbContext.Subtopics.ToListAsync();
                DisplayTable(subtopics.Where(s => s.Completed == false));
                AnsiConsole.WriteLine();

                AnsiConsole.MarkupLine("[green italic]Choose what to do next[/]");
                var choice = await AnsiConsole.PromptAsync<string>(new SelectionPrompt<string>()
                    .AddChoices(new List<string>() { "Add subtopic", "Mark subtopic finished", "Show completed subtopics", "Exit" }));

                if (choice == "Exit")
                    return;

                if (choice == "Add subtopic")
                {
                    H3("Add new subtopic", false);
                    var newSubtopic = await PromptForSubtopicDetails(dbContext.Topics);
                    if(newSubtopic != null)
                    {
                        dbContext.Add(newSubtopic);
                        await dbContext.SaveChangesAsync();
                    }
                    continue;
                }

                if( choice == "Mark subtopic finished")
                {
                    DisplayTable(subtopics.Where(s => s.Completed == false));
                    var subtopicId = await AnsiConsole.PromptAsync<int>(new TextPrompt<int>("Enter the ID of the subtopic to mark as finished:").Validate(id =>
                    {
                        return subtopics.Any(s => s.Id == id)
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Invalid subtopic ID.[/]");
                    }));
                    var subtopic = subtopics.First(s => s.Id == subtopicId);
                    subtopic.Completed = true;
                    await dbContext.SaveChangesAsync();
                    continue;
                }

                if(choice == "Show completed subtopics")
                {
                    DisplayTable(subtopics.Where(s => s.Completed == true));
                    AnsiConsole.WriteLine("Press any key to return");
                    System.Console.ReadKey();
                }

            } while (true);
        }

        private async Task<Subtopic> PromptForSubtopicDetails(IEnumerable<Topic> topics)
        {
            var description = await AnsiConsole.PromptAsync<string>(new TextPrompt<string>("Description:").Validate(s =>
            {
                return s.Length > 0 && s.Length <= 10
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Description must be between 1 and 10 characters.[/]");
            }));
            var longDescription = await AnsiConsole.PromptAsync<string>(new TextPrompt<string>("Long Description:").AllowEmpty());
            var estimatedHours = await AnsiConsole.PromptAsync<int?>(new TextPrompt<int?>("Estimated Hours:").Validate(x =>
            {
                return x != null && new List<int> { 1, 2, 4, 8, 16, 32 }.Contains(x.Value)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Estimated hours must be a power of 2 between 1,2,4,..,32[/]");
            }).AllowEmpty());
            var pluralsight = BooleanPrompt("Pluralsight course:", false);
            var completed = BooleanPrompt("Completed:", false);
            var priority = await AnsiConsole.PromptAsync<int>(new TextPrompt<int>("Priority (1-5):").Validate(p =>
            {
                return p >= 1 && p <= 5
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Priority must be between 1 and 5.[/]");
            }));
            AnsiConsole.Markup("[italic]Topics[/]");
            TopicsTable.DisplayTable(topics);
            var topic = await AnsiConsole.PromptAsync<Topic>(new SelectionPrompt<Topic>()
                .Title("Select Topic:")
                .PageSize(10)
                .AddChoices(topics)
                .UseConverter(t => t.Id.ToString()));
            var subtopic = new Subtopic
            {
                LoggedDate = DateOnly.FromDateTime(DateTime.Now),
                Topic = topic,
                Description = description,
                LongDescription = longDescription,
                EstimatedHours = estimatedHours,
                Pluralsight = pluralsight,
                Completed = completed,
                Priority = priority
            };

            DisplayTable(new[] { subtopic });
            bool confirmation = BooleanPrompt("Confirm addition to table", true);
            if (confirmation)
                return subtopic;
            else
                return null;
        }

        private static void DisplayTable(IEnumerable<Subtopic> subtopics)
        {
            var table = new Table()
                .ShowRowSeparators()
                .HeavyHeadBorder()
                .AddColumn("ID")
                .AddColumn("Logged")
                .AddColumn("Topic")
                .AddColumn("Description")
                .AddColumn("Long Description")
                .AddColumn("Hrs")
                .AddColumn("Pluralsight")
                .AddColumn("Completed")
                .AddColumn("Priority");
            foreach (var subtopic in subtopics)
            {
                string priorityStars = GetStars(subtopic.Priority ?? 0);
                table.AddRow(
                    MarkupNullableCell(subtopic.Id.ToString()),
                    MarkupNullableCell(subtopic.LoggedDate?.ToString("yyyy-MM-dd")),
                    MarkupNullableCell(subtopic.Topic?.Description),
                    MarkupNullableCell(subtopic.Description),
                    MarkupNullableCell(subtopic.LongDescription),
                    MarkupNullableCell(subtopic.EstimatedHours?.ToString()),
                    MarkupNullableCell(subtopic.Pluralsight?.ToString()),
                    MarkupNullableCell(subtopic.Completed?.ToString()),
                    new Markup(priorityStars, new Style(foreground: Color.Yellow)));
            }
            AnsiConsole.Write(table);
        }
    }
}
