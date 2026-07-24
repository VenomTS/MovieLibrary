using App.Services.Interfaces;
using DTO.Genres;
using DTO.Movies;

namespace App.Dialogs;

public partial class EditMovieDialog : Form
{
    private readonly IHttpService _httpService;
    private readonly MovieResponse _movie;

    private TextBox txtMovieName;
    private DateTimePicker dtpReleaseDate;
    private FlowLayoutPanel genresPanel;
    private Label lblNoGenres;
    private Button btnAddGenre;
    private Button btnSave;
    private Button btnCancel;

    private readonly List<GenreResponse> _genres = new();

    public string MovieName => txtMovieName.Text.Trim();

    public DateOnly ReleaseDate => DateOnly.FromDateTime(dtpReleaseDate.Value);

    public List<Guid> SelectedGenreIds =>
        genresPanel.Controls
            .OfType<CheckBox>()
            .Where(c => c.Checked)
            .Select(c => (Guid)c.Tag!)
            .ToList();

    public EditMovieDialog(
        IHttpService httpService,
        MovieResponse movie)
    {
        _httpService = httpService;
        _movie = movie;

        InitializeUI();

        Load += EditMovieDialog_Load;
    }

    private void InitializeUI()
    {
        Text = "Edit Movie";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        ClientSize = new Size(500, 600);

        Label lblMovieName = new Label
        {
            Text = "Movie Name",
            AutoSize = true,
            Location = new Point(20, 20),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        txtMovieName = new TextBox
        {
            Width = 450,
            Location = new Point(20, 45),
            Font = new Font("Segoe UI", 10),
            Text = _movie.Name
        };

        Label lblReleaseDate = new Label
        {
            Text = "Release Date",
            AutoSize = true,
            Location = new Point(20, 85),
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
        };

        dtpReleaseDate = new DateTimePicker
        {
            Width = 220,
            Location = new Point(20, 110),
            Value = _movie.ReleaseDate.ToDateTime(TimeOnly.MinValue),
            Format = DateTimePickerFormat.Short
        };

        Label lblGenres = new Label
        {
            Text = "Genres",
            AutoSize = true,
            Location = new Point(20, 155),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        btnAddGenre = new Button
        {
            Text = "+ Create Genre",
            Width = 130,
            Height = 32,
            Location = new Point(340, 150),
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };

        btnAddGenre.FlatAppearance.BorderSize = 0;

        lblNoGenres = new Label
        {
            Text = "No Genres Available",
            AutoSize = true,
            Visible = false,
            ForeColor = Color.Gray,
            Location = new Point(10, 10)
        };

        genresPanel = new FlowLayoutPanel
        {
            Location = new Point(20, 190),
            Size = new Size(450, 280),
            BorderStyle = BorderStyle.FixedSingle,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true
        };

        genresPanel.Controls.Add(lblNoGenres);

        btnSave = new Button
        {
            Text = "Save",
            Width = 100,
            Height = 38,
            Location = new Point(260, 520),
            BackColor = Color.FromArgb(46, 204, 113),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };

        btnSave.FlatAppearance.BorderSize = 0;

        btnCancel = new Button
        {
            Text = "Cancel",
            Width = 100,
            Height = 38,
            Location = new Point(370, 520),
            FlatStyle = FlatStyle.Flat
        };

        btnAddGenre.Click += BtnAddGenre_Click;
        btnSave.Click += BtnSave_Click;
        btnCancel.Click += (s, e) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        Controls.Add(lblMovieName);
        Controls.Add(txtMovieName);

        Controls.Add(lblReleaseDate);
        Controls.Add(dtpReleaseDate);

        Controls.Add(lblGenres);
        Controls.Add(btnAddGenre);

        Controls.Add(genresPanel);

        Controls.Add(btnSave);
        Controls.Add(btnCancel);

        AcceptButton = btnSave;
        CancelButton = btnCancel;
    }
    
    private async void BtnAddGenre_Click(object? sender, EventArgs e)
    {
        using var dialog = new AddGenreDialog(_httpService);

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            await LoadGenres();
        }
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtMovieName.Text))
        {
            MessageBox.Show(
                "Movie name is required.",
                "Validation",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private async void EditMovieDialog_Load(object? sender, EventArgs e)
    {
        await LoadGenres();
    }

    private async Task LoadGenres()
    {
        genresPanel.Controls.Clear();

        var response =
            await _httpService.GetAsync<List<GenreResponse>>("genres");

        var genres = response.Content ?? new List<GenreResponse>();

        _genres.Clear();
        _genres.AddRange(genres);

        if (_genres.Count == 0)
        {
            lblNoGenres.Visible = true;
            genresPanel.Controls.Add(lblNoGenres);
            return;
        }

        lblNoGenres.Visible = false;

        foreach (var genre in _genres)
        {
            var checkBox = new CheckBox
            {
                Text = genre.Name,
                Tag = genre.Id,
                Width = 400,
                AutoSize = false,
                Height = 28,
                Checked = _movie.Genres.Any(g => g.Id == genre.Id)
            };

            genresPanel.Controls.Add(checkBox);
        }
    }
}