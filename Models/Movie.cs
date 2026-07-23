namespace Models;

public class Movie
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly ReleaseDate { get; set; }

    public List<Genre> Genres { get; set; } = [];
}