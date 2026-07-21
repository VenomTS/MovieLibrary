using API.Auth.Services;
using API.Genres;
using API.MovieGenres;
using API.Movies;
using API.Rentals;
using API.Stocks;
using API.Users;
using Microsoft.EntityFrameworkCore;
using Repositories.Database;
using Repositories.Implementations;
using Repositories.Interfaces;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddSingleton<HashingService>();
builder.Services.AddSingleton<JsonWebTokenService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<MovieService>();
builder.Services.AddScoped<GenreService>();
builder.Services.AddScoped<StockService>();
builder.Services.AddScoped<MovieGenreService>();
builder.Services.AddScoped<RentalService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IMovieGenreRepository, MovieGenreRepository>();
builder.Services.AddScoped<IRentalRepository, RentalRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.Map("/", () => Results.Redirect("/scalar"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();