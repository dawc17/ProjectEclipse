using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

// Compares every Underworld definition the actual DE128 package registers with the
// reviewed owner raid XML plus historical DE template and localization XML.
// Reads sources directly; it does not reuse the generator. The documented
// normalizations are asserted explicitly: Rounds 0 -> 1, restored Sphere1/2,
// the music table, and unresolved names.
internal static class DE128UnderworldTests
{
    private static Action<bool, string> _check;
    private static int _compared;
    private static readonly string[] Languages = { "cro", "eng", "fra", "ger", "hin", "hun", "ita", "kor", "por", "rom", "rus", "spa", "swe", "tur" };
    private static readonly HashSet<string> OwnedMusic = new HashSet<string>
        { "fight_halloween2019", "flying_rocks", "halls_of_the_dead_heroes", "ninja_in_the_night_old" };
    private static readonly Dictionary<string, string> Music = new Dictionary<string, string>
    {
        { "vulcan", "raids_vulcan" }, { "crystal", "raids_crystal" }, { "fungus", "raids_fungus" }, { "vortex", "raids_vortex" },
        { "fatum", "raids_fatum" }, { "hunger", "raids_hunger" }, { "drakaina", "raids_war" }, { "fear", "raids_fear" },
        { "holyman7", "raids_arkhos" }, { "holyman8", "raids_hoaxen" }, { "dark_ritual", "fight38_dark_ritual" },
        { "fight_halloween2022", "hw22" }, { "raid_newyear18", "new_year_18" },
    };
    private static readonly Dictionary<string, string> Restored = new Dictionary<string, string>
    {
        { "Sphere1", "de128:items/magic/minor_charge_of_darkness" }, { "Sphere2", "de128:items/magic/medium_charge_of_darkness" },
        { "BODY_BERSTUUK", "de128:items/armor/berstuuk_form" }, { "HEAD_BERSTUUK", "de128:items/helm/berstuuk_mask" },
    };
    private static readonly HashSet<string> OwnedButtons = new HashSet<string>
    {
        "BattleBtnArchitect", "BattleBtnHalloween", "BattleBtnLamb", "BattleBtnNrityu", "BattleBtnPuppeteer",
        "BattleBtnRakshasa", "BattleBtnRavana", "BattleBtnShurale", "BattleBtnSnowflake", "BattleBtnWindWolf",
        "BattleBtnPrince",
    };
    private static readonly HashSet<string> OwnedAvatars = new HashSet<string>
    {
        "boss_architect_hummer_new", "boss_arkhos_hardmode_new", "boss_bison_hard_new",
        "boss_crystal_hardmode_new", "boss_fatum_hardmode_new", "boss_fire_hardmode_new",
        "boss_hoaxen_hardmode_new", "boss_hunger_hardmode_new", "boss_lamb_fungus_hard_new",
        "boss_lamb_hard_new", "boss_lamb_hunger_hard_new", "boss_mushroom_hardmode_new",
        "boss_rakshasa_hardmode_new", "boss_ravana_hard_new", "boss_saturn_hard_new",
        "boss_tenebris_hardmode_new", "boss_vortex_hardmode_new", "boss_war_hardmode_new",
        "boss_whisper_hardmode_new", "new_man_shuang_gou_hardmode_new",
    };
    private static readonly Dictionary<string, string> CoreStageAliases = new Dictionary<string, string>
    {
        { "ARMOR_IM_CEREMONIAL", "ARMOR_CEREMONIAL" },
        { "HELM_IM_CEREMONIAL", "HELM_CEREMONIAL" },
        { "RANGED_NEEDLES", "RANGED_NEEDLE" },
    };

    private static ModContentCatalog _catalog;
    private static Dictionary<string, string> _coreItemTypes;
    private static HashSet<string> _corePerks, _coreTemplates;
    private static Dictionary<string, XmlElement> _templates;

    private static void Check(bool value, string message) { _compared++; _check(value, message); }
    private static string Key(string key) => ("de128:localization/uw." + key).ToLowerInvariant();
    private static string Lower(string name) => new string(name.Select(c => char.IsLetterOrDigit(c) || c == '_' ? char.ToLowerInvariant(c) : '_').ToArray());
    private static string Avatar(string name) => OwnedAvatars.Contains(name)
        ? "de128:sprites/underworld/" + name.ToLowerInvariant() : name;
    private static float F(XmlElement e, string attribute) => float.Parse(e.GetAttribute(attribute), CultureInfo.InvariantCulture);
    private static bool Near(double a, double b) => Math.Abs(a - b) <= 1e-4 * Math.Max(1, Math.Abs(b));

