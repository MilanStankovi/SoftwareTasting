using backend.Models;
using MongoDB.Driver;

namespace backend.Repositories;

public class EventRepository : IEventRepository
{
    private readonly IMongoCollection<Event> _eventsCollection;

    public EventRepository(IMongoClient mongoClient, IConfiguration configuration)
    {
        var databaseName = configuration["MongoDb:DatabaseName"];
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException("MongoDb:DatabaseName is not configured.");
        }

        var database = mongoClient.GetDatabase(databaseName);
        _eventsCollection = database.GetCollection<Event>("events");
    }

    public async Task<List<Event>> GetAllAsync()
    {
        return await _eventsCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Event?> GetByIdAsync(string id)
    {
        return await _eventsCollection.Find(e => e.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Event> CreateAsync(Event eventItem)
    {
        await _eventsCollection.InsertOneAsync(eventItem);
        return eventItem;
    }

    public async Task<bool> UpdateAsync(string id, Event updatedEvent)
    {
        updatedEvent.Id = id;
        var result = await _eventsCollection.ReplaceOneAsync(e => e.Id == id, updatedEvent);
        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _eventsCollection.DeleteOneAsync(e => e.Id == id);
        return result.DeletedCount > 0;
    }
}
