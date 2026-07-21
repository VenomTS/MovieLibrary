namespace DTO.Movies;

public class AddMovieRequest
{
    public string Name { get; set; } = string.Empty;
    public DateOnly ReleaseDate { get; set; }
}