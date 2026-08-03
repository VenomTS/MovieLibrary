using API.Invoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Models.InvoiceDeliveries;
using Models.Invoices;
using Moq;
using Repositories;
using Repositories.Database;

namespace UnitTests.Invoices;

public class InvoiceServiceTests(DatabaseFixture fixture)
    : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task CreateAutomaticInvoice_ShouldCreateInvoiceWithNumber000001_WhenNoInvoicesExist()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var user = await Shared.CreateUser(dbContext);

        var invoiceTemplate = CreateInvoiceTemplate(user.Id);

        var repositoryManager = CreateRepositoryManager(dbContext);

        var transaction = new Mock<IDbContextTransaction>();

        repositoryManager
            .Setup(x => x.BeginTransactionAsync())
            .ReturnsAsync(transaction.Object);

        repositoryManager
            .Setup(x => x.Invoices.GetMaxNumber())
            .ReturnsAsync((string?)null);

        repositoryManager
            .Setup(x => x.Invoices.CreateAsync(It.IsAny<Invoice>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.InvoiceDeliveries.CreateAsync(
                It.IsAny<InvoiceDelivery>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var service = new InvoiceService(repositoryManager.Object);

        // Act
        await service.CreateAutomaticInvoice(invoiceTemplate);

        // Assert
        repositoryManager.Verify(
            x => x.Invoices.CreateAsync(
                It.Is<Invoice>(invoice =>
                    invoice.UserId == invoiceTemplate.UserId &&
                    invoice.Price == invoiceTemplate.Price &&
                    invoice.Description == invoiceTemplate.Description &&
                    invoice.Number == "000001")),
            Times.Once);

        repositoryManager.Verify(
            x => x.InvoiceDeliveries.CreateAsync(
                It.Is<InvoiceDelivery>(delivery =>
                    delivery.InvoiceTemplateId == invoiceTemplate.Id &&
                    delivery.ScheduleId == invoiceTemplate.ScheduleId &&
                    delivery.InvoiceId != Guid.Empty &&
                    delivery.DeliveryStatus == InvoiceDeliveryStatus.InProgress)),
            Times.Once);

        repositoryManager.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);

        transaction.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        transaction.Verify(
            x => x.RollbackAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAutomaticInvoice_ShouldIncrementInvoiceNumber()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var user = await Shared.CreateUser(dbContext);

        var invoiceTemplate = CreateInvoiceTemplate(user.Id);

        var repositoryManager = CreateRepositoryManager(dbContext);

        var transaction = new Mock<IDbContextTransaction>();

        repositoryManager
            .Setup(x => x.BeginTransactionAsync())
            .ReturnsAsync(transaction.Object);

        repositoryManager
            .Setup(x => x.Invoices.GetMaxNumber())
            .ReturnsAsync("000125");

        repositoryManager
            .Setup(x => x.Invoices.CreateAsync(It.IsAny<Invoice>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.InvoiceDeliveries.CreateAsync(
                It.IsAny<InvoiceDelivery>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var service = new InvoiceService(repositoryManager.Object);

        // Act
        await service.CreateAutomaticInvoice(invoiceTemplate);

        // Assert
        repositoryManager.Verify(
            x => x.Invoices.CreateAsync(
                It.Is<Invoice>(invoice =>
                    invoice.Number == "000126")),
            Times.Once);

        transaction.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAutomaticInvoice_ShouldFormatNumberToSixDigits()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var user = await Shared.CreateUser(dbContext);

        var invoiceTemplate = CreateInvoiceTemplate(user.Id);

        var repositoryManager = CreateRepositoryManager(dbContext);

        var transaction = new Mock<IDbContextTransaction>();

        repositoryManager
            .Setup(x => x.BeginTransactionAsync())
            .ReturnsAsync(transaction.Object);

        repositoryManager
            .Setup(x => x.Invoices.GetMaxNumber())
            .ReturnsAsync("42");

        repositoryManager
            .Setup(x => x.Invoices.CreateAsync(It.IsAny<Invoice>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.InvoiceDeliveries.CreateAsync(
                It.IsAny<InvoiceDelivery>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var service = new InvoiceService(repositoryManager.Object);

        // Act
        await service.CreateAutomaticInvoice(invoiceTemplate);

        // Assert
        repositoryManager.Verify(
            x => x.Invoices.CreateAsync(
                It.Is<Invoice>(invoice =>
                    invoice.Number == "000043")),
            Times.Once);
    }

    [Fact]
    public async Task CreateAutomaticInvoice_ShouldUse999999_WhenExistingNumberIsInvalid()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var user = await Shared.CreateUser(dbContext);

        var invoiceTemplate = CreateInvoiceTemplate(user.Id);

        var repositoryManager = CreateRepositoryManager(dbContext);

        var transaction = new Mock<IDbContextTransaction>();

        repositoryManager
            .Setup(x => x.BeginTransactionAsync())
            .ReturnsAsync(transaction.Object);

        repositoryManager
            .Setup(x => x.Invoices.GetMaxNumber())
            .ReturnsAsync("INVALID");

        repositoryManager
            .Setup(x => x.Invoices.CreateAsync(It.IsAny<Invoice>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.InvoiceDeliveries.CreateAsync(
                It.IsAny<InvoiceDelivery>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var service = new InvoiceService(repositoryManager.Object);

        // Act
        await service.CreateAutomaticInvoice(invoiceTemplate);

        // Assert
        repositoryManager.Verify(
            x => x.Invoices.CreateAsync(
                It.Is<Invoice>(invoice =>
                    invoice.Number == "999999")),
            Times.Once);
    }

    [Fact]
    public async Task CreateAutomaticInvoice_ShouldCreateNextInvoiceNumber()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var user = await Shared.CreateUser(dbContext);

        var invoiceTemplate = new InvoiceTemplate
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            ScheduleId = Guid.NewGuid(),
            Price = 100m,
            Description = "Test invoice"
        };

        var repositoryManager = CreateRepositoryManager(dbContext);

        var transaction = new Mock<IDbContextTransaction>();

        repositoryManager
            .Setup(x => x.BeginTransactionAsync())
            .ReturnsAsync(transaction.Object);

        repositoryManager
            .Setup(x => x.Invoices.GetMaxNumber())
            .ReturnsAsync("000998");

        repositoryManager
            .Setup(x => x.Invoices.CreateAsync(It.IsAny<Invoice>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.InvoiceDeliveries.CreateAsync(It.IsAny<InvoiceDelivery>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var service = new InvoiceService(repositoryManager.Object);

        // Act
        await service.CreateAutomaticInvoice(invoiceTemplate);

        // Assert
        repositoryManager.Verify(
            x => x.Invoices.CreateAsync(
                It.Is<Invoice>(invoice =>
                    invoice.Number == "000999" &&
                    invoice.UserId == invoiceTemplate.UserId &&
                    invoice.Price == invoiceTemplate.Price &&
                    invoice.Description == invoiceTemplate.Description)),
            Times.Once);

        transaction.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task CreateAutomaticInvoice_ShouldCreateDeliveryReferencingCreatedInvoice()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var user = await Shared.CreateUser(dbContext);

        var invoiceTemplate = CreateInvoiceTemplate(user.Id);

        var repositoryManager = CreateRepositoryManager(dbContext);

        var transaction = new Mock<IDbContextTransaction>();

        repositoryManager
            .Setup(x => x.BeginTransactionAsync())
            .ReturnsAsync(transaction.Object);

        repositoryManager
            .Setup(x => x.Invoices.GetMaxNumber())
            .ReturnsAsync("000010");

        Invoice? createdInvoice = null;
        InvoiceDelivery? createdDelivery = null;

        repositoryManager
            .Setup(x => x.Invoices.CreateAsync(It.IsAny<Invoice>()))
            .Callback<Invoice>(invoice => createdInvoice = invoice)
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.InvoiceDeliveries.CreateAsync(
                It.IsAny<InvoiceDelivery>()))
            .Callback<InvoiceDelivery>(
                delivery => createdDelivery = delivery)
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var service = new InvoiceService(repositoryManager.Object);

        // Act
        await service.CreateAutomaticInvoice(invoiceTemplate);

        // Assert
        Assert.NotNull(createdInvoice);
        Assert.NotNull(createdDelivery);

        Assert.Equal(
            createdInvoice.Id,
            createdDelivery.InvoiceId);

        Assert.Equal(
            invoiceTemplate.Id,
            createdDelivery.InvoiceTemplateId);

        Assert.Equal(
            invoiceTemplate.ScheduleId,
            createdDelivery.ScheduleId);

        Assert.Equal(
            InvoiceDeliveryStatus.InProgress,
            createdDelivery.DeliveryStatus);
    }

    [Fact]
    public async Task CreateAutomaticInvoice_ShouldRollback_WhenSaveChangesFails()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var user = await Shared.CreateUser(dbContext);

        var invoiceTemplate = CreateInvoiceTemplate(user.Id);

        var repositoryManager = CreateRepositoryManager(dbContext);

        var transaction = new Mock<IDbContextTransaction>();

        repositoryManager
            .Setup(x => x.BeginTransactionAsync())
            .ReturnsAsync(transaction.Object);

        repositoryManager
            .Setup(x => x.Invoices.GetMaxNumber())
            .ReturnsAsync("000001");

        repositoryManager
            .Setup(x => x.Invoices.CreateAsync(It.IsAny<Invoice>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.InvoiceDeliveries.CreateAsync(
                It.IsAny<InvoiceDelivery>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .ThrowsAsync(new Exception("Database error"));

        var service = new InvoiceService(repositoryManager.Object);

        // Act
        await service.CreateAutomaticInvoice(invoiceTemplate);

        // Assert
        transaction.Verify(
            x => x.RollbackAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        transaction.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAutomaticInvoice_ShouldSetInvoiceDataFromTemplate()
    {
        await using var dbContext = fixture.CreateDbContext();
        await ClearDatabase(dbContext);

        var user = await Shared.CreateUser(dbContext);

        var invoiceTemplate = CreateInvoiceTemplate(user.Id);

        invoiceTemplate.Price = 125.50m;
        invoiceTemplate.Description = "Monthly subscription";

        var repositoryManager = CreateRepositoryManager(dbContext);

        var transaction = new Mock<IDbContextTransaction>();

        repositoryManager
            .Setup(x => x.BeginTransactionAsync())
            .ReturnsAsync(transaction.Object);

        repositoryManager
            .Setup(x => x.Invoices.GetMaxNumber())
            .ReturnsAsync("000005");

        repositoryManager
            .Setup(x => x.Invoices.CreateAsync(It.IsAny<Invoice>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.InvoiceDeliveries.CreateAsync(
                It.IsAny<InvoiceDelivery>()))
            .Returns(Task.CompletedTask);

        repositoryManager
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var service = new InvoiceService(repositoryManager.Object);

        Invoice? createdInvoice = null;

        repositoryManager
            .Setup(x => x.Invoices.CreateAsync(It.IsAny<Invoice>()))
            .Callback<Invoice>(invoice => createdInvoice = invoice)
            .Returns(Task.CompletedTask);

        // Act
        await service.CreateAutomaticInvoice(invoiceTemplate);

        // Assert
        Assert.NotNull(createdInvoice);

        Assert.Equal(
            invoiceTemplate.UserId,
            createdInvoice.UserId);

        Assert.Equal(
            invoiceTemplate.Price,
            createdInvoice.Price);

        Assert.Equal(
            invoiceTemplate.Description,
            createdInvoice.Description);

        Assert.Equal(
            "000006",
            createdInvoice.Number);

        Assert.Equal(
            DateOnly.FromDateTime(DateTime.Now),
            createdInvoice.DateCreated);
    }

    private static Mock<IRepositoryManager> CreateRepositoryManager(
        AppDbContext dbContext)
    {
        return new Mock<IRepositoryManager>();
    }

    private static InvoiceTemplate CreateInvoiceTemplate(Guid userId)
    {
        return new InvoiceTemplate
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ScheduleId = Guid.NewGuid(),
            Price = 100m,
            Description = "Test invoice"
        };
    }

    private static async Task ClearDatabase(AppDbContext dbContext)
    {
        await dbContext.Database.ExecuteSqlRawAsync("""
                                                    TRUNCATE TABLE
                                                        "InvoiceDeliveries",
                                                        "Invoices",
                                                        "InvoiceTemplates",
                                                        "Schedules"
                                                    CASCADE;
                                                    """);
    }

}