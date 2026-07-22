using API.OneOfTypes;
using DTO.Rentals;
using DTO.SearchQueries;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Models;
using OneOf;
using OneOf.Types;
using Repositories.Interfaces;

namespace API.Rentals
{
    public class RentalService(IRentalRepository rentalRepo, IInventoryRecordRepository inventoryRepo, IMovieRepository movieRepo, IUserRepository userRepo)
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

            var rentingDate = request.DateRented == null ? DateOnly.FromDateTime(DateTime.Now) : request.DateRented.Value;

            var totalInInventory = await inventoryRepo.GetTotalAmount(request.MovieId, DateOnly.MinValue, rentingDate);
            var totalRented = await rentalRepo.GetByMovieIdAsync(request.MovieId);
            var totalNotReturned = totalRented.Count(x => x.DateReturned == null);

            if (totalInInventory - totalNotReturned <= 0)
                return new MovieOutOfStock();

            var rental = new Rental
            {
                MovieId = request.MovieId,
                UserId = request.UserId,
                DateRented = rentingDate,
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

            await rentalRepo.SaveChangesAsync();
            return new Success();
        }
    }
}
