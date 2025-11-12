using DAL.Models;

namespace JonasTodo.Api.DTOs.Mappers;

public static class TopicMappers
{
    public static TopicDto ToDto(this Topic t)
    {
        ArgumentNullException.ThrowIfNull(t);

        var dto = new TopicDto(
            t.Id,
            t.Description ?? string.Empty,
            t.Priority,
            t.DateLogged,
            t.LongDescriptions ?? string.Empty
        )
        {
            Subtopics = t.Subtopics?
                .Select(st => st.ToDto(parentTopicDescription: t.Description))
                .ToList()
                ?? new List<SubtopicDto>()
        };

        return dto;
    }

    public static Topic ToModel(this CreateTopicDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var model = new Topic
        {
            Description = dto.Description,
            Priority = dto.Priority,
            DateLogged = DateOnly.FromDateTime(DateTime.UtcNow),
            LongDescriptions = dto.LongDescriptions
        };
        return model;
    }
}