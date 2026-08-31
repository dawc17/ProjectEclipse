$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$sources = @(
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModId.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\AssetId.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\DefinitionId.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\SemanticVersion.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\VersionRange.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModManifest.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModManifestReader.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModDiagnostics.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModDiscovery.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\DependencyResolver.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\AssetProvider.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\AssetResolver.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\LooseModProvider.cs')
)

$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$csc = $null
if (Test-Path $vswhere) {
    $csc = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild `
        -find 'MSBuild\**\Bin\Roslyn\csc.exe' | Select-Object -First 1
}
if (-not $csc -or -not (Test-Path $csc)) {
    $csc = Get-ChildItem (Join-Path $env:ProgramFiles 'Microsoft Visual Studio') -Recurse `
        -Filter csc.exe -ErrorAction SilentlyContinue | Select-Object -First 1 -ExpandProperty FullName
}
if (-not $csc -or -not (Test-Path $csc)) {
    throw 'Could not locate the Visual Studio Roslyn C# compiler.'
}

$testRoot = Join-Path $root 'Temp\ModdingContractsTest'
New-Item -ItemType Directory -Force -Path $testRoot | Out-Null
$harness = Join-Path $testRoot 'Program.cs'
$exe = Join-Path $testRoot 'ModdingContractsTest.exe'

@'
using System;
using System.Collections.Generic;
using System.IO;
using Eclipse.Modding;

internal static class Program
{
    private sealed class FakeCoreProvider : IAssetProvider
    {
        public ModId Namespace => ModId.Parse("core");

        public bool TryDescribe(AssetId id, out AssetMetadata metadata)
        {
            metadata = null;
            if (id.Namespace != Namespace || id.Path != "ui/test") return false;
            metadata = new AssetMetadata(id, AssetKind.Unknown, AssetSourceKind.Core,
                string.Empty, -1, "fake-core");
            return true;
        }
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    private static void Reject(Action action, string message)
    {
        try
        {
            action();
        }
        catch (FormatException)
        {
            return;
        }
        throw new Exception(message);
    }

    private static string Manifest(string id, string version, string api, params string[] dependencies)
    {
        string text =
            "schema = 1\n" +
            "id = \"" + id + "\"\n" +
            "name = \"" + id + "\"\n" +
            "version = \"" + version + "\"\n" +
            "api = \"" + api + "\"\n" +
            "authors = [\"Tester\"]\n" +
            "entrypoint = \"scripts/main.lua\"\n" +
            "capabilities = [\"content.register\"]\n";
        for (int i = 0; i < dependencies.Length; i += 2)
        {
            text += "\n[[dependencies]]\n" +
                "id = \"" + dependencies[i] + "\"\n" +
                "version = \"" + dependencies[i + 1] + "\"\n";
        }
        return text;
    }

    private static ModDescriptor Descriptor(string id, string version, params string[] dependencies)
    {
        ModManifest manifest = ModManifestReader.ParseExternal(
            Manifest(id, version, ">=0.1 <1.0", dependencies), id + "/mod.toml");
        return new ModDescriptor(manifest, Path.GetFullPath(id), ModSourceKind.Loose);
    }

    private static bool HasCode(IReadOnlyList<ModDiagnostic> diagnostics, string code)
    {
        for (int i = 0; i < diagnostics.Count; i++)
            if (diagnostics[i].Code == code) return true;
        return false;
    }

