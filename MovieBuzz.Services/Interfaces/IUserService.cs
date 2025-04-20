using MovieBuzz.Core.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> RegisterUserAsync(RegisterUserDto registerUserDto);
        Task<UserResponseDto> LoginUserAsync(LoginDto loginDto);
        Task<UserResponseDto> GetUserByIdAsync(int userId);
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    }
}
