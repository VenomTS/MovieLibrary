namespace API.Rentals
{
    public class RentalSearch
    {
        public Guid? MovieId { get; set; }
        public Guid? UserId { get; set; }
        public DateOnly? DateRented { get; set; }
    }
}
