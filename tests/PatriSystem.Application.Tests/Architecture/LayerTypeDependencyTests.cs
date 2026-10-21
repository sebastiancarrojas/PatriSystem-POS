using System.Reflection;

using NetArchTest.Rules;

using Shouldly;

namespace PatriSystem.Application.Tests.Architecture;

public sealed class LayerTypeDependencyTests
{
    private static readonly string[] ForbiddenFrameworks =
    [
        "Microsoft.EntityFrameworkCore",
        "Microsoft.AspNetCore",
        "Microsoft.Extensions.Identity",
        "Microsoft.IdentityModel",
        "System.IdentityModel"
    ];

    [Fact]
    public void DomainTypes_AllTypes_DoNotDependOnFrameworksOrOtherLayers()
    {
        var domain = Assembly.Load("PatriSystem.Domain");

        var result = Types.InAssembly(domain)
            .ShouldNot()
            .HaveDependencyOnAny([
                .. ForbiddenFrameworks,
                "PatriSystem.Application",
                "PatriSystem.Infrastructure",
                "PatriSystem.Api"
            ])
            .GetResult();

        AssertSuccessful(result);
    }

    [Fact]
    public void ApplicationTypes_AllTypes_DoNotDependOnFrameworksOrOuterLayers()
    {
        var application = Assembly.Load("PatriSystem.Application");

        var result = Types.InAssembly(application)
            .ShouldNot()
            .HaveDependencyOnAny([
                .. ForbiddenFrameworks,
                "PatriSystem.Infrastructure",
                "PatriSystem.Api"
            ])
            .GetResult();

        AssertSuccessful(result);
    }

    private static void AssertSuccessful(TestResult result)
    {
        var failing = string.Join(", ", result.FailingTypeNames ?? []);

        result.IsSuccessful.ShouldBeTrue($"Tipos que violan la regla: {failing}");
    }
}