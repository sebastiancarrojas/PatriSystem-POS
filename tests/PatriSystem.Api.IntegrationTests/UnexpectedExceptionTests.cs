using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using PatriSystem.Api.IntegrationTests.TestSupport;

using Shouldly;

namespace PatriSystem.Api.IntegrationTests;

public sealed class UnexpectedExceptionTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string Route = "/test-support/errors/unexpected";

    [Fact]
    public async Task GetUnexpected_UnhandledException_ReturnsGenericInternalServerError()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(Route);

        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.ShouldNotBeNull();
        problem.Status.ShouldBe(StatusCodes.Status500InternalServerError);
        problem.Title.ShouldBe("Ocurrió un error inesperado.");
        problem.Detail.ShouldBeNull();
    }

    [Fact]
    public async Task GetUnexpected_EnvironmentIsNotDevelopment_DoesNotLeakExceptionDetails()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(Route);

        var body = await response.Content.ReadAsStringAsync();
        body.ShouldNotContain(ErrorTestController.SecretMessage);
        body.ShouldNotContain(nameof(InvalidOperationException));
    }
}