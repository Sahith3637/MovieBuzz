

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
        // User Mappings
        CreateUserMappings();

        // Movie Mappings
        CreateMovieMappings();

        // Show Mappings
        CreateShowMappings();

        // Booking Mappings
        CreateBookingMappings();
    }

    private void CreateUserMappings()
    {
        // RegisterUserDto → User
        CreateMap<RegisterUserDto, User>()
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(_ => "User")) // Default role
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // User → UserResponseDto
        CreateMap<User, UserResponseDto>()
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
    }

    private void CreateMovieMappings()
    {
        // CreateMovieDto → Movie
        CreateMap<CreateMovieDto, Movie>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // MovieDto → Movie (for updates)
        CreateMap<MovieDto, Movie>()
            .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // Movie → MovieResponseDto
        CreateMap<Movie, MovieResponseDto>();

        // Movie → MovieSummaryDto
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



















//using AutoMapper;
//using MovieBuzz.Core.Dtos.Bookings;
//using MovieBuzz.Core.Dtos.Movies;
//using MovieBuzz.Core.Dtos.Shows;
//using MovieBuzz.Core.Dtos.Users;
//using MovieBuzz.Core.Entities;

//namespace MovieBuzz.Core.Mapping;

//public class MappingProfile : Profile
//{
//    public MappingProfile()
//    {

//        CreateMap<RegisterUserDto, User>()
//        .ForMember(dest => dest.Password, opt => opt.Ignore()) // Hashed separately
//        .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.Now))
//        .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
//        .ForMember(dest => dest.Role, opt => opt.MapFrom(_ => "user"));

//        CreateMap<User, UserResponseDto>()
//            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));



//        CreateMap<Movie, MovieDto>()
//            .ReverseMap()
//            .ForMember(dest => dest.Bookings, opt => opt.Ignore())
//            .ForMember(dest => dest.Shows, opt => opt.Ignore());

//        CreateMap<Movie, MovieResponseDto>();

//        CreateMap<CreateMovieDto, Movie>()
//        .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.Now))
//        .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(_ => DateTime.Now))
//        .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
//        .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(_ => "admin"))
//        .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(_ => "admin"));

//        CreateMap<Show, ShowDto>()
//        .ReverseMap()
//        .ForMember(dest => dest.Bookings, opt => opt.Ignore())
//        .ForMember(dest => dest.Movie, opt => opt.Ignore());

//        CreateMap<Show, ShowResponseDto>();



//        CreateMap<CreateBookingDto, Booking>()
//            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.Now));

//        CreateMap<Booking, BookingResponseDto>()
//        .ForMember(dest => dest.MovieName, opt => opt.MapFrom(src => src.Movie.MovieName))
//        .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Movie.Genre))
//        .ForMember(dest => dest.PosterImageUrl, opt => opt.MapFrom(src => src.Movie.PosterImageUrl))
//        .ForMember(dest => dest.ShowTime, opt => opt.MapFrom(src => src.Show.ShowTime))
//        .ForMember(dest => dest.ShowDate, opt => opt.MapFrom(src => src.Show.ShowDate));

//        CreateMap<Booking, AdminBookingDto>()
//            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
//            .ForMember(dest => dest.MovieName, opt => opt.MapFrom(src => src.Movie.MovieName))
//            .ForMember(dest => dest.ShowTime, opt => opt.MapFrom(src => src.Show.ShowTime))
//            .ForMember(dest => dest.ShowDate, opt => opt.MapFrom(src => src.Show.ShowDate));
//    }
//}






//using AutoMapper;
//using MovieBuzz.Core.Dtos.Bookings;
//using MovieBuzz.Core.Dtos.Movies;
//using MovieBuzz.Core.Dtos.Shows;
//using MovieBuzz.Core.DTOs.Bookings;
//using MovieBuzz.Core.DTOs.Shows;
//using MovieBuzz.Core.Entities;

//namespace MovieBuzz.Core.Mapping;

//public class MappingProfile : Profile
//{
//    public MappingProfile()
//    {
//        // Movie Mappings
//        CreateMap<CreateMovieDto, Movie>()
//            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.Now))
//            .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(_ => DateTime.Now))
//            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));

//        CreateMap<Movie, MovieResponseDto>();

//        // Show Mappings
//        CreateMap<CreateShowDto, Show>();
//        CreateMap<Show, ShowResponseDto>();

//        // Booking Mappings
//        CreateMap<CreateBookingDto, Booking>()
//            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(_ => DateTime.Now))
//            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src =>
//                src.NumberOfTickets * src.Show!.Movie.Price)); // Null-forgiving operator

//        CreateMap<Booking, BookingResponseDto>()
//            .ForMember(dest => dest.MovieName, opt => opt.MapFrom(src => src.Movie.MovieName))
//            .ForMember(dest => dest.ShowTime, opt => opt.MapFrom(src => src.Show.ShowTime));
//    }
//}