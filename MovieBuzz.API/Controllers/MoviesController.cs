using Microsoft.AspNetCore.Mvc;
using MovieBuzz.Core.Dtos.Movies;
using MovieBuzz.Services.Interfaces;
using MovieBuzz.Core.Exceptions;
using MovieBuzz.Core.Dtos.Shows;
using MovieBuzz.Core.DTOs.Shows;
using MovieBuzz.Services.Services;

namespace MovieBuzz.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMovies()
        {
            try
            {
                var movies = await _movieService.GetAllMoviesAsync();
                return Ok(new
                {
                    Success = true,
                    Data = movies,
                    Message = "All movies retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Error retrieving movies: {ex.Message}"
                });
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveMovies()
        {
            try
            {
                var movies = await _movieService.GetActiveMoviesAsync();
                return Ok(new
                {
                    Success = true,
                    Data = movies,
                    Message = "Active movies retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Error retrieving active movies: {ex.Message}"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovie(int id)
        {
            try
            {
                var movie = await _movieService.GetMovieByIdAsync(id);
                return Ok(new
                {
                    Success = true,
                    Data = movie,
                    Message = "Movie retrieved successfully"
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddMovie([FromBody] CreateMovieDto movieDto)
        {
            try
            {
                var movie = await _movieService.AddMovieAsync(movieDto);
                return CreatedAtAction(nameof(GetMovie), new { id = movie.MovieId }, new
                {
                    Success = true,
                    Data = movie,
                    Message = "Movie added successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = $"Error adding movie: {ex.Message}"
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieDto movieDto)
        {
            try
            {
                var movie = await _movieService.UpdateMovieAsync(id, movieDto);
                return Ok(new
                {
                    Success = true,
                    Data = movie,
                    Message = "Movie updated successfully"
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = $"Error updating movie: {ex.Message}"
                });
            }
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleMovieStatus(int id)
        {
            try
            {
                var result = await _movieService.ToggleMovieStatusAsync(id);
                if (result)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "Movie status toggled successfully"
                    });
                }
                return NotFound(new
                {
                    Success = false,
                    Message = "Movie not found"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Error toggling movie status: {ex.Message}"
                });
            }
        }

        // added
        [HttpPost("with-shows")]
        public async Task<IActionResult> CreateMovieWithShows([FromBody] MovieWithShowsDto dto)
        {
            try
            {
                var result = await _movieService.CreateMovieWithShowsAsync(dto);
                return CreatedAtAction(nameof(GetMovie), new { id = result.Movie.MovieId }, new
                {
                    Success = true,
                    Data = result,
                    Message = $"Movie and {result.Shows.Count} shows created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Error creating movie with shows: {ex.Message}"
                });
            }
        }

        [HttpPut("with-shows/{movieId}")]
        public async Task<IActionResult> UpdateMovieWithShows(int movieId, [FromBody] UpdateMovieWithShowsDto dto)
        {
            try
            {
                var result = await _movieService.UpdateMovieWithShowsAsync(movieId, dto);
                return Ok(new
                {
                    Success = true,
                    Data = result,
                    Message = $"Movie and {result.Shows.Count} shows updated successfully"
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Error updating movie with shows: {ex.Message}"
                });
            }
        }

        //[HttpPost("with-shows")]
        //public async Task<IActionResult> CreateMovieWithShows([FromBody] MovieWithShowsDto dto)
        //{
        //    try
        //    {
        //        // 1. Create the movie first
        //        var movieResponse = await _movieService.AddMovieAsync(dto.Movie);

        //        // 2. Prepare shows with the generated movieId
        //        var showsToCreate = dto.Shows.Select(s => new CreateShowDto
        //        {
        //            MovieId = movieResponse.MovieId,
        //            ShowTime = s.ShowTime,
        //            ShowDate = s.ShowDate,
        //            AvailableSeats = s.AvailableSeats
        //        }).ToList();

        //        // 3. Create all shows
        //        var createdShows = new List<ShowResponseDto>();
        //        foreach (var showDto in showsToCreate)
        //        {
        //            var show = await _showService.AddShowAsync(showDto);
        //            createdShows.Add(show);
        //        }

        //        return CreatedAtAction(nameof(GetMovie), new { id = movieResponse.MovieId }, new
        //        {
        //            Success = true,
        //            Data = new
        //            {
        //                Movie = movieResponse,
        //                Shows = createdShows
        //            },
        //            Message = $"Movie and {createdShows.Count} shows created successfully"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            Success = false,
        //            Message = $"Error creating movie with shows: {ex.Message}"
        //        });
        //    }
        //}
    }
}