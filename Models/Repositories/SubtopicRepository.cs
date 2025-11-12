using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace DAL.Repositories
{
    public class SubtopicRepository : ISubtopicRepository
    {
        private readonly IDbContextFactory<ToDoContext> _factory;
        private readonly ILogger<SubtopicRepository> _logger;

        public SubtopicRepository(IDbContextFactory<ToDoContext> factory, ILogger<SubtopicRepository> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async Task<List<Subtopic>> GetByTopicAsync(int topicId, CancellationToken ct = default)
        {
            _logger.LogInformation("GetByTopicAsync: Retrieving subtopics for topic {TopicId}", topicId);
            await using var ctx = _factory.CreateDbContext();
            var subtopics = await ctx.Subtopics
                .AsNoTracking()
                .Include(s => s.Topic)
                .Where(s => s.TopicId == topicId)
                .ToListAsync(cancellationToken: ct);
            _logger.LogInformation("GetByTopicAsync: Retrieved {Count} subtopics for topic {TopicId}", subtopics.Count, topicId);
            return subtopics;
        }

        public async Task<Subtopic?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            _logger.LogInformation("GetByIdAsync: Retrieving subtopic with id {SubtopicId}", id);
            await using var ctx = _factory.CreateDbContext();
            var subtopic = await ctx.Subtopics
                .AsNoTracking()
                .Include(s => s.Topic)
                .SingleOrDefaultAsync(s => s.Id == id, ct);
            if (subtopic == null)
            {
                _logger.LogWarning("GetByIdAsync: Subtopic with id {SubtopicId} not found", id);
            }
            return subtopic;
        }

        public async Task AddAsync(Subtopic subtopic, CancellationToken ct = default)
        {
            _logger.LogInformation("AddAsync: Adding new subtopic with description '{Description}' for topic {TopicId}", 
                subtopic.Description, subtopic.TopicId);
            await using var ctx = _factory.CreateDbContext();
            await ctx.Subtopics.AddAsync(subtopic, ct);
            await ctx.SaveChangesAsync(ct);
            _logger.LogInformation("AddAsync: Successfully added subtopic with id {SubtopicId}", subtopic.Id);
        }

        public async Task<IEnumerable<Subtopic>> GetPredicatedAsync(Expression<Func<Subtopic, bool>> predicate,
            CancellationToken ct = default)
        {
            _logger.LogInformation("GetPredicatedAsync: Retrieving subtopics with predicate");
            await using var ctx = _factory.CreateDbContext();
            var subtopics = await ctx.Subtopics
                .AsNoTracking()
                .Include(s => s.Topic)
                .Where(predicate)
                .ToListAsync(ct);
            _logger.LogInformation("GetPredicatedAsync: Retrieved {Count} subtopics", subtopics.Count);
            return subtopics;
        }

        public async IAsyncEnumerable<Subtopic> StreamAllAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            _logger.LogInformation("StreamAllAsync: Starting to stream subtopics");
            var ctx = _factory.CreateDbContext();
            await using (ctx.ConfigureAwait(false))
            {
                var enumerator = ctx.Subtopics
                    .AsNoTracking()
                    .Include(s => s.Topic)
                    .AsAsyncEnumerable()
                    .GetAsyncEnumerator(ct);
                try
                {
                    int count = 0;
                    while (await enumerator.MoveNextAsync())
                    {
                        count++;
                        yield return enumerator.Current;
                    }
                    _logger.LogInformation("StreamAllAsync: Streamed {Count} subtopics", count);
                }
                finally
                {
                    await enumerator.DisposeAsync();
                }
            }
        }
    }
}