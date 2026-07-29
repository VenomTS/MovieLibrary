using API.Schedules;
using Models.Schedules.Rules;

namespace UnitTests;

public class DailySchedulerTests
{
    [Fact]
    public void Daily_EveryDay_ReturnsTomorrow()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            startDate: new DateOnly(2025, 7, 1),
            interval: 1);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15));

        Assert.Equal(
            new DateOnly(2025, 7, 16),
            result);
    }


    [Fact]
    public void Daily_EveryThreeDays_ReturnsCorrectDate()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            startDate: new DateOnly(2025, 7, 1),
            interval: 3);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15));

        Assert.Equal(
            new DateOnly(2025, 7, 18),
            result);
    }


    [Fact]
    public void Daily_WhenStartDateIsFuture_UsesStartDate()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            startDate: new DateOnly(2025, 7, 20),
            interval: 1);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15));

        Assert.Equal(
            new DateOnly(2025, 7, 21),
            result);
    }

    [Fact]
    public void Daily_IntervalOne_ReturnsNextDay()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2025, 7, 1),
            1);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15));

        Assert.Equal(
            new DateOnly(2025, 7, 16),
            result);
    }


    [Fact]
    public void Daily_IntervalTwo_ReturnsTwoDaysLater()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2025, 7, 1),
            2);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15));

        Assert.Equal(
            new DateOnly(2025, 7, 17),
            result);
    }


    [Fact]
    public void Daily_IntervalSeven_ReturnsOneWeekLater()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2025, 7, 1),
            7);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15));

        Assert.Equal(
            new DateOnly(2025, 7, 22),
            result);
    }


    [Fact]
    public void Daily_StartDateInFuture_UsesStartDate()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2025, 7, 20),
            1);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15));

        Assert.Equal(
            new DateOnly(2025, 7, 21),
            result);
    }


    [Fact]
    public void Daily_StartDateEqualsToday_ReturnsTomorrow()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2025, 7, 15),
            1);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15));

        Assert.Equal(
            new DateOnly(2025, 7, 16),
            result);
    }


    [Fact]
    public void Daily_StartDateBeforeToday_IgnoresOldStartDate()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2025, 7, 1),
            1);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15));

        Assert.Equal(
            new DateOnly(2025, 7, 16),
            result);
    }


    [Fact]
    public void Daily_IntervalThirtyDays_ReturnsNextMonthApproximately()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2025, 7, 1),
            30);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15));

        Assert.Equal(
            new DateOnly(2025, 8, 14),
            result);
    }


    [Fact]
    public void Daily_CrossesMonthBoundary()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2025, 7, 1),
            1);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 31));

        Assert.Equal(
            new DateOnly(2025, 8, 1),
            result);
    }


    [Fact]
    public void Daily_CrossesYearBoundary()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2025, 12, 31),
            1);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 12, 31));

        Assert.Equal(
            new DateOnly(2026, 1, 1),
            result);
    }


    [Fact]
    public void Daily_LeapYear_February29()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2024, 2, 28),
            1);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2024, 2, 28));

        Assert.Equal(
            new DateOnly(2024, 2, 29),
            result);
    }


    [Fact]
    public void Daily_AfterLeapDay_ReturnsMarchFirst()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2024, 2, 29),
            1);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2024, 2, 29));

        Assert.Equal(
            new DateOnly(2024, 3, 1),
            result);
    }


    [Fact]
    public void Daily_NonLeapFebruary28_ReturnsMarchFirst()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2025, 2, 28),
            1);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 2, 28));

        Assert.Equal(
            new DateOnly(2025, 3, 1),
            result);
    }


    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(100)]
    public void Daily_VariousIntervals_ReturnsCorrectFutureDate(int interval)
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2025, 7, 1),
            interval);

        var today = new DateOnly(2025, 7, 10);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            today);

        Assert.Equal(
            today.AddDays(interval),
            result);
    }


    [Fact]
    public void Daily_LargeInterval_Works()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2025, 1, 1),
            365);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 6, 1));

        Assert.Equal(
            new DateOnly(2026, 6, 1),
            result);
    }
        
    [Fact]
    public void Daily_WhenCalledAfterLastTrigger_ReturnsNextOccurrence()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2025, 7, 1),
            3);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 10));

        Assert.Equal(
            new DateOnly(2025, 7, 13),
            result);
    }
        
    [Fact]
    public void Daily_IntervalOneThousandDays_Works()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2025, 1, 1),
            1000);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 1, 1));

        Assert.Equal(
            new DateOnly(2027, 9, 28),
            result);
    }
        
    [Fact]
    public void Daily_LeapYear_FromFebruary28_ReturnsFebruary29()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2024, 2, 28),
            1);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2024, 2, 28));

        Assert.Equal(
            new DateOnly(2024, 2, 29),
            result);
    }


    [Fact]
    public void Daily_LeapYear_FromFebruary29_ReturnsMarchFirst()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2024, 2, 29),
            1);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2024, 2, 29));

        Assert.Equal(
            new DateOnly(2024, 3, 1),
            result);
    }


    [Fact]
    public void Daily_LeapYear_CrossesLeapDayWithIntervalTwo()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2024, 2, 28),
            2);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2024, 2, 28));

        Assert.Equal(
            new DateOnly(2024, 3, 1),
            result);
    }


    [Fact]
    public void Daily_NonLeapYear_FromFebruary28_ReturnsMarchFirst()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2025, 2, 28),
            1);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 2, 28));

        Assert.Equal(
            new DateOnly(2025, 3, 1),
            result);
    }


    [Fact]
    public void Daily_LeapYear_StartOfFebruary_ReturnsCorrectDate()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2024, 2, 1),
            28);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2024, 2, 1));

        Assert.Equal(
            new DateOnly(2024, 2, 29),
            result);
    }


    [Fact]
    public void Daily_LeapYear_CrossesIntoMarchWithLargeInterval()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2024, 2, 1),
            60);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2024, 2, 1));

        Assert.Equal(
            new DateOnly(2024, 4, 1),
            result);
    }


    [Fact]
    public void Daily_CrossesLeapYearBoundary()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2023, 12, 31),
            1);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2023, 12, 31));

        Assert.Equal(
            new DateOnly(2024, 1, 1),
            result);
    }


    [Fact]
    public void Daily_FromLeapYearToNextYear_Works()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2024, 2, 29),
            366);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2024, 2, 29));

        Assert.Equal(
            new DateOnly(2025, 3, 1),
            result);
    }


    [Fact]
    public void Daily_Year2000_IsLeapYear()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2000, 2, 28),
            1);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2000, 2, 28));

        Assert.Equal(
            new DateOnly(2000, 2, 29),
            result);
    }


    [Fact]
    public void Daily_Year2100_IsNotLeapYear()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(2100, 2, 28),
            1);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2100, 2, 28));

        Assert.Equal(
            new DateOnly(2100, 3, 1),
            result);
    }


    [Theory]
    [InlineData(2020)]
    [InlineData(2024)]
    [InlineData(2028)]
    public void Daily_KnownLeapYears_ContainFebruary29(int year)
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(year, 2, 28),
            1);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(year, 2, 28));

        Assert.Equal(
            new DateOnly(year, 2, 29),
            result);
    }
    
    [Theory]
    [InlineData(2020)]
    [InlineData(2024)]
    [InlineData(2028)]
    public void Daily_KnownLeapYears_ContainFebruary29LargeGap(int year)
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(year, 2, 28),
            25);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(year, 2, 28));

        Assert.Equal(
            new DateOnly(year, 3, 24),
            result);
    }


    [Theory]
    [InlineData(2021)]
    [InlineData(2022)]
    [InlineData(2023)]
    [InlineData(2025)]
    public void Daily_KnownNonLeapYearsSkipFebruary29(int year)
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Daily,
            new DateOnly(year, 2, 28),
            1);

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(year, 2, 28));

        Assert.Equal(
            new DateOnly(year, 3, 1),
            result);
    }
}