namespace API.MovieGenres.DTO;

public class RemoveMovieGenreRequest
{
    public Guid MovieId { get; set; }
    public Guid GenreId { get; set; }
}