using Microsoft.Extensions.DependencyInjection;

using Shouldly;

namespace PatriSystem.Application.Tests;

public sealed class DependencyInjectionTests
{
    [Fact]
    public void AddApplication_EmptyServiceCollection_ReturnsSameCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddApplication();

        result.ShouldBeSameAs(services);
    }
}