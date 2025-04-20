using AutoMapper;
using MovieBuzz.Core.Dtos.Bookings;
using MovieBuzz.Core.Entities;
using MovieBuzz.Core.Exceptions;
using MovieBuzz.Repository.Interfaces;
using MovieBuzz.Services.Interfaces;

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
                ?? throw new NotFoundException($"Booking with ID {bookingId} not found");

            return _mapper.Map<BookingResponseDto>(booking);
        }

        public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto bookingDto)
        {
            // Validate user exists
            var user = await _unitOfWork.Users.GetUserByIdAsync(bookingDto.UserId)
                ?? throw new NotFoundException($"User with ID {bookingDto.UserId} not found");

            // Validate show exists and get with movie details
            var show = await _unitOfWork.Shows.GetShowWithMovieAsync(bookingDto.ShowId)
                ?? throw new NotFoundException($"Show with ID {bookingDto.ShowId} not found");

            // Check available seats
            if (show.AvailableSeats < bookingDto.NumberOfTickets)
            {
                throw new BusinessRuleException("Not enough available seats for this show");
            }

            // Create booking
            var booking = _mapper.Map<Booking>(bookingDto);
            booking.MovieId = show.MovieId;
            booking.TotalPrice = bookingDto.NumberOfTickets * show.Movie.Price;
            booking.CreatedOn = DateTime.UtcNow;

            // Update available seats
            show.AvailableSeats -= bookingDto.NumberOfTickets;

            // Save changes
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