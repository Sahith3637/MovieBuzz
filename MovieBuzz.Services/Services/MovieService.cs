using AutoMapper;
using MovieBuzz.Core.Dtos.Movies;
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
    }
}