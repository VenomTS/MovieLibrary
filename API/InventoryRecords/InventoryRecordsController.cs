using DTO.InventoryRecords;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.InventoryRecords
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class InventoryRecordsController(InventoryRecordService inventoryService) : ControllerBase
    {
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await inventoryService.GetByIdAsync(id);

            return result.Match<IActionResult>(
                Ok,
                _ => NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"The inventory record with ID {id} was not found",
                }));
        }
        
        [HttpGet]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetAll()
        {
            var result = await inventoryService.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("movie/{movieId:guid}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetByMovieId([FromRoute] Guid movieId)
        {
            var result = await inventoryService.GetByMovieId(movieId);

            return result.Match<IActionResult>(
                Ok,
                _ => NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"The movie with ID {movieId} was not found",
                }));
        }

        [HttpPost]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Create(CreateInventoryRecordRequest request)
        {
            var result = await inventoryService.AddAsync(request);

            return result.Match<IActionResult>(
                inventoryRecord => CreatedAtAction(nameof(GetById), new { id = inventoryRecord.Id }, inventoryRecord),
                _ => NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"The movie with ID {request.MovieId} was not found",
                }));
        }
    }
}
