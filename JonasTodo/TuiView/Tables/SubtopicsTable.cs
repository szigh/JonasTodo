using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using static JonasTodoConsole.Extensions;

namespace JonasTodoConsole.TuiView.Tables
{
    public class SubtopicsTable : ISubtopicsTable
    {
        private readonly IDbContextFactory<ToDoContext> _dbFactory;
        private readonly ISubtopicRepository _subtopicRepository;
        private readonly ITopicRepository _topicRepository;

        public SubtopicsTable(IDbContextFactory<ToDoContext> dbFactory,
            ISubtopicRepository subtopicRepository,
            ITopicRepository topicRepository
            )
        {
            _dbFactory = dbFactory;
            _subtopicRepository = subtopicRepository;
            _topicRepository = topicRepository;
        }

        public async Task RunAsync(CancellationToken ct = default)
        {
            AnsiConsole.Clear();

            //https://www.asciiart.eu/text-to-ascii-art <- would be better to use a library or API
            AnsiConsole.MarkupLine(@"[green italic] ____        _     _              _          
/ ___| _   _| |__ | |_ ___  _ __ (_) ___ ___ 
\___ \| | | | '_ \| __/ _ \| '_ \| |/ __/ __|
 ___) | |_| | |_) | || (_) | |_) | | (__\__ \
|____/ \__,_|_.__/ \__\___/| .__/|_|\___|___/
                           |_|               [/]" + Environment.NewLine);

            await using ToDoContext dbContext = await _dbFactory.CreateDbContextAsync(ct);
            AnsiConsole.WriteLine();

            do
            {
                await DisplayTable(asyncSubtopics: _subtopicRepository.StreamAllAsync(ct), ct: ct);
                AnsiConsole.WriteLine();

                AnsiConsole.MarkupLine("[green italic]Choose what to do next[/]");
                var choice = await AnsiConsole.PromptAsync(new SelectionPrompt<string>()
                    .AddChoices(new List<string>() { "Add subtopic", "Mark subtopic finished", "Show completed subtopics", "Exit" }), ct);

                H3(choice, false);

                if (choice == "Exit")
                    return;

                if (choice == "Add subtopic")
                {
                    var newSubtopic = await PromptSubtopic(ct);
                    if (newSubtopic != null)
                    {
                        await _subtopicRepository.AddAsync(newSubtopic, ct);
                    }
                    continue;
                }

                if (choice == "Mark subtopic finished")
                {
                    await MarkSubtopicFinishedAsync(dbContext, ct);
                    continue;
                }

                if (choice == "Show completed subtopics")
                {
                    await DisplayTable(await _subtopicRepository.GetPredicatedAsync(s => s.Completed == true, ct), ct: ct);
                    AnsiConsole.WriteLine("Press any key to return");
                    Console.ReadKey();
                }

            } while (true);
        }

        private async Task MarkSubtopicFinishedAsync(ToDoContext dbContext, CancellationToken ct)
        {
            await DisplayTable(await _subtopicRepository.GetPredicatedAsync(s => s.Completed != true, ct), ct: ct);
            Subtopic? st = null;
            do
            {
                var subtopicId = await AnsiConsole.PromptAsync(
                    new TextPrompt<int>("Enter the ID of the subtopic to mark as finished (or -1 to cancel):"),
                    ct);
                if (subtopicId == -1)
                    return;
                st = await _subtopicRepository.GetByIdAsync(subtopicId, ct);

            } while (st == null);

            st.Completed = true;
            await dbContext.SaveChangesAsync(ct);
        }

        private async Task<Subtopic?> PromptSubtopic(CancellationToken ct)
        {
            var description = await AnsiConsole.PromptAsync(new TextPrompt<string>("Description:").Validate(s =>
            {
                return s.Length > 0 && s.Length <= 10
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Description must be between 1 and 10 characters.[/]");
            }), ct);
            var longDescription = await AnsiConsole.PromptAsync(new TextPrompt<string>("Long Description:").AllowEmpty(), ct);
            var estimatedHours = await AnsiConsole.PromptAsync(new TextPrompt<int?>("Estimated Hours:").Validate(x =>
            {
                return x != null && new List<int> { 1, 2, 4, 8, 16, 32 }.Contains(x.Value)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Estimated hours must be a power of 2 between 1,2,4,..,32[/]");
            }).AllowEmpty(), ct);
            var pluralsight = BooleanPrompt("Pluralsight course:", false);
            var completed = BooleanPrompt("Completed:", false);
            var priority = await AnsiConsole.PromptAsync(new TextPrompt<int>("Priority (1-5):").Validate(p =>
            {
                return p >= 1 && p <= 5
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Priority must be between 1 and 5.[/]");
            }), ct);
            AnsiConsole.Markup("[italic]Topics[/]");
            var topic = await AnsiConsole.PromptAsync(new SelectionPrompt<Topic>()
                .Title("Select Topic:")
                .PageSize(10)
                .AddChoices(await _topicRepository.GetAllAsync(ct))
                .UseConverter(t => $"{t.Id}. Description: {t.Description ?? ""} {t.LongDescriptions ?? ""}"), ct);
            var subtopic = new Subtopic
            {
                LoggedDate = DateOnly.FromDateTime(DateTime.Now),
                TopicId = topic.Id,
                Description = description,
                LongDescription = longDescription,
                EstimatedHours = estimatedHours,
                Pluralsight = pluralsight,
                Completed = completed,
                Priority = priority
            };

            await DisplayTable([subtopic], ct: ct);
            bool confirmation = BooleanPrompt("Confirm addition to table", true);
            return confirmation ? subtopic : null;
        }

        private async static Task DisplayTable(IEnumerable<Subtopic>? subtopics = default,
            IAsyncEnumerable<Subtopic>? asyncSubtopics = default,
            CancellationToken ct = default)
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
            if (subtopics != null)
            {
                foreach (var subtopic in subtopics)
                {
                    AddSubtopicToTable(table, subtopic);
                }
            }
            if (asyncSubtopics != null)
            {
                await foreach (Subtopic subtopic in asyncSubtopics.WithCancellation(ct))
                {
                    AddSubtopicToTable(table, subtopic);
                    await Task.Delay(100, ct);//for display effect
                }
            }
            AnsiConsole.Write(table);
        }
        static void AddSubtopicToTable(Table table, Subtopic subtopic)
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
    }
}
