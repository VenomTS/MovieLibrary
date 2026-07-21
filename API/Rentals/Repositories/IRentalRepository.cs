namespace API.Rentals.Repositories
{
    public interface IRentalRepository
    {
        public Task<List<Rental>> GetAllRentals(RentalSearch rentalSearch);
        public Task<Rental?> GetRentalById(Guid id);
        public Task AddRental(Rental rental);
        public Task UpdateRental(Guid rentalId, Rental newRental);
    }
}
