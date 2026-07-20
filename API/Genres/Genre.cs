using API.Movies;

namespace API.Genres;

public class Genre
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<Movie> Movies { get; set; } = [];
}