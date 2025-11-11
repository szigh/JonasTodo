using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace DAL.Repositories
{
    public class TopicRepository : ITopicRepository
    {
        private readonly IDbContextFactory<ToDoContext> _factory;

        public TopicRepository(IDbContextFactory<ToDoContext> factory)
        {
            _factory = factory;
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