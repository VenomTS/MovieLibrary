using API.OneOfTypes;
using DTO.Rentals;
using DTO.SearchQueries;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using OneOf;
using OneOf.Types;
using Repositories.Interfaces;

namespace API.Rentals
{
    public class RentalService(IRentalRepository rentalRepo, IInventoryRecordRepository inventoryRepo, IMovieRepository movieRepo, UserManager<AppUser> userManager)
    {
        public async Task<OneOf<RentalDetailResponse, NotFound>> GetByIdAsync(Guid id)
        {
            var rental = await rentalRepo.GetByIdAsync(id, x => x.Movie, x => x.AppUser);
            if (rental == null)
                return new NotFound();

            return new RentalDetailResponse
            {
                Id = rental.Id,
                Movie = new RentalMovieResponse
                {
                    Id = rental.Movie.Id,
                    Name = rental.Movie.Name,
                    DateReleased = rental.Movie.ReleaseDate,
                },
                User = new RentalUserResponse
                {
                    Id = rental.UserId,
                    Name = rental.AppUser.UserName ?? "No UserName",
                },
                DateRented = rental.DateRented,
                DateReturned = rental.DateReturned,
            };
        }
        
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

        public async Task<OneOf<RentalResponse, AlreadyRenting, MovieNotFound, UserNotFound, MovieOutOfStock>> RentMovie(RentRequest request)
        {
            var movieExists = await movieRepo.MovieExistsAsync(request.MovieId);
            if (!movieExists)
                return new MovieNotFound();
            
            var userExists = await userManager.Users.AnyAsync(x => x.Id == request.UserId);
            if (!userExists)
                return new UserNotFound();

            var rentals = await rentalRepo.Search(new RentalSearchQuery
            {
                UserId = request.UserId,
                MovieId = request.MovieId,
            });

            if (rentals.Any(x => x.DateReturned == null))
                return new AlreadyRenting();

            var rentingDate = request.DateRented ?? DateOnly.FromDateTime(DateTime.Now);

            var totalInInventory = await inventoryRepo.GetTotalAmount(request.MovieId, rentingDate);
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

            return new RentalResponse
            {
                Id = rental.Id,
                MovieId = rental.MovieId,
                UserId = rental.UserId,
                DateRented = rental.DateRented,
                DateReturned = rental.DateReturned,
            };
        }

        public async Task<OneOf<RentalResponse, RentalNotFound>> ReturnMovie(Guid rentalId, ReturnRequest request)
        {
            var rental = await rentalRepo.GetByIdAsync(rentalId);

            if (rental == null)
                return new RentalNotFound();

            rental.DateReturned = request.DateReturned ?? DateOnly.FromDateTime(DateTime.Now);

            await rentalRepo.SaveChangesAsync();

            return new RentalResponse
            {
                Id = rental.Id,
                MovieId = rental.MovieId,
                UserId = rental.UserId,
                DateRented = rental.DateRented,
                DateReturned = rental.DateReturned,
            };
        }

        public async Task<OneOf<List<UserRentalsResponse>, NotFound>> GetUnreturnedByUserId(Guid userId)
        {
            var userExists = await userManager.Users.AnyAsync(x => x.Id == userId);
            if (!userExists)
                return new NotFound();
            var rentals = await rentalRepo.GetByUserIdAsync(userId);
            var unreturned = rentals.Where(x => x.DateReturned == null);

            var rentalsDto = unreturned.Select(x => new UserRentalsResponse
            {
                Id = x.Id,
                Movie = new RentalMovieResponse
                {
                    Id = x.Movie.Id,
                    Name = x.Movie.Name,
                    DateReleased = x.Movie.ReleaseDate,
                },
                DateRented = x.DateRented,
            });
            return rentalsDto.ToList();
        }
    }
}
