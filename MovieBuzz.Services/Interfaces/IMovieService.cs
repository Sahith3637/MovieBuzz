using MovieBuzz.Core.Dtos.Movies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Services.Interfaces
{
    public interface IMovieService
    {
        Task<MovieResponseDto> GetMovieByIdAsync(int movieId);
        Task<IEnumerable<MovieResponseDto>> GetAllMoviesAsync();
        Task<IEnumerable<MovieSummaryDto>> GetActiveMoviesAsync();
        Task<MovieResponseDto> AddMovieAsync(CreateMovieDto movieDto);
        Task<MovieResponseDto> UpdateMovieAsync(int movieId, MovieDto movieDto);
        Task<bool> ToggleMovieStatusAsync(int movieId);

        Task<MovieWithShowsResponseDto> CreateMovieWithShowsAsync(MovieWithShowsDto dto);
        Task<MovieWithShowsResponseDto> UpdateMovieWithShowsAsync(int movieId, UpdateMovieWithShowsDto dto);
    }
}
