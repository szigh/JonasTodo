using DAL.Models;
using JonasTodo.Api.DTOs.Mappers;
using Xunit;

namespace JonasTodo.Api.Tests
{
    public class DTOMapper
    {
        [Fact]
        public void Topic_ToDto_MapsPropertiesAndSubtopics()
        {
            // arrange
            var topic = new Topic
            {
                Id = 11,
                Description = "Topic A",
                Priority = 3,
                DateLogged = new DateOnly(2025, 1, 2),
                LongDescriptions = "Long text",
                Subtopics = new List<Subtopic>()
            };

            var sub = new Subtopic
            {
                Id = 21,
                TopicId = 11,
                Topic = topic,
                Description = "Sub A",
                LoggedDate = new DateOnly(2025, 2, 3),
                EstimatedHours = 5,
                Pluralsight = true,
                LongDescription = "Sub long",
                Completed = false,
                Priority = 1
            };

            topic.Subtopics.Add(sub);

            // act
            var dto = topic.ToDto();

            // assert
            Assert.Equal(topic.Id, dto.Id);
            Assert.Equal(topic.Description, dto.Description);
            Assert.Equal(topic.Priority, dto.Priority);
            Assert.Equal(topic.DateLogged, dto.DateLogged);
            Assert.Equal(topic.LongDescriptions, dto.LongDescriptions);

            Assert.NotNull(dto.Subtopics);
            Assert.Single(dto.Subtopics);

            var subDto = dto.Subtopics[0];
            Assert.Equal(sub.Id, subDto.Id);
            Assert.Equal(topic.Description, subDto.Topic);
            Assert.Equal(sub.Description, subDto.Description);
            Assert.Equal(sub.LoggedDate, subDto.LoggedDate);
            Assert.Equal(sub.EstimatedHours, subDto.EstimatedHours);
            Assert.Equal(sub.Pluralsight, subDto.Pluralsight);
            Assert.Equal(sub.LongDescription, subDto.LongDescription);
            Assert.Equal(sub.Completed, subDto.Completed);
            Assert.Equal(sub.Priority, subDto.Priority);
        }

        [Fact]
        public void Subtopic_ToDto_UsesParentDescriptionWhenTopicNull()
        {
            // arrange
            var sub = new Subtopic
            {
                Id = 31,
                TopicId = 5,
                Topic = null, // topic navigation not populated
                Description = "Sub B",
                LongDescription = "Long B",
            };

            // act
            var dto = sub.ToDto(parentTopicDescription: "ParentDesc");

            // assert
            Assert.Equal(sub.Id, dto.Id);
            Assert.Equal("ParentDesc", dto.Topic);
            Assert.Equal(sub.Description, dto.Description);
        }

        [Fact]
        public void ToDto_ThrowsOnNullTopicOrSubtopic()
        {
            Topic? nullTopic = null;
            Subtopic? nullSub = null;

            Assert.Throws<ArgumentNullException>(() => nullTopic!.ToDto());
            Assert.Throws<ArgumentNullException>(() => nullSub!.ToDto());
        }
    }
}