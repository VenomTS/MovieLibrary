using API.Schedules;
using Quartz;

namespace API.Quartz;

public class SendInvoicesJob(ScheduleService scheduleService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var scheduled = await scheduleService.GetScheduledSchedulesAsync();
        
        if(scheduled.Count == 0)
            Console.WriteLine("No invoices to be sent");
        
        foreach(var schedule in scheduled)
        {
            Console.WriteLine($"Sending invoice {schedule.Id} to User {schedule.UserId}");
        }
    }
}