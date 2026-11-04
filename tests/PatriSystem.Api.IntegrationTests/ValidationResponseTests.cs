using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc;

using Shouldly;

namespace PatriSystem.Api.IntegrationTests;

public sealed class ValidationResponseTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string Route = "/test-support/validation";

    [Fact]
    public async Task PostValidation_InvalidFields_ReturnsBadRequestWithFieldErrors()
    {
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonAsync(
            Route,
            new { Email = string.Empty, Password = "abc" });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        response.Content.Headers.ContentType.ShouldNotBeNull().MediaType
            .ShouldBe("application/problem+json");

        var body = await response.Content.ReadAsStringAsync();
        body.ShouldContain("\"Email\"", Case.Sensitive);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem.ShouldNotBeNull();

        var errors = new Dictionary<string, string[]>(
            problem.Errors,
            StringComparer.OrdinalIgnoreCase);

        errors.Count.ShouldBe(2);
        errors["Email"].Length.ShouldBe(1);
        errors["Password"].Length.ShouldBe(2);
    }

    [Fact]
    public async Task PostValidation_ValidFields_ReturnsOk()
    {
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonAsync(
            Route,
            new { Email = "caja@patrisystem.test", Password = "Segura12345" });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}