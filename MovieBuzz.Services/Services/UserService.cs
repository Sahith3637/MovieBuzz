using AutoMapper;
using MovieBuzz.Core.Dtos.Users;
using MovieBuzz.Core.Entities;
using MovieBuzz.Core.Exceptions;
using MovieBuzz.Repository.Interfaces;
using MovieBuzz.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        //public async Task<UserResponseDto> RegisterUserAsync(RegisterUserDto registerUserDto)
        //{
        //    var existingUser = await _unitOfWork.Users.GetUserByUsernameAsync(registerUserDto.UserName);
        //    if (existingUser != null)
        //        throw MovieBuzzExceptions.Conflict("Username already exists");

        //    var user = _mapper.Map<User>(registerUserDto);
        //    user.IsActive = true;
        //    user.Password = PasswordHasher.HashPassword(registerUserDto.Password);

        //    await _unitOfWork.Users.AddUserAsync(user);
        //    await _unitOfWork.CompleteAsync();

        //    return _mapper.Map<UserResponseDto>(user);
        //}

        //public async Task<UserResponseDto> LoginUserAsync(LoginDto loginDto)
        //{
        //    var user = await _unitOfWork.Users.GetUserByUsernameAsync(loginDto.UserName)
        //        ?? throw MovieBuzzExceptions.Unauthorized("Invalid username or password");

        //    if (user.Password != loginDto.Password)
        //        throw MovieBuzzExceptions.Unauthorized("Invalid username or password");

        //    return _mapper.Map<UserResponseDto>(user);
        //}



        public async Task<UserResponseDto> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
           
            var existingUser = await _unitOfWork.Users.GetUserByUsernameAsync(registerUserDto.UserName);
            if (existingUser != null)
                throw MovieBuzzExceptions.Conflict("Username already exists");

            
            var existingPhoneUser = await _unitOfWork.Users.GetUserByPhoneAsync(registerUserDto.PhoneNo);
            if (existingPhoneUser != null)
                throw MovieBuzzExceptions.Conflict("Phone number already registered");

            
            var existingEmailUser = await _unitOfWork.Users.GetUserByEmailAsync(registerUserDto.EmailId);
            if (existingEmailUser != null)
                throw MovieBuzzExceptions.Conflict("Email already registered");

            var user = _mapper.Map<User>(registerUserDto);
            user.IsActive = true;
            user.CreatedOn = DateTime.Now;
            user.Role = "User"; // Default role
            user.Password = PasswordHasher.HashPassword(registerUserDto.Password); // Hash the password

            await _unitOfWork.Users.AddUserAsync(user);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<UserResponseDto>(user);
        }

        //public async Task<UserResponseDto> LoginUserAsync(LoginDto loginDto)
        //{
        //    var user = await _unitOfWork.Users.GetUserByUsernameAsync(loginDto.UserName)
        //        ?? throw MovieBuzzExceptions.Unauthorized("Invalid username or password");

        //    // Check if user is too old (born before 1930) or too young (under 3)
        //    var today = DateOnly.FromDateTime(DateTime.Today);
        //    var age = today.Year - user.DateOfBirth.Year;
        //    if (user.DateOfBirth > today.AddYears(-age)) age--;

        //    if (age < 3)
        //        throw MovieBuzzExceptions.Unauthorized("User must be at least 3 years old");

        //    if (age > 93) // 2023 - 1930 = 93
        //        throw MovieBuzzExceptions.Unauthorized("User account is too old");

        //    // Verify password
        //    if (!PasswordHasher.VerifyPassword(loginDto.Password, user.Password))
        //        throw MovieBuzzExceptions.Unauthorized("Invalid username or password");

        //    return _mapper.Map<UserResponseDto>(user);
        //}



        public async Task<UserResponseDto> LoginUserAsync(LoginDto loginDto)
        {
           
            var user = await _unitOfWork.Users.GetUserByUsernameAsync(loginDto.UserName);

           
            if (user == null || user.UserName != loginDto.UserName)
            {
                throw MovieBuzzExceptions.Unauthorized("Invalid username or password");
            }

            // Check if user is too old (born before 1930) or too young (under 3)
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - user.DateOfBirth.Year;
            if (user.DateOfBirth > today.AddYears(-age)) age--;

            if (age < 3)
                throw MovieBuzzExceptions.Unauthorized("User must be at least 3 years old");

            if (age > 93) // 2023 - 1930 = 93
                throw MovieBuzzExceptions.Unauthorized("User account is too old");

            // Verify password
            if (!PasswordHasher.VerifyPassword(loginDto.Password, user.Password))
                throw MovieBuzzExceptions.Unauthorized("Invalid username or password");

            return _mapper.Map<UserResponseDto>(user);
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

        //public async Task<bool> ToggleUserStatusAsync(int userId)
        //{
        //    var result = await _unitOfWork.Users.ToggleUserActiveStatusAsync(userId);
        //    if (result) await _unitOfWork.CompleteAsync();
        //    return result;
        //}
    }
}