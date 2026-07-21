using Microsoft.EntityFrameworkCore;
using Models;
using Repositories;
using Repositories.Databasee;

namespace API.MovieGenres.Repositories;

public class MovieGenreRepository(AppDbContext dbContext) : RepositoryBase<MovieGenre>(dbContext), IMovieGenreRepository
{
    public async Task<bool> MovieGenreExists(MovieGenre movieGenre)
    {
        return await dbContext.MovieGenres.AnyAsync(x =>
            x.MovieId == movieGenre.MovieId && x.GenreId == movieGenre.GenreId);
    }
}