using DTO.SearchQueries;
using Microsoft.EntityFrameworkCore;
using Models;
using Repositories;
using Repositories.Databasee;

namespace API.Rentals.Repositories
{
    public class RentalRepository(AppDbContext dbContext) : RepositoryBase<Rental>(dbContext), IRentalRepository
    {
        public async Task<IEnumerable<Rental>> Search(RentalSearchQuery query)
        {
            var rentals = dbContext.Rentals.Include(x => x.Movie).Include(x => x.User).AsQueryable();

            if (query.UserId != null)
                rentals = rentals.Where(x => x.UserId == query.UserId);

            if (query.MovieId != null)
                rentals = rentals.Where(x => x.MovieId == query.MovieId);

            if (query.DateRented != null)
                rentals = rentals.Where(x => x.DateRented == query.DateRented);

            return await rentals.ToListAsync();
        }
    }
}
