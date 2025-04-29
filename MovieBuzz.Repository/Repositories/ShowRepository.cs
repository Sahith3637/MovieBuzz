using Microsoft.EntityFrameworkCore;
using MovieBuzz.Core.Entities;
using MovieBuzz.Core.Exceptions;
using MovieBuzz.Repository.Context;
using MovieBuzz.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBuzz.Repository.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private readonly AppDbContext _context;

        public ShowRepository(AppDbContext context)
        {
            _context = context;
        }   
        public async Task<Show?> GetShowByIdAsync(int showId)
        {
            return await _context.Shows.FindAsync(showId);
        }

        public async Task<Show?> GetShowWithMovieAsync(int showId)
        {
            return await _context.Shows
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(s => s.ShowId == showId);
        }

        public async Task<IEnumerable<Show>> GetAllShowsAsync()
        {
            return await _context.Shows.ToListAsync();
        }
        // Example fix for async warning
        public async Task<IEnumerable<Show>> GetShowsWithMovieDetailsByMovieIdAsync(int movieId)
        {
            return await _context.Shows
                .Include(s => s.Movie)
                .Where(s => s.MovieId == movieId)
                .AsNoTracking()
                .ToListAsync(); // Note the await here
        }

        public async Task<IEnumerable<Show>> GetShowsByDateAsync(DateOnly date)
        {
            return await _context.Shows
                .Where(s => s.ShowDate == date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Show>> GetShowsByMovieIdAsync(int movieId)
        {
            return await _context.Shows
                .Where(s => s.MovieId == movieId)
                .ToListAsync();
        }

        public async Task<Show> AddShowAsync(Show show)
        {
            await _context.Shows.AddAsync(show);
            return show;
        }

        public async Task<Show> UpdateShowAsync(Show show)
        {
             _context.Shows.Update(show);
            return show;
        }
    }
}