using Microsoft.EntityFrameworkCore;
using MovieBuzz.Core.Entities;
using MovieBuzz.Repository.Context;
using MovieBuzz.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Repository.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Movie)
                .Include(b => b.Show)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }
        public async Task<Booking> AddBookingAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            return booking;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Movie)
                .Include(b => b.Show)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Movie)
                .Include(b => b.Show)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByMovieIdAsync(int movieId)
        {
            return await _context.Bookings
                .Where(b => b.MovieId == movieId)
                .Include(b => b.User)
                .Include(b => b.Show)
                .ToListAsync();
        }
    }
}
