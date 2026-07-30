using DTO.Schedules;
using Models.Schedules.Rules;

namespace App.Views;

public partial class WeeklyView : UserControl
{
    private NumericUpDown intervalInput;

    private readonly Dictionary<DaysOfWeek, CheckBox> dayCheckboxes = new();

    private readonly ScheduleResponse _schedule;

    public WeeklyView(ScheduleResponse schedule)
    {
        _schedule = schedule;

        SetupUI();
        LoadData();
    }

    private void SetupUI()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.White;

        Label recurLabel = new Label
        {
            Text = "Recur every",
            AutoSize = true,
            Location = new Point(10, 15),
            Font = new Font("Segoe UI", 10)
        };

        intervalInput = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 9999,
            Width = 70,
            Location = new Point(95, 13),
            Font = new Font("Segoe UI", 10)
        };

        Label weeksLabel = new Label
        {
            Text = "weeks on:",
            AutoSize = true,
            Location = new Point(170, 15),
            Font = new Font("Segoe UI", 10)
        };

        Controls.Add(recurLabel);
        Controls.Add(intervalInput);
        Controls.Add(weeksLabel);

        DaysOfWeek[] days =
        {
            DaysOfWeek.Monday,
            DaysOfWeek.Tuesday,
            DaysOfWeek.Wednesday,
            DaysOfWeek.Thursday,
            DaysOfWeek.Friday,
            DaysOfWeek.Saturday,
            DaysOfWeek.Sunday
        };

        string[] names =
        {
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday",
            "Sunday"
        };

        for (int i = 0; i < days.Length; i++)
        {
            CheckBox checkbox = new CheckBox
            {
                Text = names[i],
                AutoSize = true,
                Location = new Point(
                    10 + (i % 2) * 180,
                    55 + (i / 2) * 35),
                Font = new Font("Segoe UI", 10)
            };

            dayCheckboxes.Add(days[i], checkbox);
            Controls.Add(checkbox);
        }
    }

    private void LoadData()
    {
        intervalInput.Value = Math.Max(1, _schedule.Interval);

        DaysOfWeek selectedDays =
            _schedule.DaysOfWeek ?? DaysOfWeek.Monday;

        foreach (var pair in dayCheckboxes)
        {
            pair.Value.Checked =
                selectedDays.HasFlag(pair.Key);
        }
    }

    public ScheduleResponse GetSchedule()
    {
        DaysOfWeek selectedDays = 0;

        foreach (var pair in dayCheckboxes)
        {
            if (pair.Value.Checked)
                selectedDays |= pair.Key;
        }

        return new ScheduleResponse
        {
            Id = _schedule.Id,
            StartDate = _schedule.StartDate,
            EndDate = _schedule.EndDate,
            NextOccurrence = _schedule.NextOccurrence,
            Frequency = Frequency.Weekly,
            Interval = (int)intervalInput.Value,
            DaysOfWeek = selectedDays
        };
    }
}