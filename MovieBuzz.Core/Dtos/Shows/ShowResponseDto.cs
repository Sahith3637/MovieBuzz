using MovieBuzz.Core.Dtos.Movies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Core.Dtos.Shows
{
    public class ShowResponseDto : ShowDto
    {
        public int ShowId { get; set; }
        public required MovieSummaryDto Movie { get; set; }
    }
}
