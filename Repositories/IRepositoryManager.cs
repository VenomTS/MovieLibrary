using Repositories.Interfaces;

namespace Repositories;

public interface IRepositoryManager
{
    public IGenreRepository Genres { get; set; }
    public IInventoryRecordRepository InventoryRecords { get; set; }
    public IMovieGenreRepository MovieGenres { get; set; }
    public IMovieRepository Movies { get; set; }
    public IRentalRepository Rentals { get; set; }
}