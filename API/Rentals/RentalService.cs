using API.OneOfTypes;
using DTO.Rentals;
using DTO.SearchQueries;
using Models;
using OneOf;
using OneOf.Types;
using Repositories.Interfaces;

namespace API.Rentals
{
    public class RentalService(IRentalRepository rentalRepo, IStockRepository stockRepo, IMovieRepository movieRepo, IUserRepository userRepo)
    {
        public async Task<List<RentalResponse>> GetAllRentals(RentalSearchQuery rentalSearch)
        {
            var rentals = await rentalRepo.Search(rentalSearch);

            var rentalsDto = rentals.Select(x => new RentalResponse
            {
                Id = x.Id,
                MovieId = x.MovieId,
                UserId = x.UserId,
                DateRented = x.DateRented,
                DateReturned = x.DateReturned,
            }).ToList();

            return rentalsDto;
        }

        public async Task<OneOf<Success, MovieNotFound, UserNotFound, MovieOutOfStock>> RentMovie(RentRequest request)
        {
            var movieExists = await movieRepo.MovieExistsAsync(request.MovieId);
            if (!movieExists)
                return new MovieNotFound();

            var userExists = await userRepo.UserExistsAsync(request.UserId);
            if (!userExists)
                return new UserNotFound();

            var stock = await stockRepo.GetByIdAsync(request.MovieId);
            if (stock == null || stock.Amount <= 0)
                return new MovieOutOfStock();

            // What if 2 users decrease same movie, possible off by 1
            stock.Amount -= 1;

            var rental = new Rental
            {
                MovieId = request.MovieId,
                UserId = request.UserId,
                DateRented = request.DateRented ?? DateOnly.FromDateTime(DateTime.Now),
                DateReturned = null
            };

            await rentalRepo.CreateAsync(rental);
            await rentalRepo.SaveChangesAsync();
            return new Success();
        }

        public async Task<OneOf<Success, RentalNotFound>> ReturnMovie(ReturnRequest request)
        {
            var rental = await rentalRepo.GetByIdAsync(request.RentalId);

            if (rental == null)
                return new RentalNotFound();

            rental.DateReturned = request.DateReturned ?? DateOnly.FromDateTime(DateTime.Now);

            var stock = await stockRepo.GetByIdAsync(rental.MovieId);
            if (stock == null)
                throw new Exception("This should not be reachable");

            // What if 2 users increase same movie, possible off by 1
            stock.Amount += 1;

            await stockRepo.SaveChangesAsync();
            return new Success();
        }
    }
}
