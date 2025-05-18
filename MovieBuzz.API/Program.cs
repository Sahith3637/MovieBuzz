using Microsoft.EntityFrameworkCore;
using MovieBuzz.Core.Data;
using MovieBuzz.Repository.Context;
using MovieBuzz.Repository.Interfaces;
using MovieBuzz.Repository.Repositories;
using MovieBuzz.Services.Interfaces;
using MovieBuzz.Services.Services;
using Serilog.Events;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;
using MovieBuzz.Services.Monitor;
using MovieBuzz.API.Middleware;
using MovieBuzz.Core.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/moviebuzz-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        shared: true)
    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("MovieBuzzDB"),
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "Logs",
            AutoCreateSqlTable = true,
            BatchPostingLimit = 50,
            BatchPeriod = TimeSpan.FromSeconds(5)
        },
        columnOptions: new ColumnOptions() // Optional column customization
        {
            AdditionalColumns = new Collection<SqlColumn>
            {
                new SqlColumn { DataType = SqlDbType.NVarChar, ColumnName = "User" },
                new SqlColumn { DataType = SqlDbType.NVarChar, ColumnName = "Machine" }
            }
        })
    .CreateLogger();

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));


builder.Services.AddSingleton(jwtSettings);



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});
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


builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddControllers();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IShowRepository, ShowRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IShowService, ShowService>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddSingleton<ApiMonitoringService>();

////============================================Test data=======================================
builder.Services.AddScoped<TestSeederService>();
//// ===========================================================================================

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AdminOrUser", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") || context.User.IsInRole("User")));
});


// Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MovieBuzz API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("*",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});


var app = builder.Build();
app.UseCors("*");
app.UseMiddleware<ExceptionHandlingMiddleware>();


app.MapGet("/api-stats", (ApiMonitoringService monitor) =>
{
    var stats = monitor.GetApiStatistics();
    return Results.Ok(new
    {
        Summary = stats.ToDictionary(
            s => s.Key,
            s => new
            {
                s.Value.TotalRequests,
                s.Value.SuccessfulRequests,
                s.Value.FailedRequests,
                s.Value.SuccessRate,
                s.Value.AverageDurationMs,
                LastExecuted = s.Value.LastExecuted.ToString("o")
            }),
        RecentExecutions = stats.ToDictionary(
            s => s.Key,
            s => s.Value.ExecutionHistory.Select(e => new
            {
                Timestamp = e.Timestamp.ToString("o"),
                e.DurationMs,
                e.Success,
                e.StatusCode
            }).ToList())
    });
}).Produces<object>(200);
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
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<LoggingMiddleware>();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
