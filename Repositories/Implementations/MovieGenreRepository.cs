using Microsoft.EntityFrameworkCore;
using Models;
using Repositories;
using Repositories.Database;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class MovieGenreRepository(AppDbContext dbContext) : RepositoryBase<MovieGenre>(dbContext), IMovieGenreRepository
{
    private readonly AppDbContext dbContext = dbContext;

    public async Task<bool> MovieGenreExists(MovieGenre movieGenre)
    {
        return await dbContext.MovieGenres.AnyAsync(x =>
            x.MovieId == movieGenre.MovieId && x.GenreId == movieGenre.GenreId);
    }
}