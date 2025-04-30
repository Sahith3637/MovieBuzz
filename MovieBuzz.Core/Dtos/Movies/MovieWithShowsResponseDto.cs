using MovieBuzz.Core.Dtos.Shows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Core.Dtos.Movies
{
    public class MovieWithShowsResponseDto
    {
        public MovieResponseDto Movie { get; set; }
        public List<ShowResponseDto> Shows { get; set; } = new();
    }
}
