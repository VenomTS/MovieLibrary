using Models;

namespace Repositories.Interfaces;

public interface IStockRepository : IRepositoryBase<Stock>
{
    public Task<Stock?> GetByIdAsync(Guid id);
}