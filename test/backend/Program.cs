using backend.Models;
using backend.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var mongoConnectionString = builder.Configuration["MongoDb:ConnectionString"];
if (string.IsNullOrWhiteSpace(mongoConnectionString))
{
    throw new InvalidOperationException("MongoDb:ConnectionString is not configured.");
}

builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnectionString));
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IOrganizerRepository, OrganizerRepository>();
builder.Services.AddScoped<IVenueRepository, VenueRepository>();
builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontendPolicy");
app.UseHttpsRedirection();
app.MapControllers();

app.MapGet("/api/health/db", async (IMongoClient client, IConfiguration config) =>
{
    var databaseName = config["MongoDb:DatabaseName"];
    if (string.IsNullOrWhiteSpace(databaseName))
    {
        return Results.Problem("MongoDb:DatabaseName is not configured.");
    }

    var database = client.GetDatabase(databaseName);
    await database.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
    return Results.Ok(new { status = "MongoDB connected" });
})
.WithName("MongoHealth");

app.MapPost("/api/seed", async (IMongoClient client, IConfiguration config) =>
{
    var databaseName = config["MongoDb:DatabaseName"];
    if (string.IsNullOrWhiteSpace(databaseName))
    {
        return Results.Problem("MongoDb:DatabaseName is not configured.");
    }

    var database = client.GetDatabase(databaseName);
    var organizersCollection = database.GetCollection<Organizer>("organizers");
    var venuesCollection = database.GetCollection<Venue>("venues");
    var eventsCollection = database.GetCollection<Event>("events");
    var registrationsCollection = database.GetCollection<Registration>("registrations");

    if (await organizersCollection.Find(_ => true).AnyAsync())
    {
        return Results.Ok(new { message = "Seed skipped: data already exists." });
    }

    var organizer1Id = ObjectId.GenerateNewId().ToString();
    var organizer2Id = ObjectId.GenerateNewId().ToString();
    var venue1Id = ObjectId.GenerateNewId().ToString();
    var venue2Id = ObjectId.GenerateNewId().ToString();
    var event1Id = ObjectId.GenerateNewId().ToString();
    var event2Id = ObjectId.GenerateNewId().ToString();

    var organizers = new List<Organizer>
    {
        new()
        {
            Id = organizer1Id,
            Name = "TechFuture Events",
            Email = "contact@techfuture.rs",
            Phone = "+38160111222",
            CompanyName = "TechFuture DOO",
            TaxId = "109876543",
            Address = "Bulevar Oslobodjenja 10",
            City = "Novi Sad",
            Country = "Serbia",
            IsVerified = true,
            CreatedAt = DateTime.UtcNow
        },
        new()
        {
            Id = organizer2Id,
            Name = "Creative Minds",
            Email = "info@creativeminds.rs",
            Phone = "+381641234567",
            CompanyName = "Creative Minds Studio",
            TaxId = "107654321",
            Address = "Kralja Petra 22",
            City = "Belgrade",
            Country = "Serbia",
            IsVerified = true,
            CreatedAt = DateTime.UtcNow
        }
    };

    var venues = new List<Venue>
    {
        new()
        {
            Id = venue1Id,
            Name = "Master Hall",
            Address = "Kej zrtava racije 4",
            City = "Novi Sad",
            Country = "Serbia",
            Capacity = 350,
            HasParking = true,
            IndoorOutdoorType = "Indoor",
            ContactEmail = "hall@master.rs",
            ContactPhone = "+38121111222",
            Rating = 4.7
        },
        new()
        {
            Id = venue2Id,
            Name = "Riverside Garden",
            Address = "Ada Ciganlija 1",
            City = "Belgrade",
            Country = "Serbia",
            Capacity = 500,
            HasParking = true,
            IndoorOutdoorType = "Outdoor",
            ContactEmail = "booking@riverside.rs",
            ContactPhone = "+38111222333",
            Rating = 4.5
        }
    };

    var events = new List<Event>
    {
        new()
        {
            Id = event1Id,
            Title = "Tech Summit 2026",
            Description = "Conference focused on cloud, AI, and software testing.",
            Category = "Conference",
            StartDate = DateTime.UtcNow.AddDays(20),
            EndDate = DateTime.UtcNow.AddDays(22),
            Status = "Scheduled",
            MaxAttendees = 300,
            BasePrice = 149.99m,
            Currency = "EUR",
            OrganizerId = organizer1Id,
            VenueId = venue1Id,
            Tags = ["AI", "Cloud", "Testing"]
        },
        new()
        {
            Id = event2Id,
            Title = "Design & Product Day",
            Description = "Practical workshops on UX and product strategy.",
            Category = "Workshop",
            StartDate = DateTime.UtcNow.AddDays(35),
            EndDate = DateTime.UtcNow.AddDays(35),
            Status = "Scheduled",
            MaxAttendees = 180,
            BasePrice = 89.00m,
            Currency = "EUR",
            OrganizerId = organizer2Id,
            VenueId = venue2Id,
            Tags = ["UX", "Product", "Research"]
        }
    };

    var registrations = new List<Registration>
    {
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            EventId = event1Id,
            AttendeeFullName = "Milan Petrovic",
            AttendeeEmail = "milan.petrovic@example.com",
            TicketType = "Standard",
            PricePaid = 149.99m,
            PaymentStatus = "Paid",
            CheckInStatus = "NotCheckedIn",
            RegisteredAt = DateTime.UtcNow,
            Notes = "Vegetarian meal preference"
        },
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            EventId = event2Id,
            AttendeeFullName = "Jovana Markovic",
            AttendeeEmail = "jovana.markovic@example.com",
            TicketType = "VIP",
            PricePaid = 129.00m,
            PaymentStatus = "Paid",
            CheckInStatus = "NotCheckedIn",
            RegisteredAt = DateTime.UtcNow,
            Notes = "Needs front-row seat"
        }
    };

    await organizersCollection.InsertManyAsync(organizers);
    await venuesCollection.InsertManyAsync(venues);
    await eventsCollection.InsertManyAsync(events);
    await registrationsCollection.InsertManyAsync(registrations);

    return Results.Ok(new
    {
        message = "Seed completed.",
        organizers = organizers.Count,
        venues = venues.Count,
        events = events.Count,
        registrations = registrations.Count
    });
})
.WithName("SeedData");

app.Run();
