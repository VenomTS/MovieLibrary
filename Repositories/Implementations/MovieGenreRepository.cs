using Microsoft.EntityFrameworkCore;
using Models;

namespace API.MovieGenres.Repositories;

public class MovieGenreRepository(AppDbContext dbContext) : IMovieGenreRepository
{
    public async Task AddMovieGenre(MovieGenre movieGenre)
    {
        await dbContext.MovieGenres.AddAsync(movieGenre);
        await dbContext.SaveChangesAsync();
    }

    public async Task RemoveMovieGenre(MovieGenre movieGenre)
    {
        dbContext.MovieGenres.Remove(movieGenre);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> MovieGenreExists(MovieGenre movieGenre)
    {
        return await dbContext.MovieGenres.AnyAsync(x =>
            x.MovieId == movieGenre.MovieId && x.GenreId == movieGenre.GenreId);
    }
}