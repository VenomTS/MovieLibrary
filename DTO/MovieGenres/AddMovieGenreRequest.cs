namespace DTO.MovieGenres;

public class AddMovieGenreRequest
{
    public Guid MovieId { get; set; }
    public Guid GenreId { get; set; }
}