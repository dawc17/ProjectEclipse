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
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\ModSaveData.cs'),
    (Join-Path $root 'Assets\Scripts\Eclipse\Runtime\Modding\CoreContentImporter.cs'),
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
using System.Xml;
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

    public static int Main(string[] args)
    {
        CheckCoreAndSaveContracts(args[0]);
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
                AssetId.Parse("content.weapon:models/mdl_weapon_example"), "Katana");
            RejectContent(() => registration.RegisterShopListing(registeredWeapon.Id,
                ModShopSection.Weapons, 53, new ModPrice(ModPriceCurrency.Coins, 1000)),
                "Equipment shop listing above the vanilla level cap was accepted.");
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
            committedWeapon.Damage == 0 && committedWeapon.SubType == "Katana" &&
            committedWeapon.Progression == ItemProgressionKind.Vanilla,
            "Committed weapon lookup is wrong.");

        var levelRules = new ModContentCatalog();
        using (ModRegistrationTransaction registration = levelRules.BeginRegistration(contentMod))
        {
            DefinitionId armorTitle = registration.AddLocalization("armor.level_test", "eng", "Level Armor");
            ArmorDefinition armor = registration.RegisterArmor("level_test", armorTitle,
                AssetId.Parse("content.weapon:sprites/armor"), AssetId.Parse("content.weapon:models/armor"));
            RejectContent(() => registration.RegisterShopListing(armor.Id, ModShopSection.Armor, 1,
                new ModPrice(ModPriceCurrency.Coins, 1)), "Armor below the vanilla progression minimum was accepted.");
            registration.RegisterShopListing(armor.Id, ModShopSection.Armor, 2,
                new ModPrice(ModPriceCurrency.Coins, 1));

            DefinitionId rangedTitle = registration.AddLocalization("ranged.level_test", "eng", "Level Ranged");
            RangedDefinition ranged = registration.RegisterRanged("level_test", rangedTitle,
                AssetId.Parse("content.weapon:sprites/ranged"), AssetId.Parse("content.weapon:models/ranged"), "Shuriken");
            RejectContent(() => registration.RegisterShopListing(ranged.Id, ModShopSection.Ranged, 5,
                new ModPrice(ModPriceCurrency.Coins, 1)), "Ranged below the vanilla progression minimum was accepted.");
            registration.RegisterShopListing(ranged.Id, ModShopSection.Ranged, 6,
                new ModPrice(ModPriceCurrency.Coins, 1));
            registration.Commit();
        }

        var redirectsCatalog = new ModContentCatalog();
        using (ModRegistrationTransaction registration = redirectsCatalog.BeginRegistration(contentMod))
        {
            DefinitionId title = registration.AddLocalization("weapon.renamed", "eng", "Renamed Blade");
            WeaponDefinition renamed = registration.RegisterWeapon("renamed_blade", title,
                AssetId.Parse("content.weapon:sprites/weapon"),
                AssetId.Parse("content.weapon:models/mdl_weapon_example"), "Katana");
            registration.RegisterItemAlias("weapon/old_blade", renamed.Id);
            registration.RegisterItemTombstone("weapon/retired_blade");
            RejectContent(() => registration.RegisterItemAlias("armor/old_blade", renamed.Id),
                "Cross-category item alias was accepted.");
            registration.Commit();
        }
        ItemDefinition redirected;
        Assert(redirectsCatalog.TryResolveItem(DefinitionId.Parse("content.weapon:items/weapon/old_blade"), out redirected) &&
            redirected.Id == DefinitionId.Parse("content.weapon:items/weapon/renamed_blade"),
            "Item alias did not resolve to the current definition.");
        Assert(!redirectsCatalog.TryResolveItem(DefinitionId.Parse("content.weapon:items/weapon/retired_blade"), out redirected),
            "Item tombstone resolved as live content.");
        ItemRedirectDefinition tombstone;
        Assert(redirectsCatalog.TryGetItemRedirect(DefinitionId.Parse("content.weapon:items/weapon/retired_blade"), out tombstone) &&
            tombstone.IsTombstone, "Item tombstone was not retained in the registry.");

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
                AssetId.Parse("content.weapon:models/mdl_weapon_example"), "Katana");
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

    private static void CheckCoreAndSaveContracts(string projectRoot)
    {
        var vanilla = new XmlDocument();
        vanilla.Load(Path.Combine(projectRoot, "Assets", "vanillaXml", "list.xml"));
        var weaponNodes = new List<XmlNode>();
        foreach (XmlNode node in vanilla.SelectNodes("/List/Items/Item[@Type='Weapon']")) weaponNodes.Add(node);
        var armorNodes = new List<XmlNode>();
        foreach (XmlNode node in vanilla.SelectNodes("/List/Items/Item[@Type='Armor']")) armorNodes.Add(node);
        var helmNodes = new List<XmlNode>();
        foreach (XmlNode node in vanilla.SelectNodes("/List/Items/Item[@Type='Helm']")) helmNodes.Add(node);
        var rangedNodes = new List<XmlNode>();
        foreach (XmlNode node in vanilla.SelectNodes("/List/Items/Item[@Type='Ranged']")) rangedNodes.Add(node);
        var magicNodes = new List<XmlNode>();
        foreach (XmlNode node in vanilla.SelectNodes("/List/Items/Item[@Type='Magic']")) magicNodes.Add(node);
        var languages = CoreContentImporter.ReadLocalizations(Path.Combine(projectRoot, "Assets", "vanillaXml", "localizations"));
        var catalog = new ModContentCatalog();
        Assert(CoreContentImporter.ImportWeapons(catalog, weaponNodes, languages) == 210 && catalog.Weapons.Count == 210,
            "Canonical vanilla weapon coverage changed.");
        Assert(CoreContentImporter.ImportArmors(catalog, armorNodes, languages) == 179 && catalog.Armors.Count == 179,
            "Canonical vanilla armor coverage changed.");
        Assert(CoreContentImporter.ImportHelms(catalog, helmNodes, languages) == 193 && catalog.Helms.Count == 193,
            "Canonical vanilla helm coverage changed.");
        Assert(CoreContentImporter.ImportRanged(catalog, rangedNodes, languages) == 85 && catalog.Ranged.Count == 85,
            "Canonical vanilla ranged coverage changed.");
        Assert(CoreContentImporter.ImportMagic(catalog, magicNodes, languages) == 73 && catalog.Magic.Count == 73,
            "Canonical vanilla magic coverage changed.");
        foreach (XmlNode node in weaponNodes)
        {
            WeaponDefinition weapon;
            string name = node.Attributes["Name"].Value;
            Assert(catalog.TryGetWeapon(CoreContentImporter.WeaponId(name), out weapon) &&
                weapon.LegacyName == name && weapon.LegacyItemXml == node.OuterXml,
                "Core import lost legacy identity or source fields: " + name);
        }
        foreach (XmlNode node in armorNodes)
        {
            ArmorDefinition armor;
            string name = node.Attributes["Name"].Value;
            Assert(catalog.TryGetArmor(CoreContentImporter.ArmorId(name), out armor) &&
                armor.LegacyName == name && armor.LegacyItemXml == node.OuterXml,
                "Core armor import lost legacy identity or source fields: " + name);
        }
        foreach (XmlNode node in helmNodes)
        {
            HelmDefinition helm;
            string name = node.Attributes["Name"].Value;
            Assert(catalog.TryGetHelm(CoreContentImporter.HelmId(name), out helm) &&
                helm.LegacyName == name && helm.LegacyItemXml == node.OuterXml,
                "Core helm import lost legacy identity or source fields: " + name);
        }
        foreach (XmlNode node in rangedNodes)
        {
            string name = node.Attributes["Name"].Value;
            if (name == "GlaivebowArrow") continue; // Legacy list.xml contains two distinct rows with this same Name.
            RangedDefinition ranged;
            Assert(catalog.TryGetRanged(CoreContentImporter.RangedId(name), out ranged) &&
                ranged.LegacyName == name && ranged.LegacyItemXml == node.OuterXml,
                "Core ranged import lost legacy identity or source fields: " + name);
        }
        XmlNodeList duplicateRanged = vanilla.SelectNodes("/List/Items/Item[@Type='Ranged' and @Name='GlaivebowArrow']");
        RangedDefinition glaiveArrow;
        RangedDefinition rifleBullet;
        Assert(duplicateRanged.Count == 2 &&
            catalog.TryGetRanged(DefinitionId.Parse("core:items/ranged/glaivebowarrow"), out glaiveArrow) &&
            catalog.TryGetRanged(DefinitionId.Parse("core:items/ranged/glaivebowarrow/riflebullet"), out rifleBullet) &&
            glaiveArrow.LegacyItemXml == duplicateRanged[0].OuterXml && rifleBullet.LegacyItemXml == duplicateRanged[1].OuterXml,
            "Duplicate vanilla ranged names were not deterministically disambiguated.");
        foreach (XmlNode node in magicNodes)
        {
            MagicDefinition magic;
            string name = node.Attributes["Name"].Value;
            Assert(catalog.TryGetMagic(CoreContentImporter.MagicId(name), out magic) &&
                magic.LegacyName == name && magic.LegacyItemXml == node.OuterXml,
                "Core magic import lost legacy identity or source fields: " + name);
        }
        WeaponDefinition fists;
        WeaponDefinition kunai;
        Assert(catalog.TryGetWeapon(DefinitionId.Parse("core:items/weapon/fists"), out fists) &&
            fists.Damage == 0 && !fists.HasIcon && !fists.HasModel, "Core Fists was coerced into a normal mod weapon.");
        Assert(catalog.TryGetWeapon(DefinitionId.Parse("core:items/weapon/weapon_kunai"), out kunai) &&
            kunai.Damage == -4 && kunai.HasModel, "Core negative damage sentinel was lost.");
        ArmorDefinition body;
        ArmorDefinition kenji;
        Assert(catalog.TryGetArmor(DefinitionId.Parse("core:items/armor/body"), out body) &&
            body.BodyDefense == 0 && body.UnarmedDamage == 0 && body.HasModel,
            "Core Body armor projection lost default defense/model fields.");
        Assert(catalog.TryGetArmor(DefinitionId.Parse("core:items/armor/body_kenji"), out kenji) &&
            kenji.BodyDefense == -5 && kenji.UnarmedDamage == -5 && kenji.HasModel,
            "Core armor negative sentinel values were lost.");
        HelmDefinition head;
        Assert(catalog.TryGetHelm(DefinitionId.Parse("core:items/helm/head"), out head) &&
            head.HeadDefense == 0 && head.HasModel, "Core default helm projection changed.");
        RangedDefinition noRanged;
        RangedDefinition kunaiRanged;
        Assert(catalog.TryGetRanged(DefinitionId.Parse("core:items/ranged/noranged"), out noRanged) &&
            !noRanged.HasModel && noRanged.RangedDamage == 0 && noRanged.WeaponDamage == 0,
            "Core NoRanged sentinel was coerced into a normal ranged item.");
        Assert(catalog.TryGetRanged(DefinitionId.Parse("core:items/ranged/ranged_fish"), out kunaiRanged) &&
            kunaiRanged.WeaponDamage == -4, "Core ranged WeaponDamage sentinel was lost.");
        MagicDefinition noMagic;
        Assert(catalog.TryGetMagic(DefinitionId.Parse("core:items/magic/nomagic"), out noMagic) &&
            !noMagic.HasModel && noMagic.MagicDamage == 0, "Core NoMagic sentinel was coerced into a normal magic item.");
        Assert(catalog.ShopListings.Count == 0, "Core import invented shop availability/pricing for vanilla weapons.");
        RejectContent(() => CoreContentImporter.ImportWeapons(catalog, weaponNodes, languages), "Duplicate core weapon import was accepted.");
        RejectContent(() => CoreContentImporter.ImportArmors(catalog, armorNodes, languages), "Duplicate core armor import was accepted.");
        RejectContent(() => CoreContentImporter.ImportHelms(catalog, helmNodes, languages), "Duplicate core helm import was accepted.");
        RejectContent(() => CoreContentImporter.ImportRanged(catalog, rangedNodes, languages), "Duplicate core ranged import was accepted.");
        RejectContent(() => CoreContentImporter.ImportMagic(catalog, magicNodes, languages), "Duplicate core magic import was accepted.");
        Assert(catalog.Weapons.Count == 210 && catalog.Armors.Count == 179 && catalog.Helms.Count == 193 &&
            catalog.Ranged.Count == 85 && catalog.Magic.Count == 73 && catalog.Localizations.Count == 739,
            "Failed core import was not atomic.");
        ModDescriptor mod = Descriptor("example.weapon", "1.0.0", "core", ">=1.0 <2.0");
        string coreOnlyFingerprint = ModSaveData.ComputeContentSetFingerprint(new[] { mod }, catalog);
        Assert(coreOnlyFingerprint.StartsWith("sha256:", StringComparison.Ordinal) && coreOnlyFingerprint.Length == 71,
            "Content-set fingerprint did not use the stable SHA-256 wire format.");
        using (ModRegistrationTransaction registration = catalog.BeginRegistration(mod))
        {
            DefinitionId title = registration.AddLocalization("blade", "eng", "Example Blade");
            registration.RegisterWeapon("example_blade", title, AssetId.Parse("example.weapon:sprites/weapon"),
                AssetId.Parse("core:gamedata/models/mdl_weapon_katana_ritual"), "Katana");
            registration.Commit();
        }
        Assert(catalog.Weapons.Count == 211, "Core and external weapons did not coexist in the same registry.");
        string contentFingerprint = ModSaveData.ComputeContentSetFingerprint(new[] { mod }, catalog);
        Assert(contentFingerprint != coreOnlyFingerprint,
            "Content-set fingerprint ignored registered content changes without a version bump.");
        ModDescriptor helper = Descriptor("helper.mod", "2.0.0", "core", ">=1.0 <2.0");
        string orderedFingerprint = ModSaveData.ComputeContentSetFingerprint(new[] { mod, helper }, catalog);
        string reversedFingerprint = ModSaveData.ComputeContentSetFingerprint(new[] { helper, mod }, catalog);
        Assert(orderedFingerprint == reversedFingerprint,
            "Content-set fingerprint depends on active mod discovery order.");
        ModDescriptor changedVersion = Descriptor("example.weapon", "1.0.1", "core", ">=1.0 <2.0");
        Assert(ModSaveData.ComputeContentSetFingerprint(new[] { changedVersion }, catalog) != contentFingerprint,
            "Content-set fingerprint ignored an active mod version change.");
        catalog.Freeze();
        bool frozen = false;
        try { CoreContentImporter.ImportMagic(catalog, new XmlNode[0], languages); }
        catch (InvalidOperationException) { frozen = true; }
        Assert(frozen, "Core importer bypassed registry freezing.");

        const string itemId = "example.weapon:items/weapon/example_blade";
        var save = new XmlDocument();
        save.LoadXml("<Warrior Weapon='" + itemId + "'><Items>" +
            "<Item Name='" + itemId + "' Count='1' UpgradeLevel='1201' Equipped='1' DeliveryTime='1234' " +
            "FutureAttribute='opaque'><Enchantments><FuturePerk value='preserve'/></Enchantments></Item>" +
            "</Items></Warrior>");
        XmlNode record = save.DocumentElement["Items"].FirstChild;
        string originalRecord = record.OuterXml;
        var installed = new HashSet<string>(StringComparer.Ordinal) { "Fists" };
        Assert(ModSaveData.IsMissingItem(record, installed.Contains), "Missing mod item was treated as available.");
        XmlNode view = ModSaveData.CreateEquipmentView(save.DocumentElement, installed.Contains, slot => "Fists");
        Assert(view.Attributes["Weapon"].Value == "Fists" && save.DocumentElement.GetAttribute("Weapon") == itemId &&
            record.OuterXml == originalRecord, "Missing equipment fallback mutated persistent ownership or equipped ID.");
        Assert(ModSaveData.RecordContext(save.DocumentElement, new[] { mod }, catalog), "Mod save context was not written.");
        Assert(save.DocumentElement["EclipseMods"].Attributes["contentHash"]?.Value == contentFingerprint,
            "Mod save context did not persist the current content-set fingerprint.");
        Assert(ModSaveData.RecordContext(save.DocumentElement, new ModDescriptor[0]), "Missing mod context was not recorded.");
        XmlElement lastSeen = (XmlElement)save.SelectSingleNode("/Warrior/EclipseMods/Mod");
        Assert(lastSeen.GetAttribute("version") == "1.0.0" && lastSeen.GetAttribute("active") == "false",
            "Removing a mod erased its last-seen version.");
        var reloaded = new XmlDocument();
        reloaded.LoadXml(save.OuterXml);
        record = reloaded.DocumentElement["Items"].FirstChild;
        Assert(record.OuterXml == originalRecord && ModSaveData.IsMissingItem(record, installed.Contains),
            "An absent mod item did not survive save/reload unchanged.");
        installed.Add(itemId);
        Assert(!ModSaveData.IsMissingItem(record, installed.Contains) &&
            ReferenceEquals(ModSaveData.CreateEquipmentView(reloaded.DocumentElement, installed.Contains, slot => "Fists"),
                reloaded.DocumentElement), "Restored mod did not recover its original saved item/equipment reference.");
        Assert(record.Attributes["Count"].Value == "1" && record.Attributes["UpgradeLevel"].Value == "1201" &&
            record.OuterXml == originalRecord, "Restored item lost ownership, upgrade, or opaque enchantment data.");
        XmlElement state = reloaded.DocumentElement["EclipseMods"];
        state.SetAttribute("schema", "99");
        string futureState = state.OuterXml;
        Assert(!ModSaveData.RecordContext(reloaded.DocumentElement, new[] { mod }) && state.OuterXml == futureState,
            "Unknown future mod-save schema was overwritten.");
        Console.WriteLine("Core/save contracts: PASS (740 vanilla equipment definitions + external mod; orphan save/reload/restore).");
    }
}
'@ | Set-Content -LiteralPath $harness -Encoding UTF8

& $csc /nologo /langversion:9.0 /target:exe "/out:$exe" @sources $harness
if ($LASTEXITCODE -ne 0) { throw "Contract compilation failed with exit code $LASTEXITCODE." }

& $exe $root
if ($LASTEXITCODE -ne 0) { throw "Contract tests failed with exit code $LASTEXITCODE." }
