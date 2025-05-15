using AutoMapper;
using MovieBuzz.Core.Dtos.Movies;
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
    public class MovieService : IMovieService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IShowService _showService;

        public MovieService(IUnitOfWork unitOfWork, IMapper mapper, IShowService showService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _showService = showService;
        }

        public async Task<MovieResponseDto> GetMovieByIdAsync(int movieId)
        {
            var movie = await _unitOfWork.Movies.GetMovieByIdAsync(movieId)
                ?? throw MovieBuzzExceptions.NotFound($"Movie with ID {movieId} not found");

            return _mapper.Map<MovieResponseDto>(movie);
        }

        public async Task<IEnumerable<MovieResponseDto>> GetAllMoviesAsync()
        {
            var movies = await _unitOfWork.Movies.GetAllMoviesAsync();
            return _mapper.Map<IEnumerable<MovieResponseDto>>(movies);
        }

        public async Task<IEnumerable<MovieSummaryDto>> GetActiveMoviesAsync()
        {
            var movies = await _unitOfWork.Movies.GetActiveMoviesAsync();
            return _mapper.Map<IEnumerable<MovieSummaryDto>>(movies);
        }

        public async Task<MovieResponseDto> AddMovieAsync(CreateMovieDto movieDto)
        {
            var movie = _mapper.Map<Movie>(movieDto);
            movie.IsActive = true;

            await _unitOfWork.Movies.AddMovieAsync(movie);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<MovieResponseDto>(movie);
        }

        public async Task<MovieResponseDto> UpdateMovieAsync(int movieId, MovieDto movieDto)
        {
            var movie = await _unitOfWork.Movies.GetMovieByIdAsync(movieId)
                ?? throw MovieBuzzExceptions.NotFound($"Movie with ID {movieId} not found");

            _mapper.Map(movieDto, movie);
            await _unitOfWork.Movies.UpdateMovieAsync(movie);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<MovieResponseDto>(movie);
        }

        public async Task<bool> ToggleMovieStatusAsync(int movieId)
        {
            var result = await _unitOfWork.Movies.ToggleMovieActiveStatusAsync(movieId);
            if (result) await _unitOfWork.CompleteAsync();
            return result;
        }

        // added
        //public async Task<MovieWithShowsResponseDto> CreateMovieWithShowsAsync(MovieWithShowsDto dto)
        //{
        //    // Create movie
        //    var movie = _mapper.Map<Movie>(dto.Movie);
        //    movie.IsActive = true;
        //    await _unitOfWork.Movies.AddMovieAsync(movie);
        //    await _unitOfWork.CompleteAsync();

        //    // Create shows
        //    var shows = new List<ShowResponseDto>();
        //    foreach (var showDto in dto.Shows)
        //    {
        //        var show = _mapper.Map<Show>(new CreateShowDto
        //        {
        //            MovieId = movie.MovieId,
        //            ShowTime = showDto.ShowTime,
        //            ShowDate = showDto.ShowDate,
        //            AvailableSeats = showDto.AvailableSeats
        //        });

        //        await _unitOfWork.Shows.AddShowAsync(show);
        //        shows.Add(_mapper.Map<ShowResponseDto>(show));
        //    }

        //    await _unitOfWork.CompleteAsync();

        //    return new MovieWithShowsResponseDto
        //    {
        //        Movie = _mapper.Map<MovieResponseDto>(movie),
        //        Shows = shows
        //    };
        //}

        public async Task<MovieWithShowsResponseDto> CreateMovieWithShowsAsync(MovieWithShowsDto dto)
        {
            // Validate all show times first (before creating anything)
            foreach (var showDto in dto.Shows)
            {
                if (!DateTime.TryParse(showDto.ShowTime, out var showTime))
                {
                    throw MovieBuzzExceptions.BusinessRule("Invalid show time format");
                }
                var timeSpan = showTime.TimeOfDay;

                var existingShowsSameTime = await _unitOfWork.Shows.GetShowsByDateAsync(showDto.ShowDate);
                if (existingShowsSameTime.Any(s =>
                {
                    if (!DateTime.TryParse(s.ShowTime, out var existingShowTime))
                        return false;
                    return existingShowTime.TimeOfDay == timeSpan;
                }))
                {
                    throw MovieBuzzExceptions.Conflict($"A show already exists at {showDto.ShowTime} on {showDto.ShowDate}");
                }

                foreach (var existingShow in existingShowsSameTime)
                {
                    if (!DateTime.TryParse(existingShow.ShowTime, out var existingShowTime))
                        continue;

                    var existingTimeSpan = existingShowTime.TimeOfDay;
                    var timeDifference = Math.Abs((timeSpan - existingTimeSpan).TotalMinutes);

                    if (timeDifference < 200)
                    {
                        throw MovieBuzzExceptions.BusinessRule(
                            $"There must be at least 200 minutes between shows. Conflict with show at {existingShow.ShowTime}");
                    }
                }
            }

            // Only create movie if all shows are valid
            var movie = _mapper.Map<Movie>(dto.Movie);
            movie.IsActive = true;
            await _unitOfWork.Movies.AddMovieAsync(movie);
            await _unitOfWork.CompleteAsync(); // Save to get the ID

            // Create shows (we know they're valid at this point)
            var shows = new List<ShowResponseDto>();
            foreach (var showDto in dto.Shows)
            {
                var show = _mapper.Map<Show>(new CreateShowDto
                {
                    MovieId = movie.MovieId,
                    ShowTime = showDto.ShowTime,
                    ShowDate = showDto.ShowDate,
                    AvailableSeats = showDto.AvailableSeats
                });

                await _unitOfWork.Shows.AddShowAsync(show);
                shows.Add(_mapper.Map<ShowResponseDto>(show));
            }

            await _unitOfWork.CompleteAsync();

            return new MovieWithShowsResponseDto
            {
                Movie = _mapper.Map<MovieResponseDto>(movie),
                Shows = shows
            };
        }



        public async Task<MovieWithShowsResponseDto> UpdateMovieWithShowsAsync(int movieId, UpdateMovieWithShowsDto dto)
        {
            var movie = await _unitOfWork.Movies.GetMovieByIdAsync(movieId)
                ?? throw MovieBuzzExceptions.NotFound($"Movie with ID {movieId} not found");
            if (!movie.IsActive)
            {
                throw MovieBuzzExceptions.BusinessRule("Cannot update shows for an inactive movie");
            }
            // Validate all show times first (before updating anything)
            foreach (var showDto in dto.Shows)
            {
                if (!DateTime.TryParse(showDto.ShowTime, out var showTime))
                {
                    throw MovieBuzzExceptions.BusinessRule("Cannot add shows for past date");
                }
                

                
                var timeSpan = showTime.TimeOfDay;

                // Get existing shows, excluding the current show if it's an update
                var existingShowsSameTime = (await _unitOfWork.Shows.GetShowsByDateAsync(showDto.ShowDate))
                    .Where(s => !showDto.ShowId.HasValue || s.ShowId != showDto.ShowId.Value)
                    .ToList();

                // Check for exact time conflicts
                if (existingShowsSameTime.Any(s =>
                {
                    if (!DateTime.TryParse(s.ShowTime, out var existingShowTime))
                        return false;
                    return existingShowTime.TimeOfDay == timeSpan;
                }))
                {
                    throw MovieBuzzExceptions.Conflict($"A show already exists at {showDto.ShowTime} on {showDto.ShowDate}");
                }

                // Check 300-minute gap rule
                foreach (var existingShow in existingShowsSameTime)
                {
                    if (!DateTime.TryParse(existingShow.ShowTime, out var existingShowTime))
                        continue;

                    var existingTimeSpan = existingShowTime.TimeOfDay;
                    var timeDifference = Math.Abs((timeSpan - existingTimeSpan).TotalMinutes);

                    if (timeDifference < 200)
                    {
                        throw MovieBuzzExceptions.BusinessRule(
                            $"There must be at least 200 minutes between shows. Conflict with show at {existingShow.ShowTime}");
                    }
                }
            }

            // Only proceed with updates if all shows are valid
            //var movie = await _unitOfWork.Movies.GetMovieByIdAsync(movieId)
            //    ?? throw MovieBuzzExceptions.NotFound($"Movie with ID {movieId} not found");

            // Update movie
            _mapper.Map(dto.Movie, movie);
            await _unitOfWork.Movies.UpdateMovieAsync(movie);

            // Process shows (we know they're valid at this point)
            var shows = new List<ShowResponseDto>();
            foreach (var showDto in dto.Shows)
            {
                if (showDto.ShowId.HasValue) // Update existing show
                {
                    var show = await _unitOfWork.Shows.GetShowByIdAsync(showDto.ShowId.Value)
                        ?? throw MovieBuzzExceptions.NotFound($"Show with ID {showDto.ShowId} not found");

                    _mapper.Map(showDto, show);
                    await _unitOfWork.Shows.UpdateShowAsync(show);
                    shows.Add(_mapper.Map<ShowResponseDto>(show));
                }
                else // Create new show
                {
                    var show = _mapper.Map<Show>(new CreateShowDto
                    {
                        MovieId = movieId,
                        ShowTime = showDto.ShowTime,
                        ShowDate = showDto.ShowDate,
                        AvailableSeats = showDto.AvailableSeats
                    });

                    await _unitOfWork.Shows.AddShowAsync(show);
                    shows.Add(_mapper.Map<ShowResponseDto>(show));
                }
            }

            await _unitOfWork.CompleteAsync();

            return new MovieWithShowsResponseDto
            {
                Movie = _mapper.Map<MovieResponseDto>(movie),
                Shows = shows
            };
        }

    }
}