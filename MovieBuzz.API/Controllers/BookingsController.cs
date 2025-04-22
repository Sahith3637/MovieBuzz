using Microsoft.AspNetCore.Mvc;
using MovieBuzz.Core.Dtos.Bookings;
using MovieBuzz.Services.Interfaces;
using MovieBuzz.Services.Services;

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
            var booking = await _bookingService.CreateBookingAsync(bookingDto);
            return CreatedAtAction(
                nameof(GetUserBooking),
                new { userName = booking.UserName, bookingId = booking.BookingId },
                booking
            );
        }

        [HttpGet("user/{userName}/{bookingId}")]
        public async Task<IActionResult> GetUserBooking(string userName, int bookingId)
        {
            var booking = await _bookingService.GetBookingByIdAsync(bookingId);

            if (booking == null || booking.UserName != userName)
            {
                return NotFound(new
                {
                    Message = "Booking not found or doesn't belong to user",
                    UserName = userName,
                    BookingId = bookingId
                });
            }

            return Ok(booking);
        }

        [HttpGet("user/{userName}")]
        public async Task<IActionResult> GetUserBookings(string userName)
        {
            var bookings = await _bookingService.GetBookingsByUserNameAsync(userName);
            return Ok(bookings);
        }

        [HttpGet("admin")]
        public async Task<IActionResult> GetAllBookingsAdmin()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        [HttpGet("admin/movie/{movieId}")]
        public async Task<IActionResult> GetBookingsByMovieAdmin(int movieId)
        {
            var bookings = await _bookingService.GetBookingsByMovieIdAsync(movieId);
            return Ok(bookings);
        }

        [HttpGet("admin/{bookingId}")]
        public async Task<IActionResult> GetBookingAdmin(int bookingId)
        {
            var booking = await _bookingService.GetBookingByIdAsync(bookingId);
            return booking != null ? Ok(booking) : NotFound();
        }
        //[HttpGet("with-movie/{showId}")]
        //public async Task<IActionResult> GetShowWithMovie(int showId)
        //{
        //    var show = await _showService.GetShowWithMovieAsync(showId);
        //    return Ok(show);
        //}
    }
}