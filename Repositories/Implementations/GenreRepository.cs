using API.Genres.Repositories;
using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Databasee;

namespace Repositories.Implementations;

public class GenreRepository(AppDbContext dbContext) : RepositoryBase<Genre>(dbContext), IGenreRepository
{
    public async Task<bool> GenreExistsAsync(string name)
    {
        return await dbContext.Genres.AnyAsync(x => x.Name == name);
    }

    public async Task<bool> GenreExistsAsync(Guid id)
    {
        return await dbContext.Genres.AnyAsync(x => x.Id == id);
    }
}