using API.Movies;

namespace API.Stocks;

public class Stock
{
    public Guid MovieId { get; set; }
    public int Amount { get; set; }

    public Movie Movie { get; set; }
}