namespace API.MovieGenres.Repositories;

public interface IMovieGenreRepository
{
    public Task AddMovieGenre(MovieGenre movieGenre);
    public Task RemoveMovieGenre(MovieGenre movieGenre);
    public Task<bool> MovieGenreExists(MovieGenre movieGenre);
}