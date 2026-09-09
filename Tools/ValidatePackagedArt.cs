// Isolated smoke fixture for the project-owned TAR/LZ4 art path. It has no ResearchSources,
// recovered Android AssetBundles, or loose decoded sprite/audio payload tree.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Content;
using Eclipse.Modding;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
#endif

public static class ValidatePackagedArt
{
    private static int checks;

    private sealed class TestFighterOperations : IModFighterOperations
    {
        public readonly List<double> HealthChanges = new List<double>();
        public readonly List<double> MagicChanges = new List<double>();

        public bool TryChangeHealth(double amount, out string error)
        {
            HealthChanges.Add(amount);
            error = string.Empty;
            return true;
        }

        public bool TryAddMagicCharge(double amount, out string error)
        {
            MagicChanges.Add(amount);
            error = string.Empty;
            return true;
        }
    }

    private static void Require(bool value, string message)
    {
        if (!value) throw new InvalidDataException(message);
        checks++;
    }

    private static void RejectAsset(Action action, string message)
    {
        try { action(); }
        catch (InvalidDataException) { checks++; return; }
        throw new InvalidDataException(message);
    }

    private static void CheckPackagedBundles()
    {
        TextAsset manifest = Resources.Load<TextAsset>(PackagedArtCatalog.CatalogResourcePath);
        Require(manifest != null, "Packaged catalog missing");
        PackagedArtCatalog.Catalog catalog = PackagedArtCatalog.ReadCatalog(manifest.text);
        Resources.UnloadAsset(manifest);
        Require(catalog.version == 3, "Expected TAR/LZ4 catalog v3");

        int archives = catalog.bundles.Count(x => !string.IsNullOrEmpty(x.file));
        int fonts = catalog.bundles.Sum(x => x.assets.Count(a => !string.IsNullOrEmpty(a.font)));
        Require(catalog.bundles.Length == 95 && archives == 94 && fonts == 10,
            "Unexpected TAR/LZ4 catalog coverage");

        Sprite agnis = PackagedArtCatalog.Load<Sprite>("UI/Items/AgnisSeal.helm");
        Require(agnis != null && agnis.texture != null && agnis.rect.width > 0 && agnis.rect.height > 0,
            "AgnisSeal sprite lookup failed");
        Require(agnis.vertices.Length >= 3 && agnis.uv.Length == agnis.vertices.Length,
            "AgnisSeal sprite geometry invalid");

        string[] avatarSamples =
        {
            "UI/Users/avatar_human",
            "UI/Users/boss_wind_wolf",
            "UI/Users/boss_shurale_ny22",
            "UI/Users/man_glaive_3"
        };
        foreach (string address in avatarSamples)
        {
            Sprite avatar = PackagedArtCatalog.Load<Sprite>(address);
            Require(avatar != null && avatar.texture != null && avatar.pixelsPerUnit >= 200f &&
                avatar.vertices.Length >= 3 && avatar.uv.Length == avatar.vertices.Length &&
                avatar.uv.All(value => value.x >= 0f && value.x <= 1f && value.y >= 0f && value.y <= 1f),
                "Upscaled TAR avatar geometry invalid: " + address);
        }

        Texture2D texture = PackagedArtCatalog.Load<Texture2D>("UI/Items/AgnisSeal");
        Require(texture != null && texture.width > 0 && texture.height > 0,
            "AgnisSeal texture lookup failed");

        AudioClip audio = PackagedArtCatalog.Load<AudioClip>("gamedata/music/fight_independence_day");
        Require(audio != null && audio.samples > 0 && audio.channels > 0 && audio.frequency > 0,
            "TAR PCM audio lookup failed");

        Font font = PackagedArtCatalog.Load<Font>("UI/Fonts/notoserif-regular");
        Require(font != null && font.material != null, "Loose font lookup failed");

        Require(PackagedArtCatalog.Load<TextAsset>("gamedata/list") == null,
            "Art catalog overrides gameplay XML");

        Require(PackagedArtCatalog.ContainsExactAddress("UI/Items/AgnisSeal"),
            "Exact packaged address is not indexed");
        Require(!PackagedArtCatalog.ContainsExactAddress("UI/Items/AgnisSeal.helm"),
            "Exact-address API accepted a sprite-member compatibility alias");
        var coreProvider = new CoreAssetProvider();
        AssetMetadata coreAsset;
        Require(coreProvider.TryDescribe(AssetId.Parse("core:UI/Items/AgnisSeal"), out coreAsset) &&
            coreAsset.SourceKind == AssetSourceKind.Core,
            "CoreAssetProvider did not expose packaged logical address");
        Require(!coreProvider.TryDescribe(AssetId.Parse("core:UI/Items/does_not_exist"), out coreAsset),
            "CoreAssetProvider resolved an unknown packaged address");
        CheckModHost(coreProvider);
        CheckRepositoryEnchantmentSample();
        CheckLocalizationPatchLua();
        CheckModStateLua();

        string model = PackagedArtCatalog.LoadModelText("gamedata/models/mdl_skeleton");
        Require(!string.IsNullOrEmpty(model) && model.Contains("<Scene") && model.Contains("<Figures>"),
            "TAR model lookup failed");

        string atlas = PackagedArtCatalog.LoadLocationDataText("Textures/Locations/arena/arena_bg_xml.xml");
        Require(!string.IsNullOrEmpty(atlas) && atlas.Contains("<plist"),
            "TAR location atlas-data lookup failed");

        Sprite[] location = PackagedArtCatalog.LoadWithSubAssets<Sprite>("Textures/Locations/arena/arena_bg");
        Require(location != null && location.Any(x => x != null && x.texture != null),
            "TAR location-art lookup failed");

        PackagedArtCatalog.BundleRecord coreLocations = catalog.bundles.FirstOrDefault(x =>
            string.Equals(x.name, "CORE_LOCATIONS", StringComparison.OrdinalIgnoreCase));
        Require(coreLocations != null && string.Equals(coreLocations.file, "CORE_LOCATIONS.tar.lz4",
            StringComparison.OrdinalIgnoreCase), "CORE_LOCATIONS group missing");

        string[] moonAtlases =
        {
            "moon_bg", "moon_clouds", "moon_atlas_layer1", "moon_atlas_layer2",
            "moon_atlas_layer3", "moon_atlas_layer4"
        };
        foreach (string moonAtlas in moonAtlases)
        {
            string address = "Textures/Locations/moon/" + moonAtlas;
            Require(coreLocations.assets.Any(x => string.Equals(x.address, address,
                StringComparison.OrdinalIgnoreCase)), "Moon TAR atlas address missing: " + moonAtlas);
        }

        // OverrideGeometry is only supported reliably in play/player context. Keep editor
        // command-mode checks to safe full-rect sprites; the standalone smoke below walks
        // every CORE_LOCATIONS address and validates Moon's tight mesh inside Start().
        if (!Application.isEditor || Application.isPlaying)
        {
            foreach (string moonAtlas in moonAtlases)
            {
                Sprite[] moonSprites = PackagedArtCatalog.LoadWithSubAssets<Sprite>(
                    "Textures/Locations/moon/" + moonAtlas);
                Require(moonSprites != null && moonSprites.Any(x => x != null && x.texture != null),
                    "Moon TAR atlas missing: " + moonAtlas);
            }
            Sprite moonLayer3 = PackagedArtCatalog.Load<Sprite>("Textures/Locations/moon/layer3");
            Require(moonLayer3 != null && moonLayer3.texture != null && moonLayer3.vertices.Length == 98,
                "Moon layer3 exact-path/tight-mesh lookup failed");
        }
        Sprite moonBackground = PackagedArtCatalog.Load<Sprite>("Textures/Locations/moon/background_1");
        Sprite dojoBackground = PackagedArtCatalog.Load<Sprite>("Textures/Locations/dojo/background_1");
        Require(moonBackground != null && dojoBackground != null &&
            moonBackground.texture != dojoBackground.texture,
            "Moon background incorrectly resolved to Dojo by basename");

        Debug.Log("[PackagedArtTest] PASS " + (Application.isEditor ? "editor" : "standalone") +
            ": " + checks + " checks; " + catalog.bundles.Length + " groups; " + archives +
            " TAR/LZ4 archives; " + fonts + " loose fonts.");
    }

    private static void CheckRepositoryEnchantmentSample()
    {
        string modsRoot = Path.Combine(Application.dataPath, "TestMods");
        using (ModHost host = ModHost.Build(modsRoot))
        {
            Require(!host.HasErrors && host.EnabledMods.Count == 1 &&
                host.EnabledMods[0].Id.Value == "example.enchantment",
                "Tracked example.enchantment manifest/assets did not load cleanly");
            var logs = new List<ModLogEntry>();
            using (ModScriptSession scripts = host.StartScripts(new MoonSharpScriptRuntime(), entry => logs.Add(entry), content =>
            {
                var perksDocument = new XmlDocument();
                perksDocument.Load(Path.Combine(GameplayContentArchive.GetXmlRoot(), "perks.xml"));
                var perkNodes = new List<XmlNode>();
                foreach (XmlNode node in perksDocument.SelectNodes("/Perks/Perk")) perkNodes.Add(node);
                CoreContentImporter.ImportPerks(content, perkNodes);
            }))
            {
                Require(!scripts.HasErrors && scripts.ActiveMods.Count == 1,
                    "Tracked example.enchantment Lua entrypoint failed");
                ModBehaviorDefinition behavior = null;
                PerkDefinition perk = null;
                EnchantmentDefinition enchantment = null;
                Require(scripts.Content.TryGetBehavior(
                        DefinitionId.Parse("example.enchantment:behaviors/battle_charge"), out behavior) &&
                    scripts.Content.TryGetPerk(
                        DefinitionId.Parse("example.enchantment:perks/battle_focus"), out perk) &&
                    perk.HasBehavior && perk.Behavior == behavior.Id &&
                    perk.Description == DefinitionId.Parse(
                        "example.enchantment:localization/perk.battle_focus.description") &&
                    scripts.Content.TryGetEnchantment(
                        DefinitionId.Parse("example.enchantment:enchantments/battle_charge_weapon"),
                        out enchantment) && enchantment.HasBehavior && !enchantment.HasPerk &&
                    enchantment.Behavior == behavior.Id && enchantment.HasIcon &&
                    enchantment.Description == DefinitionId.Parse(
                        "example.enchantment:localization/enchantment.battle_charge.description") &&
                    Math.Abs(enchantment.InitialParameters["magic_charge"].Number - 0.35d) < 0.000001d &&
                    Math.Abs(enchantment.InitialParameters["health_bonus"].Number - 0.05d) < 0.000001d,
                    "Tracked example.enchantment did not register its API 0.3 reusable behavior/perk/enchantment");

                var operations = new TestFighterOperations();
                string error;
                Require(scripts.TryInvokeBehavior(behavior.Id, ModEffectEvent.FightBegin,
                        enchantment.InitialParameters,
                        new Dictionary<string, string>(StringComparer.Ordinal)
                        {
                            { "side", "player" },
                            { "source", "enchantment" },
                            { "enchantment_id", enchantment.Id.ToString() },
                        }, operations, out error) && string.IsNullOrEmpty(error) &&
                    operations.MagicChanges.Count == 1 &&
                    Math.Abs(operations.MagicChanges[0] - 0.35d) < 0.000001d &&
                    operations.HealthChanges.Count == 1 &&
                    Math.Abs(operations.HealthChanges[0] - 0.05d) < 0.000001d &&
                    logs.Any(x => x.ModId.Value == "example.enchantment" &&
                        x.Message.Contains("battle_charge activated")),
                    "Tracked example.enchantment did not execute its custom fighter operations: " + error);
            }
        }
    }

