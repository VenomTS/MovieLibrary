using Microsoft.EntityFrameworkCore;
using Models;
using Repositories;
using Repositories.Database;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class StockRepository(AppDbContext dbContext) : RepositoryBase<Stock>(dbContext), IStockRepository
{
}