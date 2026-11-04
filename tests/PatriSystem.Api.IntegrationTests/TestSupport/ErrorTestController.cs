using Microsoft.AspNetCore.Mvc;

namespace PatriSystem.Api.IntegrationTests.TestSupport;

[ApiController]
[Route("test-support/errors")]
public sealed class ErrorTestController : ControllerBase
{
    public const string SecretMessage = "secreto-interno-no-debe-filtrarse";

    [HttpGet("unexpected")]
    public IActionResult Unexpected()
    {
        throw new InvalidOperationException(SecretMessage + Request.Path.Value);
    }
}