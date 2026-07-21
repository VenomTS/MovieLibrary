using DTO.Movies;
using DTO.SearchQueries;
using Microsoft.AspNetCore.Mvc;

namespace API.Movies;

[ApiController]
[Route("api/v1/[controller]")]
public class MoviesController(MovieService movieService) : ControllerBase
{

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetMovieById(Guid id)
    {
        var result = await movieService.GetMovieByIdAsync(id);

        return result.Match<IActionResult>(
            _ => Ok(result.AsT0),
            _ => NotFound("Movie not found"));
    }
    
    [HttpPost]
    public async Task<IActionResult> AddMovie([FromBody] AddMovieRequest request)
    {
        var result = await movieService.AddMovieAsync(request);

        return result.Match<IActionResult>(
            _ => CreatedAtAction(nameof(GetMovieById), new { id = result.AsT0.Id }, result.AsT0),
            _ => Conflict("Movie already exists"));
    }

    [HttpGet]
    public async Task<IActionResult> GetMovies([FromQuery] MovieSearchQuery query)
    {
        var result = await movieService.GetMoviesAsync(query);

        return Ok(result);
    }
}