using MovieBuzz.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Repository.Interfaces
{
    public interface IMovieRepository
    {
        Task<Movie> AddMovieAsync(Movie movie);
        Task<Movie?> GetMovieByIdAsync(int movieId);
        Task<IEnumerable<Movie>> GetAllMoviesAsync();
        Task<IEnumerable<Movie>> GetActiveMoviesAsync();
        Task<Movie> UpdateMovieAsync(Movie movie);
        Task<bool> ToggleMovieActiveStatusAsync(int movieId);
    }
}
