namespace DAL.Models
{
    public class SubtopicWithTopicDTO
    {
        public string? Topic { get; set; }
        public string? Subtopic { get; set; }
        public string? Description { get; set; }
        public int Priority { get; set; }
        public int EstimatedHours { get; set; }
    }
}
