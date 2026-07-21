using Models;
using Repositories;

namespace API.MovieGenres.Repositories;

public interface IMovieGenreRepository : IRepositoryBase<MovieGenre>
{
    public Task<bool> MovieGenreExists(MovieGenre movieGenre);
}