using DTO.Movies;
using DTO.SearchQueries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Movies;

[ApiController]
[Route("api/v1/[controller]")]
public class MoviesController(MovieService movieService) : ControllerBase
{

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Customer")]
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
    [Authorize(Roles = "Librarian")]
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
    [Authorize(Roles = "Customer, Librarian")]
    public async Task<IActionResult> GetMovies([FromQuery] MovieSearchQuery query)
    {
        var result = await movieService.GetMoviesAsync(query);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Librarian")]
    public async Task<IActionResult> UpdateMovie([FromRoute] Guid id, [FromBody] UpdateMovieRequest request)
    {
        var result = await movieService.UpdateAsync(id, request);

        return result.Match<IActionResult>(
            Ok,
            _ => Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Detail = $"The {request.Name} with release date {request.ReleaseDate} already exists",
            }),
            _ => NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Detail = $"The movie with ID {id} was not found",
            }));
    }
}