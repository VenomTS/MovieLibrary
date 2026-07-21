using DTO.SearchQueries;

namespace API.Movies.Repositories;

public interface IMovieRepository
{
    public Task<IEnumerable<Movie>> GetMoviesAsync(MovieSearchQuery query);
    public Task<IEnumerable<Movie>> GetAvailableMoviesAsync(MovieSearchQuery query);
    public Task<Movie?> GetMovieByIdAsync(Guid movieId);
    public Task<Movie> AddMovieAsync(Movie movie);
    public Task<bool> MovieExistsAsync(Movie movie);
    public Task<bool> MovieExistsAsync(Guid movieId);
}