using API.Database;
using Microsoft.EntityFrameworkCore;

namespace API.Stocks.Repositories;

public class StockRepository(AppDbContext dbContext) : IStockRepository
{
    public async Task<Stock?> GetByIdAsync(Guid movieId)
    {
        var stock = await dbContext.Stocks.FirstOrDefaultAsync(x => x.MovieId == movieId);
        return stock;
    }

    public async Task AddAsync(Stock stock)
    {
        await dbContext.Stocks.AddAsync(stock);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, Stock updatedStock)
    {
        var existingStock = await GetByIdAsync(id);
        if (existingStock == null)
            return;
        
        existingStock.Amount = updatedStock.Amount;
        await dbContext.SaveChangesAsync();
    }
}