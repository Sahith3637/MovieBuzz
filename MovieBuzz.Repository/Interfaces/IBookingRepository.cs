using MovieBuzz.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieBuzz.Repository.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking> AddBookingAsync(Booking booking);
        Task<Booking?> GetBookingByIdAsync(int bookingId);
        Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId);
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<IEnumerable<Booking>> GetBookingsByMovieIdAsync(int movieId);
    }
}