using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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

            await WriteTable(dbContext);

            AnsiConsole.WriteLine("Press [green]ENTER[/] to return to the main menu");
            System.Console.ReadKey();
        }

        private async Task WriteTable(ToDoContext dbContext)
        {
            var subtopics = await dbContext.Subtopics.ToListAsync();
            var topics = await dbContext.Topics.ToListAsync();
            var table = new Table()
                .Border(TableBorder.DoubleEdge)
                .ShowRowSeparators()
                .MinimalHeavyHeadBorder()
                .AddColumn("ID")
                .AddColumn("Logged")
                .AddColumn("Description")
                .AddColumn("Long Description")
                .AddColumn("Topic")
                .AddColumn("Hrs")
                .AddColumn("Completed")
                .AddColumn("Priority");
            foreach (var subtopic in subtopics)
            {
                string priorityStars = GetStars(subtopic.Priority ?? 0, Emoji);
                table.AddRow(
                    MarkupNullableCell(subtopic.Id.ToString()),
                    MarkupNullableCell(subtopic.LoggedDate?.ToString("yyyy-MM-dd") ?? "N/A"),
                    MarkupNullableCell(subtopic.Description ?? "N/A"),
                    MarkupNullableCell(subtopic.LongDescription ?? "N/A"),
                    MarkupNullableCell(subtopic.Topic?.Description ?? "N/A"),
                    MarkupNullableCell(subtopic.EstimatedHours.ToString() ?? "N/A"),
                    MarkupNullableCell(subtopic.Completed == false || subtopic.Completed == null ? "N/A" : "False"),
                    new Markup(priorityStars, new Style(foreground: Color.Yellow)));
                
            }
            AnsiConsole.Write(table);
            return;
        }
    }
}
