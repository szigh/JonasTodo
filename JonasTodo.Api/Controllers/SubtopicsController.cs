using DAL.Models;
using DAL.Repositories;
using JonasTodo.Api.DTOs;
using JonasTodo.Api.DTOs.Mappers;
using JonasTodo.Api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace JonasTodo.Api.Controllers
{
    [ApiController]
    [Route(RouteNames.Subtopics.ControllerRoute)]
    public class SubtopicsController : ControllerBase
    {
        private readonly ISubtopicRepository _repo;

        public SubtopicsController(ISubtopicRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubtopicDto>>> GetAll(CancellationToken ct)
        {
            // Use GetPredicatedAsync to return all items
            var items = await _repo.GetPredicatedAsync(_ => true, ct);
            return Ok(items.Select(x => x.ToDto()));
        }

        [HttpGet("{id:int}", Name = RouteNames.Subtopics.GetById)]
        public async Task<ActionResult<SubtopicDto>> GetById(int id, CancellationToken ct)
        {
            var item = await _repo.GetByIdAsync(id, ct);
            if (item == null) return NotFound();
            return Ok(item.ToDto());
        }

        [HttpGet("bytopic/{topicId:int}")]
        public async Task<ActionResult<IEnumerable<SubtopicDto>>> GetByTopic(int topicId, CancellationToken ct)
        {
            var items = await _repo.GetByTopicAsync(topicId, ct);
            return Ok(items.Select(x => x.ToDto()));
        }

        [HttpPost]
        public async Task<ActionResult<Subtopic>> Create([FromBody] Subtopic subtopic, CancellationToken ct)
        {
            await _repo.AddAsync(subtopic, ct);
            return CreatedAtAction(nameof(GetById), new { id = subtopic.Id }, subtopic);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Subtopic subtopic, CancellationToken ct)
        {
            if (id != subtopic.Id) return BadRequest("Id mismatch.");
            // repository implements UpdateAsync (added to interface)
            await _repo.UpdateAsync(subtopic, ct);
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
