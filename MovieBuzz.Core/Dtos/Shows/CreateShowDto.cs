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
    [NoPastDate(ErrorMessage = "Show date cannot be in the past")]
    public DateOnly ShowDate { get; set; }

    [Range(1, 100)]
    public int AvailableSeats { get; set; }
}
  
// Add this custom validation attribute
public class NoPastDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is DateOnly date)
        {
            return date >= DateOnly.FromDateTime(DateTime.Today);
        }
        return false;
    }
}