    private static void CheckLocalizationPatchLua()
    {
        string modsRoot = Path.Combine(Application.temporaryCachePath,
            "sf2de-localization-patch-" + Guid.NewGuid().ToString("N"));
        try
        {
            string modRoot = Path.Combine(modsRoot, "patch.localization");
            Directory.CreateDirectory(Path.Combine(modRoot, "scripts"));
            File.WriteAllText(Path.Combine(modRoot, "scripts", "main.lua"),
                "local sf2 = require(\"sf2\")\n" +
                "sf2.localization.patch { target=\"core:localization/weapon_nunchaku\", language=\"eng\", value=\"Nunchaku\" }\n");
            File.WriteAllText(Path.Combine(modRoot, "mod.toml"),
                "schema = 1\n" +
                "id = \"patch.localization\"\n" +
                "name = \"Localization Patch Fixture\"\n" +
                "version = \"1.0.0\"\n" +
                "api = \">=0.4 <1.0\"\n" +
                "authors = [\"Test\"]\n" +
                "entrypoint = \"scripts/main.lua\"\n" +
                "capabilities = [\"content.patch\"]\n\n" +
                "[[dependencies]]\n" +
                "id = \"core\"\n" +
                "version = \">=1.0 <2.0\"\n");

            using (ModHost host = ModHost.Build(modsRoot))
            using (ModScriptSession scripts = host.StartScripts(new MoonSharpScriptRuntime(), null, content =>
            {
                var list = new XmlDocument();
                list.Load(Path.Combine(GameplayContentArchive.GetXmlRoot(), "list.xml"));
                XmlNode nunchaku = list.SelectSingleNode(
                    "/List/Items/Item[@Type='Weapon' and @Name='WEAPON_NUNCHAKU']");
                CoreContentImporter.ImportWeapons(content, new[] { nunchaku },
                    CoreContentImporter.ReadLocalizations(
                        Path.Combine(GameplayContentArchive.GetXmlRoot(), "localizations")));
            }))
            {
                Require(!host.HasErrors && !scripts.HasErrors && scripts.ActiveMods.Count == 1,
                    "Lua localization patch fixture failed to initialize");
                LocalizationDefinition patched;
                Require(scripts.Content.Patches.Count == 1 && scripts.Content.TryGetLocalization(
                        DefinitionId.Parse("core:localization/weapon_nunchaku"), out patched) &&
                    patched.GetOrEnglish("eng") == "Nunchaku",
                    "sf2.localization.patch did not commit through the public Lua API");

                LocalizationManager.ResetModdingTestLanguage("eng");
                LocalizationManager.SetBaseStringForTest("WEAPON_NUNCHAKU", "Nunchacku");
                using (var adapter = new LegacyContentAdapter(scripts.Content))
                {
                    adapter.ApplyLocalization();
                    Require(LocalizationManager.GetStringForTest("WEAPON_NUNCHAKU") == "Nunchaku",
                        "Core localization patch did not bind to the recovered runtime key");
                }
                Require(LocalizationManager.GetExternalStringForTest("WEAPON_NUNCHAKU") == null &&
                    LocalizationManager.GetStringForTest("WEAPON_NUNCHAKU") == "Nunchacku",
                    "Removing the patch overlay did not reveal the canonical base localization again");
            }

            string conflictRoot = Path.Combine(modsRoot, "patch.localization.conflict");
            Directory.CreateDirectory(Path.Combine(conflictRoot, "scripts"));
            File.WriteAllText(Path.Combine(conflictRoot, "scripts", "main.lua"),
                "local sf2 = require(\"sf2\")\n" +
                "sf2.localization.patch { target=\"core:localization/weapon_nunchaku\", language=\"eng\", value=\"Forbidden last writer\" }\n");
            File.WriteAllText(Path.Combine(conflictRoot, "mod.toml"),
                "schema = 1\n" +
                "id = \"patch.localization.conflict\"\n" +
                "name = \"Localization Conflict Fixture\"\n" +
                "version = \"1.0.0\"\n" +
                "api = \">=0.4 <1.0\"\n" +
                "authors = [\"Test\"]\n" +
                "entrypoint = \"scripts/main.lua\"\n" +
                "capabilities = [\"content.patch\"]\n\n" +
                "[[dependencies]]\n" +
                "id = \"core\"\n" +
                "version = \">=1.0 <2.0\"\n\n" +
                "[[dependencies]]\n" +
                "id = \"patch.localization\"\n" +
                "version = \">=1.0 <2.0\"\n");

            using (ModHost host = ModHost.Build(modsRoot))
            using (ModScriptSession scripts = host.StartScripts(new MoonSharpScriptRuntime(), null, content =>
            {
                var list = new XmlDocument();
                list.Load(Path.Combine(GameplayContentArchive.GetXmlRoot(), "list.xml"));
                XmlNode nunchaku = list.SelectSingleNode(
                    "/List/Items/Item[@Type='Weapon' and @Name='WEAPON_NUNCHAKU']");
                CoreContentImporter.ImportWeapons(content, new[] { nunchaku },
                    CoreContentImporter.ReadLocalizations(
                        Path.Combine(GameplayContentArchive.GetXmlRoot(), "localizations")));
            }))
            {
                LocalizationDefinition patched;
                Require(scripts.HasErrors && scripts.ActiveMods.Count == 1 &&
                    scripts.ActiveMods[0].Id.Value == "patch.localization" &&
                    scripts.Content.TryGetLocalization(DefinitionId.Parse("core:localization/weapon_nunchaku"),
                        out patched) && patched.GetOrEnglish("eng") == "Nunchaku",
                    "Overlapping Lua patches behaved like last-mod-wins or disabled the valid owner");
                Require(scripts.Diagnostics.Any(x => x.Code == "SCRIPT001" &&
                    x.Source == "patch.localization.conflict" && x.Message.Contains("values/eng") &&
                    x.Message.Contains("patch.localization") && x.Message.Contains("patch.localization.conflict")),
                    "Overlapping Lua patch did not emit a deterministic owner/field conflict diagnostic");
            }
        }
        finally
        {
            if (Directory.Exists(modsRoot)) Directory.Delete(modsRoot, true);
        }
    }

    private static void CheckModStateLua()
    {
        string modsRoot = Path.Combine(Application.temporaryCachePath,
            "sf2de-state-" + Guid.NewGuid().ToString("N"));
        string modRoot = Path.Combine(modsRoot, "state.fixture");
        try
        {
            Directory.CreateDirectory(Path.Combine(modRoot, "scripts"));
            WriteStateFixture(modRoot, "1.0.0",
                "sf2.state.register {\n" +
                "  version = 1,\n" +
                "  fields = { fights = { type=sf2.state.INTEGER, required=true, default=0 } },\n" +
                "}\n" +
                "sf2.behaviors.register {\n" +
                "  id = \"state_counter\",\n" +
                "  on_fight_begin = function(parameters, fighter)\n" +
                "    local fights = sf2.state.get(\"fights\")\n" +
                "    sf2.state.set { fights = fights + 1 }\n" +
                "  end,\n" +
                "}\n");

            var save = new XmlDocument();
            save.LoadXml("<Warrior />");
            using (ModHost host = ModHost.Build(modsRoot))
            using (ModScriptSession scripts = host.StartScripts(new MoonSharpScriptRuntime()))
            {
                Require(!host.HasErrors && !scripts.HasErrors && scripts.ActiveMods.Count == 1 &&
                    scripts.State.Definitions.Count == 1,
                    "Lua state v1 fixture failed to register its typed schema");
                Require(ModSaveData.RecordContext(save.DocumentElement, scripts.ActiveMods, scripts.Content),
                    "Lua state fixture could not create EclipseMods ownership context");
                Require(scripts.BindState(save.DocumentElement).Count == 0,
                    "Lua state v1 fixture failed to bind to the player save");
                string error;
                Require(scripts.TryInvokeBehavior(DefinitionId.Parse("state.fixture:behaviors/state_counter"),
                        ModEffectEvent.FightBegin,
                        new Dictionary<string, ModParameterValue>(StringComparer.Ordinal),
                        new Dictionary<string, string>(StringComparer.Ordinal), out error) && string.IsNullOrEmpty(error),
                    "State-backed Lua behavior could not write bound state: " + error);
                ModParameterValue fights;
                Require(scripts.State.TryGetValue(ModId.Parse("state.fixture"), "fights", out fights) &&
                    fights.Integer == 1,
                    "sf2.state.get/set did not round-trip typed integer state");
                XmlElement state = (XmlElement)save.SelectSingleNode(
                    "/Warrior/EclipseMods/Mod[@id='state.fixture']/State");
                Require(state != null && state.GetAttribute("format") == "1" && state.GetAttribute("version") == "1" &&
                    state.SelectSingleNode("Value[@name='fights' and @type='integer' and @value='1']") != null,
                    "Lua state write did not commit to the namespaced save node");
            }

            WriteStateFixture(modRoot, "2.0.0",
                "sf2.state.register {\n" +
                "  version = 2,\n" +
                "  fields = {\n" +
                "    fights = { type=sf2.state.INTEGER, required=true, default=0 },\n" +
                "    migrated = { type=sf2.state.BOOLEAN, required=true, default=false },\n" +
                "  },\n" +
                "  migrations = {\n" +
                "    [1] = function(state) state.migrated = true end,\n" +
                "  },\n" +
                "}\n");
            using (ModHost host = ModHost.Build(modsRoot))
            using (ModScriptSession scripts = host.StartScripts(new MoonSharpScriptRuntime()))
            {
                Require(!host.HasErrors && !scripts.HasErrors && scripts.ActiveMods.Count == 1,
                    "Lua state v2 fixture failed to initialize");
                Require(ModSaveData.RecordContext(save.DocumentElement, scripts.ActiveMods, scripts.Content),
                    "Lua state v2 fixture could not refresh ownership context");
                Require(scripts.BindState(save.DocumentElement).Count == 0,
                    "Lua v1 -> v2 state migration failed");
                ModParameterValue fights;
                ModParameterValue migrated;
                Require(scripts.State.TryGetValue(ModId.Parse("state.fixture"), "fights", out fights) &&
                    fights.Integer == 1 &&
                    scripts.State.TryGetValue(ModId.Parse("state.fixture"), "migrated", out migrated) && migrated.Boolean,
                    "Lua migration did not preserve old values and add the new typed field");
                XmlElement state = (XmlElement)save.SelectSingleNode(
                    "/Warrior/EclipseMods/Mod[@id='state.fixture']/State");
                Require(state != null && state.GetAttribute("version") == "2" &&
                    state.SelectSingleNode("Value[@name='migrated' and @type='boolean' and @value='1']") != null,
                    "Successful Lua migration did not commit schema 2 state");
            }

            XmlElement beforeFailure = (XmlElement)save.SelectSingleNode(
                "/Warrior/EclipseMods/Mod[@id='state.fixture']/State");
            string exactBeforeFailure = beforeFailure.OuterXml;
            WriteStateFixture(modRoot, "3.0.0",
                "sf2.state.register {\n" +
                "  version = 3,\n" +
                "  fields = {\n" +
                "    fights = { type=sf2.state.INTEGER, required=true, default=0 },\n" +
                "    migrated = { type=sf2.state.BOOLEAN, required=true, default=false },\n" +
                "  },\n" +
                "  migrations = {\n" +
                "    [2] = function(state) error(\"intentional-state-migration-failure\") end,\n" +
                "  },\n" +
                "}\n");
            using (ModHost host = ModHost.Build(modsRoot))
            using (ModScriptSession scripts = host.StartScripts(new MoonSharpScriptRuntime()))
            {
                Require(!host.HasErrors && !scripts.HasErrors && scripts.ActiveMods.Count == 1,
                    "Lua state v3 failure fixture failed before migration could be tested");
                Require(ModSaveData.RecordContext(save.DocumentElement, scripts.ActiveMods, scripts.Content),
                    "Lua state v3 fixture could not refresh ownership context");
                IReadOnlyList<ModDiagnostic> diagnostics = scripts.BindState(save.DocumentElement);
                Require(diagnostics.Count == 1 && diagnostics[0].Code == "STATE003" &&
                    diagnostics[0].Message.Contains("intentional-state-migration-failure"),
                    "Failing Lua state migration did not produce the expected state diagnostic");
                XmlElement afterFailure = (XmlElement)save.SelectSingleNode(
                    "/Warrior/EclipseMods/Mod[@id='state.fixture']/State");
                Require(afterFailure != null && afterFailure.OuterXml == exactBeforeFailure,
                    "Failing Lua state migration partially rewrote the saved state");
            }
        }
        finally
        {
            if (Directory.Exists(modsRoot)) Directory.Delete(modsRoot, true);
        }
    }

