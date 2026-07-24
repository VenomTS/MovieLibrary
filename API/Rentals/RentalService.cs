using System.Security.Claims;
using API.OneOfTypes;
using AutoMapper;
using DTO.Rentals;
using DTO.SearchQueries;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using OneOf;
using OneOf.Types;
using Repositories;
using Repositories.Interfaces;

namespace API.Rentals;

// Dodati RepositoryManager koji bi abstractovo sve repositories
// Dodati BUILD QUERY u RepositoryManager-u koji ce buildati selected query

public class RentalService(IMapper mapper, IRepositoryManager repositoryManager, IRentalRepository rentalRepo, IInventoryRecordRepository inventoryRepo, IMovieRepository movieRepo, UserManager<AppUser> userManager)
{
    public async Task<OneOf<RentalDetailResponse, NotFound>> GetByIdAsync(Guid id)
    {
        var rental = await rentalRepo.GetByIdAsync(id, x => x.Movie, x => x.AppUser);
        if (rental == null)
            return new NotFound();
            
        return mapper.Map<RentalDetailResponse>(rental);
    }
        
    public async Task<List<RentalResponse>> GetAllRentals(RentalSearchQuery rentalSearch)
    {
        var rentals = await rentalRepo.Search(rentalSearch);

        var rentalsDto = mapper.Map<List<RentalResponse>>(rentals);
        return rentalsDto;
    }

    public async Task<OneOf<RentalResponse,
        Unauthorized,
        AlreadyRenting, 
        MovieNotFound, 
        UserNotFound, 
        MovieOutOfStock>> RentMovie(RentRequest request, ClaimsPrincipal user)
    {
        var currUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currUserId == null || request.UserId.ToString() != currUserId)
            return new Unauthorized();
            
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

        var available = await repositoryManager.InventoryRecords.AsQueryable()
            .Where(x => x.MovieId == request.MovieId)
            .SumAsync(x => x.Amount) -
                        repositoryManager.Rentals.AsQueryable().Count(x => x.MovieId == request.MovieId && x.DateReturned == null);

        if (available <= 0)
            return new MovieOutOfStock();

        var rental = mapper.Map<Rental>(request);
        rental.DateRented = rentingDate;

        await rentalRepo.CreateAsync(rental);
        await rentalRepo.SaveChangesAsync();
        
        return mapper.Map<RentalResponse>(rental);
    }

    public async Task<OneOf<RentalResponse, Unauthorized, RentalNotFound>> ReturnMovie(Guid rentalId, ReturnRequest request, ClaimsPrincipal user)
    {
        var rental = await rentalRepo.GetByIdAsync(rentalId);

        if (rental == null)
            return new RentalNotFound();
            
        var currUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currUserId == null || rental.UserId.ToString() != currUserId)
            return new Unauthorized();

        rental.DateReturned = request.DateReturned ?? DateOnly.FromDateTime(DateTime.Now);

        await rentalRepo.SaveChangesAsync();
        
        return mapper.Map<RentalResponse>(rental);
    }

    public async Task<OneOf<List<UserRentalsResponse>, Unauthorized, NotFound>> GetUnreturnedByUserId(Guid userId, ClaimsPrincipal user)
    {
        var currUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currUserId == null || userId.ToString() != currUserId)
            return new Unauthorized();
            
        var userExists = await userManager.Users.AnyAsync(x => x.Id == userId);
        if (!userExists)
            return new NotFound();

        var unreturned = await repositoryManager.Rentals.AsQueryable()
            .Include(x => x.Movie)
            .Where(x => x.UserId == userId && x.DateReturned == null).ToListAsync();
        
        return mapper.Map<List<UserRentalsResponse>>(unreturned);
    }
}