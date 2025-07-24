using System.ComponentModel.DataAnnotations;

namespace DfE.FindInformationAcademiesTrusts.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class DfeEmailAddressAttribute()
    : ValidationAttribute("Enter a DfE email address in the correct format, e.g. joe.bloggs@education.gov.uk")
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string email)
        {
            return null;
        }

        return IsValidEmailAddress(email) switch
        {
            true => ValidationResult.Success,
            _ => new ValidationResult(ErrorMessage)
        };
    }

    private static bool IsValidEmailAddress(string email)
    {
        var emailParts = email.Split("@");

        var isCorrectFormat = emailParts.Length == 2;
        if (!isCorrectFormat) return false;

        var name = emailParts[0];
        var domain = emailParts[1];

        var isCorrectDomain = string.Equals(domain, "education.gov.uk", StringComparison.InvariantCultureIgnoreCase);

        return isCorrectDomain && IsCorrectName(name);
    }

    private static bool IsCorrectName(string name)
    {
        var nameParts = name.Split(".");
        return !nameParts.Any(string.IsNullOrWhiteSpace) && nameParts.All(part => part.All(char.IsLetterOrDigit));
    }
}
