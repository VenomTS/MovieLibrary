namespace DTO.Stocks;

public class UpdateStockRequest
{
    public Guid MovieId { get; set; }
    public int NewStockAmount { get; set; }
}