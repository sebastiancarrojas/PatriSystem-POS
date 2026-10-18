using System.Net;

using Shouldly;

namespace PatriSystem.Api.IntegrationTests;

public sealed class HealthLiveTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task GetHealthLive_ApiRunning_ReturnsOkHealthy()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/health/live");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).ShouldBe("Healthy");
    }
}