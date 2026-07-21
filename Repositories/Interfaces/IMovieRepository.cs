using DTO.SearchQueries;
using Models;
using Repositories;

namespace API.Movies.Repositories;

public interface IMovieRepository : IRepositoryBase<Movie>
{
    public Task<IEnumerable<Movie>> Search(MovieSearchQuery query);
    public Task<bool> MovieExistsAsync(Movie movie);
    public Task<bool> MovieExistsAsync(Guid movieId);
}