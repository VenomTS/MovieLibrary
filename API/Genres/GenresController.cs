using DTO.Genres;
using Microsoft.AspNetCore.Mvc;

namespace API.Genres;

[ApiController]
[Route("api/v1/[controller]")]
public class GenresController(GenreService genreService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await genreService.GetByIdAsync(id);

        return result.Match<IActionResult>(
            Ok,
            _ => NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Detail = $"The genre with ID {id} was not found",
            }));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateGenre([FromBody] CreateGenreRequest request)
    {
        var result = await genreService.CreateGenre(request);

        return result.Match<IActionResult>(
            genre => CreatedAtAction(nameof(GetById), new { genre.Id }, genre),
            _ => Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Detail = $"The {request.Name} genre already exists",
            }));
    }
}