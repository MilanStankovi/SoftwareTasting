using backend.Models;

namespace backend.Repositories;

public interface IRegistrationRepository
{
    Task<List<Registration>> GetAllAsync();
    Task<Registration?> GetByIdAsync(string id);
    Task<Registration> CreateAsync(Registration registration);
    Task<bool> UpdateAsync(string id, Registration registration);
    Task<bool> DeleteAsync(string id);
}