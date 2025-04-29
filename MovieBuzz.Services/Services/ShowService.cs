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
        //    var movie = await _unitOfWork.Movies.GetMovieByIdAsync(showDto.MovieId)
        //        ?? throw MovieBuzzExceptions.NotFound($"Movie with ID {showDto.MovieId} not found");

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

            // Convert show time to TimeSpan for comparison
            if (!DateTime.TryParse(showDto.ShowTime, out var showTime))
            {
                throw MovieBuzzExceptions.BusinessRule("Invalid show time format");
            }
            var timeSpan = showTime.TimeOfDay;

            // Check for duplicate shows (same movie, date, and time)
            var existingShows = await _unitOfWork.Shows.GetShowsByMovieIdAsync(showDto.MovieId);
            if (existingShows.Any(s => s.ShowDate == showDto.ShowDate && s.ShowTime == showDto.ShowTime))
            {
                throw MovieBuzzExceptions.Conflict("A show with the same time already exists for this movie");
            }

            // Check maximum 4 shows per day
            var showsOnSameDate = existingShows.Where(s => s.ShowDate == showDto.ShowDate).ToList();
            if (showsOnSameDate.Count >= 4)
            {
                throw MovieBuzzExceptions.BusinessRule("Maximum of 4 shows per day allowed");
            }

            // Check 180 minutes gap between shows
            foreach (var existingShow in showsOnSameDate)
            {
                if (!DateTime.TryParse(existingShow.ShowTime, out var existingShowTime))
                {
                    continue;
                }

                var existingTimeSpan = existingShowTime.TimeOfDay;
                var timeDifference = Math.Abs((timeSpan - existingTimeSpan).TotalMinutes);

                if (timeDifference < 180)
                {
                    throw MovieBuzzExceptions.BusinessRule($"There must be at least 180 minutes between shows. Conflict with show at {existingShow.ShowTime}");
                }
            }

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