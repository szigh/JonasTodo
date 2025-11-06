using DAL.Models;
using JonasTodoConsole.TuiView.TableViewer.Spectre.Console;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using System.Text;

namespace JonasTodoConsole.TuiView.DataInterfaces
{
    internal class TopicsTable : IRead, IUpdate, ITopicsTable
    {
        private readonly IDbContextFactory<LearningDbContext> _contextFac;

        public TopicsTable(IDbContextFactory<LearningDbContext> contextFactory)
        {
            _contextFac = contextFactory;
        }

        internal async Task RunAsync()
        {
            using var dbContext = _contextFac.CreateDbContext();
            //var results = dbContext.Topics.AsQueryable()..ToList<Topic>();
            //DrawTableToConsole(results);

        }

        public bool TryUpdate<T>(int id, Func<T, T> updateFunction)
        {
            throw new NotImplementedException();
        }

        bool IRead.TryGet<T>(out T data)
        {
            throw new NotImplementedException();
        }

        void DrawTableToConsole(IEnumerable<Topic>? results)
        {
            if (results == null || !results.Any())
            {
                AnsiConsole.MarkupLine("[red]No topics found.[/]");
                return;
            }

            var table = new Table();

            table.AddColumn("ID");
            table.AddColumn("Logged");
            table.AddColumn("Description");
            table.AddColumn("Long Description");
            table.AddColumn("Priority");

            foreach (var topic in results)
            {
                var priority = topic.Priority ?? 0;
                var sb = new StringBuilder(5, 5);
                if (topic.Priority > 0)
                {
                    while (priority-- > 0)
                    {
                        sb.Append(":star:");
                    }
                }
                table.AddRow(
                    topic.Id.ToString(),
                    topic.DateLogged?.ToString("yyyy-MM-dd") ?? "N/A",
                    topic.Description ?? "N/A",
                    topic.LongDescriptions ?? "N/A",
                    sb.ToString()
                );
            }

            AnsiConsole.Write(table);
        }
    }
}
