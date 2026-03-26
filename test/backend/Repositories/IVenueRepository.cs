using backend.Models;

namespace backend.Repositories;

public interface IVenueRepository
{
    Task<List<Venue>> GetAllAsync();
    Task<Venue?> GetByIdAsync(string id);
    Task<Venue> CreateAsync(Venue venue);
    Task<bool> UpdateAsync(string id, Venue venue);
    Task<bool> DeleteAsync(string id);
}