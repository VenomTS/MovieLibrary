using API.Movies;
using API.Users;
using Microsoft.EntityFrameworkCore;

namespace API.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Movie> Movies { get; set; }
}