using DAL.Models;

namespace Core
{
    public interface ISelectTable
    {
        IEnumerable<Job> SelectAllJobs();
        IEnumerable<Topic> SelectAllTopics();
        IEnumerable<Subtopic> SelectAllSubtopics();
        IEnumerable<Subtopic> SelectUnfinishedSubtopics();
    }
}
