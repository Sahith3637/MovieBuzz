using System;
using System.ComponentModel.DataAnnotations;

namespace MovieBuzz.Core.DTOs.Shows;

public class CreateShowDto
{
    [Required]
    public int MovieId { get; set; }

    [Required]
    [RegularExpression(@"^(1[0-2]|0?[1-9]):[0-5][0-9] (AM|PM)$",
        ErrorMessage = "Show time must be in 12-hour format (e.g., 9:00 AM or 12:30 PM)")]
    public string ShowTime { get; set; } = null!;

    [Required]
    [NoPastDate]
    [WithinThirtyDays(ErrorMessage = "Show date cannot be more than 30 days from today")]
    public DateOnly ShowDate { get; set; }

    [Required]
    [Range(1, 100, ErrorMessage = "Available seats must be between 1 and 100")]
    public int AvailableSeats { get; set; }
}

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

public class WithinThirtyDaysAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is DateOnly date)
        {
            var maxDate = DateOnly.FromDateTime(DateTime.Today.AddDays(30));
            return date <= maxDate;
        }
        return false;
    }
}