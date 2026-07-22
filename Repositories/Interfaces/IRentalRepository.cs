using DTO.SearchQueries;
using Models;
using Repositories;

namespace Repositories.Interfaces
{
    public interface IRentalRepository : IRepositoryBase<Rental>
    {
        public Task<Rental?> GetByIdAsync(Guid id);
        public Task<IEnumerable<Rental>> Search(RentalSearchQuery query);
    }
}
