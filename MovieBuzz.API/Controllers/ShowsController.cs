//using Microsoft.AspNetCore.Mvc;
//using MovieBuzz.Core.Dtos.Shows;
//using MovieBuzz.Services.Interfaces;
//using Microsoft.Extensions.Logging;
//using MovieBuzz.Core.DTOs.Shows;

//namespace MovieBuzz.API.Controllers
//{
//    [Route("[controller]")]
//    [ApiController]
//    public class ShowsController : ControllerBase
//    {
//        private readonly IShowService _showService;
//        private readonly ILogger<ShowsController> _logger;

//        public ShowsController(
//            IShowService showService,
//            ILogger<ShowsController> logger)
//        {
//            _showService = showService;
//            _logger = logger;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAllShows()
//        {
//            _logger.LogInformation("Fetching all shows");

//            var shows = await _showService.GetAllShowsAsync();

//            _logger.LogInformation("Retrieved {ShowCount} shows", shows.Count());

//            return Ok(new
//            {
//                Success = true,
//                Data = shows,
//                Message = "All shows retrieved successfully"
//            });
//        }

//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetShow(int id)
//        {
//            _logger.LogDebug("Fetching show {ShowId}", id);

//            var show = await _showService.GetShowByIdAsync(id);

//            _logger.LogInformation("Retrieved show {ShowId}", id);

//            return Ok(new
//            {
//                Success = true,
//                Data = show,
//                Message = "Show retrieved successfully"
//            });
//        }

//        [HttpGet("movie/{movieId}")]
//        public async Task<IActionResult> GetShowsByMovie(int movieId)
//        {
//            _logger.LogInformation("Fetching shows for movie {MovieId}", movieId);

//            var shows = await _showService.GetShowsByMovieIdAsync(movieId);

//            _logger.LogInformation("Found {ShowCount} shows for movie {MovieId}",
//                shows.Count(), movieId);

//            return Ok(new
//            {
//                Success = true,
//                Data = shows,
//                Message = $"Shows for movie {movieId} retrieved successfully"
//            });
//        }

//        [HttpPost]
//        public async Task<IActionResult> AddShow([FromBody] CreateShowDto showDto)
//        {
//            _logger.LogInformation(
//                "Adding new show for movie {MovieId} at {ShowTime} on {ShowDate}",
//                showDto.MovieId,
//                showDto.ShowTime,
//                showDto.ShowDate);

//            var show = await _showService.AddShowAsync(showDto);

//            _logger.LogInformation(
//                "Show {ShowId} created successfully with {AvailableSeats} seats available",
//                show.ShowId,
//                show.AvailableSeats);

//            return CreatedAtAction(
//                nameof(GetShow),
//                new { id = show.ShowId },
//                new
//                {
//                    Success = true,
//                    Data = show,
//                    Message = "Show added successfully"
//                });
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateShow(int id, [FromBody] ShowDto showDto)
//        {
//            _logger.LogInformation("Updating show {ShowId}", id);

//            var show = await _showService.UpdateShowAsync(id, showDto);

//            _logger.LogInformation(
//                "Show {ShowId} updated successfully. New available seats: {AvailableSeats}",
//                id,
//                show.AvailableSeats);

//            return Ok(new
//            {
//                Success = true,
//                Data = show,
//                Message = "Show updated successfully"
//            });
//        }
//    }
//}


using Microsoft.AspNetCore.Mvc;
using MovieBuzz.Core.Dtos.Shows;
using MovieBuzz.Services.Interfaces;
using Microsoft.Extensions.Logging;
using MovieBuzz.Core.DTOs.Shows;

namespace MovieBuzz.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        private readonly IShowService _showService;
        private readonly ILogger<ShowsController> _logger;

        public ShowsController(
            IShowService showService,
            ILogger<ShowsController> logger)
        {
            _showService = showService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShows()
        {
            _logger.LogInformation("Fetching all shows");

            var shows = await _showService.GetAllShowsAsync();

            _logger.LogInformation("Retrieved {ShowCount} shows", shows?.Count() ?? 0);

            return Ok(new
            {
                Success = true,
                Data = shows,
                Message = "All shows retrieved successfully"
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShow(int id)
        {
            _logger.LogDebug("Fetching show {ShowId}", id);

            var show = await _showService.GetShowByIdAsync(id);

            if (show == null)
            {
                _logger.LogWarning("Show {ShowId} not found", id);
                return NotFound(new
                {
                    Success = false,
                    Message = "Show not found"
                });
            }

            _logger.LogInformation("Retrieved show {ShowId}", id);

            return Ok(new
            {
                Success = true,
                Data = show,
                Message = "Show retrieved successfully"
            });
        }

        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetShowsByMovie(int movieId)
        {
            _logger.LogInformation("Fetching shows for movie {MovieId}", movieId);

            var shows = await _showService.GetShowsByMovieIdAsync(movieId);

            _logger.LogInformation("Found {ShowCount} shows for movie {MovieId}",
                shows?.Count() ?? 0, movieId);

            return Ok(new
            {
                Success = true,
                Data = shows,
                Message = $"Shows for movie {movieId} retrieved successfully"
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddShow([FromBody] CreateShowDto showDto)
        {
            _logger.LogInformation(
                "Adding new show for movie {MovieId} at {ShowTime} on {ShowDate}",
                showDto.MovieId,
                showDto.ShowTime,
                showDto.ShowDate);

            var show = await _showService.AddShowAsync(showDto);

            _logger.LogInformation(
                "Show {ShowId} created successfully with {AvailableSeats} seats available",
                show?.ShowId ?? 0,
                show?.AvailableSeats ?? 0);

            return CreatedAtAction(
                nameof(GetShow),
                new { id = show?.ShowId ?? 0 },
                new
                {
                    Success = true,
                    Data = show,
                    Message = "Show added successfully"
                });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShow(int id, [FromBody] ShowDto showDto)
        {
            _logger.LogInformation("Updating show {ShowId}", id);

            var show = await _showService.UpdateShowAsync(id, showDto);

            if (show == null)
            {
                _logger.LogWarning("Show {ShowId} not found for update", id);
                return NotFound(new
                {
                    Success = false,
                    Message = "Show not found"
                });
            }

            _logger.LogInformation(
                "Show {ShowId} updated successfully. New available seats: {AvailableSeats}",
                id,
                show.AvailableSeats);

            return Ok(new
            {
                Success = true,
                Data = show,
                Message = "Show updated successfully"
            });
        }
    }
}
