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
        { "policy.services", "policy.timers", "content.register", "content.patch", "combat.modify_outgoing_hit", "combat.effects" };
    private static readonly DefinitionId Sword = DefinitionId.Parse("de128:items/weapon/titans_desolator");
    private static readonly DefinitionId CoreSword = CoreContentImporter.WeaponId("WEAPON_TITAN_GIANT_SWORD");
    private static XmlDocument _items;
    private static XmlDocument _perks;
    private static XmlDocument _stages;
    private static Dictionary<string, XmlDocument> _languages;
    private static int _checks;

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
            else return false;
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
        CoreContentImporter.ImportRanged(catalog, _items.SelectNodes("/List/Items/Item").Cast<XmlNode>(), _languages);
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

    private static ModDescriptor Peer(string fixture, string id, string capability, string script)
    {
        string modsRoot = Path.Combine(fixture, "peers", id, "Mods");
        string directory = Path.Combine(modsRoot, id);
        Directory.CreateDirectory(Path.Combine(directory, "scripts"));
        File.WriteAllText(Path.Combine(directory, "mod.toml"),
            "schema = 1\nid = \"" + id + "\"\nname = \"Foundation test peer\"\n" +
            "version = \"1.0.0\"\nauthors = [\"Eclipse tests\"]\nentrypoint = \"scripts/main.lua\"\n" +
            "capabilities = [\"" + capability + "\"]\n\n[[dependencies]]\nid = \"core\"\nversion = \">=1.0 <2.0\"\n", Utf8);
        File.WriteAllText(Path.Combine(directory, "scripts", "main.lua"), "local sf2 = require(\"sf2\")\n" + script, Utf8);
        return Discover(modsRoot, id);
    }

    private static void Load(ModDescriptor mod, ModContentCatalog catalog, string missingAsset = null)
    {
        using (LoadLive(mod, catalog, missingAsset)) { }
    }

    private static IModScriptContext LoadLive(ModDescriptor mod, ModContentCatalog catalog, string missingAsset = null)
    {
        ImportCore(catalog);
        // Actual canonical projections and controlled art metadata; no profile is bound.
        var assets = new AssetResolver(new IAssetProvider[] { new CoreMetadata(missingAsset), new LooseModProvider(mod) });
        using (var transaction = catalog.BeginRegistration(mod))
        {
            var api = new ModApiFacade(mod, assets, transaction, new ModStateRuntime(), null);
            var context = new MoonSharpScriptRuntime().CreateContext(mod, api);
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
        Check(Services.All(ModPolicies.FeatureEnabled), "A failed or absent mod left service disables behind.");
        if (catalog != null)
        {
            Check(!catalog.TryGetItem(Sword, out _), "Failed or absent DE128 left its weapon registered.");
            Check(catalog.ItemInnatePerks.Count == 0, "Failed or absent DE128 left innate perks registered.");
            Check(catalog.Patches.Count == 0, "Failed or absent DE128 left content patches registered.");
        }
    }

    private static void CheckDE(ModContentCatalog catalog)
    {
        ModPolicies.Content = catalog;
        Check(catalog.TimerPolicies.Count == 1 && catalog.TryGetTimer("forge", out var timer) &&
            timer.Owner.Value == "de128", "The forge policy is not exclusively owned by DE128.");
        Check(ModPolicies.DeliverySeconds("forge", 120) == 0, "New forge orders are not instant.");
        Check(ModPolicies.SkipEnabled("forge"), "Already-pending orders lost their normal skip path.");
        Check(Services.All(service => !ModPolicies.FeatureEnabled(service)), "A DE service gate is missing.");
        Check(ModPolicies.FeatureEnabled("campaign"), "An unrelated feature was disabled.");
        Check(ModPolicies.DeliverySeconds("shop", 120) == 120, "An unrelated timer was modified.");
        Check(catalog.TryGetItem(Sword, out var definition) && definition is WeaponDefinition,
            "The actual package did not register Desolator.");
        var weapon = (WeaponDefinition)definition;
        Check(weapon.SubType == "TitanGiantSword" && weapon.Progression == ItemProgressionKind.Vanilla,
            "Desolator lost its move family or canonical progression profile.");
        Check(weapon.Icon.ToString() == "core:ui/items/weapon17.img_weapon_boss_giant_sword" &&
            weapon.Model.ToString() == "core:gamedata/models/mdl_weapon_giant_sword", "Desolator art IDs changed.");
        Check(catalog.Weapons.Count(item => !item.IsCore) == 1 && catalog.ShopListings.Count == 0,
            "DE128 added unexpected equipment or a purchasable listing.");
        Check(catalog.ItemInnatePerks.Count == 1 && catalog.ItemInnatePerks[0].Item == Sword &&
            catalog.ItemInnatePerks[0].Entries.Select(entry => entry.Perk).SequenceEqual(new[] {
                CoreContentImporter.PerkId("PERK_TITAN"), CoreContentImporter.PerkId("PERK_ANTI_SHOCK") }),
            "Desolator innate loadout changed.");
        Check(catalog.TryGetLocalization(weapon.DisplayName, out var title) &&
            title.Id.Namespace.Value == "de128" && title.GetOrEnglish("eng") == "Titan's Desolator",
            "Desolator's mod-owned English title is missing.");
        Check(catalog.Rewards.Count == 1 && catalog.TryGetReward(
            DefinitionId.Parse("de128:rewards/titans_desolator"), out var reward) &&
            reward.Items.Count == 1 && reward.Items[0].Item == Sword && reward.Items[0].UsesConfiguration &&
            reward.Choices.Count == 0 && reward.Gems == 0 && catalog.ItemDefaultEnchantments.Count == 0,
            "Desolator must use one configured reward without changing equipment defaults or currencies.");
        Check(catalog.TryGetFight(DefinitionId.Parse("core:fights/zone_7/c3_boss_titan_eclipsemode/6"), out var titan) &&
            titan.RewardDrops.Count == 1 && catalog.Fights.Count(fight => fight.RewardDrops.Count != 0) == 1,
            "DE128 must patch only the final Eclipse Titan reward.");
        var drop = titan.RewardDrops[0];
        Check(drop.ResultIndex == 1 && drop.Mode == ModRuleMode.Eclipse && !drop.MinimumLevel.HasValue &&
            !drop.MaximumLevel.HasValue && drop.Reward.Id.ToString() == "de128:rewards/titans_desolator",
            "Desolator reward changed its winning slot, mode or level gate.");
        Check(catalog.Modes.Count == 0 && catalog.Warriors.Count == 0 && catalog.FightRules.Count == 0 &&
            !catalog.Fights.Any(fight => !fight.IsCore) && catalog.Quests.Count == 0,
            "Disabled Ascension registered live modes, opponents, fights, rules or quests.");
        Check(!catalog.Localizations.Any(value => value.Id.Namespace.Value == "de128" &&
            (value.Id.LocalId.StartsWith("ascension") || value.Id.LocalId == "zones/ascension")),
            "Disabled Ascension registered live localization.");
        Check(catalog.Perks.Count(perk => !perk.IsCore) == 2 && catalog.Behaviors.Count == 2,
            "DE combat perk definitions are missing or unexpected behaviors were registered.");
        foreach (int level in new[] { 4, 8, 11, 14, 17 })
            Check(catalog.TryGetProgressionBranch(level, out var branch) && branch.Entries.Count == 2,
                "XML-evidenced DE perk branch is missing at " + level);
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

    private static void Run(string source, string fixture, string repository)
    {
        _items = ReadXml(Path.Combine(repository, "Assets/vanillaXml/list.xml"));
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

        CheckBase(null);
        var enabled = new ModContentCatalog();
        Load(mod, enabled);
        CheckDE(enabled);
        string fingerprint = ModSaveData.ComputeContentSetFingerprint(new[] { mod }, enabled);

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
            var rejected = new ModContentCatalog();
            ExpectFailure(restricted, rejected, missing);
            Check(rejected.TimerPolicies.Count == 0, "Missing capability left a committed timer.");
            CheckBase(rejected);
        }

        foreach (string module in new[] { "timers", "equipment", "rewards", "combat_perks", "progression" })
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
        CheckLocalizationReferences(fixture);
        CheckRewardConfiguration(mod, fixture);
        CheckTrialFingerprints(fixture);
        DECombatPerksTests.Run(mod, repository, (descriptor, content) => LoadLive(descriptor, content), Check);
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
