using DTO.Rentals;
using DTO.SearchQueries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Rentals;

[ApiController]
[Route("api/v1/[controller]")]
public class RentalsController(RentalService rentalService) : ControllerBase
{
    // Post = Rent; Patch = Return
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Librarian")]
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

    [HttpGet("user/{userId:guid}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetByUserId([FromRoute] Guid userId)
    {
        var result = await rentalService.GetUnreturnedByUserId(userId, User);

        return result.Match<IActionResult>(
            Ok,
            _ => Unauthorized(),
            _ => NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Detail = $"The user with ID {userId} was not found",
            }));
    }
    
    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> RentMovie([FromBody] RentRequest request)
    {
        var result = await rentalService.RentMovie(request, User);

        return result.Match<IActionResult>(
            rental => CreatedAtAction(nameof(GetById), new { id = rental.Id }, rental),
            _ => Unauthorized(),
            _ => BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Detail = $"User is already renting selected movie",
            }),
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

    [HttpPatch("{rentalId:guid}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> ReturnMovie([FromRoute] Guid rentalId, [FromBody] ReturnRequest request)
    {
        var result = await rentalService.ReturnMovie(rentalId, request, User);

        return result.Match<IActionResult>(
            Ok,
            _ => Unauthorized(),
            _ => NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Detail = $"The rental with ID {rentalId} was not found",
            }));
    }

    [HttpGet]
    [Authorize(Roles = "Librarian")]
    public async Task<IActionResult> GetAllRentals([FromQuery] RentalSearchQuery rentalSearch)
    {
        var result = await rentalService.GetAllRentals(rentalSearch);
        return Ok(result);
    }
}