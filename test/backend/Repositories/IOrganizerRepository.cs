using backend.Models;

namespace backend.Repositories;

public interface IOrganizerRepository
{
    Task<List<Organizer>> GetAllAsync();
    Task<Organizer?> GetByIdAsync(string id);
    Task<Organizer> CreateAsync(Organizer organizer);
    Task<bool> UpdateAsync(string id, Organizer organizer);
    Task<bool> DeleteAsync(string id);
}