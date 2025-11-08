using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace DAL.Repositories
{
    public class TopicRepository : ITopicRepository
    {
        private readonly IDbContextFactory<ToDoContext> _factory;
        private readonly ILogger<TopicRepository> _logger;
        private readonly IHostEnvironment _env;

        public TopicRepository(IDbContextFactory<ToDoContext> factory,
            ILogger<TopicRepository> logger,
            IHostEnvironment env //todo remove later
                                 )
        {
            _factory = factory;
            _logger = logger;
            _env = env;
        }

        public async Task<List<Topic>> GetAllAsync(CancellationToken ct = default)
        {
            await using var ctx = _factory.CreateDbContext();
            return await ctx.Topics.AsNoTracking().ToListAsync(ct);
        }

        public async Task<Topic?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var ctx = _factory.CreateDbContext();
            return await ctx.Topics.FindAsync([id], ct);
        }

        public async Task AddAsync(Topic topic, CancellationToken ct = default)
        {
            await using var ctx = _factory.CreateDbContext();
            await ctx.Topics.AddAsync(topic, ct);
            await ctx.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<Topic>> GetPredicatedAsync(Expression<Func<Topic, bool>> predicate, CancellationToken ct = default)
        {
            await using var ctx = _factory.CreateDbContext();
            return await ctx.Topics.AsNoTracking().Where(predicate).ToListAsync(ct);
        }

        public async IAsyncEnumerable<Topic> StreamAllAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            var ctx = _factory.CreateDbContext();
            await using (ctx.ConfigureAwait(false))
            {
                var enumerator = ctx.Topics.AsAsyncEnumerable().GetAsyncEnumerator(ct);
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