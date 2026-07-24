namespace DTO.Rentals;

public class RentalMovieResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly ReleaseDate { get; set; }
}