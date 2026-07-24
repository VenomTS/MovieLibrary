using App.APIResponses;
using App.Dialogs;
using App.Services.Interfaces;
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

        editRolesButton.Location = new Point(
            card.Width - editRolesButton.Width - 25,
            40);

        card.Resize += (s, e) =>
        {
            editRolesButton.Left = card.Width - editRolesButton.Width - 25;
        };

        editRolesButton.Click += async (s, e) =>
        {
            using var dialog = new EditAccountDialog(user, _roles);

            if (await dialog.ShowDialogAsync() != DialogResult.OK)
                return;

            var selectedRoleIds = dialog.SelectedRoleIds;

            var response = await _httpService.PutAsync<UpdateUserRoleRequest, EmptyResponse>($"auth/users/{user.Id}",
                new UpdateUserRoleRequest
                {
                    Roles = selectedRoleIds
                });
            
            Console.WriteLine(response.Status);

            await LoadUsers();
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

        userPanel.Controls.Add(card);
    }
}