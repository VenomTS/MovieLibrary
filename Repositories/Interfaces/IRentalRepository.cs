using DTO.SearchQueries;
using Models;
using Repositories;

namespace API.Rentals.Repositories
{
    public interface IRentalRepository : IRepositoryBase<Rental>
    {
        public Task<IEnumerable<Rental>> Search(RentalSearchQuery query);
    }
}
