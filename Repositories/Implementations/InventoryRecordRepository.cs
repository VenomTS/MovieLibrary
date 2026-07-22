using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Database;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class InventoryRecordRepository(AppDbContext dbContext) : RepositoryBase<InventoryRecord>(dbContext), IInventoryRecordRepository
    {
        public async Task<List<InventoryRecord>> GetInventoryRecordsByMovieId(Guid movieId)
        {
            var records = await dbContext.InventoryRecords.Where(x => x.MovieId == movieId).ToListAsync();
            return records;
        }
    }
}
