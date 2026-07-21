namespace DTO.SearchQueries
{
    public class RentalSearchQuery
    {
        public Guid? MovieId { get; set; }
        public Guid? UserId { get; set; }
        public DateOnly? DateRented { get; set; }
    }
}
