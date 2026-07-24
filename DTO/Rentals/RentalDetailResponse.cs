namespace DTO.Rentals;

public class RentalDetailResponse
{
    public Guid Id { get; set; }

    public RentalMovieResponse Movie { get; set; }

    public RentalUserResponse User { get; set; }

    public DateOnly DateRented { get; set; }
    public DateOnly? DateReturned { get; set; }
}