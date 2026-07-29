using API.Schedules;
using Models.Schedules.Rules;

namespace UnitTests;
public class WeeklySchedulerTests
{
    [Fact]
    public void Weekly_Monday_ReturnsNextMondayFirst()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            startDate: new DateOnly(2025, 7, 1),
            interval: 1);

        schedule.RecurrenceRule.DaysOfWeek =
            DaysOfWeek.Monday;


        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15)); // Tuesday

        Assert.Equal(
            new DateOnly(2025, 7, 21),
            result);
    }


    [Fact]
    public void Weekly_MondayAndWednesday_ReturnsWednesday()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            startDate: new DateOnly(2025, 7, 1),
            interval: 1);

        schedule.RecurrenceRule.DaysOfWeek =
            DaysOfWeek.Monday |
            DaysOfWeek.Wednesday;


        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15)); // Tuesday

        Assert.Equal(
            new DateOnly(2025, 7, 16),
            result);
    }


    [Fact]
    public void Weekly_IntervalTwoWeeks_SkipsWeek()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            startDate: new DateOnly(2025, 7, 1),
            interval: 2);

        schedule.RecurrenceRule.DaysOfWeek =
            DaysOfWeek.Monday;


        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 14));

        Assert.Equal(
            new DateOnly(2025, 7, 28),
            result);
    }


    [Fact]
    public void Weekly_Sunday_ReturnsNextSundayFirst()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            startDate: new DateOnly(2025, 7, 1),
            interval: 1);

        schedule.RecurrenceRule.DaysOfWeek =
            DaysOfWeek.Sunday;


        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15));

        Assert.Equal(
            new DateOnly(2025, 7, 20),
            result);
    }
    
    [Fact]
    public void Weekly_Monday_ReturnsNextMonday()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            new DateOnly(2025, 7, 1),
            1);

        schedule.RecurrenceRule.DaysOfWeek = DaysOfWeek.Monday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15)); // Tuesday

        Assert.Equal(
            new DateOnly(2025, 7, 21),
            result);
    }


    [Fact]
    public void Weekly_TodayIsExecutionDay_ReturnsNextOccurrenceNotToday()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            new DateOnly(2025, 7, 1),
            1);

        schedule.RecurrenceRule.DaysOfWeek = DaysOfWeek.Monday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 14)); // Monday

        Assert.Equal(
            new DateOnly(2025, 7, 21),
            result);
    }


    [Fact]
    public void Weekly_Wednesday_ReturnsNextWednesday()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            new DateOnly(2025, 7, 1),
            1);

        schedule.RecurrenceRule.DaysOfWeek = DaysOfWeek.Wednesday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15)); // Tuesday

        Assert.Equal(
            new DateOnly(2025, 7, 16),
            result);
    }


    [Fact]
    public void Weekly_Friday_ReturnsSameWeekFriday()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            new DateOnly(2025, 7, 1),
            1);

        schedule.RecurrenceRule.DaysOfWeek = DaysOfWeek.Friday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 14)); // Monday

        Assert.Equal(
            new DateOnly(2025, 7, 18),
            result);
    }


    [Fact]
    public void Weekly_MondayAndWednesday_ReturnsWednesdayWhenMondayPassed()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            new DateOnly(2025, 7, 1),
            1);

        schedule.RecurrenceRule.DaysOfWeek =
            DaysOfWeek.Monday |
            DaysOfWeek.Wednesday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15)); // Tuesday

        Assert.Equal(
            new DateOnly(2025, 7, 16),
            result);
    }


    [Fact]
    public void Weekly_MondayAndWednesday_ReturnsMondayBeforeWednesday()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            new DateOnly(2025, 7, 1),
            1);

        schedule.RecurrenceRule.DaysOfWeek =
            DaysOfWeek.Monday |
            DaysOfWeek.Wednesday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 13)); // Sunday

        Assert.Equal(
            new DateOnly(2025, 7, 14),
            result);
    }
    
    [Fact]
    public void Weekly_MondayAndWednesday_ReturnsMondayBeforeWednesdayTwoGap()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            new DateOnly(2025, 7, 1),
            2);

        schedule.RecurrenceRule.DaysOfWeek =
            DaysOfWeek.Monday |
            DaysOfWeek.Wednesday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 13)); // Sunday

        Assert.Equal(
            new DateOnly(2025, 7, 21),
            result);
    }


    [Fact]
    public void Weekly_Sunday_ReturnsNextSunday()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            new DateOnly(2025, 7, 1),
            1);

        schedule.RecurrenceRule.DaysOfWeek = DaysOfWeek.Sunday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15));

        Assert.Equal(
            new DateOnly(2025, 7, 20),
            result);
    }


    [Fact]
    public void Weekly_MondayAfterSundayExecution_ReturnsNextWeek()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            new DateOnly(2025, 7, 1),
            1);

        schedule.RecurrenceRule.DaysOfWeek =
            DaysOfWeek.Sunday |
            DaysOfWeek.Monday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 13)); // Sunday

        Assert.Equal(
            new DateOnly(2025, 7, 14),
            result);
    }


    [Fact]
    public void Weekly_EveryTwoWeeks_SkipsOneWeek()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            new DateOnly(2025, 7, 1),
            2);

        schedule.RecurrenceRule.DaysOfWeek = DaysOfWeek.Monday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 14));

        Assert.Equal(
            new DateOnly(2025, 7, 28),
            result);
    }


    [Fact]
    public void Weekly_EveryFourWeeks_Works()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            new DateOnly(2025, 7, 1),
            4);

        schedule.RecurrenceRule.DaysOfWeek = DaysOfWeek.Monday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 14));

        Assert.Equal(
            new DateOnly(2025, 8, 11),
            result);
    }


    [Fact]
    public void Weekly_CrossesMonthBoundary()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            new DateOnly(2025, 7, 1),
            1);

        schedule.RecurrenceRule.DaysOfWeek = DaysOfWeek.Monday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 27)); // Sunday

        Assert.Equal(
            new DateOnly(2025, 7, 28),
            result);
    }


    [Fact]
    public void Weekly_CrossesYearBoundary()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            new DateOnly(2025, 12, 1),
            1);

        schedule.RecurrenceRule.DaysOfWeek = DaysOfWeek.Monday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 12, 29));

        Assert.Equal(
            new DateOnly(2026, 1, 5),
            result);
    }


    [Fact]
    public void Weekly_CrossesLeapDay()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            new DateOnly(2024, 2, 1),
            1);

        schedule.RecurrenceRule.DaysOfWeek = DaysOfWeek.Monday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2024, 2, 25));

        Assert.Equal(
            new DateOnly(2024, 2, 26),
            result);
    }


    [Fact]
    public void Weekly_AfterLeapDay_ReturnsCorrectWeekday()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            new DateOnly(2024, 2, 1),
            1);

        schedule.RecurrenceRule.DaysOfWeek = DaysOfWeek.Monday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2024, 2, 29));

        Assert.Equal(
            new DateOnly(2024, 3, 4),
            result);
    }


    [Theory]
    [InlineData(DaysOfWeek.Monday)]
    [InlineData(DaysOfWeek.Tuesday)]
    [InlineData(DaysOfWeek.Wednesday)]
    [InlineData(DaysOfWeek.Thursday)]
    [InlineData(DaysOfWeek.Friday)]
    [InlineData(DaysOfWeek.Saturday)]
    [InlineData(DaysOfWeek.Sunday)]
    public void Weekly_AllWeekdays_ReturnNextSelectedDay(DaysOfWeek day)
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            new DateOnly(2025, 7, 1),
            1);

        schedule.RecurrenceRule.DaysOfWeek = day;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 10));

        Assert.True(result > new DateOnly(2025, 7, 10));
    }


    [Fact]
    public void Weekly_AllDays_ReturnsTomorrow()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            new DateOnly(2025, 7, 1),
            1);

        schedule.RecurrenceRule.DaysOfWeek =
            DaysOfWeek.Monday |
            DaysOfWeek.Tuesday |
            DaysOfWeek.Wednesday |
            DaysOfWeek.Thursday |
            DaysOfWeek.Friday |
            DaysOfWeek.Saturday |
            DaysOfWeek.Sunday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 7, 15));

        Assert.Equal(
            new DateOnly(2025, 7, 16),
            result);
    }


    [Fact]
    public void Weekly_LargeInterval_Works()
    {
        var schedule = Shared.CreateSchedule(
            Frequency.Weekly,
            new DateOnly(2025, 1, 1),
            52);

        schedule.RecurrenceRule.DaysOfWeek = DaysOfWeek.Monday;

        var result = ScheduleService.GetNextOccurrence(
            schedule,
            new DateOnly(2025, 1, 6));

        Assert.Equal(
            new DateOnly(2026, 1, 5),
            result);
    }
}