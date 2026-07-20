using API.MovieGenres.DTO;
using API.MovieGenres.Repositories;

namespace API.MovieGenres;

public class MovieGenreService(IMovieGenreRepository movieGenreRepo)
{
    public async Task AddMovieGenre(AddMovieGenreRequest request)
    {
        var movieGenre = new MovieGenre
        {
            MovieId = request.MovieId,
            GenreId = request.GenreId
        };
        var movieGenreExists = await movieGenreRepo.MovieGenreExists(movieGenre);
        if (movieGenreExists)
            return;
        
        await movieGenreRepo.AddMovieGenre(movieGenre);
    }

    public async Task RemoveMovieGenre(RemoveMovieGenreRequest request)
    {
        var movieGenre = new MovieGenre
        {
            MovieId = request.MovieId,
            GenreId = request.GenreId
        };
        
        var movieGenreExists = await movieGenreRepo.MovieGenreExists(movieGenre);
        if (movieGenreExists)
            return;
        
        await movieGenreRepo.RemoveMovieGenre(movieGenre);
    }
}