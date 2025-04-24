using MovieBuzz.Repository.Context;
using MovieBuzz.Repository.Interfaces;
using System.Threading.Tasks;

namespace MovieBuzz.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Movies = new MovieRepository(_context);
            Shows = new ShowRepository(_context);
            Bookings = new BookingRepository(_context);
        }

        public IUserRepository Users { get; }
        public IMovieRepository Movies { get; }
        public IShowRepository Shows { get; }
        public IBookingRepository Bookings { get; }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}