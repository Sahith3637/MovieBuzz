using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class RegisterUserDto
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters")]
    public required string LastName { get; set; }

    [Required(ErrorMessage = "Date of birth is required")]
    [DataType(DataType.Date)]
    [NoFutureDate(ErrorMessage = "Date of birth cannot be in the future")]
    [MinimumAge(3, ErrorMessage = "User must be at least 3 years old")]
    [MaximumAge(93, ErrorMessage = "User must be born after 1930")]
    public DateOnly DateOfBirth { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters")]
    [AllowedEmailDomains(new string[] { "@gmail.com", "@moviebuzz.com", "@yahoo.com", "@outlook.com", "@vivejaitservices.com" },
        ErrorMessage = "Email domain is not allowed")]
    public required string EmailId { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Phone number must be 10 digits starting with 6,7,8 or 9")]
    public required string PhoneNo { get; set; }

    [Required(ErrorMessage = "Username is required")]
    [StringLength(30, MinimumLength = 4, ErrorMessage = "Username must be between 4 and 30 characters")]
    public required string UserName { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "Password must contain at least 8 characters, one uppercase, one lowercase, one number and one special character")]
    public required string Password { get; set; }
}

// Custom validation attribute for no future dates
public class NoFutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is DateOnly date)
        {
            return date <= DateOnly.FromDateTime(DateTime.Today);
        }
        return false;
    }
}

// Custom validation attribute for minimum age
public class MinimumAgeAttribute : ValidationAttribute
{
    private readonly int _minimumAge;

    public MinimumAgeAttribute(int minimumAge)
    {
        _minimumAge = minimumAge;
    }

    public override bool IsValid(object? value)
    {
        if (value is DateOnly dateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth > today.AddYears(-age)) age--;

            return age >= _minimumAge;
        }
        return false;
    }
}

// Custom validation attribute for maximum age (born after 1930)
public class MaximumAgeAttribute : ValidationAttribute
{
    private readonly int _maximumAge;

    public MaximumAgeAttribute(int maximumAge)
    {
        _maximumAge = maximumAge;
    }

    public override bool IsValid(object? value)
    {
        if (value is DateOnly dateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth > today.AddYears(-age)) age--;

            return age <= _maximumAge;
        }
        return false;
    }
}

// Custom validation attribute for allowed email domains
public class AllowedEmailDomainsAttribute : ValidationAttribute
{
    private readonly string[] _allowedDomains;

    public AllowedEmailDomainsAttribute(string[] allowedDomains)
    {
        _allowedDomains = allowedDomains;
    }

    public override bool IsValid(object? value)
    {
        if (value is string email)
        {
            foreach (var domain in _allowedDomains)
            {
                if (email.EndsWith(domain, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }
        return false;
    }
}