    private static void WriteStateFixture(string modRoot, string version, string body)
    {
        File.WriteAllText(Path.Combine(modRoot, "scripts", "main.lua"),
            "local sf2 = require(\"sf2\")\n" + body);
        File.WriteAllText(Path.Combine(modRoot, "mod.toml"),
            "schema = 1\n" +
            "id = \"state.fixture\"\n" +
            "name = \"State Fixture\"\n" +
            "version = \"" + version + "\"\n" +
            "api = \">=0.5 <1.0\"\n" +
            "authors = [\"Test\"]\n" +
            "entrypoint = \"scripts/main.lua\"\n" +
            "capabilities = [\"content.register\", \"state.read\", \"state.write\"]\n\n" +
            "[[dependencies]]\n" +
            "id = \"core\"\n" +
            "version = \">=1.0 <2.0\"\n");
    }

    private static void CheckModHost(CoreAssetProvider coreProvider)
    {
        string modsRoot = Path.Combine(Application.temporaryCachePath,
            "sf2de-modhost-" + Guid.NewGuid().ToString("N"));
        try
        {
            string example = Path.Combine(modsRoot, "example.weapon");
            Directory.CreateDirectory(Path.Combine(example, "assets", "sprites"));
            Directory.CreateDirectory(Path.Combine(example, "assets", "textures"));
            Directory.CreateDirectory(Path.Combine(example, "assets", "models"));
            Directory.CreateDirectory(Path.Combine(example, "assets", "audio"));
            File.WriteAllBytes(Path.Combine(example, "assets", "textures", "weapon.png"), CreateTestPng());
            File.WriteAllText(Path.Combine(example, "assets", "sprites", "weapon.asset"),
                "type=sprite\ntexture=textures/weapon.png\n" +
                "pivot = [0.25, 0.75]\n" +
                "pixels_per_unit = 50\n" +
                "filter = \"point\"\n" +
                "wrap = \"clamp\"\n");
            File.WriteAllText(Path.Combine(example, "assets", "sprites", "crop.asset"),
                "type=\"sprite\"\ntexture=\"textures/weapon.png\"\nrect=[1, 0, 1, 2]\n" +
                "pivot=[0, 1]\npixels_per_unit=25\nfilter=\"point\"\n");
            File.WriteAllText(Path.Combine(example, "assets", "sprites", "smooth.asset"),
                "\uFEFFtype=sprite\ntexture=textures/weapon.png\n");
            File.WriteAllText(Path.Combine(example, "assets", "sprites", "repeat.asset"),
                "type=sprite\ntexture=textures/weapon.png\nwrap=\"repeat\"\nmipmaps=true\n");
            File.WriteAllBytes(Path.Combine(example, "assets", "sprites", "legacy.png"), CreateTestPng());
            File.WriteAllText(Path.Combine(example, "assets", "sprites", "legacy.sprite.toml"),
                "pivot=[0.25, 0.75]\npixels_per_unit=50\nfilter=\"point\"\n");
            string[] invalidSprites = {
                "type=sprite\n", // Missing texture.
                "type=sprite\ntexture=../weapon.png\n",
                "type=sprite\ntexture=/textures/weapon.png\n",
                "type=sprite\ntexture=other.mod:textures/weapon.png\n",
                "type=sprite\ntexture=C:/weapon.png\n",
                "type=sprite\ntexture=textures/missing.png\n",
                "type=sprite\ntexture=textures/weapon.jpg\n",
                "type=sprite\ntexture=.png\n",
                "type=sprite\ntexture=textures/.png\n",
                "type=sprite\ntexture=sprites/legacy.png\n", // A sprite ID is not a texture ID.
                "type=sprite\ntexture=textures/weapon.png\nrect=[0,0,3,2]\n",
                "type=sprite\ntexture=textures/weapon.png\nborder=[2,0,2,0]\n",
                "type=sprite\ntexture=textures/weapon.png\npixels_per_unit=0\n",
                "type=sprite\ntexture=textures/weapon.png\npivot=[NaN,0]\n",
                "type=sprite\ntexture=textures/weapon.png\nnamespace=other.mod\n",
                "type=sprite\ntexture=textures/weapon.png\naddress=other\n",
                "type=sprite\ntexture=textures/weapon.png\nname=other\n",
            };
            for (int i = 0; i < invalidSprites.Length; i++)
                File.WriteAllText(Path.Combine(example, "assets", "sprites", "invalid_" + i + ".asset"), invalidSprites[i]);
            File.WriteAllText(Path.Combine(example, "assets", "models", "mdl_weapon_example.xml"),
                "<Scene><Figures /></Scene>");
            File.WriteAllBytes(Path.Combine(example, "assets", "audio", "test.wav"), CreateTestWav());
            Directory.CreateDirectory(Path.Combine(example, "localizations"));
            File.WriteAllText(Path.Combine(example, "localizations", "eng.toml"),
                "weapon.example_blade = \"Example Blade\"\n");
            File.WriteAllText(Path.Combine(example, "localizations", "pol.toml"),
                "weapon.example_blade = \"Przykladowe Ostrze\"\n");
            Directory.CreateDirectory(Path.Combine(example, "scripts"));
            File.WriteAllText(Path.Combine(example, "scripts", "helper.lua"),
                "return { value = \"helper-ok\" }\n");
            File.WriteAllText(Path.Combine(example, "scripts", "main.lua"),
                "local sf2 = require(\"sf2\")\n" +
                "assert(io == nil and os == nil and debug == nil and loadfile == nil and dofile == nil)\n" +
                "assert(sf2.mod.id == \"example.weapon\")\n" +
                "assert(sf2.assets.qualify(\"sprites/weapon\") == \"example.weapon:sprites/weapon\")\n" +
                "assert(sf2.assets.exists(\"sprites/weapon\"))\n" +
                "local helper = require(\"helper\")\n" +
                "assert(helper.value == \"helper-ok\")\n" +
                "local title = sf2.localization.key(\"weapon.example_blade\")\n" +
                "local weapon = sf2.items.register_weapon {\n" +
                "  id = \"example_blade\",\n" +
                "  display_name = title,\n" +
                "  icon = sf2.assets.sprite(\"sprites/weapon\"),\n" +
                "  model = sf2.assets.model(\"models/mdl_weapon_example\"),\n" +
                "}\n" +
                "sf2.items.alias { from = \"weapon/example_blade_legacy\", to = weapon }\n" +
                "sf2.items.tombstone { id = \"weapon/example_blade_retired\" }\n" +
                "assert(sf2.shop.add == sf2.shop.addItem)\n" +
                "assert(sf2.mod.log == sf2.log.info and sf2.mod.warn == sf2.log.warn and sf2.mod.error == sf2.log.error)\n" +
                "sf2.shop.addItem {\n" +
                "  section = sf2.shop.WEAPONS,\n" +
                "  item = weapon,\n" +
                "  level = 12,\n" +
                "  price = sf2.price.coins(1000),\n" +
                "}\n" +
                "sf2.log.info(\"lua-entry-ok\")\n" +
                "sf2.log.debug(\"lua-debug-ok\")\n" +
                "sf2.log.warn(\"lua-warn-ok\")\n" +
                "sf2.log.error(\"lua-error-ok\")\n" +
                "sf2.mod.log(\"lua-legacy-ok\")\n");
            File.WriteAllText(Path.Combine(example, "mod.toml"),
                "schema = 1\n" +
                "id = \"example.weapon\"\n" +
                "name = \"Example Weapon\"\n" +
                "version = \"1.0.0\"\n" +
                "api = \">=0.1 <1.0\"\n" +
                "authors = [\"Test\"]\n" +
                "entrypoint = \"scripts/main.lua\"\n" +
                "capabilities = [\"content.register\"]\n\n" +
                "[[dependencies]]\n" +
                "id = \"core\"\n" +
                "version = \">=1.0 <2.0\"\n");

            string loadout = Path.Combine(modsRoot, "example.loadout");
            Directory.CreateDirectory(Path.Combine(loadout, "localizations"));
            Directory.CreateDirectory(Path.Combine(loadout, "scripts"));
            File.WriteAllText(Path.Combine(loadout, "localizations", "eng.toml"),
                "armor.eclipse_mantle = \"Eclipse Mantle\"\n" +
                "helm.eclipse_pumpkin = \"Eclipse Pumpkin\"\n" +
                "ranged.eclipse_skull = \"Eclipse Skull\"\n" +
                "magic.eclipse_asteroid = \"Eclipse Asteroid\"\n");
            File.WriteAllText(Path.Combine(loadout, "scripts", "main.lua"),
                "local sf2 = require(\"sf2\")\n" +
                "local armor = sf2.items.register_armor { id=\"eclipse_mantle\", display_name=sf2.localization.key(\"armor.eclipse_mantle\"), icon=sf2.assets.sprite(\"core:UI/Items/Armor12.img_armor_mantle_of_night\"), model=sf2.assets.model(\"core:gamedata/models/mdl_armor_mantle_of_night\") }\n" +
                "local helm = sf2.items.register_helm { id=\"eclipse_pumpkin\", display_name=sf2.localization.key(\"helm.eclipse_pumpkin\"), icon=sf2.assets.sprite(\"core:UI/Items/Helm31.img_helm_hw14_pumpkin\"), model=sf2.assets.model(\"core:gamedata/models/mdl_helm_hw14_pumpkin\") }\n" +
                "local ranged = sf2.items.register_ranged { id=\"eclipse_skull\", display_name=sf2.localization.key(\"ranged.eclipse_skull\"), icon=sf2.assets.sprite(\"core:UI/Items/Ranged12.img_ranged_hw15_skull\"), model=sf2.assets.model(\"core:gamedata/models/mdl_ranged_hw15_skull\"), subtype=\"Skull\" }\n" +
                "local magic = sf2.items.register_magic { id=\"eclipse_asteroid\", display_name=sf2.localization.key(\"magic.eclipse_asteroid\"), icon=sf2.assets.sprite(\"core:UI/Items/Magic4.img_magic_asteroid\"), model=sf2.assets.model(\"core:gamedata/models/mdl_magic_asteroid\"), subtype=\"MagicAsteroid\" }\n" +
                "sf2.shop.addItem { section=sf2.shop.ARMOR, item=armor, level=2, price=sf2.price.coins(1) }\n" +
                "sf2.shop.addItem { section=sf2.shop.HELMETS, item=helm, level=2, price=sf2.price.coins(1) }\n" +
                "sf2.shop.addItem { section=sf2.shop.RANGED, item=ranged, level=6, price=sf2.price.coins(1) }\n" +
                "sf2.shop.addItem { section=sf2.shop.MAGIC, item=magic, level=6, price=sf2.price.coins(1) }\n" +
                "sf2.log.info(\"loadout-entry-ok\")\n");
            File.WriteAllText(Path.Combine(loadout, "mod.toml"),
                "schema = 1\n" +
                "id = \"example.loadout\"\n" +
                "name = \"Example Loadout\"\n" +
                "version = \"1.0.0\"\n" +
                "api = \">=0.1 <1.0\"\n" +
                "authors = [\"Test\"]\n" +
                "entrypoint = \"scripts/main.lua\"\n" +
                "capabilities = [\"content.register\"]\n\n" +
                "[[dependencies]]\n" +
                "id = \"core\"\n" +
                "version = \">=1.0 <2.0\"\n");

            string enchantment = Path.Combine(modsRoot, "example.enchantment");
            Directory.CreateDirectory(Path.Combine(enchantment, "localizations"));
            Directory.CreateDirectory(Path.Combine(enchantment, "scripts"));
            File.WriteAllText(Path.Combine(enchantment, "localizations", "eng.toml"),
                "perk.eclipse_lifesteal = \"Eclipse Behavior Perk\"\n" +
                "perk.eclipse_lifesteal.description = \"Behavior-backed perk with independent typed defaults.\"\n" +
                "enchantment.eclipse_lifesteal = \"Eclipse Behavior Enchantment\"\n" +
                "enchantment.eclipse_lifesteal.description = \"Direct behavior-backed enchantment with independent state.\"\n" +
                "perk.legacy_lifesteal = \"Legacy Lifesteal Perk\"\n" +
                "perk.legacy_lifesteal.description = \"API 0.2 template compatibility test.\"\n");
            File.WriteAllText(Path.Combine(enchantment, "scripts", "main.lua"),
                "local sf2 = require(\"sf2\")\n" +
                "local behavior = sf2.behaviors.register {\n" +
                "  id = \"lifesteal\",\n" +
                "  parameters = {\n" +
                "    chance = sf2.behaviors.NUMBER,\n" +
                "    stacks = { type = sf2.behaviors.INTEGER, required = false, default = 1 },\n" +
                "  },\n" +
                "  on_fight_begin = function(parameters, fighter)\n" +
                "    if fighter.phase == \"initial\" then\n" +
                "      if parameters.chance ~= 0.65 or parameters.stacks ~= 1 then error(\"initial typed behavior mismatch\") end\n" +
                "    elseif fighter.phase == \"saved\" then\n" +
                "      if parameters.chance ~= 0.8 or parameters.stacks ~= 2 then error(\"saved typed behavior mismatch\") end\n" +
                "    elseif fighter.phase == \"perk_saved\" then\n" +
                "      if parameters.chance ~= 0.3 or parameters.stacks ~= 4 then error(\"saved perk typed behavior mismatch\") end\n" +
                "    elseif fighter.phase == \"interactive\" then\n" +
                "      if parameters.chance ~= 0.65 or parameters.stacks ~= 1 then error(\"interactive typed behavior mismatch\") end\n" +
                "      fighter:add_magic_charge(parameters.chance)\n" +
                "      fighter:change_health(parameters.stacks / 100)\n" +
                "    else\n" +
                "      error(\"unknown behavior test phase\")\n" +
                "    end\n" +
                "    sf2.log.info(\"behavior-fight-begin-\" .. fighter.phase)\n" +
                "  end,\n" +
                "}\n" +
                "local perk = sf2.perks.register {\n" +
                "  id = \"eclipse_lifesteal\",\n" +
                "  behavior = behavior,\n" +
                "  kind = sf2.perks.SINGLE,\n" +
                "  display_name = sf2.localization.key(\"perk.eclipse_lifesteal\"),\n" +
                "  description = sf2.localization.key(\"perk.eclipse_lifesteal.description\"),\n" +
                "  parameters = { chance = 0.25 },\n" +
                "}\n" +
                "sf2.enchantments.register {\n" +
                "  id = \"eclipse_lifesteal_weapon\",\n" +
                "  behavior = behavior,\n" +
                "  display_name = sf2.localization.key(\"enchantment.eclipse_lifesteal\"),\n" +
                "  description = sf2.localization.key(\"enchantment.eclipse_lifesteal.description\"),\n" +
                "  recipe = sf2.enchantments.MEDIUM,\n" +
                "  item_types = { sf2.enchantments.WEAPON },\n" +
                "  parameters = { chance = 0.65 },\n" +
                "}\n" +
                "local legacy_perk = sf2.perks.register {\n" +
                "  id = \"legacy_lifesteal\",\n" +
                "  template = sf2.perks.get(\"core:perks/PERK_ITEM_SPECIAL_LIFESTEAL_WEAPON\"),\n" +
                "  display_name = sf2.localization.key(\"perk.legacy_lifesteal\"),\n" +
                "  description = sf2.localization.key(\"perk.legacy_lifesteal.description\"),\n" +
                "  parameters = { Chance = 0.2 },\n" +
                "}\n" +
                "sf2.enchantments.register {\n" +
                "  id = \"legacy_lifesteal_weapon\",\n" +
                "  perk = legacy_perk,\n" +
                "  recipe = sf2.enchantments.MEDIUM,\n" +
                "  item_types = { sf2.enchantments.WEAPON },\n" +
                "}\n" +
                "sf2.log.info(\"enchantment-entry-ok\")\n");
            File.WriteAllText(Path.Combine(enchantment, "mod.toml"),
                "schema = 1\n" +
                "id = \"example.enchantment\"\n" +
                "name = \"Example Enchantment\"\n" +
                "version = \"1.0.0\"\n" +
                "api = \">=0.3 <1.0\"\n" +
                "authors = [\"Test\"]\n" +
                "entrypoint = \"scripts/main.lua\"\n" +
                "capabilities = [\"content.register\", \"combat.change_life\", \"combat.magic_charge\"]\n\n" +
                "[[dependencies]]\n" +
                "id = \"core\"\n" +
                "version = \">=1.0 <2.0\"\n");

            string broken = Path.Combine(modsRoot, "broken.mod");
            Directory.CreateDirectory(broken);
            File.WriteAllText(Path.Combine(broken, "mod.toml"),
                "schema = 1\n" +
                "id = \"broken.mod\"\n" +
                "name = \"Broken\"\n" +
                "version = \"1.0.0\"\n" +
                "api = \">=0.1 <1.0\"\n" +
                "authors = [\"Test\"]\n" +
                "entrypoint = \"scripts/main.lua\"\n" +
                "capabilities = [\"content.register\"]\n\n" +
                "[[dependencies]]\n" +
                "id = \"missing.mod\"\n" +
                "version = \">=1.0 <2.0\"\n");

            string scriptFailure = Path.Combine(modsRoot, "script.failure");
            Directory.CreateDirectory(Path.Combine(scriptFailure, "scripts"));
            File.WriteAllText(Path.Combine(scriptFailure, "scripts", "main.lua"),
                "require(\"../escape\")\n");
            File.WriteAllText(Path.Combine(scriptFailure, "mod.toml"),
                "schema = 1\n" +
                "id = \"script.failure\"\n" +
                "name = \"Script Failure\"\n" +
                "version = \"1.0.0\"\n" +
                "api = \">=0.1 <1.0\"\n" +
                "authors = [\"Test\"]\n" +
                "entrypoint = \"scripts/main.lua\"\n" +
                "capabilities = [\"content.register\"]\n\n" +
                "[[dependencies]]\n" +
                "id = \"core\"\n" +
                "version = \">=1.0 <2.0\"\n");

            string scriptRunaway = Path.Combine(modsRoot, "script.runaway");
            Directory.CreateDirectory(Path.Combine(scriptRunaway, "scripts"));
            File.WriteAllText(Path.Combine(scriptRunaway, "scripts", "main.lua"),
                "while true do end\n");
            File.WriteAllText(Path.Combine(scriptRunaway, "mod.toml"),
                "schema = 1\n" +
                "id = \"script.runaway\"\n" +
                "name = \"Script Runaway\"\n" +
                "version = \"1.0.0\"\n" +
                "api = \">=0.1 <1.0\"\n" +
                "authors = [\"Test\"]\n" +
                "entrypoint = \"scripts/main.lua\"\n" +
                "capabilities = [\"content.register\"]\n\n" +
                "[[dependencies]]\n" +
                "id = \"core\"\n" +
                "version = \">=1.0 <2.0\"\n");

            string registrationFailure = Path.Combine(modsRoot, "registration.failure");
            Directory.CreateDirectory(Path.Combine(registrationFailure, "assets", "sprites"));
            Directory.CreateDirectory(Path.Combine(registrationFailure, "assets", "models"));
            Directory.CreateDirectory(Path.Combine(registrationFailure, "localizations"));
            Directory.CreateDirectory(Path.Combine(registrationFailure, "scripts"));
            File.WriteAllBytes(Path.Combine(registrationFailure, "assets", "sprites", "weapon.png"), CreateTestPng());
            File.WriteAllText(Path.Combine(registrationFailure, "assets", "models", "weapon.xml"),
                "<Scene><Figures /></Scene>");
            File.WriteAllText(Path.Combine(registrationFailure, "localizations", "eng.toml"),
                "weapon.rollback = \"Must Roll Back\"\n");
            File.WriteAllText(Path.Combine(registrationFailure, "scripts", "main.lua"),
                "local sf2 = require(\"sf2\")\n" +
                "sf2.items.register_weapon {\n" +
                "  id = \"rollback\",\n" +
                "  display_name = sf2.localization.key(\"weapon.rollback\"),\n" +
                "  icon = sf2.assets.sprite(\"sprites/weapon\"),\n" +
                "  model = sf2.assets.model(\"models/weapon\"),\n" +
                "}\n" +
                "error(\"rollback-after-registration\")\n");
            File.WriteAllText(Path.Combine(registrationFailure, "mod.toml"),
                "schema = 1\n" +
                "id = \"registration.failure\"\n" +
                "name = \"Registration Failure\"\n" +
                "version = \"1.0.0\"\n" +
                "api = \">=0.1 <1.0\"\n" +
                "authors = [\"Test\"]\n" +
                "entrypoint = \"scripts/main.lua\"\n" +
                "capabilities = [\"content.register\"]\n\n" +
                "[[dependencies]]\n" +
                "id = \"core\"\n" +
                "version = \">=1.0 <2.0\"\n");

            ModHost host = ModHost.Build(modsRoot);
            Require(host.HasErrors, "Broken loose mod did not surface diagnostics");
            Require(host.EnabledMods.Count == 6 &&
                host.EnabledMods.Any(x => x.Id.Value == "example.weapon") &&
                host.EnabledMods.Any(x => x.Id.Value == "example.loadout") &&
                host.EnabledMods.Any(x => x.Id.Value == "example.enchantment") &&
                host.EnabledMods.Any(x => x.Id.Value == "registration.failure") &&
                host.EnabledMods.Any(x => x.Id.Value == "script.failure") &&
                host.EnabledMods.Any(x => x.Id.Value == "script.runaway"),
                "Dependency-invalid mod affected independently mountable mods");
            AssetMetadata metadata;
            Require(host.Assets.TryDescribe(AssetId.Parse("example.weapon:sprites/weapon"), out metadata) &&
                metadata.Kind == AssetKind.Sprite, "ModHost did not mount loose sprite");
            AssetBytes bytes;
            Require(host.Assets.TryRead(AssetId.Parse("example.weapon:sprites/weapon"), out bytes) &&
                bytes.Data.Length > 4, "ModHost did not read loose sprite bytes");
            Require(host.Assets.TryDescribe(AssetId.Parse("core:UI/Items/AgnisSeal"), out metadata),
                "ModHost did not mount core provider");
            Require(host.Assets.TryDescribe(
                    AssetId.Parse("core:UI/Items/Armor12.img_armor_mantle_of_night"), out metadata) &&
                metadata.Kind == AssetKind.Sprite,
                "Core provider did not expose a real atlas member as a typed sprite identity");
            Require(!host.Assets.TryDescribe(
                    AssetId.Parse("core:UI/Items/Armor12.this_member_does_not_exist"), out metadata),
                "Core provider accepted a nonexistent atlas member");
            Require(host.FormatReport().Contains("DEP005"), "ModHost report omitted dependency diagnostic");

            ModDescriptor exampleMod = host.EnabledMods.First(x => x.Id.Value == "example.weapon");
            var policyApi = new ModApiFacade(exampleMod, host.Assets, null);
            Require(policyApi.QualifyAsset("core:UI/Items/AgnisSeal").Namespace.Value == "core",
                "Declared core dependency was rejected by cross-namespace asset policy");
            bool undeclaredNamespaceRejected = false;
            try { policyApi.QualifyAsset("script.failure:scripts/main"); }
            catch (InvalidOperationException) { undeclaredNamespaceRejected = true; }
            Require(undeclaredNamespaceRejected,
                "Mod API allowed a cross-namespace asset reference without a declared dependency");

            Sprite looseSprite = host.TypedAssets.LoadSprite(AssetId.Parse("example.weapon:sprites/weapon"));
            Require(looseSprite != null && looseSprite.texture != null && looseSprite.texture.width == 2 &&
                looseSprite.texture.height == 2 && looseSprite.texture.filterMode == FilterMode.Point,
                "Typed loose sprite decode failed");
            Require(Mathf.Abs(looseSprite.pixelsPerUnit - 50f) < 0.001f &&
                (looseSprite.pivot - new Vector2(0.5f, 1.5f)).sqrMagnitude < 0.0001f,
                "Loose sprite descriptor metadata was not applied");
            Require(looseSprite.name == "weapon" &&
                host.TypedAssets.LoadSprite(AssetId.Parse("example.weapon:sprites/weapon")) == looseSprite,
                "Sprite filename-derived name or instance cache is wrong");
            Sprite mantleIcon = host.TypedAssets.LoadSprite(
                AssetId.Parse("core:UI/Items/Armor12.img_armor_mantle_of_night"));
            Sprite pumpkinIcon = host.TypedAssets.LoadSprite(
                AssetId.Parse("core:UI/Items/Helm31.img_helm_hw14_pumpkin"));
            Sprite skullIcon = host.TypedAssets.LoadSprite(
                AssetId.Parse("core:UI/Items/Ranged12.img_ranged_hw15_skull"));
            Sprite asteroidIcon = host.TypedAssets.LoadSprite(
                AssetId.Parse("core:UI/Items/Magic4.img_magic_asteroid"));
            Require(mantleIcon != null && pumpkinIcon != null && skullIcon != null && asteroidIcon != null &&
                mantleIcon != pumpkinIcon && mantleIcon != skullIcon && mantleIcon != asteroidIcon &&
                pumpkinIcon != skullIcon && pumpkinIcon != asteroidIcon && skullIcon != asteroidIcon,
                "Typed core atlas-member icons did not resolve to four distinct sprites");
            Sprite cropped = host.TypedAssets.LoadSprite(AssetId.Parse("example.weapon:sprites/crop"));
            Require(cropped != looseSprite && cropped.texture == looseSprite.texture &&
                cropped.rect == new Rect(1, 0, 1, 2) && cropped.pivot == new Vector2(0, 2) &&
                cropped.pixelsPerUnit == 25,
                "Two descriptors did not share their texture while preserving independent sprite settings");
            Texture2D directTexture = host.TypedAssets.LoadUnityAsset<Texture2D>(AssetId.Parse("example.weapon:textures/weapon"));
            Sprite smooth = host.TypedAssets.LoadSprite(AssetId.Parse("example.weapon:sprites/smooth"));
            Require(directTexture != null && smooth.texture == directTexture && directTexture != looseSprite.texture &&
                directTexture.filterMode == FilterMode.Bilinear && looseSprite.texture.filterMode == FilterMode.Point,
                "Direct texture loading or independent texture settings depended on sprite load order");
            Sprite repeating = host.TypedAssets.LoadSprite(AssetId.Parse("example.weapon:sprites/repeat"));
            Require(repeating.texture != directTexture && repeating.texture.wrapMode == TextureWrapMode.Repeat &&
                repeating.texture.mipmapCount > 1 && directTexture.wrapMode == TextureWrapMode.Clamp &&
                directTexture.mipmapCount == 1, "Wrap/mipmap cache variants changed an existing texture");
            Sprite legacy = host.TypedAssets.LoadSprite(AssetId.Parse("example.weapon:sprites/legacy"));
            Require(legacy != null && legacy.pixelsPerUnit == 50 && legacy.texture.filterMode == FilterMode.Point &&
                host.TypedAssets.LoadTexture(AssetId.Parse("example.weapon:sprites/legacy")) == legacy.texture,
                "Legacy PNG/sidecar or sprite-to-texture compatibility regressed");
            for (int i = 0; i < invalidSprites.Length; i++)
            {
                AssetId invalidId = AssetId.Parse("example.weapon:sprites/invalid_" + i);
                RejectAsset(() => host.TypedAssets.LoadSprite(invalidId), "Invalid sprite was accepted: " + invalidId);
            }
            RejectAsset(() => host.TypedAssets.LoadSprite(AssetId.Parse("example.weapon:textures/weapon")),
                "Raw texture was accepted as a sprite");
            Require(cropped.texture == looseSprite.texture && looseSprite.texture.filterMode == FilterMode.Point,
                "A failed sprite load invalidated an existing shared texture");
            using (var reversed = new ModAssetLoader(host.Assets))
            {
                Texture2D firstTexture = reversed.LoadTexture(AssetId.Parse("example.weapon:textures/weapon"));
                Sprite firstSmooth = reversed.LoadSprite(AssetId.Parse("example.weapon:sprites/smooth"));
                Sprite laterPoint = reversed.LoadSprite(AssetId.Parse("example.weapon:sprites/weapon"));
                Require(firstSmooth.texture == firstTexture && laterPoint.texture != firstTexture &&
                    firstTexture.filterMode == FilterMode.Bilinear && laterPoint.texture.filterMode == FilterMode.Point,
                    "Loading texture settings in reverse order changed sharing or filtering");
            }

            string looseModel = host.TypedAssets.LoadModelText(
                AssetId.Parse("example.weapon:models/mdl_weapon_example"));
            Require(looseModel != null && looseModel.Contains("<Scene") && looseModel.Contains("<Figures"),
                "Typed loose model XML decode failed");
            AudioClip looseAudio = host.TypedAssets.LoadAudio(AssetId.Parse("example.weapon:audio/test"));
            Require(looseAudio != null && looseAudio.samples == 4 && looseAudio.channels == 1 &&
                looseAudio.frequency == 8000, "Typed loose WAV decode failed");

            string coreModel = host.TypedAssets.LoadModelText(AssetId.Parse("core:gamedata/models/mdl_skeleton"));
            Require(!string.IsNullOrEmpty(coreModel) && coreModel.Contains("<Scene"),
                "Typed core model did not delegate to PackagedArtCatalog");

            Require(host.Assets.TryDescribe(AssetId.Parse("example.weapon:scripts/main"), out metadata) &&
                metadata.Kind == AssetKind.Text && metadata.Format == ".lua",
                "Loose script source was not mounted in the virtual filesystem");
            Require(host.Assets.TryDescribe(AssetId.Parse("example.weapon:localizations/eng"), out metadata) &&
                metadata.Kind == AssetKind.Text && metadata.Format == ".toml",
                "Loose localization source was not mounted in the virtual filesystem");
            var scriptLogs = new List<ModLogEntry>();
            var scriptRuntime = new MoonSharpScriptRuntime();
            Require(scriptRuntime.Name.StartsWith("MoonSharp ", StringComparison.Ordinal),
                "MoonSharp runtime did not report its interpreter identity");
            using (ModScriptSession scripts = host.StartScripts(scriptRuntime, entry => scriptLogs.Add(entry), content =>
            {
                var perksDocument = new XmlDocument();
                perksDocument.Load(Path.Combine(GameplayContentArchive.GetXmlRoot(), "perks.xml"));
                var perkNodes = new List<XmlNode>();
                foreach (XmlNode node in perksDocument.SelectNodes("/Perks/Perk")) perkNodes.Add(node);
                CoreContentImporter.ImportPerks(content, perkNodes);
            }))
            {
                Require(scripts.HasErrors, "Failing Lua mod did not produce script diagnostics");
                Require(scripts.ActiveMods.Count == 3 && scripts.ActiveMods.Any(x => x.Id.Value == "example.weapon") &&
                    scripts.ActiveMods.Any(x => x.Id.Value == "example.loadout") &&
                    scripts.ActiveMods.Any(x => x.Id.Value == "example.enchantment"),
                    "Failing Lua mod disabled the independent working script mod");
                Require(scripts.Diagnostics.Any(x => x.Code == "SCRIPT001" && x.Source == "script.failure" &&
                    x.Message.Contains("Unsafe module name")),
                    "Lua runtime failure was not attributed to the offending mod/source");
                Require(scripts.Diagnostics.Any(x => x.Code == "SCRIPT001" && x.Source == "script.runaway" &&
                    x.Message.Contains("instruction budget exceeded")),
                    "Runaway Lua entrypoint was not stopped by the instruction budget");
                Require(scripts.Diagnostics.Any(x => x.Code == "SCRIPT001" && x.Source == "registration.failure" &&
                    x.Message.Contains("rollback-after-registration")),
                    "Post-registration Lua failure was not attributed to its mod");
                Require(scriptLogs.Any(x => x.ModId.Value == "example.weapon" && x.Level == ModLogLevel.Info &&
                    x.Message == "lua-entry-ok"),
                    "Sandboxed Lua entrypoint did not execute/log through sf2.log.info");
                Require(scriptLogs.Any(x => x.ModId.Value == "example.loadout" && x.Level == ModLogLevel.Info &&
                    x.Message == "loadout-entry-ok"), "Multi-category Lua example did not execute");
                Require(scriptLogs.Any(x => x.ModId.Value == "example.enchantment" && x.Level == ModLogLevel.Info &&
                    x.Message == "enchantment-entry-ok"), "Perk/enchantment Lua example did not execute");
                Require(scriptLogs.Any(x => x.ModId.Value == "example.weapon" && x.Level == ModLogLevel.Debug &&
                    x.Message == "lua-debug-ok") &&
                    scriptLogs.Any(x => x.ModId.Value == "example.weapon" && x.Level == ModLogLevel.Warning &&
                    x.Message == "lua-warn-ok") &&
                    scriptLogs.Any(x => x.ModId.Value == "example.weapon" && x.Level == ModLogLevel.Error &&
                    x.Message == "lua-error-ok") &&
                    scriptLogs.Any(x => x.ModId.Value == "example.weapon" && x.Level == ModLogLevel.Info &&
                    x.Message == "lua-legacy-ok"), "Lua log levels/compatibility alias lost severity or mod attribution");
                Require(scripts.Content.IsFrozen && scripts.Content.Localizations.Count == 11 &&
                    scripts.Content.Weapons.Count == 1 && scripts.Content.Armors.Count == 1 &&
                    scripts.Content.Helms.Count == 1 && scripts.Content.Ranged.Count == 1 &&
                    scripts.Content.Magic.Count == 1 && scripts.Content.ItemRedirects.Count == 2 &&
                    scripts.Content.ShopListings.Count == 5 && scripts.Content.Perks.Count == 212 &&
                    scripts.Content.Enchantments.Count == 2 && scripts.Content.Behaviors.Count == 1,
                    "Successful Lua registration did not commit the complete equipment transactions");
                LocalizationDefinition title;
                Require(scripts.Content.TryGetLocalization(
                    DefinitionId.Parse("example.weapon:localization/weapon.example_blade"), out title) &&
                    title.GetOrEnglish("eng") == "Example Blade",
                    "Loose English localization did not commit through the transaction");
                WeaponDefinition weapon;
                Require(scripts.Content.TryGetWeapon(
                    DefinitionId.Parse("example.weapon:items/weapon/example_blade"), out weapon) &&
                    weapon.Damage == 0 && weapon.SubType == "Katana" &&
                    weapon.Progression == ItemProgressionKind.Vanilla &&
                    weapon.Icon == AssetId.Parse("example.weapon:sprites/weapon") &&
                    weapon.Model == AssetId.Parse("example.weapon:models/mdl_weapon_example"),
                    "Lua weapon definition did not preserve typed values");
                ShopListingDefinition listing;
                Require(scripts.Content.TryGetShopListing(
                    DefinitionId.Parse("example.weapon:shop/weapons/example_blade"), out listing) &&
                    listing.Item == weapon.Id && listing.Level == 12 &&
                    listing.Price == new ModPrice(ModPriceCurrency.Coins, 1000),
                    "Lua shop listing did not preserve item/level/price values");
                ArmorDefinition armor;
                HelmDefinition helm;
                RangedDefinition ranged;
                MagicDefinition magic;
                Require(scripts.Content.TryGetArmor(DefinitionId.Parse("example.loadout:items/armor/eclipse_mantle"), out armor) &&
                    armor.BodyDefense == 0 && armor.UnarmedDamage == 0 && armor.Progression == ItemProgressionKind.Vanilla &&
                    armor.Icon == AssetId.Parse("core:UI/Items/Armor12.img_armor_mantle_of_night") && armor.HasModel,
                    "Lua armor definition did not preserve typed values");
                Require(scripts.Content.TryGetHelm(DefinitionId.Parse("example.loadout:items/helm/eclipse_pumpkin"), out helm) &&
                    helm.HeadDefense == 0 && helm.Progression == ItemProgressionKind.Vanilla &&
                    helm.Icon == AssetId.Parse("core:UI/Items/Helm31.img_helm_hw14_pumpkin") && helm.HasModel,
                    "Lua helm definition did not preserve typed values");
                Require(scripts.Content.TryGetRanged(DefinitionId.Parse("example.loadout:items/ranged/eclipse_skull"), out ranged) &&
                    ranged.RangedDamage == 0 && ranged.SubType == "Skull" && ranged.Progression == ItemProgressionKind.Vanilla &&
                    ranged.Icon == AssetId.Parse("core:UI/Items/Ranged12.img_ranged_hw15_skull") && ranged.HasModel,
                    "Lua ranged definition did not preserve typed values");
                Require(scripts.Content.TryGetMagic(DefinitionId.Parse("example.loadout:items/magic/eclipse_asteroid"), out magic) &&
                    magic.MagicDamage == 0 && magic.SubType == "MagicAsteroid" && magic.Progression == ItemProgressionKind.Vanilla &&
                    magic.Icon == AssetId.Parse("core:UI/Items/Magic4.img_magic_asteroid") && magic.HasModel,
                    "Lua magic definition did not preserve typed values");
                PerkDefinition externalPerk = null;
                EnchantmentDefinition externalEnchantment = null;
                ModBehaviorDefinition externalBehavior = null;
                PerkDefinition legacyCompatibilityPerk = null;
                EnchantmentDefinition legacyCompatibilityEnchantment = null;
                Require(scripts.Content.TryGetPerk(
                        DefinitionId.Parse("example.enchantment:perks/eclipse_lifesteal"), out externalPerk) &&
                    scripts.Content.TryGetBehavior(
                        DefinitionId.Parse("example.enchantment:behaviors/lifesteal"), out externalBehavior) &&
                    externalPerk.HasBehavior && !externalPerk.HasTemplate &&
                    externalPerk.Behavior == externalBehavior.Id &&
                    externalPerk.DisplayName == DefinitionId.Parse("example.enchantment:localization/perk.eclipse_lifesteal") &&
                    !externalPerk.HasIcon &&
                    Math.Abs(externalPerk.InitialParameters["chance"].Number - 0.25d) < 0.000001d &&
                    externalPerk.InitialParameters["stacks"].Integer == 1 &&
                    scripts.Content.TryGetEnchantment(
                        DefinitionId.Parse("example.enchantment:enchantments/eclipse_lifesteal_weapon"),
                        out externalEnchantment) && externalEnchantment.HasBehavior && !externalEnchantment.HasPerk &&
                    externalEnchantment.Behavior == externalBehavior.Id &&
                    externalEnchantment.DisplayName == DefinitionId.Parse("example.enchantment:localization/enchantment.eclipse_lifesteal") &&
                    externalEnchantment.DisplayName != externalPerk.DisplayName &&
                    Math.Abs(externalEnchantment.InitialParameters["chance"].Number - 0.65d) < 0.000001d &&
                    externalEnchantment.InitialParameters["stacks"].Integer == 1 &&
                    externalEnchantment.Recipe == ModEnchantmentRecipe.Medium &&
                    externalEnchantment.Equipment.Count == 1 &&
                    externalEnchantment.Equipment[0] == ModEquipmentKind.Weapon &&
                    scripts.Content.TryGetPerk(
                        DefinitionId.Parse("example.enchantment:perks/legacy_lifesteal"), out legacyCompatibilityPerk) &&
                    legacyCompatibilityPerk.HasTemplate && !legacyCompatibilityPerk.HasBehavior &&
                    scripts.Content.TryGetEnchantment(
                        DefinitionId.Parse("example.enchantment:enchantments/legacy_lifesteal_weapon"),
                        out legacyCompatibilityEnchantment) && legacyCompatibilityEnchantment.HasPerk &&
                    !legacyCompatibilityEnchantment.HasBehavior &&
                    legacyCompatibilityEnchantment.Perk == legacyCompatibilityPerk.Id,
                    "Lua behavior/perk/enchantment definitions did not preserve typed values or decoupling");
                string behaviorError;
                Require(scripts.TryInvokeBehavior(externalBehavior.Id, ModEffectEvent.FightBegin,
                        externalEnchantment.InitialParameters,
                        new Dictionary<string, string>(StringComparer.Ordinal) { { "phase", "initial" } },
                        out behaviorError) && string.IsNullOrEmpty(behaviorError) &&
                    scriptLogs.Any(x => x.ModId.Value == "example.enchantment" && x.Level == ModLogLevel.Info &&
                        x.Message == "behavior-fight-begin-initial"),
                    "Bounded behavior callback did not receive typed parameters/context or log successfully: " + behaviorError);
                var fighterOperations = new TestFighterOperations();
                Require(scripts.TryInvokeBehavior(externalBehavior.Id, ModEffectEvent.FightBegin,
                        externalEnchantment.InitialParameters,
                        new Dictionary<string, string>(StringComparer.Ordinal) { { "phase", "interactive" } },
                        fighterOperations, out behaviorError) && string.IsNullOrEmpty(behaviorError) &&
                    fighterOperations.MagicChanges.Count == 1 &&
                    Math.Abs(fighterOperations.MagicChanges[0] - 0.65d) < 0.000001d &&
                    fighterOperations.HealthChanges.Count == 1 &&
                    Math.Abs(fighterOperations.HealthChanges[0] - 0.01d) < 0.000001d,
                    "Lua fighter capability calls did not reach the bounded host operations: " + behaviorError);
                ItemDefinition aliasedItem;
                Require(scripts.Content.TryResolveItem(
                        DefinitionId.Parse("example.weapon:items/weapon/example_blade_legacy"), out aliasedItem) &&
                    aliasedItem.Id == weapon.Id,
                    "Lua item alias did not resolve to its current item handle");
                Require(!scripts.Content.TryResolveItem(
                        DefinitionId.Parse("example.weapon:items/weapon/example_blade_retired"), out aliasedItem),
                    "Lua item tombstone incorrectly resolved as live content");
                WeaponDefinition rolledBack;
                Require(!scripts.Content.TryGetWeapon(
                    DefinitionId.Parse("registration.failure:items/weapon/rollback"), out rolledBack),
                    "Failing Lua mod leaked a staged weapon into the committed registry");
            }
            host.Dispose();
            host.Dispose(); // Shared textures must have a single owner and disposal must be repeatable.
#if UNITY_EDITOR
            if (!Application.isPlaying)
                Require(looseSprite == null && cropped == null && smooth == null && directTexture == null && legacy == null,
                    "Disposing the mod host leaked sprite or texture instances");
#endif

            ListSF.ResetModdingTestItems();
            ListSF.SeedModdingTestCoreItems();
            GameUtils.FDEJIIDIPBI.SeedCore(Path.Combine(GameplayContentArchive.GetXmlRoot(), "perks.xml"));
            ItemInfo vanillaKatana = ListSF.DJBOFEEKJMP().KCCDBEEKBCG("WEAPON_KATANA");
            ItemInfo vanillaBody = ListSF.DJBOFEEKJMP().KCCDBEEKBCG("Body");
            ItemInfo vanillaHead = ListSF.DJBOFEEKJMP().KCCDBEEKBCG("Head");
            ItemInfo vanillaNoRanged = ListSF.DJBOFEEKJMP().KCCDBEEKBCG("NoRanged");
            ItemInfo vanillaNoMagic = ListSF.DJBOFEEKJMP().KCCDBEEKBCG("NoMagic");
            ItemInfo[] duplicateRanged = ListSF.DJBOFEEKJMP().HCDLKHKBEPF().Where(x => x.Name == "GlaivebowArrow").ToArray();
            Require(vanillaKatana != null, "Vanilla weapon fixture was not seeded");
            Require(vanillaBody != null && vanillaHead != null && vanillaNoRanged != null && vanillaNoMagic != null &&
                duplicateRanged.Length == 2, "Vanilla equipment fixture was not seeded completely");
            LocalizationManager.ResetModdingTestLanguage("eng");
            ModRuntime.StartGameContent(modsRoot);
            ModScriptSession runtimeScripts = ModRuntime.Scripts;
            Require(runtimeScripts != null, "ModRuntime startup failed while importing core content");
            Require(runtimeScripts.ActiveMods.Count == 3 && runtimeScripts.ActiveMods.Any(x => x.Id.Value == "example.weapon") &&
                runtimeScripts.ActiveMods.Any(x => x.Id.Value == "example.loadout") &&
                runtimeScripts.ActiveMods.Any(x => x.Id.Value == "example.enchantment") &&
                runtimeScripts.Content.Weapons.Count == 211 && runtimeScripts.Content.Armors.Count == 180 &&
                runtimeScripts.Content.Helms.Count == 194 &&
                runtimeScripts.Content.Ranged.Count == 86 && runtimeScripts.Content.Magic.Count == 74 &&
                runtimeScripts.Content.ShopListings.Count == 5 && runtimeScripts.Content.Perks.Count == 212 &&
                runtimeScripts.Content.Enchantments.Count == 2 && runtimeScripts.Content.Behaviors.Count == 1 &&
                runtimeScripts.Diagnostics.Any(x => x.Code == "SCRIPT001" && x.Source == "script.failure") &&
                runtimeScripts.Diagnostics.Any(x => x.Code == "SCRIPT001" && x.Source == "registration.failure") &&
                runtimeScripts.Diagnostics.Any(x => x.Code == "SCRIPT001" && x.Source == "script.runaway"),
                "ModRuntime did not isolate the failing Lua mod during startup");
            const string externalPerkId = "example.enchantment:perks/eclipse_lifesteal";
            const string externalEnchantmentId = "example.enchantment:enchantments/eclipse_lifesteal_weapon";
            const string legacyPerkId = "example.enchantment:perks/legacy_lifesteal";
            const string legacyEnchantmentId = "example.enchantment:enchantments/legacy_lifesteal_weapon";
            PerkInfoItem legacyPerk = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(externalPerkId);
            PerkInfoItem legacyEnchantment = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(externalEnchantmentId);
            PerkInfoItem compatibilityPerk = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(legacyPerkId);
            string externalPresentationTitle = string.Empty;
            string externalPresentationDescription = string.Empty;
            Require(legacyPerk != null && legacyPerk.HAAKMBKCMCO.Attributes["Alias"]?.Value ==
                    "example.enchantment:localization/perk.eclipse_lifesteal" &&
                legacyPerk.HAAKMBKCMCO["Set"] == null && legacyEnchantment != null &&
                legacyEnchantment.HAAKMBKCMCO.Attributes["Alias"]?.Value ==
                    "example.enchantment:localization/enchantment.eclipse_lifesteal" &&
                legacyEnchantment.HAAKMBKCMCO.Attributes["Description"]?.Value ==
                    "example.enchantment:localization/enchantment.eclipse_lifesteal.description" &&
                ModRuntime.TryGetExternalEffectPresentation(externalEnchantmentId,
                    out externalPresentationTitle, out externalPresentationDescription) &&
                externalPresentationTitle == "example.enchantment:localization/enchantment.eclipse_lifesteal" &&
                externalPresentationDescription ==
                    "example.enchantment:localization/enchantment.eclipse_lifesteal.description" &&
                ForgeManager.ELEBLBJKDBI().HasExternalEnchantmentCandidate("Medium", "Weapon", externalEnchantmentId) &&
                ForgeManager.ELEBLBJKDBI().HasExternalEnchantmentMetadata("Medium", "Weapon", externalEnchantmentId,
                    externalEnchantmentId, "Single") &&
                ForgeManager.ELEBLBJKDBI().HasExternalEnchantmentParameter("Medium", "Weapon", externalEnchantmentId,
                    "chance", "0.65") &&
                ForgeManager.ELEBLBJKDBI().HasExternalEnchantmentParameter("Medium", "Weapon", externalEnchantmentId,
                    "stacks", "1") && compatibilityPerk != null &&
                ForgeManager.ELEBLBJKDBI().HasExternalEnchantmentCandidate("Medium", "Weapon", legacyPerkId) &&
                ForgeManager.ELEBLBJKDBI().HasExternalEnchantmentMetadata("Medium", "Weapon", legacyPerkId,
                    legacyEnchantmentId, "Single"),
                "Behavior-backed perk/enchantment was not adapted into the recovered runtime");

            var savedBehaviorEffect = new XmlDocument();
            savedBehaviorEffect.LoadXml("<Perk Name='" + externalEnchantmentId + "' EclipseEnchantment='" +
                externalEnchantmentId + "' EclipseKind='Single'><Set Aspect='321'/><EclipseParams Format='1'>" +
                "<Param Name='chance' Value='0.8'/><Param Name='stacks' Value='2'/></EclipseParams></Perk>");
            Require(ModRuntime.TryReadSavedEnchantment(savedBehaviorEffect.DocumentElement,
                    out EnchantmentDefinition savedEnchantment, out ModEffectInstance savedInstance,
                    out string savedReadError) && string.IsNullOrEmpty(savedReadError) &&
                savedEnchantment.Id == DefinitionId.Parse(externalEnchantmentId) &&
                Math.Abs(savedInstance.Values["chance"].Number - 0.8d) < 0.000001d &&
                savedInstance.Values["stacks"].Integer == 2,
                "Saved EclipseEnchantment/EclipseParams did not resolve to the direct behavior definition: " + savedReadError);
            Require(ModRuntime.TryInvokeSavedEnchantmentFightBegin(savedBehaviorEffect.DocumentElement,
                    new Dictionary<string, string>(StringComparer.Ordinal) { { "phase", "saved" } },
                    out string savedInvokeError) && string.IsNullOrEmpty(savedInvokeError),
                "Saved typed enchantment did not reach its bounded behavior handler: " + savedInvokeError);
            var savedBehaviorPerk = new XmlDocument();
            savedBehaviorPerk.LoadXml("<Perk Name='" + externalPerkId + "' Level='1'><EclipseParams Format='1'>" +
                "<Param Name='chance' Value='0.3'/><Param Name='stacks' Value='4'/></EclipseParams></Perk>");
            Require(ModRuntime.TryReadSavedPerk(savedBehaviorPerk.DocumentElement,
                    out PerkDefinition savedPerk, out ModEffectInstance savedPerkInstance,
                    out string savedPerkReadError) && string.IsNullOrEmpty(savedPerkReadError) &&
                savedPerk.Id == DefinitionId.Parse(externalPerkId) &&
                Math.Abs(savedPerkInstance.Values["chance"].Number - 0.3d) < 0.000001d &&
                savedPerkInstance.Values["stacks"].Integer == 4,
                "Saved behavior-backed perk EclipseParams did not resolve as typed instance state: " + savedPerkReadError);
            Require(ModRuntime.TryInvokeSavedPerkFightBegin(savedBehaviorPerk.DocumentElement,
                    new Dictionary<string, string>(StringComparer.Ordinal) { { "phase", "perk_saved" } },
                    new TestFighterOperations(), out string savedPerkInvokeError) && string.IsNullOrEmpty(savedPerkInvokeError),
                "Saved typed perk did not reach its bounded behavior handler: " + savedPerkInvokeError);
            Require(ListSF.DJBOFEEKJMP().HCDLKHKBEPF().Count == 745 &&
                ListSF.DJBOFEEKJMP().KCCDBEEKBCG("core:items/weapon/weapon_katana") == vanillaKatana &&
                vanillaKatana.Name == "WEAPON_KATANA", "Core registry import duplicated or renamed a legacy weapon");
            Require(ListSF.DJBOFEEKJMP().KCCDBEEKBCG("core:items/armor/body") == vanillaBody &&
                ListSF.DJBOFEEKJMP().KCCDBEEKBCG("core:items/helm/head") == vanillaHead &&
                ListSF.DJBOFEEKJMP().KCCDBEEKBCG("core:items/ranged/noranged") == vanillaNoRanged &&
                ListSF.DJBOFEEKJMP().KCCDBEEKBCG("core:items/magic/nomagic") == vanillaNoMagic &&
                ListSF.DJBOFEEKJMP().KCCDBEEKBCG("core:items/ranged/glaivebowarrow/riflebullet") == duplicateRanged[1],
                "Qualified core equipment lookup did not resolve to the exact legacy ItemInfo source");
            WeaponDefinition coreKatana;
            Require(runtimeScripts.Content.TryGetWeapon(DefinitionId.Parse("core:items/weapon/weapon_katana"), out coreKatana) &&
                coreKatana.LegacyItemXml == vanillaKatana.NodeXML.OuterXml,
                "Core registry did not preserve the loaded vanilla source definition");
            LocalizationDefinition coreKatanaName;
            Require(runtimeScripts.Content.TryGetLocalization(coreKatana.DisplayName, out coreKatanaName) &&
                coreKatanaName.GetOrEnglish("eng") == "Katana",
                "Core localization was not read from the gameplay XML root");
            ArmorDefinition coreBody;
            Require(runtimeScripts.Content.TryGetArmor(DefinitionId.Parse("core:items/armor/body"), out coreBody) &&
                coreBody.LegacyName == "Body" && coreBody.HasModel && coreBody.BodyDefense == 0 &&
                coreBody.UnarmedDamage == 0,
                "Core armor registry projection did not preserve the vanilla Body definition");
            HelmDefinition coreHead;
            Require(runtimeScripts.Content.TryGetHelm(DefinitionId.Parse("core:items/helm/head"), out coreHead) &&
                coreHead.LegacyName == "Head" && coreHead.HasModel && coreHead.HeadDefense == 0,
                "Core helm registry projection did not preserve the vanilla Head definition");
            RangedDefinition coreNoRanged;
            Require(runtimeScripts.Content.TryGetRanged(DefinitionId.Parse("core:items/ranged/noranged"), out coreNoRanged) &&
                coreNoRanged.LegacyName == "NoRanged" && !coreNoRanged.HasModel,
                "Core ranged registry projection did not preserve the vanilla NoRanged definition");
            MagicDefinition coreNoMagic;
            Require(runtimeScripts.Content.TryGetMagic(DefinitionId.Parse("core:items/magic/nomagic"), out coreNoMagic) &&
                coreNoMagic.LegacyName == "NoMagic" && !coreNoMagic.HasModel,
                "Core magic registry projection did not preserve the vanilla NoMagic definition");
            var saveMetadata = new XmlDocument();
            saveMetadata.LoadXml("<Warrior />");
            ModRuntime.RecordSaveContext(saveMetadata.DocumentElement);
            string expectedContentHash = ModSaveData.ComputeContentSetFingerprint(runtimeScripts.ActiveMods, runtimeScripts.Content);
            Require(saveMetadata.DocumentElement["EclipseMods"]?.GetAttribute("contentHash") == expectedContentHash &&
                expectedContentHash.StartsWith("sha256:", StringComparison.Ordinal) && expectedContentHash.Length == 71,
                "ModRuntime did not persist the current deterministic content-set fingerprint");
            const string legacyItemId = "example.weapon:items/weapon/example_blade";
            const string legacyLocalizationId = "example.weapon:localization/weapon.example_blade";
            ItemInfo legacyWeapon = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(legacyItemId);
            Require(legacyWeapon != null && legacyWeapon.Type == "Weapon" && legacyWeapon.SubType == "Katana" &&
                legacyWeapon.FileName == "example.weapon:sprites/weapon" &&
                legacyWeapon.Model == "example.weapon:models/mdl_weapon_example" &&
                legacyWeapon.Text == legacyLocalizationId && legacyWeapon.Level == 12 &&
                legacyWeapon.UpgradeLevel == 1200 && legacyWeapon.WeaponDamage == 261 &&
                legacyWeapon.Price == 1000 && legacyWeapon.BonusPrice == 0 &&
                legacyWeapon.UpgradeTemplate == "Weapon_Bonus",
                "Committed weapon was not adapted to the expected legacy ItemInfo fields");
            Require(ListSF.DJBOFEEKJMP().KCCDBEEKBCG("example.weapon:items/weapon/example_blade_legacy") == legacyWeapon,
                "Legacy item lookup did not resolve an old namespaced save ID through the item alias");
            Require(ListSF.DJBOFEEKJMP().KCCDBEEKBCG("example.weapon:items/weapon/example_blade_retired") == null,
                "Tombstoned item ID unexpectedly resolved to a live legacy item");
            ItemInfo legacyArmor = ListSF.DJBOFEEKJMP().KCCDBEEKBCG("example.loadout:items/armor/eclipse_mantle");
            ItemInfo legacyHelm = ListSF.DJBOFEEKJMP().KCCDBEEKBCG("example.loadout:items/helm/eclipse_pumpkin");
            ItemInfo legacyRanged = ListSF.DJBOFEEKJMP().KCCDBEEKBCG("example.loadout:items/ranged/eclipse_skull");
            ItemInfo legacyMagic = ListSF.DJBOFEEKJMP().KCCDBEEKBCG("example.loadout:items/magic/eclipse_asteroid");
            Require(legacyArmor != null && legacyArmor.Type == "Armor" && legacyArmor.Level == 2 &&
                legacyArmor.UpgradeLevel == 200 && legacyArmor.BodyDefense == 22 && legacyArmor.UnarmedDamage == 8 &&
                legacyArmor.FileName == "core:ui/items/armor12.img_armor_mantle_of_night" &&
                legacyArmor.UpgradeTemplate == "Armor_Bonus",
                "External armor was not adapted through the legacy runtime");
            Require(legacyHelm != null && legacyHelm.Type == "Helm" && legacyHelm.Level == 2 &&
                legacyHelm.UpgradeLevel == 200 && legacyHelm.HeadDefense == 18 &&
                legacyHelm.FileName == "core:ui/items/helm31.img_helm_hw14_pumpkin" &&
                legacyHelm.UpgradeTemplate == "Helm_Bonus", "External helm was not adapted through the legacy runtime");
            Require(legacyRanged != null && legacyRanged.Type == "Ranged" && legacyRanged.Level == 6 &&
                legacyRanged.UpgradeLevel == 600 && legacyRanged.RangedDamage == 105 &&
                legacyRanged.SubType == "Skull" &&
                legacyRanged.FileName == "core:ui/items/ranged12.img_ranged_hw15_skull" &&
                legacyRanged.UpgradeTemplate == "Ranged_Bonus",
                "External ranged item was not adapted through the legacy runtime");
            Require(legacyMagic != null && legacyMagic.Type == "Magic" && legacyMagic.Level == 6 &&
                legacyMagic.UpgradeLevel == 600 && legacyMagic.MagicDamage == 105 &&
                legacyMagic.SubType == "MagicAsteroid" &&
                legacyMagic.FileName == "core:ui/items/magic4.img_magic_asteroid" &&
                legacyMagic.UpgradeTemplate == "Magic_Bonus",
                "External magic item was not adapted through the legacy runtime");

            ModRuntime.ApplyLegacyLocalization();
            Require(LocalizationManager.GetExternalStringForTest(legacyLocalizationId) == "Example Blade",
                "English mod localization did not enter the legacy localization table");
            Require(LocalizationManager.GetExternalStringForTest(externalPresentationDescription) ==
                    "Direct behavior-backed enchantment with independent state.",
                "Behavior-backed enchantment description localization did not enter the legacy localization table");
            Require(LocalizationManager.GetExternalStringForTest(legacyItemId) == "Example Blade",
                "Legacy item-name localization alias was not published for the mod weapon");
            LocalizationManager.ChangeModdingTestLanguage("pol");
            Require(LocalizationManager.GetExternalStringForTest(legacyLocalizationId) == "Przykladowe Ostrze",
                "Mod localization was not reapplied after a legacy language change");
            Require(LocalizationManager.GetExternalStringForTest(legacyItemId) == "Przykladowe Ostrze",
                "Legacy item-name localization alias was not reapplied after a language change");

            string bridgeModel = ModRuntime.LoadQualifiedModelText(
                "example.weapon:models/mdl_weapon_example.xml");
            Require(!string.IsNullOrEmpty(bridgeModel) && bridgeModel.Contains("<Scene"),
                "Qualified mod model lookup with recovered .xml suffix failed");
            Sprite bridgeLoose = ResourcesAndBundles.Load<Sprite>("example.weapon:sprites/weapon");
            Require(bridgeLoose != null && bridgeLoose.texture != null,
                "ResourcesAndBundles did not route qualified loose sprite through ModRuntime");
            Sprite bridgeCore = ResourcesAndBundles.Load<Sprite>("core:Textures/Locations/moon/background_1");
            Require(bridgeCore != null && bridgeCore.texture != null,
                "ResourcesAndBundles did not route qualified core sprite through ModRuntime");
            Sprite legacyCore = ResourcesAndBundles.Load<Sprite>("Textures/Locations/moon/background_1");
            Require(legacyCore != null && legacyCore.texture != null,
                "Namespaced bridge regressed unqualified legacy resource loading");
            Sprite legacyCoreByName = ResourcesAndBundles.Load<Sprite>("AgnisSeal.helm");
            Require(legacyCoreByName != null && legacyCoreByName.texture != null,
                "Implicit core routing lost PackagedArtCatalog legacy atlas-member compatibility");
            ModRuntime.Shutdown();
            Require(ListSF.DJBOFEEKJMP().HCDLKHKBEPF().Count == 740 &&
                ListSF.DJBOFEEKJMP().KCCDBEEKBCG("WEAPON_KATANA") == vanillaKatana,
                "ModRuntime shutdown removed a vanilla weapon");
            Require(ListSF.DJBOFEEKJMP().KCCDBEEKBCG(legacyItemId) == null,
                "ModRuntime shutdown did not remove the injected legacy weapon");
            Require(LocalizationManager.GetExternalStringForTest(legacyLocalizationId) == null,
                "ModRuntime shutdown did not remove injected localization aliases");
            Require(GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(externalPerkId) == null &&
                GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(externalEnchantmentId) == null &&
                GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(legacyPerkId) == null &&
                !ForgeManager.ELEBLBJKDBI().HasExternalEnchantmentCandidate("Medium", "Weapon", externalEnchantmentId) &&
                !ForgeManager.ELEBLBJKDBI().HasExternalEnchantmentCandidate("Medium", "Weapon", legacyPerkId),
                "ModRuntime shutdown did not remove injected perk/enchantment runtime state");
        }
        finally
        {
            ModRuntime.Shutdown();
            if (Directory.Exists(modsRoot)) Directory.Delete(modsRoot, true);
        }
    }

