//using Microsoft.AspNetCore.Mvc;
//using MovieBuzz.Core.Dtos.Movies;
//using MovieBuzz.Services.Interfaces;
//using Microsoft.Extensions.Logging;

//namespace MovieBuzz.API.Controllers
//{
//    [Route("[controller]")]
//    [ApiController]
//    public class MoviesController : ControllerBase
//    {
//        private readonly IMovieService _movieService;
//        private readonly ILogger<MoviesController> _logger;

//        public MoviesController(
//            IMovieService movieService,
//            ILogger<MoviesController> logger)
//        {
//            _movieService = movieService;
//            _logger = logger;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAllMovies()
//        {
//            _logger.LogInformation("Fetching all movies");

//            var movies = await _movieService.GetAllMoviesAsync();

//            _logger.LogInformation("Retrieved {MovieCount} movies", movies.Count());

//            return Ok(new
//            {
//                Success = true,
//                Data = movies,
//                Message = "All movies retrieved successfully"
//            });
//        }

//        [HttpGet("active")]
//        public async Task<IActionResult> GetActiveMovies()
//        {
//            _logger.LogInformation("Fetching active movies");

//            var movies = await _movieService.GetActiveMoviesAsync();

//            _logger.LogInformation("Retrieved {ActiveMovieCount} active movies", movies.Count());

//            return Ok(new
//            {
//                Success = true,
//                Data = movies,
//                Message = "Active movies retrieved successfully"
//            });
//        }

//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetMovie(int id)
//        {
//            _logger.LogInformation("Fetching movie with ID {MovieId}", id);

//            var movie = await _movieService.GetMovieByIdAsync(id);

//            _logger.LogInformation("Movie {MovieId} retrieved successfully", id);

//            return Ok(new
//            {
//                Success = true,
//                Data = movie,
//                Message = "Movie retrieved successfully"
//            });
//        }

//        [HttpPost]
//        public async Task<IActionResult> AddMovie([FromBody] CreateMovieDto movieDto)
//        {
//            _logger.LogInformation("Adding new movie: {MovieTitle}", movieDto.MovieName);

//            var movie = await _movieService.AddMovieAsync(movieDto);

//            _logger.LogInformation("Movie {MovieId} added successfully", movie.MovieId);

//            return CreatedAtAction(nameof(GetMovie), new { id = movie.MovieId }, new
//            {
//                Success = true,
//                Data = movie,
//                Message = "Movie added successfully"
//            });
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieDto movieDto)
//        {
//            _logger.LogInformation("Updating movie {MovieId}", id);

//            var movie = await _movieService.UpdateMovieAsync(id, movieDto);

//            _logger.LogInformation("Movie {MovieId} updated successfully", id);

//            return Ok(new
//            {
//                Success = true,
//                Data = movie,
//                Message = "Movie updated successfully"
//            });
//        }

//        [HttpPatch("{id}/toggle-status")]
//        public async Task<IActionResult> ToggleMovieStatus(int id)
//        {
//            _logger.LogInformation("Toggling status for movie {MovieId}", id);

//            var result = await _movieService.ToggleMovieStatusAsync(id);

//            if (result)
//            {
//                _logger.LogInformation("Successfully toggled status for movie {MovieId}", id);
//                return Ok(new
//                {
//                    Success = true,
//                    Message = "Movie status toggled successfully"
//                });
//            }

//            _logger.LogWarning("Movie {MovieId} not found for status toggle", id);
//            return NotFound(new
//            {
//                Success = false,
//                Message = "Movie not found"
//            });
//        }

//        [HttpPost("with-shows")]
//        public async Task<IActionResult> CreateMovieWithShows([FromBody] MovieWithShowsDto dto)
//        {
//            _logger.LogInformation("Creating movie with {ShowCount} shows", dto.Shows.Count);

//            var result = await _movieService.CreateMovieWithShowsAsync(dto);

//            _logger.LogInformation(
//                "Created movie {MovieId} with {ShowCount} shows",
//                result.Movie.MovieId,
//                result.Shows.Count);

//            return CreatedAtAction(
//                nameof(GetMovie),
//                new { id = result.Movie.MovieId },
//                new
//                {
//                    Success = true,
//                    Data = result,
//                    Message = $"Movie and {result.Shows.Count} shows created successfully"
//                });
//        }

//        [HttpPut("with-shows/{movieId}")]
//        public async Task<IActionResult> UpdateMovieWithShows(int movieId, [FromBody] UpdateMovieWithShowsDto dto)
//        {
//            _logger.LogInformation(
//                "Updating movie {MovieId} with {ShowCount} shows",
//                movieId,
//                dto.Shows.Count);

//            var result = await _movieService.UpdateMovieWithShowsAsync(movieId, dto);

//            _logger.LogInformation(
//                "Updated movie {MovieId} with {ShowCount} shows",
//                movieId,
//                result.Shows.Count);

//            return Ok(new
//            {
//                Success = true,
//                Data = result,
//                Message = $"Movie and {result.Shows.Count} shows updated successfully"
//            });
//        }
//    }
//}


