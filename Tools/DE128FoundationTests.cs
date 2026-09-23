using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Eclipse.Modding;

internal static class DE128FoundationTests
{
    private static readonly string[] Services =
        { "paid_offers", "battle_pass", "ads", "rewarded_video", "online_services", "payments" };
    private static readonly UTF8Encoding Utf8 = new UTF8Encoding(false);
    private static readonly string[] Capabilities =
        { "policy.services", "policy.timers", "content.register", "content.patch", "combat.modify_outgoing_hit", "combat.effects", "story.events", "story.progression", "profile.read", "state.read", "state.write", "ui.create", "presentation.navigate" };
    private static readonly HashSet<string> CallTimeCapabilities = new HashSet<string> { "profile.read", "state.read", "ui.create", "presentation.navigate" };
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
                catalog.ItemDefaultEnchantments.Count == 0, "Failed or absent DE128 left restored equipment behind.");
            Check(catalog.ItemInnatePerks.Count == 0, "Failed or absent DE128 left innate perks registered.");
            Check(catalog.Patches.Count == 0, "Failed or absent DE128 left content patches registered.");
            Check(!catalog.ItemAvailabilityPolicies.Any(), "Failed or absent DE128 left shop policies registered.");
            Check(catalog.ItemCombatSubtypes.Count == 0 && catalog.ItemTacticSubtypes.Count == 0,
                "Failed or absent DE128 left combat classification patches.");
            Check(catalog.Moves.Count == 0 && catalog.MoveItemLockExtensions.Count == 0 && catalog.MoveCombatPatches.Count == 0,
                "Failed or absent DE128 left moves or item-lock extensions.");
        }
    }

    private static void CheckDE(ModContentCatalog catalog)
    {
        ModPolicies.Content = catalog;
        Check(catalog.TimerPolicies.Count == 1 && catalog.TryGetTimer("forge", out var timer) &&
            timer.Owner.Value == "de128", "The forge policy is not exclusively owned by DE128.");
        Check(ModPolicies.DeliverySeconds("forge", 120) == 0, "New forge orders are not instant.");
        Check(ModPolicies.SkipEnabled("forge"), "Already-pending orders lost their normal skip path.");
        Check(ModPolicies.CompletePending("forge"), "DE pending orders are not eligible for normal settlement.");
        Check(Services.All(service => !ModPolicies.FeatureEnabled(service)), "A DE service gate is missing.");
        Check(ModPolicies.FeatureEnabled("campaign"), "An unrelated feature was disabled.");
        Check(catalog.ItemCombatSubtypes.Count == 5 && catalog.ItemTacticSubtypes.Count == 0,
            "DE combat classification patches are incomplete.");
        Check(catalog.Moves.Count == 35 && catalog.MoveItemLockExtensions.Count == 10 && catalog.MoveCombatPatches.Count == 5,
            "Chinese swords combat/preview registrations or lock extensions are incomplete.");
        var slash = catalog.Moves.Single(move => move.Id.LocalId == "chinese_swords_super_slash");
        Check(slash.Graph.Presentation.Profile.DisplayName.HasValue &&
            catalog.TryGetLocalization(slash.Graph.Presentation.Profile.DisplayName.Value, out var moveTitle) &&
            moveTitle.GetOrEnglish("eng") == "Super Slash", "Chinese swords profile title is not localized.");
        Check(slash.Animation.ToString() == "de128:animations/chinese_swords_super_slash_old" &&
            catalog.ItemCombatSubtypes.Any(patch => patch.Item == CoreContentImporter.WeaponId("WEAPON_CHNY21_JIAN") && patch.Subtype == "ChineseSwords"),
            "Chinese swords binary/subtype registration changed.");
        Check(catalog.ItemAvailabilityPolicies.Count() == 48 &&
            catalog.ItemAvailabilityPolicies.Where(policy => policy.Item.Namespace.Value == "core").All(policy => policy.Owner.Value == "de128" &&
                policy.Visibility == ModItemVisibility.ForceVisible && policy.MinimumLevel >= 15),
            "DE shop policies are incomplete after registration/rebuild/conflict.");
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
        Check(catalog.Rewards.Count(value => !value.Id.LocalId.StartsWith("sensei_act_") && !value.Id.LocalId.StartsWith("uw_")) == 1 &&
            catalog.Rewards.Count(value => value.Id.LocalId.StartsWith("sensei_act_")) == 57 && catalog.TryGetReward(
            DefinitionId.Parse("de128:rewards/titans_desolator"), out var reward) &&
            reward.Items.Count == 1 && reward.Items[0].Item == Sword && reward.Items[0].UsesConfiguration &&
            reward.Choices.Count == 0 && reward.Gems == 0 && catalog.ItemDefaultEnchantments.Count == 23 && !catalog.ItemDefaultEnchantments.Any(value => value.Item == Sword),
            "Desolator must use one configured reward without changing equipment defaults or currencies.");
        Check(catalog.TryGetFight(DefinitionId.Parse("core:fights/zone_7/c3_boss_titan_eclipsemode/6"), out var titan) &&
            titan.RewardDrops.Count == 1 && catalog.Fights.Count(fight => fight.RewardDrops.Count != 0) == 1,
            "DE128 must patch only the final Eclipse Titan reward.");
        var drop = titan.RewardDrops[0];
        Check(drop.ResultIndex == 1 && drop.Mode == ModRuleMode.Eclipse && !drop.MinimumLevel.HasValue &&
            !drop.MaximumLevel.HasValue && drop.Reward.Id.ToString() == "de128:rewards/titans_desolator",
            "Desolator reward changed its winning slot, mode or level gate.");
        // The only mod-owned opponents, fights and rules are the Sensei story's and the Underworld's.
        bool Owned(string id) => id.StartsWith("sensei_") || id.StartsWith("uw_");
        Check(catalog.Modes.Count == 0 && catalog.Quests.Count == 0 &&
            catalog.Warriors.Count(value => value.Id.LocalId.StartsWith("sensei_")) == 34 && catalog.Warriors.All(value => Owned(value.Id.LocalId)) &&
            catalog.Fights.Count(fight => !fight.IsCore && fight.Id.LocalId.StartsWith("sensei_act_")) == 23 &&
            catalog.Fights.Where(fight => !fight.IsCore).All(fight => Owned(fight.Id.LocalId)) &&
            catalog.FightRules.All(rule => Owned(rule.Id.LocalId)),
            "Disabled Ascension registered live modes, opponents, fights, rules or quests.");
        // Active Underworld: eight Underworld pages generated from the archived raid stages.
        var underworldZones = catalog.Zones.Where(zone => !zone.IsCore && zone.Underworld).ToArray();
        var underworldBattles = catalog.Battles.Where(battle => underworldZones.Any(zone => zone.Id == battle.Zone)).ToArray();
        Check(underworldZones.Length == 8 && underworldBattles.Length == 76 &&
            underworldBattles.Count(battle => battle.PowerMode == ModPowerMode.Normal) == 36 &&
            underworldBattles.Count(battle => battle.PowerMode == ModPowerMode.Power) == 36 &&
            underworldBattles.Count(battle => battle.PowerMode == ModPowerMode.Always) == 4 &&
            underworldBattles.Count(battle => battle.Icons != null) == 25,
            "Underworld pages, Power Mode twins or shipped map buttons differ from the archive.");
        var underworldRewards = catalog.Rewards.Where(reward => reward.Id.LocalId.StartsWith("uw_")).ToArray();
        Check(catalog.Fights.Count(fight => fight.Id.LocalId.StartsWith("uw_")) == 76 && underworldRewards.Length == 180 &&
            underworldRewards.Sum(reward => reward.Currencies.Count) == 108 &&
            catalog.WarriorTemplates.Count(template => template.Body != null && template.Id.Namespace.Value == "de128") == 66 &&
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
            catalog.Battles.Where(value => !value.IsCore && value.Preview.StartsWith("de128:")).Count() == 10,
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

    private static void CheckSharedMovePatches(ModDescriptor mod, ModContentCatalog catalog, string fixture, string repository)
    {
        var archive = ReadXml(Path.Combine(repository,"Assets/DExml/animations/moves.xml"));
        var vanilla = ReadXml(Path.Combine(repository,"Assets/vanillaXml/animations/moves.xml"));
        foreach (var patch in catalog.MoveCombatPatches)
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
            if (patch.Hit != null)
                Check(oldMove.SelectSingleNode("Intervals/Interval/Hit").Attributes["Name"].Value == patch.Hit.Expected &&
                    newMove.SelectSingleNode("Intervals/Interval/Hit").Attributes["Name"].Value == patch.Hit.Value,"Move reaction patch differs from archive.");
            if (patch.SoundFrame != null)
            {
                var value=patch.SoundFrame;
                Check(oldMove.SelectSingleNode("Actions/Sound[@Name='"+value.Name+"']").Attributes["Frame"].Value == value.Expected.ToString() &&
                    newMove.SelectSingleNode("Actions/RandomSound[Sound/@Name='"+value.Name+"']").Attributes["Frame"].Value == value.Value.ToString(),"Sound frame patch differs from archive.");
            }
            foreach (var condition in patch.Conditions)
                Check(condition.Kind == ModMoveConditionKind.ModExists && condition.Name == "Stun" && condition.Not &&
                    newMove.SelectSingleNode("Conditions/ModExists[@Name='Stun' and @Not='1']") != null &&
                    oldMove.SelectSingleNode("Conditions/ModExists[@Name='Stun']") == null,"Added native condition differs from archive.");
        }
        var peer = Peer(fixture,"fixture.move-patch-conflict","content.patch",
            "sf2.moves.patch { move='MassBombPlayer', conditions={{type='mod_exists',name='Other'}} }");
        var peerFirst=new ModContentCatalog();Load(peer,peerFirst);
        ExpectFailure(mod,peerFirst,"Move combat patch already owned: MassBombPlayer");
        Check(peerFirst.MoveCombatPatches.Count==1 && peerFirst.MoveCombatPatches[0].Owner==peer.Id &&
            peerFirst.TimerPolicies.Count==0 && !peerFirst.Weapons.Any(item=>!item.IsCore),"Conflicting DE registration leaked content.");
        ExpectFailure(peer,catalog,"Move combat patch already owned: MassBombPlayer");CheckDE(catalog);
        int index=0;
        foreach(string body in new[]{"move='Test'", "move='bad:name', hit={expected='High',value='Low'}",
            "move='Test', hit={expected='High',value='High'}", "move='Test', hit={expected='High',value='Unknown'}",
            "move='Test', interval_end={name='Attack',expected=42,value=40}","move='Test', interval_end={name='Uninterrupt',expected=42,value=-1}",
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
            "sf2.moves.patch {move='Test',sound_frame={name='snd',expected=18,value=16}}",
            "sf2.moves.patch {move='Test',conditions={{type='mod_exists',name='Stun'}}}",
            "sf2.moves.patch {move='Test',conditions={{type='mod_exists',name='Stun',['not']=true}}}"};
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
        var catalog = new ModContentCatalog();
        RewardItemGrant actualGrant;
        using (LoadLive(actualMod, catalog))
        {
            CheckDE(catalog);
            int underworld = DE128UnderworldTests.Run(catalog, _repository, (ok, message) => Check(ok, message));
            Console.WriteLine("Underworld archive comparisons: " + underworld);
            var reward = catalog.Rewards.Single(value => value.Id.LocalId == "titans_desolator");
            Check(reward.TryGetGrant(0, out actualGrant) && !reward.TryGetGrant(-1, out _) &&
                !reward.TryGetGrant(1, out _), "Reward flat index validation failed.");
            foreach (int level in new[] { 1, 2, 7, 51, 52 })
            {
                var configuration = actualGrant.Configure(level);
                Check(configuration.Level == level && configuration.Enchantments.Count == 1,
                    "The actual DE128 callback did not configure player-level equipment.");
                var enchantment = configuration.Enchantments[0];
                Check(enchantment.Perk == CoreContentImporter.PerkId("PERK_ITEM_SPECIAL_LIFESTEAL_WEAPON") &&
                    Math.Abs(enchantment.Aspect.Value - (3639d / 100 * level + 60)) < 0.0000001,
                    "The actual DE128 callback changed the archived Lifesteal formula.");
                var repeated = actualGrant.Configure(level);
                Check(repeated != configuration && repeated.Enchantments != configuration.Enchantments &&
                    repeated.Enchantments[0].Aspect == enchantment.Aspect, "Reward results are shared or nondeterministic.");
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
    return {level=context.player_level, enchantments={{perk=perk, aspect=123.45}}}
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
                    recovered.Enchantments[0].Aspect == 123.45,
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
            bool ownedFamily = patch.Subtype == "ChineseSwords" && catalog.Moves.Count == 35 &&
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
        var archivedItems = ReadXml(Path.Combine(repository, "Assets/DExml/list.xml"));
        foreach (XmlElement item in archivedItems.SelectNodes("/List/Items/Item[@Type='Weapon' or @Type='Armor' or @Type='Helm' or @Type='Ranged' or @Type='Magic']"))
        {
            if (_items.SelectSingleNode("/List/Items/Item[@Name='" + item.GetAttribute("Name") + "']") != null) continue;
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
        DE128ShopTests.Run(mod, enabled, repository, Check);
        DE128EquipmentTests.Run(mod, enabled, repository, Check);
        CheckSharedMovePatches(mod, enabled, fixture, repository);
        CheckShopContracts(source, fixture, mod, enabled);
        CheckCombatEquipment(source, fixture, repository, mod, enabled);
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

        foreach (string module in new[] { "timers", "equipment", "combat_equipment", "chinese_swords", "chinese_swords_data", "restored_weapons", "restored_equipment", "sphere1", "sphere2", "sphere3", "combo_sphere3", "shared_moves", "shop", "rewards", "combat_perks", "progression" })
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
