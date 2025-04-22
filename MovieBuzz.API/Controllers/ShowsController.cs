using Microsoft.AspNetCore.Mvc;
using MovieBuzz.Core.Dtos.Shows;
using MovieBuzz.Core.DTOs.Shows;
using MovieBuzz.Services.Interfaces;

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
            var shows = await _showService.GetAllShowsAsync();
            return Ok(shows);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShow(int id)
        {
            var show = await _showService.GetShowByIdAsync(id);
            return Ok(show);
        }

        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetShowsByMovie(int movieId)
        {
            var shows = await _showService.GetShowsByMovieIdAsync(movieId);
            return Ok(shows);
        }

        [HttpPost]
        public async Task<IActionResult> AddShow([FromBody] CreateShowDto showDto)
        {
            var show = await _showService.AddShowAsync(showDto);
            return CreatedAtAction(nameof(GetShow), new { id = show.ShowId }, show);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShow(int id, [FromBody] ShowDto showDto)
        {
            var show = await _showService.UpdateShowAsync(id, showDto);
            return Ok(show);
        }
    }
}