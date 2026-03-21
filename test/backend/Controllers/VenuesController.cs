using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenuesController : ControllerBase
{
    private readonly VenueRepository _venueRepository;

    public VenuesController(VenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Venue>>> GetAll()
    {
        var venues = await _venueRepository.GetAllAsync();
        return Ok(venues);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Venue>> GetById(string id)
    {
        var venue = await _venueRepository.GetByIdAsync(id);
        if (venue is null)
        {
            return NotFound();
        }

        return Ok(venue);
    }

    [HttpPost]
    public async Task<ActionResult<Venue>> Create([FromBody] Venue venue)
    {
        var createdVenue = await _venueRepository.CreateAsync(venue);
        return CreatedAtAction(nameof(GetById), new { id = createdVenue.Id }, createdVenue);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Venue venue)
    {
        var updated = await _venueRepository.UpdateAsync(id, venue);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _venueRepository.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
