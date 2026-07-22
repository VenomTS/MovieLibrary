using Models;

namespace Repositories.Interfaces
{
    public interface IInventoryRecordRepository : IRepositoryBase<InventoryRecord>
    {
        public Task<List<InventoryRecord>> GetInventoryRecordsByMovieId(Guid movieId);
    }
}