    public static int Run(ModContentCatalog catalog, string repository, Action<bool, string> check)
    {
        _catalog = catalog; _check = check; _compared = 0;
        XmlDocument Read(string relative) { var d = new XmlDocument { XmlResolver = null }; d.Load(Path.Combine(repository, relative)); return d; }
        var raid = Read("ResearchSources/de128_assets/gamedata/raid_stages_default.xml");
        _templates = Read("Assets/DExml/stages.xml").SelectNodes("//Templates/Template").Cast<XmlElement>()
            .GroupBy(t => t.GetAttribute("Name")).ToDictionary(g => g.Key, g => g.First());
        _coreTemplates = new HashSet<string>(Read("Assets/vanillaXml/stages.xml").SelectNodes("//Templates/Template").Cast<XmlElement>().Select(t => t.GetAttribute("Name")));
        _coreItemTypes = new Dictionary<string, string>();
        foreach (XmlElement item in Read("Assets/vanillaXml/list.xml").SelectNodes("/List/Items/Item"))
            if (item.HasAttribute("Type") && !_coreItemTypes.ContainsKey(item.GetAttribute("Name"))) _coreItemTypes[item.GetAttribute("Name")] = item.GetAttribute("Type");
        string aliases = File.ReadAllText(Path.Combine(repository, "Assets/Scripts/Eclipse/Content/ItemListCompatibility.cs"));
        foreach (var pair in CoreStageAliases)
        {
            Check(aliases.Contains("{ \"" + pair.Key + "\", \"" + pair.Value + "\" }"), "Core stage alias changed: " + pair.Key);
            _coreItemTypes.Add(pair.Key, _coreItemTypes[pair.Value]);
        }
        _corePerks = new HashSet<string>(Read("Assets/vanillaXml/perks.xml").SelectNodes("//Perk").Cast<XmlElement>().Select(p => p.GetAttribute("Name")));

        var zones = raid.SelectNodes("/Stages/Zones/Zone").Cast<XmlElement>().ToArray();
        Check(zones.Length == 8, "Archive raid zone count changed");
        var keys = new HashSet<string>();
        for (int tier = 1; tier <= zones.Length; tier++)
        {
            var zoneXml = zones[tier - 1];
            var zoneId = DefinitionId.Parse("de128:zones/underworld_tier_" + tier);
            Check(catalog.TryGetZone(zoneId, out var zone) && zone.Underworld && zone.FileName == zoneXml.GetAttribute("FileName"),
                "Underworld page differs: " + zoneXml.GetAttribute("Name"));
            Check(catalog.TryGetLocalization(DefinitionId.Parse("de128:localization/zones/underworld_tier_" + tier), out var zoneTitle),
                "Underworld page title missing: " + zoneXml.GetAttribute("Name"));
            CheckTranslations(repository, zoneXml.GetAttribute("Name"), zoneTitle);
            var battles = zoneXml.SelectNodes("Battle").Cast<XmlElement>().ToArray();
            Check(zone.Battles.Count == battles.Length, "Underworld page battle count differs: " + zoneXml.GetAttribute("Name"));
            foreach (var battleXml in battles) CompareBattle(zone, battleXml, battles, keys);
        }
        foreach (string key in keys)
        {
            Check(catalog.TryGetLocalization(DefinitionId.Parse(Key(key)), out var localization), "Underworld text missing: " + key);
            CheckTranslations(repository, key, localization);
        }
        return _compared;
    }

    private static readonly Dictionary<string, Dictionary<string, string>> Words = new Dictionary<string, Dictionary<string, string>>();
    private static void CheckTranslations(string repository, string key, LocalizationDefinition localization)
    {
        foreach (string language in Languages)
        {
            if (!Words.TryGetValue(language, out var words))
            {
                words = new Dictionary<string, string>();
                var doc = new XmlDocument { XmlResolver = null };
                doc.Load(Path.Combine(repository, "Assets/DExml/localizations/" + language + ".xml"));
                foreach (XmlElement word in doc.GetElementsByTagName("Word"))
                    if (!words.ContainsKey(word.GetAttribute("Title"))) words[word.GetAttribute("Title")] = word.InnerText;
                Words[language] = words;
            }
            if (!words.TryGetValue(key, out var expected)) continue;
            Check(localization.TryGet(language, out var actual) && actual == expected, "Translation differs: " + language + " " + key);
        }
    }

