namespace MovieBuzz.Core.Exceptions
{
    // Static factory class for creating exceptions
    public static class MovieBuzzExceptions
    {
        // 400 Bad Request for business rule violations
        public static BusinessRuleException BusinessRule(string message)
            => new BusinessRuleException(message);

        // 404 Not Found
        public static NotFoundException NotFound(string message)
            => new NotFoundException(message);

        // 401 Unauthorized
        public static UnauthorizedException Unauthorized(string message)
            => new UnauthorizedException(message);

        // 409 Conflict
        public static ConflictException Conflict(string message)
            => new ConflictException(message);
    }

    // Specific exception classes
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }

    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}