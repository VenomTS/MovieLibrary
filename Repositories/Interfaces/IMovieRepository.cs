using DTO.SearchQueries;
using Models;
using Repositories;

namespace Repositories.Interfaces;

public interface IMovieRepository : IRepositoryBase<Movie>
{
    public Task<Movie?> GetByIdAsync(Guid id);
    public Task<IEnumerable<Movie>> Search(MovieSearchQuery query);
    public Task<bool> MovieExistsAsync(Movie movie);
    public Task<bool> MovieExistsAsync(Guid movieId);
}