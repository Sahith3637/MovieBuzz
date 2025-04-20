namespace MovieBuzz.Core.Exceptions
{
    // 400 Bad Request for business rule violations
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}