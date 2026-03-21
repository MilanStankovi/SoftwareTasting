using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizersController : ControllerBase
{
    private readonly OrganizerRepository _organizerRepository;

    public OrganizersController(OrganizerRepository organizerRepository)
    {
        _organizerRepository = organizerRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Organizer>>> GetAll()
    {
        var organizers = await _organizerRepository.GetAllAsync();
        return Ok(organizers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Organizer>> GetById(string id)
    {
        var organizer = await _organizerRepository.GetByIdAsync(id);
        if (organizer is null)
        {
            return NotFound();
        }

        return Ok(organizer);
    }

    [HttpPost]
    public async Task<ActionResult<Organizer>> Create([FromBody] Organizer organizer)
    {
        var createdOrganizer = await _organizerRepository.CreateAsync(organizer);
        return CreatedAtAction(nameof(GetById), new { id = createdOrganizer.Id }, createdOrganizer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Organizer organizer)
    {
        var updated = await _organizerRepository.UpdateAsync(id, organizer);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _organizerRepository.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
