using API.Genres;
using API.InventoryRecords;
using API.MovieGenres;
using API.Movies;
using API.Rentals;
using API.Users;
using Microsoft.EntityFrameworkCore;
using Repositories.Database;
using Repositories.Implementations;
using Repositories.Interfaces;
using Scalar.AspNetCore;
using API.Auth;
using Microsoft.AspNetCore.Identity;
using Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddAuthentication(IdentityConstants.BearerScheme)
    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services
    .AddIdentityCore<AppUser>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();

// Services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<MovieService>();
builder.Services.AddScoped<GenreService>();
builder.Services.AddScoped<MovieGenreService>();
builder.Services.AddScoped<RentalService>();
builder.Services.AddScoped<InventoryRecordService>();
builder.Services.AddSingleton<IEmailSender<AppUser>, NoOpEmailSender>();

// Repositories
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IMovieGenreRepository, MovieGenreRepository>();
builder.Services.AddScoped<IRentalRepository, RentalRepository>();
builder.Services.AddScoped<IInventoryRecordRepository, InventoryRecordRepository>();

var app = builder.Build();

// Seed roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    string[] roles = ["Admin", "Librarian", "Customer"];

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.Map("/", () => Results.Redirect("/scalar"));
}

app.UseHttpsRedirection();

app.MapGroup("/api/v1/Auth").WithTags("Auth").MapIdentityApi<AppUser>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();