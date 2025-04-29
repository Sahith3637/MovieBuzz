using MovieBuzz.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Repository.Interfaces
{
    public interface IShowRepository
    {
        Task<Show?> GetShowByIdAsync(int showId);
        Task<Show?> GetShowWithMovieAsync(int showId);
        Task<IEnumerable<Show>> GetAllShowsAsync();
        Task<IEnumerable<Show>> GetShowsByMovieIdAsync(int movieId);
        Task<IEnumerable<Show>> GetShowsWithMovieDetailsByMovieIdAsync(int movieId);
        Task<Show> AddShowAsync(Show show);
        Task<Show> UpdateShowAsync(Show show);
    }
}
