using Microsoft.EntityFrameworkCore;
using MovieBuzz.Core.Data;
using MovieBuzz.Repository.Context;
using MovieBuzz.Repository.Interfaces;
using MovieBuzz.Repository.Repositories;
using MovieBuzz.Services.Interfaces;
using MovieBuzz.Services.Services;

var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("MovieBuzzDB"),
//    sqlOptions => sqlOptions.EnableRetryOnFailure()));
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MovieBuzzDB"),
        sqlOptions => sqlOptions.EnableRetryOnFailure());

    // These methods belong to DbContextOptionsBuilder
    options.EnableDetailedErrors();
    options.EnableSensitiveDataLogging();
    options.LogTo(Console.WriteLine, LogLevel.Information);
});

// Add services to the container
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddControllers();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IShowRepository, ShowRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Service Layer
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IShowService, ShowService>();
builder.Services.AddScoped<IBookingService, BookingService>();

////============================================Test data=======================================
builder.Services.AddScoped<TestSeederService>();
//// ===========================================================================================

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repository Layer


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated(); // Creates DB if not exists
        Console.WriteLine("Database verified/created successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while initializing the database: {ex.Message}");
    }
}
//app.Use(async (context, next) =>
//{
//    try
//    {
//        await next();
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"Unhandled exception: {ex}");
//        context.Response.StatusCode = 500;
//        await context.Response.WriteAsync("An unexpected error occurred");
//    }
//});
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unhandled exception: {ex}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner Exception: {ex.InnerException}");
        }
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync($"An unexpected error occurred: {ex.Message}");
    }
});
app.MapGet("/testdb", async (AppDbContext dbContext) =>
{
    try
    {
        await dbContext.Database.CanConnectAsync();
        return "Database connection successful!";
    }
    catch (Exception ex)
    {
        return $"Database connection failed: {ex.Message}";
    }
});
//builder.Services.AddScoped<TestSeederService>();
//builder.Services.AddScoped<TestSeederService, TestSeederService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
