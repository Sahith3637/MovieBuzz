using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Core.Dtos.Bookings
{
    public class AdminBookingDto
    {
        public int BookingId { get; set; }
        public string UserName { get; set; } = null!;
        public string MovieName { get; set; } = null!;
        public int NumberOfTickets { get; set; }
        public string ShowTime { get; set; } = null!;
        public DateOnly ShowDate { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
