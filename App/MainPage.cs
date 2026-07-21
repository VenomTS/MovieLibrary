using DTO.Movies;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace App
{
    public partial class MainPage : Form
    {
        private HttpClientHandler handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        private readonly HttpClient _httpClient;

        private DataGridView dgvMovies;
        private Button btnRefresh;
        private TextBox txtSearch;

        public MainPage()
        {
            _httpClient = new HttpClient(handler);
            InitializeComponent();
            BuildComponents();

            _httpClient.BaseAddress = new Uri("http://localhost:5126/");
        }

        private List<MovieResponse> _movies = new();

        private async Task LoadMovies()
        {
            var response = await _httpClient.GetAsync("api/v1/movies");
            response.EnsureSuccessStatusCode();

            _movies = await response.Content.ReadFromJsonAsync<List<MovieResponse>>();

            DisplayMovies(_movies);
        }

        private void DisplayMovies(IEnumerable<MovieResponse> movies)
        {
            dgvMovies.Rows.Clear();

            foreach (var movie in movies)
            {
                dgvMovies.Rows.Add(
                    movie.Name,
                    string.Join(", ", movie.Genres.Select(g => g.Name)),
                    movie.ReleaseDate.ToShortDateString(),
                    movie.Stock.AmountInStock);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            var filtered = _movies.Where(m =>
                m.Name.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase));

            DisplayMovies(filtered);
        }

        private async void MovieForm_Load(object sender, EventArgs e)
        {
            await LoadMovies();
        }

        private void BuildComponents()
        {
            this.Text = "Movies";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            txtSearch = new TextBox
            {
                Left = 20,
                Top = 20,
                Width = 250
            };

            btnRefresh = new Button
            {
                Text = "Refresh",
                Left = 290,
                Top = 18,
                Width = 100
            };

            btnRefresh.Click += async (s, e) => await LoadMovies();

            dgvMovies = new DataGridView
            {
                Left = 20,
                Top = 60,
                Width = 840,
                Height = 470,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            dgvMovies.Columns.Add("Name", "Name");
            dgvMovies.Columns.Add("Genres", "Genres");
            dgvMovies.Columns.Add("ReleaseDate", "Release Date");
            dgvMovies.Columns.Add("Stock", "Stock");

            Controls.Add(txtSearch);
            Controls.Add(btnRefresh);
            Controls.Add(dgvMovies);

            Load += MovieForm_Load;
        }
    }
}
