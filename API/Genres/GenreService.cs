using API.Genres.Repositories;
using API.OneOfTypes;
using DTO.Genres;
using Models;
using OneOf;

namespace API.Genres;

public class GenreService(IGenreRepository genreRepo)
{
    public async Task<OneOf<GenreResponse, GenreAlreadyExists>> CreateGenre(CreateGenreRequest request)
    {
        var genreExists = await genreRepo.GenreExistsAsync(request.Name);
        if (genreExists)
            return new GenreAlreadyExists();

        var genre = new Genre
        {
            Name = request.Name
        };

        await genreRepo.CreateAsync(genre);
        return new GenreResponse
        {
            Id = genre.Id,
            Name = genre.Name
        };
    }
}