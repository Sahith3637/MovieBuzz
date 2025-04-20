
using AutoMapper;
using MovieBuzz.Core.Dtos.Users;
using MovieBuzz.Core.Entities;

namespace MovieBuzz.Core.Data;

public class TestSeederService
{
    private readonly IMapper _mapper;

    public TestSeederService(IMapper mapper)
    {
        _mapper = mapper;
    }

    public List<UserResponseDto> GetTestUsers()
    {
        // 1. Create test users
        var testUsers = new List<User>
        {
            new User
            {
                UserId = 100,
                FirstName = "Admin",
                LastName = "User",
                EmailId = "admin@moviebuzz.com",
                PhoneNo = "1234567890",
                Role = "admin",
                IsActive = true
            },
            new User
            {
                UserId = 101,
                FirstName = "Test",
                LastName = "User",
                EmailId = "test@moviebuzz.com",
                PhoneNo = "9876543210",
                Role = "user",
                IsActive = true
            }
        };

        // 2. Map to DTOs
        return _mapper.Map<List<UserResponseDto>>(testUsers);
    }
}
