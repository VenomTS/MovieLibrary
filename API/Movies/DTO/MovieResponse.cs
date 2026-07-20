namespace API.Movies.DTO;

public class MovieResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly ReleaseDate { get; set; }
}