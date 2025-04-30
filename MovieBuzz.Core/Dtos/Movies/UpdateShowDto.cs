using MovieBuzz.Core.Dtos.Shows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Core.Dtos.Movies
{
    public class UpdateShowDto : ShowDto
    {
        public int? ShowId { get; set; } // Null means new show
    }
}
