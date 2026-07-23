using DTO.Movies;
using DTO.SearchQueries;
using Microsoft.AspNetCore.Mvc;

namespace API.Movies;

[ApiController]
[Route("api/v1/[controller]")]
public class MoviesController(MovieService movieService) : ControllerBase
{

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await movieService.GetMovieByIdAsync(id);

        return result.Match<IActionResult>(
            Ok,
            _ => NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Detail = $"The movie with ID {id} was not found",
            }));
    }
    
    [HttpPost]
    public async Task<IActionResult> AddMovie([FromBody] AddMovieRequest request)
    {
        var result = await movieService.AddMovieAsync(request);

        return result.Match<IActionResult>(
            movie => CreatedAtAction(nameof(GetById), new { id = movie.Id }, movie),
            _ => NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Detail = $"The specified movie already exists",
            }));
    }

    [HttpGet]
    public async Task<IActionResult> GetMovies([FromQuery] MovieSearchQuery query)
    {
        var result = await movieService.GetMoviesAsync(query);

        return Ok(result);
    }
}