    private static void CompareBattle(ZoneDefinition zone, XmlElement xml, XmlElement[] siblings, HashSet<string> keys)
    {
        string name = xml.GetAttribute("Name");
        var id = DefinitionId.Parse("de128:battles/uw_" + Lower(name));
        Check(_catalog.TryGetBattle(id, out var battle) && battle.Zone == zone.Id, "Missing Underworld battle " + name);
        var kind = xml.GetAttribute("Type") == "SURVIVAL" ? ModBattleKind.Survival : ModBattleKind.Final;
        Check(battle.Kind == kind && battle.X == int.Parse(xml.GetAttribute("X")) && battle.Y == int.Parse(xml.GetAttribute("Y")),
            "Battle type/position differs: " + name);
        keys.Add(xml.GetAttribute("Alias")); keys.Add(xml.GetAttribute("Title"));
        Check(battle.Alias == Key(xml.GetAttribute("Alias")) && battle.Title == Key(xml.GetAttribute("Title")), "Battle alias/title differs: " + name);
        string description = xml.GetAttribute("Description");
        int brace = description.IndexOf('{');
        string descriptionKey = brace < 0 ? description : description.Substring(0, brace);
        if (description.Length != 0) keys.Add(descriptionKey);
        Check(battle.Description == (description.Length == 0 ? string.Empty : Key(descriptionKey) + (brace < 0 ? string.Empty : description.Substring(brace))),
            "Battle description differs: " + name);
        // Owner decision: Faradeya uses Dandy's high-resolution dojo_india25 (see generator).
        string location = xml.GetAttribute("Location") == "dojo_india24" ? "dojo_india25" : xml.GetAttribute("Location");
        Check(battle.Location == location && battle.Preview == xml.GetAttribute("Preview") && battle.Icon == xml.GetAttribute("Icon"),
            "Battle location/preview/icon differs: " + name);
        string music = xml.GetAttribute("Music");
        // Ids no packaged track provides ship with DE128 as audio assets under their archive id.
        string expected = OwnedMusic.Contains(music) ? "de128:audio/underworld/" + music
            : Music.TryGetValue(music, out var mapped) ? mapped : music;
        Check(battle.Music == expected, "Battle music mapping differs: " + name + " (" + battle.Music + ")");
        string atlas = xml.GetAttribute("IconAtlas");
        if (OwnedButtons.Contains(atlas))
            Check(battle.Icons != null && battle.IconAtlas == string.Empty &&
                battle.Icons.Base == "de128:sprites/underworld/" + atlas.ToLowerInvariant() + "_base" &&
                battle.Icons.Active == "de128:sprites/underworld/" + atlas.ToLowerInvariant() + "_active" && battle.Icons.Locked == string.Empty,
                "Shipped map button differs: " + name);
        else
            Check(battle.Icons == null && battle.IconAtlas == atlas, "Core map button differs: " + name);
        bool twin = name.EndsWith("_HARDMODE", StringComparison.Ordinal);
        bool hasTwin = siblings.Any(s => s.GetAttribute("Name") == name + "_HARDMODE");
        var expectedMode = twin ? ModPowerMode.Power : hasTwin ? ModPowerMode.Normal : ModPowerMode.Always;
        Check(battle.PowerMode == expectedMode, "Power Mode visibility differs: " + name);
        var fights = xml.SelectNodes("Fight").Cast<XmlElement>().ToArray();
        Check(battle.Fights.Count == fights.Length, "Fight count differs: " + name);
        for (int i = 0; i < fights.Length; i++) CompareFight(battle, fights[i], name, keys);
    }

