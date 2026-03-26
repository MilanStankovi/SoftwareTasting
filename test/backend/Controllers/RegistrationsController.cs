using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistrationsController : ControllerBase
{
    private readonly IRegistrationRepository _registrationRepository;

    public RegistrationsController(IRegistrationRepository registrationRepository)
    {
        _registrationRepository = registrationRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Registration>>> GetAll()
    {
        var registrations = await _registrationRepository.GetAllAsync();
        return Ok(registrations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Registration>> GetById(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("Id cannot be null or empty.");
        }
        var registration = await _registrationRepository.GetByIdAsync(id);
        if (registration is null)
        {
            return NotFound();
        }

        return Ok(registration);
    }

    [HttpPost]
    public async Task<ActionResult<Registration>> Create([FromBody] Registration? registration)
    {
        if (registration is null)
        {
            return BadRequest("Registration cannot be null.");
        }
        var createdRegistration = await _registrationRepository.CreateAsync(registration);
        return CreatedAtAction(nameof(GetById), new { id = createdRegistration.Id }, createdRegistration);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string? id, [FromBody] Registration? registration)
    {
        if (string.IsNullOrEmpty(id) || registration is null || registration.Id != id)
        {
            return BadRequest("Invalid id or registration data.");
        }
        var updated = await _registrationRepository.UpdateAsync(id, registration);
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
        var deleted = await _registrationRepository.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
