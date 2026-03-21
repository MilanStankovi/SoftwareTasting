using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models;

public class Event
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public int MaxAttendees { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal BasePrice { get; set; }

    public string Currency { get; set; } = "EUR";

    [BsonRepresentation(BsonType.ObjectId)]
    public string OrganizerId { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string VenueId { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = [];
}
