using MovieBuzz.Core.Dtos.Users;
using System.Threading.Tasks;

namespace MovieBuzz.Services.Interfaces
{
    public interface IUserService
    {
        Task<(UserResponseDto user, string token)> RegisterUserAsync(RegisterUserDto registerUserDto);
        Task<(UserResponseDto user, string token)> LoginUserAsync(LoginDto loginDto);
        Task<UserResponseDto> GetUserByIdAsync(int userId);
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    }
}