using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Core.Dtos.Shows
{
    public class ShowWithMovieDetailsDto : ShowDto
    {
        public int ShowId { get; set; }
        public string MovieName { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public int AgeRestriction { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string PosterImageUrl { get; set; } = null!;
        public string TrailerUrl { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}