using Repositories.Interfaces;

namespace UnitTests.Scheduler;

using API.Schedules;
using Models.Schedules;
using Models.Schedules.Rules;
using Moq;
using Repositories;

public class ScheduleServiceTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task UpdateNextOccurrenceAsync_ShouldUpdateNextOccurrenceAndSaveChanges()
    {
        // Arrange
        var repositoryManager = new Mock<IRepositoryManager>();
        var scheduleRepository = new Mock<IScheduleRepository>();

        repositoryManager
            .Setup(x => x.Schedules)
            .Returns(scheduleRepository.Object);

        scheduleRepository
            .Setup(x => x.Update(It.IsAny<Schedule>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var schedule = new Schedule
        {
            Id = Guid.NewGuid(),
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10)),
            EndDate = null,
            RecurrenceRule = new RecurrenceRule
            {
                Frequency = Frequency.Daily,
                Interval = 1
            }
        };

        var expectedNextOccurrence =
            DateOnly.FromDateTime(DateTime.Now).AddDays(1);

        var service = new ScheduleService(repositoryManager.Object);

        // Act
        await service.UpdateNextOccurrenceAsync(schedule);

        // Assert
        Assert.Equal(
            expectedNextOccurrence,
            schedule.NextOccurrence);

        scheduleRepository.Verify(
            x => x.Update(schedule),
            Times.Once);

        repositoryManager.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCalculateNextOccurrenceCreateScheduleAndSaveChanges()
    {
        // Arrange
        var repositoryManager = new Mock<IRepositoryManager>();
        var scheduleRepository = new Mock<IScheduleRepository>();

        repositoryManager
            .Setup(x => x.Schedules)
            .Returns(scheduleRepository.Object);

        scheduleRepository
            .Setup(x => x.CreateAsync(It.IsAny<Schedule>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var schedule = new Schedule
        {
            Id = Guid.NewGuid(),
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10)),
            EndDate = null,
            RecurrenceRule = new RecurrenceRule
            {
                Frequency = Frequency.Daily,
                Interval = 1
            }
        };

        var expectedNextOccurrence =
            DateOnly.FromDateTime(DateTime.Now).AddDays(1);

        var service = new ScheduleService(repositoryManager.Object);

        // Act
        var result = await service.CreateAsync(schedule);

        // Assert
        Assert.Same(schedule, result);

        Assert.Equal(
            expectedNextOccurrence,
            result.NextOccurrence);

        scheduleRepository.Verify(
            x => x.CreateAsync(schedule),
            Times.Once);

        repositoryManager.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }
    
    [Fact]
    public async Task FindOrCreateSchedule_ShouldMerge_WhenBothStartDatesAreInThePast()
    {
        await using var dbContext = fixture.CreateDbContext();
        await fixture.ClearDatabaseAsync();

        var repositoryManager = new Mock<IRepositoryManager>();

        repositoryManager
            .Setup(x => x.Schedules.AsQueryable())
            .Returns(dbContext.Schedules);

        repositoryManager
            .Setup(x => x.Schedules.CreateAsync(It.IsAny<Schedule>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var service = new ScheduleService(repositoryManager.Object);

        var existingSchedule = new Schedule
        {
            Id = Guid.NewGuid(),
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = null,
            RecurrenceRule = new RecurrenceRule
            {
                Frequency = Frequency.Monthly,
                Interval = 1,
                Ordinal = Ordinal.First,
                OrdinalType = OrdinalType.Monday
            }
        };

        dbContext.Schedules.Add(existingSchedule);
        await dbContext.SaveChangesAsync();

        var schedule = new Schedule
        {
            StartDate = new DateOnly(2026, 5, 1),
            EndDate = null,
            RecurrenceRule = new RecurrenceRule
            {
                Frequency = Frequency.Monthly,
                Interval = 1,
                Ordinal = Ordinal.First,
                OrdinalType = OrdinalType.Monday
            }
        };

        var result = await service.FindOrCreateSchedule(schedule);

        Assert.Equal(existingSchedule.Id, result.Id);
    }
    
    [Fact]
    public async Task FindOrCreateSchedule_ShouldMerge_WhenBothStartDatesAreInThePastInverse()
    {
        await using var dbContext = fixture.CreateDbContext();
        await fixture.ClearDatabaseAsync();

        var repositoryManager = new Mock<IRepositoryManager>();

        repositoryManager
            .Setup(x => x.Schedules.AsQueryable())
            .Returns(dbContext.Schedules);

        repositoryManager
            .Setup(x => x.Schedules.CreateAsync(It.IsAny<Schedule>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var service = new ScheduleService(repositoryManager.Object);

        var existingSchedule = new Schedule
        {
            Id = Guid.NewGuid(),
            StartDate = new DateOnly(2026, 5, 1),
            EndDate = null,
            RecurrenceRule = new RecurrenceRule
            {
                Frequency = Frequency.Monthly,
                Interval = 1,
                Ordinal = Ordinal.First,
                OrdinalType = OrdinalType.Monday
            }
        };

        dbContext.Schedules.Add(existingSchedule);
        await dbContext.SaveChangesAsync();

        var schedule = new Schedule
        {
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = null,
            RecurrenceRule = new RecurrenceRule
            {
                Frequency = Frequency.Monthly,
                Interval = 1,
                Ordinal = Ordinal.First,
                OrdinalType = OrdinalType.Monday
            }
        };

        var result = await service.FindOrCreateSchedule(schedule);

        Assert.Equal(existingSchedule.Id, result.Id);
    }
    
    [Fact]
    public async Task FindOrCreateSchedule_ShouldMerge_WhenBothStartDatesAreInTheFutureAndEqual()
    {
        await using var dbContext = fixture.CreateDbContext();
        await fixture.ClearDatabaseAsync();

        var repositoryManager = new Mock<IRepositoryManager>();

        repositoryManager
            .Setup(x => x.Schedules.AsQueryable())
            .Returns(dbContext.Schedules);

        repositoryManager
            .Setup(x => x.Schedules.CreateAsync(It.IsAny<Schedule>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var service = new ScheduleService(repositoryManager.Object);

        var existingSchedule = new Schedule
        {
            Id = Guid.NewGuid(),
            StartDate = new DateOnly(2026, 10, 1),
            EndDate = null,
            RecurrenceRule = new RecurrenceRule
            {
                Frequency = Frequency.Monthly,
                Interval = 1,
                Ordinal = Ordinal.First,
                OrdinalType = OrdinalType.Monday
            }
        };

        dbContext.Schedules.Add(existingSchedule);
        await dbContext.SaveChangesAsync();

        var schedule = new Schedule
        {
            StartDate = new DateOnly(2026, 10, 1),
            EndDate = null,
            RecurrenceRule = new RecurrenceRule
            {
                Frequency = Frequency.Monthly,
                Interval = 1,
                Ordinal = Ordinal.First,
                OrdinalType = OrdinalType.Monday
            }
        };

        var result = await service.FindOrCreateSchedule(schedule);

        Assert.Equal(existingSchedule.Id, result.Id);
    }
    
    [Fact]
    public async Task FindOrCreateSchedule_ShouldNotMerge_WhenBothStartDatesAreInTheFutureButDifferent()
    {
        await using var dbContext = fixture.CreateDbContext();
        await fixture.ClearDatabaseAsync();

        var repositoryManager = new Mock<IRepositoryManager>();

        repositoryManager
            .Setup(x => x.Schedules.AsQueryable())
            .Returns(dbContext.Schedules);

        repositoryManager
            .Setup(x => x.Schedules.CreateAsync(It.IsAny<Schedule>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var service = new ScheduleService(repositoryManager.Object);

        var existingSchedule = new Schedule
        {
            Id = Guid.NewGuid(),
            StartDate = new DateOnly(2026, 10, 1),
            EndDate = null,
            RecurrenceRule = new RecurrenceRule
            {
                Frequency = Frequency.Monthly,
                Interval = 1,
                Ordinal = Ordinal.First,
                OrdinalType = OrdinalType.Monday
            }
        };

        dbContext.Schedules.Add(existingSchedule);
        await dbContext.SaveChangesAsync();

        var schedule = new Schedule
        {
            StartDate = new DateOnly(2026, 11, 1),
            EndDate = null,
            RecurrenceRule = new RecurrenceRule
            {
                Frequency = Frequency.Monthly,
                Interval = 1,
                Ordinal = Ordinal.First,
                OrdinalType = OrdinalType.Monday
            }
        };

        var result = await service.FindOrCreateSchedule(schedule);

        Assert.NotEqual(existingSchedule.Id, result.Id);
    }
    
    [Fact]
    public async Task FindOrCreateSchedule_ShouldNotMerge_WhenBothStartDatesAreInTheFutureButDifferentInverse()
    {
        await using var dbContext = fixture.CreateDbContext();
        await fixture.ClearDatabaseAsync();

        var repositoryManager = new Mock<IRepositoryManager>();

        repositoryManager
            .Setup(x => x.Schedules.AsQueryable())
            .Returns(dbContext.Schedules);

        repositoryManager
            .Setup(x => x.Schedules.CreateAsync(It.IsAny<Schedule>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var service = new ScheduleService(repositoryManager.Object);

        var existingSchedule = new Schedule
        {
            Id = Guid.NewGuid(),
            StartDate = new DateOnly(2026, 11, 1),
            EndDate = null,
            RecurrenceRule = new RecurrenceRule
            {
                Frequency = Frequency.Monthly,
                Interval = 1,
                Ordinal = Ordinal.First,
                OrdinalType = OrdinalType.Monday
            }
        };

        dbContext.Schedules.Add(existingSchedule);
        await dbContext.SaveChangesAsync();

        var schedule = new Schedule
        {
            StartDate = new DateOnly(2026, 10, 1),
            EndDate = null,
            RecurrenceRule = new RecurrenceRule
            {
                Frequency = Frequency.Monthly,
                Interval = 1,
                Ordinal = Ordinal.First,
                OrdinalType = OrdinalType.Monday
            }
        };

        var result = await service.FindOrCreateSchedule(schedule);

        Assert.NotEqual(existingSchedule.Id, result.Id);
    }
    
    [Fact]
    public async Task FindOrCreateSchedule_ShouldNotMerge_WhenOneStartDateIsInThePastAndOtherIsInTheFuture()
    {
        await using var dbContext = fixture.CreateDbContext();
        await fixture.ClearDatabaseAsync();

        var repositoryManager = new Mock<IRepositoryManager>();

        repositoryManager
            .Setup(x => x.Schedules.AsQueryable())
            .Returns(dbContext.Schedules);

        repositoryManager
            .Setup(x => x.Schedules.CreateAsync(It.IsAny<Schedule>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var service = new ScheduleService(repositoryManager.Object);

        var existingSchedule = new Schedule
        {
            Id = Guid.NewGuid(),
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = null,
            RecurrenceRule = new RecurrenceRule
            {
                Frequency = Frequency.Monthly,
                Interval = 1,
                Ordinal = Ordinal.First,
                OrdinalType = OrdinalType.Monday
            }
        };

        dbContext.Schedules.Add(existingSchedule);
        await dbContext.SaveChangesAsync();

        var schedule = new Schedule
        {
            StartDate = new DateOnly(2026, 10, 1),
            EndDate = null,
            RecurrenceRule = new RecurrenceRule
            {
                Frequency = Frequency.Monthly,
                Interval = 1,
                Ordinal = Ordinal.First,
                OrdinalType = OrdinalType.Monday
            }
        };

        var result = await service.FindOrCreateSchedule(schedule);

        Assert.NotEqual(existingSchedule.Id, result.Id);
    }
    
    [Fact]
    public async Task FindOrCreateSchedule_ShouldNotMerge_WhenOneStartDateIsInThePastAndOtherIsInTheFutureInverse()
    {
        await using var dbContext = fixture.CreateDbContext();
        await fixture.ClearDatabaseAsync();

        var repositoryManager = new Mock<IRepositoryManager>();

        repositoryManager
            .Setup(x => x.Schedules.AsQueryable())
            .Returns(dbContext.Schedules);

        repositoryManager
            .Setup(x => x.Schedules.CreateAsync(It.IsAny<Schedule>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var service = new ScheduleService(repositoryManager.Object);

        var existingSchedule = new Schedule
        {
            Id = Guid.NewGuid(),
            StartDate = new DateOnly(2026, 10, 1),
            EndDate = null,
            RecurrenceRule = new RecurrenceRule
            {
                Frequency = Frequency.Monthly,
                Interval = 1,
                Ordinal = Ordinal.First,
                OrdinalType = OrdinalType.Monday
            }
        };

        dbContext.Schedules.Add(existingSchedule);
        await dbContext.SaveChangesAsync();

        var schedule = new Schedule
        {
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = null,
            RecurrenceRule = new RecurrenceRule
            {
                Frequency = Frequency.Monthly,
                Interval = 1,
                Ordinal = Ordinal.First,
                OrdinalType = OrdinalType.Monday
            }
        };

        var result = await service.FindOrCreateSchedule(schedule);

        Assert.NotEqual(existingSchedule.Id, result.Id);
    }
}