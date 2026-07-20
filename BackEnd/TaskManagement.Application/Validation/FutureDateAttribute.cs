using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.Validation;

// This is the primary enforcement of "due date cannot be in the past" -
// [ApiController] runs this automatically on every request and returns 400
// before the action method body even runs. TaskItem.EnsureDueDateNotInPast()
// is the backstop for anything that constructs a task outside this path.
/// <summary>Validation attribute requiring a DateTime to be today or later.</summary>
public class FutureDateAttribute : ValidationAttribute
{
    /// <summary>Fails validation if the value is a past date.</summary>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not DateTime date)
        {
            return ValidationResult.Success;
        }

        return date.Date < DateTime.UtcNow.Date
            ? new ValidationResult(ErrorMessage ?? "Date cannot be in the past.")
            : ValidationResult.Success;
    }
}