    private static void CompareFight(BattleDefinition battle, XmlElement xml, string battleName, HashSet<string> keys)
    {
        string prefix = "uw_" + Lower(battleName) + "_" + Lower(xml.GetAttribute("Name"));
        Check(_catalog.TryGetFight(DefinitionId.Parse("de128:fights/" + prefix), out var fight) && fight.Battle == battle.Id, "Missing fight " + prefix);
        int rounds = int.Parse(xml.GetAttribute("Rounds"));
        Check(fight.Rounds == (rounds == 0 ? 1 : rounds) && fight.RoundTime == int.Parse(xml.GetAttribute("RoundTime")) &&
            fight.Replays == int.Parse(xml.GetAttribute("Replays")) && fight.Power == int.Parse(xml.GetAttribute("Power")) &&
            Near(fight.HealthRecovery, xml.HasAttribute("HealthRecovery") ? F(xml, "HealthRecovery") : 1), // native default 1
            "Fight settings differ: " + prefix + " rounds=" + fight.Rounds + " time=" + fight.RoundTime + " replays=" + fight.Replays + " power=" + fight.Power + " hr=" + fight.HealthRecovery);
        var rewards = xml.SelectNodes("Rewards/Reward").Cast<XmlElement>().ToArray();
        Check(fight.Rewards.Count == rewards.Length, "Reward slot count differs: " + prefix);
        for (int i = 0; i < rewards.Length; i++)
        {
            Check(_catalog.TryGetReward(fight.Rewards[i], out var reward), "Missing reward " + fight.Rewards[i]);
            var r = rewards[i];
            Check(reward.Gems == (r.HasAttribute("Bonus") ? int.Parse(r.GetAttribute("Bonus")) : 0) &&
                reward.Experience == (r.HasAttribute("Exp") ? int.Parse(r.GetAttribute("Exp")) : 0) &&
                reward.PrizeBase.HasValue == r.HasAttribute("PrizeBase") && (!reward.PrizeBase.HasValue || Near(reward.PrizeBase.Value, F(r, "PrizeBase"))) &&
                reward.Items.Count == 0 && reward.Choices.Count == 0, "Reward differs: " + prefix + " slot " + i);
            var currencies = r.SelectNodes("Currency").Cast<XmlElement>().ToArray();
            Check(reward.Currencies.Count == currencies.Length && currencies.Select((c, j) =>
                reward.Currencies[j].Currency == c.GetAttribute("Name") && Near(reward.Currencies[j].ExpectedValue, F(c, "ExpectedValue")) &&
                reward.Currencies[j].ShowReward == (c.GetAttribute("ShowReward") == "1") && c.GetAttribute("Drop") == "1").All(ok => ok),
                "Currency drops differ: " + prefix + " slot " + i);
        }
        var warriors = xml.SelectNodes("Warriors/Warrior").Cast<XmlElement>().ToArray();
        Check(fight.Warriors.Count == warriors.Length, "Opponent count differs: " + prefix);
        for (int i = 0; i < warriors.Length; i++)
        {
            Check(_catalog.TryGetWarrior(fight.Warriors[i], out var warrior), "Missing warrior " + fight.Warriors[i]);
            CompareWarrior(warrior, warriors[i], prefix + " opponent " + (i + 1), keys);
        }
        var rules = xml.SelectSingleNode("Rules").ChildNodes.OfType<XmlElement>().Where(Emits).ToArray();
        Check(fight.Rules.Count == rules.Length, "Rule count differs: " + prefix);
        for (int i = 0; i < rules.Length; i++) CompareRule(fight.Rules[i], rules[i], prefix + " rule " + (i + 1), keys);
    }

    private static string Template(string name) => _coreTemplates.Contains(name)
        ? "core:warrior-templates/" + name.ToLowerInvariant() : "de128:warrior-templates/uw_" + Lower(name);

