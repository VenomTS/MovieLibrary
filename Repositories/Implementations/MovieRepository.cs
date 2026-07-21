using DTO.SearchQueries;
using Microsoft.EntityFrameworkCore;
using Models;
using Repositories;
using Repositories.Databasee;

namespace API.Movies.Repositories;

public class MovieRepository(AppDbContext dbContext) : RepositoryBase<Movie>(dbContext), IMovieRepository
{
    public async Task<IEnumerable<Movie>> Search(MovieSearchQuery query)
    {
        var movies = dbContext.Movies.Include(x => x.Genres).Include(x => x.Stock).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Name))
            movies = movies.Where(x => x.Name.Contains(query.Name));

        if (query.IsAvailable != null && query.IsAvailable.Value)
            movies = movies.Where(x => x.Stock.Amount > 0);

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