using AutoMapper;
using MovieBuzz.Core.Dtos.Shows;
using MovieBuzz.Core.DTOs.Shows;
using MovieBuzz.Core.Entities;
using MovieBuzz.Core.Exceptions;
using MovieBuzz.Repository.Interfaces;
using MovieBuzz.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieBuzz.Services.Services
{
    public class ShowService : IShowService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShowService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ShowResponseDto> GetShowByIdAsync(int showId)
        {
            var show = await _unitOfWork.Shows.GetShowWithMovieAsync(showId)
                ?? throw MovieBuzzExceptions.NotFound($"Show with ID {showId} not found");

            return _mapper.Map<ShowResponseDto>(show);
        }

        public async Task<IEnumerable<ShowResponseDto>> GetAllShowsAsync()
        {
            var shows = await _unitOfWork.Shows.GetAllShowsAsync();
            return _mapper.Map<IEnumerable<ShowResponseDto>>(shows);
        }

        //public async Task<IEnumerable<ShowResponseDto>> GetShowsByMovieIdAsync(int movieId)
        //{
        //    var shows = await _unitOfWork.Shows.GetShowsByMovieIdAsync(movieId);
        //    return _mapper.Map<IEnumerable<ShowResponseDto>>(shows);
        //}

        public async Task<IEnumerable<ShowWithMovieDetailsDto>> GetShowsByMovieIdAsync(int movieId)
        {
            var shows = await _unitOfWork.Shows.GetShowsWithMovieDetailsByMovieIdAsync(movieId);
            return _mapper.Map<IEnumerable<ShowWithMovieDetailsDto>>(shows);
        }


        //public async Task<ShowResponseDto> AddShowAsync(CreateShowDto showDto)
        //{
        //    // Validate movie exists
        //    var movie = await _unitOfWork.Movies.GetMovieByIdAsync(showDto.MovieId)
        //        ?? throw MovieBuzzExceptions.NotFound($"Movie with ID {showDto.MovieId} not found");

        //    // Convert show time to TimeSpan for comparison
        //    if (!DateTime.TryParse(showDto.ShowTime, out var showTime))
        //    {
        //        throw MovieBuzzExceptions.BusinessRule("Invalid show time format");
        //    }
        //    var timeSpan = showTime.TimeOfDay;

        //    // Get ALL shows on the same date (not just for this movie)
        //    var allShowsSameDate = (await _unitOfWork.Shows.GetShowsByMovieIdAsync(showDto.MovieId))
        //        .Where(s => s.ShowDate == showDto.ShowDate)
        //        .ToList();

        //    // Check for duplicate time slots (same time, any movie)
        //    if (allShowsSameDate.Any(s =>
        //    {
        //        if (!DateTime.TryParse(s.ShowTime, out var existingShowTime))
        //            return false;
        //        return existingShowTime.TimeOfDay == timeSpan;
        //    }))
        //    {
        //        throw MovieBuzzExceptions.Conflict("A show already exists at this time (single screen limitation)");
        //    }

        //    // Check maximum 4 shows per day for this movie
        //    var showsSameMovieSameDate = allShowsSameDate.Where(s => s.MovieId == showDto.MovieId).ToList();
        //    if (showsSameMovieSameDate.Count >= 4)
        //    {
        //        throw MovieBuzzExceptions.BusinessRule("Maximum of 4 shows per day allowed for this movie");
        //    }

        //    // Check 180 minutes gap between ALL shows (not just same movie)
        //    foreach (var existingShow in allShowsSameDate)
        //    {
        //        if (!DateTime.TryParse(existingShow.ShowTime, out var existingShowTime))
        //        {
        //            continue;
        //        }

        //        var existingTimeSpan = existingShowTime.TimeOfDay;
        //        var timeDifference = Math.Abs((timeSpan - existingTimeSpan).TotalMinutes);

        //        if (timeDifference < 180)
        //        {
        //            throw MovieBuzzExceptions.BusinessRule(
        //                $"There must be at least 180 minutes between shows. Conflict with show at {existingShow.ShowTime} " +
        //                $"for movie ID {existingShow.MovieId}");
        //        }
        //    }

        //    var show = _mapper.Map<Show>(showDto);
        //    await _unitOfWork.Shows.AddShowAsync(show);
        //    await _unitOfWork.CompleteAsync();

        //    return _mapper.Map<ShowResponseDto>(show);
        //}

        public async Task<ShowResponseDto> AddShowAsync(CreateShowDto showDto)
        {
            // Validate movie exists
            var movie = await _unitOfWork.Movies.GetMovieByIdAsync(showDto.MovieId)
                ?? throw MovieBuzzExceptions.NotFound($"Movie with ID {showDto.MovieId} not found");

            // Convert show time to TimeSpan
            if (!DateTime.TryParse(showDto.ShowTime, out var showTime))
            {
                throw MovieBuzzExceptions.BusinessRule("Invalid show time format");
            }
            var newShowTimeSpan = showTime.TimeOfDay;

            // 🔄 Get all shows on the same date (across all movies for single-screen enforcement)
            var allShowsSameDate = (await _unitOfWork.Shows.GetAllShowsAsync())
                .Where(s => s.ShowDate == showDto.ShowDate)
                .ToList();

            // ❌ Conflict: Show already exists at this time on same date (any movie)
            if (allShowsSameDate.Any(s =>
            {
                if (!DateTime.TryParse(s.ShowTime, out var existingTime))
                    return false;
                return existingTime.TimeOfDay == newShowTimeSpan;
            }))
            {
                throw MovieBuzzExceptions.Conflict(
                    $"A show already exists at {showDto.ShowTime} on {showDto.ShowDate}. Single screen cannot have overlapping shows.");
            }

            // ❌ Conflict: More than 4 shows for this movie on the same date
            var sameMovieSameDate = allShowsSameDate
                .Where(s => s.MovieId == showDto.MovieId)
                .ToList();

            if (sameMovieSameDate.Count >= 4)
            {
                throw MovieBuzzExceptions.BusinessRule(
                    "A maximum of 4 shows per day is allowed for a single movie.");
            }

            // ❌ Conflict: Less than 180 minutes gap from any existing show on the same date
            foreach (var existingShow in allShowsSameDate)
            {
                if (!DateTime.TryParse(existingShow.ShowTime, out var existingTime))
                    continue;

                var existingTimeSpan = existingTime.TimeOfDay;
                var gapInMinutes = Math.Abs((newShowTimeSpan - existingTimeSpan).TotalMinutes);

                if (gapInMinutes < 180)
                {
                    throw MovieBuzzExceptions.BusinessRule(
                        $"There must be at least 180 minutes between shows. Conflict with show at {existingShow.ShowTime} for movie ID {existingShow.MovieId}");
                }
            }

            // ✅ Passed all checks — Add show
            var show = _mapper.Map<Show>(showDto);
            await _unitOfWork.Shows.AddShowAsync(show);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ShowResponseDto>(show);
        }
     



        public async Task<ShowResponseDto> UpdateShowAsync(int showId, ShowDto showDto)
        {
            var show = await _unitOfWork.Shows.GetShowByIdAsync(showId)
                ?? throw MovieBuzzExceptions.NotFound($"Show with ID {showId} not found");

            _mapper.Map(showDto, show);
            await _unitOfWork.Shows.UpdateShowAsync(show);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ShowResponseDto>(show);
        }
    }
}