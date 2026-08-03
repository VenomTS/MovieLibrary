using Microsoft.EntityFrameworkCore;
using Models.Invoices;
using Models.Schedules;
using Models.Schedules.Rules;
using Repositories.Implementations;

namespace UnitTests.InvoiceTemplates;

public class InvoiceTemplateRepositoryTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task AsQueryable_ShouldReturnInvoiceTemplates()
    {
        // Arrange
        await using var dbContext = fixture.CreateDbContext();

        var user = await Shared.CreateUser(dbContext);
        var schedule = CreateSchedule();

        var invoiceTemplate = CreateInvoiceTemplate(
            user.Id,
            schedule);

        dbContext.InvoiceTemplates.Add(invoiceTemplate);
        await dbContext.SaveChangesAsync();

        var repository = new InvoiceTemplateRepository(dbContext);

        // Act
        var result = await repository
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == invoiceTemplate.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(invoiceTemplate.Id, result.Id);
        Assert.Equal(user.Id, result.UserId);
    }


    [Fact]
    public async Task GetByIdAsync_ShouldReturnInvoiceTemplate()
    {
        // Arrange
        await using var dbContext = fixture.CreateDbContext();

        var user = await Shared.CreateUser(dbContext);

        var schedule = CreateSchedule();

        var invoiceTemplate = CreateInvoiceTemplate(
            user.Id,
            schedule);

        dbContext.InvoiceTemplates.Add(invoiceTemplate);
        await dbContext.SaveChangesAsync();

        var repository = new InvoiceTemplateRepository(dbContext);

        // Act
        var result = await repository.GetByIdAsync(invoiceTemplate.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(invoiceTemplate.Id, result.Id);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(invoiceTemplate.Price, result.Price);
        Assert.Equal(invoiceTemplate.Description, result.Description);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenInvoiceTemplateDoesNotExist()
    {
        // Arrange
        await using var dbContext = fixture.CreateDbContext();
        var repository = new InvoiceTemplateRepository(dbContext);

        // Act
        var result = await repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldIncludeRequestedNavigationProperty()
    {
        // Arrange
        await using var dbContext = fixture.CreateDbContext();

        var user = await Shared.CreateUser(dbContext);
        var schedule = CreateSchedule();

        var invoiceTemplate = CreateInvoiceTemplate(
            user.Id,
            schedule);

        dbContext.InvoiceTemplates.Add(invoiceTemplate);
        await dbContext.SaveChangesAsync();

        await dbContext.DisposeAsync();

        await using var queryContext = fixture.CreateDbContext();
        var repository = new InvoiceTemplateRepository(queryContext);

        // Act
        var result = await repository.GetByIdAsync(
            invoiceTemplate.Id,
            x => x.Schedule);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Schedule);
        Assert.Equal(schedule.Id, result.Schedule.Id);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateInvoiceTemplate()
    {
        // Arrange
        await using var dbContext = fixture.CreateDbContext();

        var user = await Shared.CreateUser(dbContext);
        var schedule = CreateSchedule();

        var invoiceTemplate = CreateInvoiceTemplate(
            user.Id,
            schedule);

        var repository = new InvoiceTemplateRepository(dbContext);

        // Act
        await repository.CreateAsync(invoiceTemplate);
        await repository.SaveChangesAsync();

        // Assert
        var created = await dbContext.InvoiceTemplates
            .FirstOrDefaultAsync(x => x.Id == invoiceTemplate.Id);

        Assert.NotNull(created);
        Assert.Equal(user.Id, created.UserId);
        Assert.Equal(100m, created.Price);
        Assert.Equal("Test invoice", created.Description);
    }

    [Fact]
    public async Task Update_ShouldUpdateInvoiceTemplate()
    {
        // Arrange
        await using var dbContext = fixture.CreateDbContext();

        var user = await Shared.CreateUser(dbContext);
        var schedule = CreateSchedule();

        var invoiceTemplate = CreateInvoiceTemplate(
            user.Id,
            schedule);

        dbContext.InvoiceTemplates.Add(invoiceTemplate);
        await dbContext.SaveChangesAsync();

        var repository = new InvoiceTemplateRepository(dbContext);

        // Act
        invoiceTemplate.Price = 250m;
        invoiceTemplate.Description = "Updated invoice";

        await repository.Update(invoiceTemplate);
        await repository.SaveChangesAsync();

        // Assert
        var updated = await dbContext.InvoiceTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == invoiceTemplate.Id);

        Assert.NotNull(updated);
        Assert.Equal(250m, updated.Price);
        Assert.Equal("Updated invoice", updated.Description);
    }

    [Fact]
    public async Task Delete_ShouldDeleteInvoiceTemplate()
    {
        // Arrange
        await using var dbContext = fixture.CreateDbContext();

        var user = await Shared.CreateUser(dbContext);
        var schedule = CreateSchedule();

        var invoiceTemplate = CreateInvoiceTemplate(
            user.Id,
            schedule);

        dbContext.InvoiceTemplates.Add(invoiceTemplate);
        await dbContext.SaveChangesAsync();

        var repository = new InvoiceTemplateRepository(dbContext);

        // Act
        await repository.Delete(invoiceTemplate);
        await repository.SaveChangesAsync();

        // Assert
        var deleted = await dbContext.InvoiceTemplates
            .FirstOrDefaultAsync(x => x.Id == invoiceTemplate.Id);

        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteById_ShouldDeleteInvoiceTemplate()
    {
        // Arrange
        await using var dbContext = fixture.CreateDbContext();

        var user = await Shared.CreateUser(dbContext);
        var schedule = CreateSchedule();

        var invoiceTemplate = CreateInvoiceTemplate(
            user.Id,
            schedule);

        dbContext.InvoiceTemplates.Add(invoiceTemplate);
        await dbContext.SaveChangesAsync();

        var repository = new InvoiceTemplateRepository(dbContext);

        // Act
        await repository.DeleteById(invoiceTemplate.Id);
        await repository.SaveChangesAsync();

        // Assert
        var deleted = await dbContext.InvoiceTemplates
            .FirstOrDefaultAsync(x => x.Id == invoiceTemplate.Id);

        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteById_ShouldNotDeleteAnything_WhenIdDoesNotExist()
    {
        // Arrange
        await using var dbContext = fixture.CreateDbContext();

        var repository = new InvoiceTemplateRepository(dbContext);

        var nonExistingId = Guid.NewGuid();

        // Act
        await repository.DeleteById(nonExistingId);
        await repository.SaveChangesAsync();

        // Assert
        Assert.False(
            await dbContext.InvoiceTemplates
                .AnyAsync(x => x.Id == nonExistingId));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllInvoiceTemplates()
    {
        // Arrange
        await using var dbContext = fixture.CreateDbContext();

        var user1 = await Shared.CreateUser(dbContext);
        var user2 = await Shared.CreateUser(dbContext);

        var schedule1 = CreateSchedule();
        var schedule2 = CreateSchedule();

        var template1 = CreateInvoiceTemplate(user1.Id, schedule1);
        var template2 = CreateInvoiceTemplate(user2.Id, schedule2);

        dbContext.InvoiceTemplates.AddRange(
            template1,
            template2);

        await dbContext.SaveChangesAsync();

        var repository = new InvoiceTemplateRepository(dbContext);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result, x => x.Id == template1.Id);
        Assert.Contains(result, x => x.Id == template2.Id);
    }

    [Fact]
    public async Task GetAllAsync_ShouldIncludeRequestedNavigationProperty()
    {
        // Arrange
        await using var dbContext = fixture.CreateDbContext();

        var user = await Shared.CreateUser(dbContext);
        var schedule = CreateSchedule();

        var invoiceTemplate = CreateInvoiceTemplate(
            user.Id,
            schedule);

        dbContext.InvoiceTemplates.Add(invoiceTemplate);
        await dbContext.SaveChangesAsync();

        await dbContext.DisposeAsync();

        await using var queryContext = fixture.CreateDbContext();
        var repository = new InvoiceTemplateRepository(queryContext);

        // Act
        var result = await repository.GetAllAsync(
            x => x.Schedule);

        // Assert
        var template = Assert.Single(result, x => x.Id == invoiceTemplate.Id);

        Assert.NotNull(template.Schedule);
        Assert.Equal(schedule.Id, template.Schedule.Id);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoInvoiceTemplatesExistForQuery()
    {
        // Arrange
        await using var dbContext = fixture.CreateDbContext();

        var repository = new InvoiceTemplateRepository(dbContext);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistChanges()
    {
        // Arrange
        await using var dbContext = fixture.CreateDbContext();

        var user = await Shared.CreateUser(dbContext);
        var schedule = CreateSchedule();

        var invoiceTemplate = CreateInvoiceTemplate(
            user.Id,
            schedule);

        var repository = new InvoiceTemplateRepository(dbContext);

        // Act
        await repository.CreateAsync(invoiceTemplate);
        await repository.SaveChangesAsync();

        // Assert
        var exists = await dbContext.InvoiceTemplates
            .AnyAsync(x => x.Id == invoiceTemplate.Id);

        Assert.True(exists);
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

}