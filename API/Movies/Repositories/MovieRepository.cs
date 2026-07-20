using API.Database;
using Microsoft.EntityFrameworkCore;

namespace API.Movies.Repositories;

public class MovieRepository(AppDbContext dbContext) : IMovieRepository
{
    public async Task<IEnumerable<Movie>> GetMoviesAsync(SearchQuery query)
    {
        var movies = dbContext.Movies.AsQueryable();
        
        // Improve-able
        if(!string.IsNullOrWhiteSpace(query.Name))
            movies = movies.Where(x => x.Name.Contains(query.Name));
        
        return await movies.ToListAsync();
    }

    public async Task<IEnumerable<Movie>> GetAvailableMoviesAsync(SearchQuery query)
    {
        throw new NotImplementedException();
    }

    public async Task<Movie?> GetMovieByIdAsync(Guid movieId)
    {
        var movie = await dbContext.Movies.FindAsync(movieId);
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