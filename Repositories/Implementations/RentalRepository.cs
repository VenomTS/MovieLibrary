using DTO.SearchQueries;
using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Database;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class RentalRepository(AppDbContext dbContext) : RepositoryBase<Rental>(dbContext), IRentalRepository
    {
        private readonly AppDbContext dbContext = dbContext;

        public async Task<List<Rental>> GetByMovieIdAsync(Guid movieId)
        {
            return await dbContext.Rentals.Where(x => x.MovieId == movieId).ToListAsync();
        }

        public async Task<IEnumerable<Rental>> Search(RentalSearchQuery query)
        {
            var rentals = dbContext.Rentals.Include(x => x.Movie).Include(x => x.AppUser).AsQueryable();

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
