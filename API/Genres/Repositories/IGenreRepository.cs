namespace API.Genres.Repositories;

public interface IGenreRepository
{
    public Task<IEnumerable<Genre>> GetAllAsync();
    public Task<Genre?> GetByIdAsync(Guid id);
    public Task<Genre> CreateAsync(Genre genre);
    public Task<bool> GenreExistsAsync(string name);
}