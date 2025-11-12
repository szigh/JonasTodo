namespace JonasTodo.Api.DTOs;

public sealed record SubtopicDto(int Id, string Topic, string Description,
    DateOnly? LoggedDate, int? EstimatedHours, bool? Pluralsight, string LongDescription, 
    bool? Completed, int? Priority);

public sealed record CreateSubtopicDto(int TopicId, string Description, int? Priority,
    string? LongDescriptions);

public sealed record UpdateSubtopicDto(string Description, int? Priority, DateOnly? DateLogged,
    string? LongDescriptions);

public sealed record SubtopicSummaryDto(int Id, string Description, int? Priority);
