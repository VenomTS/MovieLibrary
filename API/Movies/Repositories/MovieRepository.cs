using API.Database;
using Microsoft.EntityFrameworkCore;

namespace API.Movies.Repositories;

public class MovieRepository(AppDbContext dbContext) : IMovieRepository
{
    public async Task<IEnumerable<Movie>> GetMoviesAsync(SearchQuery query)
    {
        var movies = dbContext.Movies.Include(x => x.Genres).Include(x => x.Stock).AsQueryable();
        
        // Improve-able
        if(!string.IsNullOrWhiteSpace(query.Name))
            movies = movies.Where(x => x.Name.Contains(query.Name));
        
        return await movies.ToListAsync();
    }

    public async Task<IEnumerable<Movie>> GetAvailableMoviesAsync(SearchQuery query)
    {
        var movies = dbContext.Movies.Include(x => x.Genres).Include(x => x.Stock).AsQueryable();

        movies = movies.Where(x => x.Stock.Amount > 0);
        
        if(!string.IsNullOrWhiteSpace(query.Name))
            movies = movies.Where(x => x.Name.Contains(query.Name));

        return await movies.ToListAsync();
    }

    public async Task<Movie?> GetMovieByIdAsync(Guid movieId)
    {
        var movie = await dbContext.Movies.Include(x => x.Genres).Include(x => x.Stock).FirstOrDefaultAsync(x => x.Id == movieId);
        return movie;
    }

    public async Task<Movie> AddMovieAsync(Movie movie)
    {
        await dbContext.Movies.AddAsync(movie);
        await dbContext.SaveChangesAsync();
        return movie;
    }

    public async Task<bool> MovieExistsAsync(Movie movie)
    {
        // For 2 movies to be the same, they MUST have the same name and release date
        return await dbContext.Movies.AnyAsync(x => x.Name == movie.Name && 
                                                    x.ReleaseDate == movie.ReleaseDate);
    }
}