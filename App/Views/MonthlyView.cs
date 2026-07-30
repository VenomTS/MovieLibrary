
using DTO.Schedules;
using Models.Schedules.Rules;

namespace App.Views;

public partial class MonthlyView : UserControl
{
    private RadioButton dayOfMonthRadioButton;
    private RadioButton ordinalRadioButton;

    private NumericUpDown dayOfMonthInput;

    private NumericUpDown dayIntervalInput;
    private NumericUpDown ordinalIntervalInput;

    private ComboBox ordinalComboBox;
    private ComboBox ordinalTypeComboBox;

    private readonly ScheduleResponse _schedule;

    public MonthlyView(ScheduleResponse schedule)
    {
        _schedule = schedule;

        SetupUI();
        LoadData();
    }

    private void SetupUI()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.White;

        // ----------------------------------------------------
        // Day of month
        // ----------------------------------------------------

        dayOfMonthRadioButton = new RadioButton
        {
            Text = "Day",
            AutoSize = true,
            Location = new Point(10, 15),
            Font = new Font("Segoe UI", 10)
        };

        dayOfMonthInput = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 31,
            Width = 55,
            Location = new Point(65, 13),
            Font = new Font("Segoe UI", 10)
        };

        Label dayOfEveryLabel = new Label
        {
            Text = "of every",
            AutoSize = true,
            Location = new Point(130, 15),
            Font = new Font("Segoe UI", 10)
        };

        dayIntervalInput = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 9999,
            Width = 55,
            Location = new Point(195, 13),
            Font = new Font("Segoe UI", 10)
        };

        Label dayMonthsLabel = new Label
        {
            Text = "months",
            AutoSize = true,
            Location = new Point(260, 15),
            Font = new Font("Segoe UI", 10)
        };

        // ----------------------------------------------------
        // Ordinal
        // ----------------------------------------------------

        ordinalRadioButton = new RadioButton
        {
            Text = "The",
            AutoSize = true,
            Location = new Point(10, 60),
            Font = new Font("Segoe UI", 10)
        };

        ordinalComboBox = new ComboBox
        {
            Width = 90,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(60, 57),
            Font = new Font("Segoe UI", 10)
        };

        ordinalComboBox.Items.AddRange(
            Enum.GetNames<Ordinal>());

        ordinalTypeComboBox = new ComboBox
        {
            Width = 110,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(160, 57),
            Font = new Font("Segoe UI", 10)
        };

        ordinalTypeComboBox.Items.AddRange(
            Enum.GetNames<OrdinalType>());

        Label ordinalOfEveryLabel = new Label
        {
            Text = "of every",
            AutoSize = true,
            Location = new Point(280, 60),
            Font = new Font("Segoe UI", 10)
        };

        ordinalIntervalInput = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 9999,
            Width = 55,
            Location = new Point(340, 58),
            Font = new Font("Segoe UI", 10)
        };

        Label ordinalMonthsLabel = new Label
        {
            Text = "months",
            AutoSize = true,
            Location = new Point(405, 60),
            Font = new Font("Segoe UI", 10)
        };

        // ----------------------------------------------------
        // Controls
        // ----------------------------------------------------

        Controls.Add(dayOfMonthRadioButton);
        Controls.Add(dayOfMonthInput);
        Controls.Add(dayOfEveryLabel);
        Controls.Add(dayIntervalInput);
        Controls.Add(dayMonthsLabel);

        Controls.Add(ordinalRadioButton);
        Controls.Add(ordinalComboBox);
        Controls.Add(ordinalTypeComboBox);
        Controls.Add(ordinalOfEveryLabel);
        Controls.Add(ordinalIntervalInput);
        Controls.Add(ordinalMonthsLabel);

        // Keep both interval inputs synchronized.
        dayIntervalInput.ValueChanged += (s, e) =>
        {
            if (ordinalIntervalInput.Value != dayIntervalInput.Value)
            {
                ordinalIntervalInput.Value =
                    dayIntervalInput.Value;
            }
        };

        ordinalIntervalInput.ValueChanged += (s, e) =>
        {
            if (dayIntervalInput.Value != ordinalIntervalInput.Value)
            {
                dayIntervalInput.Value =
                    ordinalIntervalInput.Value;
            }
        };

        dayOfMonthRadioButton.CheckedChanged += (s, e) =>
        {
            UpdateEnabledState();
        };

        ordinalRadioButton.CheckedChanged += (s, e) =>
        {
            UpdateEnabledState();
        };
    }

    private void LoadData()
    {
        int interval = Math.Max(1, _schedule.Interval);

        dayIntervalInput.Value = interval;
        ordinalIntervalInput.Value = interval;

        if (_schedule.DayOfMonth.HasValue)
        {
            dayOfMonthRadioButton.Checked = true;

            dayOfMonthInput.Value = Math.Clamp(
                _schedule.DayOfMonth.Value,
                1,
                31);
        }
        else
        {
            ordinalRadioButton.Checked = true;
        }

        if (_schedule.Ordinal.HasValue)
        {
            ordinalComboBox.SelectedItem =
                _schedule.Ordinal.Value.ToString();
        }

        if (_schedule.OrdinalType.HasValue)
        {
            ordinalTypeComboBox.SelectedItem =
                _schedule.OrdinalType.Value.ToString();
        }

        UpdateEnabledState();
    }

    private void UpdateEnabledState()
    {
        bool dayMode = dayOfMonthRadioButton.Checked;

        dayOfMonthInput.Enabled = dayMode;
        dayIntervalInput.Enabled = dayMode;

        ordinalComboBox.Enabled = !dayMode;
        ordinalTypeComboBox.Enabled = !dayMode;
        ordinalIntervalInput.Enabled = !dayMode;
    }

    public ScheduleResponse GetSchedule()
    {
        var schedule = new ScheduleResponse
        {
            Id = _schedule.Id,
            StartDate = _schedule.StartDate,
            EndDate = _schedule.EndDate,
            NextOccurrence = _schedule.NextOccurrence,
            Frequency = Frequency.Monthly
        };

        if (dayOfMonthRadioButton.Checked)
        {
            schedule.Interval = (int)dayIntervalInput.Value;

            schedule.DayOfMonth =
                (int)dayOfMonthInput.Value;

            schedule.Ordinal = null;
            schedule.OrdinalType = null;
        }
        else
        {
            schedule.Interval =
                (int)ordinalIntervalInput.Value;

            schedule.DayOfMonth = null;

            if (ordinalComboBox.SelectedItem != null)
            {
                schedule.Ordinal =
                    Enum.Parse<Ordinal>(
                        ordinalComboBox.SelectedItem.ToString()!);
            }

            if (ordinalTypeComboBox.SelectedItem != null)
            {
                schedule.OrdinalType =
                    Enum.Parse<OrdinalType>(
                        ordinalTypeComboBox.SelectedItem.ToString()!);
            }
        }

        return schedule;
    }
}
