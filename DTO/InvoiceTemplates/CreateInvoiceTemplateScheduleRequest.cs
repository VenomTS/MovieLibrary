using Models.Schedules.Rules;

namespace DTO.InvoiceTemplates;

public class CreateInvoiceTemplateScheduleRequest
{
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public Frequency Frequency { get; set; }
    public int Interval { get; set; }
    public DaysOfWeek? DaysOfWeek { get; set; }
    public int? DayOfMonth { get; set; }
    public Ordinal? Ordinal { get; set; }
    public OrdinalType? OrdinalType { get; set; }
}