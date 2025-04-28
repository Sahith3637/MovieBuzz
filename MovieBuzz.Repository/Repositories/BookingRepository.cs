using Microsoft.EntityFrameworkCore;
using MovieBuzz.Core.Entities;
using MovieBuzz.Core.Exceptions;
using MovieBuzz.Repository.Context;
using MovieBuzz.Repository.Interfaces;

namespace MovieBuzz.Repository.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;
        private const string DateTimeFormat = "dd/MM/yyyy HH:mm";

        public BookingRepository(AppDbContext context) => _context = context;

        //public async Task<Booking> AddBookingAsync(Booking booking)
        //{
        //    var show = await _context.Shows
        //        .FirstOrDefaultAsync(s => s.ShowId == booking.ShowId);

        //    if (show == null)
        //    {
        //        throw MovieBuzzExceptions.NotFound("Show not found"); // Fixed from NotFound
        //    }

        //    //Parse show time
        //    if (!DateTime.TryParseExact(show.ShowTime, DateTimeFormat,
        //        CultureInfo.InvariantCulture, DateTimeStyles.None, out var showDateTime))
        //    {
        //        throw MovieBuzzExceptions.BusinessRule("Invalid show time format");
        //    }

        //    if (showDateTime < DateTime.Now)
        //    {
        //        throw MovieBuzzExceptions.BusinessRule("Cannot book for past shows");
        //    }

        //    // Get movie and user
        //    var movie = await _context.Movies.FindAsync(booking.MovieId);
        //    var user = await _context.Users.FindAsync(booking.UserId);

        //    if (movie == null || user == null)
        //    {
        //        throw MovieBuzzExceptions.NotFound("Movie or user not found");
        //    }

        //    // Age validation with proper DateOnly handling
        //    var today = DateOnly.FromDateTime(DateTime.Today);
        //    var age = today.Year - user.DateOfBirth.Year;

        //    if (user.DateOfBirth > today.AddYears(-age))
        //    {
        //        age--;
        //    }

        //    if (age < movie.AgeRestriction)
        //    {
        //        throw MovieBuzzExceptions.BusinessRule($"User must be at least {movie.AgeRestriction} years old");
        //    }

        //    await _context.Bookings.AddAsync(booking);
        //    return booking;
        //}


        //public async Task<Booking> AddBookingAsync(Booking booking)
        //{
        //    await _context.Bookings.AddAsync(booking);
        //    return booking;
        //}

        public async Task<Booking> AddBookingAsync(Booking booking)
        {
            var show = await _context.Shows
                .Include(s => s.Movie) // Add this to include Movie information
                .FirstOrDefaultAsync(s => s.ShowId == booking.ShowId);

            if (show == null)
            {
                throw MovieBuzzExceptions.NotFound("Show not found");
            }

            // Parse show time and date
            var showDateTime = show.ShowDate.ToDateTime(TimeOnly.Parse(show.ShowTime));

            if (showDateTime < DateTime.Now)
            {
                throw MovieBuzzExceptions.BusinessRule("Cannot book for past shows");
            }

            // Get user
            var user = await _context.Users.FindAsync(booking.UserId);

            if (user == null)
            {
                throw MovieBuzzExceptions.NotFound("User not found");
            }

            // Age validation
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - user.DateOfBirth.Year;

            if (user.DateOfBirth > today.AddYears(-age))
            {
                age--;
            }

            if (age < show.Movie.AgeRestriction)
            {
                throw MovieBuzzExceptions.BusinessRule($"User must be at least {show.Movie.AgeRestriction} years old to book this movie");
            }

            await _context.Bookings.AddAsync(booking);
            return booking;
        }
        public async Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Movie)
                .Include(b => b.Show)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Movie)
                .Include(b => b.Show)
                .ToListAsync(); // Removed ordering
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Movie)
                .Include(b => b.Show)
                .ToListAsync(); // Removed ordering
        }

        public async Task<IEnumerable<Booking>> GetBookingsByMovieIdAsync(int movieId)
        {
            return await _context.Bookings
                .Where(b => b.MovieId == movieId)
                .Include(b => b.User)
                .Include(b => b.Show)
                .ToListAsync(); // Removed ordering
        }
    }
}