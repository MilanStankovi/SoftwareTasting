using backend.Models;
using MongoDB.Driver;

namespace backend.Repositories;

public class RegistrationRepository : IRegistrationRepository
{
    private readonly IMongoCollection<Registration> _registrationsCollection;

    public RegistrationRepository(IMongoClient mongoClient, IConfiguration configuration)
    {
        var databaseName = configuration["MongoDb:DatabaseName"];
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException("MongoDb:DatabaseName is not configured.");
        }

        var database = mongoClient.GetDatabase(databaseName);
        _registrationsCollection = database.GetCollection<Registration>("registrations");
    }

    public async Task<List<Registration>> GetAllAsync()
    {
        return await _registrationsCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Registration?> GetByIdAsync(string id)
    {
        return await _registrationsCollection.Find(r => r.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Registration> CreateAsync(Registration registration)
    {
        await _registrationsCollection.InsertOneAsync(registration);
        return registration;
    }

    public async Task<bool> UpdateAsync(string id, Registration updatedRegistration)
    {
        updatedRegistration.Id = id;
        var result = await _registrationsCollection.ReplaceOneAsync(r => r.Id == id, updatedRegistration);
        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _registrationsCollection.DeleteOneAsync(r => r.Id == id);
        return result.DeletedCount > 0;
    }
}
