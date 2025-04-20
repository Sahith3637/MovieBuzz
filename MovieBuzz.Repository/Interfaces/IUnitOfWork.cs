using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IMovieRepository Movies { get; }
        IShowRepository Shows { get; }
        IBookingRepository Bookings { get; }
        Task<int> CompleteAsync();
    }
}
