using API.OneOfTypes;
using DTO.Stocks;
using OneOf;
using OneOf.Types;
using Repositories.Interfaces;

namespace API.Stocks;

public class StockService(IStockRepository stockRepo)
{
    public async Task<OneOf<Success, StockNotFound>> UpdateStock(UpdateStockRequest request)
    {
        var stock = await stockRepo.GetByIdAsync(request.MovieId);
        if (stock == null)
            return new StockNotFound();

        stock.Amount = request.NewStockAmount;
        await stockRepo.UpdateAsync(stock.MovieId, stock);
        return new Success();
    }
}