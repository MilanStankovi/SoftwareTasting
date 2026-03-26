using backend.Models;
using MongoDB.Driver;

namespace backend.Repositories;

public class OrganizerRepository : IOrganizerRepository
{
    private readonly IMongoCollection<Organizer> _organizersCollection;

    public OrganizerRepository(IMongoClient mongoClient, IConfiguration configuration)
    {
        var databaseName = configuration["MongoDb:DatabaseName"];
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException("MongoDb:DatabaseName is not configured.");
        }

        var database = mongoClient.GetDatabase(databaseName);
        _organizersCollection = database.GetCollection<Organizer>("organizers");
    }

    public async Task<List<Organizer>> GetAllAsync()
    {
        return await _organizersCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Organizer?> GetByIdAsync(string id)
    {
        return await _organizersCollection.Find(o => o.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Organizer> CreateAsync(Organizer organizer)
    {
        await _organizersCollection.InsertOneAsync(organizer);
        return organizer;
    }

    public async Task<bool> UpdateAsync(string id, Organizer updatedOrganizer)
    {
        updatedOrganizer.Id = id;
        var result = await _organizersCollection.ReplaceOneAsync(o => o.Id == id, updatedOrganizer);
        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _organizersCollection.DeleteOneAsync(o => o.Id == id);
        return result.DeletedCount > 0;
    }
}
