using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MovieBuzz.Core.Entities;

public partial class Booking
{
    [Key]
    [Column("BookingID")]
    public int BookingId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [Column("ShowID")]
    public int ShowId { get; set; }

    [Column("MovieID")]
    public int MovieId { get; set; }

    public int NumberOfTickets { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal TotalPrice { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    [ForeignKey("MovieId")]
    [InverseProperty("Bookings")]
    public virtual Movie Movie { get; set; } = null!;

    [ForeignKey("ShowId")]
    [InverseProperty("Bookings")]
    public virtual Show Show { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Bookings")]
    public virtual User User { get; set; } = null!;
}
