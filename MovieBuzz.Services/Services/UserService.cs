using AutoMapper;
using MovieBuzz.Core.Dtos.Users;
using MovieBuzz.Core.Entities;
using MovieBuzz.Core.Exceptions;
using MovieBuzz.Core.Security;
using MovieBuzz.Repository.Interfaces;
using MovieBuzz.Services.Interfaces;
using System.Security.Claims;

namespace MovieBuzz.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<(UserResponseDto user, string token)> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            // Check if username exists
            var existingUser = await _unitOfWork.Users.GetUserByUsernameAsync(registerUserDto.UserName);
            if (existingUser != null)
                throw MovieBuzzExceptions.Conflict("Username already exists");

            // Check if email exists
            var existingEmailUser = await _unitOfWork.Users.GetUserByEmailAsync(registerUserDto.EmailId);
            if (existingEmailUser != null)
                throw MovieBuzzExceptions.Conflict("Email already registered");

            // Check if phone exists
            var existingPhoneUser = await _unitOfWork.Users.GetUserByPhoneAsync(registerUserDto.PhoneNo);
            if (existingPhoneUser != null)
                throw MovieBuzzExceptions.Conflict("Phone number already registered");

            var user = _mapper.Map<User>(registerUserDto);
            user.IsActive = true;
            user.CreatedOn = DateTime.UtcNow;
            user.Role = "User"; // Default role
            user.Password = PasswordHasher.HashPassword(registerUserDto.Password);

            await _unitOfWork.Users.AddUserAsync(user);
            await _unitOfWork.CompleteAsync();

            var token = _authService.GenerateJwtToken(user);
            return (_mapper.Map<UserResponseDto>(user), token);
        }

        public async Task<(UserResponseDto user, string token)> LoginUserAsync(LoginDto loginDto)
        {
            var user = await _unitOfWork.Users.GetUserByUsernameAsync(loginDto.UserName);

            if (user == null || !PasswordHasher.VerifyPassword(loginDto.Password, user.Password))
            {
                throw MovieBuzzExceptions.Unauthorized("Invalid username or password");
            }

            if (!user.IsActive)
            {
                throw MovieBuzzExceptions.Unauthorized("User account is inactive");
            }

            var token = _authService.GenerateJwtToken(user);
            return (_mapper.Map<UserResponseDto>(user), token);
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetUserByIdAsync(userId)
                ?? throw MovieBuzzExceptions.NotFound($"User with ID {userId} not found");

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllUsersAsync();
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }
    }
}