using System.ComponentModel.DataAnnotations;

//Check if the value is greater than 0
namespace Api.Utils.Annotations
{
    public class GreaterThanZeroAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {   
            if (value is int intValue && intValue > 0 && intValue < int.MaxValue)
            {
                return ValidationResult.Success;
            }
            else if (value is double doubleValue && doubleValue > 0 && doubleValue < double.MaxValue)
            {
                return ValidationResult.Success;
            }
            else if (value is float floatValue && floatValue > 0 && floatValue < float.MaxValue)
            {
                return ValidationResult.Success;
            }

            var errorMessage = ErrorMessage ?? "Value must be greater than 0";
            var memberName = validationContext.MemberName;
            
            return new ValidationResult(errorMessage, new List<string> { memberName });
        }
    }
}