    private static void CompareWarrior(WarriorDefinition warrior, XmlElement xml, string where, HashSet<string> keys)
    {
        string template = xml.GetAttribute("Template");
        Check(warrior.HasTemplate && warrior.Template.ToString() == Template(template), "Opponent template differs: " + where);
        // The archived Aggressive tactic can skip Fly for a full survival wave.
        // This exact fighter inherits Aggressive and selects Fly when native conditions permit.
        string tactic = where == "uw_survival_demon_1 opponent 4" && template == "Boss_Wasp_Young" &&
            xml.GetAttribute("Tactic") == "Aggressive" ? "de128:tactics/wasp_fly" : xml.GetAttribute("Tactic");
        if (where == "uw_survival_demon_1 opponent 3" && template == "Boss_Butcher_Young" &&
            xml.GetAttribute("Tactic") == "Aggressive") tactic = "de128:tactics/butcher_earthquake";
        if (where == "uw_survival_demon_1 opponent 2" && template == "Boss_Hermit_Young" &&
            xml.GetAttribute("Tactic") == "Aggressive") tactic = "de128:tactics/hermit_storm";
        if (template == "Girl_Drakaina" && xml.GetAttribute("Tactic") == "Aggressive")
            tactic = "de128:tactics/war_whirl";
        if (template == "Cyborg_Gatekeeper" && xml.GetAttribute("Tactic") == "Aggressive")
            tactic = "de128:tactics/gatekeeper_power_field";
        if (template == "Girl_Blackness" && xml.GetAttribute("Tactic") == "Aggressive")
            tactic = "de128:tactics/blackness_grasp";
        if (template == "Girl_Saturn" && xml.GetAttribute("Tactic") == "Aggressive")
        {
            var set = xml.SelectSingleNode("Perks/Perk[@Name='PERK_SUPER_BLASTER']/Set") as XmlElement;
            Check(set != null && set.GetAttribute("InitialFrames") == "300", "Saturn's opening delay differs: " + where);
            tactic = set.GetAttribute("Frames") == "550" ? "de128:tactics/saturn_blaster_power" :
                set.GetAttribute("Frames") == "660" ? "de128:tactics/saturn_blaster" :
                throw new Exception("Saturn's recast delay differs: " + where);
        }
        if (template == "Man_Dandy" && xml.GetAttribute("Tactic") == "Aggressive")
        {
            var set = xml.SelectSingleNode("Perks/Perk[@Name='PERK_LIGHTING_CHAIN']/Set") as XmlElement;
            tactic = set == null ? "de128:tactics/dandy_lightning_chain" :
                set.GetAttribute("Frames") == "500" ? "de128:tactics/dandy_lightning_chain_power" :
                throw new Exception("Dandy's recast delay differs: " + where);
        }
        if (template == "Man_Hoaxen" && xml.GetAttribute("Tactic") == "Aggressive")
            tactic = "de128:tactics/hoaxen_tentacles";
        if (template == "Man_Stalker" && xml.GetAttribute("Tactic") == "Aggressive")
        {
            var set = xml.SelectSingleNode("Perks/Perk[@Name='PERK_HUNTERFLY']/Set") as XmlElement;
            tactic = set?.GetAttribute("Frames") == "900" ? "de128:tactics/hunter_fly" :
                set?.GetAttribute("Frames") == "800" ? "de128:tactics/hunter_fly_power" :
                throw new Exception("Hunter's archived Fly delay differs: " + where);
        }
        if (template == "Man_Berstuuk" && xml.GetAttribute("Tactic") == "Aggressive")
            tactic = "de128:tactics/berstuuk_root_potion";
        if (template == "Man_Arkhos" && xml.GetAttribute("Tactic") == "Aggressive")
            tactic = "de128:tactics/arkhos_rat_wave";
        if (template == "Man_Tenebris" && xml.GetAttribute("Tactic") == "Aggressive")
            tactic = "de128:tactics/tenebris_fear_ray";
        var teleportPerk = xml.SelectSingleNode("Perks/Perk[@Name='PERK_TELEPORTATION']") as XmlElement;
        if (teleportPerk != null)
        {
            var set = teleportPerk.SelectSingleNode("Set") as XmlElement;
            string frames = set?.GetAttribute("Frames") ?? string.Empty;
            tactic = "de128:tactics/" + (frames == "300" ? "widow_teleportation_fast" :
                frames == "480" ? "widow_teleportation_power" :
                frames == "660" ? "widow_teleportation_slow" : "widow_teleportation");
        }
        Check(warrior.Tactic == tactic && warrior.Avatar == Avatar(xml.GetAttribute("Avatar")) &&
            warrior.HealthBars == (xml.HasAttribute("ShieldTotal") ? int.Parse(xml.GetAttribute("ShieldTotal")) : 0), "Opponent fields differ: " + where);
        var attributes = new Dictionary<string, float>();
        foreach (var attribute in new[] { "MagicInitialCharge", "WarriorPower" })
            if (xml.HasAttribute(attribute)) attributes[attribute] = F(xml, attribute);
        Check(warrior.Attributes.Count == attributes.Count && attributes.All(p => warrior.Attributes.TryGetValue(p.Key, out var v) && Near(v, p.Value)),
            "Opponent attributes differ: " + where);
        var deltas = xml.SelectNodes("AttributesAlign/Delta").Cast<XmlElement>().ToArray();
        Check(warrior.AttributeAlignments.Count == deltas.Length && deltas.Select((d, j) => Near(warrior.AttributeAlignments[j].Factor, F(d, "Factor")) &&
            Near(warrior.AttributeAlignments[j].Shift, F(d, "Shift")) && warrior.AttributeAlignments[j].Priority == int.Parse(d.GetAttribute("Priority"))).All(ok => ok),
            "Opponent alignments differ: " + where);
        ComparePerks(warrior.PerkLoadout.ToArray(), xml.SelectNodes("Perks/Perk").Cast<XmlElement>(), where);
        CompareItems(warrior, xml, where);
        if (_catalog.TryGetWarriorTemplate(warrior.Template, out var owned) && owned.Body != null) CompareTemplate(owned, template, keys);
    }

