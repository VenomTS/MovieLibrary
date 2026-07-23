namespace DTO.InventoryRecords;

public class InventoryRecordSingularResponse
{
    public Guid Id { get; set; }
    public Guid MovieId { get; set; }
    public int Amount { get; set; }
    public DateOnly Date { get; set; }
}