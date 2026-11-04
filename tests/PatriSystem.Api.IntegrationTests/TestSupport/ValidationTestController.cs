using FluentValidation;

using Microsoft.AspNetCore.Mvc;

using PatriSystem.Api.Errors;
using PatriSystem.Application.Results;

namespace PatriSystem.Api.IntegrationTests.TestSupport;

[ApiController]
[Route("test-support/validation")]
public sealed class ValidationTestController : ControllerBase
{
    private readonly SampleRequestValidator _validator = new();

    [HttpPost]
    public IActionResult Post(SampleRequest request)
    {
        var result = _validator.Validate(request);

        return result.IsValid
            ? Ok()
            : result.ToValidationError().ToActionResult();
    }
}

public sealed record SampleRequest(string Email, string Password);

public sealed class SampleRequestValidator : AbstractValidator<SampleRequest>
{
    public SampleRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("El correo es obligatorio.");
        RuleFor(x => x.Password).MinimumLength(10).WithMessage("Muy corta.");
        RuleFor(x => x.Password).Matches("[0-9]").WithMessage("Falta un número.");
    }
}