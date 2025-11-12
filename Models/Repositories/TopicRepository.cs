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
        private readonly ILogger<ToDoContext> _logger;

        public TopicRepository(IDbContextFactory<ToDoContext> factory, 
            ILogger<ToDoContext> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async Task<List<Topic>> GetAllAsync(CancellationToken ct = default)
        {
            await using var ctx = _factory.CreateDbContext();
            return await ctx.Topics.AsNoTracking().ToListAsync(ct);
        }

        public async Task<Topic?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var ctx = _factory.CreateDbContext();
            return await ctx.Topics.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);
        }

        public async Task AddAsync(Topic topic, CancellationToken ct = default)
        {
            await using var ctx = _factory.CreateDbContext();
            await ctx.Topics.AddAsync(topic, ct);
            await ctx.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Topic topic, CancellationToken ct = default)
        {
            await using var ctx = _factory.CreateDbContext();
            try
            {
                ctx.Topics.Update(topic);
                await ctx.SaveChangesAsync(ct);
            } catch(DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error while updating topic with id {TopicId}", topic.Id);
                throw;
            }
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await using var ctx = _factory.CreateDbContext();
            var entity = await ctx.Topics.Include(t => t.Subtopics).FirstOrDefaultAsync(t => t.Id == id, ct);
            if (entity == null)
            {
                _logger.LogWarning($"Could not find entity with id {id} to delete.");
                return;
            }
            foreach (var subtopic in entity.Subtopics.ToList())
            {
                ctx.Subtopics.Remove(subtopic);
            }
            ctx.Topics.Remove(entity);
            await ctx.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<Topic>> GetPredicatedAsync(Expression<Func<Topic, bool>> predicate, 
            CancellationToken ct = default)
        {
            await using var ctx = _factory.CreateDbContext();
            return await ctx.Topics.AsNoTracking().Where(predicate).ToListAsync(ct);
        }

        public async IAsyncEnumerable<Topic> StreamAllAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            var ctx = _factory.CreateDbContext();
            await using (ctx.ConfigureAwait(false))
            {
                var enumerator = ctx.Topics.AsNoTracking().AsAsyncEnumerable().GetAsyncEnumerator(ct);
                try
                {
                    while (await enumerator.MoveNextAsync())
                    {
                        yield return enumerator.Current;
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