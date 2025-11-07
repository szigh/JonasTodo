using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace JonasTodoConsole.TuiView.ANSI
{
    public class SubtopicsTable
    {
        private readonly IDbContextFactory<LearningDbContext> _dbFactory;

        public SubtopicsTable(IDbContextFactory<LearningDbContext> dbFactory)
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

            using LearningDbContext dbContext = await WriteTable();
            AnsiConsole.WriteLine();
        }

        private async Task<LearningDbContext> WriteTable()
        {
            var dbContext = _dbFactory.CreateDbContext();
            var subtopics = await dbContext.Subtopics.ToListAsync();
            var topics = await dbContext.Topics.ToListAsync();
            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Logged");
            table.AddColumn("Description");
            table.AddColumn("Long Description");
            table.AddColumn("Topic");
            table.AddColumn("Estimated hours");
            table.AddColumn("Completed");
            table.AddColumn("Priority");
            foreach (var subtopic in subtopics)
            {
                string priorityStars = GetStars(subtopic);
                table.AddRow(
                    new Markup(subtopic.Id.ToString()),
                    new Markup(subtopic.LoggedDate?.ToString("yyyy-MM-dd") ?? "N/A"),
                    new Markup(subtopic.Description ?? "N/A"),
                    new Markup(subtopic.LongDescription ?? "N/A"),
                    new Markup(topics.FirstOrDefault(
                        t => t != null && t.Id == subtopic.TopicId)?.Description ?? "N/A"),
                    new Markup(subtopic.EstimatedHours.ToString() ?? "N/A"),
                    new Markup(subtopic.Priority.ToString() ?? "N/A"),
                    new Markup(priorityStars, new Style(foreground: Color.Yellow)))
                ;
            }
            AnsiConsole.Write(table);
            return dbContext;
        }

        private static string GetStars(Subtopic topic)
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
