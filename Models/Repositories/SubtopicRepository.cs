using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace DAL.Repositories
{
    public class SubtopicRepository : ISubtopicRepository
    {
        private readonly IDbContextFactory<ToDoContext> _factory;

        public SubtopicRepository(IDbContextFactory<ToDoContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<Subtopic>> GetByTopicAsync(int topicId, CancellationToken ct = default)
        {
            await using var ctx = _factory.CreateDbContext();
            return await ctx.Subtopics
                .Include(s => s.Topic)
                .Where(s => s.TopicId == topicId)
                .ToListAsync(cancellationToken: ct);
        }

        public async Task<Subtopic?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var ctx = _factory.CreateDbContext();
            return await ctx.Subtopics
                .Include(s => s.Topic)
                .SingleOrDefaultAsync(s => s.Id == id, ct);
        }

        public async Task AddAsync(Subtopic subtopic, CancellationToken ct = default)
        {
            await using var ctx = _factory.CreateDbContext();
            await ctx.Subtopics.AddAsync(subtopic, ct);
            await ctx.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<Subtopic>> GetPredicatedAsync(Expression<Func<Subtopic, bool>> predicate,
            CancellationToken ct = default)
        {
            await using var ctx = _factory.CreateDbContext();
            return await ctx.Subtopics
                .AsNoTracking()
                .Include(s => s.Topic)
                .Where(predicate)
                .ToListAsync(ct);
        }

        public async IAsyncEnumerable<Subtopic> StreamAllAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
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