    private static byte[] CreateTestPng()
    {
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        try
        {
            texture.SetPixels(new[] { Color.red, Color.green, Color.blue, Color.white });
            texture.Apply();
            return texture.EncodeToPNG();
        }
        finally
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) UnityEngine.Object.DestroyImmediate(texture);
            else UnityEngine.Object.Destroy(texture);
#else
            UnityEngine.Object.Destroy(texture);
#endif
        }
    }

    private static byte[] CreateTestWav()
    {
        short[] samples = { 0, 1000, -1000, 0 };
        using (var output = new MemoryStream())
        using (var writer = new BinaryWriter(output))
        {
            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + samples.Length * 2);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
            writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)1);
            writer.Write(8000);
            writer.Write(16000);
            writer.Write((short)2);
            writer.Write((short)16);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            writer.Write(samples.Length * 2);
            foreach (short sample in samples) writer.Write(sample);
            return output.ToArray();
        }
    }

    private static int CheckLocationCoverage(PackagedArtCatalog.Catalog catalog)
    {
        var seen = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
        int count = 0;
        foreach (PackagedArtCatalog.BundleRecord bundle in catalog.bundles)
        {
            if (string.Equals(bundle.name, "LOCATION_DATA", StringComparison.OrdinalIgnoreCase))
                continue;
            foreach (PackagedArtCatalog.ArtRecord asset in bundle.assets)
            {
                string address = asset.address ?? string.Empty;
                if (!address.StartsWith("Textures/Locations/", StringComparison.OrdinalIgnoreCase) &&
                    !address.StartsWith("Textures/Location_effects/", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (!seen.Add(address)) continue;
                count++;
            }
        }
        return count;
    }

    private static int CheckCoreLocationRuntimeCoverage(PackagedArtCatalog.Catalog catalog)
    {
        PackagedArtCatalog.BundleRecord group = catalog.bundles.FirstOrDefault(x =>
            string.Equals(x.name, "CORE_LOCATIONS", StringComparison.OrdinalIgnoreCase));
        Require(group != null, "CORE_LOCATIONS group missing during runtime coverage");
        int count = 0;
        foreach (PackagedArtCatalog.ArtRecord asset in group.assets)
        {
            Sprite[] sprites = PackagedArtCatalog.LoadWithSubAssets<Sprite>(asset.address);
            Require(sprites != null && sprites.Any(x => x != null && x.texture != null),
                "CORE_LOCATIONS runtime lookup failed: " + asset.address);
            count++;
        }
        return count;
    }

    private static int CheckLocationDataCoverage(PackagedArtCatalog.Catalog catalog)
    {
        PackagedArtCatalog.BundleRecord group = catalog.bundles.FirstOrDefault(x =>
            string.Equals(x.name, "LOCATION_DATA", StringComparison.OrdinalIgnoreCase));
        Require(group != null, "LOCATION_DATA group missing");
        int count = 0;
        foreach (PackagedArtCatalog.ArtRecord asset in group.assets)
        {
            string text = PackagedArtCatalog.LoadLocationDataText(asset.address + ".xml");
            Require(!string.IsNullOrEmpty(text), "TAR location data missing: " + asset.address);
            var xml = new XmlDocument();
            xml.LoadXml(text);
            Require(xml.DocumentElement != null &&
                (xml.DocumentElement.Name == "plist" || xml.DocumentElement.Name == "dict"),
                "TAR location data is not plist XML: " + asset.address);
            count++;
        }
        return count;
    }

#if UNITY_EDITOR
    public static void RunEditor()
    {
        try
        {
            // Exercise the production resolver: loose canonical XML in the editor,
            // and the packaged Resources archive (not StreamingAssets) in the player.
            const string gameplayResource = "Assets/Resources/SF2Content/gameplay.bytes";
            File.WriteAllBytes(gameplayResource, GameplayContentArchive.CreateArchive(GameplayContentArchive.GetXmlRoot()));
            AssetDatabase.ImportAsset(gameplayResource, ImportAssetOptions.ForceSynchronousImport);
            PlayerSettings.companyName = "EclipseTests";
            PlayerSettings.productName = "PackagedContentSmoke";
            PackagedArtCatalog.ValidateProjectFiles(Application.dataPath);
            CheckPackagedBundles();
            TextAsset manifest = Resources.Load<TextAsset>(PackagedArtCatalog.CatalogResourcePath);
            PackagedArtCatalog.Catalog catalog = PackagedArtCatalog.ReadCatalog(manifest.text);
            Resources.UnloadAsset(manifest);
            int locationAddresses = CheckLocationCoverage(catalog);
            Debug.Log("[PackagedArtTest] location coverage PASS: " + locationAddresses + " unique addresses.");
            int locationData = CheckLocationDataCoverage(catalog);
            Debug.Log("[PackagedArtTest] location data PASS: " + locationData + " atlas records.");
            if (Environment.GetCommandLineArgs().Contains("-buildContentSmoke"))
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), "Assets/ContentSmoke.unity");
                BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
                {
                    scenes = new[] { "Assets/ContentSmoke.unity" },
                    locationPathName = "Build/ContentSmoke.exe",
                    target = BuildTarget.StandaloneWindows64,
                    options = BuildOptions.Development
                });
                Require(report.summary.result == BuildResult.Succeeded, "Content smoke player build failed");
            }
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogError("[PackagedArtTest] " + exception);
            EditorApplication.Exit(1);
        }
    }
