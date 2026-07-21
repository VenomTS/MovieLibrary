using API.Movies.DTO;

namespace API.Movies;

public class SearchQuery
{
    public string? Name { get; set; }
    public bool? IsAvailable { get; set; }
    public List<string> Genres { get; set; } = [];
}