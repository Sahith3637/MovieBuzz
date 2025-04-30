using MovieBuzz.Core.DTOs.Shows;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Core.Dtos.Movies
{
    public class UpdateMovieWithShowsDto
    {
        public MovieDto Movie { get; set; }
        public List<UpdateShowDto> Shows { get; set; } = new();
    }
}
