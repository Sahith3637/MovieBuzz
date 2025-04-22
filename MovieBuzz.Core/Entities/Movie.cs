using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MovieBuzz.Core.Entities;

public partial class Movie
{
    [Key]
    [Column("MovieID")]
    public int MovieId { get; set; }

    [StringLength(100)]
    public string MovieName { get; set; } = null!;

    [StringLength(50)]
    public string Genre { get; set; } = null!;

    public int AgeRestriction { get; set; }

    public int Duration { get; set; }

    public string? Description { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    [Column("PosterImageURL")]
    public required string PosterImageUrl { get; set; }

    [Column("TrailerURL")]
    public string? TrailerUrl { get; set; }

    public bool IsActive { get; set; }

    [StringLength(50)]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    [StringLength(50)]
    public string UpdatedBy { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime UpdatedOn { get; set; }

    [InverseProperty("Movie")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [InverseProperty("Movie")]
    public virtual ICollection<Show> Shows { get; set; } = new List<Show>();
}
