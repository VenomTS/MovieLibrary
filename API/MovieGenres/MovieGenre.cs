using API.Genres;
using API.Movies;

namespace API.MovieGenres;

public class MovieGenre
{
    public Guid MovieId { get; set; }
    public Guid GenreId { get; set; }

    public Movie Movie { get; set; }
    public Genre Genre { get; set; }
}