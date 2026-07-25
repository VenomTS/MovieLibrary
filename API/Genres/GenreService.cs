using API.OneOfTypes;
using AutoMapper;
using DTO.Genres;
using Models;
using OneOf;
using Repositories;
using Repositories.Interfaces;

namespace API.Genres;

public class GenreService(IMapper mapper, IRepositoryManager repositoryManager)
{
    public async Task<OneOf<GenreResponse, GenreNotFound>> GetByIdAsync(Guid id)
    {
        var genre = await repositoryManager.Genres.GetByIdAsync(id);
        if (genre == null)
            return new GenreNotFound();
        
        return mapper.Map<GenreResponse>(genre);
    }
    public async Task<OneOf<GenreResponse, GenreAlreadyExists>> CreateGenre(CreateGenreRequest request)
    {
        var genreExists = await repositoryManager.Genres.GenreExistsAsync(request.Name);
        if (genreExists)
            return new GenreAlreadyExists();
        
        var genre = mapper.Map<Genre>(request);

        await repositoryManager.Genres.CreateAsync(genre);
        await repositoryManager.Genres.SaveChangesAsync();
        
        return mapper.Map<GenreResponse>(genre);
    }

    public async Task<List<GenreResponse>> GetAllAsync()
    {
        var genres = await repositoryManager.Genres.GetAllAsync();
        
        var genreDto = mapper.Map<List<GenreResponse>>(genres);
        
        return genreDto.ToList();
    }
}