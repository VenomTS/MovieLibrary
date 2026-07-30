using API.OneOfTypes;
using DTO.InvoiceTemplates;
using DTO.Schedules;
using Microsoft.EntityFrameworkCore;
using Models.Invoices;
using Models.Schedules;
using Models.Schedules.Rules;
using OneOf;
using OneOf.Types;
using Repositories;

namespace API.InvoiceTemplates;

public class InvoiceTemplateService(IRepositoryManager repositoryManager)
{
    public async Task<OneOf<InvoiceTemplateResponse, InvoiceTemplateAlreadyExists>> CreateAsync(CreateInvoiceTemplateRequest request)
    {
        // Ako se ne moze napraviti, edituj ga?
        if(!await CanCreateInvoiceTemplate(request))
            return new InvoiceTemplateAlreadyExists();
        
        var schedule = new Schedule
        {
            StartDate = request.Schedule.StartDate,
            EndDate = request.Schedule.EndDate,
            RecurrenceRule = new RecurrenceRule
            {
                Frequency = request.Schedule.Frequency,
                Interval = request.Schedule.Interval,
                DaysOfWeek = request.Schedule.DaysOfWeek,
                DayOfMonth = request.Schedule.DayOfMonth,
                Ordinal = request.Schedule.Ordinal,
                OrdinalType = request.Schedule.OrdinalType,
            }
        };

        var existingSchedule = await repositoryManager.Schedules.AsQueryable()
            .Where(x => x.StartDate <= schedule.StartDate &&
                        x.EndDate == schedule.EndDate &&
                        x.RecurrenceRule.Frequency == schedule.RecurrenceRule.Frequency &&
                        x.RecurrenceRule.Interval == schedule.RecurrenceRule.Interval &&
                        x.RecurrenceRule.DaysOfWeek == schedule.RecurrenceRule.DaysOfWeek &&
                        x.RecurrenceRule.DayOfMonth == schedule.RecurrenceRule.DayOfMonth &&
                        x.RecurrenceRule.Ordinal == schedule.RecurrenceRule.Ordinal &&
                        x.RecurrenceRule.OrdinalType == schedule.RecurrenceRule.OrdinalType)
            .FirstOrDefaultAsync();

        if (existingSchedule == null)
        {
            await repositoryManager.Schedules.CreateAsync(schedule);
            existingSchedule = schedule;
        }
            
        // Checks, whether there exists Invoice Template for this user, etc, etc...
        var invoice = new InvoiceTemplate
        {
            UserId = request.UserId,
            ScheduleId = existingSchedule.Id,
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

    private async Task<bool> CanCreateInvoiceTemplate(CreateInvoiceTemplateRequest request)
    {
        return await repositoryManager.InvoiceTemplates.GetByUserIdAsync(request.UserId) != null;
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