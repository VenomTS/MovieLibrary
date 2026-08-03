using System.Text.Json.Serialization;
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
using API.InvoiceDeliveries;
using API.Invoices;
using API.InvoiceTemplates;
using API.Quartz;
using API.Schedules;
using Microsoft.AspNetCore.Identity;
using Models;
using Quartz;
using Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
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

builder.Services.AddAutoMapper(_ => { }, typeof(API.Mapper.AutoMapper));

// Services
// builder.Services.AddHostedService<RentalsWorker>();

builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("InvoiceHandler");
    
    q.AddJob<InvoiceHandler>(options => options.WithIdentity(jobKey));
    
    // Trigger once on startup
    q.AddTrigger(options => options.ForJob(jobKey)
        .WithIdentity("InvoiceHandlerStartupTrigger")
        .StartNow());
    
    // Trigger every 15 seconds
    q.AddTrigger(options => options.ForJob(jobKey)
        .WithIdentity("InvoiceHandlerTrigger")
        .WithCronSchedule("0 0 6 * * ?"));
});

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<MovieService>();
builder.Services.AddScoped<GenreService>();
builder.Services.AddScoped<MovieGenreService>();
builder.Services.AddScoped<RentalService>();
builder.Services.AddScoped<InventoryRecordService>();
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<InvoiceService>();
builder.Services.AddScoped<InvoiceTemplateService>();
builder.Services.AddScoped<InvoiceDeliveryService>();
builder.Services.AddScoped<InvoiceSendingService>();
builder.Services.AddSingleton<IEmailSender<AppUser>, NoOpEmailSender>();

// Repositories
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IMovieGenreRepository, MovieGenreRepository>();
builder.Services.AddScoped<IRentalRepository, RentalRepository>();
builder.Services.AddScoped<IInventoryRecordRepository, InventoryRecordRepository>();
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceTemplateRepository, InvoiceTemplateRepository>();
builder.Services.AddScoped<IInvoiceDeliveryRepository, InvoiceDeliveryRepository>();

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