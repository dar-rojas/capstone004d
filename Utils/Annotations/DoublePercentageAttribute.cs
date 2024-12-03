using System.ComponentModel.DataAnnotations;

// Check if the value is a double between 0 and 1
public class DoublePercentageAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is double doubleValue && doubleValue >= 0.0 && doubleValue <= 1.0)
        {
            return ValidationResult.Success;
        }

        var errorMessage = ErrorMessage ?? "Value must be between 0.0 and 1.0";
        var memberName = validationContext.MemberName;

        return new ValidationResult(errorMessage, new List<string> { memberName });
    }
}