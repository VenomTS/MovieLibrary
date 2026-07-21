using API.Database;
using Microsoft.EntityFrameworkCore;

namespace API.Rentals.Repositories
{
    public class RentalRepository(AppDbContext dbContext) : IRentalRepository
    {
        public async Task AddRental(Rental rental)
        {
            await dbContext.Rentals.AddAsync(rental);
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<Rental>> GetAllRentals(RentalSearch rentalSearch)
        {
            var rentals = dbContext.Rentals.AsQueryable();

            if (rentalSearch.MovieId != null)
                rentals = rentals.Where(x => x.MovieId == rentalSearch.MovieId.Value);

            if (rentalSearch.UserId != null)
                rentals = rentals.Where(x => x.UserId == rentalSearch.UserId.Value);

            if (rentalSearch.DateRented != null)
                rentals = rentals.Where(x => x.DateRented == rentalSearch.DateRented.Value);

            return await rentals.ToListAsync();
        }

        public async Task<Rental?> GetRentalById(Guid id)
        {
            var rental = await dbContext.Rentals.FirstOrDefaultAsync(x => x.Id == id);
            return rental;
        }

        public async Task UpdateRental(Guid rentalId, Rental newRental)
        {
            var existingRental = await GetRentalById(rentalId);
            if (existingRental == null)
                return;

            existingRental.DateReturned = newRental.DateReturned;
            await dbContext.SaveChangesAsync();
        }
    }
}
