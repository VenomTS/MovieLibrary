using App.Account;
using App.Services.Interfaces;
using DTO.Movies;
using DTO.Rentals;
using System.Net;
using Timer = System.Windows.Forms.Timer;

namespace App.UserControls;

public partial class RentMoviesView : UserControl
{
    private FlowLayoutPanel moviePanel;
    private Button refreshButton;
    private TextBox searchBox;
    private Label titleLabel;
    private Timer _searchTimer;

    private readonly IHttpService _httpService;
    private readonly AccountManager _accountManager;

    private const int DebounceTimeMS = 150;


    public RentMoviesView(
        IHttpService httpService,
        AccountManager accountManager)
    {
        InitializeComponent();

        _httpService = httpService;
        _accountManager = accountManager;

        SetupUI();

        Load += RentMoviesView_Load;
    }


    private void SetupUI()
{
    BackColor = Color.FromArgb(245, 246, 250);

    Panel headerPanel = new Panel
    {
        Dock = DockStyle.Top,
        Height = 70,
        BackColor = Color.White,
        Padding = new Padding(20, 0, 20, 0)
    };


    titleLabel = new Label
    {
        Text = "Rent Movies",
        Font = new Font(
            "Segoe UI",
            20,
            FontStyle.Bold),
        ForeColor = Color.FromArgb(40, 40, 40),
        AutoSize = true,
        Dock = DockStyle.Left,
        TextAlign = ContentAlignment.MiddleLeft
    };


    Panel actionPanel = new Panel
    {
        Dock = DockStyle.Right,
        Width = 390,
        Height = 70
    };


    searchBox = new TextBox
    {
        Width = 240,
        Height = 35,
        Font = new Font(
            "Segoe UI",
            10),
        Location = new Point(0, 17)
    };
    
    _searchTimer = new Timer
    {
        Interval = DebounceTimeMS
    };

    _searchTimer.Tick += async (s, e) =>
    {
        _searchTimer.Stop();
        await LoadMovies();
    };


    refreshButton = new Button
    {
        Text = "Refresh",
        Width = 120,
        Height = 38,
        Location = new Point(255, 15),
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
        await LoadMovies();
    };


    searchBox.TextChanged += (s, e) =>
    {
        _searchTimer.Stop();
        _searchTimer.Start();
    };


    actionPanel.Controls.Add(searchBox);
    actionPanel.Controls.Add(refreshButton);


    headerPanel.Controls.Add(actionPanel);
    headerPanel.Controls.Add(titleLabel);


    moviePanel = new FlowLayoutPanel
    {
        Dock = DockStyle.Fill,
        AutoScroll = true,
        FlowDirection = FlowDirection.TopDown,
        WrapContents = false,
        Padding = new Padding(25),
        BackColor = Color.FromArgb(245, 246, 250)
    };


    moviePanel.Resize += (s, e) =>
    {
        foreach (Control control in moviePanel.Controls)
        {
            control.Width =
                moviePanel.ClientSize.Width - 60;
        }
    };


    headerPanel.Resize += (s, e) =>
    {
        searchBox.Top =
            (headerPanel.Height - searchBox.Height) / 2;

        refreshButton.Top =
            (headerPanel.Height - refreshButton.Height) / 2;
    };


    Controls.Add(moviePanel);
    Controls.Add(headerPanel);
}


    private async void RentMoviesView_Load(
        object sender,
        EventArgs e)
    {
        await LoadMovies();
    }


    private async Task LoadMovies()
    {
        moviePanel.Controls.Clear();


        string search =
            string.IsNullOrWhiteSpace(searchBox.Text)
            ? ""
            : $"?name={searchBox.Text}";


        var response = await _httpService.GetAsync<List<MovieResponse>>($"movies{search}");
        var movies = response.Content;

        if(movies == null || movies.Count == 0)
        {
            moviePanel.Controls.Add(
                new Label
                {
                    Text = "No movies available",
                    Font = new Font(
                        "Segoe UI",
                        12),
                    AutoSize = true,
                    Padding = new Padding(20)
                });

            return;
        }


        foreach(var movie in movies)
        {
            AddMovieCard(movie);
        }
    }



    private void AddMovieCard(MovieResponse movie)
    {
        Panel card = new Panel
        {
            Height = 120,
            Width = moviePanel.ClientSize.Width - 60,
            BackColor = Color.White,
            Margin = new Padding(0,0,0,15),
            Padding = new Padding(20)
        };


        Label movieName = new Label
        {
            Text = movie.Name,
            Font = new Font(
                "Segoe UI",
                14,
                FontStyle.Bold),
            ForeColor =
                Color.FromArgb(30,30,30),
            AutoSize = true,
            Location =
                new Point(20,15)
        };


        string genres =
            movie.Genres.Count == 0
            ? "None"
            : string.Join(
                ", ",
                movie.Genres.Select(x=>x.Name));


        Label details = new Label
        {
            AutoSize = true,
            Location =
                new Point(20,45),
            ForeColor = Color.Gray,
            Font =
                new Font(
                    "Segoe UI",
                    10),
            Text =
                $"Genres: {genres}\n" +
                $"Release: {movie.ReleaseDate:d}\n" +
                $"Available: {movie.Stock}"
        };


        Button rentButton = new Button
        {
            Text = movie.Stock > 0
                ? "Rent Movie"
                : "Unavailable",

            Width = 140,
            Height = 38,

            BackColor =
                movie.Stock > 0
                ? Color.FromArgb(46,204,113)
                : Color.Gray,

            ForeColor = Color.White,

            FlatStyle = FlatStyle.Flat,

            Font =
                new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold),

            Cursor = Cursors.Hand
        };


        rentButton.FlatAppearance.BorderSize = 0;


        rentButton.Location =
            new Point(
                card.Width -
                rentButton.Width -
                25,
                40);


        card.Resize += (s,e)=>
        {
            rentButton.Left =
                card.Width -
                rentButton.Width -
                25;
        };


        if(movie.Stock > 0)
        {
            rentButton.Click += async (s,e)=>
            {
                await RentMovie(movie);
            };
        }


        card.MouseEnter += (s,e)=>
        {
            card.BackColor =
                Color.FromArgb(235,245,255);
        };


        card.MouseLeave += (s,e)=>
        {
            card.BackColor = Color.White;
        };


        card.Controls.Add(movieName);
        card.Controls.Add(details);
        card.Controls.Add(rentButton);


        moviePanel.Controls.Add(card);
    }



    private async Task RentMovie(MovieResponse movie)
    {
        var response =
            await _httpService
            .PostAsync<RentRequest,RentalResponse>(
                "rentals",
                new RentRequest
                {
                    MovieId = movie.Id,
                    UserId = _accountManager.User!.Id,
                    DateRented =
                        DateOnly.FromDateTime(
                            DateTime.Now)
                });

        if(response.Status == HttpStatusCode.Created)
            MessageBox.Show($"You rented {movie.Name}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        else if(response.Status == HttpStatusCode.Conflict)
            MessageBox.Show($"{movie.Name} is out of stock", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        else if(response.Status == HttpStatusCode.BadRequest)
            MessageBox.Show($"You are already renting that movie", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        await LoadMovies();
    }
}