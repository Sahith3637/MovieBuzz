using Microsoft.EntityFrameworkCore;
using MovieBuzz.Repository.Context;

namespace MovieBuzz.Repository.Data.SeedData;

public class SeedManager : IDatabaseSeeder
{
    private readonly AppDbContext _context;

    public SeedManager(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        // Add other seed calls here later
    }

    public static void ConfigureSeed(ModelBuilder modelBuilder)
    {
        UserSeed.SeedUsers(modelBuilder);
        // Add other seed configurations here
    }
}