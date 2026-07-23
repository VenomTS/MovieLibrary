using DTO.SearchQueries;
using Models;
using Repositories;

namespace Repositories.Interfaces
{
    public interface IRentalRepository : IRepositoryBase<Rental>
    {
        public Task<IEnumerable<Rental>> Search(RentalSearchQuery query);
        public Task<List<Rental>> GetByMovieIdAsync(Guid movieId);
    }
}