using Microsoft.AspNetCore.Mvc;
using MovieBuzz.Core.Dtos.Movies;
using MovieBuzz.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace MovieBuzz.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(
            IMovieService movieService,
            ILogger<MoviesController> logger)
        {
            _movieService = movieService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMovies()
        {
            _logger.LogInformation("Fetching all movies");

            var movies = await _movieService.GetAllMoviesAsync();

            _logger.LogInformation("Retrieved {MovieCount} movies", movies?.Count() ?? 0);

            return Ok(new
            {
                Success = true,
                Data = movies,
                Message = "All movies retrieved successfully"
            });
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveMovies()
        {
            _logger.LogInformation("Fetching active movies");

            var movies = await _movieService.GetActiveMoviesAsync();

            _logger.LogInformation("Retrieved {ActiveMovieCount} active movies", movies?.Count() ?? 0);

            return Ok(new
            {
                Success = true,
                Data = movies,
                Message = "Active movies retrieved successfully"
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovie(int id)
        {
            _logger.LogInformation("Fetching movie with ID {MovieId}", id);

            var movie = await _movieService.GetMovieByIdAsync(id);

            if (movie == null)
            {
                _logger.LogWarning("Movie {MovieId} not found", id);
                return NotFound(new
                {
                    Success = false,
                    Message = "Movie not found"
                });
            }

            _logger.LogInformation("Movie {MovieId} retrieved successfully", id);

            return Ok(new
            {
                Success = true,
                Data = movie,
                Message = "Movie retrieved successfully"
            });
        }

        //[HttpPost]
        //public async Task<IActionResult> AddMovie([FromBody] CreateMovieDto movieDto)
        //{
        //    _logger.LogInformation("Adding new movie: {MovieTitle}", movieDto.MovieName);

        //    var movie = await _movieService.AddMovieAsync(movieDto);

        //    _logger.LogInformation("Movie {MovieId} added successfully", movie.MovieId);

        //    return CreatedAtAction(nameof(GetMovie), new { id = movie.MovieId }, new
        //    {
        //        Success = true,
        //        Data = movie,
        //        Message = "Movie added successfully"
        //    });
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieDto movieDto)
        //{
        //    _logger.LogInformation("Updating movie {MovieId}", id);

        //    var movie = await _movieService.UpdateMovieAsync(id, movieDto);

        //    if (movie == null)
        //    {
        //        _logger.LogWarning("Movie {MovieId} not found", id);
        //        return NotFound(new
        //        {
        //            Success = false,
        //            Message = "Movie not found"
        //        });
        //    }

        //    _logger.LogInformation("Movie {MovieId} updated successfully", id);

        //    return Ok(new
        //    {
        //        Success = true,
        //        Data = movie,
        //        Message = "Movie updated successfully"
        //    });
        //}

        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleMovieStatus(int id)
        {
            _logger.LogInformation("Toggling status for movie {MovieId}", id);

            var result = await _movieService.ToggleMovieStatusAsync(id);

            if (result)
            {
                _logger.LogInformation("Successfully toggled status for movie {MovieId}", id);
                return Ok(new
                {
                    Success = true,
                    Message = "Movie status toggled successfully"
                });
            }

            _logger.LogWarning("Movie {MovieId} not found for status toggle", id);
            return NotFound(new
            {
                Success = false,
                Message = "Movie not found"
            });
        }

        [HttpPost("with-shows")]
        public async Task<IActionResult> CreateMovieWithShows([FromBody] MovieWithShowsDto dto)
        {
            _logger.LogInformation("Creating movie with {ShowCount} shows", dto.Shows?.Count ?? 0);

            var result = await _movieService.CreateMovieWithShowsAsync(dto);

            _logger.LogInformation(
                "Created movie {MovieId} with {ShowCount} shows",
                result.Movie.MovieId,
                result.Shows?.Count ?? 0);

            return CreatedAtAction(
                nameof(GetMovie),
                new { id = result.Movie.MovieId },
                new
                {
                    Success = true,
                    Data = result,
                    Message = $"Movie and {result.Shows?.Count ?? 0} shows created successfully"
                });
        }

        [HttpPut("with-shows/{movieId}")]
        public async Task<IActionResult> UpdateMovieWithShows(int movieId, [FromBody] UpdateMovieWithShowsDto dto)
        {
            _logger.LogInformation(
                "Updating movie {MovieId} with {ShowCount} shows",
                movieId,
                dto.Shows?.Count ?? 0);

            var result = await _movieService.UpdateMovieWithShowsAsync(movieId, dto);

            if (result == null)
            {
                _logger.LogWarning("Movie {MovieId} not found for update", movieId);
                return NotFound(new
                {
                    Success = false,
                    Message = "Movie not found"
                });
            }

            _logger.LogInformation(
                "Updated movie {MovieId} with {ShowCount} shows",
                movieId,
                result.Shows?.Count ?? 0);

            return Ok(new
            {
                Success = true,
                Data = result,
                Message = $"Movie and {result.Shows?.Count ?? 0} shows updated successfully"
            });
        }
    }
}
