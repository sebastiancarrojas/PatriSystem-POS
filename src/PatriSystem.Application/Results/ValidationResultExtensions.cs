using FluentValidation.Results;

namespace PatriSystem.Application.Results;

public static class ValidationResultExtensions
{
    public static ValidationError ToValidationError(this ValidationResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var failures = result.Errors
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(failure => failure.ErrorMessage).ToArray());

        return new ValidationError(
            "validation.failed",
            "Uno o más campos no son válidos.",
            failures);
    }
}