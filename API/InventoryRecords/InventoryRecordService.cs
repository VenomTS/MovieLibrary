using API.OneOfTypes;
using AutoMapper;
using DTO.InventoryRecords;
using Models;
using OneOf;
using OneOf.Types;
using Repositories.Interfaces;

namespace API.InventoryRecords
{
    public class InventoryRecordService(IMapper mapper, IInventoryRecordRepository inventoryRepo, IMovieRepository movieRepo)
    {
        public async Task<OneOf<InventoryRecordSingularResponse, NotFound>> GetByIdAsync(Guid id)
        {
            var record = await inventoryRepo.GetByIdAsync(id);
            if (record == null)
                return new NotFound();
            
            return mapper.Map<InventoryRecordSingularResponse>(record);
        }
        
        public async Task<OneOf<InventoryRecordSingularResponse, MovieNotFound>> AddAsync(CreateInventoryRecordRequest request)
        {
            var movieExists = await movieRepo.MovieExistsAsync(request.MovieId);
            if (!movieExists)
                return new MovieNotFound();
            
            var inventoryRecord = mapper.Map<InventoryRecord>(request);

            await inventoryRepo.CreateAsync(inventoryRecord);
            await inventoryRepo.SaveChangesAsync();
            
            return mapper.Map<InventoryRecordSingularResponse>(inventoryRecord);
        }

        public async Task<List<InventoryRecordResponse>> GetAllAsync()
        {
            var records = await inventoryRepo.GetAllAsync();

            // No clue how to convert this into AutoMapper
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
            
            // Same here
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
