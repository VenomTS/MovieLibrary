using API.OneOfTypes;
using API.Stocks;
using DTO.Movies;
using DTO.SearchQueries;
using Models;
using OneOf;
using Repositories.Interfaces;

namespace API.Movies;

public class MovieService(IMovieRepository movieRepo, IStockRepository stockRepo)
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
                AmountInStock = x.Stock.Amount,
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

        var stock = new Stock
        {
            MovieId = movie.Id,
            Amount = 0,
        };
        await stockRepo.CreateAsync(stock);
        
        return movieResponse;
    }

    public async Task<OneOf<MovieResponse, MovieNotFound>> GetMovieByIdAsync(Guid id)
    {
        var movie = await movieRepo.GetByIdAsync(id);

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
                    AmountInStock = movie.Stock.Amount,
                }
            };
    }
}