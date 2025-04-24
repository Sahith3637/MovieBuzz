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

        public async Task<IEnumerable<ShowResponseDto>> GetShowsByMovieIdAsync(int movieId)
        {
            var shows = await _unitOfWork.Shows.GetShowsByMovieIdAsync(movieId);
            return _mapper.Map<IEnumerable<ShowResponseDto>>(shows);
        }

        public async Task<ShowResponseDto> AddShowAsync(CreateShowDto showDto)
        {
            var movie = await _unitOfWork.Movies.GetMovieByIdAsync(showDto.MovieId)
                ?? throw MovieBuzzExceptions.NotFound($"Movie with ID {showDto.MovieId} not found");

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