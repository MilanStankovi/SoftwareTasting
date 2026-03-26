using backend.Controllers;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace BackendTests;

[TestFixture]
public class EventTests
{
    private Mock<IEventRepository> _mockEventRepository;
    private EventsController _controller;

    [SetUp]
    public void Setup()
    {
        _mockEventRepository = new Mock<IEventRepository>();
        _controller = new EventsController(_mockEventRepository.Object);
    }

    [Test]
    public async Task GetAll_ReturnsOkResult_WithListOfEvents()
    {
        // Arrange
        var events = new List<Event>
        {
            new Event { Id = "1", Title = "Event 1" },
            new Event { Id = "2", Title = "Event 2" }
        };
        _mockEventRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(events);

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(events));
    }

    [Test]
    public async Task GetAll_ReturnsOkResult_WithEmptyList()
    {
        // Arrange
        var events = new List<Event>();
        _mockEventRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(events);

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(events));
    }

    [Test]
    public async Task GetById_ReturnsOkResult_WhenEventExists()
    {
        // Arrange
        var eventItem = new Event { Id = "1", Title = "Test Event" };
        _mockEventRepository.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync(eventItem);

        // Act
        var result = await _controller.GetById("1");

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(eventItem));
    }

    [Test]
    public async Task GetById_ReturnsNotFound_WhenEventDoesNotExist()
    {
        // Arrange
        _mockEventRepository.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync((Event)null);

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
    public async Task Create_ReturnsCreatedAtActionResult_WithCreatedEvent()
    {
        // Arrange
        var newEvent = new Event { Title = "New Event", Description = "Description" };
        var createdEvent = new Event { Id = "1", Title = "New Event", Description = "Description" };
        _mockEventRepository.Setup(repo => repo.CreateAsync(newEvent)).ReturnsAsync(createdEvent);

        // Act
        var result = await _controller.Create(newEvent);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.EqualTo(createdEvent));
        Assert.That(createdResult.ActionName, Is.EqualTo(nameof(_controller.GetById)));
    }

    [Test]
    public async Task Create_ReturnsBadRequest_WhenEventIsNull()
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
        var updatedEvent = new Event { Id = "1", Title = "Updated Event" };
        _mockEventRepository.Setup(repo => repo.UpdateAsync("1", updatedEvent)).ReturnsAsync(true);

        // Act
        var result = await _controller.Update("1", updatedEvent);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Update_ReturnsNotFound_WhenEventDoesNotExist()
    {
        // Arrange
        var updatedEvent = new Event { Id = "1", Title = "Updated Event" };
        _mockEventRepository.Setup(repo => repo.UpdateAsync("1", updatedEvent)).ReturnsAsync(false);

        // Act
        var result = await _controller.Update("1", updatedEvent);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Update_ReturnsBadRequest_WhenIdIsNullOrEventIsNullOrIdMismatch()
    {
        // Act
        var result = await _controller.Update(null, new Event { Id = "1" });

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Delete_ReturnsNoContent_WhenDeleteSucceeds()
    {
        // Arrange
        _mockEventRepository.Setup(repo => repo.DeleteAsync("1")).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete("1");

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Delete_ReturnsNotFound_WhenEventDoesNotExist()
    {
        // Arrange
        _mockEventRepository.Setup(repo => repo.DeleteAsync("1")).ReturnsAsync(false);

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
