using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Core.Dtos.Shows
{
    public class ShowDto
    {
        [Required(ErrorMessage = "Movie ID is required")]
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Show time is required")]
        [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Show time must be in HH:MM format")]
        public required string ShowTime { get; set; }

        [Required(ErrorMessage = "Show date is required")]
        public required DateOnly ShowDate { get; set; }

        [Required(ErrorMessage = "Available seats is required")]
        [Range(1, 100, ErrorMessage = "Available seats must be between 1 and 100")]
        public int AvailableSeats { get; set; }
        public bool IsHouseFull => AvailableSeats <= 0;
    }
}
