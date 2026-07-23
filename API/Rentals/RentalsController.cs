using DTO.Rentals;
using DTO.SearchQueries;
using Microsoft.AspNetCore.Mvc;

namespace API.Rentals;

[ApiController]
[Route("api/v1/[controller]")]
public class RentalsController(RentalService rentalService) : ControllerBase
{
    // Post = Rent; Patch = Return
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await rentalService.GetByIdAsync(id);

        return result.Match<IActionResult>(
            Ok,
            _ => NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Detail = $"The rental with ID {id} was not found",
            }));
    }
    
    [HttpPost]
    public async Task<IActionResult> RentMovie([FromBody] RentRequest request)
    {
        var result = await rentalService.RentMovie(request);

        return result.Match<IActionResult>(
            rental => CreatedAtAction(nameof(GetById), new { id = rental.Id }, rental),
            _ => NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Detail = $"The movie with ID {request.MovieId} was not found",
            }),
            _ => NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Detail = $"The user with ID {request.UserId} was not found",
            }),
            _ => NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Detail = $"The movie is out of stock",
            }));
    }

    [HttpPatch]
    public async Task<IActionResult> ReturnMovie([FromBody] ReturnRequest request)
    {
        var result = await rentalService.ReturnMovie(request);

        return result.Match<IActionResult>(
            Ok,
            _ => NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Detail = $"The rental with ID {request.RentalId} was not found",
            }));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRentals([FromQuery] RentalSearchQuery rentalSearch)
    {
        var result = await rentalService.GetAllRentals(rentalSearch);
        return Ok(result);
    }
}