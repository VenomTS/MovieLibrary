using DTO;
using DTO.Users;

namespace App.Dialogs;

public partial class EditAccountDialog : Form
{
    private readonly AppUserResponse _user;
    private readonly List<RoleResponse> _availableRoles;

    private FlowLayoutPanel rolePanel;
    private Button saveButton;
    private Button cancelButton;

    private readonly Dictionary<Guid, CheckBox> _roleCheckboxes = new();

    public List<Guid> SelectedRoleIds =>
        _roleCheckboxes
            .Where(x => x.Value.Checked)
            .Select(x => x.Key)
            .ToList();

    public EditAccountDialog(
        AppUserResponse user,
        List<RoleResponse> availableRoles)
    {
        InitializeComponent();

        _user = user;
        _availableRoles = availableRoles;

        SetupUI();
        LoadRoles();
    }

    private void SetupUI()
    {
        Text = $"Edit Roles - {_user.Mail}";
        Width = 400;
        Height = 450;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        BackColor = Color.FromArgb(245, 246, 250);

        rolePanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 300,
            AutoScroll = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(20),
            BackColor = Color.White
        };

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

        saveButton.Location = new Point(140, 15);
        cancelButton.Location = new Point(250, 15);

        buttonPanel.Controls.Add(saveButton);
        buttonPanel.Controls.Add(cancelButton);

        Controls.Add(rolePanel);
        Controls.Add(buttonPanel);
    }

    private void LoadRoles()
    {
        foreach (var role in _availableRoles)
        {
            CheckBox checkbox = new CheckBox
            {
                Text = role.Name,
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(5),
                Checked = _user.Roles.Contains(role.Name)
            };

            _roleCheckboxes.Add(role.Id, checkbox);

            rolePanel.Controls.Add(checkbox);
        }
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
        /*
            SelectedRoleIds contains the roles the user should have.

            Example:
            
            var roleIds = SelectedRoleIds;

            Implement API call here.
        */

        DialogResult = DialogResult.OK;
        Close();
    }
}