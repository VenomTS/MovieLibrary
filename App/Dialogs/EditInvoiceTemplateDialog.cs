using App.Account;
using DTO;
using DTO.Users;
using App.Services.Interfaces;
using App.Views;
using DTO.InvoiceTemplates;
using DTO.Schedules;
using Models.Schedules.Rules;

namespace App.Dialogs;

public partial class EditInvoiceTemplateDialog : Form
{
    private readonly IHttpService _httpService;
    private readonly AccountManager _accountManager;

    private TextBox priceTextBox;
    private TextBox descriptionTextBox;

    private ComboBox frequencyComboBox;
    private Panel schedulePanel;

    private Button saveButton;
    private Button cancelButton;

    private DailyView? dailyScheduleView;
    private WeeklyView? weeklyScheduleView;
    private MonthlyView? monthlyScheduleView;

    private InvoiceTemplateDetailedResponse? _template;

    private bool _loadingFrequency;

    public EditInvoiceTemplateDialog(
        AccountManager accountManager,
        IHttpService httpService)
    {
        InitializeComponent();

        _accountManager = accountManager;
        _httpService = httpService;

        SetupUI();

        Load += EditInvoiceTemplateDialog_Load;
    }

    public InvoiceTemplateDetailedResponse? Template => _template;

    private async void EditInvoiceTemplateDialog_Load(
        object? sender,
        EventArgs e)
    {
        await LoadInvoiceTemplate();
    }

    private void SetupUI()
    {
        Text = "Edit Invoice Template";
        Width = 600;
        Height = 700;

        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        BackColor = Color.FromArgb(245, 246, 250);

        Panel contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            AutoScroll = true
        };

        // ----------------------------------------------------
        // Price
        // ----------------------------------------------------

