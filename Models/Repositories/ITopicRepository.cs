using DAL.Models;
using System.Linq.Expressions;

namespace DAL.Repositories
{
    public interface ITopicRepository
    {
        Task<List<Topic>> GetAllAsync(CancellationToken ct = default);
        Task<Topic?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(Topic topic, CancellationToken ct = default);
        Task<IEnumerable<Topic>> GetPredicatedAsync(Expression<Func<Topic, bool>> predicate, CancellationToken ct = default);
        IAsyncEnumerable<Topic> StreamAllAsync(CancellationToken ct = default);
    }
}