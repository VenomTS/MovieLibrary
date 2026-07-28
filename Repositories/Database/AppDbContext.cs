using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Schedules;

namespace Repositories.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<MovieGenre> MovieGenres { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<InventoryRecord> InventoryRecords { get; set; }

    public DbSet<ScheduleBase> Schedules { get; set; }
    public DbSet<Invoice> Invoices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasPostgresExtension("pg_trgm");
        
        modelBuilder.Entity<AppUser>(b =>
        {
            b.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");
        });
        
        modelBuilder.Entity<MovieGenre>().HasKey(x => new { x.MovieId, x.GenreId });
        modelBuilder.Entity<Movie>().HasMany(x => x.Genres).WithMany(x => x.Movies).UsingEntity<MovieGenre>();
        
        modelBuilder.Entity<Rental>().HasOne(x => x.AppUser).WithMany().HasForeignKey(x => x.UserId);
        modelBuilder.Entity<Invoice>().HasOne(x => x.AppUser).WithMany().HasForeignKey(x => x.UserId);
        
        modelBuilder.Entity<ScheduleBase>()
            .HasDiscriminator<string>("ScheduleType")
            .HasValue<DailySchedule>("Daily")
            .HasValue<WeeklySchedule>("Weekly")
            .HasValue<MonthlySchedule>("Monthly");
        modelBuilder.Entity<ScheduleBase>().HasOne(x => x.AppUser).WithOne().HasForeignKey<ScheduleBase>(x => x.UserId);
    }
}