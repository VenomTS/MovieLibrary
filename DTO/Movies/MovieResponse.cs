namespace DTO.Movies;

public class MovieResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly ReleaseDate { get; set; }
    public List<MovieGenreResponse> Genres { get; set; } = [];
    public int Stock { get; set; }
}