using backend.Controllers;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace BackendTests;

[TestFixture]
public class VenueTests
{
    private Mock<IVenueRepository> _mockVenueRepository;
    private VenuesController _controller;

    [SetUp]
    public void Setup()
    {
        _mockVenueRepository = new Mock<IVenueRepository>();
        _controller = new VenuesController(_mockVenueRepository.Object);
    }

    [Test]
    public async Task GetAll_ReturnsOkResult_WithListOfVenues()
    {
        // Arrange
        var venues = new List<Venue>
        {
            new Venue { Id = "1", Name = "Venue 1" },
            new Venue { Id = "2", Name = "Venue 2" }
        };
        _mockVenueRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(venues);

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(venues));
    }

    [Test]
    public async Task GetAll_ReturnsOkResult_WithEmptyList()
    {
        // Arrange
        var venues = new List<Venue>();
        _mockVenueRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(venues);

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(venues));
    }

    [Test]
    public async Task GetById_ReturnsOkResult_WhenVenueExists()
    {
        // Arrange
        var venue = new Venue { Id = "1", Name = "Test Venue" };
        _mockVenueRepository.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync(venue);

        // Act
        var result = await _controller.GetById("1");

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(venue));
    }

    [Test]
    public async Task GetById_ReturnsNotFound_WhenVenueDoesNotExist()
    {
        // Arrange
        _mockVenueRepository.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync((Venue)null);

        // Act
        var result = await _controller.GetById("1");

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task GetById_ReturnsBadRequest_WhenIdIsNullOrEmpty()
    {
        // Act
        var result = await _controller.GetById(null);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Create_ReturnsCreatedAtActionResult_WithCreatedVenue()
    {
        // Arrange
        var newVenue = new Venue { Name = "New Venue", Address = "123 Main St" };
        var createdVenue = new Venue { Id = "1", Name = "New Venue", Address = "123 Main St" };
        _mockVenueRepository.Setup(repo => repo.CreateAsync(newVenue)).ReturnsAsync(createdVenue);

        // Act
        var result = await _controller.Create(newVenue);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.EqualTo(createdVenue));
        Assert.That(createdResult.ActionName, Is.EqualTo(nameof(_controller.GetById)));
    }

    [Test]
    public async Task Create_ReturnsBadRequest_WhenVenueIsNull()
    {
        // Act
        var result = await _controller.Create(null);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Update_ReturnsNoContent_WhenUpdateSucceeds()
    {
        // Arrange
        var updatedVenue = new Venue { Id = "1", Name = "Updated Venue" };
        _mockVenueRepository.Setup(repo => repo.UpdateAsync("1", updatedVenue)).ReturnsAsync(true);

        // Act
        var result = await _controller.Update("1", updatedVenue);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Update_ReturnsNotFound_WhenVenueDoesNotExist()
    {
        // Arrange
        var updatedVenue = new Venue { Id = "1", Name = "Updated Venue" };
        _mockVenueRepository.Setup(repo => repo.UpdateAsync("1", updatedVenue)).ReturnsAsync(false);

        // Act
        var result = await _controller.Update("1", updatedVenue);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Update_ReturnsBadRequest_WhenIdIsNullOrVenueIsNullOrIdMismatch()
    {
        // Act
        var result = await _controller.Update(null, new Venue { Id = "1" });

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Delete_ReturnsNoContent_WhenDeleteSucceeds()
    {
        // Arrange
        _mockVenueRepository.Setup(repo => repo.DeleteAsync("1")).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete("1");

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Delete_ReturnsNotFound_WhenVenueDoesNotExist()
    {
        // Arrange
        _mockVenueRepository.Setup(repo => repo.DeleteAsync("1")).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete("1");

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Delete_ReturnsBadRequest_WhenIdIsNullOrEmpty()
    {
        // Act
        var result = await _controller.Delete(null);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
}
