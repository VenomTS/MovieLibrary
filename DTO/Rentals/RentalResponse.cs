namespace DTO.Rentals
{
    public class RentalResponse
    {
        public Guid MovieId { get; set; }
        public Guid UserId { get; set; }
        public DateOnly DateRented { get; set; }
        public DateOnly? DateReturned { get; set; }
    }
}
