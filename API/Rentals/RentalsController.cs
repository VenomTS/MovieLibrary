using DTO.Rentals;
using DTO.SearchQueries;
using Microsoft.AspNetCore.Mvc;

namespace API.Rentals
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RentalsController(RentalService rentalService) : ControllerBase
    {
        // Post = Rent; Patch = Return
        [HttpPost]
        public async Task<IActionResult> RentMovie([FromBody] RentRequest request)
        {
            var result = await rentalService.RentMovie(request);

            return result.Match<IActionResult>(
                _ => NoContent(),
                _ => NotFound("Movie not found"),
                _ => NotFound("AppUser not found"),
                _ => Ok("Movie out of stock"));
        }

        [HttpPatch]
        public async Task<IActionResult> ReturnMovie([FromBody] ReturnRequest request)
        {
            var result = await rentalService.ReturnMovie(request);

            return result.Match<IActionResult>(
                _ => NoContent(),
                _ => NotFound("Rental not found"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRentals([FromQuery] RentalSearchQuery rentalSearch)
        {
            var result = await rentalService.GetAllRentals(rentalSearch);
            return Ok(result);
        }
    }
}
