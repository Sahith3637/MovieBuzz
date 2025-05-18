

using AutoMapper;
using MovieBuzz.Core.Dtos.Bookings;
using MovieBuzz.Core.Dtos.Movies;
using MovieBuzz.Core.Dtos.Shows;
using MovieBuzz.Core.DTOs.Shows;
using MovieBuzz.Core.Dtos.Users;
using MovieBuzz.Core.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        
        CreateUserMappings();

        
        CreateMovieMappings();

        
        CreateShowMappings();

      
        CreateBookingMappings();
    }

    private void CreateUserMappings()
    {
        
        CreateMap<RegisterUserDto, User>()
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(_ => "User")) 
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.UtcNow));

        
        CreateMap<User, UserResponseDto>()
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
    }

    private void CreateMovieMappings()
    {
       
        CreateMap<CreateMovieDto, Movie>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(_ => DateTime.UtcNow));

        
        CreateMap<MovieDto, Movie>()
            .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(_ => DateTime.UtcNow));

       
        CreateMap<Movie, MovieResponseDto>();

        
        CreateMap<Movie, MovieSummaryDto>();
    }

    private void CreateShowMappings()
    {
        // CreateShowDto → Show
        CreateMap<CreateShowDto, Show>();

        // ShowDto → Show
        CreateMap<ShowDto, Show>();
        CreateMap<Show, ShowDto>();

        //Show → ShowResponseDto
        CreateMap<Show, ShowResponseDto>()
       .IncludeBase<Show, ShowDto>();
        //     .ForMember(dest => dest.MovieName,
        //opt => opt.MapFrom(src => src.Movie.MovieName));

        CreateMap<Show, ShowWithMovieDetailsDto>()
       .IncludeBase<Show, ShowDto>()
       .ForMember(dest => dest.ShowId, opt => opt.MapFrom(src => src.ShowId))
       .ForMember(dest => dest.MovieName, opt => opt.MapFrom(src => src.Movie.MovieName))
       .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Movie.Genre))
       .ForMember(dest => dest.AgeRestriction, opt => opt.MapFrom(src => src.Movie.AgeRestriction))
       .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Movie.Duration))
       .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Movie.Description))
       .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Movie.Price))
       .ForMember(dest => dest.PosterImageUrl, opt => opt.MapFrom(src => src.Movie.PosterImageUrl))
       .ForMember(dest => dest.TrailerUrl, opt => opt.MapFrom(src => src.Movie.TrailerUrl))
       .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Movie.IsActive));

    }



    private void CreateBookingMappings()
    {
        // CreateBookingDto → Booking
        CreateMap<CreateBookingDto, Booking>()
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.TotalPrice, opt => opt.Ignore()); // Will be set in service

        // Booking → BookingResponseDto
        CreateMap<Booking, BookingResponseDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.MovieName, opt => opt.MapFrom(src => src.Movie.MovieName))
            .ForMember(dest => dest.PosterImageUrl, opt => opt.MapFrom(src => src.Movie.PosterImageUrl))
            .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Movie.Genre))
            .ForMember(dest => dest.ShowTime, opt => opt.MapFrom(src => src.Show.ShowTime))
            .ForMember(dest => dest.ShowDate, opt => opt.MapFrom(src => src.Show.ShowDate));

        // Booking → AdminBookingDto
        CreateMap<Booking, AdminBookingDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.MovieName, opt => opt.MapFrom(src => src.Movie.MovieName))
            .ForMember(dest => dest.ShowTime, opt => opt.MapFrom(src => src.Show.ShowTime))
            .ForMember(dest => dest.ShowDate, opt => opt.MapFrom(src => src.Show.ShowDate));
    }
}







