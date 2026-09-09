$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$testRoot = Join-Path $root 'Temp/Phase1ShowcaseRuntime'
$modsRoot = Join-Path $testRoot 'Mods'
$modRoot = Join-Path $modsRoot 'example.phase1'
Remove-Item -Recurse -Force $testRoot -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force $modsRoot | Out-Null
Copy-Item -Recurse -Force (Join-Path $root 'Mods/example.phase1') $modRoot

$sources = @(
    'Assets/Scripts/Eclipse/Runtime/Modding/ModId.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/AssetId.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/DefinitionId.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/SemanticVersion.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/VersionRange.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/ModManifest.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/ModManifestReader.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/ModDiagnostics.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/ModDiscovery.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/DependencyResolver.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/AssetProvider.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/AssetResolver.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/LooseModProvider.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/ModScripting.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/ModScriptingP1C.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/ModScriptingP1D.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/ModWarriorTemplates.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/ModSaveData.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/CoreContentImporter.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/CoreContentImporterP1C.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/ModContent.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/ModContentP1B.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/ModContentP1C.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/ModContentP1D.cs',
    'Assets/Scripts/Eclipse/Runtime/Modding/ModLocalizationLoader.cs',
    'Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntime.cs',
    'Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntimeP1D.cs'
) | ForEach-Object { Join-Path $root $_ }

$program = Join-Path $testRoot 'Program.cs'
$exe = Join-Path $testRoot 'Phase1ShowcaseRuntime.dll'
@'
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Eclipse.Modding;

internal sealed class EmptyCoreProvider : IAssetProvider
{
    public ModId Namespace => ModId.Parse("core");
    public bool TryDescribe(AssetId id, out AssetMetadata metadata)
    {
        metadata = null;
        if (id.Namespace != Namespace || id.Path != "gamedata/models/mdl_weapon_katana_ritual") return false;
        metadata = new AssetMetadata(id, AssetKind.Model, AssetSourceKind.Core, string.Empty, -1, "core-model-fixture");
        return true;
    }
}

internal static class Program
{
    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    private static void ImportMinimumCore(ModContentCatalog catalog)
    {
        CoreContentImporter.ImportForgeEconomicProfiles(catalog, new[] { "Simple" });
        var templates = new XmlDocument();
        templates.LoadXml("<Templates><Warrior Name='Default'/></Templates>");
        CoreContentImporter.ImportWarriorTemplates(catalog, templates.DocumentElement);
    }

    private static ModDescriptor Discover(string modsRoot)
    {
        ModDiscoveryResult discovery = ModDiscovery.DiscoverLoose(modsRoot);
        Assert(!discovery.HasErrors, "Discovery failed: " + string.Join(" | ", discovery.Diagnostics));
        DependencyResolutionResult resolution = DependencyResolver.Resolve(discovery.Mods,
            ModPlatformVersions.Api, ModPlatformVersions.Core);
        Assert(!resolution.HasErrors, "Dependency resolution failed: " + string.Join(" | ", resolution.Diagnostics));
        Assert(resolution.OrderedMods.Count == 1 && resolution.OrderedMods[0].Id.Value == "example.phase1",
            "Did not resolve exactly the showcase mod.");
        return resolution.OrderedMods[0];
    }

    private static void Execute(ModDescriptor mod, AssetResolver assets, ModContentCatalog catalog,
        ModStateRuntime state, bool expectCommit)
    {
        IModScriptContext context = null;
        ModRegistrationTransaction registration = null;
        try
        {
            registration = catalog.BeginRegistration(mod);
            ModLocalizationLoader.Load(mod, assets, registration);
            var api = new ModApiFacade(mod, assets, registration, state, null);
            context = new MoonSharpScriptRuntime().CreateContext(mod, api);
            context.ExecuteEntrypoint();
            registration.Commit();
            if (!expectCommit) throw new Exception("Duplicate showcase transaction unexpectedly committed.");
        }
        catch (ModContentException)
        {
            if (expectCommit) throw;
            state.RemoveDefinition(mod.Id);
        }
        finally
        {
            registration?.Dispose();
            context?.Dispose();
        }
    }

