using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using PatriSystem.Api.Errors;
using PatriSystem.Application.Results;

using Shouldly;

namespace PatriSystem.Api.UnitTests.Errors;

public sealed class BusinessErrorMapperTests
{
    [Fact]
    public void ToProblemDetails_ValidationError_ReturnsBadRequestWithFailedFields()
    {
        var failures = new Dictionary<string, string[]>
        {
            ["email"] = ["El correo es obligatorio."],
            ["password"] = ["Muy corta.", "Falta un número."]
        };
        var error = new ValidationError("validation.failed", "Datos inválidos.", failures);

        var problem = BusinessErrorMapper.ToProblemDetails(error);

        var validation = problem.ShouldBeOfType<ValidationProblemDetails>();
        validation.Status.ShouldBe(StatusCodes.Status400BadRequest);
        validation.Detail.ShouldBe("Datos inválidos.");
        validation.Extensions["code"].ShouldBe("validation.failed");
        validation.Errors.Count.ShouldBe(2);
        validation.Errors["email"].ShouldBe(failures["email"]);
        validation.Errors["password"].ShouldBe(failures["password"]);
    }

    [Fact]
    public void ToProblemDetails_UnauthorizedError_ReturnsUnauthorized()
    {
        var problem = BusinessErrorMapper.ToProblemDetails(
            new UnauthorizedError("auth.invalid", "Credenciales inválidas."));

        problem.Status.ShouldBe(StatusCodes.Status401Unauthorized);
        problem.Detail.ShouldBe("Credenciales inválidas.");
        problem.Extensions["code"].ShouldBe("auth.invalid");
    }

    [Fact]
    public void ToProblemDetails_NotFoundError_ReturnsNotFound()
    {
        var problem = BusinessErrorMapper.ToProblemDetails(
            new NotFoundError("user.not_found", "Usuario no encontrado."));

        problem.Status.ShouldBe(StatusCodes.Status404NotFound);
        problem.Detail.ShouldBe("Usuario no encontrado.");
        problem.Extensions["code"].ShouldBe("user.not_found");
    }

    [Fact]
    public void ToProblemDetails_ConflictError_ReturnsConflict()
    {
        var problem = BusinessErrorMapper.ToProblemDetails(
            new ConflictError("user.duplicate", "El usuario ya existe."));

        problem.Status.ShouldBe(StatusCodes.Status409Conflict);
        problem.Detail.ShouldBe("El usuario ya existe.");
        problem.Extensions["code"].ShouldBe("user.duplicate");
    }

    [Fact]
    public void ToProblemDetails_UnsupportedErrorType_ThrowsNotSupportedException()
    {
        Should.Throw<NotSupportedException>(
            () => { _ = BusinessErrorMapper.ToProblemDetails(new UnknownError()); });
    }

    private sealed record UnknownError() : BusinessError("unknown", "Desconocido.");
}