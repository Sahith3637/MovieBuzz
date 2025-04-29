using MovieBuzz.Core.Dtos.Shows;
using MovieBuzz.Core.DTOs.Shows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Services.Interfaces
{
    public interface IShowService
    {
        Task<ShowResponseDto> GetShowByIdAsync(int showId);
        Task<IEnumerable<ShowResponseDto>> GetAllShowsAsync();
        //Task<IEnumerable<ShowResponseDto>> GetShowsByMovieIdAsync(int movieId);
        Task<IEnumerable<ShowWithMovieDetailsDto>> GetShowsByMovieIdAsync(int movieId);
        Task<ShowResponseDto> AddShowAsync(CreateShowDto showDto);
        Task<ShowResponseDto> UpdateShowAsync(int showId, ShowDto showDto);
    }
}
