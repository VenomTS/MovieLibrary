using DTO.SearchQueries;
using Microsoft.EntityFrameworkCore;
using Models;
using Repositories;
using Repositories.Database;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class MovieRepository(AppDbContext dbContext) : RepositoryBase<Movie>(dbContext), IMovieRepository
{
    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        return await dbContext.Movies.Include(x => x.Genres).FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task<IEnumerable<Movie>> Search(MovieSearchQuery query)
    {
        var movies = dbContext.Movies.Include(x => x.Genres).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Name))
            movies = movies.Where(x => x.Name.Contains(query.Name));

        foreach (var genre in query.Genres)
            movies = movies.Where(x => x.Genres.Any(g => g.Name == genre));

        return await movies.ToListAsync();
    }

    public async Task<bool> MovieExistsAsync(Movie movie)
    {
        // For 2 movies to be the same, they MUST have the same name and release date
        return await dbContext.Movies.AnyAsync(x => x.Name == movie.Name && 
                                                    x.ReleaseDate == movie.ReleaseDate);
    }

    public async Task<bool> MovieExistsAsync(Guid movieId)
    {
        return await dbContext.Movies.AnyAsync(x => x.Id == movieId);
    }
}