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
