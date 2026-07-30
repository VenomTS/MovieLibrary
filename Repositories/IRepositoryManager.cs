using Microsoft.AspNetCore.Identity;
using Models;
using Repositories.Implementations;
using Repositories.Interfaces;

namespace Repositories;

public interface IRepositoryManager
{
    public IGenreRepository Genres { get; set; }
    public IInventoryRecordRepository InventoryRecords { get; set; }
    public IMovieGenreRepository MovieGenres { get; set; }
    public IMovieRepository Movies { get; set; }
    public IRentalRepository Rentals { get; set; }
    public IScheduleRepository Schedules { get; set; }
    public IInvoiceRepository Invoices { get; set; }
    public IInvoiceDeliveryRepository InvoiceDeliveries { get; set; }
    public IInvoiceTemplateRepository InvoiceTemplates { get; set; }
    public UserManager<AppUser> Users { get; set; }

    public Task SaveChangesAsync();
    public Task<T> ExecuteProcedure<T>(string command, params CommandParameter[] args);
    public Task ExecuteProcedure(string command, params CommandParameter[] args);
}