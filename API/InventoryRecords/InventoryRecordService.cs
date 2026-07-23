using API.OneOfTypes;
using DTO.InventoryRecords;
using Models;
using OneOf;
using OneOf.Types;
using Repositories.Interfaces;

namespace API.InventoryRecords
{
    public class InventoryRecordService(IInventoryRecordRepository inventoryRepo, IMovieRepository movieRepo)
    {
        public async Task<OneOf<InventoryRecordSingularResponse, NotFound>> GetByIdAsync(Guid id)
        {
            var record = await inventoryRepo.GetByIdAsync(id);
            if (record == null)
                return new NotFound();

            return new InventoryRecordSingularResponse()
            {
                Id = record.Id,
                MovieId = record.MovieId,
                Amount = record.Amount,
                Date = record.Date
            };
        }
        
        public async Task<OneOf<InventoryRecordSingularResponse, MovieNotFound>> AddAsync(CreateInventoryRecordRequest request)
        {
            var movieExists = await movieRepo.MovieExistsAsync(request.MovieId);
            if (!movieExists)
                return new MovieNotFound();

            var inventoryRecord = new InventoryRecord
            {
                MovieId = request.MovieId,
                Amount = request.Amount,
                Date = request.Date ?? DateOnly.FromDateTime(DateTime.Now)
            };

            await inventoryRepo.CreateAsync(inventoryRecord);
            await inventoryRepo.SaveChangesAsync();

            return new InventoryRecordSingularResponse
            {
                MovieId = inventoryRecord.MovieId,
                Amount = inventoryRecord.Amount,
                Date = inventoryRecord.Date
            };
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

        public async Task<OneOf<List<InventoryRecordDataResponse>, MovieNotFound>> GetByMovieId(Guid movieId)
        {
            var movieExists = await movieRepo.MovieExistsAsync(movieId);
            if (!movieExists)
                return new MovieNotFound();

            var records = await inventoryRepo.GetByMovieId(movieId);

            var recordsDto = records.GroupBy(x => x.Date)
                .Select(x => new InventoryRecordDataResponse
                {
                    Date = x.Key,
                    Amount = x.Sum(y => y.Amount),
                }).ToList();

            return recordsDto;
        }
    }
}
