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

        public MovieService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
        public async Task<MovieWithShowsResponseDto> CreateMovieWithShowsAsync(MovieWithShowsDto dto)
        {
            // Create movie
            var movie = _mapper.Map<Movie>(dto.Movie);
            movie.IsActive = true;
            await _unitOfWork.Movies.AddMovieAsync(movie);
            await _unitOfWork.CompleteAsync();

            // Create shows
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
            // Update movie
            var movie = await _unitOfWork.Movies.GetMovieByIdAsync(movieId)
                ?? throw MovieBuzzExceptions.NotFound($"Movie with ID {movieId} not found");

            _mapper.Map(dto.Movie, movie);
            await _unitOfWork.Movies.UpdateMovieAsync(movie);

            // Process shows
            var shows = new List<ShowResponseDto>();
            foreach (var showDto in dto.Shows)
            {
                if (showDto.ShowId.HasValue) // Update existing
                {
                    var show = await _unitOfWork.Shows.GetShowByIdAsync(showDto.ShowId.Value)
                        ?? throw MovieBuzzExceptions.NotFound($"Show with ID {showDto.ShowId} not found");

                    _mapper.Map(showDto, show);
                    await _unitOfWork.Shows.UpdateShowAsync(show);
                    shows.Add(_mapper.Map<ShowResponseDto>(show));
                }
                else // Create new
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