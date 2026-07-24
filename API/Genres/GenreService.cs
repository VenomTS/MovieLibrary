using API.OneOfTypes;
using DTO.Genres;
using Models;
using OneOf;
using Repositories.Interfaces;

namespace API.Genres;

public class GenreService(IGenreRepository genreRepo)
{
    public async Task<OneOf<GenreResponse, GenreNotFound>> GetByIdAsync(Guid id)
    {
        var genre = await genreRepo.GetByIdAsync(id);
        if (genre == null)
            return new GenreNotFound();

        return new GenreResponse
        {
            Id = genre.Id,
            Name = genre.Name
        };
    }
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
        await genreRepo.SaveChangesAsync();

        return new GenreResponse
        {
            Id = genre.Id,
            Name = genre.Name
        };
    }

    public async Task<List<GenreResponse>> GetAllAsync()
    {
        var genres = await genreRepo.GetAllAsync();

        var genreDto = genres.Select(x => new GenreResponse
        {
            Id = x.Id,
            Name = x.Name
        });
        
        return genreDto.ToList();
    }
}