using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Database;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class GenreRepository(AppDbContext dbContext) : RepositoryBase<Genre>(dbContext), IGenreRepository
{
    private readonly AppDbContext dbContext = dbContext;

    public async Task<Genre?> GetByGenreNameAsync(string genreName)
    {
        return await dbContext.Genres.FirstOrDefaultAsync(x => x.Name == genreName);
    }

    public async Task<bool> GenreExistsAsync(string name)
    {
        return await dbContext.Genres.AnyAsync(x => x.Name == name);
    }

    public async Task<bool> GenreExistsAsync(Guid id)
    {
        return await dbContext.Genres.AnyAsync(x => x.Id == id);
    }
}