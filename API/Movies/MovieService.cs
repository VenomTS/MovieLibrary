using API.Movies.Repositories;
using API.OneOfTypes;
using API.Stocks;
using API.Stocks.Repositories;
using DTO.Movies;
using DTO.SearchQueries;
using OneOf;

namespace API.Movies;

public class MovieService(IMovieRepository movieRepo, IStockRepository stockRepo)
{
    public async Task<List<MovieResponse>> GetMoviesAsync(MovieSearchQuery query)
    {
        IEnumerable<Movie> movies;

        if (query.IsAvailable != null && query.IsAvailable.Value)
            movies = await movieRepo.GetAvailableMoviesAsync(query);
        else
            movies = await movieRepo.GetMoviesAsync(query);

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

        movie = await movieRepo.AddMovieAsync(movie);
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
        await stockRepo.AddAsync(stock);
        
        return movieResponse;
    }

    public async Task<OneOf<MovieResponse, MovieNotFound>> GetMovieByIdAsync(Guid id)
    {
        var movie = await movieRepo.GetMovieByIdAsync(id);

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