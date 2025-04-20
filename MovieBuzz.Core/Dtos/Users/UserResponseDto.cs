using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Core.Dtos.Users
{
    public class UserResponseDto
    {
        public int UserId { get; set; }
        public required string FullName { get; set; } // Combined first+last
        public DateOnly DateOfBirth { get; set; }
        public required string EmailId { get; set; }
        public required string PhoneNo { get; set; }
        public required string UserName { get; set; }
        public required string Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
