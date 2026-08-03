using API.InvoiceTemplates;
using API.OneOfTypes;
using API.Schedules;
using DTO.InvoiceTemplates;
using Microsoft.AspNetCore.Identity;
using Models;
using Models.InvoiceDeliveries;
using Models.Invoices;
using Models.Schedules;
using Models.Schedules.Rules;
using Moq;
using OneOf.Types;
using Repositories;
using Repositories.Interfaces;

namespace UnitTests.InvoiceTemplates;

public class InvoiceTemplateServiceTests(DatabaseFixture fixture)
    : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    
    public async Task InitializeAsync()
    {
        await fixture.ClearDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await fixture.ClearDatabaseAsync();
    }
    
    [Fact]
    public async Task CreateAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        await using var dbContext = fixture.CreateDbContext();

        var repositoryManager = CreateRepositoryManager();
        var userManager = CreateUserManager();
        var scheduleService = CreateScheduleService(repositoryManager.Object);

        var request = CreateCreateRequest(Guid.NewGuid());

        userManager
            .Setup(x => x.FindByIdAsync(request.UserId.ToString()))
            .ReturnsAsync((AppUser?)null);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.True(result.IsT1);
        Assert.IsType<NotFound>(result.AsT1);

        repositoryManager.Verify(
            x => x.InvoiceTemplates.GetByUserIdAsync(
                It.IsAny<Guid>()),
            Times.Never);

        repositoryManager.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnAlreadyExists_WhenUserAlreadyHasInvoiceTemplate()
    {
        await using var dbContext = fixture.CreateDbContext();

        var user = await Shared.CreateUser(dbContext);

        var repositoryManager = CreateRepositoryManager();
        var userManager = CreateUserManager();
        var scheduleService = CreateScheduleService(repositoryManager.Object);

        var existingInvoice = new InvoiceTemplate
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Price = 100m,
            Description = "Existing invoice"
        };

        var request = CreateCreateRequest(user.Id);

        userManager
            .Setup(x => x.FindByIdAsync(user.Id.ToString()))
            .ReturnsAsync(user);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.GetByUserIdAsync(user.Id))
            .ReturnsAsync(existingInvoice);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.True(result.IsT2);
        Assert.IsType<InvoiceTemplateAlreadyExists>(result.AsT2);

        repositoryManager.Verify(
            x => x.InvoiceTemplates.CreateAsync(
                It.IsAny<InvoiceTemplate>()),
            Times.Never);

        repositoryManager.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateInvoiceTemplate_WhenUserExistsAndNoTemplateExists()
    {
        await using var dbContext = fixture.CreateDbContext();

        var user = await Shared.CreateUser(dbContext);

        var repositoryManager = CreateRepositoryManager();
        var userManager = CreateUserManager();

        var scheduleRepository = new Mock<IScheduleRepository>();

        repositoryManager
            .SetupGet(x => x.Schedules)
            .Returns(scheduleRepository.Object);

        var scheduleService =
            CreateScheduleService(repositoryManager.Object);

        var request = CreateCreateRequest(user.Id);

        userManager
            .Setup(x => x.FindByIdAsync(user.Id.ToString()))
            .ReturnsAsync(user);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.GetByUserIdAsync(user.Id))
            .ReturnsAsync((InvoiceTemplate?)null);

        scheduleRepository
            .Setup(x => x.AsQueryable())
            .Returns(dbContext.Schedules);

        Schedule? createdSchedule = null;

        scheduleRepository
            .Setup(x => x.CreateAsync(It.IsAny<Schedule>()))
            .Callback<Schedule>(x =>
            {
                x.Id = Guid.NewGuid();
                createdSchedule = x;
            })
            .Returns(Task.CompletedTask);

        InvoiceTemplate? createdInvoice = null;

        repositoryManager
            .Setup(x => x.InvoiceTemplates.CreateAsync(
                It.IsAny<InvoiceTemplate>()))
            .Callback<InvoiceTemplate>(x =>
            {
                x.Id = Guid.NewGuid();
                createdInvoice = x;
            })
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.True(result.IsT0);

        var response = result.AsT0;

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(user.Id, response.UserId);
        Assert.Equal(request.Price, response.Price);
        Assert.Equal(request.Description, response.Description);

        Assert.NotNull(createdSchedule);
        Assert.Equal(
            createdSchedule.Id,
            response.ScheduleId);

        Assert.NotNull(createdInvoice);
        Assert.Equal(user.Id, createdInvoice.UserId);
        Assert.Equal(
            createdSchedule.Id,
            createdInvoice.ScheduleId);
        Assert.Equal(
            request.Price,
            createdInvoice.Price);
        Assert.Equal(
            request.Description,
            createdInvoice.Description);

        Assert.NotEqual(
            default,
            createdSchedule.NextOccurrence);

        scheduleRepository.Verify(
            x => x.CreateAsync(
                It.IsAny<Schedule>()),
            Times.Once);

        repositoryManager.Verify(
            x => x.InvoiceTemplates.CreateAsync(
                It.IsAny<InvoiceTemplate>()),
            Times.Once);

        repositoryManager.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldReuseExistingSchedule()
    {
        await using var dbContext = fixture.CreateDbContext();

        var user = await Shared.CreateUser(dbContext);

        var existingSchedule = CreateSchedule();

        dbContext.Schedules.Add(existingSchedule);
        await dbContext.SaveChangesAsync();

        var repositoryManager = CreateRepositoryManager();
        var userManager = CreateUserManager();

        var scheduleRepository = new Mock<IScheduleRepository>();

        repositoryManager
            .SetupGet(x => x.Schedules)
            .Returns(scheduleRepository.Object);

        scheduleRepository
            .Setup(x => x.AsQueryable())
            .Returns(dbContext.Schedules);

        var scheduleService =
            CreateScheduleService(repositoryManager.Object);

        var request = CreateCreateRequest(
            user.Id,
            existingSchedule.StartDate,
            existingSchedule.EndDate);

        userManager
            .Setup(x => x.FindByIdAsync(user.Id.ToString()))
            .ReturnsAsync(user);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.GetByUserIdAsync(user.Id))
            .ReturnsAsync((InvoiceTemplate?)null);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.CreateAsync(
                It.IsAny<InvoiceTemplate>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.True(result.IsT0);
        Assert.Equal(
            existingSchedule.Id,
            result.AsT0.ScheduleId);

        scheduleRepository.Verify(
            x => x.CreateAsync(
                It.IsAny<Schedule>()),
            Times.Never);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnNotFound_WhenInvoiceTemplateDoesNotExist()
    {
        await using var dbContext = fixture.CreateDbContext();

        var repositoryManager = CreateRepositoryManager();
        var userManager = CreateUserManager();
        var scheduleService =
            CreateScheduleService(repositoryManager.Object);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.GetByUserIdAsync(
                It.IsAny<Guid>()))
            .ReturnsAsync((InvoiceTemplate?)null);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        // Act
        var result =
            await service.GetByUserIdAsync(Guid.NewGuid());

        // Assert
        Assert.True(result.IsT1);
        Assert.IsType<NotFound>(result.AsT1);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnDetailedInvoiceTemplate()
    {
        await using var dbContext = fixture.CreateDbContext();

        var userId = Guid.NewGuid();
        var schedule = CreateSchedule();

        var invoiceTemplate = new InvoiceTemplate
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ScheduleId = schedule.Id,
            Price = 125.50m,
            Description = "Monthly invoice",
            Schedule = schedule
        };

        var repositoryManager = CreateRepositoryManager();
        var userManager = CreateUserManager();
        var scheduleService =
            CreateScheduleService(repositoryManager.Object);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.GetByUserIdAsync(userId))
            .ReturnsAsync(invoiceTemplate);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        // Act
        var result =
            await service.GetByUserIdAsync(userId);

        // Assert
        Assert.True(result.IsT0);

        var response = result.AsT0;

        Assert.NotNull(response);
        Assert.Equal(invoiceTemplate.Id, response.Id);
        Assert.Equal(userId, response.UserId);
        Assert.Equal(125.50m, response.Price);
        Assert.Equal(
            "Monthly invoice",
            response.Description);

        Assert.NotNull(response.Schedule);

        Assert.Equal(
            schedule.Id,
            response.Schedule.Id);

        Assert.Equal(
            schedule.StartDate,
            response.Schedule.StartDate);

        Assert.Equal(
            schedule.EndDate,
            response.Schedule.EndDate);

        Assert.Equal(
            schedule.NextOccurrence,
            response.Schedule.NextOccurrence);

        Assert.Equal(
            schedule.RecurrenceRule.Frequency,
            response.Schedule.Frequency);

        Assert.Equal(
            schedule.RecurrenceRule.Interval,
            response.Schedule.Interval);

        Assert.Equal(
            schedule.RecurrenceRule.DaysOfWeek,
            response.Schedule.DaysOfWeek);

        Assert.Equal(
            schedule.RecurrenceRule.DayOfMonth,
            response.Schedule.DayOfMonth);

        Assert.Equal(
            schedule.RecurrenceRule.Ordinal,
            response.Schedule.Ordinal);

        Assert.Equal(
            schedule.RecurrenceRule.OrdinalType,
            response.Schedule.OrdinalType);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedInvoiceTemplates()
    {
        await using var dbContext = fixture.CreateDbContext();

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

        var repositoryManager = CreateRepositoryManager();
        var userManager = CreateUserManager();
        var scheduleService =
            CreateScheduleService(repositoryManager.Object);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.GetAllAsync())
            .ReturnsAsync(invoices);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);

        for (var i = 0; i < invoices.Count; i++)
        {
            Assert.Equal(
                invoices[i].Id,
                result[i].Id);

            Assert.Equal(
                invoices[i].UserId,
                result[i].UserId);

            Assert.Equal(
                invoices[i].ScheduleId,
                result[i].ScheduleId);

            Assert.Equal(
                invoices[i].Price,
                result[i].Price);

            Assert.Equal(
                invoices[i].Description,
                result[i].Description);
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoInvoicesExist()
    {
        await using var dbContext = fixture.CreateDbContext();

        var repositoryManager = CreateRepositoryManager();
        var userManager = CreateUserManager();
        var scheduleService =
            CreateScheduleService(repositoryManager.Object);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.GetAllAsync())
            .ReturnsAsync(new List<InvoiceTemplate>());

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetScheduledInvoicesAsync_ShouldReturnInvoicesDueOnDate()
    {
        await using var dbContext = fixture.CreateDbContext();

        var date = new DateOnly(2026, 2, 1);

        var user = await Shared.CreateUser(dbContext);

        var schedule = CreateSchedule();
        schedule.NextOccurrence = date;

        var invoice = CreateInvoiceTemplate(
            user.Id,
            schedule);

        dbContext.InvoiceTemplates.Add(invoice);
        await dbContext.SaveChangesAsync();

        var repositoryManager = CreateRepositoryManager();
        var userManager = CreateUserManager();
        var scheduleService =
            CreateScheduleService(repositoryManager.Object);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.AsQueryable())
            .Returns(dbContext.InvoiceTemplates);

        repositoryManager
            .Setup(x => x.InvoiceDeliveries.AsQueryable())
            .Returns(dbContext.InvoiceDeliveries);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        // Act
        var result =
            await service.GetScheduledInvoicesAsync(date);

        // Assert
        Assert.Contains(
            result,
            x => x.Id == invoice.Id);
    }

    [Fact]
    public async Task GetScheduledInvoicesAsync_ShouldReturnInvoice_WhenNextOccurrenceEqualsDate()
    {
        await using var dbContext = fixture.CreateDbContext();

        var date = new DateOnly(2026, 2, 1);

        var user = await Shared.CreateUser(dbContext);

        var schedule = CreateSchedule();
        schedule.NextOccurrence = date;

        var invoice = CreateInvoiceTemplate(
            user.Id,
            schedule);

        dbContext.InvoiceTemplates.Add(invoice);
        await dbContext.SaveChangesAsync();

        var repositoryManager = CreateRepositoryManager();
        var userManager = CreateUserManager();
        var scheduleService =
            CreateScheduleService(repositoryManager.Object);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.AsQueryable())
            .Returns(dbContext.InvoiceTemplates);

        repositoryManager
            .Setup(x => x.InvoiceDeliveries.AsQueryable())
            .Returns(dbContext.InvoiceDeliveries);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        // Act
        var result =
            await service.GetScheduledInvoicesAsync(date);

        // Assert
        Assert.Contains(
            result,
            x => x.Id == invoice.Id);
    }

    [Fact]
    public async Task GetScheduledInvoicesAsync_ShouldReturnInvoice_WhenNextOccurrenceIsBeforeDate()
    {
        await using var dbContext = fixture.CreateDbContext();

        var date = new DateOnly(2026, 2, 10);

        var user = await Shared.CreateUser(dbContext);

        var schedule = CreateSchedule();
        schedule.NextOccurrence = date.AddDays(-1);

        var invoice = CreateInvoiceTemplate(
            user.Id,
            schedule);

        dbContext.InvoiceTemplates.Add(invoice);
        await dbContext.SaveChangesAsync();

        var repositoryManager = CreateRepositoryManager();
        var userManager = CreateUserManager();
        var scheduleService =
            CreateScheduleService(repositoryManager.Object);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.AsQueryable())
            .Returns(dbContext.InvoiceTemplates);

        repositoryManager
            .Setup(x => x.InvoiceDeliveries.AsQueryable())
            .Returns(dbContext.InvoiceDeliveries);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        // Act
        var result =
            await service.GetScheduledInvoicesAsync(date);

        // Assert
        Assert.Contains(
            result,
            x => x.Id == invoice.Id);
    }

    [Fact]
    public async Task GetScheduledInvoicesAsync_ShouldNotReturnInvoice_WhenNextOccurrenceIsAfterDate()
    {
        await using var dbContext = fixture.CreateDbContext();

        var date = new DateOnly(2026, 2, 1);

        var user = await Shared.CreateUser(dbContext);

        var schedule = CreateSchedule();
        schedule.NextOccurrence = date.AddDays(1);

        var invoice = CreateInvoiceTemplate(
            user.Id,
            schedule);

        dbContext.InvoiceTemplates.Add(invoice);
        await dbContext.SaveChangesAsync();

        var repositoryManager = CreateRepositoryManager();
        var userManager = CreateUserManager();
        var scheduleService =
            CreateScheduleService(repositoryManager.Object);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.AsQueryable())
            .Returns(dbContext.InvoiceTemplates);

        repositoryManager
            .Setup(x => x.InvoiceDeliveries.AsQueryable())
            .Returns(dbContext.InvoiceDeliveries);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        // Act
        var result =
            await service.GetScheduledInvoicesAsync(date);

        // Assert
        Assert.DoesNotContain(
            result,
            x => x.Id == invoice.Id);
    }

    [Fact]
    public async Task GetScheduledInvoicesAsync_ShouldNotReturnInvoice_WhenAlreadyDeliveredOnDate()
    {
        await using var dbContext = fixture.CreateDbContext();

        var date = new DateOnly(2026, 2, 1);

        var user = await Shared.CreateUser(dbContext);

        var schedule = CreateSchedule();
        schedule.NextOccurrence = date;

        var invoiceTemplate = CreateInvoiceTemplate(
            user.Id,
            schedule);

        dbContext.InvoiceTemplates.Add(invoiceTemplate);
        await dbContext.SaveChangesAsync();

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Number = "000001"
        };

        dbContext.Invoices.Add(invoice);
        await dbContext.SaveChangesAsync();

        var delivery = new InvoiceDelivery
        {
            InvoiceId = invoice.Id,
            InvoiceTemplateId = invoiceTemplate.Id,
            ScheduleId = schedule.Id,
            DateCreated = date
        };

        dbContext.InvoiceDeliveries.Add(delivery);
        await dbContext.SaveChangesAsync();

        var repositoryManager = CreateRepositoryManager();
        var userManager = CreateUserManager();
        var scheduleService =
            CreateScheduleService(repositoryManager.Object);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.AsQueryable())
            .Returns(dbContext.InvoiceTemplates);

        repositoryManager
            .Setup(x => x.InvoiceDeliveries.AsQueryable())
            .Returns(dbContext.InvoiceDeliveries);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        // Act
        var result =
            await service.GetScheduledInvoicesAsync(date);

        // Assert
        Assert.DoesNotContain(
            result,
            x => x.Id == invoiceTemplate.Id);

    }

    [Fact]
    public async Task GetScheduledInvoicesAsync_ShouldReturnInvoice_WhenDeliveryExistsForDifferentDate()
    {
        await using var dbContext = fixture.CreateDbContext();

        var date = new DateOnly(2026, 2, 1);

        var user = await Shared.CreateUser(dbContext);

        // InvoiceTemplate
        var schedule = CreateSchedule();
        schedule.NextOccurrence = date;

        var invoiceTemplate = CreateInvoiceTemplate(
            user.Id,
            schedule);

        dbContext.InvoiceTemplates.Add(invoiceTemplate);
        await dbContext.SaveChangesAsync();

        // Invoice generated from the InvoiceTemplate
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Number = "000001"
        };

        dbContext.Invoices.Add(invoice);
        await dbContext.SaveChangesAsync();

        // InvoiceDelivery references the Invoice, InvoiceTemplate and Schedule
        var delivery = new InvoiceDelivery
        {
            InvoiceId = invoice.Id,
            InvoiceTemplateId = invoiceTemplate.Id,
            ScheduleId = schedule.Id,
            DateCreated = date.AddDays(-1)
        };

        dbContext.InvoiceDeliveries.Add(delivery);
        await dbContext.SaveChangesAsync();

        var repositoryManager = CreateRepositoryManager();
        var userManager = CreateUserManager();
        var scheduleService =
            CreateScheduleService(repositoryManager.Object);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.AsQueryable())
            .Returns(dbContext.InvoiceTemplates);

        repositoryManager
            .Setup(x => x.InvoiceDeliveries.AsQueryable())
            .Returns(dbContext.InvoiceDeliveries);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        // Act
        var result =
            await service.GetScheduledInvoicesAsync(date);

        // Assert
        Assert.Contains(
            result,
            x => x.Id == invoiceTemplate.Id);


    }

    [Fact]
    public async Task PutAsync_ShouldReturnNotFound_WhenInvoiceTemplateDoesNotExist()
    {
        await using var dbContext = fixture.CreateDbContext();

        var repositoryManager = CreateRepositoryManager();
        var userManager = CreateUserManager();
        var scheduleService =
            CreateScheduleService(repositoryManager.Object);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.GetByIdAsync(
                It.IsAny<Guid>()))
            .ReturnsAsync((InvoiceTemplate?)null);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        var request = CreateUpdateRequest();

        // Act
        var result = await service.PutAsync(
            Guid.NewGuid(),
            request);

        // Assert
        Assert.True(result.IsT1);
        Assert.IsType<NotFound>(result.AsT1);

        repositoryManager.Verify(
            x => x.InvoiceTemplates.Update(
                It.IsAny<InvoiceTemplate>()),
            Times.Never);

        repositoryManager.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task PutAsync_ShouldUpdateInvoiceTemplate_WhenNewScheduleIsCreated()
    {
        await using var dbContext = fixture.CreateDbContext();

        var user = await Shared.CreateUser(dbContext);

        var oldSchedule = CreateSchedule();

        var invoice = CreateInvoiceTemplate(
            user.Id,
            oldSchedule);

        var repositoryManager = CreateRepositoryManager();
        var userManager = CreateUserManager();

        var scheduleRepository = new Mock<IScheduleRepository>();

        repositoryManager
            .SetupGet(x => x.Schedules)
            .Returns(scheduleRepository.Object);

        scheduleRepository
            .Setup(x => x.AsQueryable())
            .Returns(dbContext.Schedules);

        var createdScheduleId = Guid.NewGuid();

        scheduleRepository
            .Setup(x => x.CreateAsync(
                It.IsAny<Schedule>()))
            .Callback<Schedule>(schedule =>
            {
                schedule.Id = createdScheduleId;
            })
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.GetByIdAsync(invoice.Id))
            .ReturnsAsync(invoice);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.Update(invoice))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var scheduleService =
            CreateScheduleService(repositoryManager.Object);

        var request = CreateUpdateRequest(
            price: 300m,
            description: "Updated invoice",
            startDate: new DateOnly(2026, 3, 1),
            endDate: new DateOnly(2026, 12, 31));

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        // Act
        var result = await service.PutAsync(
            invoice.Id,
            request);

        // Assert
        Assert.True(result.IsT0);

        var response = result.AsT0;

        Assert.Equal(invoice.Id, response.Id);
        Assert.Equal(invoice.UserId, response.UserId);
        Assert.Equal(300m, response.Price);
        Assert.Equal(
            "Updated invoice",
            response.Description);
        Assert.Equal(
            createdScheduleId,
            response.ScheduleId);

        Assert.Equal(300m, invoice.Price);
        Assert.Equal(
            "Updated invoice",
            invoice.Description);
        Assert.Equal(
            createdScheduleId,
            invoice.ScheduleId);

        scheduleRepository.Verify(
            x => x.CreateAsync(
                It.IsAny<Schedule>()),
            Times.Once);

        repositoryManager.Verify(
            x => x.InvoiceTemplates.Update(invoice),
            Times.Once);

        repositoryManager.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task PutAsync_ShouldReuseExistingSchedule()
    {
        await using var dbContext = fixture.CreateDbContext();

        var user = await Shared.CreateUser(dbContext);

        var schedule = CreateSchedule();

        dbContext.Schedules.Add(schedule);
        await dbContext.SaveChangesAsync();

        var invoice = CreateInvoiceTemplate(
            user.Id,
            schedule);

        var repositoryManager = CreateRepositoryManager();
        var userManager = CreateUserManager();

        var scheduleRepository = new Mock<IScheduleRepository>();

        repositoryManager
            .SetupGet(x => x.Schedules)
            .Returns(scheduleRepository.Object);

        scheduleRepository
            .Setup(x => x.AsQueryable())
            .Returns(dbContext.Schedules);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.GetByIdAsync(invoice.Id))
            .ReturnsAsync(invoice);

        repositoryManager
            .Setup(x => x.InvoiceTemplates.Update(invoice))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var scheduleService =
            CreateScheduleService(repositoryManager.Object);

        var request = CreateUpdateRequest(
            startDate: schedule.StartDate,
            endDate: schedule.EndDate);

        var service = new InvoiceTemplateService(
            repositoryManager.Object,
            userManager.Object,
            scheduleService);

        // Act
        var result = await service.PutAsync(
            invoice.Id,
            request);

        // Assert
        Assert.True(result.IsT0);

        Assert.Equal(
            schedule.Id,
            result.AsT0.ScheduleId);

        scheduleRepository.Verify(
            x => x.CreateAsync(
                It.IsAny<Schedule>()),
            Times.Never);

        repositoryManager.Verify(
            x => x.InvoiceTemplates.Update(invoice),
            Times.Once);

        repositoryManager.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    private static Mock<IRepositoryManager> CreateRepositoryManager()
    {
        return new Mock<IRepositoryManager>();
    }

    private static Mock<UserManager<AppUser>> CreateUserManager()
    {
        var store = new Mock<IUserStore<AppUser>>();

        return new Mock<UserManager<AppUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);
    }

    private static ScheduleService CreateScheduleService(
        IRepositoryManager repositoryManager)
    {
        return new ScheduleService(repositoryManager);
    }

    private static Schedule CreateSchedule(
        DateOnly? startDate = null,
        DateOnly? endDate = null)
    {
        return new Schedule
        {
            Id = Guid.NewGuid(),
            StartDate =
                startDate ?? new DateOnly(2026, 1, 1),
            EndDate =
                endDate ?? new DateOnly(2026, 12, 31),
            NextOccurrence =
                new DateOnly(2026, 2, 1),
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
        Guid userId,
        DateOnly? startDate = null,
        DateOnly? endDate = null)
    {
        return new CreateInvoiceTemplateRequest
        {
            UserId = userId,
            Price = 100m,
            Description = "Test invoice",
            Schedule = new CreateInvoiceTemplateScheduleRequest
            {
                StartDate =
                    startDate ?? new DateOnly(2026, 1, 1),
                EndDate =
                    endDate ?? new DateOnly(2026, 12, 31),
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
                StartDate =
                    startDate ?? new DateOnly(2026, 1, 1),
                EndDate =
                    endDate ?? new DateOnly(2026, 12, 31),
                Frequency = Frequency.Monthly,
                Interval = 1,
                DayOfMonth = 1
            }
        };
    }

}