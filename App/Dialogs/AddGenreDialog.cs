using System.Net;
using App.Services.Interfaces;
using DTO.Genres;

namespace App.Dialogs;

public partial class AddGenreDialog : Form
{
    private readonly IHttpService _httpService;

    private TextBox txtGenreName;
    private Button btnSave;
    private Button btnCancel;

    public string GenreName => txtGenreName.Text.Trim();

    public AddGenreDialog(IHttpService httpService)
    {
        _httpService = httpService;

        InitializeUI();
    }

    private void InitializeUI()
    {
        Text = "Add Genre";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(400, 170);

        Label lblGenre = new Label
        {
            Text = "Genre Name",
            AutoSize = true,
            Location = new Point(20, 20),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        txtGenreName = new TextBox
        {
            Location = new Point(20, 45),
            Width = 350,
            Font = new Font("Segoe UI", 10)
        };

        btnSave = new Button
        {
            Text = "Save",
            Width = 100,
            Height = 35,
            Location = new Point(160, 105),
            BackColor = Color.FromArgb(46, 204, 113),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };

        btnSave.FlatAppearance.BorderSize = 0;

        btnCancel = new Button
        {
            Text = "Cancel",
            Width = 100,
            Height = 35,
            Location = new Point(270, 105),
            BackColor = Color.LightGray,
            FlatStyle = FlatStyle.Flat
        };

        btnSave.Click += SaveClicked;
        btnCancel.Click += (s, e) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        Controls.Add(lblGenre);
        Controls.Add(txtGenreName);
        Controls.Add(btnSave);
        Controls.Add(btnCancel);

        AcceptButton = btnSave;
        CancelButton = btnCancel;
    }

    private async void SaveClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtGenreName.Text))
        {
            MessageBox.Show(
                "Genre name is required.",
                "Validation",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return;
        }

        var request = new CreateGenreRequest
        {
            Name = txtGenreName.Text.Trim()
        };

        var response = await _httpService.PostAsync<CreateGenreRequest, GenreResponse>("genres", request);

        if (response.Status == HttpStatusCode.Created)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            MessageBox.Show(
                "Failed to create genre.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}