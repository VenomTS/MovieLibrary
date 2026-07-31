using System.Data;
using Repositories;

namespace API.InvoiceCounters;

public class InvoiceCounterService(IRepositoryManager repositoryManager)
{
    public async Task<int> GetAndIncrementCountByYear(int year)
    {
        const string command = """
                               INSERT INTO "InvoiceCounters" ("Year", "Count")
                               VALUES (@year, 1)
                               ON CONFLICT ("Year")
                               DO UPDATE
                               SET "Count" = "InvoiceCounters"."Count" + 1
                               RETURNING "Count";
                               """;
        
        var counter = await repositoryManager.ExecuteProcedure<int>(command, new CommandParameter
        {
            Name = "year",
            Value = year
        });

        return counter;
    }
}