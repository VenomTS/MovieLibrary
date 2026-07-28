using Models;

namespace Repositories.Interfaces;

public interface IInvoiceRepository : IRepositoryBase<Invoice>
{
    public Task<Invoice?> GetLastInvoiceByScheduleId(Guid scheduleId);
}