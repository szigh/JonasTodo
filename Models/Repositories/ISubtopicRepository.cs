using DAL.Models;
using System.Linq.Expressions;

namespace DAL.Repositories
{
    public interface ISubtopicRepository
    {
        Task<List<Subtopic>> GetByTopicAsync(int topicId, CancellationToken ct = default);
        Task<Subtopic?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(Subtopic subtopic, CancellationToken ct = default);
        Task UpdateAsync(Subtopic subtopic, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        IAsyncEnumerable<Subtopic> StreamAllAsync(CancellationToken ct = default);
        Task<IEnumerable<Subtopic>> GetPredicatedAsync(Expression<Func<Subtopic, bool>> predicate, CancellationToken ct = default);
    }
}