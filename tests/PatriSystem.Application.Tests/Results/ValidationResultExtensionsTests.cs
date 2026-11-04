using FluentValidation;

using PatriSystem.Application.Results;

using Shouldly;

namespace PatriSystem.Application.Tests.Results;

public sealed class ValidationResultExtensionsTests
{
    [Fact]
    public void ToValidationError_FailuresOnDifferentFields_GroupsMessagesByField()
    {
        var result = new SampleValidator().Validate(new Sample(string.Empty, "abc"));

        var error = result.ToValidationError();

        error.Failures.Count.ShouldBe(2);
        error.Failures.Keys.ShouldContain("Email");
        error.Failures.Keys.ShouldContain("Password");
        error.Failures["Email"].Length.ShouldBe(1);
        error.Failures["Password"].Length.ShouldBe(2);
    }

    [Fact]
    public void ToValidationError_AnyFailure_UsesValidationFailedCode()
    {
        var result = new SampleValidator().Validate(new Sample(string.Empty, "abc"));

        var error = result.ToValidationError();

        error.Code.ShouldBe("validation.failed");
    }

    private sealed record Sample(string Email, string Password);

    private sealed class SampleValidator : AbstractValidator<Sample>
    {
        public SampleValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("El correo es obligatorio.");
            RuleFor(x => x.Password).MinimumLength(10).WithMessage("Muy corta.");
            RuleFor(x => x.Password).Matches("[0-9]").WithMessage("Falta un número.");
        }
    }
}