using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Core.Exceptions
{
    public class MovieNotFoundException : Exception
    {
        public MovieNotFoundException() { }
        public MovieNotFoundException(string message) : base(message) { }
        public MovieNotFoundException(string message, Exception inner)
            : base(message, inner) { }
    }
}
