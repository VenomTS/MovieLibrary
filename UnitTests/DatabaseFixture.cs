using Microsoft.EntityFrameworkCore;
using Repositories.Database;

namespace UnitTests;

public class DatabaseFixture
{
    private const string ConnectionString =
        "Host=localhost;" +
        "Port=5432;" +
        "Database=movielibrary_test;" +
        "Username=postgres;" +
        "Password=Postgres123";

    public AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new AppDbContext(options);
    }
    
    public async Task ClearDatabaseAsync()
    {
        await using var dbContext = CreateDbContext();

        // Delete dependent data first.
        dbContext.InvoiceDeliveries.RemoveRange(
            dbContext.InvoiceDeliveries);

        dbContext.InvoiceTemplates.RemoveRange(
            dbContext.InvoiceTemplates);

        dbContext.Schedules.RemoveRange(
            dbContext.Schedules);

        await dbContext.SaveChangesAsync();
    }
}