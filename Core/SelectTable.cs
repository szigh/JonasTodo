using DAL.Models;
using JonasTodo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core
{
    public class SelectTable(IServiceScopeFactory serviceScopeFactory) : ISelectTable
    {
        public IEnumerable<Job> SelectAllJobs()
        {
            using var scope = serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<LearningDbContext>();
            return [.. dbContext.Jobs];
        }

        public IEnumerable<Subtopic> SelectAllSubtopics()
        {
            using var scope = serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<LearningDbContext>();
            return [.. dbContext.Subtopics];
        }

        public IEnumerable<Topic> SelectAllTopics()
        {
            using var scope = serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<LearningDbContext>();
            return [.. dbContext.Topics];
        }

        public IEnumerable<Subtopic> SelectUnfinishedSubtopics()
        {
            using var scope = serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<LearningDbContext>();
            return [.. dbContext.Subtopics.Where(subtopic => !(subtopic.Completed ?? false))];
        }

        public async Task<IEnumerable<SubtopicWithTopicDTO>> GetJoinedSubtopicsWithTopics()
        {
            using var scope = serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<LearningDbContext>();
            var dtoResults = await dbContext.Database.SqlQuery<SubtopicWithTopicDTO>( //demonstrating direct use of SQL
$@"SELECT t.Description AS Topic, s.Description AS Subtopic, s.Long_description AS Description, s.Priority, s.Estimated_hours AS 'EstimatedHours'
FROM Subtopics s
RIGHT OUTER JOIN Topics t
ON s.Topic_ID = t.ID
WHERE s.Completed = 0
ORDER BY t.Description, s.Priority DESC;").ToListAsync();
            return dtoResults;
        }
    }
}
