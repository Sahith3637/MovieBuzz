using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Core.Dtos.Bookings
{
    public class BookingResponseDto
    {
        public int BookingId { get; set; }
        public required string UserName { get; set; }
        public required string MovieName { get; set; }
        public required string PosterImageUrl { get; set; }
        public required string Genre { get; set; }
        public required string ShowTime { get; set; }
        public required DateOnly ShowDate { get; set; }
        public required int NumberOfTickets { get; set; }
        public required decimal TotalPrice { get; set; }
        public required DateTime CreatedOn { get; set; }

        public string FormattedDate => ShowDate.ToString("dd MMM yyyy");
        
    }
}
