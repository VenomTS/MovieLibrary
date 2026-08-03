using API.Schedules;
using Models.Schedules;
using Models.Schedules.Rules;
using Moq;
using Repositories;

namespace UnitTests.Scheduler;

public class SchedulerGetExistingOrCreateTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
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