        Label priceLabel = new Label
        {
            Text = "Price",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 20)
        };

        priceTextBox = new TextBox
        {
            Width = 200,
            Location = new Point(20, 45),
            Font = new Font("Segoe UI", 10)
        };

        // ----------------------------------------------------
        // Description
        // ----------------------------------------------------

        Label descriptionLabel = new Label
        {
            Text = "Description",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 85)
        };

        descriptionTextBox = new TextBox
        {
            Width = 520,
            Height = 70,
            Location = new Point(20, 110),
            Font = new Font("Segoe UI", 10),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical
        };

        // ----------------------------------------------------
        // Frequency
        // ----------------------------------------------------

        Label frequencyLabel = new Label
        {
            Text = "Frequency",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 200)
        };

        frequencyComboBox = new ComboBox
        {
            Width = 200,
            Height = 30,
            Location = new Point(20, 225),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 10)
        };

        frequencyComboBox.Items.AddRange(
            new object[]
            {
                Frequency.Daily,
                Frequency.Weekly,
                Frequency.Monthly
            });

        frequencyComboBox.SelectedIndexChanged += FrequencyComboBox_SelectedIndexChanged;

        // ----------------------------------------------------
        // Schedule
        // ----------------------------------------------------

        Label scheduleLabel = new Label
        {
            Text = "Schedule",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 270)
        };

        schedulePanel = new Panel
        {
            Location = new Point(20, 300),
            Width = 520,
            Height = 270,
            BackColor = Color.White,
            Padding = new Padding(15)
        };

        contentPanel.Controls.Add(priceLabel);
        contentPanel.Controls.Add(priceTextBox);

        contentPanel.Controls.Add(descriptionLabel);
        contentPanel.Controls.Add(descriptionTextBox);

        contentPanel.Controls.Add(frequencyLabel);
        contentPanel.Controls.Add(frequencyComboBox);

        contentPanel.Controls.Add(scheduleLabel);
        contentPanel.Controls.Add(schedulePanel);

        // ----------------------------------------------------
        // Buttons
        // ----------------------------------------------------

        saveButton = new Button
        {
            Text = "Save",
            Width = 100,
            Height = 35,
            BackColor = Color.FromArgb(46, 204, 113),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        saveButton.FlatAppearance.BorderSize = 0;

        cancelButton = new Button
        {
            Text = "Cancel",
            Width = 100,
            Height = 35,
            BackColor = Color.FromArgb(149, 165, 166),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        cancelButton.FlatAppearance.BorderSize = 0;

        saveButton.Click += SaveButton_Click;

        cancelButton.Click += (s, e) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        Panel buttonPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 70,
            Padding = new Padding(20)
        };

        saveButton.Location = new Point(340, 15);
        cancelButton.Location = new Point(450, 15);

        buttonPanel.Controls.Add(saveButton);
        buttonPanel.Controls.Add(cancelButton);

        Controls.Add(contentPanel);
        Controls.Add(buttonPanel);
    }

    private async Task LoadInvoiceTemplate()
    {
        try
        {
            saveButton.Enabled = false;
            frequencyComboBox.Enabled = false;

            var response =
                await _httpService.GetAsync<InvoiceTemplateDetailedResponse>(
                    $"invoicetemplates/{_accountManager.User!.Id}");

            _template = response.Content;

            if (_template == null)
            {
                MessageBox.Show(
                    "Invoice template could not be found.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                DialogResult = DialogResult.Cancel;
                Close();

                return;
            }

            FillTemplateData();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Failed to load invoice template.\n\n{ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            DialogResult = DialogResult.Cancel;
            Close();
        }
        finally
        {
            saveButton.Enabled = true;
            frequencyComboBox.Enabled = true;
        }
    }

    private void FillTemplateData()
    {
        if (_template == null)
            return;

        priceTextBox.Text = _template.Price.ToString("0.##");
        descriptionTextBox.Text = _template.Description;

        if (_template.Schedule == null)
            return;

        _loadingFrequency = true;

        frequencyComboBox.SelectedItem =
            _template.Schedule.Frequency;

        _loadingFrequency = false;

        ShowScheduleView(_template.Schedule.Frequency);
    }

    private void FrequencyComboBox_SelectedIndexChanged(
        object? sender,
        EventArgs e)
    {
        if (_loadingFrequency)
            return;

        if (_template?.Schedule == null)
            return;

        if (frequencyComboBox.SelectedItem is not Frequency frequency)
            return;

        ShowScheduleView(frequency);
    }

    private void ShowScheduleView(Frequency frequency)
    {
        if (_template == null)
            return;

        schedulePanel.Controls.Clear();

        dailyScheduleView = null;
        weeklyScheduleView = null;
        monthlyScheduleView = null;

        // Preserve the existing schedule data where possible.
        ScheduleResponse schedule = CreateScheduleForFrequency(
            frequency);

        switch (frequency)
        {
            case Frequency.Daily:
                dailyScheduleView = new DailyView(schedule);
                schedulePanel.Controls.Add(dailyScheduleView);
                break;

            case Frequency.Weekly:
                weeklyScheduleView = new WeeklyView(schedule);
                schedulePanel.Controls.Add(weeklyScheduleView);
                break;

            case Frequency.Monthly:
                monthlyScheduleView = new MonthlyView(schedule);
                schedulePanel.Controls.Add(monthlyScheduleView);
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(frequency),
                    frequency,
                    "Unsupported frequency.");
        }
    }

    private ScheduleResponse CreateScheduleForFrequency(
        Frequency frequency)
    {
        if (_template?.Schedule == null)
        {
            return new ScheduleResponse
            {
                Frequency = frequency,
                Interval = 1,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                NextOccurrence = DateOnly.FromDateTime(DateTime.Today)
            };
        }

        var current = _template.Schedule;

        // Preserve all values that are common between frequencies.
        var schedule = new ScheduleResponse
        {
            Id = current.Id,
            StartDate = current.StartDate,
            EndDate = current.EndDate,
            NextOccurrence = current.NextOccurrence,

            Frequency = frequency,

            Interval = Math.Max(1, current.Interval)
        };

        switch (frequency)
        {
            case Frequency.Daily:

                // Daily does not use monthly/weekly values.
                schedule.DaysOfWeek = null;
                schedule.DayOfMonth = null;
                schedule.Ordinal = null;
                schedule.OrdinalType = null;

                break;

            case Frequency.Weekly:

                // Preserve existing weekly days if available.
                schedule.DaysOfWeek =
                    current.DaysOfWeek
                    ?? (
                        DaysOfWeek.Monday |
                        DaysOfWeek.Tuesday |
                        DaysOfWeek.Wednesday |
                        DaysOfWeek.Thursday |
                        DaysOfWeek.Friday
                    );

                schedule.DayOfMonth = null;
                schedule.Ordinal = null;
                schedule.OrdinalType = null;

                break;

            case Frequency.Monthly:

                // Preserve existing monthly configuration if available.
                if (current.DayOfMonth.HasValue)
                {
                    schedule.DayOfMonth =
                        current.DayOfMonth.Value;

                    schedule.Ordinal = null;
                    schedule.OrdinalType = null;
                }
                else
                {
                    schedule.DayOfMonth = null;

                    schedule.Ordinal =
                        current.Ordinal ?? Ordinal.First;

                    schedule.OrdinalType =
                        current.OrdinalType ?? OrdinalType.Monday;
                }

                schedule.DaysOfWeek = null;

                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(frequency),
                    frequency,
                    "Unsupported frequency.");
        }

        return schedule;
    }

    private void SaveButton_Click(
        object? sender,
        EventArgs e)
    {
        if (_template == null)
            return;

        if (!decimal.TryParse(
                priceTextBox.Text,
                out decimal price))
        {
            MessageBox.Show(
                "Please enter a valid price.",
                "Invalid Price",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            priceTextBox.Focus();
            return;
        }

        if (frequencyComboBox.SelectedItem is not Frequency frequency)
        {
            MessageBox.Show(
                "Please select a frequency.",
                "Invalid Frequency",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return;
        }

        _template.Price = price;
        _template.Description = descriptionTextBox.Text;

        if (dailyScheduleView != null)
        {
            _template.Schedule =
                dailyScheduleView.GetSchedule();
        }
        else if (weeklyScheduleView != null)
        {
            _template.Schedule =
                weeklyScheduleView.GetSchedule();
        }
        else if (monthlyScheduleView != null)
        {
            _template.Schedule =
                monthlyScheduleView.GetSchedule();
        }

        if (_template.Schedule != null)
        {
            _template.Schedule.Frequency = frequency;
        }

        /*
            Implement API call here later.

            _httpService.PutAsync(...)

            Template now contains:
                - Price
                - Description
                - Schedule
                - Selected Frequency
        */

        DialogResult = DialogResult.OK;
        Close();
    }
}