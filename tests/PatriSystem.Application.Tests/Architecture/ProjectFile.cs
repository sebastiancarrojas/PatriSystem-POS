using System.Xml.Linq;

namespace PatriSystem.Application.Tests.Architecture;

internal sealed class ProjectFile
{
    private readonly XDocument _document;

    private ProjectFile(XDocument document) => _document = document;

    public IReadOnlyList<string> ProjectReferences => _document
        .Descendants("ProjectReference")
        .Select(element => (string?)element.Attribute("Include") ?? string.Empty)
        .Select(include => Path.GetFileNameWithoutExtension(include.Replace('\\', '/')))
        .ToList();

    public IReadOnlyList<string> PackageReferences => _document
        .Descendants("PackageReference")
        .Select(element => (string?)element.Attribute("Include") ?? string.Empty)
        .ToList();

    public IReadOnlyList<string> FrameworkReferences => _document
        .Descendants("FrameworkReference")
        .Select(element => (string?)element.Attribute("Include") ?? string.Empty)
        .ToList();

    public static ProjectFile Load(string relativePath)
    {
        var path = Path.Combine(FindRepositoryRoot(), relativePath);
        return new ProjectFile(XDocument.Load(path));
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "PatriSystem.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new InvalidOperationException("No se encontró la raíz del repositorio (PatriSystem.slnx).");
    }
}