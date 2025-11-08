using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace DAL.Repositories
{
    public class SubtopicRepository : ISubtopicRepository
    {
        private readonly IDbContextFactory<ToDoContext> _factory;
        private readonly ILogger<SubtopicRepository> _logger;
        private readonly IHostEnvironment _env;

        public SubtopicRepository(IDbContextFactory<ToDoContext> factory,
            ILogger<SubtopicRepository> logger,
            IHostEnvironment env //todo remove
                                 )
        {
            _factory = factory;
            _logger = logger;
            _env = env;
        }

        public async Task<List<Subtopic>> GetByTopicAsync(int topicId)
        {
            using var ctx = _factory.CreateDbContext();
            return await ctx.Subtopics
                .Where(s => s.TopicId == topicId)
                .ToListAsync();
        }

        public async Task<Subtopic?> GetByIdAsync(int id)
        {
            using var ctx = _factory.CreateDbContext();
            return await ctx.Subtopics.FindAsync(id);
        }

        public async Task AddAsync(Subtopic subtopic)
        {
            using var ctx = _factory.CreateDbContext();
            await ctx.Subtopics.AddAsync(subtopic);
            await ctx.SaveChangesAsync();
        }

        public async Task<IEnumerable<Subtopic>> GetPredicatedAsync(Expression<Func<Subtopic, bool>> predicate,
            CancellationToken ct = default)
        {
            using var ctx = _factory.CreateDbContext();
            return await ctx.Subtopics.AsNoTracking().Where(predicate).ToListAsync(ct);
        }

        public async IAsyncEnumerable<Subtopic> StreamAllAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            var ctx = _factory.CreateDbContext();
            await using (ctx.ConfigureAwait(false))
            {
                var enumerator = ctx.Subtopics.AsAsyncEnumerable().GetAsyncEnumerator(ct);
                Stopwatch? sw = null;
                try
                {
                    if (_env.IsDevelopment()) sw = Stopwatch.StartNew();
                    while (await enumerator.MoveNextAsync())
                    {
                        yield return enumerator.Current;
                    }
                    if (_env.IsDevelopment() && sw != null)
                    {
                        sw.Stop();
                        _logger.LogInformation($"Streamed all Topics in {sw.ElapsedMilliseconds}");
                    }
                }
                finally
                {
                    await enumerator.DisposeAsync();
                }
            }
        }
    }
}