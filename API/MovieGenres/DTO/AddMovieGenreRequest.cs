namespace API.MovieGenres.DTO;

public class AddMovieGenreRequest
{
    public Guid MovieId { get; set; }
    public Guid GenreId { get; set; }
}