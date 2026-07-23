using App.Services.Interfaces;
using DTO.Movies;
using DTO.Rentals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;
using App.Account;

namespace App.UserControls
{
    public partial class RentMoviesView : UserControl
    {

        private DataGridView dgvMovies;
        private Button btnRefresh;
        private TextBox txtSearch;

        //
        private IHttpService _httpService;
        private AccountManager _accountManager;

        private List<MovieResponse> _movies = [];

        public RentMoviesView(IHttpService httpService, AccountManager accountManager)
        {
            InitializeComponent();
            InitializeControls();

            _httpService = httpService;
            _accountManager = accountManager;

        }

        private async Task LoadMovies()
        {
            var movieName = txtSearch.Text;
            var movieSearchParam = string.IsNullOrWhiteSpace(movieName) ? "" : $"?name={movieName}";

            var (statusCode, movies) = await _httpService.GetAsync<List<MovieResponse>>($"movies{movieSearchParam}");
            if (movies == null)
                throw new Exception("Movies are null");

            _movies = movies;
            DisplayMovies();
        }

        private void DisplayMovies()
        {
            dgvMovies.Rows.Clear();

            foreach (var movie in _movies)
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
                    UserId = _accountManager.User!.Id,
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

        private void InitializeControls()
        {
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
