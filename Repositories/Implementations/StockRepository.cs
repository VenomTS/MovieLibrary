using Microsoft.EntityFrameworkCore;
using Models;
using Repositories;
using Repositories.Databasee;
using Repositories.Interfaces;

namespace API.Stocks.Repositories;

public class StockRepository(AppDbContext dbContext) : RepositoryBase<Stock>(dbContext), IStockRepository
{
}