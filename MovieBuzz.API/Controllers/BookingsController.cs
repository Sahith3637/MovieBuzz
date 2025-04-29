using Microsoft.AspNetCore.Mvc;
using MovieBuzz.Core.Dtos.Bookings;
using MovieBuzz.Services.Interfaces;
using MovieBuzz.Core.Exceptions;

namespace MovieBuzz.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto bookingDto)
        {
            try
            {
                var booking = await _bookingService.CreateBookingAsync(bookingDto);
                return CreatedAtAction(
                    nameof(GetBookingById),
                    new { bookingId = booking.BookingId },
                    new
                    {
                        Success = true,
                        Data = booking,
                        Message = "Booking created successfully"
                    }
                );
            }
            catch (Exception ex) when (ex is BusinessRuleException || ex is NotFoundException)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetBookingById(int bookingId)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(bookingId);
                return Ok(new
                {
                    Success = true,
                    Data = booking,
                    Message = "Booking retrieved successfully"
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
        }

        [HttpGet("admin")]
        public async Task<IActionResult> GetAllBookings()
        {
            try
            {
                var bookings = await _bookingService.GetAllBookingsAsync();
                return Ok(new
                {
                    Success = true,
                    Data = bookings,
                    Message = "All bookings retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("admin/movie/{movieId}")]
        public async Task<IActionResult> GetBookingsByMovie(int movieId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByMovieIdAsync(movieId);
                return Ok(new
                {
                    Success = true,
                    Data = bookings,
                    Message = $"Bookings for movie {movieId} retrieved successfully"
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
                    Message = ex.Message
                });
            }
        }
    }
}