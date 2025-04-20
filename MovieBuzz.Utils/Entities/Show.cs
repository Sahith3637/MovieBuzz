using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MovieBuzz.Core.Entities;

public partial class Show
{
    [Key]
    [Column("ShowID")]
    public int ShowId { get; set; }

    [Column("MovieID")]
    public int MovieId { get; set; }

    [StringLength(20)]
    public string ShowTime { get; set; } = null!;

    public DateOnly ShowDate { get; set; }

    public int AvailableSeats { get; set; }

    [InverseProperty("Show")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [ForeignKey("MovieId")]
    [InverseProperty("Shows")]
    public virtual Movie Movie { get; set; } = null!;
}
