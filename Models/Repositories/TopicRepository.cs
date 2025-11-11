using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace DAL.Repositories
{
    public class TopicRepository : ITopicRepository
    {
        private readonly IDbContextFactory<ToDoContext> _factory;
        private readonly ILogger<TopicRepository> _logger;

        public TopicRepository(IDbContextFactory<ToDoContext> factory, ILogger<TopicRepository> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async Task<List<Topic>> GetAllAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("GetAllAsync: Retrieving all topics");
            await using var ctx = _factory.CreateDbContext();
            var topics = await ctx.Topics.AsNoTracking().ToListAsync(ct);
            _logger.LogInformation("GetAllAsync: Retrieved {Count} topics", topics.Count);
            return topics;
        }

        public async Task<Topic?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            _logger.LogInformation("GetByIdAsync: Retrieving topic with id {TopicId}", id);
            await using var ctx = _factory.CreateDbContext();
            var topic = await ctx.Topics.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);
            if (topic == null)
            {
                _logger.LogWarning("GetByIdAsync: Topic with id {TopicId} not found", id);
            }
            return topic;
        }

        public async Task AddAsync(Topic topic, CancellationToken ct = default)
        {
            _logger.LogInformation("AddAsync: Adding new topic with description '{Description}'", topic.Description);
            await using var ctx = _factory.CreateDbContext();
            await ctx.Topics.AddAsync(topic, ct);
            await ctx.SaveChangesAsync(ct);
            _logger.LogInformation("AddAsync: Successfully added topic with id {TopicId}", topic.Id);
        }

        public async Task<IEnumerable<Topic>> GetPredicatedAsync(Expression<Func<Topic, bool>> predicate, 
            CancellationToken ct = default)
        {
            _logger.LogInformation("GetPredicatedAsync: Retrieving topics with predicate");
            await using var ctx = _factory.CreateDbContext();
            var topics = await ctx.Topics.AsNoTracking().Where(predicate).ToListAsync(ct);
            _logger.LogInformation("GetPredicatedAsync: Retrieved {Count} topics", topics.Count);
            return topics;
        }

        public async IAsyncEnumerable<Topic> StreamAllAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            _logger.LogInformation("StreamAllAsync: Starting to stream topics");
            var ctx = _factory.CreateDbContext();
            await using (ctx.ConfigureAwait(false))
            {
                var enumerator = ctx.Topics.AsNoTracking().AsAsyncEnumerable().GetAsyncEnumerator(ct);
                try
                {
                    int count = 0;
                    while (await enumerator.MoveNextAsync())
                    {
                        count++;
                        yield return enumerator.Current;
                    }
                    _logger.LogInformation("StreamAllAsync: Streamed {Count} topics", count);
                }
                finally
                {
                    await enumerator.DisposeAsync();
                }
            }
        }
    }
}