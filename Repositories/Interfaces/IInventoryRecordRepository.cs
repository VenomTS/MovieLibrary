using Models;

namespace Repositories.Interfaces
{
    public interface IInventoryRecordRepository : IRepositoryBase<InventoryRecord>
    {
        public Task<List<InventoryRecord>> GetByMovieId(Guid movieId);
        public Task<int> GetTotalAmount(Guid movieId, DateOnly untilDate);
    }
}