#else
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InstallPlayerSmoke()
    {
        new GameObject("PackagedArtPlayerSmoke").AddComponent<PackagedArtPlayerSmoke>();
    }

    private sealed class PackagedArtPlayerSmoke : MonoBehaviour
    {
        private void Start()
        {
            try
            {
                CheckPackagedBundles();
                Sprite custom = PackagedArtCatalog.Load<Sprite>("UI/Items/gift_box_red_n_gold");
                Require(custom != null && custom.texture != null, "Custom-mesh sprite lookup failed");
                Require(custom.vertices.Length == 81 && custom.triangles.Length >= 3,
                    "Custom-mesh sprite geometry was not preserved");
                Debug.Log("[PackagedArtTest] custom mesh PASS: " + custom.vertices.Length + " vertices.");
                TextAsset manifest = Resources.Load<TextAsset>(PackagedArtCatalog.CatalogResourcePath);
                PackagedArtCatalog.Catalog catalog = PackagedArtCatalog.ReadCatalog(manifest.text);
                Resources.UnloadAsset(manifest);
                int coreLocations = CheckCoreLocationRuntimeCoverage(catalog);
                Debug.Log("[PackagedArtTest] core location runtime PASS: " + coreLocations + " exact addresses.");
                Sprite moonLayer3 = PackagedArtCatalog.Load<Sprite>("Textures/Locations/moon/layer3");
                Require(moonLayer3 != null && moonLayer3.vertices.Length == 98,
                    "Moon layer3 runtime mesh regression");
                Debug.Log("[PackagedArtTest] Moon layer3 PASS: " + moonLayer3.vertices.Length + " vertices.");
                Application.Quit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError("[PackagedArtTest] " + exception);
                Application.Quit(1);
            }
        }
    }
#endif
}
