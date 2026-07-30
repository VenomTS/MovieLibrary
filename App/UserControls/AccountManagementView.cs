using System.Net;
using App.Account;
using App.APIResponses;
using App.Dialogs;
using App.Services.Interfaces;
using DTO.InvoiceTemplates;
using DTO.Users;

namespace App.UserControls;

public partial class AccountManagementView : UserControl
{
    private FlowLayoutPanel userPanel;
    private Label titleLabel;

    private readonly IHttpService _httpService;

    private List<RoleResponse> _roles = new();

    public AccountManagementView(IHttpService httpService)
    {
        InitializeComponent();

        _httpService = httpService;

        SetupUI();

        Load += UserManagementView_Load;
    }

    private void SetupUI()
    {
        BackColor = Color.FromArgb(245, 246, 250);

        Panel headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = Color.White,
            Padding = new Padding(20)
        };

        titleLabel = new Label
        {
            Text = "User Management",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = Color.FromArgb(40, 40, 40),
            AutoSize = true,
            Dock = DockStyle.Left,
            TextAlign = ContentAlignment.MiddleLeft
        };

        headerPanel.Controls.Add(titleLabel);

        userPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(25),
            BackColor = Color.FromArgb(245, 246, 250)
        };

        userPanel.Resize += (s, e) =>
        {
            foreach (Control control in userPanel.Controls)
            {
                control.Width = userPanel.ClientSize.Width - 60;
            }
        };

        Controls.Add(userPanel);
        Controls.Add(headerPanel);
    }

    private async void UserManagementView_Load(object sender, EventArgs e)
    {
        await LoadUsers();
    }

    private async Task LoadUsers()
    {
        userPanel.Controls.Clear();

        var rolesResponse = await _httpService.GetAsync<List<RoleResponse>>("auth/roles");

        _roles = rolesResponse.Content ?? new List<RoleResponse>();

        var usersResponse = await _httpService.GetAsync<List<AppUserResponse>>("auth/users");
        var users = usersResponse.Content;

        if (users == null || users.Count == 0)
        {
            userPanel.Controls.Add(
                new Label
                {
                    Text = "No users available",
                    Font = new Font("Segoe UI", 12),
                    AutoSize = true,
                    Padding = new Padding(20)
                });

            return;
        }

        foreach (var user in users)
        {
            AddUserCard(user);
        }
    }

    private void AddUserCard(AppUserResponse user)
    {
        Panel card = new Panel
        {
            Height = 120,
            Width = userPanel.ClientSize.Width - 60,
            BackColor = Color.White,
            Margin = new Padding(0, 0, 0, 15),
            Padding = new Padding(20)
        };

        Label emailLabel = new Label
        {
            Text = user.Mail,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 30, 30),
            AutoSize = true,
            Location = new Point(20, 15)
        };

        string roles = user.Roles == null || user.Roles.Count == 0
            ? "None"
            : string.Join(", ", user.Roles);

        Label rolesLabel = new Label
        {
            Text = $"Roles: {roles}",
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.Gray,
            AutoSize = true,
            Location = new Point(20, 50)
        };

        Button editRolesButton = new Button
        {
            Text = "Edit Roles",
            Width = 120,
            Height = 38,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            Cursor = Cursors.Hand,
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };

        editRolesButton.FlatAppearance.BorderSize = 0;

        Button startSubscriptionButton = new Button
        {
            Text = "Start Subscription",
            Width = 150,
            Height = 38,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(46, 204, 113),
            ForeColor = Color.White,
            Cursor = Cursors.Hand,
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };

        startSubscriptionButton.FlatAppearance.BorderSize = 0;

        // Position the buttons next to each other
        startSubscriptionButton.Location = new Point(
            card.Width - startSubscriptionButton.Width - 25,
            40);

        editRolesButton.Location = new Point(
            startSubscriptionButton.Left - editRolesButton.Width - 10,
            40);

        card.Resize += (s, e) =>
        {
            startSubscriptionButton.Left =
                card.Width - startSubscriptionButton.Width - 25;

            editRolesButton.Left =
                startSubscriptionButton.Left - editRolesButton.Width - 10;
        };

        editRolesButton.Click += async (s, e) =>
        {
            using var dialog = new EditAccountDialog(user, _roles);

            if (await dialog.ShowDialogAsync() != DialogResult.OK)
                return;

            var selectedRoleIds = dialog.SelectedRoleIds;

            var response = await _httpService.PutAsync<UpdateUserRoleRequest, EmptyResponse>(
                $"auth/users/{user.Id}",
                new UpdateUserRoleRequest
                {
                    Roles = selectedRoleIds
                });

            await LoadUsers();
        };

        // TODO: Implement subscription functionality
        startSubscriptionButton.Click += async (s, e) =>
        {
            using var dialog = new EditInvoiceTemplateDialog(
                user,
                _httpService);

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            var template = dialog.Template;

            // API call goes here later.
            // Ovdje raditi gluposti sa njim...
            if (template == null)
                throw new Exception("Template is null?");

            var schedule = new CreateInvoiceTemplateScheduleRequest
            {
                StartDate = template.Schedule.StartDate,
                EndDate = template.Schedule.EndDate,
                Frequency = template.Schedule.Frequency,
                Interval = template.Schedule.Interval,
                DaysOfWeek = template.Schedule.DaysOfWeek,
                DayOfMonth = template.Schedule.DayOfMonth,
                Ordinal = template.Schedule.Ordinal,
                OrdinalType = template.Schedule.OrdinalType,
            };

            var invoiceTemplate = new CreateInvoiceTemplateRequest
            {
                UserId = user.Id,
                Price = template.Price,
                Description = template.Description,
                Schedule = schedule,
            };

            var response =
                await _httpService.PostAsync<CreateInvoiceTemplateRequest, InvoiceTemplateResponse>("invoicetemplates",
                    invoiceTemplate);

            if (response.Status != HttpStatusCode.OK)
                MessageBox.Show($"{response.Status}", "Error");
            else
                MessageBox.Show("Successfully created an invoice template", "Success");
        };

        card.MouseEnter += (s, e) =>
        {
            card.BackColor = Color.FromArgb(235, 245, 255);
        };

        card.MouseLeave += (s, e) =>
        {
            card.BackColor = Color.White;
        };

        card.Controls.Add(emailLabel);
        card.Controls.Add(rolesLabel);
        card.Controls.Add(editRolesButton);
        card.Controls.Add(startSubscriptionButton);

        userPanel.Controls.Add(card);
    }
}
