using MovieBuzz.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MovieBuzz.Repository.Data.SeedData;

public static class UserSeed
{
    public static void SeedUsers(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User
            {
                UserId = 100,
                FirstName = "Admin",
                LastName = "User",
                DateOfBirth = new DateOnly(2000, 1, 1),
                EmailId = "admin@moviebuzz.com",
                PhoneNo = "1234567890",
                UserName = "admin",
                Password = HashedPassword("admin123"), // See step 3 for hashing
                Role = "admin",
                IsActive = true,
                CreatedOn = DateTime.Now
            },
            new User
            {
                UserId = 101,
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = new DateOnly(1995, 5, 15),
                EmailId = "test@moviebuzz.com",
                PhoneNo = "9876543210",
                UserName = "testuser",
                Password = HashedPassword("Test@123"),
                Role = "user",
                IsActive = true,
                CreatedOn = DateTime.Now
            }
        );
    }

    private static string HashedPassword(string password)
    {
        // Temporary simple hash - replace with real hashing later
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }
}