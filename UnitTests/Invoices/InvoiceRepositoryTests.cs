using Microsoft.EntityFrameworkCore;
using Models.Invoices;
using Repositories.Database;
using Repositories.Implementations;
using Xunit.Abstractions;

namespace UnitTests.Invoices;

public class InvoiceRepositoryTests(DatabaseFixture fixture)
    : IClassFixture<DatabaseFixture>
{

    [Fact]
    public async Task GetMaxNumber_ShouldReturnHighestInvoiceNumber()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var user = await Shared.CreateUser(dbContext);

        dbContext.Invoices.AddRange(
            new Invoice
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Number = "0001"
            },
            new Invoice
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Number = "0005"
            },
            new Invoice
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Number = "0003"
            });

        await dbContext.SaveChangesAsync();

        var repository = new InvoiceRepository(dbContext);

        // Act
        var result = await repository.GetMaxNumber();

        // Assert
        Assert.Equal("0005", result);
    }

    [Fact]
    public async Task GetMaxNumber_ShouldReturnOnlyNumber_WhenOnlyOneInvoiceExists()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var user = await Shared.CreateUser(dbContext);

        dbContext.Invoices.Add(
            new Invoice
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Number = "0001"
            });

        await dbContext.SaveChangesAsync();

        var repository = new InvoiceRepository(dbContext);

        // Act
        var result = await repository.GetMaxNumber();

        // Assert
        Assert.Equal("0001", result);
    }

    [Fact]
    public async Task GetMaxNumber_ShouldReturnLexicographicallyHighestNumber()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var user = await Shared.CreateUser(dbContext);

        dbContext.Invoices.AddRange(
            new Invoice
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Number = "100"
            },
            new Invoice
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Number = "200"
            },
            new Invoice
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Number = "150"
            });

        await dbContext.SaveChangesAsync();

        var repository = new InvoiceRepository(dbContext);

        // Act
        var result = await repository.GetMaxNumber();

        // Assert
        Assert.Equal("200", result);
    }

    [Fact]
    public async Task GetMaxNumber_ShouldReturnNull_WhenNoInvoicesExist()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var repository = new InvoiceRepository(dbContext);

        // Act
        var result = await repository.GetMaxNumber();

        // Assert
        Assert.Null(result);
    }


    private static async Task ClearDatabase(AppDbContext dbContext)
    {
        await dbContext.Database.ExecuteSqlRawAsync("""
                                                    TRUNCATE TABLE
                                                        "InvoiceDeliveries",
                                                        "Invoices"
                                                    CASCADE;
                                                    """);
    }

}