using Microsoft.AspNetCore.Mvc;
using MovieBuzz.Core.Data;

namespace MovieBuzz.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly TestSeederService _testSeeder;

        public TestController(TestSeederService testSeeder)
        {
            _testSeeder = testSeeder;
        }

        [HttpGet("users")]
        public IActionResult GetTestUsers()
        {
            var users = _testSeeder.GetTestUsers();
            return Ok(new
            {
                Message = "This is TEST DATA - not saved to database",
                Users = users
            });
        }
    }
}
