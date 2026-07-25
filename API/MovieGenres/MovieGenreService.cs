using API.OneOfTypes;
using AutoMapper;
using DTO.MovieGenres;
using Models;
using OneOf;
using OneOf.Types;
using Repositories;

namespace API.MovieGenres;

public class MovieGenreService(IMapper mapper, IRepositoryManager repositoryManager)
{
    public async Task<OneOf<Success, MovieNotFound, GenreNotFound, MovieGenreAlreadyExists>> AddMovieGenre(AddMovieGenreRequest request)
    {
        var movieExists = await repositoryManager.Movies.MovieExistsAsync(request.MovieId);
        if(!movieExists)
            return new MovieNotFound();
        
        var genreExists = await  repositoryManager.Genres.GenreExistsAsync(request.GenreId);
        if(!genreExists)
            return new GenreNotFound();
        
        var movieGenre = mapper.Map<MovieGenre>(request);
        var movieGenreExists = await  repositoryManager.MovieGenres.MovieGenreExists(movieGenre);
        if (movieGenreExists)
            return new MovieGenreAlreadyExists();
        
        await  repositoryManager.MovieGenres.CreateAsync(movieGenre);
        await repositoryManager.SaveChangesAsync();
        return new Success();
    }
}