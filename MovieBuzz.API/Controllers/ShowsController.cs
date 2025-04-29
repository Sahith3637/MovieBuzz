using Microsoft.AspNetCore.Mvc;
using MovieBuzz.Core.Dtos.Shows;
using MovieBuzz.Services.Interfaces;
using MovieBuzz.Core.Exceptions;
using MovieBuzz.Core.DTOs.Shows;

namespace MovieBuzz.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        private readonly IShowService _showService;

        public ShowsController(IShowService showService)
        {
            _showService = showService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShows()
        {
            try
            {
                var shows = await _showService.GetAllShowsAsync();
                return Ok(new
                {
                    Success = true,
                    Data = shows,
                    Message = "All shows retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Error retrieving shows: {ex.Message}"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShow(int id)
        {
            try
            {
                var show = await _showService.GetShowByIdAsync(id);
                return Ok(new
                {
                    Success = true,
                    Data = show,
                    Message = "Show retrieved successfully"
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

        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetShowsByMovie(int movieId)
        {
            try
            {
                var shows = await _showService.GetShowsByMovieIdAsync(movieId);
                return Ok(new
                {
                    Success = true,
                    Data = shows,
                    Message = $"Shows for movie {movieId} retrieved successfully"
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
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Error retrieving shows for movie: {ex.Message}"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddShow([FromBody] CreateShowDto showDto)
        {
            try
            {
                var show = await _showService.AddShowAsync(showDto);
                return CreatedAtAction(nameof(GetShow), new { id = show.ShowId }, new
                {
                    Success = true,
                    Data = show,
                    Message = "Show added successfully"
                });
            }
            catch (NotFoundException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Error adding show: {ex.Message}"
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShow(int id, [FromBody] ShowDto showDto)
        {
            try
            {
                var show = await _showService.UpdateShowAsync(id, showDto);
                return Ok(new
                {
                    Success = true,
                    Data = show,
                    Message = "Show updated successfully"
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
                    Message = $"Error updating show: {ex.Message}"
                });
            }
        }
    }
}