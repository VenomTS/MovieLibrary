namespace API.Stocks.Repositories;

public interface IStockRepository
{
    public Task<Stock?> GetByIdAsync(Guid movieId);
    public Task AddAsync(Stock stock);
    public Task UpdateAsync(Guid id, Stock updatedStock);
}