using App.Account;
using App.Services.Interfaces;
using DTO.Rentals;
using System.Net;

namespace App.UserControls;

public partial class MyRentalsView : UserControl
{
    private readonly IHttpService _httpService;
    private readonly AccountManager _accountManager;

    private FlowLayoutPanel rentalPanel;
    private Button refreshButton;
    private Label titleLabel;

    public MyRentalsView(
        IHttpService httpService,
        AccountManager accountManager)
    {
        InitializeComponent();

        _httpService = httpService;
        _accountManager = accountManager;

        SetupUI();

        Load += MyRentalsView_Load;
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
            Text = "My Rentals",
            Font = new Font(
                "Segoe UI",
                20,
                FontStyle.Bold),
            ForeColor = Color.FromArgb(40, 40, 40),
            AutoSize = true,
            Dock = DockStyle.Left,
            TextAlign = ContentAlignment.MiddleLeft
        };

        refreshButton = new Button
        {
            Text = "Refresh",
            Width = 120,
            Height = 38,
            Dock = DockStyle.Right,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            Font = new Font(
                "Segoe UI",
                10,
                FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        refreshButton.FlatAppearance.BorderSize = 0;

        refreshButton.Click += async (s, e) =>
        {
            await LoadRentals();
        };

        headerPanel.Controls.Add(refreshButton);
        headerPanel.Controls.Add(titleLabel);

        rentalPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(25),
            BackColor = Color.FromArgb(245, 246, 250)
        };

        rentalPanel.Resize += (s, e) =>
        {
            foreach (Control control in rentalPanel.Controls)
            {
                control.Width = rentalPanel.ClientSize.Width - 60;
            }
        };

        Controls.Add(rentalPanel);
        Controls.Add(headerPanel);
    }

    private async void MyRentalsView_Load(
        object? sender,
        EventArgs e)
    {
        await LoadRentals();
    }

    private async Task LoadRentals()
    {
        rentalPanel.Controls.Clear();

        var response = await _httpService.GetAsync<List<UserRentalsResponse>>(
            $"rentals/user/{_accountManager.User!.Id}");

        var rentals = response.Content;

        if (rentals == null || rentals.Count == 0)
        {
            rentalPanel.Controls.Add(
                new Label
                {
                    Text = "You currently have no rented movies.",
                    Font = new Font(
                        "Segoe UI",
                        12),
                    AutoSize = true,
                    Padding = new Padding(20)
                });

            return;
        }

        foreach (var rental in rentals)
        {
            AddRentalCard(rental);
        }
    }

    private void AddRentalCard(UserRentalsResponse rental)
    {
        Panel card = new Panel
        {
            Height = 120,
            Width = rentalPanel.ClientSize.Width - 60,
            BackColor = Color.White,
            Margin = new Padding(0, 0, 0, 15),
            Padding = new Padding(20)
        };

        Label movieName = new Label
        {
            Text = rental.Movie.Name,
            Font = new Font(
                "Segoe UI",
                14,
                FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 30, 30),
            AutoSize = true,
            Location = new Point(20, 15)
        };

        Label details = new Label
        {
            AutoSize = true,
            Location = new Point(20, 45),
            ForeColor = Color.Gray,
            Font = new Font(
                "Segoe UI",
                10),
            Text =
                $"Released: {rental.Movie.ReleaseDate:d}\n" +
                $"Rented: {rental.DateRented:d}"
        };

        Button returnButton = new Button
        {
            Text = "Return Movie",
            Width = 140,
            Height = 38,
            BackColor = Color.FromArgb(231, 76, 60),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font(
                "Segoe UI",
                9,
                FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        returnButton.FlatAppearance.BorderSize = 0;

        returnButton.Location = new Point(
            card.Width - returnButton.Width - 25,
            40);

        card.Resize += (s, e) =>
        {
            returnButton.Left =
                card.Width -
                returnButton.Width -
                25;
        };

        returnButton.Click += async (s, e) =>
        {
            await ReturnRental(rental);
        };

        card.MouseEnter += (s, e) =>
        {
            card.BackColor = Color.FromArgb(235, 245, 255);
        };

        card.MouseLeave += (s, e) =>
        {
            card.BackColor = Color.White;
        };

        card.Controls.Add(movieName);
        card.Controls.Add(details);
        card.Controls.Add(returnButton);

        rentalPanel.Controls.Add(card);
    }

    private async Task ReturnRental(UserRentalsResponse rental)
    {
        var response = await _httpService.PatchAsync<ReturnRequest, RentalResponse>($"rentals/{rental.Id}",
            new ReturnRequest
            {
                DateReturned = DateOnly.FromDateTime(DateTime.Now)
            });

        if (response.Status == HttpStatusCode.OK)
        {
            MessageBox.Show(
                $"You returned {rental.Movie.Name}",
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            await LoadRentals();
        }
        else
        {
            MessageBox.Show(
                "Failed to return movie.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}