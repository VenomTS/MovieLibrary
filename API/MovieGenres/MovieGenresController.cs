using API.MovieGenres.DTO;
using Microsoft.AspNetCore.Mvc;

namespace API.MovieGenres;

[ApiController]
[Route("api/v1/[controller]")]
public class MovieGenresController(MovieGenreService movieGenreService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AssignGenre([FromBody] AddMovieGenreRequest request)
    {
        var result = await movieGenreService.AddMovieGenre(request);

        return result.Match<IActionResult>(
            _ => NoContent(),
            _ => NotFound("Movie not found"),
            _ => NotFound("Genre not found"),
            _ => Conflict("Movie is already that genre"));
    }
}