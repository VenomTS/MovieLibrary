using API.Movies;
using API.Users;

namespace API.Rentals
{
    public class Rental
    {
        public Guid Id { get; set; }
        public Guid MovieId { get; set; }
        public Guid UserId { get; set; }
        public DateOnly DateRented { get; set; }
        public DateOnly? DateReturned { get; set; }

        public Movie Movie { get; set; }
        public User User { get; set; }
    }
}
