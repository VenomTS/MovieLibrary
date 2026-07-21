namespace API.Stocks.DTO;

public class UpdateStockRequest
{
    public Guid MovieId { get; set; }
    public int NewStockAmount { get; set; }
}