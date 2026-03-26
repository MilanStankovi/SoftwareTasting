using backend.Models;

namespace backend.Repositories;

public interface IEventRepository
{
    Task<List<Event>> GetAllAsync();
    Task<Event?> GetByIdAsync(string id);
    Task<Event> CreateAsync(Event eventItem);
    Task<bool> UpdateAsync(string id, Event eventItem);
    Task<bool> DeleteAsync(string id);
}