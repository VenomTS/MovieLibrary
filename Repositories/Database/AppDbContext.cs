using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.InvoiceDeliveries;
using Models.Invoices;
using Models.Schedules;
using Models.Schedules.Rules;

namespace Repositories.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<MovieGenre> MovieGenres { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<InventoryRecord> InventoryRecords { get; set; }

    public DbSet<RecurrenceRule> RecurrenceRules { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceCounter> InvoiceCounters { get; set; }
    public DbSet<InvoiceTemplate> InvoiceTemplates { get; set; }
    public DbSet<InvoiceDelivery> InvoiceDeliveries { get; set; }

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
        modelBuilder.Entity<InvoiceCounter>().HasKey(x => x.Year);
        modelBuilder.Entity<InvoiceDelivery>().HasKey(x => new { x.InvoiceTemplateId, x.ScheduleId, x.DateCreated });

        modelBuilder.Entity<Schedule>().OwnsOne(x => x.RecurrenceRule);
        
    }
}