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
                    booking
                );
            }
            catch (Exception ex) when (ex is BusinessRuleException || ex is NotFoundException)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetBookingById(int bookingId)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(bookingId);
                return Ok(booking);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        //[HttpGet("user/{userId}")]
        //public async Task<IActionResult> GetBookingsByUser(int userId)
        //{
        //    var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
        //    return Ok(bookings);
        //}

        [HttpGet("admin")]
        //[ApiExplorerSettings(GroupName = "Admin")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        [HttpGet("admin/movie/{movieId}")]
        //[ApiExplorerSettings(GroupName = "Admin")]
        public async Task<IActionResult> GetBookingsByMovie(int movieId)
        {
            var bookings = await _bookingService.GetBookingsByMovieIdAsync(movieId);
            return Ok(bookings);
        }
    }
}