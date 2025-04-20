using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MovieBuzz.Core.Entities;

[Index("EmailId", Name = "UQ__Users__7ED91AEE3D199261", IsUnique = true)]
[Index("UserName", Name = "UQ__Users__C9F28456C72214F5", IsUnique = true)]
[Index("PhoneNo", Name = "UQ__Users__F3EE33E27D21F13E", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [StringLength(50)]
    public string LastName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    [Column("EmailID")]
    [StringLength(100)]
    public string EmailId { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string PhoneNo { get; set; } = null!;

    [StringLength(50)]
    public string UserName { get; set; } = null!;

    [StringLength(50)]
    public string Password { get; set; } = null!;

    [StringLength(20)]
    public string Role { get; set; } = null!;

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
