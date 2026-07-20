namespace API.Movies.Repositories;

public interface IMovieRepository
{
    public Task<IEnumerable<Movie>> GetMoviesAsync(SearchQuery query);
    public Task<IEnumerable<Movie>> GetAvailableMoviesAsync(SearchQuery query);
    public Task<Movie?> GetMovieByIdAsync(Guid movieId);
    public Task<Movie> AddMovieAsync(Movie movie);
    public Task<bool> MovieExistsAsync(Movie movie);
}