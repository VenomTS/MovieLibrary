using API.Movies.Repositories;
using API.OneOfTypes;
using API.Rentals.Repositories;
using API.Stocks.Repositories;
using API.Users.Repository;
using DTO.Rentals;
using DTO.SearchQueries;
using OneOf;
using OneOf.Types;

namespace API.Rentals
{
    public class RentalService(IRentalRepository rentalRepo, IStockRepository stockRepo, IMovieRepository movieRepo, IUserRepository userRepo)
    {
        public async Task<List<RentalResponse>> GetAllRentals(RentalSearchQuery rentalSearch)
        {
            var rentals = await rentalRepo.GetAllRentals(rentalSearch);

            var rentalsDto = rentals.Select(x => new RentalResponse
            {
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
            // It should never be null since it is created when movie is added to the DB
            if (stock == null || stock.Amount <= 0)
                return new MovieOutOfStock();

            // What if 2 users decrease same movie, possible off by 1
            stock.Amount -= 1;
            await stockRepo.UpdateAsync(request.MovieId, stock);

            var rental = new Rental
            {
                MovieId = request.MovieId,
                UserId = request.UserId,
                DateRented = request.DateRented == null ? DateOnly.FromDateTime(DateTime.Now) : request.DateRented.Value,
                DateReturned = null
            };

            await rentalRepo.AddRental(rental);
            return new Success();
        }

        public async Task<OneOf<Success, RentalNotFound>> ReturnMovie(ReturnRequest request)
        {
            var rental = await rentalRepo.GetRentalById(request.RentalId);

            if (rental == null)
                return new RentalNotFound();

            rental.DateReturned = request.DateReturned == null ? DateOnly.FromDateTime(DateTime.Now) : request.DateReturned.Value;
            await rentalRepo.UpdateRental(request.RentalId, rental);

            var stock = await stockRepo.GetByIdAsync(rental.MovieId);
            if (stock == null)
                throw new Exception("This should not be reachable");

            // What if 2 users increase same movie, possible off by 1
            stock.Amount += 1;
            await stockRepo.UpdateAsync(stock.MovieId, stock);
            return new Success();
        }
    }
}
