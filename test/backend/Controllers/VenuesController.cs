using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenuesController : ControllerBase
{
    private readonly IVenueRepository _venueRepository;

    public VenuesController(IVenueRepository venueRepository)
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
    public async Task<ActionResult<Venue>> GetById(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("Id cannot be null or empty.");
        }
        var venue = await _venueRepository.GetByIdAsync(id);
        if (venue is null)
        {
            return NotFound();
        }

        return Ok(venue);
    }

    [HttpPost]
    public async Task<ActionResult<Venue>> Create([FromBody] Venue? venue)
    {
        if (venue is null)
        {
            return BadRequest("Venue cannot be null.");
        }
        var createdVenue = await _venueRepository.CreateAsync(venue);
        return CreatedAtAction(nameof(GetById), new { id = createdVenue.Id }, createdVenue);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string? id, [FromBody] Venue? venue)
    {
        if (string.IsNullOrEmpty(id) || venue is null || venue.Id != id)
        {
            return BadRequest("Invalid id or venue data.");
        }
        var updated = await _venueRepository.UpdateAsync(id, venue);
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
        var deleted = await _venueRepository.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
