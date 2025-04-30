using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Core.Dtos.Movies
{
    public class MovieWithShowsDto
    {
        public CreateMovieDto Movie { get; set; }
        public List<ShowCreationDto> Shows { get; set; } = new();
    }
}
