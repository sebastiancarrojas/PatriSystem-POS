using Microsoft.AspNetCore.Mvc;

using PatriSystem.Application.Results;

namespace PatriSystem.Api.Errors;

public static class BusinessErrorMapper
{
    public static ProblemDetails ToProblemDetails(BusinessError error)
    {
        ArgumentNullException.ThrowIfNull(error);

        ProblemDetails problem = error switch
        {
            ValidationError validation => new ValidationProblemDetails(
                validation.Failures.ToDictionary(failure => failure.Key, failure => failure.Value))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Uno o más campos no son válidos."
            },
            UnauthorizedError => new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "No autorizado."
            },
            NotFoundError => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Recurso no encontrado."
            },
            ConflictError => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflicto con el estado actual del recurso."
            },
            _ => throw new NotSupportedException(
                $"Tipo de error no soportado: {error.GetType().Name}.")
        };

        problem.Detail = error.Description;
        problem.Extensions["code"] = error.Code;

        return problem;
    }
}