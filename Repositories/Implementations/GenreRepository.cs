using Microsoft.EntityFrameworkCore;
using Models;
using Repositories;
using Repositories.Databasee;

namespace API.Genres.Repositories;

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