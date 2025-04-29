using Microsoft.AspNetCore.Mvc;
using MovieBuzz.Core.Dtos.Users;
using MovieBuzz.Services.Interfaces;
using MovieBuzz.Core.Exceptions;

namespace MovieBuzz.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            try
            {
                var user = await _userService.RegisterUserAsync(registerDto);
                return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, new
                {
                    Success = true,
                    Data = user,
                    Message = "User registered successfully"
                });
            }
            catch (ConflictException ex)
            {
                return Conflict(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Error registering user: {ex.Message}"
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var user = await _userService.LoginUserAsync(loginDto);
                return Ok(new
                {
                    Success = true,
                    Data = user,
                    Message = "Login successful"
                });
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Error during login: {ex.Message}"
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(new
                {
                    Success = true,
                    Data = users,
                    Message = "All users retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Error retrieving users: {ex.Message}"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                return Ok(new
                {
                    Success = true,
                    Data = user,
                    Message = "User retrieved successfully"
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Error retrieving user: {ex.Message}"
                });
            }
        }

        // [HttpPatch("{id}/toggle-status")]
        // public async Task<IActionResult> ToggleUserStatus(int id)
        // {
        //     try
        //     {
        //         var result = await _userService.ToggleUserStatusAsync(id);
        //         if (result)
        //         {
        //             return Ok(new
        //             {
        //                 Success = true,
        //                 Message = "User status toggled successfully"
        //             });
        //         }
        //         return NotFound(new
        //         {
        //             Success = false,
        //             Message = "User not found"
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, new
        //         {
        //             Success = false,
        //             Message = $"Error toggling user status: {ex.Message}"
        //         });
        //     }
        // }
    }
}