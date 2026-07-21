using API.OneOfTypes;
using DTO.MovieGenres;
using Models;
using OneOf;
using OneOf.Types;
using Repositories.Interfaces;

namespace API.MovieGenres;

public class MovieGenreService(IMovieGenreRepository movieGenreRepo, IMovieRepository movieRepo, IGenreRepository genreRepo)
{
    public async Task<OneOf<Success, MovieNotFound, GenreNotFound, MovieGenreAlreadyExists>> AddMovieGenre(AddMovieGenreRequest request)
    {
        var movieExists = await movieRepo.MovieExistsAsync(request.MovieId);
        if(!movieExists)
            return new MovieNotFound();
        
        var genreExists = await genreRepo.GenreExistsAsync(request.GenreId);
        if(!genreExists)
            return new GenreNotFound();
        
        var movieGenre = new MovieGenre
        {
            MovieId = request.MovieId,
            GenreId = request.GenreId
        };
        var movieGenreExists = await movieGenreRepo.MovieGenreExists(movieGenre);
        if (movieGenreExists)
            return new MovieGenreAlreadyExists();
        
        await movieGenreRepo.CreateAsync(movieGenre);
        return new Success();
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
        
        await movieGenreRepo.Delete(movieGenre);
    }
}