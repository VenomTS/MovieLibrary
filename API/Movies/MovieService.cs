using API.Movies.DTO;
using API.Movies.Repositories;
using API.OneOfTypes;
using OneOf;

namespace API.Movies;

public class MovieService(IMovieRepository movieRepo)
{
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
        };
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
            };
    }
}