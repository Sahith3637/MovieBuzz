using Microsoft.AspNetCore.Mvc;
using MovieBuzz.Core.Dtos.Movies;
using MovieBuzz.Services.Interfaces;
using MovieBuzz.Core.Exceptions;

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
    }
}