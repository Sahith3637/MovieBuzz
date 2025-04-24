using AutoMapper;
using MovieBuzz.Core.Dtos.Bookings;
using MovieBuzz.Core.Entities;
using MovieBuzz.Core.Exceptions;
using MovieBuzz.Repository.Interfaces;
using MovieBuzz.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieBuzz.Services.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BookingResponseDto> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _unitOfWork.Bookings.GetBookingByIdAsync(bookingId)
                ?? throw MovieBuzzExceptions.NotFound($"Booking with ID {bookingId} not found");

            return _mapper.Map<BookingResponseDto>(booking);
        }

        public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto bookingDto)
        {
            // Get show with movie information
            var show = await _unitOfWork.Shows.GetShowWithMovieAsync(bookingDto.ShowId)
                ?? throw MovieBuzzExceptions.NotFound("Show not found");

            // Get user
            var user = await _unitOfWork.Users.GetUserByIdAsync(bookingDto.UserId)
                ?? throw MovieBuzzExceptions.NotFound("User not found");

            // Validate seats
            if (show.AvailableSeats < bookingDto.NumberOfTickets)
            {
                throw MovieBuzzExceptions.BusinessRule($"Only {show.AvailableSeats} seats available");
            }

            // Create booking
            var booking = new Booking
            {
                UserId = bookingDto.UserId,
                ShowId = bookingDto.ShowId,
                MovieId = show.MovieId,
                NumberOfTickets = bookingDto.NumberOfTickets,
                TotalPrice = bookingDto.NumberOfTickets * show.Movie.Price
            };

            // Update available seats
            show.AvailableSeats -= bookingDto.NumberOfTickets;

            await _unitOfWork.Bookings.AddBookingAsync(booking);
            await _unitOfWork.Shows.UpdateShowAsync(show);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<BookingResponseDto>(booking);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync()
        {
            var bookings = await _unitOfWork.Bookings.GetAllBookingsAsync();
            return _mapper.Map<IEnumerable<BookingResponseDto>>(bookings);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetBookingsByUserIdAsync(int userId)
        {
            var bookings = await _unitOfWork.Bookings.GetBookingsByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<BookingResponseDto>>(bookings);
        }

        public async Task<IEnumerable<AdminBookingDto>> GetBookingsByMovieIdAsync(int movieId)
        {
            var bookings = await _unitOfWork.Bookings.GetBookingsByMovieIdAsync(movieId);
            return _mapper.Map<IEnumerable<AdminBookingDto>>(bookings);
        }
    }
}