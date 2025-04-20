using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Core.Exceptions
{
    // 404 Not Found
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    // 401 Unauthorized
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }

    // 409 Conflict
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}
