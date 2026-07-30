using DTO.Schedules;
using Models.Schedules.Rules;

namespace App.Views;

public partial class DailyView : UserControl
{
    private RadioButton everyDaysRadioButton;
    private RadioButton weekdayRadioButton;

    private NumericUpDown intervalInput;

    private readonly ScheduleResponse _schedule;

    public DailyView(ScheduleResponse schedule)
    {
        _schedule = schedule;

        SetupUI();
        LoadData();
    }

    private void SetupUI()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.White;

        everyDaysRadioButton = new RadioButton
        {
            Text = "Every",
            AutoSize = true,
            Location = new Point(10, 15),
            Font = new Font("Segoe UI", 10)
        };

        intervalInput = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 9999,
            Width = 70,
            Location = new Point(75, 13),
            Font = new Font("Segoe UI", 10)
        };

        Label daysLabel = new Label
        {
            Text = "days",
            AutoSize = true,
            Location = new Point(150, 17),
            Font = new Font("Segoe UI", 10)
        };

        weekdayRadioButton = new RadioButton
        {
            Text = "Every weekday",
            AutoSize = true,
            Location = new Point(10, 55),
            Font = new Font("Segoe UI", 10)
        };

        Controls.Add(everyDaysRadioButton);
        Controls.Add(intervalInput);
        Controls.Add(daysLabel);
        Controls.Add(weekdayRadioButton);
    }

    private void LoadData()
    {
        intervalInput.Value = Math.Max(1, _schedule.Interval);

        // There is no explicit "weekday" property in ScheduleResponse.
        // We use the existing DaysOfWeek value to determine this option.
        //
        // A null DaysOfWeek means normal daily recurrence.
        // Monday-Friday means "Every weekday".
        //
        // If your backend represents this differently, adjust this logic.

        bool isWeekday =
            _schedule.DaysOfWeek.HasValue &&
            _schedule.DaysOfWeek.Value ==
            (DaysOfWeek.Monday |
             DaysOfWeek.Tuesday |
             DaysOfWeek.Wednesday |
             DaysOfWeek.Thursday |
             DaysOfWeek.Friday);

        weekdayRadioButton.Checked = isWeekday;
        everyDaysRadioButton.Checked = !isWeekday;

        intervalInput.Enabled = everyDaysRadioButton.Checked;

        everyDaysRadioButton.CheckedChanged += (s, e) =>
        {
            intervalInput.Enabled = everyDaysRadioButton.Checked;
        };
    }

    public ScheduleResponse GetSchedule()
    {
        var schedule = new ScheduleResponse
        {
            Id = _schedule.Id,
            StartDate = _schedule.StartDate,
            EndDate = _schedule.EndDate,
            NextOccurrence = _schedule.NextOccurrence,
            Frequency = Frequency.Daily
        };

        if (weekdayRadioButton.Checked)
        {
            schedule.Interval = 1;

            schedule.DaysOfWeek =
                DaysOfWeek.Monday |
                DaysOfWeek.Tuesday |
                DaysOfWeek.Wednesday |
                DaysOfWeek.Thursday |
                DaysOfWeek.Friday;
        }
        else
        {
            schedule.Interval = (int)intervalInput.Value;
            schedule.DaysOfWeek = null;
        }

        return schedule;
    }
}