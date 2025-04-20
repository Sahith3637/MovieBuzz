using MovieBuzz.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace MovieBuzz.Core.Dtos.Bookings;

public class CreateBookingDto
{
    [Required(ErrorMessage = "User ID is required")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Show ID is required")]
    public int ShowId { get; set; }

    [Required(ErrorMessage = "Number of tickets is required")]
    [Range(1, 10, ErrorMessage = "Number of tickets must be between 1 and 10")]
    public int NumberOfTickets { get; set; }
}