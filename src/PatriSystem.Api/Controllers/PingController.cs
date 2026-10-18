using Microsoft.AspNetCore.Mvc;

namespace PatriSystem.Api.Controllers;

/// <summary>
/// Endpoint de ejemplo para comprobar que la API responde.
/// </summary>
[ApiController]
[Route("api/ping")]
public sealed class PingController : ControllerBase
{
    /// <summary>
    /// Devuelve el estado de la API.
    /// </summary>
    /// <returns>Un objeto con el estado <c>ok</c>.</returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType<PingResponse>(StatusCodes.Status200OK)]
    public IActionResult Get() => Ok(new PingResponse("ok"));
}

/// <summary>
/// Respuesta del endpoint de ping.
/// </summary>
/// <param name="Status">Estado de la API.</param>
public sealed record PingResponse(string Status);