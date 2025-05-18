using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBuzz.Core.Dtos.Bookings;
using MovieBuzz.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace MovieBuzz.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize] // Require authentication by default
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(
            IBookingService bookingService,
            ILogger<BookingsController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto bookingDto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var currentUserId))
                return Unauthorized(new { Success = false, Message = "Invalid user ID claim" });

            bookingDto.UserId = currentUserId; // Prevent user spoofing

            _logger.LogInformation(
                "Creating booking for user {UserId} for movie {MovieId} with {NumberOfTickets} tickets",
                bookingDto.UserId, bookingDto.MovieId, bookingDto.NumberOfTickets);

            var booking = await _bookingService.CreateBookingAsync(bookingDto);

            _logger.LogInformation("Booking {BookingId} created successfully", booking.BookingId);

            return CreatedAtAction(
                nameof(GetBookingById),
                new { bookingId = booking.BookingId },
                new
                {
                    Success = true,
                    Data = booking,
                    Message = "Booking created successfully"
                });
        }

        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetBookingById(int bookingId)
        {
            _logger.LogDebug("Fetching booking {BookingId}", bookingId);

            var booking = await _bookingService.GetBookingByIdAsync(bookingId);

            if (booking == null)
                return NotFound(new { Success = false, Message = "Booking not found" });

            _logger.LogInformation("Retrieved booking {BookingId}", bookingId);

            return Ok(new
            {
                Success = true,
                Data = booking,
                Message = "Booking retrieved successfully"
            });
        }

        [HttpGet("admin")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllBookings()
        {
            _logger.LogInformation("Admin fetching all bookings");

            var bookings = await _bookingService.GetAllBookingsAsync();

            _logger.LogInformation("Retrieved {BookingCount} bookings", bookings.Count());

            return Ok(new
            {
                Success = true,
                Data = bookings,
                Message = "All bookings retrieved successfully"
            });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetBookingsByUser(int userId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var currentUserId))
                return Unauthorized(new { Success = false, Message = "Invalid user ID claim" });

            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole != "Admin" && currentUserId != userId)
            {
                _logger.LogWarning("User {CurrentUserId} tried to access bookings for user {UserId}", currentUserId, userId);
                return Forbid();
            }

            _logger.LogInformation("Fetching bookings for user {UserId}", userId);

            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);

            _logger.LogInformation("Found {BookingCount} bookings for user {UserId}",
                bookings.Count(), userId);

            return Ok(new
            {
                Success = true,
                Data = bookings,
                Message = $"Bookings for user {userId} retrieved successfully"
            });
        }

        [HttpGet("admin/movie/{movieId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetBookingsByMovie(int movieId)
        {
            _logger.LogInformation("Admin fetching bookings for movie {MovieId}", movieId);

            var bookings = await _bookingService.GetBookingsByMovieIdAsync(movieId);

            _logger.LogInformation("Found {BookingCount} bookings for movie {MovieId}",
                bookings.Count(), movieId);

            return Ok(new
            {
                Success = true,
                Data = bookings,
                Message = $"Bookings for movie {movieId} retrieved successfully"
            });
        }
    }
}
