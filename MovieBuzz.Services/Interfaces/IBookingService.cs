using MovieBuzz.Core.Dtos.Bookings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponseDto> GetBookingByIdAsync(int bookingId);
        Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto bookingDto);
        Task<IEnumerable<BookingResponseDto>> GetBookingsByUserNameAsync(string userName);
        Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync();
        Task<IEnumerable<BookingResponseDto>> GetBookingsByUserIdAsync(int userId);
        Task<IEnumerable<AdminBookingDto>> GetBookingsByMovieIdAsync(int movieId);
    }
}
