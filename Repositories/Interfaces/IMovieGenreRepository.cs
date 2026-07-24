using Models;
using Repositories;

namespace Repositories.Interfaces;

public interface IMovieGenreRepository : IRepositoryBase<MovieGenre>
{
    public Task<bool> MovieGenreExists(MovieGenre movieGenre);
    public Task<List<MovieGenre>> GetByMovieId(Guid movieId);
}