    public static int Main()
    {
        ModId mod = ModId.Parse("example.weapon");
        Assert(mod.Value == "example.weapon", "ModId changed a valid ID.");
        Reject(() => ModId.Parse("Example.Weapon"), "Uppercase mod ID was accepted.");
        Reject(() => ModId.Parse("example/weapon"), "Slash in mod ID was accepted.");
        Reject(() => ModId.Parse(" example.weapon"), "Whitespace in mod ID was accepted.");

        AssetId asset = AssetId.Parse(@"core:Sprites\Shop\Katana");
        Assert(asset.ToString() == "core:sprites/shop/katana", "AssetId canonicalization is wrong.");
        Assert(asset == AssetId.Parse("core:sprites/shop/katana"), "Canonical AssetIds are not equal.");
        Reject(() => AssetId.Parse("sprites/shop/katana"), "Unqualified AssetId was accepted.");
        Reject(() => AssetId.Parse("core:../katana"), "Traversal AssetId was accepted.");
        Reject(() => AssetId.Parse("core:sprites//katana"), "Empty AssetId segment was accepted.");
        Reject(() => AssetId.Parse(@"core:C:\katana"), "Drive-qualified AssetId was accepted.");

        DefinitionId definition = DefinitionId.Parse("example.weapon:Items/Weapon/Example_Blade");
        Assert(definition.Namespace.Value == "example.weapon", "Definition namespace is wrong.");
        Assert(definition.Category == "items", "Definition category is wrong.");
        Assert(definition.LocalId == "weapon/example_blade", "Definition local ID is wrong.");
        Assert(definition.ToString() == "example.weapon:items/weapon/example_blade", "DefinitionId canonicalization is wrong.");
        Reject(() => DefinitionId.Parse("example.weapon:weapon"), "Definition without category/id split was accepted.");
        Reject(() => DefinitionId.Parse("example.weapon:items/../weapon"), "Traversal DefinitionId was accepted.");

        SemanticVersion stable = SemanticVersion.Parse("1.2.3");
        SemanticVersion prerelease = SemanticVersion.Parse("1.2.3-beta.2");
        Assert(stable > prerelease, "Stable SemVer must sort after prerelease.");
        Assert(SemanticVersion.Parse("1.2.3+build.7") == stable, "Build metadata affected SemVer precedence.");
        Reject(() => SemanticVersion.Parse("1.2"), "Incomplete manifest SemVer was accepted.");
        Reject(() => SemanticVersion.Parse("01.2.3"), "Leading-zero SemVer was accepted.");

        VersionRange range = VersionRange.Parse(">=0.1 <1.0");
        Assert(range.Contains(SemanticVersion.Parse("0.1.0")), "Range rejected its lower boundary.");
        Assert(range.Contains(SemanticVersion.Parse("0.9.9")), "Range rejected an interior version.");
        Assert(!range.Contains(SemanticVersion.Parse("1.0.0")), "Range accepted its exclusive upper boundary.");

        ModManifest manifest = ModManifestReader.ParseExternal(
            Manifest("example.weapon", "1.0.0", ">=0.1 <1.0", "core", ">=1.0 <2.0"));
        Assert(manifest.Id.Value == "example.weapon", "Manifest ID was parsed incorrectly.");
        Assert(manifest.Version == SemanticVersion.Parse("1.0.0"), "Manifest version was parsed incorrectly.");
        Assert(manifest.Dependencies.Count == 1 && manifest.Dependencies[0].Id.Value == "core",
            "Manifest dependency was parsed incorrectly.");
        Assert(manifest.Entrypoint == "scripts/main.lua", "Entrypoint normalization is wrong.");
        Reject(() => ModManifestReader.ParseExternal(Manifest("core", "1.0.0", ">=0.1 <1.0")),
            "Reserved core manifest ID was accepted externally.");
        Reject(() => ModManifestReader.ParseExternal(
            Manifest("bad.fields", "1.0.0", ">=0.1 <1.0", "core", ">=1.0 <2.0") +
            "capabilities = [\"events.combat\"]\n"),
            "Root field after dependency table was accepted as valid TOML schema.");

        ModDescriptor baseMod = Descriptor("a.base", "1.2.0", "core", ">=1.0 <2.0");
        ModDescriptor addon = Descriptor("b.addon", "1.0.0", "a.base", ">=1.0 <2.0");
        DependencyResolutionResult ordered = DependencyResolver.Resolve(
            new[] { addon, baseMod }, ModPlatformVersions.Api, ModPlatformVersions.Core);
        Assert(!ordered.HasErrors, "Valid dependency graph produced errors.");
        Assert(ordered.OrderedMods.Count == 2 && ordered.OrderedMods[0].Id.Value == "a.base" &&
            ordered.OrderedMods[1].Id.Value == "b.addon", "Dependency order is not deterministic/topological.");

        DependencyResolutionResult missing = DependencyResolver.Resolve(
            new[] { Descriptor("missing.user", "1.0.0", "missing.target", ">=1.0 <2.0"),
                    Descriptor("independent.mod", "1.0.0") },
            ModPlatformVersions.Api, ModPlatformVersions.Core);
        Assert(missing.HasErrors && HasCode(missing.Diagnostics, "DEP005"), "Missing dependency was not diagnosed.");
        Assert(missing.OrderedMods.Count == 1 && missing.OrderedMods[0].Id.Value == "independent.mod",
            "Independent mod was disabled by another mod's missing dependency.");

        DependencyResolutionResult disabledDependency = DependencyResolver.Resolve(
            new[] { Descriptor("broken.base", "1.0.0", "missing.target", ">=1.0 <2.0"),
                    Descriptor("broken.user", "1.0.0", "broken.base", ">=1.0 <2.0") },
            ModPlatformVersions.Api, ModPlatformVersions.Core);
        Assert(disabledDependency.HasErrors && HasCode(disabledDependency.Diagnostics, "DEP008"),
            "Dependent of a disabled mod was not diagnosed.");
        Assert(disabledDependency.OrderedMods.Count == 0, "Dependent of a disabled mod remained enabled.");

        DependencyResolutionResult mismatch = DependencyResolver.Resolve(
            new[] { Descriptor("old.base", "1.0.0"), Descriptor("new.user", "1.0.0", "old.base", ">=2.0 <3.0") },
            ModPlatformVersions.Api, ModPlatformVersions.Core);
        Assert(mismatch.HasErrors && HasCode(mismatch.Diagnostics, "DEP006"), "Dependency version mismatch was not diagnosed.");

        DependencyResolutionResult cycle = DependencyResolver.Resolve(
            new[] { Descriptor("cycle.a", "1.0.0", "cycle.b", ">=1.0 <2.0"),
                    Descriptor("cycle.b", "1.0.0", "cycle.a", ">=1.0 <2.0") },
            ModPlatformVersions.Api, ModPlatformVersions.Core);
        Assert(cycle.HasErrors && HasCode(cycle.Diagnostics, "DEP007"), "Dependency cycle was not diagnosed.");

        string modsRoot = Path.Combine(Path.GetTempPath(), "sf2de-modding-contracts-" + Guid.NewGuid().ToString("N"));
        try
        {
            string validRoot = Path.Combine(modsRoot, "example.weapon");
            Directory.CreateDirectory(validRoot);
            File.WriteAllText(Path.Combine(validRoot, "mod.toml"),
                Manifest("example.weapon", "1.0.0", ">=0.1 <1.0", "core", ">=1.0 <2.0"));
            string spriteRoot = Path.Combine(validRoot, "assets", "sprites");
            string modelRoot = Path.Combine(validRoot, "assets", "models");
            Directory.CreateDirectory(spriteRoot);
            Directory.CreateDirectory(modelRoot);
            File.WriteAllBytes(Path.Combine(spriteRoot, "weapon.png"), new byte[] { 1, 2, 3, 4 });
            File.WriteAllText(Path.Combine(spriteRoot, "weapon.sprite.toml"), "pivot = [0.5, 0.5]");
            File.WriteAllText(Path.Combine(modelRoot, "mdl_weapon_example.xml"), "<Scene><Figures /></Scene>");
            Directory.CreateDirectory(Path.Combine(modsRoot, "notes"));
            ModDiscoveryResult discovery = ModDiscovery.DiscoverLoose(modsRoot);
            Assert(!discovery.HasErrors, "Valid loose discovery produced an error.");
            Assert(discovery.Mods.Count == 1 && discovery.Mods[0].Id.Value == "example.weapon",
                "Loose mod discovery did not return the expected mod.");
            Assert(HasCode(discovery.Diagnostics, "MOD001"), "Directory-without-manifest warning was not produced.");

            LooseModProvider loose = new LooseModProvider(discovery.Mods[0]);
            AssetResolver resolver = new AssetResolver(new IAssetProvider[] { new FakeCoreProvider(), loose });
            AssetId weapon = resolver.Qualify(discovery.Mods[0].Id, "Sprites/Weapon");
            Assert(weapon.ToString() == "example.weapon:sprites/weapon", "Unqualified asset reference was not canonicalized.");
            AssetMetadata weaponMeta;
            Assert(resolver.TryDescribe(weapon, out weaponMeta), "Loose sprite was not described.");
            Assert(weaponMeta.Kind == AssetKind.Sprite && weaponMeta.Format == ".png" && weaponMeta.Size == 4,
                "Loose sprite metadata is wrong.");
            AssetBytes weaponBytes;
            Assert(resolver.TryRead(weapon, out weaponBytes), "Loose sprite bytes were not readable.");
            Assert(weaponBytes.Data.Length == 4 && weaponBytes.Data[0] == 1 && weaponBytes.Data[3] == 4,
                "Loose sprite bytes changed.");

            AssetId modelId = AssetId.Parse("example.weapon:models/mdl_weapon_example");
            AssetMetadata modelMeta;
            Assert(resolver.TryDescribe(modelId, out modelMeta) && modelMeta.Kind == AssetKind.Model,
                "Loose model was not indexed as a model.");
            Assert(!resolver.TryDescribe(AssetId.Parse("other.mod:sprites/weapon"), out weaponMeta),
                "Resolver crossed namespace ownership.");
            AssetMetadata coreMeta;
            Assert(resolver.TryDescribe(AssetId.Parse("core:ui/test"), out coreMeta) &&
                coreMeta.SourceKind == AssetSourceKind.Core, "Core logical provider was not routed.");
            Assert(!resolver.TryRead(AssetId.Parse("core:ui/test"), out weaponBytes),
                "Logical-only core provider unexpectedly exposed raw bytes.");

            bool duplicateNamespaceRejected = false;
            try { new AssetResolver(new IAssetProvider[] { new FakeCoreProvider(), new FakeCoreProvider() }); }
            catch (InvalidOperationException) { duplicateNamespaceRejected = true; }
            Assert(duplicateNamespaceRejected, "Duplicate provider namespace was accepted.");
        }
        finally
        {
            if (Directory.Exists(modsRoot)) Directory.Delete(modsRoot, true);
        }

        Console.WriteLine("Modding foundation contracts: PASS");
        return 0;
    }
}
'@ | Set-Content -LiteralPath $harness -Encoding UTF8

& $csc /nologo /langversion:9.0 /target:exe "/out:$exe" @sources $harness
if ($LASTEXITCODE -ne 0) { throw "Contract compilation failed with exit code $LASTEXITCODE." }

& $exe
if ($LASTEXITCODE -ne 0) { throw "Contract tests failed with exit code $LASTEXITCODE." }
