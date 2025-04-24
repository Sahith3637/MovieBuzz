using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Core.Dtos.Movies
{
    public class MovieSummaryDto
    {
        public int MovieId { get; set; }
        //public required string MovieName { get; set; }
        public required string PosterImageUrl { get; set; }
        public required string Genre { get; set; }
        public required int Duration { get; set; }
    }
}
