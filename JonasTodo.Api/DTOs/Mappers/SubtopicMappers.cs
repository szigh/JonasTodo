using DAL.Models;

namespace JonasTodo.Api.DTOs.Mappers;

public static class SubtopicMappers
{
    public static SubtopicDto ToDto(this Subtopic st,
        string? parentTopicDescription = null)
    {
        ArgumentNullException.ThrowIfNull(st);
        var dto = new SubtopicDto(
            st.Id,
            parentTopicDescription ?? st.Topic?.Description ?? string.Empty,
            st.Description ?? string.Empty,
            st.LoggedDate,
            st.EstimatedHours,
            st.Pluralsight,
            st.LongDescription ?? string.Empty,
            st.Completed,
            st.Priority
        );
        return dto;
    }
}