    private static readonly HashSet<string> ComparedTemplates = new HashSet<string>();
    private static void CompareTemplate(WarriorTemplateDefinition template, string name, HashSet<string> keys)
    {
        if (!ComparedTemplates.Add(name)) return;
        var xml = _templates[name];
        var body = template.Body;
        string parent = xml.GetAttribute("Template");
        Check(parent.Length == 0 ? !body.HasTemplate : body.HasTemplate && body.Template.ToString() == Template(parent), "Template parent differs: " + name);
        if (xml.HasAttribute("FirstName")) keys.Add(xml.GetAttribute("FirstName"));
        Check(body.FirstName == (xml.HasAttribute("FirstName") ? Key(xml.GetAttribute("FirstName")) : string.Empty) &&
            body.Avatar == Avatar(xml.GetAttribute("Avatar")) && body.Voice == xml.GetAttribute("Voice") &&
            body.HealthBars == (xml.HasAttribute("ShieldTotal") ? int.Parse(xml.GetAttribute("ShieldTotal")) : 0), "Template fields differ: " + name);
        var expected = xml.Attributes.Cast<XmlAttribute>().Where(a => !new[] { "Name", "Template", "FirstName", "Avatar", "Voice", "ShieldTotal" }.Contains(a.Name)).ToArray();
        Check(body.Attributes.Count == expected.Length && expected.All(a => body.Attributes.TryGetValue(a.Name, out var v) &&
            Near(v, float.Parse(a.Value, CultureInfo.InvariantCulture))), "Template attributes differ: " + name);
        CompareItems(body, xml, "template " + name);
        if (parent.Length != 0 && !_coreTemplates.Contains(parent) &&
            _catalog.TryGetWarriorTemplate(body.Template, out var parentDefinition)) CompareTemplate(parentDefinition, parent, keys);
    }

    private static void CompareItems(WarriorDefinition warrior, XmlElement xml, string where)
    {
        var refs = new List<string>(); string skeleton = string.Empty;
        foreach (XmlElement item in xml.SelectNodes("Items/Item"))
        {
            string name = item.GetAttribute("Name");
            if (Restored.TryGetValue(name, out var restored)) { if (!refs.Contains(restored)) refs.Add(restored); continue; }
            if (!_coreItemTypes.TryGetValue(name, out var type)) continue; // unresolved in the core catalog
            if (type == "Skeleton") { skeleton = name; continue; }
            string id = ("core:items/" + type.ToLowerInvariant() + "/" + name).ToLowerInvariant();
            if (!refs.Contains(id)) refs.Add(id);
        }
        Check(warrior.Items.Select(i => i.ToString()).SequenceEqual(refs) && warrior.Skeleton == skeleton, "Items differ: " + where);
    }

    private static void ComparePerks(WarriorPerkDefinition[] actual, IEnumerable<XmlElement> xml, string where)
    {
        var expected = xml.Where(p => _corePerks.Contains(p.GetAttribute("Name"))).ToArray();
        Check(actual.Length == expected.Length, "Perk count differs: " + where);
        for (int i = 0; i < Math.Min(actual.Length, expected.Length); i++)
        {
            var row = actual[i]; var perk = expected[i];
            var settings = perk.SelectNodes("Set").Cast<XmlElement>().SelectMany(s => s.Attributes.Cast<XmlAttribute>()).ToDictionary(a => a.Name, a => a.Value);
            double? Get(string name) => settings.TryGetValue(name, out var v) ? double.Parse(v, CultureInfo.InvariantCulture) : (double?)null;
            bool Same(double? a, double? b) => a.HasValue == b.HasValue && (!a.HasValue || Near(a.Value, b.Value));
            var extra = settings.Keys.Where(k => k != "Aspect" && k != "Chance" && k != "ChanceFactor" && k != "Frames").ToArray();
            var expectedFrames = Get("Frames");
            if (perk.GetAttribute("Name") == "PERK_LIGHTING_CHAIN" && !expectedFrames.HasValue)
                expectedFrames = 600; // reviewed DE perk base; core default is 300
            if (perk.GetAttribute("Name") == "PERK_TELEPORTATION" && !expectedFrames.HasValue)
                expectedFrames = 600; // reviewed DE perk base; core default is 300
            Check(row.Perk.ToString() == ("core:perks/" + perk.GetAttribute("Name")).ToLowerInvariant() &&
                Same(row.Aspect, Get("Aspect")) && Same(row.Chance, Get("Chance")) && Same(row.ChanceFactor, Get("ChanceFactor")) &&
                Same(row.Frames, expectedFrames) && row.Parameters.Count == extra.Length &&
                extra.All(k => row.Parameters.TryGetValue(k, out var v) && Near(v, double.Parse(settings[k], CultureInfo.InvariantCulture))),
                "Perk settings differ: " + where + " " + perk.GetAttribute("Name"));
        }
    }

