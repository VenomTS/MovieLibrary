using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Database;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class InventoryRecordRepository(AppDbContext dbContext) : RepositoryBase<InventoryRecord>(dbContext), IInventoryRecordRepository
    {
        public async Task<List<InventoryRecord>> GetByMovieId(Guid movieId)
        {
            var records = await dbContext.InventoryRecords.Where(x => x.MovieId == movieId).ToListAsync();
            return records;
        }

        public async Task<int> GetTotalAmount(Guid movieId, DateOnly startDate, DateOnly endDate)
        {
            var records = await dbContext.InventoryRecords.Where(x => x.MovieId == movieId && x.Date >= startDate && x.Date <= endDate).SumAsync(x => x.Amount);

            return records;
        }
    }
}
