using API.OneOfTypes;
using AutoMapper;
using DTO.Genres;
using Models;
using OneOf;
using Repositories.Interfaces;

namespace API.Genres;

public class GenreService(IMapper mapper, IGenreRepository genreRepo)
{
    public async Task<OneOf<GenreResponse, GenreNotFound>> GetByIdAsync(Guid id)
    {
        var genre = await genreRepo.GetByIdAsync(id);
        if (genre == null)
            return new GenreNotFound();
        
        return mapper.Map<GenreResponse>(genre);
    }
    public async Task<OneOf<GenreResponse, GenreAlreadyExists>> CreateGenre(CreateGenreRequest request)
    {
        var genreExists = await genreRepo.GenreExistsAsync(request.Name);
        if (genreExists)
            return new GenreAlreadyExists();
        
        var genre = mapper.Map<Genre>(request);

        await genreRepo.CreateAsync(genre);
        await genreRepo.SaveChangesAsync();
        
        return mapper.Map<GenreResponse>(genre);
    }

    public async Task<List<GenreResponse>> GetAllAsync()
    {
        var genres = await genreRepo.GetAllAsync();
        
        var genreDto = mapper.Map<List<GenreResponse>>(genres);
        
        return genreDto.ToList();
    }
}