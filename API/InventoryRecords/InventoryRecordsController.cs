using DTO.InventoryRecords;
using Microsoft.AspNetCore.Mvc;

namespace API.InventoryRecords
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class InventoryRecordsController(InventoryRecordService inventoryService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await inventoryService.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("{movieId:guid}")]
        public async Task<IActionResult> GetByMovieId([FromRoute] Guid movieId)
        {
            var result = await inventoryService.GetByMovieId(movieId);

            return result.Match<IActionResult>(
                Ok,
                _ => NotFound("Movie not found"));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateInventoryRecordRequest request)
        {
            var result = await inventoryService.AddAsync(request);

            return result.Match<IActionResult>(
                _ => NoContent(),
                _ => NotFound("Movie not found"));
        }
    }
}
