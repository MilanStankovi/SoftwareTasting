using backend.Models;
using MongoDB.Driver;

namespace backend.Repositories;

public class VenueRepository : IVenueRepository
{
    private readonly IMongoCollection<Venue> _venuesCollection;

    public VenueRepository(IMongoClient mongoClient, IConfiguration configuration)
    {
        var databaseName = configuration["MongoDb:DatabaseName"];
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException("MongoDb:DatabaseName is not configured.");
        }

        var database = mongoClient.GetDatabase(databaseName);
        _venuesCollection = database.GetCollection<Venue>("venues");
    }

    public async Task<List<Venue>> GetAllAsync()
    {
        return await _venuesCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Venue?> GetByIdAsync(string id)
    {
        return await _venuesCollection.Find(v => v.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Venue> CreateAsync(Venue venue)
    {
        await _venuesCollection.InsertOneAsync(venue);
        return venue;
    }

    public async Task<bool> UpdateAsync(string id, Venue updatedVenue)
    {
        updatedVenue.Id = id;
        var result = await _venuesCollection.ReplaceOneAsync(v => v.Id == id, updatedVenue);
        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _venuesCollection.DeleteOneAsync(v => v.Id == id);
        return result.DeletedCount > 0;
    }
}
