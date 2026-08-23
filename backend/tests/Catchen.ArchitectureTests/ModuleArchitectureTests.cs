using System.Reflection;
using ArchUnitNET.Domain;
using ArchUnitNET.Fluent.Syntax.Elements.Types;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;
using Assembly = System.Reflection.Assembly;

namespace Catchen.ArchitectureTests;

/// <summary>
/// Machine-checked modular-monolith rules from Agents.md §2.
///
/// FIXTURE MAINTENANCE: when a new module is added, add its assembly to
/// <see cref="_moduleAssemblyNames"/>, its allowed Catchen dependencies to
/// <see cref="_allowedModuleDependencies"/>, and its ProjectReference to the
/// Api csproj expectation. A failing architecture test is the signal that the
/// fixture (or the module graph) is out of date.
/// </summary>
public sealed class ModuleArchitectureTests
{
    private static readonly string[] _moduleAssemblyNames =
    {
        "Catchen.Identity",
        "Catchen.Catalog",
        "Catchen.Editorial",
        "Catchen.Commerce",
        "Catchen.Documents",
        "Catchen.Affiliates",
        "Catchen.Moderation",
        "Catchen.Reporting",
        "Catchen.Data",
        "Catchen.Api",
    };

    /// <summary>Declared domain-module dependencies (Agents.md §2). Domain
    /// modules are independent at scaffold time; allowed edges grow here as
    /// capabilities land, never silently in csproj files.</summary>
    private static readonly IReadOnlyDictionary<string, string[]> _allowedModuleDependencies =
        new Dictionary<string, string[]>
        {
            ["Catchen.Identity"] = Array.Empty<string>(),
            ["Catchen.Catalog"] = Array.Empty<string>(),
            ["Catchen.Editorial"] = ["Catchen.Catalog", "Catchen.Identity"],
            ["Catchen.Commerce"] = ["Catchen.Catalog", "Catchen.Identity"],
            ["Catchen.Documents"] = ["Catchen.Catalog", "Catchen.Commerce"],
            ["Catchen.Affiliates"] = Array.Empty<string>(),
            ["Catchen.Moderation"] = Array.Empty<string>(),
            ["Catchen.Reporting"] = Array.Empty<string>(),
        };

    private static readonly Architecture _architecture = new ArchLoader()
        .LoadAssemblies(_moduleAssemblyNames.Select(Assembly.Load).ToArray())
        .Build();

    private static GivenTypesConjunction DataTypes()
    {
        return Types()
        .That().ResideInNamespaceMatching(@"^Catchen\.Data($|\.)");
    }

    private static GivenTypesConjunction ApplicationDbContextType()
    {
        return Types()
        .That().HaveFullName("Catchen.Data.AppDbContext");
    }

    [Fact]
    public void Modules_do_not_depend_on_Catchen_Data()
    {
        Types().That().ResideInNamespaceMatching(@"^Catchen($|\.)")
            .And().DoNotResideInNamespaceMatching(@"^Catchen\.Data($|\.)")
            .And().DoNotResideInNamespaceMatching(@"^Catchen\.Api($|\.)")
            .Should().NotDependOnAny(DataTypes())
            .Because("modules must never reference Catchen.Data; services depend on the base DbContext")
            .WithoutRequiringPositiveResults()
            .Check(_architecture);
    }

    [Fact]
    public void Modules_do_not_depend_on_the_concrete_AppDbContext()
    {
        Types().That().ResideInNamespaceMatching(@"^Catchen($|\.)")
            .And().DoNotResideInNamespaceMatching(@"^Catchen\.Data($|\.)")
            .And().DoNotResideInNamespaceMatching(@"^Catchen\.Api($|\.)")
            .Should().NotDependOnAny(ApplicationDbContextType())
            .Because("services inject the base Microsoft.EntityFrameworkCore.DbContext to avoid circular references")
            .WithoutRequiringPositiveResults()
            .Check(_architecture);
    }

    [Fact]
    public void Module_dependency_graph_matches_Agents_md()
    {
        foreach (var (module, allowedDependencies) in _allowedModuleDependencies)
        {
            var forbidden = _moduleAssemblyNames
                .Where(ns => ns != module && !allowedDependencies.Contains(ns))
                .ToArray();

            Types().That().ResideInNamespace(module)
                .Should().NotDependOnAny(Types().That().ResideInNamespaceMatching(
                    $"^({string.Join("|", forbidden.Select(ns => $"{ns}(\\.|$)"))})"))
                .Because($"'{module}' may only depend on: {(allowedDependencies.Length == 0 ? "no other Catchen module" : string.Join(", ", allowedDependencies))}")
                .WithoutRequiringPositiveResults()
                .Check(_architecture);
        }
    }

    [Fact]
    public void Modules_do_not_reference_Api()
    {
        Types().That().ResideInNamespaceMatching(@"^Catchen($|\.)")
            .And().DoNotResideInNamespaceMatching(@"^Catchen\.Api($|\.)")
            .Should().NotDependOnAny(Types().That().ResideInNamespaceMatching(@"^Catchen\.Api($|\.)"))
            .Because("the Api project is the composition root; no module may depend on it")
            .WithoutRequiringPositiveResults()
            .Check(_architecture);
    }

    [Fact]
    public void Api_is_the_composition_root_and_references_every_module()
    {
        // The C# compiler drops assembly references to modules whose types are
        // not used, so the normative check is the csproj ProjectReference graph,
        // which is stable regardless of how much a module currently registers.
        var apiCsproj = Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..",
            "src", "Catchen.Api", "Catchen.Api.csproj");
        var projectText = File.ReadAllText(Path.GetFullPath(apiCsproj));

        foreach (var module in _moduleAssemblyNames.Where(ns => ns != "Catchen.Api"))
        {
            Assert.True(
                projectText.Contains($"{module}.csproj", StringComparison.OrdinalIgnoreCase),
                $"Catchen.Api (composition root) must reference '{module}'.");
        }
    }

    [Fact]
    public void Every_module_on_disk_is_declared_in_the_fixture()
    {
        // Adding a module without updating this fixture must fail loudly.
        var srcDir = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..", "src"));

        var onDisk = Directory.GetDirectories(srcDir, "Catchen.*")
            .Select(Path.GetFileName)
            .ToArray();

        foreach (var dir in onDisk)
        {
            Assert.True(
                _moduleAssemblyNames.Contains(dir),
                $"'{dir}' exists under backend/src but is not declared in the architecture fixture.");
        }
    }
}
