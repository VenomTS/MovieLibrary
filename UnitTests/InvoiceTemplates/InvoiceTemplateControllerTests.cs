using API.InvoiceTemplates;
using API.OneOfTypes;
using DTO.InvoiceTemplates;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Moq;
using OneOf;
using OneOf.Types;
using Repositories;

namespace UnitTests.InvoiceTemplates;

public class InvoiceTemplatesControllerTests
{
    private readonly Mock<InvoiceTemplateService> service;
    private readonly InvoiceTemplatesController controller;

    public InvoiceTemplatesControllerTests()
    {
        var repositoryManager = new Mock<IRepositoryManager>();
        var userStore = new Mock<IUserStore<AppUser>>();

        var userManager = new Mock<UserManager<AppUser>>(
            userStore.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        var scheduleService =
            new API.Schedules.ScheduleService(
                repositoryManager.Object);

        service = new Mock<InvoiceTemplateService>(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        controller = new InvoiceTemplatesController(
            service.Object);
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenServiceReturnsInvoiceTemplate()
    {
        // Arrange
        var request = new CreateInvoiceTemplateRequest
        {
            UserId = Guid.NewGuid(),
            Price = 100m,
            Description = "Test invoice"
        };

        var response = new InvoiceTemplateResponse
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            ScheduleId = Guid.NewGuid(),
            Price = request.Price,
            Description = request.Description
        };

        service
            .Setup(x => x.CreateAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await controller.Create(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Same(response, okResult.Value);

        service.Verify(
            x => x.CreateAsync(request),
            Times.Once);
    }

    [Fact]
    public async Task Create_ShouldReturnNotFound_WhenServiceReturnsNotFound()
    {
        // Arrange
        var request = new CreateInvoiceTemplateRequest
        {
            UserId = Guid.NewGuid(),
            Price = 100m,
            Description = "Test invoice"
        };

        service
            .Setup(x => x.CreateAsync(request))
            .ReturnsAsync(new NotFound());

        // Act
        var result = await controller.Create(request);

        // Assert
        Assert.IsType<NotFoundResult>(result);

        service.Verify(
            x => x.CreateAsync(request),
            Times.Once);
    }

    [Fact]
    public async Task Create_ShouldReturnConflict_WhenInvoiceTemplateAlreadyExists()
    {
        // Arrange
        var request = new CreateInvoiceTemplateRequest
        {
            UserId = Guid.NewGuid(),
            Price = 100m,
            Description = "Test invoice"
        };

        service
            .Setup(x => x.CreateAsync(request))
            .ReturnsAsync(new InvoiceTemplateAlreadyExists());

        // Act
        var result = await controller.Create(request);

        // Assert
        Assert.IsType<ConflictResult>(result);

        service.Verify(
            x => x.CreateAsync(request),
            Times.Once);
    }

    [Fact]
    public async Task Put_ShouldReturnOk_WhenServiceReturnsInvoiceTemplate()
    {
        // Arrange
        var id = Guid.NewGuid();

        var request = new UpdateInvoiceTemplateRequest
        {
            Price = 200m,
            Description = "Updated invoice"
        };

        var response = new InvoiceTemplateResponse
        {
            Id = id,
            UserId = Guid.NewGuid(),
            ScheduleId = Guid.NewGuid(),
            Price = request.Price,
            Description = request.Description
        };

        service
            .Setup(x => x.PutAsync(id, request))
            .ReturnsAsync(response);

        // Act
        var result = await controller.Put(id, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Same(response, okResult.Value);

        service.Verify(
            x => x.PutAsync(id, request),
            Times.Once);
    }

    [Fact]
    public async Task Put_ShouldReturnNotFound_WhenServiceReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        var request = new UpdateInvoiceTemplateRequest
        {
            Price = 200m,
            Description = "Updated invoice"
        };

        service
            .Setup(x => x.PutAsync(id, request))
            .ReturnsAsync(new NotFound());

        // Act
        var result = await controller.Put(id, request);

        // Assert
        Assert.IsType<NotFoundResult>(result);

        service.Verify(
            x => x.PutAsync(id, request),
            Times.Once);
    }

    [Fact]
    public async Task GetByUserId_ShouldReturnOk_WhenServiceReturnsInvoiceTemplate()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var response = new InvoiceTemplateDetailedResponse
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Price = 150m,
            Description = "Monthly invoice"
        };

        service
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(response);

        // Act
        var result = await controller.GetByUserId(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Same(response, okResult.Value);

        service.Verify(
            x => x.GetByUserIdAsync(userId),
            Times.Once);
    }

    [Fact]
    public async Task GetByUserId_ShouldReturnNotFound_WhenServiceReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        service
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new NotFound());

        // Act
        var result = await controller.GetByUserId(userId);

        // Assert
        Assert.IsType<NotFoundResult>(result);

        service.Verify(
            x => x.GetByUserIdAsync(userId),
            Times.Once);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithInvoiceTemplates()
    {
        // Arrange
        var response = new List<InvoiceTemplateResponse>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ScheduleId = Guid.NewGuid(),
                Price = 100m,
                Description = "Invoice 1"
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ScheduleId = Guid.NewGuid(),
                Price = 200m,
                Description = "Invoice 2"
            }
        };

        service
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(response);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Same(response, okResult.Value);

        service.Verify(
            x => x.GetAllAsync(),
            Times.Once);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithEmptyList_WhenNoInvoiceTemplatesExist()
    {
        // Arrange
        var response = new List<InvoiceTemplateResponse>();

        service
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(response);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Same(response, okResult.Value);
        Assert.Empty((List<InvoiceTemplateResponse>)okResult.Value!);

        service.Verify(
            x => x.GetAllAsync(),
            Times.Once);
    }
}