using API.OneOfTypes;
using DTO.InventoryRecords;
using Models;
using OneOf;
using OneOf.Types;
using Repositories.Interfaces;
using System.Runtime.InteropServices;

namespace API.InventoryRecords
{
    public class InventoryRecordService(IInventoryRecordRepository inventoryRepo, IMovieRepository movieRepo)
    {
        public async Task<OneOf<Success, MovieNotFound>> AddAsync(CreateInventoryRecordRequest request)
        {
            var movieExists = await movieRepo.MovieExistsAsync(request.MovieId);
            if (!movieExists)
                return new MovieNotFound();

            var inventoryRecord = new InventoryRecord
            {
                MovieId = request.MovieId,
                Amount = request.Amount,
                Date = request.Date == null ? DateOnly.FromDateTime(DateTime.Now) : request.Date.Value
            };

            await inventoryRepo.CreateAsync(inventoryRecord);
            await inventoryRepo.SaveChangesAsync();
            return new Success();
        }

        public async Task<List<InventoryRecordResponse>> GetAllAsync()
        {
            var records = await inventoryRepo.GetAllAsync();

            var recordsDto = records.GroupBy(x => x.MovieId).Select(
                x => new InventoryRecordResponse
                {
                    MovieId = x.Key,
                    InventoryRecordData = x.GroupBy(y => y.Date).Select(y => new InventoryRecordDataResponse
                    {
                        Date = y.Key,
                        Amount = y.Sum(z => z.Amount)
                    }).ToList()
                }
                );

            return recordsDto.ToList();
        }
    }
}
