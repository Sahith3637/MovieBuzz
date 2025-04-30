using MovieBuzz.Core.DTOs.Shows;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Core.Dtos.Movies
{
    public class ShowCreationDto
    {
        [Required]
        [RegularExpression(@"^(1[0-2]|0?[1-9]):[0-5][0-9] (AM|PM)$")]
        public string ShowTime { get; set; }

        [Required]
        [NoPastDate]
        public DateOnly ShowDate { get; set; }

        [Required]
        [Range(1, 100)]
        public int AvailableSeats { get; set; }
    }
}
