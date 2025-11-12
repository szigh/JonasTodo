using DAL.Models;
using DAL.Repositories;
using JonasTodo.Api.DTOs;
using JonasTodo.Api.DTOs.Mappers;
using JonasTodo.Api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace JonasTodo.Api.Controllers
{
    [ApiController]
    [Route(RouteNames.Topics.ControllerRoute)]
    public class TopicsController : ControllerBase
    {
        private readonly ITopicRepository _repo;

        public TopicsController(ITopicRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TopicDto>>> GetAll(CancellationToken ct)
        {
            var items = await _repo.GetAllAsync(ct);
            var dtos = items.Select(x => x.ToDto());
            return Ok(dtos);
        }

        [HttpGet("{id:int}", Name = RouteNames.Topics.GetById)]
        public async Task<ActionResult<TopicDto>> GetById(int id, CancellationToken ct)
        {
            var item = await _repo.GetByIdAsync(id, ct);
            if (item == null) return NotFound();
            return Ok(item.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult<TopicDto>> Create([FromBody] CreateTopicDto topicDto, CancellationToken ct)
        {
            var topic = topicDto.ToModel();
            await _repo.AddAsync(topic, ct);
            return CreatedAtAction(nameof(GetById), new { id = topic.Id }, topic);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Topic topic, CancellationToken ct)
        {
            if (id != topic.Id) return BadRequest("Id mismatch.");
            await _repo.UpdateAsync(topic, ct);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _repo.DeleteAsync(id, ct);
            return NoContent();
        }
    }
}