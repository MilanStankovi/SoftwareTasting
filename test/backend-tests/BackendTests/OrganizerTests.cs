using backend.Controllers;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace BackendTests;

[TestFixture]
public class OrganizerTests
{
    private Mock<IOrganizerRepository> _mockOrganizerRepository;
    private OrganizersController _controller;

    [SetUp]
    public void Setup()
    {
        _mockOrganizerRepository = new Mock<IOrganizerRepository>();
        _controller = new OrganizersController(_mockOrganizerRepository.Object);
    }

    [Test]
    public async Task GetAll_ReturnsOkResult_WithListOfOrganizers()
    {
        // Arrange
        var organizers = new List<Organizer>
        {
            new Organizer { Id = "1", Name = "Organizer 1" },
            new Organizer { Id = "2", Name = "Organizer 2" }
        };
        _mockOrganizerRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(organizers);

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(organizers));
    }

    [Test]
    public async Task GetAll_ReturnsOkResult_WithEmptyList()
    {
        // Arrange
        var organizers = new List<Organizer>();
        _mockOrganizerRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(organizers);

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(organizers));
    }

    [Test]
    public async Task GetById_ReturnsOkResult_WhenOrganizerExists()
    {
        // Arrange
        var organizer = new Organizer { Id = "1", Name = "Test Organizer" };
        _mockOrganizerRepository.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync(organizer);

        // Act
        var result = await _controller.GetById("1");

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(organizer));
    }

    [Test]
    public async Task GetById_ReturnsNotFound_WhenOrganizerDoesNotExist()
    {
        // Arrange
        _mockOrganizerRepository.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync((Organizer)null);

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
    public async Task Create_ReturnsCreatedAtActionResult_WithCreatedOrganizer()
    {
        // Arrange
        var newOrganizer = new Organizer { Name = "New Organizer", Email = "email@example.com" };
        var createdOrganizer = new Organizer { Id = "1", Name = "New Organizer", Email = "email@example.com" };
        _mockOrganizerRepository.Setup(repo => repo.CreateAsync(newOrganizer)).ReturnsAsync(createdOrganizer);

        // Act
        var result = await _controller.Create(newOrganizer);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.EqualTo(createdOrganizer));
        Assert.That(createdResult.ActionName, Is.EqualTo(nameof(_controller.GetById)));
    }

    [Test]
    public async Task Create_ReturnsBadRequest_WhenOrganizerIsNull()
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
        var updatedOrganizer = new Organizer { Id = "1", Name = "Updated Organizer" };
        _mockOrganizerRepository.Setup(repo => repo.UpdateAsync("1", updatedOrganizer)).ReturnsAsync(true);

        // Act
        var result = await _controller.Update("1", updatedOrganizer);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Update_ReturnsNotFound_WhenOrganizerDoesNotExist()
    {
        // Arrange
        var updatedOrganizer = new Organizer { Id = "1", Name = "Updated Organizer" };
        _mockOrganizerRepository.Setup(repo => repo.UpdateAsync("1", updatedOrganizer)).ReturnsAsync(false);

        // Act
        var result = await _controller.Update("1", updatedOrganizer);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Update_ReturnsBadRequest_WhenIdIsNullOrOrganizerIsNullOrIdMismatch()
    {
        // Act
        var result = await _controller.Update(null, new Organizer { Id = "1" });

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Delete_ReturnsNoContent_WhenDeleteSucceeds()
    {
        // Arrange
        _mockOrganizerRepository.Setup(repo => repo.DeleteAsync("1")).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete("1");

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Delete_ReturnsNotFound_WhenOrganizerDoesNotExist()
    {
        // Arrange
        _mockOrganizerRepository.Setup(repo => repo.DeleteAsync("1")).ReturnsAsync(false);

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
