using API.OneOfTypes;
using DTO.Movies;
using DTO.SearchQueries;
using Models;
using OneOf;
using Repositories.Interfaces;

namespace API.Movies;

public class MovieService(IMovieRepository movieRepo, IRentalRepository rentalRepo, IInventoryRecordRepository inventoryRepo)
{
    public async Task<List<MovieResponse>> GetMoviesAsync(MovieSearchQuery query)
    {
        var movies = await movieRepo.Search(query);

        var moviesDto = movies.Select(x => new MovieResponse
        {
            Id = x.Id,
            Name = x.Name,
            ReleaseDate = x.ReleaseDate,
            Genres = x.Genres.Select(genre => new MovieGenreResponse
            {
                Id = genre.Id,
                Name = genre.Name,
            }).ToList(),
            Stock = new MovieStockResponse
            {
                AmountInStock = GetMovieStock(x.Id).GetAwaiter().GetResult()
            }
        });
        
        return moviesDto.ToList();
    }
    
    public async Task<OneOf<MovieResponse, MovieAlreadyExists>> AddMovieAsync(AddMovieRequest request)
    {
        var movie = new Movie
        {
            Name = request.Name,
            ReleaseDate = request.ReleaseDate,
        };

        var movieExists = await movieRepo.MovieExistsAsync(movie);
        if (movieExists)
            return new MovieAlreadyExists();

        await movieRepo.CreateAsync(movie);
        var movieResponse = new MovieResponse
        {
            Id = movie.Id,
            Name = movie.Name,
            ReleaseDate = movie.ReleaseDate,
            Genres = [],
        };
        
        await movieRepo.SaveChangesAsync();
        return movieResponse;
    }

    public async Task<OneOf<MovieResponse, MovieNotFound>> GetMovieByIdAsync(Guid id)
    {
        var movie = await movieRepo.GetByIdAsync(id, x => x.Id == id);

        return movie == null
            ? new MovieNotFound()
            : new MovieResponse
            {
                Id = movie.Id,
                Name = movie.Name,
                ReleaseDate = movie.ReleaseDate,
                Genres = movie.Genres.Select(x => new MovieGenreResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList(),
                Stock = new MovieStockResponse
                {
                    AmountInStock = GetMovieStock(movie.Id).GetAwaiter().GetResult(),
                }
            };
    }

    private async Task<int> GetMovieStock(Guid movieId)
    {
        var movieExists = await movieRepo.MovieExistsAsync(movieId);
        if (!movieExists)
            return 0;

        var totalInventory = await inventoryRepo.GetTotalAmount(movieId, DateOnly.FromDateTime(DateTime.Now));
        var totalRentedMovies = await rentalRepo.GetByMovieIdAsync(movieId);
        var totalMoviesNotReturned = totalRentedMovies.Count(x => x.DateReturned == null);
        return totalInventory - totalMoviesNotReturned;
    }
}