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
            var movies = await _movieService.GetAllMoviesAsync();
            return Ok(movies);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveMovies()
        {
            var movies = await _movieService.GetActiveMoviesAsync();
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovie(int id)
        {
            try
            {
                var movie = await _movieService.GetMovieByIdAsync(id);
                return Ok(movie);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        //[ApiExplorerSettings(GroupName = "Admin")]
        public async Task<IActionResult> AddMovie([FromBody] CreateMovieDto movieDto)
        {
            var movie = await _movieService.AddMovieAsync(movieDto);
            return CreatedAtAction(nameof(GetMovie), new { id = movie.MovieId }, movie);
        }

        [HttpPut("{id}")]
        //[ApiExplorerSettings(GroupName = "Admin")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieDto movieDto)
        {
            try
            {
                var movie = await _movieService.UpdateMovieAsync(id, movieDto);
                return Ok(movie);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPatch("{id}/toggle-status")]
        //[ApiExplorerSettings(GroupName = "Admin")]
        public async Task<IActionResult> ToggleMovieStatus(int id)
        {
            var result = await _movieService.ToggleMovieStatusAsync(id);
            Response.Headers.Append("X-Status-Message", "Status toggled successfully");
            return result ? NoContent() : NotFound();
        }


    }
}