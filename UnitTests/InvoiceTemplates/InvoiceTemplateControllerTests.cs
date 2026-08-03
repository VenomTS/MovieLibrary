using API.InvoiceTemplates;
using API.Schedules;
using DTO.InvoiceTemplates;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Invoices;
using Models.Schedules;
using Models.Schedules.Rules;
using Moq;
using Repositories;
using Repositories.Database;

namespace UnitTests.InvoiceTemplates;

public class InvoiceTemplatesControllerTests(DatabaseFixture fixture)
    : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task Create_ShouldReturnOk_WhenUserExistsAndTemplateDoesNotExist()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var user = await Shared.CreateUser(dbContext);

        var repositoryManager = CreateRepositoryManager(dbContext);
        var userManager = CreateUserManager();

        userManager
            .Setup(x => x.FindByIdAsync(user.Id.ToString()))
            .ReturnsAsync(user);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.GetByUserIdAsync(user.Id))
            .ReturnsAsync((InvoiceTemplate?)null);

        repositoryManager
            .Setup(x => x.Schedules.AsQueryable())
            .Returns(dbContext.Schedules);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.CreateAsync(It.IsAny<InvoiceTemplate>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var scheduleService =
            new ScheduleService(repositoryManager.Object);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        var controller = new InvoiceTemplatesController(service);

        var request = CreateCreateRequest(user.Id);

        // Act
        var result = await controller.Create(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var response =
            Assert.IsType<InvoiceTemplateResponse>(okResult.Value);

        Assert.Equal(user.Id, response.UserId);
        Assert.Equal(request.Price, response.Price);
        Assert.Equal(request.Description, response.Description);
    }

    [Fact]
    public async Task Create_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var repositoryManager = CreateRepositoryManager(dbContext);
        var userManager = CreateUserManager();

        var userId = Guid.NewGuid();

        userManager
            .Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync((AppUser?)null);

        var scheduleService =
            new ScheduleService(repositoryManager.Object);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        var controller = new InvoiceTemplatesController(service);

        var request = CreateCreateRequest(userId);

        // Act
        var result = await controller.Create(request);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ShouldReturnConflict_WhenTemplateAlreadyExists()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var user = await Shared.CreateUser(dbContext);

        var schedule = new Schedule
        {
            Id = Guid.NewGuid(),
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 12, 31),
            NextOccurrence = new DateOnly(2026, 2, 1),
            RecurrenceRule = new RecurrenceRule
            {
                Frequency = Frequency.Monthly,
                Interval = 1,
                DayOfMonth = 1
            }
        };

        dbContext.Schedules.Add(schedule);
        await dbContext.SaveChangesAsync();

        var existingTemplate = new InvoiceTemplate
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            ScheduleId = schedule.Id,
            Price = 100m,
            Description = "Existing invoice"
        };

        dbContext.InvoiceTemplates.Add(existingTemplate);
        await dbContext.SaveChangesAsync();

        var repositoryManager = CreateRepositoryManager(dbContext);
        var userManager = CreateUserManager();

        userManager
            .Setup(x => x.FindByIdAsync(user.Id.ToString()))
            .ReturnsAsync(user);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.GetByUserIdAsync(user.Id))
            .ReturnsAsync(existingTemplate);

        var scheduleService =
            new ScheduleService(repositoryManager.Object);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        var controller = new InvoiceTemplatesController(service);

        var request = CreateCreateRequest(user.Id);

        // Act
        var result = await controller.Create(request);

        // Assert
        Assert.IsType<ConflictResult>(result);

    }

    [Fact]
    public async Task Put_ShouldReturnOk_WhenInvoiceTemplateExists()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var user = await Shared.CreateUser(dbContext);

        var schedule = CreateSchedule();

        var invoiceTemplate = CreateInvoiceTemplate(
            user.Id,
            schedule);

        dbContext.Schedules.Add(schedule);
        dbContext.InvoiceTemplates.Add(invoiceTemplate);
        await dbContext.SaveChangesAsync();

        var newSchedule = CreateSchedule();

        var repositoryManager = CreateRepositoryManager(dbContext);
        var userManager = CreateUserManager();

        repositoryManager
            .Setup(x => x.InvoiceTemplates.GetByIdAsync(invoiceTemplate.Id))
            .ReturnsAsync(invoiceTemplate);

        repositoryManager
            .Setup(x => x.Schedules.AsQueryable())
            .Returns(dbContext.Schedules);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.Update(invoiceTemplate))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Force FindOrCreateSchedule to use the new schedule.
        repositoryManager
            .Setup(x => x.Schedules.AsQueryable())
            .Returns(dbContext.Schedules);

        dbContext.Schedules.Add(newSchedule);
        await dbContext.SaveChangesAsync();

        var scheduleService =
            new ScheduleService(repositoryManager.Object);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        var controller = new InvoiceTemplatesController(service);

        var request = CreateUpdateRequest(
            price: 300m,
            description: "Updated invoice",
            startDate: newSchedule.StartDate,
            endDate: newSchedule.EndDate);

        // Act
        var result =
            await controller.Put(invoiceTemplate.Id, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var response =
            Assert.IsType<InvoiceTemplateResponse>(okResult.Value);

        Assert.Equal(invoiceTemplate.Id, response.Id);
        Assert.Equal(user.Id, response.UserId);
        Assert.Equal(300m, response.Price);
        Assert.Equal("Updated invoice", response.Description);
    }

    [Fact]
    public async Task Put_ShouldReturnNotFound_WhenInvoiceTemplateDoesNotExist()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var repositoryManager = CreateRepositoryManager(dbContext);
        var userManager = CreateUserManager();

        repositoryManager
            .Setup(x => x.InvoiceTemplates.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((InvoiceTemplate?)null);

        var scheduleService =
            new ScheduleService(repositoryManager.Object);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        var controller = new InvoiceTemplatesController(service);

        var request = CreateUpdateRequest();

        // Act
        var result =
            await controller.Put(Guid.NewGuid(), request);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetByUserId_ShouldReturnOk_WhenInvoiceTemplateExists()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var user = await Shared.CreateUser(dbContext);

        var schedule = CreateSchedule();

        var invoiceTemplate = CreateInvoiceTemplate(
            user.Id,
            schedule);

        dbContext.Schedules.Add(schedule);
        dbContext.InvoiceTemplates.Add(invoiceTemplate);
        await dbContext.SaveChangesAsync();

        var repositoryManager = CreateRepositoryManager(dbContext);
        var userManager = CreateUserManager();

        repositoryManager
            .Setup(x => x.InvoiceTemplates.GetByUserIdAsync(user.Id))
            .ReturnsAsync(invoiceTemplate);

        var scheduleService =
            new ScheduleService(repositoryManager.Object);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        var controller = new InvoiceTemplatesController(service);

        // Act
        var result =
            await controller.GetByUserId(user.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var response =
            Assert.IsType<InvoiceTemplateDetailedResponse>(
                okResult.Value);

        Assert.Equal(invoiceTemplate.Id, response.Id);
        Assert.Equal(user.Id, response.UserId);
        Assert.Equal(invoiceTemplate.Price, response.Price);
        Assert.Equal(invoiceTemplate.Description, response.Description);

        Assert.Equal(
            schedule.Id,
            response.Schedule.Id);
    }

    [Fact]
    public async Task GetByUserId_ShouldReturnNotFound_WhenInvoiceTemplateDoesNotExist()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var repositoryManager = CreateRepositoryManager(dbContext);
        var userManager = CreateUserManager();

        repositoryManager
            .Setup(x => x.InvoiceTemplates.GetByUserIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((InvoiceTemplate?)null);

        var scheduleService =
            new ScheduleService(repositoryManager.Object);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        var controller = new InvoiceTemplatesController(service);

        // Act
        var result =
            await controller.GetByUserId(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithInvoiceTemplates()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var invoices = new List<InvoiceTemplate>
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

        var repositoryManager = CreateRepositoryManager(dbContext);
        var userManager = CreateUserManager();

        repositoryManager
            .Setup(x => x.InvoiceTemplates.GetAllAsync())
            .ReturnsAsync(invoices);

        var scheduleService =
            new ScheduleService(repositoryManager.Object);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        var controller = new InvoiceTemplatesController(service);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var response =
            Assert.IsType<List<InvoiceTemplateResponse>>(
                okResult.Value);

        Assert.Equal(2, response.Count);

        Assert.Equal(
            invoices[0].Id,
            response[0].Id);

        Assert.Equal(
            invoices[1].Id,
            response[1].Id);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithEmptyList_WhenNoInvoiceTemplatesExist()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var repositoryManager = CreateRepositoryManager(dbContext);
        var userManager = CreateUserManager();

        repositoryManager
            .Setup(x => x.InvoiceTemplates.GetAllAsync())
            .ReturnsAsync(new List<InvoiceTemplate>());

        var scheduleService =
            new ScheduleService(repositoryManager.Object);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        var controller = new InvoiceTemplatesController(service);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var response =
            Assert.IsType<List<InvoiceTemplateResponse>>(
                okResult.Value);

        Assert.Empty(response);
    }

    private static Mock<IRepositoryManager> CreateRepositoryManager(
        AppDbContext dbContext)
    {
        return new Mock<IRepositoryManager>();
    }

    private static Mock<UserManager<AppUser>> CreateUserManager()
    {
        var userStore = new Mock<IUserStore<AppUser>>();

        return new Mock<UserManager<AppUser>>(
            userStore.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);
    }

    private static Schedule CreateSchedule()
    {
        return new Schedule
        {
            Id = Guid.NewGuid(),
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 12, 31),
            NextOccurrence = new DateOnly(2026, 2, 1),
            RecurrenceRule = new RecurrenceRule
            {
                Frequency = Frequency.Monthly,
                Interval = 1,
                DayOfMonth = 1
            }
        };
    }

    private static InvoiceTemplate CreateInvoiceTemplate(
        Guid userId,
        Schedule schedule)
    {
        return new InvoiceTemplate
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ScheduleId = schedule.Id,
            Price = 100m,
            Description = "Test invoice",
            Schedule = schedule
        };
    }

    private static CreateInvoiceTemplateRequest CreateCreateRequest(
        Guid userId)
    {
        return new CreateInvoiceTemplateRequest
        {
            UserId = userId,
            Price = 100m,
            Description = "Test invoice",
            Schedule = new CreateInvoiceTemplateScheduleRequest
            {
                StartDate = new DateOnly(2026, 1, 1),
                EndDate = new DateOnly(2026, 12, 31),
                Frequency = Frequency.Monthly,
                Interval = 1,
                DayOfMonth = 1
            }
        };
    }

    private static UpdateInvoiceTemplateRequest CreateUpdateRequest(
        decimal price = 200m,
        string description = "Updated invoice",
        DateOnly? startDate = null,
        DateOnly? endDate = null)
    {
        return new UpdateInvoiceTemplateRequest
        {
            Price = price,
            Description = description,
            Schedule = new CreateInvoiceTemplateScheduleRequest
            {
                StartDate = startDate ?? new DateOnly(2026, 1, 1),
                EndDate = endDate ?? new DateOnly(2026, 12, 31),
                Frequency = Frequency.Monthly,
                Interval = 1,
                DayOfMonth = 1
            }
        };
    }

    private static async Task ClearDatabase(
        AppDbContext dbContext)
    {
        await dbContext.Database.ExecuteSqlRawAsync("""
                                                    TRUNCATE TABLE
                                                        "InvoiceDeliveries",
                                                        "Invoices",
                                                        "InvoiceTemplates",
                                                        "Schedules"
                                                    CASCADE;
                                                    """);
    }

}