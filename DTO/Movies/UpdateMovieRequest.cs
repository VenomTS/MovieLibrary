namespace DTO.Movies;

public class UpdateMovieRequest
{
    public string Name { get; set; } = string.Empty;
    public DateOnly ReleaseDate { get; set; }
    public List<Guid> GenreIds { get; set; } = [];

}