    // Children that project: only genuinely unresolved perks drop out.
    private static bool Emits(XmlElement xml)
    {
        switch (xml.Name)
        {
            case "Perk": return _corePerks.Contains(xml.GetAttribute("Name"));
            case "Description": return false;
            case "ComplexRule": case "RandomRule": return xml.ChildNodes.OfType<XmlElement>().Any(Emits);
            case "EquipItem": return _coreItemTypes.ContainsKey(xml.GetAttribute("Name")) || Restored.ContainsKey(xml.GetAttribute("Name"));
            default: return true;
        }
    }

    private static ModRuleTarget Target(XmlElement xml)
    {
        switch (xml.GetAttribute("ApplyTo")) { case "Player": return ModRuleTarget.Player; case "Bot": return ModRuleTarget.Opponent; default: return ModRuleTarget.All; }
    }

    private static void CompareRule(DefinitionId id, XmlElement xml, string where, HashSet<string> keys)
    {
        Check(_catalog.TryGetFightRule(id, out var rule), "Missing rule " + id);
        switch (xml.Name)
        {
            case "RulesWithConditions":
                Check(rule.Kind == ModFightRuleKind.Behavior && id.LocalId == "sensei_raid_charge_conditional", "RaidCharge rule not shared: " + where);
                return;
            case "Perk":
                var parameters = xml.SelectNodes("Set").Cast<XmlElement>().SelectMany(s => s.Attributes.Cast<XmlAttribute>()).Where(a => a.Name != "Aspect").ToArray();
                var aspect = xml.SelectSingleNode("Set/@Aspect");
                Check(rule.Kind == ModFightRuleKind.Perk && rule.Perk.ToString() == ("core:perks/" + xml.GetAttribute("Name")).ToLowerInvariant() &&
                    rule.Target == Target(xml) && rule.PerkAspect.HasValue == (aspect != null) &&
                    (aspect == null || Near(rule.PerkAspect.Value, double.Parse(aspect.Value, CultureInfo.InvariantCulture))) &&
                    rule.PerkParameters.Count == parameters.Length &&
                    parameters.All(a => rule.PerkParameters.TryGetValue(a.Name, out var v) && Near(v, double.Parse(a.Value, CultureInfo.InvariantCulture))),
                    "Perk rule differs: " + where);
                return;
            case "ComplexRule":
            case "RandomRule":
                var children = xml.ChildNodes.OfType<XmlElement>().Where(Emits).ToArray();
                Check(rule.Kind == (xml.Name == "ComplexRule" ? ModFightRuleKind.Group : ModFightRuleKind.Random) &&
                    rule.Group.Children.Count == children.Length, "Rule group differs: " + where);
                if (xml.Name == "ComplexRule")
                {
                    var description = xml.SelectSingleNode("Description/@Alias");
                    if (description != null) keys.Add(description.Value);
                    Check(rule.Group.Description == (description == null ? string.Empty : Key(description.Value)), "Rule description differs: " + where);
                }
                else
                    Check(rule.Group.Refresh == (xml.GetAttribute("Refresh") == "EachRound" ? ModRuleRefresh.EachRound : ModRuleRefresh.EachFight),
                        "Random rule refresh differs: " + where);
                for (int i = 0; i < Math.Min(children.Length, rule.Group.Children.Count); i++)
                    CompareRule(rule.Group.Children[i], children[i], where + "." + (i + 1), keys);
                return;
            case "HotGround":
                var nodes = xml.SelectNodes("Node").Cast<XmlElement>().ToArray();
                Check(rule.Kind == ModFightRuleKind.HotGround && rule.Trial.Frames == int.Parse(xml.GetAttribute("Frames")) && rule.Target == Target(xml) &&
                    rule.Trial.Nodes.Count == nodes.Length && nodes.Select((n, j) => rule.Trial.Nodes[j].Name == n.GetAttribute("Name") &&
                        rule.Trial.Nodes[j].Axis.ToString() == n.GetAttribute("Axis") &&
                        rule.Trial.Nodes[j].Maximum.HasValue == n.HasAttribute("Max") && rule.Trial.Nodes[j].Minimum.HasValue == n.HasAttribute("Min")).All(ok => ok) &&
                    rule.Trial.Animations.SequenceEqual(xml.SelectNodes("Animation").Cast<XmlElement>().Select(a => a.GetAttribute("Name"))),
                    "Hot ground differs: " + where);
                return;
            case "RandomArea":
                Check(rule.Kind == ModFightRuleKind.RandomArea && rule.Group.Image == xml.GetAttribute("Image") && rule.Group.Icon == xml.GetAttribute("Icon") &&
                    Near(rule.Group.Width, F(xml, "Width")) && rule.Group.FadeIn == int.Parse(xml.GetAttribute("FadeIn")) &&
                    rule.Group.FramesOn == int.Parse(xml.GetAttribute("FramesOn")) && rule.Group.FadeOut == int.Parse(xml.GetAttribute("FadeOut")) &&
                    rule.Group.FramesOff == int.Parse(xml.GetAttribute("FramesOff")) && rule.Target == Target(xml), "Random area differs: " + where);
                return;
            case "LightInTheDarkness":
                Check(rule.Kind == ModFightRuleKind.LightInTheDarkness && rule.Target == Target(xml) &&
                    Near(rule.Group.LightRadius, F(xml, "LightRadius")) &&
                    Near(rule.Group.LightShape, F(xml, "LightShape")), "Spotlight rule differs: " + where);
                return;
            case "NoAnimation":
                Check(rule.Kind == ModFightRuleKind.NoAnimation && rule.Trial.Node == xml.GetAttribute("Name"), "No-animation differs: " + where);
                return;
            case "RemoveInterval":
                Check(rule.Kind == ModFightRuleKind.RemoveInterval && rule.Trial.IntervalType.ToString() == xml.GetAttribute("Type") && rule.Target == Target(xml),
                    "Remove-interval differs: " + where);
                return;
            case "Regeneration":
                Check(rule.Kind == ModFightRuleKind.Regeneration && Near(rule.Trial.Rate, F(xml, "Rate")) &&
                    rule.Trial.FramesAfterHit == int.Parse(xml.GetAttribute("FramesAfterHit")) && rule.Target == Target(xml), "Regeneration differs: " + where);
                return;
            case "Attributes":
                var values = xml.Attributes.Cast<XmlAttribute>().Where(a => a.Name != "ApplyTo").ToArray();
                Check(rule.Kind == ModFightRuleKind.Attributes && rule.Target == Target(xml) && rule.Attributes.Count == values.Length &&
                    values.All(a => rule.Attributes.TryGetValue(a.Name, out var v) && Near(v, float.Parse(a.Value, CultureInfo.InvariantCulture))),
                    "Attributes rule differs: " + where);
                return;
            case "EquipItem":
                string expectedItem = Restored.TryGetValue(xml.GetAttribute("Name"), out var restored) ? restored
                    : ("core:items/" + _coreItemTypes[xml.GetAttribute("Name")].ToLowerInvariant() + "/" + xml.GetAttribute("Name")).ToLowerInvariant();
                Check(rule.Kind == ModFightRuleKind.EquipItem && rule.Item.ToString() == expectedItem && rule.Target == Target(xml), "Equip rule differs: " + where);
                return;
            case "Avatar": case "Name": case "NoButton":
                var kind = xml.Name == "Avatar" ? ModFightRuleKind.Avatar : xml.Name == "Name" ? ModFightRuleKind.Name : ModFightRuleKind.NoButton;
                Check(rule.Kind == kind && rule.Name == xml.GetAttribute("Name") && rule.Target == Target(xml), "Named rule differs: " + where);
                return;
            case "NoHealthBar": case "InvertJoystick":
                Check(rule.Kind == (xml.Name == "NoHealthBar" ? ModFightRuleKind.NoHealthBar : ModFightRuleKind.InvertJoystick) && rule.Target == Target(xml),
                    "Flag rule differs: " + where);
                return;
            default:
                Check(false, "Unexpected archived rule " + xml.Name + " at " + where);
                return;
        }
    }
}
