using System.Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Database;
using Repositories.Interfaces;

namespace Repositories;

public class RepositoryManager(
    AppDbContext dbContext,
    IGenreRepository genreRepository,
    IInventoryRecordRepository inventoryRecordRepository,
    IMovieGenreRepository movieGenreRepository,
    IMovieRepository movieRepository,
    IRentalRepository rentalRepository
) : IRepositoryManager
{
    public IGenreRepository Genres { get; set; } = genreRepository;
    public IInventoryRecordRepository InventoryRecords { get; set; } = inventoryRecordRepository;
    public IMovieGenreRepository MovieGenres { get; set; } = movieGenreRepository;
    public IMovieRepository Movies { get; set; } = movieRepository;
    public IRentalRepository Rentals { get; set; } = rentalRepository;

    public async Task SaveChangesAsync()
    {
        await Movies.SaveChangesAsync();
    }

    public async Task<T> ExecuteProcedure<T>(string commandStr, params CommandParameter[] args)
    {
        var connection = dbContext.Database.GetDbConnection();
        
        if(connection.State != ConnectionState.Open)
            await connection.OpenAsync();
        
        await using var command = connection.CreateCommand();
        command.CommandText = commandStr;

        foreach (var arg in args)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = arg.Name;
            parameter.Value = arg.Value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
        
        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            throw new InvalidOperationException("No data returned.");
        
        return reader.GetFieldValue<T>(0);
    }

    public async Task ExecuteProcedure(string commandStr, params CommandParameter[] args)
    {
        var connection = dbContext.Database.GetDbConnection();
        
        if(connection.State != ConnectionState.Open)
            await connection.OpenAsync();
        
        await using var command = connection.CreateCommand();
        command.CommandText = commandStr;

        foreach (var arg in args)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = arg.Name;
            parameter.Value = arg.Value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
        
        await command.ExecuteNonQueryAsync();
    }
}