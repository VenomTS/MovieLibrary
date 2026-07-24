using System.Net;
using App.Dialogs;
using App.Services.Interfaces;
using DTO;
using DTO.InventoryRecords;
using DTO.Movies;

namespace App.UserControls;

public partial class MovieManagementView : UserControl
{
    private FlowLayoutPanel moviePanel;
    private Button addMovieButton;
    private Label titleLabel;

    private readonly IHttpService _httpService;

    public MovieManagementView(IHttpService httpService)
    {
        InitializeComponent();

        _httpService = httpService;

        SetupUI();

        Load += MovieInventoryControl_Load;
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
            Text = "Movie Inventory",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = Color.FromArgb(40, 40, 40),
            AutoSize = true,
            Dock = DockStyle.Left,
            TextAlign = ContentAlignment.MiddleLeft
        };

        addMovieButton = new Button
        {
            Text = "+ Add Movie",
            Width = 140,
            Height = 38,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Cursor = Cursors.Hand,
            Dock = DockStyle.Right
        };

        addMovieButton.FlatAppearance.BorderSize = 0;
        addMovieButton.Click += AddMovieButton_Click;

        headerPanel.Controls.Add(addMovieButton);
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
                control.Width = moviePanel.ClientSize.Width - 60;
            }
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

        var response = await _httpService.GetAsync<List<MovieResponse>>("movies");
        var movies = response.Content;

        if (movies == null || movies.Count == 0)
        {
            moviePanel.Controls.Add(
                new Label
                {
                    Text = "No movies available",
                    Font = new Font("Segoe UI", 12),
                    AutoSize = true,
                    Padding = new Padding(20)
                });

            return;
        }

        foreach (var movie in movies)
        {
            AddMovieRow(movie);
        }
    }

    private void AddMovieRow(MovieResponse movie)
    {
        Panel card = new Panel
        {
            Height = 120,
            Width = moviePanel.ClientSize.Width - 60,
            BackColor = Color.White,
            Margin = new Padding(0, 0, 0, 15),
            Padding = new Padding(20)
        };

        Label movieName = new Label
        {
            Text = movie.Name,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 30, 30),
            AutoSize = true,
            Location = new Point(20, 15)
        };

        string genres = movie.Genres.Count == 0
            ? "None"
            : string.Join(", ", movie.Genres.Select(x => x.Name));

        Label details = new Label
        {
            AutoSize = true,
            Location = new Point(20, 45),
            ForeColor = Color.Gray,
            Font = new Font("Segoe UI", 10),
            Text =
                $"Genres: {genres}\n" +
                $"Release: {movie.ReleaseDate:d}\n" +
                $"Available: {movie.Stock}"
        };

        Button updateButton = new Button
        {
            Text = "Update Stock",
            Width = 140,
            Height = 38,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(46, 204, 113),
            ForeColor = Color.White,
            Cursor = Cursors.Hand,
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };

        updateButton.FlatAppearance.BorderSize = 0;

        Button editButton = new Button
        {
            Text = "Edit",
            Width = 90,
            Height = 38,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(243, 156, 18),
            ForeColor = Color.White,
            Cursor = Cursors.Hand,
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };

        editButton.FlatAppearance.BorderSize = 0;

        const int spacing = 10;

        updateButton.Location = new Point(
            card.Width - updateButton.Width - 25,
            40);

        editButton.Location = new Point(
            updateButton.Left - editButton.Width - spacing,
            40);

        card.Resize += (s, e) =>
        {
            updateButton.Left = card.Width - updateButton.Width - 25;
            editButton.Left = updateButton.Left - editButton.Width - spacing;
        };

        updateButton.Click += async (s, e) =>
        {
            await UpdateInventory(movie);
        };

        editButton.Click += async (s, e) =>
        {
            await EditMovie(movie);
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
        card.Controls.Add(editButton);
        card.Controls.Add(updateButton);

        moviePanel.Controls.Add(card);
    }

    private async Task UpdateInventory(MovieResponse movie)
    {
        using var dialog = new InventoryDialog();

        if (dialog.ShowDialog() != DialogResult.OK)
            return;

        var request = new CreateInventoryRecordRequest
        {
            MovieId = movie.Id,
            Amount = dialog.Amount,
            Date = dialog.ArrivalDate
        };

        await _httpService.PostAsync<CreateInventoryRecordRequest, InventoryRecordResponse>(
            "inventoryRecords",
            request);

        await LoadMovies();
    }

    private async Task EditMovie(MovieResponse movie)
    {
        using var dialog = new EditMovieDialog(_httpService, movie);

        if (dialog.ShowDialog() != DialogResult.OK)
            return;

        var request = new UpdateMovieRequest
        {
            Name = dialog.MovieName,
            ReleaseDate = dialog.ReleaseDate,
            GenreIds = dialog.SelectedGenreIds
        };

        var response = await _httpService.PutAsync<UpdateMovieRequest, MovieResponse>(
            $"movies/{movie.Id}",
            request);

        if (response.Status == HttpStatusCode.NotFound)
        {
            MessageBox.Show("Movie not found", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        if (response.Status == HttpStatusCode.Conflict)
        {
            MessageBox.Show("Movie with given information already exists", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        
        MessageBox.Show("Movie updated", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

        await LoadMovies();
    }

    private async void AddMovieButton_Click(object sender, EventArgs e)
    {
        using var dialog = new AddMovieDialog();

        if (dialog.ShowDialog() != DialogResult.OK)
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