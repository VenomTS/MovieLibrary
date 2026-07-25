using API.OneOfTypes;
using AutoMapper;
using DTO.InventoryRecords;
using Models;
using OneOf;
using OneOf.Types;
using Repositories;
using Repositories.Interfaces;

namespace API.InventoryRecords
{
    public class InventoryRecordService(IMapper mapper, IRepositoryManager repositoryManager)
    {
        public async Task<OneOf<InventoryRecordSingularResponse, NotFound>> GetByIdAsync(Guid id)
        {
            var record = await repositoryManager.InventoryRecords.GetByIdAsync(id);
            if (record == null)
                return new NotFound();
            
            return mapper.Map<InventoryRecordSingularResponse>(record);
        }
        
        public async Task<OneOf<InventoryRecordSingularResponse, MovieNotFound>> AddAsync(CreateInventoryRecordRequest request)
        {
            var movieExists = await repositoryManager.Movies.MovieExistsAsync(request.MovieId);
            if (!movieExists)
                return new MovieNotFound();
            
            var inventoryRecord = mapper.Map<InventoryRecord>(request);

            await repositoryManager.InventoryRecords.CreateAsync(inventoryRecord);
            await repositoryManager.InventoryRecords.SaveChangesAsync();
            
            return mapper.Map<InventoryRecordSingularResponse>(inventoryRecord);
        }

        public async Task<List<InventoryRecordResponse>> GetAllAsync()
        {
            var records = await repositoryManager.InventoryRecords.GetAllAsync();

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
            var movieExists = await repositoryManager.Movies.MovieExistsAsync(movieId);
            if (!movieExists)
                return new MovieNotFound();

            var records = await repositoryManager.InventoryRecords.GetByMovieId(movieId);
            
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
