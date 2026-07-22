using Microsoft.EntityFrameworkCore;
using Models;
using Models.Users;

namespace Repositories.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<MovieGenre> MovieGenres { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<InventoryRecord> InventoryRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<MovieGenre>().HasKey(x => new { x.MovieId, x.GenreId });
        modelBuilder.Entity<Movie>().HasMany(x => x.Genres).WithMany(x => x.Movies).UsingEntity<MovieGenre>();

        // MovieId is both primary and foreign key
        modelBuilder.Entity<Stock>().HasKey(x => x.MovieId);
        modelBuilder.Entity<Stock>().HasOne(x => x.Movie).WithOne(x => x.Stock).HasForeignKey<Stock>(x => x.MovieId);
    }
}