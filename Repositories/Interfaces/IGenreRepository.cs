using Models;
using Repositories;

namespace API.Genres.Repositories;

public interface IGenreRepository : IRepositoryBase<Genre>
{
    public Task<bool> GenreExistsAsync(string name);
    public Task<bool> GenreExistsAsync(Guid id);
}