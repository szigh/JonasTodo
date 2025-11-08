using DAL.Models;
using System.Linq.Expressions;

namespace DAL.Repositories
{
    public interface ISubtopicRepository
    {
        Task<List<Subtopic>> GetByTopicAsync(int topicId);
        Task<Subtopic?> GetByIdAsync(int id);
        Task AddAsync(Subtopic subtopic);
        IAsyncEnumerable<Subtopic> StreamAllAsync(CancellationToken ct = default);
        Task<IEnumerable<Subtopic>> GetPredicatedAsync(Expression<Func<Subtopic, bool>> predicate, CancellationToken ct = default);
    }
}