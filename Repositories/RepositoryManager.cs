using Repositories.Interfaces;

namespace Repositories;

public class RepositoryManager(
    IGenreRepository genreRepository,
    IInventoryRecordRepository inventoryRecordRepository,
    IMovieGenreRepository movieGenreRepository,
    IMovieRepository movieRepository,
    IRentalRepository rentalRepository
) : IRepositoryManager
{
    public IGenreRepository Genres { get; set; } = genreRepository;
    public IInventoryRecordRepository InventoryRecords { get; set; } = inventoryRecordRepository;
    public IMovieGenreRepository MovieGenres { get; set; } = movieGenreRepository;
    public IMovieRepository Movies { get; set; } = movieRepository;
    public IRentalRepository Rentals { get; set; } = rentalRepository;
}