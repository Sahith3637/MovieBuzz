using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace MovieBuzz.Core.DTOs.Shows;

public class CreateShowDto
{
    [Required]
    public int MovieId { get; set; }

    [Required]
    [StringLength(20)]
    public string ShowTime { get; set; } = null!;

    [Required]
    public DateOnly ShowDate { get; set; }

    [Range(1, 100)]
    public int AvailableSeats { get; set; }
}
