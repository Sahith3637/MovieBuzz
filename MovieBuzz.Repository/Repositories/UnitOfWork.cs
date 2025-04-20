using MovieBuzz.Repository.Context;
using MovieBuzz.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private  IUserRepository? _users;
        private IMovieRepository? _movies;
        private IShowRepository? _shows;
        private IBookingRepository? _bookings;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            
        }

        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IMovieRepository Movies => _movies ??= new MovieRepository(_context);
        public IShowRepository Shows => _shows ??= new ShowRepository(_context);
        public IBookingRepository Bookings => _bookings ??= new BookingRepository(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
