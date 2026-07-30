using DTO.InvoiceTemplates;
using DTO.Schedules;
using Microsoft.EntityFrameworkCore;
using Models.Invoices;
using OneOf;
using OneOf.Types;
using Repositories;

namespace API.InvoiceTemplates;

public class InvoiceTemplateService(IRepositoryManager repositoryManager)
{
    public async Task<InvoiceTemplateResponse> CreateAsync(CreateInvoiceTemplateRequest request)
    {
        // Checks, whether there exists Invoice Template for this user, etc, etc...
        var invoice = new InvoiceTemplate
        {
            UserId = request.UserId,
            ScheduleId = request.ScheduleId,
            Price = request.Price,
            Description = request.Description,
        };
        
        await repositoryManager.InvoiceTemplates.CreateAsync(invoice);
        await repositoryManager.SaveChangesAsync();

        var invoiceResponse = new InvoiceTemplateResponse
        {
            Id = invoice.Id,
            UserId = invoice.UserId,
            ScheduleId = invoice.ScheduleId,
            Price = invoice.Price,
            Description = invoice.Description,
        };
        return invoiceResponse;
    }

    public async Task<OneOf<InvoiceTemplateDetailedResponse?, NotFound>> GetByUserIdAsync(Guid userId)
    {
        var invoiceTemplate = await repositoryManager.InvoiceTemplates.GetByUserIdAsync(userId);
        if (invoiceTemplate == null)
            return new NotFound();
        
        var recurrenceRule = invoiceTemplate.Schedule.RecurrenceRule;

        return new InvoiceTemplateDetailedResponse
        {
            Id = invoiceTemplate.Id,
            UserId = invoiceTemplate.UserId,
            Price = invoiceTemplate.Price,
            Description = invoiceTemplate.Description,
            Schedule = new ScheduleResponse
            {
                Id = invoiceTemplate.ScheduleId,
                StartDate = invoiceTemplate.Schedule.StartDate,
                EndDate = invoiceTemplate.Schedule.EndDate,
                NextOccurrence = invoiceTemplate.Schedule.NextOccurrence,
                Frequency = recurrenceRule.Frequency,
                Interval = recurrenceRule.Interval,
                DaysOfWeek = recurrenceRule.DaysOfWeek,
                DayOfMonth = recurrenceRule.DayOfMonth,
                Ordinal = recurrenceRule.Ordinal,
                OrdinalType = recurrenceRule.OrdinalType,
            }
        };
    }

    public async Task<List<InvoiceTemplateResponse>> GetAllAsync()
    {
        var invoices = await repositoryManager.InvoiceTemplates.GetAllAsync();

        var invoicesDto = invoices.Select(x => new InvoiceTemplateResponse
        {
            Id = x.Id,
            UserId = x.UserId,
            ScheduleId = x.ScheduleId,
            Price = x.Price,
            Description = x.Description,
        });
        return invoicesDto.ToList();
    }
    
    public async Task<List<InvoiceTemplate>> GetScheduledInvoicesAsync(DateOnly date)
    {
        // Ovo se koristi samo za nove invoices, stari (failed) invoices su handle-ovani u InvoiceDeliveryService
        var scheduledInvoices = await repositoryManager.InvoiceTemplates.AsQueryable()
            .Include(x => x.Schedule)
            .Where(invoiceTemplate => invoiceTemplate.Schedule.NextOccurrence <= date &&
                                      !repositoryManager.InvoiceDeliveries.AsQueryable()
                                          .Any(invoiceDelivery =>
                                              invoiceDelivery.InvoiceTemplateId == invoiceTemplate.Id &&
                                              invoiceDelivery.DateCreated == date &&
                                              invoiceDelivery.ScheduleId == invoiceTemplate.ScheduleId))
            .ToListAsync();
        return scheduledInvoices;
    }
}