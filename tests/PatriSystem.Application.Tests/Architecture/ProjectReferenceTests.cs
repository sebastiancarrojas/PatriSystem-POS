using Shouldly;

namespace PatriSystem.Application.Tests.Architecture;

public sealed class ProjectReferenceTests
{
    private const string DomainProject = "src/PatriSystem.Domain/PatriSystem.Domain.csproj";
    private const string ApplicationProject = "src/PatriSystem.Application/PatriSystem.Application.csproj";

    private static readonly string[] ExpectedApplicationProjectReferences = ["PatriSystem.Domain"];

    private static readonly string[] AllowedApplicationPackages =
    [
        "Microsoft.Extensions.DependencyInjection.Abstractions"
    ];

    private static readonly string[] ForbiddenPackagePrefixes =
    [
        "Microsoft.EntityFrameworkCore",
        "Microsoft.AspNetCore",
        "Microsoft.Extensions.Identity",
        "Microsoft.IdentityModel",
        "System.IdentityModel.Tokens.Jwt"
    ];

    [Fact]
    public void DomainProject_NoReferencesDeclared_HasNoDependencies()
    {
        var project = ProjectFile.Load(DomainProject);

        project.ProjectReferences.ShouldBeEmpty();
        project.PackageReferences.ShouldBeEmpty();
        project.FrameworkReferences.ShouldBeEmpty();
    }

    [Fact]
    public void ApplicationProject_ProjectReferencesDeclared_ReferencesOnlyDomain()
    {
        var project = ProjectFile.Load(ApplicationProject);

        project.ProjectReferences.ShouldBe(ExpectedApplicationProjectReferences);
        project.FrameworkReferences.ShouldBeEmpty();
    }

    [Fact]
    public void ApplicationProject_PackageReferencesDeclared_AreInAllowList()
    {
        var project = ProjectFile.Load(ApplicationProject);

        project.PackageReferences.ShouldBeSubsetOf(AllowedApplicationPackages);
    }

    [Theory]
    [InlineData(DomainProject)]
    [InlineData(ApplicationProject)]
    public void InnerLayerProject_PackagesDeclared_HasNoForbiddenFrameworks(string projectPath)
    {
        var project = ProjectFile.Load(projectPath);

        var offending = project.PackageReferences
            .Where(package => ForbiddenPackagePrefixes.Any(
                prefix => package.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        offending.ShouldBeEmpty();
    }

    [Fact]
    public void SolutionBuildProps_NoPackagesDeclared_DoesNotAddDependenciesToAllProjects()
    {
        var props = ProjectFile.Load("Directory.Build.props");

        props.PackageReferences.ShouldBeEmpty();
        props.ProjectReferences.ShouldBeEmpty();
    }
}