    public static int Main(string[] args)
    {
        string modsRoot = Path.GetFullPath(args[0]);
        ModDescriptor mod = Discover(modsRoot);
        var loose = new LooseModProvider(mod);
        var assets = new AssetResolver(new IAssetProvider[] { new EmptyCoreProvider(), loose });

        var catalog = new ModContentCatalog();
        ImportMinimumCore(catalog);
        int coreProfilesBefore = catalog.ForgeEconomicProfiles.Count;
        int coreTemplatesBefore = catalog.WarriorTemplates.Count;
        var state = new ModStateRuntime();

        Execute(mod, assets, catalog, state, true);

        Assert(catalog.TryGetItem(DefinitionId.Parse("example.phase1:items/consumable/phase_token"), out ItemDefinition item),
            "P1C item was not committed.");
        Assert(catalog.TryGetItemSet(DefinitionId.Parse("example.phase1:itemsets/phase_relics"), out ItemSetDefinition set),
            "P1C item set was not committed.");
        Assert(catalog.TryGetForgeRecipeFamily(DefinitionId.Parse("example.phase1:forge-recipes/showcase_simple"), out ForgeRecipeFamilyDefinition recipe),
            "P1C forge family was not committed.");
        Assert(catalog.Zones.Count == 1 && catalog.Battles.Count == 1 && catalog.Fights.Count == 1 &&
            catalog.Warriors.Count == 1 && catalog.FightRules.Count == 1 && catalog.Rewards.Count == 1,
            "P1A graph did not commit as one coherent slice.");
        Assert(catalog.Quests.Count == 2, "P1B quests did not commit.");
        Assert(catalog.Locations.Count == 1 && catalog.MoveTemplates.Count == 1 && catalog.Moves.Count == 1 &&
            catalog.MoveTriggers.Count == 1 && catalog.Tactics.Count == 1 && catalog.LocaleMetadata.Count == 1,
            "P1D definitions did not commit.");
        Assert(catalog.Behaviors.Count == 1 && catalog.Perks.Count == 1,
            "Behavior-backed perk did not commit.");
        Assert(catalog.ForgeEconomicProfiles.Count == coreProfilesBefore && catalog.WarriorTemplates.Count == coreTemplatesBefore,
            "Showcase mutated imported core registries.");

        int zones = catalog.Zones.Count, battles = catalog.Battles.Count, fights = catalog.Fights.Count;
        int quests = catalog.Quests.Count, items = catalog.NonEquipmentItems.Count, locations = catalog.Locations.Count;
        var duplicateState = new ModStateRuntime();
        Execute(mod, assets, catalog, duplicateState, false);
        Assert(catalog.Zones.Count == zones && catalog.Battles.Count == battles && catalog.Fights.Count == fights &&
            catalog.Quests.Count == quests && catalog.NonEquipmentItems.Count == items && catalog.Locations.Count == locations,
            "Failed duplicate transaction partially mutated committed content.");

        Assert(catalog.ForgeEconomicProfiles.Count == coreProfilesBefore && catalog.WarriorTemplates.Count == coreTemplatesBefore,
            "Rollback/teardown changed base-owned core fixtures.");

        Console.WriteLine("Phase 1 showcase MoonSharp runtime PASS: discovery, localization load, public Lua execution, transactional commit, duplicate rollback, context teardown, core fixtures unchanged.");
        return 0;
    }
}
'@ | Set-Content -Encoding UTF8 $program

$moon = Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'
$project = Join-Path $testRoot 'Phase1ShowcaseRuntime.csproj'
$compileItems = @($program) + $sources
$compileXml = ($compileItems | ForEach-Object { '    <Compile Include="' + [Security.SecurityElement]::Escape($_) + '" />' }) -join "`n"
$moonXml = [Security.SecurityElement]::Escape($moon)
@"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <Nullable>disable</Nullable>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>
  <ItemGroup>
$compileXml
    <Reference Include="MoonSharp.Interpreter">
      <HintPath>$moonXml</HintPath>
      <Private>true</Private>
    </Reference>
  </ItemGroup>
</Project>
"@ | Set-Content -Encoding UTF8 $project

dotnet build $project -nologo --verbosity quiet
if ($LASTEXITCODE -ne 0) { throw "Showcase runtime fixture compile failed: $LASTEXITCODE" }
$exe = Join-Path $testRoot 'bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'
dotnet $exe $modsRoot
if ($LASTEXITCODE -ne 0) { throw "Showcase runtime fixture failed: $LASTEXITCODE" }
