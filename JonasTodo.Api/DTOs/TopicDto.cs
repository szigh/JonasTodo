namespace JonasTodo.Api.DTOs;

public sealed record TopicDto(int Id, string Description, int? Priority, DateOnly? DateLogged, 
    string LongDescriptions)
{
    public List<SubtopicDto> Subtopics { get; set; } = [];
}

public sealed record CreateTopicDto(string Description, int? Priority,
    string? LongDescriptions);

public sealed record UpdateTopicDto(string Description, int? Priority, DateOnly? DateLogged,
    string? LongDescriptions);

public sealed record TopicSummaryDto(int Id, string Description, int? Priority);
