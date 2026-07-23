namespace Models
{
    public class Rental
    {
        public Guid Id { get; set; }
        public Guid MovieId { get; set; }
        public Guid AppUserId { get; set; }
        public DateOnly DateRented { get; set; }
        public DateOnly? DateReturned { get; set; }

        public Movie Movie { get; set; }
        public AppUser AppUser { get; set; }
    }
}
