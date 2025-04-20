using AutoMapper;
using MovieBuzz.Core.Dtos.Users;
using MovieBuzz.Core.Entities;
using MovieBuzz.Core.Exceptions;
using MovieBuzz.Repository.Interfaces;
using MovieBuzz.Services.Interfaces;

namespace MovieBuzz.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserResponseDto> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            var existingUser = await _unitOfWork.Users.GetUserByUsernameAsync(registerUserDto.UserName);
            if (existingUser != null)
                throw new ConflictException("Username already exists");

            var user = _mapper.Map<User>(registerUserDto);
            user.Role = "User";
            user.IsActive = true;
            user.CreatedOn = DateTime.UtcNow;

            await _unitOfWork.Users.AddUserAsync(user);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> LoginUserAsync(LoginDto loginDto)
        {
            var user = await _unitOfWork.Users.GetUserByUsernameAsync(loginDto.UserName)
                ?? throw new UnauthorizedException("Invalid username or password");

            if (user.Password != loginDto.Password)
                throw new UnauthorizedException("Invalid username or password");

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetUserByIdAsync(userId)
                ?? throw new NotFoundException($"User with ID {userId} not found");

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllUsersAsync();
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }
    }
}