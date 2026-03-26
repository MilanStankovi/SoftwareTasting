using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventRepository _eventRepository;

    public EventsController(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Event>>> GetAll()
    {
        var events = await _eventRepository.GetAllAsync();
        return Ok(events);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Event>> GetById(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("Id cannot be null or empty.");
        }
        var eventItem = await _eventRepository.GetByIdAsync(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        return Ok(eventItem);
    }

    [HttpPost]
    public async Task<ActionResult<Event>> Create([FromBody] Event? eventItem)
    {
        if (eventItem is null)
        {
            return BadRequest("Event cannot be null.");
        }
        var createdEvent = await _eventRepository.CreateAsync(eventItem);
        return CreatedAtAction(nameof(GetById), new { id = createdEvent.Id }, createdEvent);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string? id, [FromBody] Event? eventItem)
    {
        if (string.IsNullOrEmpty(id) || eventItem is null || eventItem.Id != id)
        {
            return BadRequest("Invalid id or event data.");
        }
        var updated = await _eventRepository.UpdateAsync(id, eventItem);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("Id cannot be null or empty.");
        }
        var deleted = await _eventRepository.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
