using App.Dialogs;
using App.Services.Interfaces;
using DTO;
using DTO.InventoryRecords;
using DTO.Movies;

namespace App.UserControls;

public partial class MovieInventoryView : UserControl
{
    private FlowLayoutPanel moviePanel;
    private Button addMovieButton;
    private Label titleLabel;

    private IHttpService _httpService;

    public MovieInventoryView(IHttpService httpService)
    {
        InitializeComponent();

        _httpService = httpService;

        SetupUI();

        Load += MovieInventoryControl_Load;
    }


    private void SetupUI()
    {
        BackColor = Color.FromArgb(245, 246, 250);


        Panel headerPanel = new Panel()
        {
            Dock = DockStyle.Top,
            Height = 60,
            BackColor = Color.FromArgb(245, 246, 250)
        };


        titleLabel = new Label()
        {
            Text = "Movie Inventory",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.FromArgb(40, 40, 40),
            AutoSize = true,
            Left = 20,
            Top = 15
        };


        addMovieButton = new Button()
        {
            Text = "+ Add Movie",
            Width = 130,
            Height = 35,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        addMovieButton.FlatAppearance.BorderSize = 0;


        // Position button on the right
        addMovieButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        addMovieButton.Left = headerPanel.Width - addMovieButton.Width - 20;
        addMovieButton.Top = 12;


        headerPanel.Resize += (s, e) =>
        {
            addMovieButton.Left = headerPanel.Width - addMovieButton.Width - 20;
        };


        addMovieButton.Click += AddMovieButton_Click;


        headerPanel.Controls.Add(titleLabel);
        headerPanel.Controls.Add(addMovieButton);



        moviePanel = new FlowLayoutPanel()
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(20),
            BackColor = Color.FromArgb(245,246,250)
        };


        Controls.Add(moviePanel);
        Controls.Add(headerPanel);
    }


    private async void MovieInventoryControl_Load(object sender, EventArgs e)
    {
        await LoadMovies();
    }


    private async Task LoadMovies()
    {
        moviePanel.Controls.Clear();


        var (statusCode, movies) =
            await _httpService.GetAsync<List<MovieResponse>>("movies");


        if (movies == null || movies.Count == 0)
        {
            moviePanel.Controls.Add(
                new Label()
                {
                    Text = "No movies available",
                    Font = new Font("Segoe UI", 12),
                    AutoSize = true,
                    Padding = new Padding(20)
                });

            return;
        }


        foreach(var movie in movies)
        {
            AddMovieRow(movie);
        }
    }



    private void AddMovieRow(MovieResponse movie)
    {
        Panel card = new Panel()
        {
            Width = 750,
            Height = 110,
            BackColor = Color.White,
            Margin = new Padding(0,0,0,15),
            Padding = new Padding(15)
        };


        Label movieName = new Label()
        {
            Text = movie.Name,
            Font = new Font(
                "Segoe UI",
                13,
                FontStyle.Bold),
            ForeColor = Color.FromArgb(30,30,30),
            AutoSize = true,
            Left = 15,
            Top = 10
        };
        
        var movieGenres = movie.Genres.Count == 0 ? "None" : string.Join(", ", movie.Genres);

        Label details = new Label()
        {
            AutoSize = true,
            Left = 15,
            Top = 40,
            ForeColor = Color.Gray,
            Font = new Font("Segoe UI",10),
            Text =
                $"Genres: {movieGenres}\n" +
                $"Release: {movie.ReleaseDate:d}\n" +
                $"Available: {movie.Stock.AmountInStock}"
        };


        Button updateButton = new Button()
        {
            Text = "Update Stock",
            Width = 140,
            Height = 35,
            Left = 570,
            Top = 35,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(46,204,113),
            ForeColor = Color.White,
            Cursor = Cursors.Hand,
            Font = new Font("Segoe UI",9,FontStyle.Bold)
        };


        updateButton.FlatAppearance.BorderSize = 0;


        updateButton.Click += async (s,e)=>
        {
            await UpdateInventory(movie);
        };


        // Hover effects
        card.MouseEnter += (s,e)=>
        {
            card.BackColor = Color.FromArgb(235,245,255);
        };

        card.MouseLeave += (s,e)=>
        {
            card.BackColor = Color.White;
        };


        card.Controls.Add(movieName);
        card.Controls.Add(details);
        card.Controls.Add(updateButton);


        moviePanel.Controls.Add(card);
    }



    private async Task UpdateInventory(MovieResponse movie)
    {
        using var dialog = new InventoryDialog();

        if(dialog.ShowDialog() != DialogResult.OK)
            return;


        var request = new CreateInventoryRecordRequest
        {
            MovieId = movie.Id,
            Amount = dialog.Amount,
            Date = dialog.ArrivalDate
        };


        await _httpService.PostAsync<CreateInventoryRecordRequest, EmptyResponse>(
            "inventoryRecords",
            request);


        await LoadMovies();
    }



    private async void AddMovieButton_Click(object sender, EventArgs e)
    {
        using var dialog = new AddMovieDialog();

        if(dialog.ShowDialog() != DialogResult.OK)
            return;


        var movie = new AddMovieRequest
        {
            Name = dialog.MovieName,
            ReleaseDate = dialog.ReleaseDate
        };


        await _httpService.PostAsync<AddMovieRequest, MovieResponse>(
            "movies",
            movie);


        await LoadMovies();
    }
}