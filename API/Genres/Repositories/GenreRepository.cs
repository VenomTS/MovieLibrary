using API.Database;
using Microsoft.EntityFrameworkCore;

namespace API.Genres.Repositories;

public class GenreRepository(AppDbContext dbContext) : IGenreRepository
{
    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        var genres = await dbContext.Genres.ToListAsync();
        return genres;
    }

    public async Task<Genre?> GetByIdAsync(Guid id)
    {
        var genre = await dbContext.Genres.FirstOrDefaultAsync(x => x.Id == id);
        return genre;
    }

    public async Task<Genre> CreateAsync(Genre genre)
    {
        await dbContext.Genres.AddAsync(genre);
        await dbContext.SaveChangesAsync();
        return genre;
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