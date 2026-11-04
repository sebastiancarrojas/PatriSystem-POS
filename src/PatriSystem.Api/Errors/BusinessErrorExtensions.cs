using Microsoft.AspNetCore.Mvc;

using PatriSystem.Application.Results;

namespace PatriSystem.Api.Errors;

public static class BusinessErrorExtensions
{
    public static ObjectResult ToActionResult(this BusinessError error)
    {
        var problem = BusinessErrorMapper.ToProblemDetails(error);

        return new ObjectResult(problem) { StatusCode = problem.Status };
    }
}