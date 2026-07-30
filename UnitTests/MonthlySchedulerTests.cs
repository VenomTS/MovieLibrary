using API.Schedules;
using Models.Schedules.Rules;

namespace UnitTests;

public class MonthlySchedulerTests
{
    [Fact]
    public void Monthly_Day15_ReturnsNextMonth15()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            startDate: new DateOnly(2025, 7, 15),
            interval: 1);

        schedule.RecurrenceRule.DayOfMonth = 15;


        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15));


        Assert.Equal(
            new DateOnly(2025, 8, 15),
            result);
    }


    [Fact]
    public void Monthly_EveryThreeMonths_ReturnsCorrectMonth()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            startDate: new DateOnly(2025, 7, 15),
            interval: 3);

        schedule.RecurrenceRule.DayOfMonth = 15;


        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15));


        Assert.Equal(
            new DateOnly(2025, 10, 15),
            result);
    }


    [Fact]
    public void Monthly_FirstMonday_ReturnsCorrectDate()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            startDate: new DateOnly(2025, 7, 7),
            interval: 1);

        schedule.RecurrenceRule.Ordinal = Ordinal.First;
        schedule.RecurrenceRule.OrdinalType = OrdinalType.Monday;


        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 7));


        Assert.Equal(
            new DateOnly(2025, 8, 4),
            result);
    }


    [Fact]
    public void Monthly_LastFriday_ReturnsCorrectDate()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            startDate: new DateOnly(2025, 7, 25),
            interval: 1);

        schedule.RecurrenceRule.Ordinal = Ordinal.Last;
        schedule.RecurrenceRule.OrdinalType = OrdinalType.Friday;


        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 25));


        Assert.Equal(
            new DateOnly(2025, 8, 29),
            result);
    }


    [Fact]
    public void Monthly_LastWeekday_ReturnsFriday()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            startDate: new DateOnly(2025, 7, 31),
            interval: 1);

        schedule.RecurrenceRule.Ordinal = Ordinal.Last;
        schedule.RecurrenceRule.OrdinalType = OrdinalType.WeekDay;


        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 31));


        Assert.Equal(
            new DateOnly(2025, 8, 29),
            result);
    }
    
    [Fact]
    public void Monthly_DayOne_ReturnsNextMonthFirst()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 7, 1),
            1);

        schedule.RecurrenceRule.DayOfMonth = 1;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 1));

        Assert.Equal(
            new DateOnly(2025, 8, 1),
            result);
    }


    [Fact]
    public void Monthly_DayFifteenth_ReturnsNextMonthFifteenth()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 7, 15),
            1);

        schedule.RecurrenceRule.DayOfMonth = 15;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15));

        Assert.Equal(
            new DateOnly(2025, 8, 15),
            result);
    }


    [Fact]
    public void Monthly_DayLastDayOfMonth_ReturnsCorrectDate()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 7, 31),
            1);

        schedule.RecurrenceRule.DayOfMonth = 31;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 31));

        Assert.Equal(
            new DateOnly(2025, 8, 31),
            result);
    }


    [Fact]
    public void Monthly_EveryTwoMonths_SkipsMonth()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 7, 15),
            2);

        schedule.RecurrenceRule.DayOfMonth = 15;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15));

        Assert.Equal(
            new DateOnly(2025, 9, 15),
            result);
    }


    [Fact]
    public void Monthly_EveryTwelveMonths_ReturnsNextYear()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 7, 15),
            12);

        schedule.RecurrenceRule.DayOfMonth = 15;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15));

        Assert.Equal(
            new DateOnly(2026, 7, 15),
            result);
    }


    [Fact]
    public void Monthly_CrossesYearBoundary()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 12, 15),
            1);

        schedule.RecurrenceRule.DayOfMonth = 15;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 12, 15));

        Assert.Equal(
            new DateOnly(2026, 1, 15),
            result);
    }

    [Fact]
    public void Monthly_FebruaryTwentyNine_InLeapYear_ReturnsFebruary29()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2024, 1, 29),
            1);

        schedule.RecurrenceRule.DayOfMonth = 29;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2024, 1, 29));

        Assert.Equal(
            new DateOnly(2024, 2, 29),
            result);
    }


    [Fact]
    public void Monthly_FebruaryTwentyNine_InNonLeapYear()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 1, 29),
            1);

        schedule.RecurrenceRule.DayOfMonth = 29;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 1, 29));

        Assert.Equal(
            new DateOnly(2025, 2, 28),
            result);
    }
    
    [Fact]
    public void Monthly_TestMonthsWithout31Days()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 1, 31),
            1);

        schedule.RecurrenceRule.DayOfMonth = 31;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 1, 31));

        Assert.Equal(
            new DateOnly(2025, 2, 28),
            result);
    }


    [Fact]
    public void Monthly_FebruaryThirtyOne_IsHandled()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 1, 31),
            1);

        schedule.RecurrenceRule.DayOfMonth = 31;
        
        Assert.Equal(new DateOnly(2025, 2, 28), ScheduleService.GetNextOccurrence(schedule, new DateOnly(2025, 1, 31)));
    }
    
    [Fact]
    public void Monthly_FromFebruaryTwentyEightToMarch()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 2, 28),
            1);

        schedule.RecurrenceRule.DayOfMonth = 28;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 2, 28));

        Assert.Equal(
            new DateOnly(2025, 3, 28),
            result);
    }

    [Fact]
    public void Monthly_FirstMonday_ReturnsCorrectDateTest()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 7, 7),
            1);

        schedule.RecurrenceRule.Ordinal = Ordinal.First;
        schedule.RecurrenceRule.OrdinalType = OrdinalType.Monday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 7));

        Assert.Equal(
            new DateOnly(2025, 8, 4),
            result);
    }
    
    [Fact]
    public void Monthly_SecondTuesday_ReturnsCorrectDate()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 7, 8),
            1);

        schedule.RecurrenceRule.Ordinal = Ordinal.Second;
        schedule.RecurrenceRule.OrdinalType = OrdinalType.Tuesday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 8));

        Assert.Equal(
            new DateOnly(2025, 8, 12),
            result);
    }
    
    [Fact]
    public void Monthly_ThirdWednesday_ReturnsCorrectDate()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 7, 16),
            1);

        schedule.RecurrenceRule.Ordinal = Ordinal.Third;
        schedule.RecurrenceRule.OrdinalType = OrdinalType.Wednesday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 16));

        Assert.Equal(
            new DateOnly(2025, 8, 20),
            result);
    }


    [Fact]
    public void Monthly_FourthFriday_ReturnsCorrectDate()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 7, 25),
            1);

        schedule.RecurrenceRule.Ordinal = Ordinal.Fourth;
        schedule.RecurrenceRule.OrdinalType = OrdinalType.Friday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 25));

        Assert.Equal(
            new DateOnly(2025, 8, 22),
            result);
    }


    [Fact]
    public void Monthly_LastFriday_ReturnsCorrectDateTest()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 7, 25),
            1);

        schedule.RecurrenceRule.Ordinal = Ordinal.Last;
        schedule.RecurrenceRule.OrdinalType = OrdinalType.Friday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 25));

        Assert.Equal(
            new DateOnly(2025, 8, 29),
            result);
    }
    [Fact]
    public void Monthly_FirstWeekDay_ReturnsMondayToFridayOnly()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 7, 1),
            1);

        schedule.RecurrenceRule.Ordinal = Ordinal.First;
        schedule.RecurrenceRule.OrdinalType = OrdinalType.WeekDay;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 1));

        Assert.Equal(
            new DateOnly(2025, 8, 1),
            result);
    }


    [Fact]
    public void Monthly_LastWeekDay_ReturnsLastBusinessDay()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 7, 31),
            1);

        schedule.RecurrenceRule.Ordinal = Ordinal.Last;
        schedule.RecurrenceRule.OrdinalType = OrdinalType.WeekDay;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 31));

        Assert.Equal(
            new DateOnly(2025, 8, 29),
            result);
    }


    [Fact]
    public void Monthly_LastWeekendDay_ReturnsSaturdayOrSunday()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 7, 31),
            1);

        schedule.RecurrenceRule.Ordinal = Ordinal.Last;
        schedule.RecurrenceRule.OrdinalType = OrdinalType.WeekEndDay;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 31));

        Assert.Equal(
            new DateOnly(2025, 8, 31),
            result);
    }
    [Fact]
    public void Monthly_LastDay_FebruaryLeapYear()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2024, 1, 31),
            1);

        schedule.RecurrenceRule.Ordinal = Ordinal.Last;
        schedule.RecurrenceRule.OrdinalType = OrdinalType.Day;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2024, 1, 31));

        Assert.Equal(
            new DateOnly(2024, 2, 29),
            result);
    }


    [Fact]
    public void Monthly_LastDay_FebruaryNonLeapYear()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 1, 31),
            1);

        schedule.RecurrenceRule.Ordinal = Ordinal.Last;
        schedule.RecurrenceRule.OrdinalType = OrdinalType.Day;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 1, 31));

        Assert.Equal(
            new DateOnly(2025, 2, 28),
            result);
    }
    
    [Fact]
    public void Monthly_AfterScheduledDay_ReturnsNextMonth()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 7, 15),
            1);

        schedule.RecurrenceRule.DayOfMonth = 15;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 20));

        Assert.Equal(
            new DateOnly(2025, 8, 15),
            result);
    }
    
    [Fact]
    public void Monthly_EveryThreeMonths_AfterSeveralOccurrences_ReturnsNextQuarter()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 1, 15),
            3);

        schedule.RecurrenceRule.DayOfMonth = 15;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 10, 16));

        Assert.Equal(
            new DateOnly(2026, 1, 15),
            result);
    }
    
    [Fact]
    public void Monthly_Day31_April_ReturnsApril30()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 3, 31),
            1);

        schedule.RecurrenceRule.DayOfMonth = 31;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 3, 31));

        Assert.Equal(
            new DateOnly(2025, 4, 30),
            result);
    }
    
    [Fact]
    public void Monthly_Day31_June_ReturnsJune30()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 5, 31),
            1);

        schedule.RecurrenceRule.DayOfMonth = 31;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 5, 31));

        Assert.Equal(
            new DateOnly(2025, 6, 30),
            result);
    }
    
    [Fact]
    public void Monthly_FirstMonday_EveryThreeMonths_ReturnsCorrectMonth()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 1, 6),
            3);

        schedule.RecurrenceRule.Ordinal = Ordinal.First;
        schedule.RecurrenceRule.OrdinalType = OrdinalType.Monday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 1, 6));

        Assert.Equal(
            new DateOnly(2025, 4, 7),
            result);
    }
    
    [Fact]
    public void Monthly_FirstWeekday_WhenMonthStartsSaturday_ReturnsMonday()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 1, 1),
            1);

        schedule.RecurrenceRule.Ordinal = Ordinal.First;
        schedule.RecurrenceRule.OrdinalType = OrdinalType.WeekDay;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 1, 1));

        Assert.Equal(
            new DateOnly(2025, 2, 3),
            result);
    }
    
    [Fact]
    public void Monthly_LastWeekday_WhenMonthEndsSunday_ReturnsFriday()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 7, 31),
            1);

        schedule.RecurrenceRule.Ordinal = Ordinal.Last;
        schedule.RecurrenceRule.OrdinalType = OrdinalType.WeekDay;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 31));

        Assert.Equal(
            new DateOnly(2025, 8, 29),
            result);
    }
    
    [Fact]
    public void Monthly_LastWeekendDay_WhenMonthEndsFriday_ReturnsPreviousSunday()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 1, 26),
            1);

        schedule.RecurrenceRule.Ordinal = Ordinal.Last;
        schedule.RecurrenceRule.OrdinalType = OrdinalType.WeekEndDay;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 1, 26));

        Assert.Equal(
            new DateOnly(2025, 2, 23),
            result);
    }
    
    [Theory]
    [InlineData(Ordinal.First, OrdinalType.Monday, 2025, 8, 4)]
    [InlineData(Ordinal.Second, OrdinalType.Tuesday, 2025, 8, 12)]
    [InlineData(Ordinal.Third, OrdinalType.Wednesday, 2025, 8, 20)]
    [InlineData(Ordinal.Fourth, OrdinalType.Friday, 2025, 8, 22)]
    public void Monthly_OrdinalWeekday_ReturnsExpectedDate(
        Ordinal ordinal,
        OrdinalType ordinalType,
        int year,
        int month,
        int day)
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Monthly,
            new DateOnly(2025, 7, 1),
            1);

        schedule.RecurrenceRule.Ordinal = ordinal;
        schedule.RecurrenceRule.OrdinalType = ordinalType;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 1));

        Assert.Equal(new DateOnly(year, month, day), result);
    }
}