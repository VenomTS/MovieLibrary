using App.Services.Interfaces;
using DTO.Movies;
using DTO.Rentals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using App.Account;

namespace App
{
    public partial class MainPage : Form
    {

        private DataGridView dgvMovies;
        private Button btnRefresh;
        private TextBox txtSearch;

        private IHttpService _httpService;
        private AccountManager _accountManager;

        public MainPage(IHttpService httpService, AccountManager accountManager)
        {
            InitializeComponent();
            BuildComponents();
            _httpService = httpService;
            _accountManager = accountManager;
        }

        private List<MovieResponse> _movies = new();

        private async Task LoadMovies()
        {
            var movieName = txtSearch.Text;
            var movieSearchParam = string.IsNullOrWhiteSpace(movieName) ? "" : $"?name={movieName}";

            (var statusCode, _movies) = await _httpService.GetAsync<List<MovieResponse>>($"movies{movieSearchParam}");
            DisplayMovies();
        }

        private void DisplayMovies()
        {
            dgvMovies.Rows.Clear();

            foreach(var movie in _movies)
            {
                int rowIndex = dgvMovies.Rows.Add(
                    movie.Name,
                    string.Join(", ", movie.Genres.Select(g => g.Name)),
                    movie.ReleaseDate.ToShortDateString(),
                    movie.Stock.AmountInStock);

                var buttonCell = (DataGridViewButtonCell)dgvMovies.Rows[rowIndex].Cells["Rent"];

                if (movie.Stock.AmountInStock > 0)
                {
                    buttonCell.Value = "Rent";
                }
                else
                {
                    buttonCell.Value = "Out of stock";
                    buttonCell.Style.ForeColor = Color.Gray;
                }

            }
        }

        private async void MovieForm_Load(object sender, EventArgs e)
        {
            await LoadMovies();
        }

        private async void DgvMovies_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dgvMovies.Columns[e.ColumnIndex].Name == "Rent")
            {
                var movie = _movies[e.RowIndex];
                // API attempt rent
                var (responseCode, content) = await _httpService.PostAsync<RentRequest, string>("rentals", new RentRequest
                {
                    MovieId = movie.Id,
                    UserId = _accountManager.User.Id,
                    DateRented = DateOnly.FromDateTime(DateTime.Now),
                });

                if (responseCode == HttpStatusCode.NoContent)
                    MessageBox.Show($"You successfully rented {movie.Name}");
                else if (responseCode == HttpStatusCode.OK)
                    MessageBox.Show("Movie is out of stock");
                else
                    MessageBox.Show($"{responseCode} - {_accountManager.User.Id} - {movie.Id}");
                // Add when movie is not found and when appUser is not found FOR SOME REASON

                await LoadMovies();
            }
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

            var rentButton = new DataGridViewButtonColumn
            {
                Name = "Rent",
                HeaderText = "",
                Text = "Rent",
                UseColumnTextForButtonValue = true
            };
            dgvMovies.Columns.Add(rentButton);

            dgvMovies.CellContentClick += DgvMovies_CellContentClick;

            Controls.Add(txtSearch);
            Controls.Add(btnRefresh);
            Controls.Add(dgvMovies);

            Load += MovieForm_Load;
        }
    }
}
