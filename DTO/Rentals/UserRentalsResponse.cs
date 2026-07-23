using System.Security.AccessControl;

namespace DTO.Rentals;

public class UserRentalsResponse
{
    public Guid Id { get; set; }
    public RentalMovieResponse Movie { get; set; }
    public DateOnly DateRented { get; set; }
}