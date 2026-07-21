namespace DTO.Rentals
{
    public class ReturnRequest
    {
        public Guid RentalId { get; set; }
        public DateOnly? DateReturned { get; set; }
    }
}
