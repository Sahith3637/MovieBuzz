using System.ComponentModel.DataAnnotations;

namespace MovieBuzz.Core.Dtos.Bookings;

public class CreateBookingDto
{
    [Required(ErrorMessage = "User ID is required")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Show ID is required")]
    public int ShowId { get; set; }

    [Required(ErrorMessage = "Movie ID is required")]
    public int MovieId { get; set; }

    [Required(ErrorMessage = "Number of tickets is required")]
    [Range(1, 6, ErrorMessage = "Number of tickets must be between 1 and 6")]
    public int NumberOfTickets { get; set; }
}