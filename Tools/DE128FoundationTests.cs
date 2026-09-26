using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Eclipse.Modding;

internal static class DE128FoundationTests
{
    [System.Runtime.InteropServices.DllImport("kernel32.dll", EntryPoint = "CreateHardLinkW",
        CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
    private static extern bool CreateHardLink(string target, string source, IntPtr securityAttributes);

    private static readonly string[] Services =
        { "paid_offers", "battle_pass", "ads", "rewarded_video", "online_services", "payments" };
    private static readonly UTF8Encoding Utf8 = new UTF8Encoding(false);
    private static readonly string[] Capabilities =
        { "policy.services", "policy.timers", "content.register", "content.patch", "combat.modify_outgoing_hit", "combat.effects", "story.events", "story.progression", "profile.read", "state.read", "state.write", "ui.create", "presentation.navigate", "presentation.dojo" };
    private static readonly HashSet<string> CallTimeCapabilities = new HashSet<string> { "profile.read", "state.read", "ui.create", "presentation.navigate", "presentation.dojo" };
    private static readonly DefinitionId Sword = DefinitionId.Parse("de128:items/weapon/titans_desolator");
    private static readonly DefinitionId CoreSword = CoreContentImporter.WeaponId("WEAPON_TITAN_GIANT_SWORD");
    private static XmlDocument _items;
    private static XmlDocument _perks;
    private static XmlDocument _stages;
    private static Dictionary<string, XmlDocument> _languages;
    private static int _checks;
    private static readonly Dictionary<string, AssetKind> RestoredAssets = new Dictionary<string, AssetKind>();
    // Core portraits the Sensei and Underworld stories name; Tools/VerifyDE128SenseiArt.cs
    // loads each natively.
    private static readonly HashSet<string> CorePortraits = new HashSet<string>(new[] {
        "boss_lynx_young", "character_ancient", "character_asian", "character_blind", "character_fanatic",
        "character_indean", "character_philosopher", "character_prince", "character_prince_evil", "character_ronin",
        "character_sadist", "character_savage", "character_sensei_young", "character_sister",
        "boss_ermin", "boss_architect", "boss_architect_hummer", "boss_arkhos", "boss_arkhos_halloween",
        "boss_berstuuk", "boss_bison", "boss_blackness", "boss_crystal", "boss_crystal_halloween", "boss_fatum",
        "boss_fire", "boss_gatekeeper", "boss_hoaxen", "boss_hunger", "boss_lamb", "boss_lamb_fungus",
        "boss_lamb_hunger", "boss_lamb_vulcan", "boss_mushroom", "boss_puppeteer_hw21", "boss_rakshasa",
        "boss_ravana", "boss_saturn", "boss_shurale_ny22", "boss_son_of_the_sun", "boss_tenebris", "boss_vortex",
        "boss_war", "boss_whisper_24", "boss_wind_wolf_new", "character_faradaya", "character_lazarus",
        "character_may_3", "character_nrityu", "character_pristess", "character_puma", "character_puppeteer",
        "character_samson", "character_simon_raid", "character_sitaram_01", "character_sitaram_02",
        "character_sitaram_03", "character_thief_2", "character_thief_3", "hunter_raid" }.Select(name => "ui/users/" + name));

    // Resolves only the declared core references. Native art decoding is a
    // separate check; arbitrary or misspelled asset IDs must not pass this fixture.
    private sealed class CoreMetadata : IAssetProvider
    {
        private readonly string _missing;
        public CoreMetadata(string missing) { _missing = missing; }
        public ModId Namespace => ModId.Parse("core");
        public bool TryDescribe(AssetId id, out AssetMetadata metadata)
        {
            metadata = null;
            if (id.Namespace != Namespace || id.Path == _missing) return false;
            AssetKind kind;
            if (id.Path == "gamedata/models/mdl_weapon_giant_sword") kind = AssetKind.Model;
            else if (id.Path == "ui/items/weapon17.img_weapon_boss_giant_sword") kind = AssetKind.Sprite;
            else if (id.Path == "ui/skills/iconmasterofstyle" || id.Path == "ui/skills/iconmasterofstyle_blue" ||
                id.Path == "ui/skills/iconcrackedapple" || id.Path == "ui/skills/iconcrackedapple_blue") kind = AssetKind.Sprite;
            else if (CorePortraits.Contains(id.Path)) kind = AssetKind.Sprite;
            else if (!RestoredAssets.TryGetValue(id.Path, out kind)) return false;
            metadata = new AssetMetadata(id, kind, AssetSourceKind.Core, string.Empty, -1, "DE128 metadata fixture");
            return true;
        }
    }

    private static XmlDocument ReadXml(string path)
    {
        var document = new XmlDocument { XmlResolver = null };
        using (var reader = XmlReader.Create(path,
            new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null }))
            document.Load(reader);
        return document;
    }

    private static void ImportCore(ModContentCatalog catalog)
    {
        if (catalog.TryGetItem(CoreSword, out _)) return;
        CoreContentImporter.ImportWeapons(catalog, _items.SelectNodes("/List/Items/Item").Cast<XmlNode>(), _languages);
        CoreContentImporter.ImportArmors(catalog, _items.SelectNodes("/List/Items/Item").Cast<XmlNode>(), _languages);
        CoreContentImporter.ImportHelms(catalog, _items.SelectNodes("/List/Items/Item").Cast<XmlNode>(), _languages);
        CoreContentImporter.ImportRanged(catalog, _items.SelectNodes("/List/Items/Item").Cast<XmlNode>(), _languages);
        CoreContentImporter.ImportMagic(catalog, _items.SelectNodes("/List/Items/Item").Cast<XmlNode>(), _languages);
        CoreContentImporter.ImportPerks(catalog, _perks.DocumentElement.ChildNodes.Cast<XmlNode>());
        CoreContentImporter.ImportStages(catalog, _stages.DocumentElement["Zones"]);
        CoreContentImporter.ImportWarriorTemplates(catalog, _stages.SelectSingleNode("Stages/Warriors/Templates"));
        CoreContentImporter.ImportForgeEconomicProfiles(catalog, new[] { "Simple", "Medium", "Complex" });
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
        _checks++;
    }

    private static ModDescriptor Discover(string modsRoot, string id)
    {
        var discovered = ModDiscovery.DiscoverLoose(modsRoot);
        Check(!discovered.HasErrors, "Discovery failed: " + string.Join(" | ", discovered.Diagnostics));
        var resolved = DependencyResolver.Resolve(discovered.Mods, ModPlatformVersions.Core);
        Check(!resolved.HasErrors, "Dependencies failed: " + string.Join(" | ", resolved.Diagnostics));
        return resolved.OrderedMods.Single(mod => mod.Id.Value == id);
    }

    private static ModDescriptor CopyPackage(string source, string fixture, string name,
        string capabilities = null, string omit = null)
    {
        string modsRoot = Path.Combine(fixture, "cases", name, "Mods");
        string destination = Path.Combine(modsRoot, "de128");
        Directory.CreateDirectory(destination);
        foreach (string file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
        {
            string relative = Path.GetRelativePath(source, file);
            if (relative.Replace('\\', '/') == omit) continue;
            string target = Path.Combine(destination, relative);
            Directory.CreateDirectory(Path.GetDirectoryName(target));
            // The negative-case matrix mutates Lua and the manifest, never packaged
            // art/audio. Share those large immutable assets when the fixture is on
            // the same volume so repeated package copies do not exhaust the disk.
            if (!OperatingSystem.IsWindows() ||
                !relative.StartsWith("assets" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                !Path.GetPathRoot(file).Equals(Path.GetPathRoot(target), StringComparison.OrdinalIgnoreCase) ||
                !CreateHardLink(target, file, IntPtr.Zero))
                File.Copy(file, target, false);
        }
        if (capabilities != null)
        {
            string manifest = Path.Combine(destination, "mod.toml");
            string original = File.ReadAllText(manifest);
            string expected = "capabilities = [" + string.Join(", ", Capabilities.Select(value => "\"" + value + "\"")) + "]";
            Check(original.Contains(expected), "Update the capability-negative fixture for the current manifest.");
            File.WriteAllText(manifest, original.Replace(expected, "capabilities = " + capabilities), Utf8);
        }
        return Discover(modsRoot, "de128");
    }

    private static ModDescriptor Peer(string fixture, string id, string capability, string script, string extraCapability = null)
    {
        string modsRoot = Path.Combine(fixture, "peers", id, "Mods");
        string directory = Path.Combine(modsRoot, id);
        Directory.CreateDirectory(Path.Combine(directory, "scripts"));
        File.WriteAllText(Path.Combine(directory, "mod.toml"),
            "schema = 1\nid = \"" + id + "\"\nname = \"Foundation test peer\"\n" +
            "version = \"1.0.0\"\nauthors = [\"Eclipse tests\"]\nentrypoint = \"scripts/main.lua\"\n" +
            "capabilities = [\"" + capability + "\"" + (extraCapability == null ? "" : ", \"" + extraCapability + "\"") + "]\n\n[[dependencies]]\nid = \"core\"\nversion = \">=1.0 <2.0\"\n", Utf8);
        File.WriteAllText(Path.Combine(directory, "scripts", "main.lua"), "local sf2 = require(\"sf2\")\n" + script, Utf8);
        return Discover(modsRoot, id);
    }

    private static void Load(ModDescriptor mod, ModContentCatalog catalog, string missingAsset = null)
    {
        using (LoadLive(mod, catalog, missingAsset)) { }
    }

    private static IModScriptContext LoadLive(ModDescriptor mod, ModContentCatalog catalog, string missingAsset = null) =>
        LoadLive(mod, catalog, new ModStoryEvents((owner, message) => throw new InvalidOperationException(message)),
            new ModStateRuntime(), missingAsset);

    private static IModScriptContext LoadLive(ModDescriptor mod, ModContentCatalog catalog, ModStoryEvents bus,
        ModStateRuntime state, string missingAsset = null)
    {
        ImportCore(catalog);
        // Actual canonical projections and controlled art metadata; no profile is bound.
        var assets = new AssetResolver(new IAssetProvider[] { new CoreMetadata(missingAsset), new LooseModProvider(mod) });
        using (var transaction = catalog.BeginRegistration(mod))
        {
            var api = new ModApiFacade(mod, assets, transaction, state, null);
            // The active package installs story hooks; supply the production event bus.
            var context = new MoonSharpScriptRuntime(null, null, null, bus).CreateContext(mod, api);
            try
            {
                ModLocalizationLoader.Load(mod, assets, transaction);
                context.ExecuteEntrypoint();
                transaction.Commit();
                return context;
            }
            catch { context.Dispose(); throw; }
        }
    }

    private static void ExpectFailure(ModDescriptor mod, ModContentCatalog catalog, string diagnostic,
        string missingAsset = null)
    {
        Exception failure = null;
        try { Load(mod, catalog, missingAsset); }
        catch (Exception exception) { failure = exception; }
        Check(failure != null && failure.ToString().Contains(diagnostic),
            "Expected '" + diagnostic + "', got: " + (failure?.ToString() ?? "success"));
    }

    private static void CheckBase(ModContentCatalog catalog)
    {
        ModPolicies.Content = catalog;
        Check(ModPolicies.DeliverySeconds("forge", 120) == 120, "Base forge duration was not restored.");
        Check(ModPolicies.SkipEnabled("forge"), "Base forge skipping was not restored.");
        Check(!ModPolicies.CompletePending("forge"), "Base pending orders were accelerated.");
        Check(Services.All(ModPolicies.FeatureEnabled), "A failed or absent mod left service disables behind.");
        if (catalog != null)
        {
            Check(!catalog.TryGetItem(Sword, out _), "Failed or absent DE128 left its weapon registered.");
            Check(!catalog.Weapons.Cast<ItemDefinition>().Concat(catalog.Armors).Concat(catalog.Helms).Concat(catalog.Ranged).Concat(catalog.Magic).Any(item => !item.IsCore) && catalog.ShopListings.Count == 0 &&
                catalog.ItemDefaultEnchantments.Count == 0 && catalog.ItemShopPrices.Count == 0 && catalog.ItemPresentations.Count == 0,
                "Failed or absent DE128 left restored equipment behind.");
            Check(catalog.ItemInnatePerks.Count == 0, "Failed or absent DE128 left innate perks registered.");
            Check(catalog.Patches.Count == 0, "Failed or absent DE128 left content patches registered.");
            Check(!catalog.ItemAvailabilityPolicies.Any(), "Failed or absent DE128 left shop policies registered.");
            Check(catalog.ItemCombatSubtypes.Count == 0 && catalog.ItemTacticSubtypes.Count == 0,
                "Failed or absent DE128 left combat classification patches.");
            Check(catalog.ForgeRecipeFamilies.Count == 0 && catalog.ForgeCandidateExclusions.Count == 0 &&
                catalog.ForgeDeviations.Count == 0, "Failed or absent DE128 left forge changes.");
            Check(catalog.Moves.Count == 0 && catalog.MoveItemLockExtensions.Count == 0 && catalog.MoveCombatPatches.Count == 0,
                "Failed or absent DE128 left moves or item-lock extensions.");
        }
    }

    private static void CheckForgeArchive(ModContentCatalog catalog)
    {
        var archived = ReadXml(Path.Combine(_repository, "Assets/DExml/forge.xml"));
        var canonical = ReadXml(Path.Combine(_repository, "Assets/vanillaXml/forge.xml"));
        var categories = Enum.GetValues<ModEquipmentKind>();
        Check(categories.Length == 5, "Forge category coverage changed.");
        string[] Candidates(XmlElement recipe) => recipe.SelectNodes("./Variations/Variation/Enchantments/Perk")
            .Cast<XmlElement>().Select(perk => perk.GetAttribute("Name")).ToArray();
        XmlElement Recipe(XmlDocument document, string name) =>
            document.SelectSingleNode("/Forge/Recipes/Recipe[@Name='" + name + "']") as XmlElement
            ?? throw new InvalidOperationException("Missing forge source recipe: " + name);
        var original = Candidates(Recipe(canonical, "Complex"));
        var first = Candidates(Recipe(archived, "Complex"));
        var second = Candidates(Recipe(archived, "Complex2"));
        var third = Candidates(Recipe(archived, "Complex3"));
        Check(original.Length == 12 && first.Length == 4 && second.Length == 4 && third.Length == 4 &&
            original.OrderBy(x => x).SequenceEqual(first.Concat(second).Concat(third).OrderBy(x => x)),
            "The archived Complex split no longer matches the canonical twelve perks.");
        var removed = second.Concat(third).ToHashSet(StringComparer.Ordinal);
        foreach (var category in categories)
        {
            var exclusions = catalog.ForgeCandidateExclusions.Where(row =>
                row.Profile == DefinitionId.Parse("core:forge-profiles/Complex") && row.Equipment == category)
                .Select(row => row.Perk.LocalId).ToArray();
            Check(exclusions.Length == 8 && exclusions.ToHashSet(StringComparer.OrdinalIgnoreCase)
                .SetEquals(removed), "Complex native candidate exclusions differ: " + category);
            var deviation = catalog.ForgeDeviations.Single(row => row.Profile ==
                DefinitionId.Parse("core:forge-profiles/Simple") && row.Equipment == category);
            var archivedItem = Recipe(archived, "Simple").SelectSingleNode(
                "./Items/Item[@Type='" + category + "']") as XmlElement;
            Check(archivedItem != null && deviation.Minimum == int.Parse(archivedItem.GetAttribute("MinDeviation")) &&
                deviation.Maximum == int.Parse(archivedItem.GetAttribute("MaxDeviation")),
                "Simple deviation differs from the archive: " + category);
        }
        foreach (var (sourceName, localId, names) in new[] {
            ("Complex2", "complex_2", second), ("Complex3", "complex_3", third),
        })
        {
            var family = catalog.ForgeRecipeFamilies.Single(row => row.Id.LocalId == localId);
            var sourceRecipe = Recipe(archived, sourceName);
            Check(family.EconomicProfile == DefinitionId.Parse("core:forge-profiles/Complex") &&
                family.Items.Count == 5 && family.Candidates.Count == 20 &&
                family.Alias == "de128:localization/forge." + localId,
                "Complex family shape or economic profile changed: " + sourceName);
            foreach (var category in categories)
            {
                var item = family.Items.Single(row => row.Equipment == category);
                var sourceItem = sourceRecipe.SelectSingleNode("./Items/Item[@Type='" + category + "']") as XmlElement;
                Check(sourceItem != null && item.Enchantments == int.Parse(sourceItem.GetAttribute("Enchantments")) &&
                    item.BarScale == sourceItem.GetAttribute("BarScale") && !item.RandomAspect &&
                    item.MinDeviation == 0 && item.MaxDeviation == 0,
                    "Complex item settings differ: " + sourceName + "/" + category);
                var candidates = family.Candidates.Where(row => row.Equipment == category)
                    .Select(row => row.Perk.LocalId).ToArray();
                Check(candidates.Length == 4 && candidates.SequenceEqual(names, StringComparer.OrdinalIgnoreCase),
                    "Complex candidate pool differs: " + sourceName + "/" + category);
                var priceName = sourceItem.GetAttribute("Prices");
                var sourcePrices = sourceRecipe.SelectNodes("./Prices/PriceBlock[@Name='" + priceName + "']/Price")
                    .Cast<XmlElement>().ToArray();
                var corePrices = Recipe(canonical, "Complex")
                    .SelectNodes("./Prices/PriceBlock[@Name='" + priceName + "']/Price")
                    .Cast<XmlElement>().ToArray();
                Check(sourcePrices.Length == corePrices.Length && sourcePrices.Length > 0 &&
                    sourcePrices.Zip(corePrices).All(pair =>
                        pair.First.Attributes.Cast<XmlAttribute>().All(attr =>
                            pair.Second.GetAttribute(attr.Name) == attr.Value) &&
                        pair.Second.Attributes.Count == pair.First.Attributes.Count),
                    "Complex prices no longer match the immutable core profile: " + sourceName + "/" + category);
            }
            Check(catalog.TryGetLocalization(DefinitionId.Parse("de128:localization/forge." + localId),
                out var title), "Forge recipe title was not registered: " + sourceName);
            foreach (var language in Directory.EnumerateFiles(Path.Combine(_repository, "Mods/de128/localizations"), "*.toml")
                .Select(Path.GetFileNameWithoutExtension))
            {
                var words = ReadXml(Path.Combine(_repository, "Assets/DExml/localizations", language + ".xml"));
                var sourceTitle = words.SelectSingleNode("//Word[@Title='forgeRecipe" + sourceName + "']")?.InnerText;
                Check(!string.IsNullOrEmpty(sourceTitle) && title.TryGet(language, out var translated) &&
                    translated == sourceTitle, "Forge recipe localization differs: " + sourceName + "/" + language);
            }
        }
    }

    private static void CheckDojoArchive(ModContentCatalog catalog)
    {
        var source = ReadXml(Path.Combine(_repository, "Assets/DExml/quests.xml"));
        var button = catalog.DojoButtons.Single();
        var archivedButton = source.SelectSingleNode("/Root/Quest[@Name='DojoChanger_MapButton']/Actions/ShowMapButton") as XmlElement;
        // The archive shows the changer on the map; Eclipse keeps its icon and
        // behavior but hosts it in the dojo menu below the disciple toggle.
        Check(archivedButton != null && button.Name == "de128.dojo_changer" && archivedButton.GetAttribute("Name") == "DojoChanger" &&
            archivedButton.GetAttribute("Image") == "Textures/Buttons/Map/credits" &&
            button.Image.ToString() == "de128:sprites/dojo_changer/credits" &&
            File.ReadAllText(Path.Combine(_repository, "Mods/de128/assets/sprites/dojo_changer/credits.asset"))
                .Contains("texture=textures/dojo_changer/credits.png") &&
            Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(
                Path.Combine(_repository, "Mods/de128/assets/textures/dojo_changer/credits.png")))) ==
                "208C19723A0BEDD9985D18F088D02341BAB0BEDFC3C9DA4061B13B3E55D2BE53",
            "Dojo button lost its archived icon or identity.");
        var choices = new[] {
            ("dojo", "DefaultDojo"), ("new_year_24_china_dojo", "DojoChinese24"),
            ("dojo_indian_event", "DojoIndia"), ("dojo_indian_event_22", "DojoIndia22"),
            ("dojo_india24", "DojoIndia24"), ("haloween_dojo", "DojoHalloween"),
            ("haloween_dojo_2019", "DojoHalloween19"), ("dojo_hw21", "DojoHalloween21"),
            ("dojo_american_event_22", "DojoAmerican22"), ("dojo_hw22", "DojoStudio")
        };
        var lua = File.ReadAllText(Path.Combine(_repository, "Mods/de128/scripts/content/dojo_changer.lua"));
        var languages = Directory.EnumerateFiles(Path.Combine(_repository, "Mods/de128/localizations"), "*.toml")
            .Select(Path.GetFileNameWithoutExtension)
            .ToDictionary(language => language, language => ReadXml(Path.Combine(
                _repository, "Assets/DExml/localizations", language + ".xml")));
        int previous = -1;
        foreach (var (location, label) in choices)
        {
            var declaration = "{ location = \"" + location + "\", label = \"" + label + "\" }";
            int current = lua.IndexOf(declaration, StringComparison.Ordinal);
            Check(current > previous, "Dojo chooser order changed: " + location);
            previous = current;
            var asset = Path.Combine(_repository, "Mods/de128/assets/sprites/dojo_changer", location + ".asset");
            var texture = Path.Combine(_repository, "Mods/de128/assets/textures/dojo_changer", location + ".png");
            Check(File.Exists(asset) && File.Exists(texture) &&
                File.ReadAllText(asset).Contains("texture=textures/dojo_changer/" + location + ".png") &&
                File.ReadAllText(asset).Contains("rect=[198, 208, 617, 617]") &&
                new FileInfo(texture).Length > 100000,
                "Dojo preview is missing its packaged artwork: " + location);
            if (location != "dojo")
            {
                var load = source.SelectSingleNode("/Root/Quest[@Name='" + label + "_Load']/Actions/ChangeDojoLocation") as XmlElement;
                Check(load != null && load.GetAttribute("Name") == location,
                    "Dojo choice differs from the archived location: " + location);
            }
            foreach (var (language, words) in languages)
            {
                var original = words.SelectSingleNode("//Word[@Title='" + label + "']")?.InnerText;
                Check(catalog.TryGetLocalization(DefinitionId.Parse("de128:localization/dojo." + label), out var localized) &&
                    localized.TryGet(language, out var actual) && actual == original && !string.IsNullOrEmpty(original),
                    "Dojo localization differs from the archive: " + label + "/" + language);
            }
        }
    }

    private static void CheckCampaignMusicArchive(ModContentCatalog catalog)
    {
        var archive = ReadXml(Path.Combine(_repository, "Assets/DExml/stages.xml"));
        var sources = new[] {
            (Zone: "ZONE_1", Battle: "Tournament_INTERMISSION", Track: "ninja_in_the_night_old", Folder: "campaign", Count: 8,
                Hash: "7B673AFA2D1AF5B16417E758364FC2EB80654E3739F2CF943128D18D7D1592B5"),
            (Zone: "ZONE_2", Battle: "BOSS_HERMIT_INTERMISSION", Track: "old_sensei_old", Folder: "campaign", Count: 1,
                Hash: "1E21A2BF2F1BEE24621D48E30F2A3AF15BE878EA54B1ADF577F66E32EB29649C"),
            (Zone: "ZONE_3", Battle: "Tournament_INTERMISSION", Track: "deadly_smoke_old", Folder: "campaign", Count: 8,
                Hash: "2CA0DBA66AD2369358F6A853247C05C35D0E18B5D6C0EAA5ADD4E62C6ECA57F1"),
            (Zone: "ZONE_6", Battle: "QuestBattle", Track: "burning_town_old", Folder: "campaign", Count: 1,
                Hash: "99E7F90061C17540E4D4588CCCC39E680FD5FE0811B5B0D512B84CCB4CE01F93"),
        };
        var expected = new HashSet<DefinitionId>();
        foreach (var source in sources)
        {
            string xpath = "/Stages/Zones/Zone[@Name='" + source.Zone + "']/Battle[@Name='" + source.Battle + "']";
            var archivedBattle = archive.SelectSingleNode(xpath) as XmlElement;
            var baseBattle = _stages.SelectSingleNode(xpath) as XmlElement;
            Check(archivedBattle != null && baseBattle != null && archivedBattle.GetAttribute("Music") == source.Track &&
                baseBattle.GetAttribute("Music") != source.Track,
                "Campaign music source or canonical baseline changed: " + source.Zone + "/" + source.Battle);
            var archivedFights = archivedBattle.SelectNodes("Fight").Cast<XmlElement>().ToArray();
            var baseFights = baseBattle.SelectNodes("Fight").Cast<XmlElement>().ToArray();
            Check(archivedFights.Length == source.Count && baseFights.Length == source.Count &&
                archivedFights.Select(fight => fight.GetAttribute("Name")).SequenceEqual(
                    baseFights.Select(fight => fight.GetAttribute("Name"))),
                "Campaign battle fight set changed: " + source.Zone + "/" + source.Battle);
            string filename = Path.Combine(_repository, "Mods/de128/assets/audio", source.Folder, source.Track + ".wav");
            Check(File.Exists(filename) && Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
                File.ReadAllBytes(filename))) == source.Hash, "Campaign track differs from owner source: " + source.Track);
            foreach (var baseFight in baseFights)
            {
                var id = CoreContentImporter.FightId(source.Zone, source.Battle, baseFight.GetAttribute("Name"));
                expected.Add(id);
                string music = "de128:audio/" + source.Folder + "/" + source.Track;
                Check(catalog.TryGetFight(id, out var fight) && fight.Music == music &&
                    fight.Battle == CoreContentImporter.BattleId(source.Zone, source.Battle),
                    "Campaign fight music patch differs: " + id);
                var projected = (XmlElement)baseFight.CloneNode(true);
                ModFightPatchProjection.Apply(projected, fight, ModContentPolicies.FightMusic, catalog, null);
                Check(projected.GetAttribute("Music") == music &&
                    projected.InnerXml == baseFight.InnerXml &&
                    projected.Attributes.Count == baseFight.Attributes.Count + (baseFight.HasAttribute("Music") ? 0 : 1),
                    "Campaign music projection changed another fight field: " + id);
            }
        }
        var patches = catalog.Patches.Where(patch => patch.Owner.Value == "de128" &&
            patch.Field == ModContentPolicies.FightMusic).ToArray();
        Check(expected.Count == 18 && patches.Length == expected.Count &&
            new HashSet<DefinitionId>(patches.Select(patch => patch.Target)).SetEquals(expected),
            "Campaign music patches differ from the 18 archived fight assignments.");
    }

    private static void CheckDojoInteraction(ModDescriptor mod)
    {
        var catalog = new ModContentCatalog();
        ImportCore(catalog);
        var assets = new AssetResolver(new IAssetProvider[] { new CoreMetadata(null), new LooseModProvider(mod) });
        string storyError = null;
        var events = new ModStoryEvents((owner, message) => storyError = owner + ": " + message);
        var selection = new ModDojoSelection();
        selection.SetCoreLocationValidator(name => name == "dojo" || name == "new_year_24_china_dojo");
        ModUiSurface view = null;
        string destination = null;
        var priorNavigation = ModSceneAccess.Open;
        ModSceneAccess.Open = scene => { destination = scene; return true; };
        try
        {
            using (var transaction = catalog.BeginRegistration(mod))
            using (var script = new MoonSharpScriptRuntime(surface => {
                view = surface; surface.SetInputAllowed(true);
            }, null, selection, events).CreateContext(mod,
                new ModApiFacade(mod, assets, transaction, new ModStateRuntime(), null)))
            {
                ModLocalizationLoader.Load(mod, assets, transaction);
                script.ExecuteEntrypoint();
                transaction.Commit();
                var save = new XmlDocument();
                save.LoadXml("<Warrior><EclipseMods schema='1'/></Warrior>");
                selection.Bind(save.DocumentElement);
                events.BindProfile();
                Check(events.HasSubscribers(ModStoryEventKind.DojoButton), "DE128 did not subscribe to dojo-button clicks.");
                events.Publish(new ModStoryEvent(ModStoryEventKind.DojoButton, null, button: "other.button"));
                Check(view == null, "Unrelated dojo button opened the dojo selector.");
                events.Publish(new ModStoryEvent(ModStoryEventKind.DojoButton, null, button: "de128.dojo_changer"));
                Check(view != null && !view.IsClosed, "DE128 dojo button did not open its localized selector (view=" +
                    (view == null ? "null" : "closed=" + view.IsClosed) + ", error=" + storyError + ").");
                Check(view.Root.Kind == ModUiKind.Stack && view.Root.Style.Frame == "scroll" &&
                    view.WidgetCount == 76 && view.Read("choice_2").Text == "" &&
                    view.Read("name_2").Text == "Chinese New Year Dojo" &&
                    !view.Read("halo_2").Visible &&
                    view.Read("preview_2").Sprite?.ToString() ==
                        "de128:sprites/dojo_changer/new_year_24_china_dojo" &&
                    view.Root.Children[0].Children[1].Kind == ModUiKind.Scroll,
                    "DE128 dojo chooser lost its framed, captioned medallion scroll gallery.");
                view.TryClick("close");
                Check(view.IsClosed && selection.SavedLocation == "", "Closing the selector changed the dojo preference.");
                events.Publish(new ModStoryEvent(ModStoryEventKind.DojoButton, null, button: "de128.dojo_changer"));
                Check(view != null && !view.IsClosed, "The selector did not reopen.");
                view.TryClick("choice_2");
                Check(view.IsClosed && selection.SavedLocation == "core:locations/new_year_24_china_dojo" &&
                    destination == "dojo", "DE128 dojo choice did not save and navigate through its real Lua callback.");
            }
        }
        finally { ModSceneAccess.Open = priorNavigation; }
    }

    // sf2.battles.patch: DE128 moves exactly the core battles whose owner stages.xml
    // placement differs from vanilla, and the API rejects unsafe or conflicting edits.
    private static void CheckBattlePositions(ModDescriptor mod, ModContentCatalog enabled, string fixture)
    {
        Dictionary<string, (string X, string Y)> Positions(string relative)
        {
            var result = new Dictionary<string, (string, string)>();
            foreach (XmlElement zone in ReadXml(Path.Combine(_repository, relative)).SelectNodes("/Stages/Zones/Zone"))
                foreach (XmlElement battle in zone.SelectNodes("Battle[@X]"))
                    result[CoreContentImporter.BattleId(zone.GetAttribute("Name"), battle.GetAttribute("Name")).ToString()] =
                        (battle.GetAttribute("X"), battle.GetAttribute("Y"));
            return result;
        }
        var vanilla = Positions("Assets/vanillaXml/stages.xml");
        var owner = Positions("ResearchSources/de128_assets/gamedata/stages.xml");
        var moved = vanilla.Keys.Where(id => owner.TryGetValue(id, out var value) && value != vanilla[id]).OrderBy(id => id).ToArray();
        var patched = enabled.Patches.Where(patch => patch.Field == "battle/position").ToArray();
        Check(moved.Length == 3 && patched.Length == 3 &&
            patched.Select(patch => patch.Target.ToString()).OrderBy(id => id).SequenceEqual(moved) &&
            patched.All(patch => patch.Owner == mod.Id && patch.Operation == ModContentPatchOperation.Replace),
            "DE128 battle placements differ from the owner stage diff: " + string.Join(",", moved));
        foreach (string id in moved)
            Check(enabled.TryGetBattle(DefinitionId.Parse(id), out var battle) &&
                battle.X.ToString(CultureInfo.InvariantCulture) == owner[id].X &&
                battle.Y.ToString(CultureInfo.InvariantCulture) == owner[id].Y, "Patched battle position differs: " + id);
        var untouched = new ModContentCatalog(); ImportCore(untouched);
        Check(untouched.TryGetBattle(DefinitionId.Parse("core:battles/zone_6/duel"), out var original) &&
            original.X == -370 && original.Y == 50 && enabled.Battles.Count(b => b.IsCore) == untouched.Battles.Count,
            "Core Duel changed outside the patch, or the core battle set changed.");

        string Fingerprint(string body)
        {
            var peer = Peer(fixture, "fixture.battle-position-" + Math.Abs(body.GetHashCode()), "content.patch", body);
            var catalog = new ModContentCatalog(); Load(peer, catalog);
            return ModSaveData.ComputeContentSetFingerprint(new[] { peer }, catalog);
        }
        string duel = "core:battles/zone_6/duel";
        var hashes = new[] {
            Fingerprint("sf2.battles.patch { target='" + duel + "', x=-300, y=100 }"),
            Fingerprint("sf2.battles.patch { target='" + duel + "', x=-301, y=100 }"),
            Fingerprint("sf2.battles.patch { target='" + duel + "', y=100 }"),
        };
        Check(hashes.Distinct().Count() == 3, "Battle placement is absent from the content fingerprint.");
        var partial = new ModContentCatalog();
        Load(Peer(fixture, "fixture.battle-position-y", "content.patch", "sf2.battles.patch { target='" + duel + "', y=100 }"), partial);
        Check(partial.TryGetBattle(DefinitionId.Parse(duel), out var half) && half.X == -370 && half.Y == 100 &&
            half.Fights.SequenceEqual(original.Fights) && half.LegacyName == original.LegacyName && half.Kind == original.Kind,
            "A y-only battle patch changed other battle fields.");

        var failures = new[] {
            ("sf2.battles.patch { target='" + duel + "' }", "needs x or y"),
            ("sf2.battles.patch { target='" + duel + "', x=10001 }", "-10000..10000"),
            ("sf2.battles.patch { target='" + duel + "', x=1.5 }", "integer"),
            ("sf2.battles.patch { target='" + duel + "', x=1, title='x' }", "title"),
            ("sf2.battles.patch { target='core:fights/zone_6/duel/1', x=1 }", "battles category"),
            ("sf2.battles.patch { target='core:battles/zone_6/missing', x=1 }", "not registered"),
            ("sf2.battles.patch { target='" + duel + "', x=1 }\nsf2.battles.patch { target='" + duel + "', y=1 }", "Duplicate battle patch"),
            ("local z=sf2.zones.register{id='z',file='Map1.1'}\nsf2.battles.register{id='b',zone=z,type=sf2.battles.STORY}\n" +
                "sf2.battles.patch { target=sf2.mod.id..':battles/b', x=1 }", "registering your own battle"),
        };
        for (int i = 0; i < failures.Length; i++)
            ExpectFailure(Peer(fixture, "fixture.battle-position-bad" + i, "content.patch", failures[i].Item1, "content.register"),
                new ModContentCatalog(), failures[i].Item2);
        ExpectFailure(Peer(fixture, "fixture.battle-position-capability", "content.register",
            "sf2.battles.patch { target='" + duel + "', x=1 }"), new ModContentCatalog(), "content.patch");
        // A second owner of the same placement conflicts and leaves the first intact.
        var shared = new ModContentCatalog();
        Load(Peer(fixture, "fixture.battle-position-first", "content.patch", "sf2.battles.patch { target='" + duel + "', x=5 }"), shared);
        ExpectFailure(mod, shared, "battle/position");
        Check(shared.TryGetBattle(DefinitionId.Parse(duel), out var kept) && kept.X == 5 && kept.Y == 50 &&
            shared.Patches.Count(patch => patch.Field == "battle/position") == 1 && !shared.Battles.Any(b => b.Id.Namespace.Value == "de128"),
            "A conflicting battle placement leaked DE128 content or replaced the first owner's patch.");
    }

    private static void CheckMapButtonFingerprint(string fixture)
    {
        string Hash(string image, int x, string anchors, string showType)
        {
            string script = "sf2.quests.register{id='map_button',events={'session'},actions={{type='show_map_button'," +
                "id='selector',image='" + image + "',x=" + x + ",y=-200," + anchors +
                ",show_type='" + showType + "'}}}";
            var peer = Peer(fixture, "fixture.map-fingerprint", "content.register", script);
            var catalog = new ModContentCatalog();
            Load(peer, catalog);
            return ModSaveData.ComputeContentSetFingerprint(new[] { peer }, catalog);
        }
        var variants = new[] {
            Hash("Textures/Buttons/Map/credits", -400, "anchor_min_x=0.5,anchor_max_x=0.5", "both"),
            Hash("Textures/Buttons/Map/credits", -401, "anchor_min_x=0.5,anchor_max_x=0.5", "both"),
            Hash("Textures/Buttons/Map/other", -400, "anchor_min_x=0.5,anchor_max_x=0.5", "both"),
            Hash("Textures/Buttons/Map/credits", -400, "anchor_min_x=0.2,anchor_max_x=0.8", "both"),
            Hash("Textures/Buttons/Map/credits", -400, "anchor_min_x=0.5,anchor_max_x=0.5", "story"),
        };
        Check(variants.Distinct().Count() == variants.Length,
            "Native map-button image, placement or display mode is absent from the content fingerprint.");
    }

    private static void CheckDE(ModContentCatalog catalog)
    {
        ModPolicies.Content = catalog;
        Check(catalog.TimerPolicies.Count == 1 && catalog.TryGetTimer("forge", out var timer) &&
            timer.Owner.Value == "de128", "The forge policy is not exclusively owned by DE128.");
        Check(ModPolicies.DeliverySeconds("forge", 120) == 0, "New forge orders are not instant.");
        Check(ModPolicies.SkipEnabled("forge"), "Already-pending orders lost their normal skip path.");
        Check(ModPolicies.CompletePending("forge"), "DE pending orders are not eligible for normal settlement.");
        Check(catalog.ForgeRecipeFamilies.Count == 2 && catalog.ForgeCandidateExclusions.Count == 40 &&
            catalog.ForgeDeviations.Count == 5, "DE forge pools or deviations are incomplete.");
        Check(Services.All(service => !ModPolicies.FeatureEnabled(service)), "A DE service gate is missing.");
        Check(ModPolicies.FeatureEnabled("campaign"), "An unrelated feature was disabled.");
        Check(catalog.ItemCombatSubtypes.Count == 5 && catalog.ItemTacticSubtypes.Count == 0,
            "DE combat classification patches are incomplete.");
        // Nine restored DE default-moveset moves (unarmed_moves.lua); archive equality: Tools/TestDE128UnarmedMoves.ps1.
        Check(new[] { "front_jump_scissors_kick", "axe_kick_old", "wall_run_up", "air_punch", "throw_leg_push", "throw_leg_push_v",
                "standup_after_leg_fall", "throw_leg_push_profile", "throw_leg_push_v_profile" }
            .All(id => catalog.Moves.Any(move => move.Id.ToString() == "de128:moves/" + id)),
            "DE default-moveset additions are missing.");
        var equivalents = new Dictionary<string, string> { ["front_jump_scissors_kick"] = "FrontJumpKick",
            ["axe_kick_old"] = "FrontKick", ["wall_run_up"] = "DoubleJumpKick", ["air_punch"] = "TwoFootJumpKick" };
        Check(equivalents.All(entry => catalog.Moves.Any(move => move.Id.ToString() == "de128:moves/" + entry.Key &&
            move.TacticEquivalent == entry.Value)), "Restored default moves lost their AI table equivalents.");
        var frontKick = catalog.MoveCombatPatches.SingleOrDefault(patch => patch.MoveName == "FrontKick");
        var backKick = catalog.MoveCombatPatches.SingleOrDefault(patch => patch.MoveName == "BackKick");
        Check(frontKick?.AddInterval?.Name == "SemiUninterrupt" && frontKick.AddInterval.Start == 0 && frontKick.AddInterval.End == 2 &&
            frontKick.IntervalStart?.Name == "Uninterrupt" && frontKick.IntervalStart.Expected == 0 && frontKick.IntervalStart.Value == 3 &&
            backKick?.IntervalEnd?.Name == "SemiUninterrupt" && backKick.IntervalEnd.Expected == 4 && backKick.IntervalEnd.Value == 6 &&
            backKick.IntervalStart?.Name == "Uninterrupt" && backKick.IntervalStart.Expected == 5 && backKick.IntervalStart.Value == 7,
            "Double-kick starters lost their cancel windows.");
        Check(catalog.Moves.Count == 61 && catalog.MoveItemLockExtensions.Count == 10 && catalog.MoveCombatPatches.Count == 37 &&
            catalog.MoveCombatPatches.Count(patch => patch.Disable) == 15,
            "Archived move registrations, boss ability replacements or lock extensions are incomplete.");
        Check(catalog.Tactics.Count == 21 && catalog.Tactics.Any(tactic => tactic.RuntimeName == "de128:tactics/wasp_fly" && tactic.CoreTemplate == "Aggressive") &&
            catalog.TryGetFight(DefinitionId.Parse("de128:fights/uw_survival_demon_1"), out var waspFight) &&
            catalog.TryGetWarrior(waspFight.Warriors[3], out var waspWarrior) &&
            waspWarrior.Tactic == "de128:tactics/wasp_fly",
            "Wasp's Underworld fighter lost its Fly-aware Aggressive tactic.");
        Check(catalog.Tactics.Any(tactic => tactic.RuntimeName == "de128:tactics/butcher_earthquake" && tactic.CoreTemplate == "Aggressive") &&
            catalog.TryGetFight(DefinitionId.Parse("de128:fights/uw_survival_demon_1"), out var butcherFight) &&
            catalog.TryGetWarrior(butcherFight.Warriors[2], out var butcherWarrior) &&
            butcherWarrior.Tactic == "de128:tactics/butcher_earthquake",
            "Butcher's Underworld fighter lost its Earthquake-aware Aggressive tactic.");
        Check(catalog.Tactics.Any(tactic => tactic.RuntimeName == "de128:tactics/hermit_storm" && tactic.CoreTemplate == "Aggressive") &&
            catalog.TryGetFight(DefinitionId.Parse("de128:fights/uw_survival_demon_1"), out var hermitFight) &&
            catalog.TryGetWarrior(hermitFight.Warriors[1], out var hermitWarrior) &&
            hermitWarrior.Tactic == "de128:tactics/hermit_storm",
            "Hermit's Underworld fighter lost its Storm-aware Aggressive tactic.");
        Check(catalog.Tactics.Any(tactic => tactic.RuntimeName == "de128:tactics/war_whirl" && tactic.CoreTemplate == "Aggressive") &&
            catalog.TryGetFight(DefinitionId.Parse("de128:fights/uw_boss_9_1"), out var warFight) &&
            catalog.TryGetWarrior(warFight.Warriors[0], out var warWarrior) &&
            warWarrior.Tactic == "de128:tactics/war_whirl" &&
            catalog.TryGetFight(DefinitionId.Parse("de128:fights/uw_boss_9_hardmode_1"), out var warPowerFight) &&
            catalog.TryGetWarrior(warPowerFight.Warriors[0], out var warPowerWarrior) &&
            warPowerWarrior.Tactic == "de128:tactics/war_whirl",
            "War's normal or Power Mode fighter lost its Whirl-aware Aggressive tactic.");
        foreach (var (fightId, tacticName) in new[] {
            ("uw_boss_7_1", "hoaxen_tentacles"),
            ("uw_boss_7_hardmode_1", "hoaxen_tentacles"),
            ("uw_boss_14_1", "hunter_fly"),
            ("uw_boss_14_hardmode_1", "hunter_fly_power"),
            ("uw_boss_berstuuk_1", "berstuuk_root_potion"),
            ("uw_boss_berstuuk_hardmode_1", "berstuuk_root_potion"),
            ("uw_boss_6_1", "arkhos_rat_wave"),
            ("uw_boss_6_hardmode_1", "arkhos_rat_wave"),
            ("uw_boss_10_1", "tenebris_fear_ray"),
            ("uw_boss_10_hardmode_1", "tenebris_fear_ray"),
        })
        {
            string runtimeTactic = "de128:tactics/" + tacticName;
            Check(catalog.Tactics.Any(tactic => tactic.RuntimeName == runtimeTactic && tactic.CoreTemplate == "Aggressive") &&
                catalog.TryGetFight(DefinitionId.Parse("de128:fights/" + fightId), out var abilityFight) &&
                catalog.TryGetWarrior(abilityFight.Warriors[0], out var abilityWarrior) &&
                abilityWarrior.Tactic == runtimeTactic,
                "Archived raid ability tactic is absent from " + fightId + ".");
        }
        var quakePlayer = catalog.Moves.Single(move => move.Id.LocalId == "butcher_earthquake_player");
        var quakeStart = catalog.Moves.Single(move => move.Id.LocalId == "butcher_earthquake_start");
        var quakeSpawn = quakePlayer.Graph.Presentation.Actions.Single(action => action.Kind == "create_projectile").Projectile;
        Check(quakePlayer.Graph.Presentation.TacticConditions.Count == 2 &&
            quakeSpawn.Item == CoreContentImporter.MagicId("MAGIC_BUTCHER_EARTHQUAKE") &&
            quakeSpawn.StartMove == quakeStart.Id && quakeSpawn.CopyParentType == null &&
            quakeStart.Intervals.Single().Attack.Hit == "Earthquake" &&
            quakeStart.Intervals.Single().Attack.Options.IgnoresBlock,
            "Butcher's authored caster, hidden-item projectile or native hit graph is incomplete.");
        var stormPlayer = catalog.Moves.Single(move => move.Id.LocalId == "hermit_storm_player");
        var stormIdle = catalog.Moves.Single(move => move.Id.LocalId == "hermit_storm_idle");
        var stormWin = catalog.Moves.Single(move => move.Id.LocalId == "hermit_storm_win");
        Check(stormPlayer.Graph.Presentation.TacticConditions.Count == 3 &&
            stormPlayer.Intervals.Count(interval => interval.Attack != null && interval.Attack.Edges.Count == 10) == 3 &&
            stormPlayer.Graph.Presentation.Actions.Single(action => action.Kind == "create_projectile").Frame == 20 &&
            stormPlayer.Graph.Presentation.Actions.Single(action => action.Effect != null).Effect.OnBackground &&
            stormIdle.Graph.Presentation.Actions.Count(action => action.Kind == "create_projectile" &&
                action.Projectile.Item == CoreContentImporter.MagicId("HERMIT_STORM")) == 2 &&
            stormWin.Conditions.Any(condition => condition.Kind == ModMoveConditionKind.RoundResult && condition.Name == "Victory"),
            "Hermit's archived caster, twin storm continuation or victory condition is incomplete.");
        var warWhirl = catalog.Moves.Single(move => move.Id.LocalId == "war_whirl_player");
        Check(warWhirl.Graph.Presentation.TacticConditions.Count == 2 &&
            warWhirl.Intervals.Single(interval => interval.Attack != null).Attack.Edges.Count == 8 &&
            warWhirl.Intervals.Single(interval => interval.Attack != null).Attack.Options.IgnoresBlock &&
            warWhirl.Graph.Presentation.Actions.Count(action => action.Kind == "effect") == 3 &&
            warWhirl.Graph.Presentation.Actions.Count(action => action.Kind == "stop_sound" &&
                action.StopSoundName == "snd_blade_fury") == 2,
            "War's archived Whirl combat graph, effect schedule or sound cleanup is incomplete.");
        var field = catalog.Moves.Single(move => move.Id.LocalId == "gatekeeper_power_field");
        var surge = catalog.Moves.Single(move => move.Id.LocalId == "gatekeeper_power_surge");
        var fieldSpawn = field.Graph.Presentation.Actions.Single(action => action.Kind == "create_projectile").Projectile;
        Check(fieldSpawn.StartMove == surge.Id && fieldSpawn.Item == CoreContentImporter.MagicId("MAGIC_FIRE_AURA") &&
            field.Graph.Presentation.Actions.Single(action => action.Effect != null).Effect.Attach?.RootPoint == "MacroBodyGatekeeper-Node975" &&
            surge.Graph.Presentation.Actions.Count(action => action.Effect?.Attach?.Player == "Parent") == 2 &&
            surge.Intervals.Single(interval => interval.Attack != null).Attack.Damage == 0.3 &&
            surge.Intervals.Single(interval => interval.Attack != null).Attack.Hit == "ElectrocutionPowerfield" &&
            catalog.TryGetFight(DefinitionId.Parse("de128:fights/uw_boss_11_1"), out var gatekeeperFight) &&
            catalog.TryGetWarrior(gatekeeperFight.Warriors[0], out var gatekeeperWarrior) &&
            gatekeeperWarrior.Tactic == "de128:tactics/gatekeeper_power_field" &&
            catalog.TryGetFight(DefinitionId.Parse("de128:fights/uw_boss_11_hardmode_1"), out var gatekeeperPowerFight) &&
            catalog.TryGetWarrior(gatekeeperPowerFight.Warriors[0], out var gatekeeperPowerWarrior) &&
            gatekeeperPowerWarrior.Tactic == "de128:tactics/gatekeeper_power_field",
            "Gatekeeper's archived cast, attached child field or both Underworld tactics are incomplete.");
        var grasp = catalog.Moves.Single(move => move.Id.LocalId == "blackness_grasp_player");
        var handStart = catalog.Moves.Single(move => move.Id.LocalId == "blackness_grasp_hand_start");
        var handAttack = catalog.Moves.Single(move => move.Id.LocalId == "blackness_grasp_hand_attack");
        var handSpawn = grasp.Graph.Presentation.Actions.Single(action => action.Kind == "create_projectile");
        var handTransition = grasp.Graph.Presentation.Actions.Single(action => action.Kind == "play_animation");
        Check(grasp.Priority == 200 && handSpawn.Frame == 9 && handSpawn.Projectile.Name == "BlackHand" &&
            handSpawn.Projectile.StartMove == handStart.Id &&
            handSpawn.Projectile.Item == CoreContentImporter.MagicId("MAGIC_ACID_CLOUD") &&
            handTransition.Frame == 17 && handTransition.PlayMove == handAttack.Id &&
            handTransition.PlayPlayer == "Child" && handTransition.ChildName == "BlackHand" &&
            handStart.Graph.Presentation.Actions.Any(action => action.Effect?.CoreSequence == "mgc_effect_black_hand") &&
            handStart.Graph.Locks.Any(condition => condition.Kind == ModMoveConditionKind.Item &&
                condition.Name == "MAGIC_ACID_CLOUD") &&
            handAttack.Graph.Locks.All(condition => condition.Kind != ModMoveConditionKind.Perk) &&
            handAttack.Intervals.Single(interval => interval.Attack != null).Attack.Damage == 0.4 &&
            handAttack.Graph.Presentation.Actions.Single(action => action.Kind == "delete_actor").Frame == 12 &&
            Math.Abs(handAttack.Graph.Presentation.Velocity.Ax + 0.4) < 0.00001 &&
            catalog.TryGetFight(DefinitionId.Parse("de128:fights/uw_boss_13_1"), out var blacknessFight) &&
            catalog.TryGetWarrior(blacknessFight.Warriors[0], out var blacknessWarrior) &&
            blacknessWarrior.Tactic == "de128:tactics/blackness_grasp" &&
            catalog.TryGetFight(DefinitionId.Parse("de128:fights/uw_boss_13_hardmode_1"), out var blacknessPowerFight) &&
            catalog.TryGetWarrior(blacknessPowerFight.Warriors[0], out var blacknessPowerWarrior) &&
            blacknessPowerWarrior.Tactic == "de128:tactics/blackness_grasp_power",
            "Blackness's caster, timed child transition, attacking hand or two tactics are incomplete.");
        var saturn = catalog.MoveCombatPatches.Single(patch => patch.MoveName == "SaturnBlasterAbilityPlayer");
        Check(saturn.Input?.Expected.Key == "Super" && saturn.Input.Value.Key == "RaidCharge" &&
            saturn.Priority?.Expected == 1000 && saturn.Priority.Value == 200 && !saturn.Disable &&
            catalog.TryGetFight(DefinitionId.Parse("de128:fights/uw_boss_12_1"), out var saturnFight) &&
            catalog.TryGetWarrior(saturnFight.Warriors[0], out var saturnWarrior) &&
            saturnWarrior.Tactic == "de128:tactics/saturn_blaster" &&
            catalog.TryGetFight(DefinitionId.Parse("de128:fights/uw_boss_12_hardmode_1"), out var saturnPowerFight) &&
            catalog.TryGetWarrior(saturnPowerFight.Warriors[0], out var saturnPowerWarrior) &&
            saturnPowerWarrior.Tactic == "de128:tactics/saturn_blaster_power",
            "Saturn's guarded native Blaster patch or Underworld encounter is incomplete.");
        var dandy = catalog.MoveCombatPatches.Single(patch => patch.MoveName == "LightingChainPlayer");
        Check(dandy.Input?.Expected.Key == "Super" && dandy.Input.Value.Key == "RaidCharge" &&
            dandy.Priority?.Expected == 9000 && dandy.Priority.Value == 200 && !dandy.Disable &&
            dandy.IntervalStart?.Name == "Uninterrupt" && dandy.IntervalStart.Expected == 9 &&
            dandy.IntervalStart.Value == 0 &&
            catalog.TryGetFight(DefinitionId.Parse("de128:fights/uw_boss_dandy_1"), out var dandyFight) &&
            catalog.TryGetWarrior(dandyFight.Warriors[0], out var dandyWarrior) &&
            dandyWarrior.Tactic == "de128:tactics/dandy_lightning_chain" &&
            dandyWarrior.PerkLoadout.Any(row => row.Perk.ToString() == "core:perks/perk_lighting_chain" && row.Frames == 600) &&
            catalog.TryGetFight(DefinitionId.Parse("de128:fights/uw_boss_dandy_hardmode_1"), out var dandyPowerFight) &&
            catalog.TryGetWarrior(dandyPowerFight.Warriors[0], out var dandyPowerWarrior) &&
            dandyPowerWarrior.Tactic == "de128:tactics/dandy_lightning_chain_power" &&
            dandyPowerWarrior.PerkLoadout.Any(row => row.Perk.ToString() == "core:perks/perk_lighting_chain" && row.Frames == 500),
            "Dandy's guarded native Lightning Chain patch or Underworld timing is incomplete.");
        foreach (var phase in new[] { "LightingChainStart", "LightingChain50", "LightingChain150",
            "LightingChain300", "LightingChain400" })
            Check(catalog.MoveCombatPatches.Any(patch => patch.MoveName == phase &&
                patch.Conditions.Count == 1 && patch.Conditions[0].Kind == ModMoveConditionKind.ActorName &&
                patch.Conditions[0].Name == "LightningChain") &&
                catalog.MovePerkLockRemovals.Any(removal => removal.MoveName == phase &&
                    removal.RuntimePerkName == "PERK_LIGHTING_CHAIN"),
                "Dandy's spawned chain phase lacks its actor-name guard: " + phase);
        var slash = catalog.Moves.Single(move => move.Id.LocalId == "chinese_swords_super_slash");
        Check(slash.Graph.Presentation.Profile.DisplayName.HasValue &&
            catalog.TryGetLocalization(slash.Graph.Presentation.Profile.DisplayName.Value, out var moveTitle) &&
            moveTitle.GetOrEnglish("eng") == "Super Slash", "Chinese swords profile title is not localized.");
        Check(slash.Animation.ToString() == "de128:animations/chinese_swords_super_slash_old" &&
            catalog.ItemCombatSubtypes.Any(patch => patch.Item == CoreContentImporter.WeaponId("WEAPON_CHNY21_JIAN") && patch.Subtype == "ChineseSwords"),
            "Chinese swords binary/subtype registration changed.");
        Check(catalog.ItemAvailabilityPolicies.Count() == 244 && catalog.ItemInitialProfiles.Count == 221 &&
            catalog.ItemShopPrices.Count == 99 && catalog.ItemPresentations.Count == 24 &&
            catalog.ItemAvailabilityPolicies.Where(policy => policy.Item.Namespace.Value == "core").All(policy => policy.Owner.Value == "de128" &&
                policy.Visibility == ModItemVisibility.ForceVisible && policy.MinimumLevel >= 1),
            "DE shop policies are incomplete after registration/rebuild/conflict.");
        Check(catalog.ItemPresentations.Any(presentation =>
                presentation.Item == CoreContentImporter.MagicId("MAGIC_VERTICAL_TRIGGER") &&
                presentation.Model.ToString() == "de128:models/underworld/mdl_vertical_trigger") &&
            catalog.ItemPresentations.Any(presentation =>
                presentation.Item == CoreContentImporter.MagicId("SMALL_COLLISION_BOX") &&
                presentation.Model.ToString() == "de128:models/underworld/mdl_small_collision_box"),
            "Berstuuk's hidden Root Potion collision geometry is missing.");
        Check(ModPolicies.DeliverySeconds("shop", 120) == 120, "An unrelated timer was modified.");
        Check(catalog.TryGetItem(Sword, out var definition) && definition is WeaponDefinition,
            "The actual package did not register Desolator.");
        var weapon = (WeaponDefinition)definition;
        Check(weapon.SubType == "TitanGiantSword" && weapon.Progression == ItemProgressionKind.Vanilla,
            "Desolator lost its move family or canonical progression profile.");
        Check(weapon.Icon.ToString() == "core:ui/items/weapon17.img_weapon_boss_giant_sword" &&
            weapon.Model.ToString() == "core:gamedata/models/mdl_weapon_giant_sword", "Desolator art IDs changed.");
        Check(catalog.Weapons.Count(item => !item.IsCore) == 11 && catalog.ShopListings.Count == 23,
            "DE128 restored weapon/listing inventory is incomplete.");
        Check(catalog.ItemInnatePerks.Count == 2 && catalog.ItemInnatePerks[0].Item == Sword &&
            catalog.ItemInnatePerks[0].Entries.Select(entry => entry.Perk).SequenceEqual(new[] {
                CoreContentImporter.PerkId("PERK_TITAN"), CoreContentImporter.PerkId("PERK_ANTI_SHOCK") }),
            "Desolator innate loadout changed.");
        Check(catalog.TryGetLocalization(weapon.DisplayName, out var title) &&
            title.Id.Namespace.Value == "de128" && title.GetOrEnglish("eng") == "Titan's Desolator",
            "Desolator's mod-owned English title is missing.");
        Check(catalog.Rewards.Count(value => !value.Id.LocalId.StartsWith("sensei_act_") && !value.Id.LocalId.StartsWith("uw_") &&
            !value.Id.LocalId.StartsWith("challenger_")) == 1 &&
            catalog.Rewards.Count(value => value.Id.LocalId.StartsWith("sensei_act_")) == 57 && catalog.TryGetReward(
            DefinitionId.Parse("de128:rewards/titans_desolator"), out var reward) &&
            reward.Items.Count == 5 && reward.Items[0].Item == Sword &&
            reward.Items.Skip(1).Select(value => value.Item.ToString()).SequenceEqual(new[] {
                "de128:items/armor/titans_form", "de128:items/helm/titans_helm",
                "de128:items/ranged/titans_harpoon", "de128:items/magic/titans_mind_throw" }) &&
            reward.Items.All(value => value.UsesConfiguration) &&
            reward.Choices.Count == 0 && reward.Gems == 0 && catalog.ItemDefaultEnchantments.Count == 240 &&
            !catalog.ItemDefaultEnchantments.Any(value => reward.Items.Any(grant => grant.Item == value.Item)),
            "The five Titan grants must be configured without changing equipment defaults or currencies.");
        Check(catalog.TryGetFight(DefinitionId.Parse("core:fights/zone_7/c3_boss_titan_eclipsemode/6"), out var titan) &&
            titan.RewardDrops.Count == 1 && catalog.Fights.Count(fight => fight.RewardDrops.Count != 0) == 1,
            "DE128 must patch only the final Eclipse Titan reward.");
        var drop = titan.RewardDrops[0];
        Check(drop.ResultIndex == 1 && drop.Mode == ModRuleMode.Eclipse && !drop.MinimumLevel.HasValue &&
            !drop.MaximumLevel.HasValue && drop.Reward.Id.ToString() == "de128:rewards/titans_desolator",
            "Desolator reward changed its winning slot, mode or level gate.");
        // The only mod-owned opponents, fights and rules are the Sensei story's, the Underworld's
        // and the Challengers'.
        bool Owned(string id) => id.StartsWith("sensei_") || id.StartsWith("uw_") || id.StartsWith("challenger_");
        Check(catalog.Modes.Count == 0 && catalog.Quests.Count == 1 &&
            catalog.Quests[0].Id.ToString() == "de128:quests/dojo_changer_map_button" &&
            catalog.Quests[0].Place == ModQuestActionPlace.Map &&
            catalog.Quests[0].Events.SequenceEqual(new[] { ModQuestEventKind.Session }) &&
            catalog.Quests[0].Actions.Count == 1 &&
            catalog.Quests[0].Actions[0].Kind == ModQuestActionKind.HideMapButton &&
            catalog.Quests[0].Actions[0].Name == "de128.dojo_changer" &&
            catalog.DojoButtons.Count == 1 && catalog.DojoButtons[0].Name == "de128.dojo_changer" &&
            catalog.Warriors.Count(value => value.Id.LocalId.StartsWith("sensei_")) == 34 && catalog.Warriors.All(value => Owned(value.Id.LocalId)) &&
            catalog.Fights.Count(fight => !fight.IsCore && fight.Id.LocalId.StartsWith("sensei_act_")) == 23 &&
            catalog.Fights.Where(fight => !fight.IsCore).All(fight => Owned(fight.Id.LocalId)) &&
            catalog.FightRules.All(rule => Owned(rule.Id.LocalId)),
            "DE128 must register only its dojo button and saved-map-button cleanup alongside the existing story content.");
        // Active Underworld: eight Underworld pages generated from the archived raid stages.
        var underworldZones = catalog.Zones.Where(zone => !zone.IsCore && zone.Underworld).ToArray();
        var underworldBattles = catalog.Battles.Where(battle => underworldZones.Any(zone => zone.Id == battle.Zone)).ToArray();
        Check(underworldZones.Length == 8 && underworldBattles.Length == 76 &&
            underworldBattles.Count(battle => battle.PowerMode == ModPowerMode.Normal) == 36 &&
            underworldBattles.Count(battle => battle.PowerMode == ModPowerMode.Power) == 36 &&
            underworldBattles.Count(battle => battle.PowerMode == ModPowerMode.Always) == 4 &&
            underworldBattles.Count(battle => battle.Icons != null) == 26,
            "Underworld pages, Power Mode twins or shipped map buttons differ from the archive.");
        var underworldRewards = catalog.Rewards.Where(reward => reward.Id.LocalId.StartsWith("uw_")).ToArray();
        Check(catalog.Fights.Count(fight => fight.Id.LocalId.StartsWith("uw_")) == 76 && underworldRewards.Length == 180 &&
            underworldRewards.Sum(reward => reward.Currencies.Count) == 108 &&
            catalog.WarriorTemplates.Count(template => template.Body != null && template.Id.Namespace.Value == "de128" &&
                template.Id.LocalId.StartsWith("uw_")) == 66 &&
            catalog.Warriors.Count(value => value.Id.LocalId.StartsWith("uw_")) == 104,
            "Underworld fights, rewards, forge drops, templates or opponents differ from the archive.");
        // Active Sensei story: synthesized guards (Default + voice) and the restored Sphere1.
        var guards = catalog.Warriors.Where(value => value.Id.LocalId.Contains("_guard_")).ToArray();
        Check(guards.Length == 22 && guards.All(value => value.HasTemplate &&
            value.Template.ToString() == "core:warrior-templates/default" && (value.Voice == "Female" || value.Voice == "Male")),
            "Sensei guards do not use the synthesized Default+voice templates.");
        Check(guards.Count(value => value.Voice == "Female") == 10 &&
            guards.Where(value => value.Id.LocalId.StartsWith("sensei_act_6_")).All(value =>
                value.Voice == "Male" && value.Items.Any(item => item.ToString() == "de128:items/magic/minor_charge_of_darkness")),
            "Sensei guard voices or the prince's restored Sphere1 differ.");
        Check(catalog.Battles.Count(value => !value.IsCore && value.Id.LocalId.StartsWith("sensei_act_")) == 12 &&
            catalog.Battles.Where(value => !value.IsCore && value.Preview.StartsWith("de128:") &&
                !value.Id.LocalId.StartsWith("challenger_")).Count() == 10,
            "Sensei battles or shipped previews missing from the active package.");
        Check(!catalog.Localizations.Any(value => value.Id.Namespace.Value == "de128" &&
            (value.Id.LocalId.StartsWith("ascension") || value.Id.LocalId == "zones/ascension")),
            "Disabled Ascension registered live localization.");
        var mindInnate=catalog.ItemInnatePerks.Single(value=>value.Item.ToString()=="de128:items/magic/mind_throw");
        Check(mindInnate.Entries.Count==1 && mindInnate.Entries[0].Perk.ToString()=="de128:perks/mind_throw",
            "MindThrow lost its innate Lua behavior");
        Check(catalog.Perks.Count(perk => !perk.IsCore) == 3 && catalog.Behaviors.Count(value => value.Id.LocalId != "sensei_raid_charge") == 3 && catalog.Behaviors.Count == 4,
            "DE combat perk definitions are missing or unexpected behaviors were registered.");
        foreach (int level in new[] { 4, 8, 11, 14, 17 })
            Check(catalog.TryGetProgressionBranch(level, out var branch) && branch.Entries.Count == 2,
                "XML-evidenced DE perk branch is missing at " + level);
    }

    private static void CheckTeleportation(ModContentCatalog catalog)
    {
        var moves = catalog.Moves.Where(move => move.ReplacementTarget != null).ToArray();
        Check(moves.Length == 2 && moves.Select(move => move.ReplacementTarget).OrderBy(name => name).SequenceEqual(
            new[] { "WidowTeleportationEnd", "WidowTeleportationStart" }),
            "The two native Teleportation phases were not replaced together.");
        var start = moves.Single(move => move.ReplacementTarget == "WidowTeleportationStart");
        var finish = moves.Single(move => move.ReplacementTarget == "WidowTeleportationEnd");
        Check(start.ExpectedNativeFile == "widow_teleportation_start.bytes" &&
            start.Animation.ToString() == "de128:animations/widow_teleportation_start" &&
            finish.ExpectedNativeFile == "widow_teleportation_end.bytes" &&
            finish.Animation.ToString() == "de128:animations/widow_teleportation_end" &&
            start.CoreTemplates.SequenceEqual(new[] { "1key", "BossAbility", "Controlled", "SoundStrike" }) &&
            finish.CoreTemplates.SequenceEqual(new[] { "ChangeDirection" }) &&
            start.Templates.Count == 0 && finish.Templates.Count == 0,
            "Teleportation binary provenance or native template graph differs.");
        Check(start.Conditions.First().Kind == ModMoveConditionKind.Keys &&
            start.Conditions.First().Keys.Single().Key == "RaidCharge" &&
            start.Conditions.Count(condition => condition.Kind == ModMoveConditionKind.CurrentAnimation &&
                condition.Player == "Enemy" && condition.Not) == 19 &&
            new[] { "150", "200", "300", "370" }.All(range => start.Conditions.Any(condition =>
                condition.Kind == ModMoveConditionKind.CurrentAnimation && condition.Player == "Enemy" && condition.Not &&
                condition.Name == "de128:moves/wasp_fly_" + range)) &&
            start.Conditions.Count(condition => condition.Kind == ModMoveConditionKind.Direction) == 1 &&
            start.Conditions.All(condition => condition.Kind != ModMoveConditionKind.Distance) &&
            start.Graph.Locks.Count == 1 && start.Graph.Locks[0].Kind == ModMoveConditionKind.Perk &&
            finish.Conditions.Count(condition => condition.Kind == ModMoveConditionKind.CurrentAnimation &&
                condition.Player == "Enemy" && condition.Not) == 19 &&
            finish.Graph.Align.ShiftModelNode == "NPivot" && finish.Graph.Align.Position.ShiftX == 100,
            "Teleportation input, enemy safety gates or alignment differs from the archive.");
        var attack = finish.Intervals.Single(interval => interval.Attack != null).Attack;
        Check(attack.Edges.SequenceEqual(new[] { "EForearm_1", "EHand_1", "EFingers_1", "EArm_1", "EArm_2",
            "EForearm_2", "EHand_2", "EFingers_2", "EChest" }) && Math.Abs(attack.Damage - 0.28) < 0.0001 &&
            attack.Options.IgnoresBlock && attack.Hit == "High" &&
            attack.DamageTerms.Count == 2 && attack.DamageTerms[1].Type == "UnarmedDamage" &&
            Math.Abs(attack.DamageTerms[1].Shift + 10) < 0.0001,
            "Teleportation's finishing strike lost its owner damage and body edges.");
        var fighters = catalog.Warriors.Where(warrior => warrior.PerkLoadout.Any(perk =>
            string.Equals(perk.Perk.ToString(), "core:perks/PERK_TELEPORTATION", StringComparison.OrdinalIgnoreCase))).ToArray();
        Check(fighters.Length == 13 && fighters.All(warrior => warrior.Tactic.StartsWith(
            "de128:tactics/widow_teleportation", StringComparison.Ordinal)) &&
            fighters.All(warrior => warrior.PerkLoadout.Single(perk =>
                string.Equals(perk.Perk.ToString(), "core:perks/PERK_TELEPORTATION", StringComparison.OrdinalIgnoreCase)).Frames.HasValue),
            "One of thirteen Teleportation fighters lost its reviewed cooldown or tactic.");
    }

    private static void CheckMoveReplacementContract(string fixture, ModContentCatalog enabled, string repository)
    {
        var peer = Peer(fixture, "fixture.move-replacement", "content.patch", "");
        string assetDirectory = Path.Combine(peer.RootPath, "assets", "animations");
        Directory.CreateDirectory(assetDirectory);
        File.Copy(Path.Combine(repository, "Mods", "de128", "assets", "animations", "widow_teleportation_start.bytes"),
            Path.Combine(assetDirectory, "replacement.bytes"));
        string Declaration(string target, string expected, int priority = 110) =>
            "sf2.moves.replace { id='replacement', target='" + target + "', expected_file='" + expected +
            "', animation=sf2.assets.binary('animations/replacement'), priority=" + priority + " }";
        File.WriteAllText(Path.Combine(peer.RootPath, "scripts", "main.lua"),
            "local sf2=require('sf2')\n" + Declaration("WidowTeleportationStart", "old.bytes"), Utf8);
        var conflict = new ModContentCatalog();
        ExpectFailure(peer, enabled, "Native move replacement already owned: WidowTeleportationStart");
        Check(enabled.Moves.Count(move => move.ReplacementTarget == "WidowTeleportationStart") == 1,
            "Conflicting native replacement changed the installed DE move.");
        void SetScript(string body) => File.WriteAllText(Path.Combine(peer.RootPath, "scripts", "main.lua"),
            "local sf2=require('sf2')\n" + body, Utf8);
        SetScript(Declaration("TestMove", "old.bytes") + "\n" + Declaration("TestMove", "old.bytes"));
        ExpectFailure(peer, conflict, "Duplicate native move replacement: TestMove");
        Check(conflict.Moves.Count == 0, "Duplicate replacement leaked a partial registration.");
        string Hash(string target, string expected, int priority)
        {
            SetScript(Declaration(target, expected, priority));
            var content = new ModContentCatalog(); Load(peer, content);
            return ModSaveData.ComputeContentSetFingerprint(new[] { peer }, content);
        }
        Check(new[] { Hash("TestMove", "old.bytes", 110), Hash("OtherMove", "old.bytes", 110),
            Hash("TestMove", "other.bytes", 110), Hash("TestMove", "old.bytes", 111) }.Distinct().Count() == 4,
            "Native replacement target, filename guard or definition is missing from the compatibility fingerprint.");
        SetScript(Declaration("TestMove", "old.xml"));
        ExpectFailure(peer, new ModContentCatalog(), "exact native .bytes filename");
    }

    private static void CheckSharedMovePatches(ModDescriptor mod, ModContentCatalog catalog, string fixture, string repository)
    {
        var archive = ReadXml(Path.Combine(repository,"Assets/DExml/animations/moves.xml"));
        var vanilla = ReadXml(Path.Combine(repository,"Assets/vanillaXml/animations/moves.xml"));
        // The double-kick starter windows go past the archive on purpose; their
        // exact values are checked with the restored default moves.
        foreach (var patch in catalog.MoveCombatPatches.Where(patch => patch.MoveName != "FrontKick" && patch.MoveName != "BackKick"))
        {
            var oldMove = (XmlElement)vanilla.SelectSingleNode("//Moves/Move[@Name='"+patch.MoveName+"']");
            var newMove = (XmlElement)archive.SelectSingleNode("//Moves/Move[@Name='"+patch.MoveName+"']");
            Check(oldMove != null && newMove != null && patch.Owner == mod.Id,"Move patch source/owner missing.");
            if (patch.IntervalEnd != null)
            {
                var value=patch.IntervalEnd;
                Check(oldMove.SelectSingleNode("Intervals/Interval[@Name='"+value.Name+"']").Attributes["End"].Value == value.Expected.ToString() &&
                    newMove.SelectSingleNode("Intervals/Interval[@Name='"+value.Name+"']").Attributes["End"].Value == value.Value.ToString(),"Move end patch differs from archive.");
            }
            if (patch.IntervalStart != null)
            {
                var value = patch.IntervalStart;
                var oldInterval = oldMove.SelectSingleNode("Intervals/Interval[@Name='"+value.Name+"']");
                var newInterval = newMove.SelectSingleNode("Intervals/Interval[@Name='"+value.Name+"']");
                Check((oldInterval.Attributes["Start"]?.Value ?? "0") == value.Expected.ToString() &&
                    (newInterval.Attributes["Start"]?.Value ?? "0") == value.Value.ToString(),
                    "Move start patch differs from archive.");
            }
            if (patch.Hit != null)
                Check(oldMove.SelectSingleNode("Intervals/Interval/Hit").Attributes["Name"].Value == patch.Hit.Expected &&
                    newMove.SelectSingleNode("Intervals/Interval/Hit").Attributes["Name"].Value == patch.Hit.Value,"Move reaction patch differs from archive.");
            if (patch.SoundFrame != null)
            {
                var value=patch.SoundFrame;
                Check(oldMove.SelectSingleNode("Actions/Sound[@Name='"+value.Name+"']").Attributes["Frame"].Value == value.Expected.ToString() &&
                    newMove.SelectSingleNode("Actions/RandomSound[Sound/@Name='"+value.Name+"']").Attributes["Frame"].Value == value.Value.ToString(),"Sound frame patch differs from archive.");
            }
            if (patch.Animation != null)
                Check(oldMove.GetAttribute("FileName") == patch.Animation.Expected &&
                    newMove.GetAttribute("FileName") == Path.GetFileName(patch.Animation.Value.Path) + ".bytes",
                    "Move clip patch differs from archive.");
            if (patch.RemoveInterval != null)
            {
                var value = patch.RemoveInterval;
                var oldInterval = (XmlElement)oldMove.SelectSingleNode("Intervals/Interval[@Name='" + value.Name + "']");
                Check(oldInterval != null && oldInterval.GetAttribute("Type") == value.Type &&
                    (oldInterval.GetAttribute("Start") == "" ? "0" : oldInterval.GetAttribute("Start")) == value.Start.ToString() &&
                    oldInterval.GetAttribute("End") == value.End.ToString() &&
                    newMove.SelectSingleNode("Intervals/Interval[@Name='" + value.Name + "']") == null,
                    "Removed native interval differs from archive.");
            }
            foreach (var condition in patch.Conditions)
                if (condition.Kind == ModMoveConditionKind.ActorName)
                    Check(patch.MoveName.StartsWith("LightingChain",StringComparison.Ordinal) &&
                        condition.Name == "LightningChain" && !condition.Not &&
                        newMove.SelectSingleNode("Locks/Perk[@Name='PERK_LIGHTING_CHAIN']") != null,
                        "Spawned chain actor guard is not supported by the archived phase.");
                else
                    Check(condition.Kind == ModMoveConditionKind.ModExists && condition.Name == "Stun" && condition.Not &&
                        newMove.SelectSingleNode("Conditions/ModExists[@Name='Stun' and @Not='1']") != null &&
                        oldMove.SelectSingleNode("Conditions/ModExists[@Name='Stun']") == null,"Added native condition differs from archive.");
        }
        var peer = Peer(fixture,"fixture.move-patch-conflict","content.patch",
            "sf2.moves.patch { move='MassBombPlayer', conditions={{mod='Other'}} }");
        var peerFirst=new ModContentCatalog();Load(peer,peerFirst);
        ExpectFailure(mod,peerFirst,"Move combat patch already owned: MassBombPlayer");
        Check(peerFirst.MoveCombatPatches.Count==1 && peerFirst.MoveCombatPatches[0].Owner==peer.Id &&
            peerFirst.TimerPolicies.Count==0 && !peerFirst.Weapons.Any(item=>!item.IsCore),"Conflicting DE registration leaked content.");
        ExpectFailure(peer,catalog,"Move combat patch already owned: MassBombPlayer");CheckDE(catalog);
        int index=0;
        foreach(string body in new[]{"move='Test'", "move='bad:name', hit={expected='High',value='Low'}",
            "move='Test', hit={expected='High',value='High'}", "move='Test', hit={expected='High',value='Unknown'}",
            "move='Test', interval_end={name='Attack',expected=42,value=40}","move='Test', interval_end={name='Uninterrupt',expected=42,value=-1}",
            "move='Test', interval_start={name='Attack',expected=9,value=0}","move='Test', interval_start={name='Uninterrupt',expected=9,value=-1}",
            "move='Test', sound_frame={name='snd',expected=18,value=16.5}","move='Test', sound_frame={name='snd',expected=18,value=100001}",
            "move='Test', sound_frame={name='snd',expected=18,value=math.huge}","move='Test', hit=true",
            "move='Test', conditions={{type='unknown'}}","move='Test', hit={expected='High',value='Low',extra=1}"})
        {
            var invalid=Peer(fixture,"fixture.move-patch-invalid-"+index++,"content.patch","sf2.moves.patch {"+body+"}");
            var failed=new ModContentCatalog();ExpectFailure(invalid,failed,"sf2.moves.patch");
            Check(failed.MoveCombatPatches.Count==0,"Invalid patch leaked registration.");
        }
        var duplicate=Peer(fixture,"fixture.move-patch-duplicate","content.patch",
            "for i=1,2 do sf2.moves.patch { move='Test', hit={expected='High',value='Low'} } end");
        var duplicateCatalog=new ModContentCatalog();ExpectFailure(duplicate,duplicateCatalog,"Duplicate move combat patch");
        Check(duplicateCatalog.MoveCombatPatches.Count==0,"Duplicate patch leaked registration.");
        string Hash(string text)
        {
            File.WriteAllText(Path.Combine(peer.RootPath,"scripts/main.lua"),"local sf2=require('sf2')\n"+text,Utf8);
            var result=new ModContentCatalog();Load(peer,result);
            return ModSaveData.ComputeContentSetFingerprint(new[]{peer},result);
        }
        string[] declarations={"", "sf2.moves.patch {move='Test',hit={expected='High',value='Low'}}",
            "sf2.moves.patch {move='Test',hit={expected='High',value='Middle'}}",
            "sf2.moves.patch {move='Test',interval_end={name='Uninterrupt',expected=42,value=40}}",
            "sf2.moves.patch {move='Test',interval_start={name='Uninterrupt',expected=9,value=0}}",
            "sf2.moves.patch {move='Test',sound_frame={name='snd',expected=18,value=16}}",
            "sf2.moves.patch {move='Test',conditions={{mod='Stun'}}}",
            "sf2.moves.patch {move='Test',conditions={{not_mod='Stun'}}}",
            "sf2.moves.patch {move='Test',remove_interval={name='Evade',type='Invulnerable',start=0,['end']=47}}",
            "sf2.moves.patch {move='Test',remove_interval={name='Evade',type='Invulnerable',start=0,['end']=46}}"};
        Check(declarations.Select(Hash).Distinct().Count()==declarations.Length,"Move patch fields missing from compatibility fingerprint.");
    }

    private static void CheckInitialStats(string fixture)
    {
        string Prefix(string category, string stats) =>
            "local title=sf2.localization.register { id='stats', language='eng', value='Stats' }\n" +
            "sf2.items.register_" + category + " { id='stats', display_name=title, " +
            "icon=sf2.assets.sprite('core:ui/items/weapon17.img_weapon_boss_giant_sword'), " +
            "model=sf2.assets.model('core:gamedata/models/mdl_weapon_giant_sword'), " +
            (category == "ranged" || category == "magic" ? "subtype='Test', " : "") + stats + " }";
        var values = new[] { "weapon_damage=0", "body_defense=17, head_defense=2, unarmed_damage=4",
            "head_defense=1000000", "ranged_damage=30, weapon_damage=7", "magic_damage=42" };
        var categories = new[] { "weapon", "armor", "helm", "ranged", "magic" };
        for (int i = 0; i < categories.Length; i++)
        {
            var peer = Peer(fixture, "fixture.stats-" + categories[i], "content.register", Prefix(categories[i], "initial_stats={" + values[i] + "}"));
            var catalog = new ModContentCatalog(); Load(peer, catalog);
            Check(catalog.TryGetItem(DefinitionId.Parse(peer.Id + ":items/" + categories[i] + "/stats"), out var item) &&
                item.InitialStats != null && item.InitialStats.Values.Count > 0, "Lua initial_stats lost for " + categories[i]);
            bool immutable = false;
            try { ((IDictionary<string, int>)item.InitialStats.Values).Add("MagicDamage", 11); }
            catch (NotSupportedException) { immutable = true; }
            Check(immutable, "Registered initial stats are mutable.");
        }
        int index = 0;
        foreach (string invalid in new[] { "false", "10", "{weapon_damage='3'}", "{weapon_damage=-1}",
            "{weapon_damage=1000001}", "{weapon_damage=1.25}", "{weapon_damage=0/0}", "{weapon_damage=math.huge}",
            "{body_defense=1}", "{unknown=1}", "{3}", "{weapon_damage=true}" })
        {
            var peer = Peer(fixture, "fixture.stats-invalid-" + index++, "content.register", Prefix("weapon", "initial_stats=" + invalid));
            var catalog = new ModContentCatalog(); ExpectFailure(peer, catalog, "initial_stats");
            Check(!catalog.Weapons.Any(item => !item.IsCore), "Invalid initial stats leaked a weapon.");
        }
        var fingerprintPeer = Peer(fixture, "fixture.stats-fingerprint", "content.register", "");
        string Hash(ModEquipmentInitialStats stats)
        {
            var catalog = new ModContentCatalog();
            using (var tx = catalog.BeginRegistration(fingerprintPeer))
            {
                var title = tx.AddLocalization("stats", "eng", "Stats");
                tx.RegisterWeapon("stats", title, default(AssetId), default(AssetId), "Katana", initialStats: stats);
                tx.Commit();
            }
            return ModSaveData.ComputeContentSetFingerprint(new[] { fingerprintPeer }, catalog);
        }
        Check(new[] { Hash(null), Hash(new ModEquipmentInitialStats()), Hash(new ModEquipmentInitialStats(weaponDamage: 0)),
            Hash(new ModEquipmentInitialStats(weaponDamage: 1)) }.Distinct().Count() == 4,
            "Derived, absent, explicit zero and positive stats share compatibility fingerprints.");
    }

    private static void CheckTrialFingerprints(string fixture)
    {
        var owner = Peer(fixture, "fixture.trial-fingerprint", "content.register", "");
        string Hash(Action<ModRegistrationTransaction> register)
        {
            var content = new ModContentCatalog(); ImportCore(content);
            using (var transaction = content.BeginRegistration(owner))
            { register(transaction); transaction.Commit(); }
            return ModSaveData.ComputeContentSetFingerprint(new[] { owner }, content);
        }
        string Hot(int frames, float maximum, string animation) => Hash(transaction =>
            transaction.RegisterHotGroundRule("rule", frames,
                new[] { new ModTrialNodeLimit("NPivot", ModTrialAxis.Y, maximum: maximum) },
                new[] { animation }, ModRuleTarget.Player, ModRuleMode.All, null));
        string original = Hot(420, 30, "Jump");
        Check(original != Hot(480, 30, "Jump"), "Hot-ground timing is absent from the fingerprint.");
        Check(original != Hot(420, 31, "Jump"), "Hot-ground node bounds are absent from the fingerprint.");
        Check(original != Hot(420, 30, "ThrowFall"), "Hot-ground animations are absent from the fingerprint.");
        Check(Hash(transaction => transaction.RegisterRingOutRule("rule", "NPivot", ModTrialAxis.X,
            -600, 600, ModRuleTarget.Player, ModRuleMode.All, null)) !=
            Hash(transaction => transaction.RegisterRingOutRule("rule", "NPivot", ModTrialAxis.X,
            -500, 600, ModRuleTarget.Player, ModRuleMode.All, null)), "Ring-out bounds are absent from the fingerprint.");
        Check(Hash(transaction => transaction.RegisterRegenerationRule("rule", 0.001f, 180, ModRuleTarget.Opponent,
            ModRuleMode.All, null)) != Hash(transaction => transaction.RegisterRegenerationRule("rule", 0.002f, 180,
            ModRuleTarget.Opponent, ModRuleMode.All, null)), "Regeneration rate is absent from the fingerprint.");
        Check(Hash(transaction => transaction.RegisterNoAnimationRule("rule", "Jump", ModRuleMode.All, null)) !=
            Hash(transaction => transaction.RegisterNoAnimationRule("rule", "ThrowFall", ModRuleMode.All, null)),
            "Animation restriction is absent from the fingerprint.");
        Check(Hash(transaction => transaction.RegisterRemoveIntervalRule("rule", ModTrialIntervalType.Block,
            ModRuleTarget.Player, ModRuleMode.All, null)) != Hash(transaction => transaction.RegisterRemoveIntervalRule(
            "rule", ModTrialIntervalType.Attack, ModRuleTarget.Player, ModRuleMode.All, null)),
            "Interval restriction is absent from the fingerprint.");
        var perk = CoreContentImporter.PerkId("PERK_ITEM_SPECIAL_LIFESTEAL_WEAPON");
        Check(Hash(transaction => transaction.RegisterPerkRule("rule", perk, ModRuleTarget.Opponent, ModRuleMode.All,
            null, 100000)) != Hash(transaction => transaction.RegisterPerkRule("rule", perk, ModRuleTarget.Opponent,
            ModRuleMode.All, null, 100001)), "Perk aspect is absent from the fingerprint.");
    }

    private static void CheckRewardConfiguration(ModDescriptor actualMod, string fixture)
    {
        var archived = new XmlDocument();
        archived.Load(Path.Combine(_repository, "ResearchSources/de128_assets/gamedata/stages.xml"));
        var archivedItems = archived.SelectNodes(
            "//Zone[@Name='ZONE_7']//Battle[@Name='C3_BOSS_TITAN_ECLIPSEMODE']/Fight[@Name='6']/Rewards/Reward/EclipseModeReward/Item")
            .Cast<XmlElement>().ToArray();
        string[] sourceNames = { "WEAPON_TITAN_GIANT_SWORD", "BODY_TITAN", "HEAD_TITAN",
            "RANGED_TITANS_HARPOON", "MAGIC_MIND_THROW" };
        string[] sourcePerks = { "PERK_ITEM_SPECIAL_LIFESTEAL_WEAPON", "PERK_ITEM_SPECIAL_SHIELDING_ARMOR",
            "PERK_ITEM_SPECIAL_DAMAGE_ABSORPTION_HEAD_HELM", "PERK_ITEM_SPECIAL_PRECISION_RANGED",
            "PERK_ITEM_SPECIAL_FRENZY_MAGIC" };
        Check(archivedItems.Length == sourceNames.Length, "Owner archive changed the final Titan reward count.");
        for (int i = 0; i < sourceNames.Length; i++)
        {
            var perk = (XmlElement)archivedItems[i].SelectSingleNode("Enchantments/Perk");
            Check(archivedItems[i].GetAttribute("Name") == sourceNames[i] &&
                archivedItems[i].GetAttribute("Level") == "?Player[].Level" &&
                archivedItems[i].GetAttribute("Drop") == "1" &&
                archivedItems[i].GetAttribute("ShowReward") == "1" &&
                perk?.GetAttribute("Name") == sourcePerks[i] &&
                perk["Set"]?.GetAttribute("Aspect") == "3639 / 100 * ?Player[].Level + 60",
                "Owner archive changed Titan reward grant " + i);
        }
        var catalog = new ModContentCatalog();
        RewardItemGrant actualGrant;
        using (LoadLive(actualMod, catalog))
        {
            CheckDE(catalog);
            int underworld = DE128UnderworldTests.Run(catalog, _repository, (ok, message) => Check(ok, message));
            Console.WriteLine("Underworld archive comparisons: " + underworld);
            Console.WriteLine("Challenger archive comparisons: " +
                DE128ChallengerTests.RunContent(catalog, _repository, (ok, message) => Check(ok, message)));
            var reward = catalog.Rewards.Single(value => value.Id.LocalId == "titans_desolator");
            Check(reward.TryGetGrant(0, out actualGrant) && !reward.TryGetGrant(-1, out _) &&
                !reward.TryGetGrant(5, out _), "Reward flat index validation failed.");
            string[] perks = {
                "PERK_ITEM_SPECIAL_LIFESTEAL_WEAPON", "PERK_ITEM_SPECIAL_SHIELDING_ARMOR",
                "PERK_ITEM_SPECIAL_DAMAGE_ABSORPTION_HEAD_HELM", "PERK_ITEM_SPECIAL_PRECISION_RANGED",
                "PERK_ITEM_SPECIAL_FRENZY_MAGIC" };
            foreach (int level in new[] { 1, 2, 7, 51, 52 })
            {
                for (int i = 0; i < perks.Length; i++)
                {
                    Check(reward.TryGetGrant(i, out var grant), "Titan reward grant is missing: " + i);
                    var configuration = grant.Configure(level);
                    Check(configuration.Level == level && configuration.Enchantments.Count == 1,
                        "Titan grant did not configure player-level equipment: " + i);
                    var enchantment = configuration.Enchantments[0];
                    Check(enchantment.Perk == CoreContentImporter.PerkId(perks[i]) &&
                        Math.Abs(enchantment.Aspect.Value - (3639d / 100 * level + 60)) < 0.0000001,
                        "Titan reward enchantment differs from the archive: " + i);
                    var repeated = grant.Configure(level);
                    Check(repeated != configuration && repeated.Enchantments != configuration.Enchantments &&
                        repeated.Enchantments[0].Aspect == enchantment.Aspect,
                        "Titan reward results are shared or nondeterministic: " + i);
                }
            }
        }
        Exception disposedFailure = null;
        try { actualGrant.Configure(52); } catch (Exception error) { disposedFailure = error; }
        Check(disposedFailure is ObjectDisposedException, "Disposed Lua context retained a callable reward configuration.");

        var cases = new[] {
            (Body: "return nil", Error: "must be a table"),
            (Body: "return { money = 100 }", Error: "unknown field"),
            (Body: "return { level = 0 }", Error: "1..10000"),
            (Body: "return { level = 1.5 }", Error: "integer"),
            (Body: "return { level = 10001 }", Error: "1..10000"),
            (Body: "return { enchantments = {{ perk = {} }} }", Error: "perk handle"),
            (Body: "return { enchantments = {{ perk = weapon }} }", Error: "perk handle"),
            (Body: "return { enchantments = {{ perk = perk }, { perk = perk }} }", Error: "duplicate"),
            (Body: "return { enchantments = {[2] = { perk = perk }} }", Error: "dense array"),
            (Body: "return { enchantments = {{ perk = perk, aspect = '1900' }} }", Error: "number"),
            (Body: "return { enchantments = {{ perk = perk, aspect = -1 }} }", Error: "finite"),
            (Body: "return { enchantments = {{ perk = perk, aspect = 0/0 }} }", Error: "finite"),
            (Body: "return { enchantments = {{ perk = perk, aspect = math.huge }} }", Error: "finite"),
            (Body: "return { enchantments = {{ perk = perk, aspect = 2147483648 }} }", Error: "finite"),
            (Body: "return { enchantments = {{ perk = perk, chance = 1.1 }} }", Error: "chance"),
            (Body: "return { enchantments = {{ perk = perk, chance_factor = 10001 }} }", Error: "chance_factor"),
            (Body: "return { enchantments = {{ perk = perk, frames = -1 }} }", Error: "frames"),
            (Body: "return { enchantments = {{ perk = perk, parameters = { Chance = 1 } }} }", Error: "parameter"),
            (Body: "return { enchantments = {{ perk = perk, parameters = { DamageFactor = 0/0 } }} }", Error: "finite"),
            (Body: "local rows = {}; for i=1,65 do rows[i] = {perk=perk} end; return {enchantments=rows}", Error: "64"),
            (Body: "sf2.state.set { x = 1 }; return {}", Error: "sf2 operations are unavailable"),
            (Body: "sf2.profile.level(); return {}", Error: "sf2 operations are unavailable"),
            (Body: "sf2.scenes.open('shop'); return {}", Error: "sf2 operations are unavailable"),
            (Body: "error('callback failure')", Error: "callback failure"),
            (Body: "while true do end", Error: "instruction budget"),
        };
        var script = new StringBuilder(@"
local weapon = sf2.items.get('core:items/weapon/WEAPON_TITAN_GIANT_SWORD')
local perk = sf2.perks.get('core:perks/PERK_ITEM_SPECIAL_LIFESTEAL_WEAPON')
sf2.rewards.register {id='valid', items={{item=weapon, upgrade=2, configure=function(context)
    assert(context.item_id == 'core:items/weapon/weapon_titan_giant_sword')
    return {level=context.player_level, enchantments={{perk=perk, aspect=123.45,
        chance_factor=2.5, chance=0.3, frames=300, parameters={Base=-1000, DamageFactor=15850}}}}
end}}, choices={{items={{item=weapon, weight=3, configure=function(context) return {} end}}}}}
");
        for (int i = 0; i < cases.Length; i++)
            script.Append("sf2.rewards.register {id='bad").Append(i).Append("', items={{item=weapon, configure=function(context) ")
                .Append(cases[i].Body).Append(" end}}}\n");
        var peer = Peer(fixture, "fixture.reward-config", "content.register", script.ToString());
        var peerCatalog = new ModContentCatalog();
        using (LoadLive(peer, peerCatalog))
        {
            Check(peerCatalog.TryGetReward(DefinitionId.Parse(peer.Id + ":rewards/valid"), out var valid),
                "Valid reward was not registered.");
            Check(valid.TryGetGrant(1, out var weighted) && weighted.UsesConfiguration &&
                weighted.Configure(52).Enchantments.Count == 0 && !weighted.Configure(52).Level.HasValue,
                "Weighted reward configuration did not preserve omitted defaults.");
            Check(!valid.TryGetGrant(2, out _), "Weighted reward flat index overflow was accepted.");
            for (int i = 0; i < cases.Length; i++)
            {
                var grant = peerCatalog.Rewards.Single(reward => reward.Id.LocalId == "bad" + i).Items[0];
                Exception failure = null;
                try { grant.Configure(52); } catch (Exception error) { failure = error; }
                Check(failure != null && failure.ToString().Contains(cases[i].Error),
                    "Reward callback expected '" + cases[i].Error + "', got " + failure);
                var recovered = valid.Items[0].Configure(7);
                Check(recovered.Level == 7 && valid.Items[0].UpgradeNumber == 2 &&
                    recovered.Enchantments[0].Aspect == 123.45 && recovered.Enchantments[0].ChanceFactor == 2.5 &&
                    recovered.Enchantments[0].Chance == 0.3 &&
                    recovered.Enchantments[0].Frames == 300 && recovered.Enchantments[0].Parameters["Base"] == -1000 &&
                    recovered.Enchantments[0].Parameters["DamageFactor"] == 15850,
                    "A failed callback leaked its scope or changed a later reward.");
            }
        }
        var plainCatalog = new ModContentCatalog();
        ImportCore(plainCatalog);
        var configuredCatalog = new ModContentCatalog();
        ImportCore(configuredCatalog);
        using (var transaction = plainCatalog.BeginRegistration(peer))
        {
            transaction.RegisterReward("fingerprint", new[] { new RewardItemGrant(CoreSword) }, null);
            transaction.Commit();
        }
        using (var transaction = configuredCatalog.BeginRegistration(peer))
        {
            transaction.RegisterReward("fingerprint", new[] { new RewardItemGrant(CoreSword, configure:
                level => new RewardGrantConfiguration(level)) }, null);
            transaction.Commit();
        }
        Check(ModSaveData.ComputeContentSetFingerprint(new[] { peer }, plainCatalog) !=
            ModSaveData.ComputeContentSetFingerprint(new[] { peer }, configuredCatalog),
            "Configured rewards are absent from the content fingerprint.");

        foreach (string kind in new[] { "consumable", "free", "seal" })
        {
            var invalid = Peer(fixture, "fixture.reward-" + kind, "content.register",
                "local title = sf2.localization.register{id='title',language='eng',value='Token'}\n" +
                "local item = sf2.items.register_" + kind + "{id='token',display_name=title}\n" +
                "sf2.rewards.register{id='bad',items={{item=item,configure=function(ctx) return {} end}}}\n");
            var rejected = new ModContentCatalog();
            ExpectFailure(invalid, rejected, "require equipment");
            Check(rejected.Rewards.Count == 0 && !rejected.Localizations.Any(item => item.Id.Namespace == invalid.Id),
                "Rejected non-equipment reward leaked its transaction.");
        }
        var invalidCallback = Peer(fixture, "fixture.reward-function", "content.register",
            "sf2.rewards.register{id='bad',items={{item=sf2.items.get('core:items/weapon/WEAPON_TITAN_GIANT_SWORD'),configure=42}}}");
        ExpectFailure(invalidCallback, new ModContentCatalog(), "must be a Lua function");
    }

    private static void CheckLocalizationReferences(string fixture)
    {
        var owner = Peer(fixture, "fixture.strings", "content.register", "");
        var reader = Peer(fixture, "fixture.reader", "content.register", @"
local translated = sf2.localization.register { id='script_label', language='eng', value='From Lua' }
sf2.localization.register { id='script_label', language='pol', value='Z Lua' }
assert(sf2.localization.text(translated, 'eng') == 'From Lua')
assert(sf2.localization.text(translated, 'pol') == 'Z Lua')
assert(sf2.localization.text(translated, 'fra') == 'From Lua')
assert(sf2.localization.text(sf2.localization.key('script_label')) == 'From Lua')
assert(sf2.localization.text(translated) == 'From Lua')
assert(sf2.localization.text(sf2.localization.key('local_label')) == 'Local')
assert(sf2.localization.text(sf2.localization.key('fixture.reader:localization/local_label')) == 'Local')
assert(sf2.localization.key('core:localization/WEAPON_TITAN_GIANT_SWORD'))
");
        var catalog = new ModContentCatalog();
        ImportCore(catalog);
        using (var transaction = catalog.BeginRegistration(owner))
        {
            transaction.AddLocalization("label", "eng", "Foreign");
            transaction.Commit();
        }
        var assets = new AssetResolver(new IAssetProvider[] { new LooseModProvider(reader) });
        using (var transaction = catalog.BeginRegistration(reader))
        {
            transaction.AddLocalization("local_label", "eng", "Local");
            using (var context = new MoonSharpScriptRuntime().CreateContext(reader,
                new ModApiFacade(reader, assets, transaction, new ModStateRuntime(), null)))
                context.ExecuteEntrypoint();
            transaction.Commit();
        }
        Check(true, "Lua translations, fallback, and local/qualified/core localization references.");
        Check(catalog.Localizations.Count(value => value.Id.Namespace == reader.Id) == 2,
            "Lua language variants did not share their definition.");

        // The mod sandbox deliberately excludes pcall. Catch each rejected entrypoint
        // at the real host boundary and inspect its transaction before disposal.
        string[] invalidCalls = {
            "sf2.localization.register { id='local_label', language='ENG', value='Duplicate' }",
            "sf2.localization.register { id='bad_fields', language='eng', value='Bad', unknown=true }",
            "sf2.localization.register { id='bad_value', language='eng', value=42 }",
            "sf2.localization.register { id='bad_language', language='', value='Bad' }",
            "sf2.localization.register { id='bad_empty', language='eng', value='' }",
            "sf2.localization.register { id='fixture.strings:localization/label', language='eng', value='Foreign' }",
            "sf2.localization.key('fixture.strings:localization/label')",
            "sf2.localization.key('core:items/weapon/WEAPON_TITAN_GIANT_SWORD')",
            "sf2.localization.key('core:localization/not_registered')",
            "sf2.localization.key('')",
        };
        string[] diagnostics = { "Duplicate", "unknown", "value", "language", "value",
            "Definition ID", "undeclared namespace", "localization", "not registered", "empty" };
        for (int i = 0; i < invalidCalls.Length; i++)
        {
            var invalid = Peer(fixture, "fixture.invalid" + i, "content.register", invalidCalls[i]);
            var invalidAssets = new AssetResolver(new IAssetProvider[] { new LooseModProvider(invalid) });
            using (var transaction = catalog.BeginRegistration(invalid))
            {
                transaction.AddLocalization("local_label", "eng", "Original");
                Exception failure = null;
                using (var context = new MoonSharpScriptRuntime().CreateContext(invalid,
                    new ModApiFacade(invalid, invalidAssets, transaction, new ModStateRuntime(), null)))
                {
                    try { context.ExecuteEntrypoint(); }
                    catch (ModScriptException error) { failure = error; }
                }
                Check(failure != null && failure.Message.IndexOf(diagnostics[i], StringComparison.OrdinalIgnoreCase) >= 0,
                    "Incorrect rejection for " + invalidCalls[i] + ": " + failure);
                Check(transaction.RegistrationCount == 1 && transaction.ReadLocalization(
                    DefinitionId.Parse(invalid.Id + ":localization/local_label"), "eng") == "Original",
                    "Rejected localization call partially mutated its transaction.");
            }
            Check(!catalog.Localizations.Any(value => value.Id.Namespace == invalid.Id),
                "Rejected entrypoint committed a translation.");
        }
    }

    private static void CheckShopContracts(string source, string fixture, ModDescriptor mod, ModContentCatalog enabled)
    {
        const string target = "core:items/weapon/WEAPON_BP_S1_GUARDIAN";
        const string call = "local sf2 = require('sf2')\nsf2.shop.set_availability { item=sf2.items.get('" + target + "'), visibility=sf2.shop.FORCE_VISIBLE";
        var fingerprints = new List<string>();
        foreach (string value in new[] { "omitted", "0", "15", "16", "52", "-1", "53", "1.5", "'15'" })
        {
            var probe = CopyPackage(source, fixture, "shop-level-" + fingerprints.Count + "-" + value.Replace("'", ""));
            File.WriteAllText(Path.Combine(probe.RootPath, "scripts/main.lua"),
                call + (value == "omitted" ? "" : ", minimum_level=" + value) + " }\n", Utf8);
            var catalog = new ModContentCatalog();
            if (new[] { "-1", "53", "1.5", "'15'" }.Contains(value))
            {
                ExpectFailure(probe, catalog, "minimum_level");
                Check(!catalog.ItemAvailabilityPolicies.Any(), "Invalid Lua level left an availability patch.");
            }
            else
            {
                Load(probe, catalog);
                Check(catalog.TryGetItemAvailability(DefinitionId.Parse(target), out var policy) &&
                    policy.MinimumLevel == (value == "omitted" ? 0 : int.Parse(value)), "Lua level parsing changed.");
                fingerprints.Add(ModSaveData.ComputeContentSetFingerprint(new[] { mod }, catalog));
            }
        }
        Check(fingerprints[0] == fingerprints[1], "Explicit zero changed the legacy availability fingerprint.");
        Check(fingerprints.Skip(1).Distinct().Count() == 4, "Different level gates share a fingerprint.");

        var competing = Peer(fixture, "fixture.shop-conflict", "content.patch",
            "sf2.shop.set_availability { item=sf2.items.get('" + target + "'), visibility=sf2.shop.FORCE_HIDDEN }\n", "content.register");
        ExpectFailure(competing, enabled, "already patched");
        Check(enabled.TryGetItemAvailability(DefinitionId.Parse(target), out var retained) &&
            retained.Owner == mod.Id && retained.MinimumLevel == 15, "Conflict modified active DE policy.");
        var peerFirst = new ModContentCatalog();
        Load(competing, peerFirst);
        ExpectFailure(mod, peerFirst, "already patched");
        Check(peerFirst.ItemAvailabilityPolicies.Count() == 1 && peerFirst.TimerPolicies.Count == 0 &&
            !peerFirst.TryGetItem(Sword, out _) && Services.All(peerFirst.FeatureEnabled),
            "Shop conflict leaked partial DE registration.");
    }

    private static void CheckCombatEquipment(string source, string fixture, string repository, ModDescriptor mod, ModContentCatalog catalog)
    {
        var archive = ReadXml(Path.Combine(repository, "Assets/DExml/list.xml"));
        var moves = ReadXml(Path.Combine(repository, "Assets/vanillaXml/animations/moves.xml"));
        foreach (var patch in catalog.ItemCombatSubtypes)
        {
            Check(catalog.TryGetItem(patch.Item, out var item) && item.IsCore && patch.Owner == mod.Id, "Subtype patch lost core identity.");
            var original = ReadElement(item.LegacyItemXml);
            var expected = (XmlElement)archive.SelectSingleNode("/List/Items/Item[@Name='" + item.LegacyName + "']");
            Check(expected.GetAttribute("SubType") == patch.Subtype && original.GetAttribute("SubType") != patch.Subtype,
                "Subtype is not an exact archive delta: " + item.LegacyName);
            bool ownedFamily = patch.Subtype == "ChineseSwords" && catalog.Moves.Count == 61 &&
                catalog.Moves.Count(move => move.Graph.Locks.Any(condition => condition.Kind == ModMoveConditionKind.Item && condition.ItemSubType == "ChineseSwords")) == 2 &&
                catalog.MoveItemLockExtensions.Count == 10;
            Check(moves.SelectNodes("//Item[@SubType='" + patch.Subtype + "']").Count > 0 || ownedFamily,
                "Patched subtype has no complete registered move family: " + patch.Subtype);
        }
        foreach (var patch in catalog.ItemTacticSubtypes)
        {
            catalog.TryGetItem(patch.Item, out var item);
            var expected = (XmlElement)archive.SelectSingleNode("/List/Items/Item[@Name='" + item.LegacyName + "']");
            Check(ReadElement(item.LegacyItemXml).HasAttribute("TacticSubtype") && !expected.HasAttribute("TacticSubtype") && patch.Group == "",
                "Removed AI group is not an exact archive delta.");
        }
        const string id = "core:items/weapon/WEAPON_CHNY22_SPEAR";
        string prefix = "local sf2=require('sf2')\nsf2.items.set_subtype { item=sf2.items.get('" + id + "'), subtype=";
        var values = new[] { "''", "'bad value'", "'a/b'", "'a:b'", "'ż'", "string.rep('x',129)", "4", "nil" };
        for (int i = 0; i < values.Length; i++)
        {
            var invalid = CopyPackage(source, fixture, "invalid-subtype-" + i);
            File.WriteAllText(Path.Combine(invalid.RootPath, "scripts/main.lua"), prefix + values[i] + " }", Utf8);
            var rejected = new ModContentCatalog();
            ExpectFailure(invalid, rejected, "subtype");
            Check(rejected.ItemCombatSubtypes.Count == 0, "Invalid subtype leaked registration.");
        }
        var wrongKind = CopyPackage(source, fixture, "subtype-armor");
        File.WriteAllText(Path.Combine(wrongKind.RootPath, "scripts/main.lua"),
            "local sf2=require('sf2')\nsf2.items.set_subtype {item=sf2.items.get('core:items/armor/ARMOR_BP_S1_GUARDIAN'), subtype='Katana'}", Utf8);
        ExpectFailure(wrongKind, new ModContentCatalog(), "requires a weapon");
        var noCapability = CopyPackage(source, fixture, "subtype-no-capability",
            "[\"content.register\"]");
        File.WriteAllText(Path.Combine(noCapability.RootPath, "scripts/main.lua"), prefix + "'Naginata' }", Utf8);
        ExpectFailure(noCapability, new ModContentCatalog(), "content.patch");
        var economy = CopyPackage(source, fixture, "subtype-no-economy");
        File.WriteAllText(Path.Combine(economy.RootPath, "scripts/main.lua"), prefix + "'Naginata', price=0 }", Utf8);
        ExpectFailure(economy, new ModContentCatalog(), "price");
        var peer = Peer(fixture, "fixture.subtype", "content.patch", prefix.Replace("local sf2=require('sf2')\n", "") + "'Spear' }", "content.register");
        ExpectFailure(peer, catalog, "already patched");
        var peerFirst = new ModContentCatalog(); Load(peer, peerFirst);
        ExpectFailure(mod, peerFirst, "already patched");
        Check(peerFirst.ItemCombatSubtypes.Count == 1 && peerFirst.ItemTacticSubtypes.Count == 0 && peerFirst.TimerPolicies.Count == 0,
            "Subtype conflict leaked other DE patches.");
        var other = new ModContentCatalog(); ImportCore(other);
        using (var tx = other.BeginRegistration(peer)) { tx.SetCombatSubtype(DefinitionId.Parse(id), "Naginata"); tx.Commit(); }
        Check(ModSaveData.ComputeContentSetFingerprint(new[] { peer }, peerFirst) != ModSaveData.ComputeContentSetFingerprint(new[] { peer }, other),
            "Subtype content is missing from fingerprints.");
    }

    private static XmlElement ReadElement(string xml)
    { var document = new XmlDocument { XmlResolver = null }; document.LoadXml(xml); return document.DocumentElement; }

    private static void CheckInitialProfileBinding(string source, string fixture, ModDescriptor mod, ModContentCatalog enabled)
    {
        const string target = "core:items/weapon/WEAPON_BP_S1_GUARDIAN";
        string call = "sf2.items.set_initial_profile { item=sf2.items.get('" + target +
            "'), level=15, upgrade_level=1500, upgrade_template='Weapon_Bonus', legacy_paid_item='none', initial_stats={weapon_damage=342} }";
        var noCapability = CopyPackage(source, fixture, "initial-profile-no-capability", "[\"content.register\"]");
        File.WriteAllText(Path.Combine(noCapability.RootPath, "scripts/main.lua"), "local sf2=require('sf2')\n" + call, Utf8);
        ExpectFailure(noCapability, new ModContentCatalog(), "content.patch");
        var wrongField = CopyPackage(source, fixture, "initial-profile-wrong-field");
        File.WriteAllText(Path.Combine(wrongField.RootPath, "scripts/main.lua"),
            "local sf2=require('sf2')\n" + call.Replace("weapon_damage", "unknown_stat"), Utf8);
        var rejected = new ModContentCatalog();
        ExpectFailure(wrongField, rejected, "unknown_stat");
        Check(rejected.ItemInitialProfiles.Count == 0, "Invalid Lua profile leaked a catalog patch.");
        var wrongTemplate = CopyPackage(source, fixture, "initial-profile-wrong-template");
        File.WriteAllText(Path.Combine(wrongTemplate.RootPath, "scripts/main.lua"),
            "local sf2=require('sf2')\n" + call.Replace("Weapon_Bonus", "Armor_Bonus"), Utf8);
        ExpectFailure(wrongTemplate, new ModContentCatalog(), "upgrade_template");
        var wrongPaid = CopyPackage(source, fixture, "initial-profile-wrong-paid-marker");
        File.WriteAllText(Path.Combine(wrongPaid.RootPath, "scripts/main.lua"),
            "local sf2=require('sf2')\n" + call.Replace("legacy_paid_item='none'", "legacy_paid_item='unknown'"), Utf8);
        ExpectFailure(wrongPaid, new ModContentCatalog(), "legacy_paid_item");
        var peer = Peer(fixture, "fixture.initial-profile-conflict", "content.patch", call, "content.register");
        ExpectFailure(peer, enabled, "already patched");
        Check(enabled.ItemInitialProfiles.Count == 221 && enabled.ItemInitialProfiles.All(value => value.Owner == mod.Id),
            "A competing profile changed the active DE catalog.");
    }

    private sealed class ControlFighter : IModFighterOperations, IModFighterControls
    {
        internal readonly List<(object Owner, string Control, bool Blocked)> Calls = new List<(object, string, bool)>();
        internal bool Refuse;
        public bool TrySetControlBlocked(object owner, string control, bool blocked, out string error)
        {
            error = Refuse ? "fixture unavailable" : "";
            if (Refuse) return false;
            Calls.Add((owner, control, blocked)); return true;
        }
        public bool TryChangeHealth(double amount, out string error) { error="unexpected health"; return false; }
        public bool TryAddMagicCharge(double amount, out string error) { error="unexpected magic"; return false; }
    }

    private static void CheckControlCallbacks(string source, string fixture)
    {
        var peer = Peer(fixture, "fixture.controls", "content.register", @"
local pending=require('content.sensei_raid_charge')
local count=0
pending.register(function() count=count+1; return count%2==0 end)
local saved
sf2.behaviors.register{id='expired',on_round_begin=function(_,fighter)
 if saved then saved('kick',true) else saved=fighter.set_control_blocked end
end}
", "combat.effects");
        Directory.CreateDirectory(Path.Combine(peer.RootPath,"scripts/content"));
        File.Copy(Path.Combine(source,"scripts/content/sensei_raid_charge.lua"),Path.Combine(peer.RootPath,"scripts/content/sensei_raid_charge.lua"));
        var catalog=new ModContentCatalog();var fighter=new ControlFighter();
        var saved=new XmlDocument();saved.LoadXml("<Rule/>");
        var other=new XmlDocument();other.LoadXml("<Rule/>");
        var parameters=new Dictionary<string,ModParameterValue>();var context=new Dictionary<string,string>{{"side","player"},{"round","1"}};
        using(var live=LoadLive(peer,catalog)) {
            var interactive=(IModInteractiveBehaviorScriptContext)live;
            bool Call(string id, XmlNode node, out string error)=>interactive.TryInvokeBehavior(DefinitionId.Parse("fixture.controls:behaviors/"+id),ModEffectEvent.RoundBegin,parameters,context,new ModInstanceFighter(fighter,node),out error);
            Check(Call("sensei_raid_charge",saved.DocumentElement,out var error),error);
            Check(Call("sensei_raid_charge",saved.DocumentElement,out error),error);
            Check(Call("sensei_raid_charge",other.DocumentElement,out error),error);
            Check(fighter.Calls.Count==3&&fighter.Calls[0].Control=="raid_charge"&&fighter.Calls[0].Blocked&&!fighter.Calls[1].Blocked&&fighter.Calls[2].Blocked,"Actual DE condition did not alternate block/release from boolean availability.");
            Check(Equals(fighter.Calls[0].Owner,fighter.Calls[1].Owner)&&!Equals(fighter.Calls[0].Owner,fighter.Calls[2].Owner),"Control ownership lost instance provenance.");
            Check(Call("expired",saved.DocumentElement,out error),error);
            Check(!Call("expired",saved.DocumentElement,out error)&&error.Contains("expired"),"Escaped control callback remained usable.");
            fighter.Refuse=true;
            Check(!Call("sensei_raid_charge",saved.DocumentElement,out error)&&error.Contains("fixture unavailable"),"Native refusal was swallowed.");
            Check(fighter.Calls.Count==3,"Failed callbacks mutated controls.");
        }
        fighter.Refuse=false;
        int index=0;
        foreach(var body in new[]{"fighter:set_control_blocked('Punch',true)","fighter:set_control_blocked('kick',1)","fighter:set_control_blocked('kick',nil)","fighter:set_control_blocked('move',true)"}) {
            var invalid=Peer(fixture,"fixture.control-invalid-"+index++,"content.register","sf2.behaviors.register{id='invalid',on_round_begin=function(_,fighter) "+body+" end}","combat.effects");
            using(var live=LoadLive(invalid,new ModContentCatalog())) {
                Check(!((IModInteractiveBehaviorScriptContext)live).TryInvokeBehavior(DefinitionId.Parse(invalid.Id+":behaviors/invalid"),ModEffectEvent.RoundBegin,parameters,context,fighter,out _),"Invalid control argument accepted.");
                Check(fighter.Calls.Count==3,"Invalid argument reached native mutation.");
            }
        }
        fighter.Refuse=false;
        var denied=Peer(fixture,"fixture.controls-denied","content.register","sf2.behaviors.register{id='denied',on_round_begin=function(_,fighter) fighter:set_control_blocked('kick',true) end}");
        using(var live=LoadLive(denied,new ModContentCatalog()))
            Check(!((IModInteractiveBehaviorScriptContext)live).TryInvokeBehavior(DefinitionId.Parse(denied.Id+":behaviors/denied"),ModEffectEvent.RoundBegin,parameters,context,fighter,out var error)&&error.Contains("combat.effects")&&fighter.Calls.Count==3,"Missing capability allowed control mutation.");
        Check(!new ModInstanceFighter(fighter,null).TrySetControlBlocked(new object(),"kick",true,out _),"Missing instance allowed control mutation.");
        var invalidReader=Peer(fixture,"fixture.controls-reader","content.register","require('content.sensei_raid_charge').register(function() return 0 end)","combat.effects");
        Directory.CreateDirectory(Path.Combine(invalidReader.RootPath,"scripts/content"));
        File.Copy(Path.Combine(source,"scripts/content/sensei_raid_charge.lua"),Path.Combine(invalidReader.RootPath,"scripts/content/sensei_raid_charge.lua"));
        using(var live=LoadLive(invalidReader,new ModContentCatalog()))
            Check(!((IModInteractiveBehaviorScriptContext)live).TryInvokeBehavior(DefinitionId.Parse(invalidReader.Id+":behaviors/sensei_raid_charge"),ModEffectEvent.RoundBegin,parameters,context,fighter,out var error)&&error.Contains("must be boolean")&&fighter.Calls.Count==3,"Invalid availability reader mutated controls.");
    }

    private static string _repository;

    private static void Run(string source, string fixture, string repository)
    {
        _repository = repository;
        _items = ReadXml(Path.Combine(repository, "Assets/vanillaXml/list.xml"));
        Check(Eclipse.Content.ItemListCompatibility.AddHistoricalStageAliases(_items) == 3,
            "Eclipse core stage aliases changed.");
        var archivedItems = ReadXml(Path.Combine(repository, "Assets/DExml/list.xml"));
        foreach (XmlElement item in archivedItems.SelectNodes("/List/Items/Item[@Type='Weapon' or @Type='Armor' or @Type='Helm' or @Type='Ranged' or @Type='Magic']"))
        {
            var baseItem = _items.SelectSingleNode("/List/Items/Item[@Name='" + item.GetAttribute("Name") + "']") as XmlElement;
            if (baseItem != null)
            {
                if (baseItem.GetAttribute("ShopHide") == "1" && item.GetAttribute("ShopHide") != "1")
                {
                    if (baseItem.GetAttribute("Image") != item.GetAttribute("Image"))
                        RestoredAssets["ui/items/" + item.GetAttribute("Image").ToLowerInvariant()] = AssetKind.Sprite;
                    if (baseItem.GetAttribute("Model") != item.GetAttribute("Model"))
                        RestoredAssets["gamedata/models/" + item.GetAttribute("Model").ToLowerInvariant()] = AssetKind.Model;
                }
                continue;
            }
            if (item.GetAttribute("ShopHide") == "1") continue;
            RestoredAssets["gamedata/models/" + item.GetAttribute("Model").ToLowerInvariant()] = AssetKind.Model;
            RestoredAssets["ui/items/" + item.GetAttribute("Image").ToLowerInvariant()] = AssetKind.Sprite;
        }
        _perks = ReadXml(Path.Combine(repository, "Assets/vanillaXml/perks.xml"));
        _stages = ReadXml(Path.Combine(repository, "Assets/vanillaXml/stages.xml"));
        _languages = new Dictionary<string, XmlDocument> {
            { "eng", ReadXml(Path.Combine(repository, "Assets/vanillaXml/localizations/eng.xml")) } };
        Check(!Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories)
            .Any(file => string.Equals(Path.GetExtension(file), ".xml", StringComparison.OrdinalIgnoreCase)),
            "DE128 must not ship XML mod definitions.");
        var mod = CopyPackage(source, fixture, "valid");
        Check(mod.Manifest.Capabilities.Count == Capabilities.Length && Capabilities.All(mod.Manifest.Capabilities.Contains),
            "DE128 requests unexpected capabilities.");
        Check(mod.Manifest.Dependencies.Count == 1 && mod.Manifest.Dependencies[0].Id.Value == "core",
            "DE128 unexpectedly depends on another mod.");

        foreach (string asset in new[] { "gamedata/models/mdl_weapon_super_knives", "ui/items/weapon20.img_weapon_giant_sword", "gamedata/models/mdl_magic_mass_bomb", "ui/items/armor31.img_armor_old_legioner" })
        {
            var missingRestored = new ModContentCatalog();
            ExpectFailure(mod, missingRestored, asset, asset);
            CheckBase(missingRestored);
        }
        CheckBase(null);
        var enabled = new ModContentCatalog();
        Load(mod, enabled);
        CheckDE(enabled);
        CheckCampaignMusicArchive(enabled);
        CheckDojoArchive(enabled);
        CheckDojoInteraction(mod);
        CheckMapButtonFingerprint(fixture);
        CheckBattlePositions(mod, enabled, fixture);
        CheckForgeArchive(enabled);
        DE128ShopTests.Run(mod, enabled, repository, Check);
        DE128EquipmentTests.Run(mod, enabled, repository, Check);
        CheckSharedMovePatches(mod, enabled, fixture, repository);
        CheckTeleportation(enabled);
        CheckMoveReplacementContract(fixture, enabled, repository);
        CheckShopContracts(source, fixture, mod, enabled);
        CheckCombatEquipment(source, fixture, repository, mod, enabled);
        CheckInitialProfileBinding(source, fixture, mod, enabled);
        string fingerprint = ModSaveData.ComputeContentSetFingerprint(new[] { mod }, enabled);
        var legacyTimers = new ModContentCatalog();
        using (var tx = legacyTimers.BeginRegistration(mod)) { tx.SetTimer("forge", 0, true); tx.Commit(); }
        var pendingTimers = new ModContentCatalog();
        using (var tx = pendingTimers.BeginRegistration(mod)) { tx.SetTimer("forge", 0, true, true); tx.Commit(); }
        Check(ModSaveData.ComputeContentSetFingerprint(new[] { mod }, legacyTimers) !=
            ModSaveData.ComputeContentSetFingerprint(new[] { mod }, pendingTimers), "Pending policy is missing from fingerprints.");
        var invalidPending = Peer(fixture, "fixture.invalid-pending", "policy.timers",
            "sf2.timers.set { subsystem = 'forge', seconds = 120, complete_pending = true }\n");
        var rejectedPending = new ModContentCatalog();
        ExpectFailure(invalidPending, rejectedPending, "complete_pending requires seconds = 0");
        Check(rejectedPending.TimerPolicies.Count == 0, "Invalid pending policy was committed.");

        // Apply & Restart rebuilds the catalog. Disposing Lua alone is not a policy reset.
        CheckBase(new ModContentCatalog());
        var reenabled = new ModContentCatalog();
        Load(mod, reenabled);
        CheckDE(reenabled);
        Check(fingerprint == ModSaveData.ComputeContentSetFingerprint(new[] { mod }, reenabled),
            "Repeated loading changed the policy fingerprint.");

        var ads = Peer(fixture, "fixture.ads", "policy.services", "sf2.services.disable(\"ads\")\n");
        Load(ads, enabled);
        CheckDE(enabled);
        var peerFirst = new ModContentCatalog();
        Load(ads, peerFirst);
        Load(mod, peerFirst);
        CheckDE(peerFirst);

        var timer = Peer(fixture, "fixture.timer", "policy.timers",
            "sf2.timers.set { subsystem = \"forge\", seconds = 120, skip_enabled = false }\n");
        ExpectFailure(timer, enabled, "Timer policy already owned: forge");
        CheckDE(enabled);
        var conflict = new ModContentCatalog();
        Load(timer, conflict);
        ModPolicies.Content = conflict;
        Check(!ModPolicies.CompletePending("forge"), "Omitting complete_pending changed existing policy behavior.");
        // DE queues services before its conflicting timer. None may leak on failed commit.
        ExpectFailure(mod, conflict, "Timer policy already owned: forge");
        ModPolicies.Content = conflict;
        Check(conflict.TimerPolicies.Count == 1 && conflict.TryGetTimer("forge", out var original) &&
            original.Owner.Value == "fixture.timer" && ModPolicies.DeliverySeconds("forge", 1) == 120 &&
            !ModPolicies.SkipEnabled("forge"), "Failed DE registration changed the other mod's timer.");
        Check(Services.All(ModPolicies.FeatureEnabled), "Failed DE registration leaked service disables.");

        foreach (string missing in Capabilities)
        {
            string retained = "[" + string.Join(", ", Capabilities.Where(value => value != missing).Select(value => "\"" + value + "\"")) + "]";
            var restricted = CopyPackage(source, fixture, "missing-" + missing, retained);
            // Combat capabilities are enforced when invoking the effect, not at registration.
            if (missing.StartsWith("combat."))
            {
                DECombatPerksTests.CheckMissingCapability(restricted, missing,
                    (descriptor, content) => LoadLive(descriptor, content), Check);
                continue;
            }
            // Story capabilities used only inside later callbacks (profile queries, UI) are
            // enforced when invoked; their dedicated Sensei suites cover the refusal.
            if (CallTimeCapabilities.Contains(missing))
            {
                var allowed = new ModContentCatalog();
                Load(restricted, allowed);
                Check(allowed.Fights.Count(fight => !fight.IsCore && fight.Id.LocalId.StartsWith("sensei_act_")) == 23, "Call-time capability changed registration: " + missing);
                continue;
            }
            var rejected = new ModContentCatalog();
            ExpectFailure(restricted, rejected, missing);
            Check(rejected.TimerPolicies.Count == 0, "Missing capability left a committed timer.");
            CheckBase(rejected);
        }

        foreach (string module in new[] { "timers", "equipment", "combat_equipment", "chinese_swords", "chinese_swords_data", "restored_weapons", "restored_equipment", "sphere1", "sphere2", "sphere3", "combo_sphere3", "shared_moves", "shop", "rewards", "combat_perks", "progression", "dandy_lightning_chain", "widow_teleportation" })
        {
            var incomplete = CopyPackage(source, fixture, "missing-module-" + module, omit: "scripts/content/" + module + ".lua");
            var partial = new ModContentCatalog();
            ExpectFailure(incomplete, partial, "scripts/content/" + module);
            Check(partial.TimerPolicies.Count == 0, "Missing module left a committed timer.");
            CheckBase(partial);
        }
        foreach (string missing in new[] { "gamedata/models/mdl_weapon_giant_sword", "ui/items/weapon17.img_weapon_boss_giant_sword",
            "ui/skills/iconmasterofstyle", "ui/skills/iconmasterofstyle_blue", "ui/skills/iconcrackedapple", "ui/skills/iconcrackedapple_blue" })
        {
            var partial = new ModContentCatalog();
            ExpectFailure(mod, partial, missing, missing);
            Check(partial.TimerPolicies.Count == 0, "Missing art left a committed timer.");
            CheckBase(partial);
        }
        var missingAnimation = CopyPackage(source, fixture, "missing-chinese-animation",
            omit: "assets/animations/chinese_swords_super_slash_old.bytes");
        var missingAnimationCatalog = new ModContentCatalog();
        ExpectFailure(missingAnimation, missingAnimationCatalog, "animations/chinese_swords_super_slash_old");
        CheckBase(missingAnimationCatalog);
        CheckLocalizationReferences(fixture);
        CheckRewardConfiguration(mod, fixture);
        Console.WriteLine("Underworld story checks: " + DE128UnderworldStoryTests.Run(mod, repository,
            (descriptor, content, bus, state) => LoadLive(descriptor, content, bus, state), Check));
        Console.WriteLine("Challenger story checks: " + DE128ChallengerTests.RunStory(mod, repository,
            (descriptor, content, bus, state) => LoadLive(descriptor, content, bus, state), Check));
        CheckTrialFingerprints(fixture);
        CheckInitialStats(fixture);
        DECombatPerksTests.Run(mod, repository, (descriptor, content) => LoadLive(descriptor, content), Check);
        CheckControlCallbacks(source, fixture);
        var forceDisabled = CopyPackage(source, fixture, "explicit-disabled-require");
        File.AppendAllText(Path.Combine(forceDisabled.RootPath, "scripts/main.lua"),
            "\nrequire('content.ascension')\nrequire('content.ascension_rules')\n", Utf8);
        var withoutAscension = new ModContentCatalog();
        Load(forceDisabled, withoutAscension);
        CheckDE(withoutAscension);
        Console.WriteLine("PASS: " + _checks + " DE128 foundation checks. Actual package, current MoonSharp bindings, " +
            "policy consumers, rebuild/reenable, composition, conflict rollback, capability and module failures. " +
            "Canonical core projections, Desolator reward scaling, callback safety/lifetime and missing-art rollback. " +
            "DE combat perk traces verified against archived XML evidence. Controlled art metadata only; " +
            "no Unity playtest or player save used, and the mod itself loads no XML.");
    }

    public static int Main(string[] args)
    {
        try
        {
            if (args.Length != 3) throw new ArgumentException("Expected the DE128 source folder, isolated fixture folder and repository.");
            Run(Path.GetFullPath(args[0]), Path.GetFullPath(args[1]), Path.GetFullPath(args[2]));
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            return 1;
        }
        finally { ModPolicies.Content = null; }
    }
}
