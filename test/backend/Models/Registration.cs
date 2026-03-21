using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models;

public class Registration
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string EventId { get; set; } = string.Empty;

    public string AttendeeFullName { get; set; } = string.Empty;

    public string AttendeeEmail { get; set; } = string.Empty;

    public string TicketType { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal PricePaid { get; set; }

    public string PaymentStatus { get; set; } = string.Empty;

    public string CheckInStatus { get; set; } = string.Empty;

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    public string? CancellationReason { get; set; }

    public string? Notes { get; set; }
}
