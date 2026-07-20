using API.Genres.DTO;
using Microsoft.AspNetCore.Mvc;

namespace API.Genres;

[ApiController]
[Route("api/v1/[controller]")]
public class GenresController(GenreService genreService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateGenre([FromBody] CreateGenreRequest request)
    {
        var result = await genreService.CreateGenre(request);

        return result.Match<IActionResult>(
            _ => NoContent(),
            _ => Conflict("Genre already exists"));
    }
}