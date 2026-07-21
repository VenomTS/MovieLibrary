namespace DTO.Rentals
{
    public class RentRequest
    {
        public Guid MovieId { get; set; }
        public Guid UserId { get; set; }
        public DateOnly? DateRented { get; set; }
    }
}
