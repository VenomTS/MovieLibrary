using API.Stocks.DTO;
using Microsoft.AspNetCore.Mvc;

namespace API.Stocks;

[ApiController]
[Route("api/v1/[controller]")]
public class StocksController(StockService stockService) : ControllerBase
{
    [HttpPatch]
    public async Task<IActionResult> UpdateStock(UpdateStockRequest request)
    {
        var result = await stockService.UpdateStock(request);

        return result.Match<IActionResult>(
            _ => NoContent(),
            _ => NotFound("Cannot update stock because movie does not exist in database"));
    }
}