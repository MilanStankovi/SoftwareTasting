using backend.Controllers;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace BackendTests;

[TestFixture]
public class RegistrationTests
{
    private Mock<IRegistrationRepository> _mockRegistrationRepository;
    private RegistrationsController _controller;

    [SetUp]
    public void Setup()
    {
        _mockRegistrationRepository = new Mock<IRegistrationRepository>();
        _controller = new RegistrationsController(_mockRegistrationRepository.Object);
    }

    [Test]
    public async Task GetAll_ReturnsOkResult_WithListOfRegistrations()
    {
        // Arrange
        var registrations = new List<Registration>
        {
            new Registration { Id = "1", AttendeeFullName = "John Doe" },
            new Registration { Id = "2", AttendeeFullName = "Jane Doe" }
        };
        _mockRegistrationRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(registrations);

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(registrations));
    }

    [Test]
    public async Task GetAll_ReturnsOkResult_WithEmptyList()
    {
        // Arrange
        var registrations = new List<Registration>();
        _mockRegistrationRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(registrations);

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(registrations));
    }

    [Test]
    public async Task GetById_ReturnsOkResult_WhenRegistrationExists()
    {
        // Arrange
        var registration = new Registration { Id = "1", AttendeeFullName = "Test Attendee" };
        _mockRegistrationRepository.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync(registration);

        // Act
        var result = await _controller.GetById("1");

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(registration));
    }

    [Test]
    public async Task GetById_ReturnsNotFound_WhenRegistrationDoesNotExist()
    {
        // Arrange
        _mockRegistrationRepository.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync((Registration)null);

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
    public async Task Create_ReturnsCreatedAtActionResult_WithCreatedRegistration()
    {
        // Arrange
        var newRegistration = new Registration { AttendeeFullName = "New Attendee", AttendeeEmail = "email@example.com" };
        var createdRegistration = new Registration { Id = "1", AttendeeFullName = "New Attendee", AttendeeEmail = "email@example.com" };
        _mockRegistrationRepository.Setup(repo => repo.CreateAsync(newRegistration)).ReturnsAsync(createdRegistration);

        // Act
        var result = await _controller.Create(newRegistration);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.EqualTo(createdRegistration));
        Assert.That(createdResult.ActionName, Is.EqualTo(nameof(_controller.GetById)));
    }

    [Test]
    public async Task Create_ReturnsBadRequest_WhenRegistrationIsNull()
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
        var updatedRegistration = new Registration { Id = "1", AttendeeFullName = "Updated Attendee" };
        _mockRegistrationRepository.Setup(repo => repo.UpdateAsync("1", updatedRegistration)).ReturnsAsync(true);

        // Act
        var result = await _controller.Update("1", updatedRegistration);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Update_ReturnsNotFound_WhenRegistrationDoesNotExist()
    {
        // Arrange
        var updatedRegistration = new Registration { Id = "1", AttendeeFullName = "Updated Attendee" };
        _mockRegistrationRepository.Setup(repo => repo.UpdateAsync("1", updatedRegistration)).ReturnsAsync(false);

        // Act
        var result = await _controller.Update("1", updatedRegistration);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Update_ReturnsBadRequest_WhenIdIsNullOrRegistrationIsNullOrIdMismatch()
    {
        // Act
        var result = await _controller.Update(null, new Registration { Id = "1" });

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Delete_ReturnsNoContent_WhenDeleteSucceeds()
    {
        // Arrange
        _mockRegistrationRepository.Setup(repo => repo.DeleteAsync("1")).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete("1");

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Delete_ReturnsNotFound_WhenRegistrationDoesNotExist()
    {
        // Arrange
        _mockRegistrationRepository.Setup(repo => repo.DeleteAsync("1")).ReturnsAsync(false);

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

    [Test]
    public async Task GetAll_ReturnsCorrectCount()
    {
        // Arrange
        var registrations = new List<Registration>
        {
            new Registration { Id = "1", AttendeeFullName = "A1" },
            new Registration { Id = "2", AttendeeFullName = "A2" },
            new Registration { Id = "3", AttendeeFullName = "A3" }
        };
        _mockRegistrationRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(registrations);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var value = okResult.Value as List<Registration>;
        Assert.That(value, Is.Not.Null);
        Assert.That(value.Count, Is.EqualTo(3));
    }

    [Test]
    public async Task Create_CallsRepository_AndReturnsCreated()
    {
        // Arrange
        var newRegistration = new Registration { AttendeeFullName = "Caller Attendee", AttendeeEmail = "a@example.com" };
        var createdRegistration = new Registration { Id = "40", AttendeeFullName = "Caller Attendee", AttendeeEmail = "a@example.com" };
        _mockRegistrationRepository.Setup(repo => repo.CreateAsync(It.IsAny<Registration>())).ReturnsAsync(createdRegistration);

        // Act
        var result = await _controller.Create(newRegistration);

        // Assert
        _mockRegistrationRepository.Verify(repo => repo.CreateAsync(It.Is<Registration>(r => r.AttendeeFullName == newRegistration.AttendeeFullName && r.AttendeeEmail == newRegistration.AttendeeEmail)), Times.Once);
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.EqualTo(createdRegistration));
    }
}
