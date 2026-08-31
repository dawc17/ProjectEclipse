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
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\LooseModProvider.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModScripting.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModContent.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModLocalizationLoader.cs')
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

    private static void RejectContent(Action action, string message)
    {
        try
        {
            action();
        }
        catch (ModContentException)
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
        Reject(() => ModManifestReader.ParseExternal(
            Manifest("bad.entrypoint", "1.0.0", ">=0.1 <1.0").Replace(
                "entrypoint = \"scripts/main.lua\"", "entrypoint = \"main.lua\"")),
            "Manifest entrypoint outside scripts/ was accepted.");
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

        var content = new ModContentCatalog();
        ModDescriptor contentMod = Descriptor("content.weapon", "1.0.0", "core", ">=1.0 <2.0");
        using (ModRegistrationTransaction registration = content.BeginRegistration(contentMod))
        {
            DefinitionId title = registration.AddLocalization("weapon.example_blade", "eng", "Example Blade");
            registration.AddLocalization("weapon.example_blade", "pol", "Przykladowe Ostrze");
            WeaponDefinition registeredWeapon = registration.RegisterWeapon("example_blade", title,
                AssetId.Parse("content.weapon:sprites/weapon"),
                AssetId.Parse("content.weapon:models/mdl_weapon_example"), "Katana", 18);
            ShopListingDefinition listing = registration.RegisterShopListing(registeredWeapon.Id,
                ModShopSection.Weapons, 12, new ModPrice(ModPriceCurrency.Coins, 1000));
            Assert(content.Weapons.Count == 0 && content.ShopListings.Count == 0 && content.Localizations.Count == 0,
                "Uncommitted transaction leaked into global registries.");
            Assert(listing.Item == registeredWeapon.Id && listing.Level == 12 && listing.Price.Amount == 1000,
                "Staged shop listing changed values.");
            registration.Commit();
        }
        Assert(content.Localizations.Count == 1 && content.Weapons.Count == 1 && content.ShopListings.Count == 1,
            "Committed transaction did not populate all registries.");
        LocalizationDefinition committedTitle;
        Assert(content.TryGetLocalization(DefinitionId.Parse("content.weapon:localization/weapon.example_blade"),
            out committedTitle) && committedTitle.GetOrEnglish("pol") == "Przykladowe Ostrze" &&
            committedTitle.GetOrEnglish("missing") == "Example Blade",
            "Committed localization lookup/fallback is wrong.");
        WeaponDefinition committedWeapon;
        Assert(content.TryGetWeapon(DefinitionId.Parse("content.weapon:items/weapon/example_blade"), out committedWeapon) &&
            committedWeapon.Damage == 18 && committedWeapon.SubType == "Katana",
            "Committed weapon lookup is wrong.");

        var rollbackCatalog = new ModContentCatalog();
        using (ModRegistrationTransaction rollback = rollbackCatalog.BeginRegistration(contentMod))
        {
            rollback.AddLocalization("weapon.rolled_back", "eng", "Rolled Back");
        }
        Assert(rollbackCatalog.Localizations.Count == 0, "Disposed transaction did not roll back staged definitions.");

        var atomicCatalog = new ModContentCatalog();
        using (ModRegistrationTransaction first = atomicCatalog.BeginRegistration(contentMod))
        {
            first.AddLocalization("weapon.shared", "eng", "Shared");
            first.Commit();
        }
        using (ModRegistrationTransaction conflicting = atomicCatalog.BeginRegistration(contentMod))
        {
            DefinitionId duplicateTitle = conflicting.AddLocalization("weapon.shared", "eng", "Duplicate");
            conflicting.RegisterWeapon("must_not_commit", duplicateTitle,
                AssetId.Parse("content.weapon:sprites/weapon"),
                AssetId.Parse("content.weapon:models/mdl_weapon_example"), "Katana", 10);
            RejectContent(() => conflicting.Commit(), "Registry collision was not rejected transactionally.");
        }
        Assert(atomicCatalog.Localizations.Count == 1 && atomicCatalog.Weapons.Count == 0,
            "Failed commit partially changed global registries.");

        var fallbackCatalog = new ModContentCatalog();
        using (ModRegistrationTransaction missingFallback = fallbackCatalog.BeginRegistration(contentMod))
        {
            missingFallback.AddLocalization("weapon.no_english", "pol", "Brak English");
            RejectContent(() => missingFallback.Commit(), "Localization without eng fallback was accepted.");
        }
        Assert(fallbackCatalog.Localizations.Count == 0, "Invalid localization partially committed.");

        content.Freeze();
        bool frozenRejected = false;
        try { content.BeginRegistration(contentMod); }
        catch (InvalidOperationException) { frozenRejected = true; }
        Assert(frozenRejected, "Frozen definition registries accepted a new transaction.");

        string modsRoot = Path.Combine(Path.GetTempPath(), "sf2de-modding-contracts-" + Guid.NewGuid().ToString("N"));
        try
        {
            string validRoot = Path.Combine(modsRoot, "example.weapon");
            Directory.CreateDirectory(validRoot);
            File.WriteAllText(Path.Combine(validRoot, "mod.toml"),
                Manifest("example.weapon", "1.0.0", ">=0.1 <1.0", "core", ">=1.0 <2.0"));
            string spriteRoot = Path.Combine(validRoot, "assets", "sprites");
            string textureRoot = Path.Combine(validRoot, "assets", "textures");
            string modelRoot = Path.Combine(validRoot, "assets", "models");
            Directory.CreateDirectory(spriteRoot);
            Directory.CreateDirectory(textureRoot);
            Directory.CreateDirectory(modelRoot);
            string spriteDescriptor = "type=sprite\ntexture=textures/weapon.png\npivot=[0.5, 0.5]\n";
            File.WriteAllText(Path.Combine(spriteRoot, "weapon.asset"), spriteDescriptor);
            File.WriteAllBytes(Path.Combine(textureRoot, "weapon.png"), new byte[] { 1, 2, 3, 4 });
            File.WriteAllBytes(Path.Combine(spriteRoot, "legacy.png"), new byte[] { 1, 2, 3, 4 });
            File.WriteAllText(Path.Combine(spriteRoot, "legacy.sprite.toml"), "pivot = [0.5, 0.5]");
            // Type comes from the descriptor, not its directory.
            File.WriteAllText(Path.Combine(validRoot, "assets", "outside.asset"), spriteDescriptor);
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
            Assert(weaponMeta.Kind == AssetKind.Sprite && weaponMeta.Format == ".asset",
                "Loose sprite metadata is wrong.");
            AssetBytes weaponBytes;
            Assert(resolver.TryRead(weapon, out weaponBytes), "Loose sprite bytes were not readable.");
            Assert(System.Text.Encoding.UTF8.GetString(weaponBytes.Data) == spriteDescriptor,
                "Loose sprite descriptor bytes changed.");
            AssetId texture = AssetId.Parse("example.weapon:textures/weapon");
            Assert(resolver.TryDescribe(texture, out weaponMeta) && weaponMeta.Kind == AssetKind.Texture &&
                weaponMeta.Format == ".png", "Texture was not indexed separately from sprite.");
            Assert(resolver.TryRead(texture, out weaponBytes) && weaponBytes.Data.Length == 4 &&
                weaponBytes.Data[0] == 1 && weaponBytes.Data[3] == 4, "Texture payload changed.");
            Assert(resolver.TryDescribe(AssetId.Parse("example.weapon:outside"), out weaponMeta) &&
                weaponMeta.Kind == AssetKind.Sprite, "Descriptor type depended on its folder.");
            Assert(resolver.TryDescribe(AssetId.Parse("example.weapon:sprites/legacy"), out weaponMeta) &&
                weaponMeta.Kind == AssetKind.Sprite && weaponMeta.Format == ".png", "Legacy PNG sprite regressed.");

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

            foreach (string invalid in new[] { "texture=textures/weapon.png", "type=unknown",
                "type=sprite\ntype=sprite", "type=\"sprite", "type=\"sprite\"junk" })
            {
                bool rejected = false;
                try { AssetDescriptor.GetKind(AssetDescriptor.ParseFields(invalid, "invalid.asset"), "invalid.asset"); }
                catch (InvalidDataException) { rejected = true; }
                Assert(rejected, "Malformed/unsupported descriptor was accepted: " + invalid);
            }
            Assert(AssetDescriptor.GetKind(AssetDescriptor.ParseFields("type=\"sprite\" # comment", "quoted.asset"),
                "quoted.asset") == AssetKind.Sprite, "Quoted descriptor type/comment was rejected.");

            string invalidUtf8Path = Path.Combine(spriteRoot, "invalid_utf8.asset");
            File.WriteAllBytes(invalidUtf8Path, new byte[] { 0xFF });
            bool invalidUtf8Rejected = false;
            try { new LooseModProvider(discovery.Mods[0]); }
            catch (InvalidDataException) { invalidUtf8Rejected = true; }
            Assert(invalidUtf8Rejected, "Invalid descriptor UTF-8 did not produce a mount diagnostic exception.");
            File.Delete(invalidUtf8Path);

            File.WriteAllBytes(Path.Combine(spriteRoot, "weapon.png"), new byte[] { 1 });
            bool collisionRejected = false;
            try { new LooseModProvider(discovery.Mods[0]); }
            catch (InvalidDataException) { collisionRejected = true; }
            Assert(collisionRejected, "Descriptor and legacy sprite at the same logical ID were accepted.");
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
