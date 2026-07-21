namespace DTO.SearchQueries;

public class MovieSearchQuery
{
    public string? Name { get; set; }
    public bool? IsAvailable { get; set; }
    public List<string> Genres { get; set; } = [];
}