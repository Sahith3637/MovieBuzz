using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBuzz.Core.Dtos.Users;
using MovieBuzz.Core.Exceptions;
using MovieBuzz.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace MovieBuzz.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserService userService,
            ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            _logger.LogInformation("Registering new user with username {Username}", registerDto.UserName);

            var (user, token) = await _userService.RegisterUserAsync(registerDto);

            _logger.LogInformation("User {UserId} registered successfully", user.UserId);

            return CreatedAtAction(
                nameof(GetUser),
                new { id = user.UserId },
                new
                {
                    Success = true,
                    Data = new { User = user, Token = token },
                    Message = "User registered successfully"
                });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation("Login attempt for username {Username}", loginDto.UserName);

            var (user, token) = await _userService.LoginUserAsync(loginDto);

            _logger.LogInformation("User {UserId} logged in successfully", user.UserId);

            return Ok(new
            {
                Success = true,
                Data = new { User = user, Token = token },
                Message = "Login successful"
            });
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Fetching all users");

            var users = await _userService.GetAllUsersAsync();

            _logger.LogInformation("Retrieved {UserCount} users", users.Count());

            return Ok(new
            {
                Success = true,
                Data = users,
                Message = "All users retrieved successfully"
            });
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            _logger.LogDebug("Fetching user {UserId}", id);

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

            if (currentUserRole != "Admin" && currentUserId != id)
            {
                _logger.LogWarning("User {CurrentUserId} attempted to access user {RequestedUserId} data",
                    currentUserId, id);

                throw MovieBuzzExceptions.Unauthorized("Access denied. You are not allowed to view this user's details.");
            }

            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", id);
                throw MovieBuzzExceptions.NotFound($"User with ID {id} not found.");
            }

            _logger.LogInformation("Retrieved user {UserId} with username {Username}",
                id, user.UserName);

            return Ok(new
            {
                Success = true,
                Data = user,
                Message = "User retrieved successfully"
            });
        }
    }
}
