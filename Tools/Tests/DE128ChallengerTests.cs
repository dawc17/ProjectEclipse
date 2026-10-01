using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

// Compares the seven Challenger definitions the actual DE128 package registers
// (challengers.lua) with the owner stages.xml, quests.xml and perks.xml plus the
// archived localization tables, read directly here. RunStory then plays the
// package's unlock and weapon-drop flow through the production MoonSharp bindings
// with controlled profile, map, scene and dialog hosts. Native combat, reward
// settlement and dialog rendering are not exercised.
internal static class DE128ChallengerTests
{
    private static readonly string[] Languages = { "cro", "eng", "fra", "ger", "hin", "hun", "ita", "kor", "por", "rom", "rus", "spa", "swe", "tur" };
    private const string Titan = "core:fights/zone_7/c3_boss_titan/6";
    // The DE-only child perk and the core perk the reward grants in its place.
    private const string Stranger = "PERK_ITEM_SPECIAL_BLOODRAGE_WEAPON_STRANGER";
    private const string StrangerBase = "PERK_ITEM_SPECIAL_BLOODRAGE_WEAPON";

    private sealed class Challenger
    {
        public string Zone, Battle, Key, Weapon, Image, Drop;
        public XmlElement Xml, Grant;
        public string BattleId => "de128:battles/challenger_" + Key;
        public string FightId => "de128:fights/challenger_" + Key + "_1";
        public string WeaponId => ("core:items/weapon/" + Weapon).ToLowerInvariant();
    }

    private static Action<bool, string> _check;
    private static int _checks;
    private static void Check(bool value, string message) { _checks++; _check(value, message); }
    private static string Key(string key) => ("de128:localization/challenger." + key).ToLowerInvariant();
    private static string Sprite(string name) => "de128:sprites/challenger/" + name.ToLowerInvariant();
    private static float F(XmlElement e, string attribute) => float.Parse(e.GetAttribute(attribute), CultureInfo.InvariantCulture);
    private static bool Near(double a, double b) => Math.Abs(a - b) <= 1e-4 * Math.Max(1, Math.Abs(b));

    private static XmlDocument Read(string repository, string relative)
    {
        var document = new XmlDocument { XmlResolver = null };
        document.Load(Path.Combine(repository, relative));
        return document;
    }

    // Archive Challengers with their DropWeapon_<Name> quests, in stages.xml order.
    private static List<Challenger> Archive(string repository)
    {
        var stages = Read(repository, "ResearchSources/de128_assets/gamedata/stages.xml");
        var quests = Read(repository, "ResearchSources/de128_assets/gamedata/quests.xml");
        var drops = quests.SelectNodes("//Quest[starts-with(@Name, 'DropWeapon_')]").Cast<XmlElement>()
            .ToDictionary(q => ((XmlElement)q.SelectSingleNode("Conditions/Equal[@Value1='_$Fight']")).GetAttribute("Value2"));
        var result = new List<Challenger>();
        foreach (XmlElement zone in stages.SelectNodes("/Stages/Zones/Zone"))
            foreach (XmlElement battle in zone.SelectNodes("Battle[starts-with(@Name, 'Challenger')]"))
            {
                var quest = drops[zone.GetAttribute("Name") + "|" + battle.GetAttribute("Name") + "|1"];
                var perk = (XmlElement)quest.SelectSingleNode("Actions/GivePerk");
                var dialog = (XmlElement)quest.SelectSingleNode("Actions/Dialog");
                result.Add(new Challenger
                {
                    Zone = zone.GetAttribute("Name"), Battle = battle.GetAttribute("Name"), Key = battle.GetAttribute("Icon"),
                    Xml = battle, Weapon = perk.GetAttribute("Item"), Grant = (XmlElement)perk.SelectSingleNode("Perk"),
                    Image = dialog.GetAttribute("Image"), Drop = ((XmlElement)dialog.SelectSingleNode("Line")).GetAttribute("Text"),
                });
                // Every drop quest has the same shape; only these fields vary.
                Check(quest.SelectSingleNode("Conditions/Equal[@Value1='_$FightResult' and @Value2='Win']") != null &&
                    quest.SelectSingleNode("Conditions/Equal[@Value1='?Item[" + perk.GetAttribute("Item") + "].Quantity' and @Value2='1' and @Not='1']") != null &&
                    ((XmlElement)quest.SelectSingleNode("Actions/GiveItem")).GetAttribute("Name") ==
                        "?Concat[" + perk.GetAttribute("Item") + "|,((100 * ?Player[].Level))]" &&
                    perk.GetAttribute("ApplyTo") == "Item" && dialog.GetAttribute("Title") == perk.GetAttribute("Item") &&
                    ((XmlElement)dialog.SelectSingleNode("Button")).GetAttribute("Text") == "OK" &&
                    ((XmlElement)dialog.SelectSingleNode("Button/OpenShop")).GetAttribute("Item") == perk.GetAttribute("Item"),
                    "Archive drop quest shape changed: " + quest.GetAttribute("Name"));
            }
        Check(result.Count == 7 && drops.Count == 7, "Archive Challenger count changed");
        var unlock = (XmlElement)quests.SelectSingleNode("//Quest[@Name='UnlockChallengerBattles']");
        Check(unlock.SelectSingleNode("Conditions/GreaterEqual[@Value1='?Fight[ZONE_7|C3_BOSS_TITAN|6].WinCount' and @Value2='1']") != null &&
            unlock.SelectNodes("Actions/ShowBattle").Cast<XmlElement>().Select(s => s.GetAttribute("Name") + s.GetAttribute("Locked"))
                .SequenceEqual(result.Select(c => c.Zone + "|" + c.Battle + "|0")),
            "Archive Challenger unlock changed");
        return result;
    }

    public static int RunContent(ModContentCatalog catalog, string repository, Action<bool, string> check)
    {
        _check = check; _checks = 0;
        var challengers = Archive(repository);
        var stages = Read(repository, "ResearchSources/de128_assets/gamedata/stages.xml");
        var templates = stages.SelectNodes("//Templates/Template").Cast<XmlElement>().ToDictionary(t => t.GetAttribute("Name"));
        var coreTypes = new Dictionary<string, string>();
        foreach (XmlElement item in Read(repository, "Assets/vanillaXml/list.xml").SelectNodes("/List/Items/Item"))
            if (item.HasAttribute("Type") && !coreTypes.ContainsKey(item.GetAttribute("Name"))) coreTypes[item.GetAttribute("Name")] = item.GetAttribute("Type");
        var ownerPerks = Read(repository, "ResearchSources/de128_assets/gamedata/perks.xml").SelectNodes("//Perk").Cast<XmlElement>()
            .GroupBy(p => p.GetAttribute("Name")).ToDictionary(g => g.Key, g => g.First());
        var corePerks = Read(repository, "Assets/vanillaXml/perks.xml").SelectNodes("//Perk").Cast<XmlElement>()
            .GroupBy(p => p.GetAttribute("Name")).ToDictionary(g => g.Key, g => g.First());
        Dictionary<string, string> Set(XmlElement perk) => perk.SelectNodes("Set").Cast<XmlElement>()
            .SelectMany(s => s.Attributes.Cast<XmlAttribute>()).ToDictionary(a => a.Name, a => a.Value);

        // The _STRANGER perk exists only in DE: core Bloodrage's child with Base,
        // DamageFactor and Chance set to exactly the values the drop quests repeat.
        var stranger = ownerPerks[Stranger];
        Check(!corePerks.ContainsKey(Stranger) && corePerks.TryGetValue(StrangerBase, out var strangerBase) &&
            stranger.GetAttribute("Template") == strangerBase.GetAttribute("Template") &&
            Set(strangerBase).All(p => Set(stranger).TryGetValue(p.Key, out var v) && v == p.Value),
            "Archive Stranger Bloodrage no longer extends the core weapon perk");

        var historical = new Dictionary<string, XmlElement>();
        foreach (var name in new[] { "ZONE_6|Challenger", "ZONE_6|Challenger_2" })
        {
            var parts = name.Split('|');
            historical[name] = (XmlElement)Read(repository, "Assets/DExml/stages.xml")
                .SelectSingleNode("//Zone[@Name='" + parts[0] + "']/Battle[@Name='" + parts[1] + "']");
        }
        Check(historical["ZONE_6|Challenger"].GetAttribute("X") == "-455" && historical["ZONE_6|Challenger"].GetAttribute("Y") == "130" &&
            historical["ZONE_6|Challenger_2"].GetAttribute("X") == "-225" && historical["ZONE_6|Challenger_2"].GetAttribute("Y") == "-245",
            "Historical Ronin/Nova positions changed");
        var keys = new HashSet<string> { "OK" };
        var rules = new List<DefinitionId>();
        foreach (var c in challengers)
        {
            var xml = c.Xml;
            string where = c.Zone + "|" + c.Battle;
            // Ronin and Nova follow the historical positions, which match the shipped game.
            var position = historical.TryGetValue(c.Zone + "|" + c.Battle, out var old) ? old : xml;
            Check(catalog.TryGetBattle(DefinitionId.Parse(c.BattleId), out var battle) &&
                battle.Zone.ToString() == "core:zones/" + c.Zone.ToLowerInvariant() && battle.Kind == ModBattleKind.Final &&
                xml.GetAttribute("Type") == "FINAL_BATTLE" &&
                battle.X == int.Parse(position.GetAttribute("X")) && battle.Y == int.Parse(position.GetAttribute("Y")),
                "Challenger battle placement differs: " + where);
            foreach (var attribute in new[] { "Alias", "Title", "Description" }) keys.Add(xml.GetAttribute(attribute));
            Check(battle.Alias == Key(xml.GetAttribute("Alias")) && battle.Title == Key(xml.GetAttribute("Title")) &&
                battle.Description == Key(xml.GetAttribute("Description")) && battle.Icon == c.Key &&
                battle.Location == xml.GetAttribute("Location") && battle.Preview == Sprite(xml.GetAttribute("Preview")) &&
                battle.Music == "de128:audio/challenger/" + xml.GetAttribute("Music") && battle.EclipseToggleName == string.Empty,
                "Challenger battle presentation differs: " + where);
            string atlas = xml.GetAttribute("IconAtlas");
            Check(atlas == "BattleBtn" + char.ToUpperInvariant(c.Key[0]) + c.Key.Substring(1) && battle.IconAtlas == string.Empty &&
                battle.Icons != null && battle.Icons.Base == Sprite(atlas + "_base") && battle.Icons.Active == Sprite(atlas + "_active") &&
                battle.Icons.Locked == string.Empty, "Challenger map button differs: " + where);

            var fightXml = (XmlElement)xml.SelectSingleNode("Fight");
            Check(catalog.TryGetFight(DefinitionId.Parse(c.FightId), out var fight), "Missing Challenger fight: " + where);
            Check(xml.SelectNodes("Fight").Count == 1 && fightXml.GetAttribute("Name") == "1" && fight.Battle == battle.Id &&
                fight.Rounds == int.Parse(fightXml.GetAttribute("Rounds")) && fight.RoundTime == int.Parse(fightXml.GetAttribute("RoundTime")) &&
                fight.Replays == int.Parse(fightXml.GetAttribute("Replays")) && fight.Power == int.Parse(fightXml.GetAttribute("Power")),
                "Challenger fight settings differ: " + where);

            var rewards = fightXml.SelectNodes("Rewards/Reward").Cast<XmlElement>().ToArray();
            Check(rewards.Length == 2 && fight.Rewards.Count == 2, "Challenger reward slots differ: " + where);
            for (int i = 0; i < rewards.Length; i++)
            {
                var r = rewards[i];
                Check(catalog.TryGetReward(fight.Rewards[i], out var reward) &&
                    reward.Gems == (r.HasAttribute("Bonus") ? int.Parse(r.GetAttribute("Bonus")) : 0) &&
                    reward.Experience == (r.HasAttribute("Exp") ? int.Parse(r.GetAttribute("Exp")) : 0) &&
                    reward.PrizeBase.HasValue && Near(reward.PrizeBase.Value, F(r, "PrizeBase")) &&
                    reward.Choices.Count == 0 && reward.Currencies.Count == 0 && reward.Items.Count == (i == 1 ? 1 : 0),
                    "Challenger reward differs: " + where + " slot " + i);
                if (i == 1) CompareGrant(reward, c, Set(c.Grant), where);
            }

            var warriors = fightXml.SelectNodes("Warriors/Warrior").Cast<XmlElement>().ToArray();
            Check(warriors.Length == 1 && fight.Warriors.Count == 1, "Challenger opponent count differs: " + where);
            Check(catalog.TryGetWarrior(fight.Warriors[0], out var warrior), "Missing Challenger opponent: " + where);
            var w = warriors[0];
            string templateName = w.GetAttribute("Template");
            Check(warrior.HasTemplate && warrior.Template.ToString() == "de128:warrior-templates/challenger_" + c.Key &&
                warrior.Tactic == w.GetAttribute("Tactic") && warrior.Attributes.Count == 2 &&
                new[] { "MagicInitialCharge", "WarriorPower" }.All(a => warrior.Attributes.TryGetValue(a, out var v) && Near(v, F(w, a))) &&
                warrior.Items.Count == 0 && warrior.Skeleton == string.Empty,
                "Challenger opponent differs: " + where);
            var deltas = w.SelectNodes("AttributesAlign/Delta").Cast<XmlElement>().ToArray();
            Check(warrior.AttributeAlignments.Count == deltas.Length && deltas.Select((d, j) =>
                Near(warrior.AttributeAlignments[j].Factor, F(d, "Factor")) && Near(warrior.AttributeAlignments[j].Shift, F(d, "Shift")) &&
                warrior.AttributeAlignments[j].Priority == int.Parse(d.GetAttribute("Priority"))).All(ok => ok),
                "Challenger alignments differ: " + where);
            var perks = w.SelectNodes("Perks/Perk").Cast<XmlElement>().ToArray();
            var loadout = warrior.PerkLoadout.ToArray();
            Check(perks.Length == 1 && loadout.Length == 1 &&
                loadout[0].Perk.ToString() == ("core:perks/" + perks[0].GetAttribute("Name")).ToLowerInvariant() &&
                Set(perks[0]).Count == 1 && loadout[0].Aspect.HasValue && Near(loadout[0].Aspect.Value, F((XmlElement)perks[0].SelectSingleNode("Set"), "Aspect")) &&
                !loadout[0].Chance.HasValue && !loadout[0].ChanceFactor.HasValue && !loadout[0].Frames.HasValue && loadout[0].Parameters.Count == 0,
                "Challenger opponent enchantment differs: " + where);

            var t = templates[templateName];
            Check(catalog.TryGetWarriorTemplate(warrior.Template, out var owned) && owned.Body != null, "Challenger template missing: " + where);
            var body = owned.Body;
            keys.Add(t.GetAttribute("FirstName"));
            var extra = t.Attributes.Cast<XmlAttribute>().Where(a => !new[] { "Name", "Template", "FirstName", "Avatar", "Voice" }.Contains(a.Name)).ToArray();
            Check(t.GetAttribute("Template") == "Default" && body.HasTemplate && body.Template.ToString() == "core:warrior-templates/default" &&
                body.FirstName == Key(t.GetAttribute("FirstName")) && body.Avatar == Sprite(t.GetAttribute("Avatar")) &&
                body.Voice == t.GetAttribute("Voice") && body.Attributes.Count == extra.Length &&
                extra.All(a => body.Attributes.TryGetValue(a.Name, out var v) && Near(v, float.Parse(a.Value, CultureInfo.InvariantCulture))),
                "Challenger template fields differ: " + templateName);
            var items = new List<string>(); string skeleton = string.Empty;
            foreach (XmlElement item in t.SelectNodes("Items/Item"))
            {
                string type = coreTypes[item.GetAttribute("Name")];
                if (type == "Skeleton") skeleton = item.GetAttribute("Name");
                else items.Add(("core:items/" + type + "/" + item.GetAttribute("Name")).ToLowerInvariant());
            }
            Check(body.Items.Select(i => i.ToString()).SequenceEqual(items) && body.Skeleton == skeleton,
                "Challenger template loadout differs: " + templateName);

            var ruleXml = fightXml.SelectSingleNode("Rules").ChildNodes.OfType<XmlElement>().ToArray();
            Check(ruleXml.Select(r => r.Name + ":" + r.GetAttribute("Name") + ":" + r.GetAttribute("DamageFactor") + ":" + r.GetAttribute("ApplyTo"))
                .SequenceEqual(new[] { "Perk:PERK_ANTI_SHOCK::Bot", "Attributes::-8500:Player", "Attributes::1000:Bot", "RulesWithConditions:::" }) &&
                ruleXml[3].SelectSingleNode("Conditions/Equal[@Value1='_RaidChargeButton' and @Value2='0']") != null &&
                ruleXml[3].SelectSingleNode("RuleList/NoButton[@Name='RaidCharge']") != null && fight.Rules.Count == 4,
                "Archive Challenger rules changed: " + where);
            if (rules.Count == 0) rules.AddRange(fight.Rules);
            Check(fight.Rules.SequenceEqual(rules), "Challenger fights do not share their rules: " + where);
        }
        Check(catalog.TryGetFightRule(rules[0], out var antiShock) && antiShock.Kind == ModFightRuleKind.Perk &&
            antiShock.Perk.ToString() == "core:perks/perk_anti_shock" && antiShock.Target == ModRuleTarget.Opponent &&
            !antiShock.PerkAspect.HasValue && antiShock.PerkParameters.Count == 0, "Challenger anti-shock rule differs");
        Check(catalog.TryGetFightRule(rules[1], out var player) && player.Kind == ModFightRuleKind.Attributes &&
            player.Target == ModRuleTarget.Player && player.Attributes.Count == 1 && Near(player.Attributes["DamageFactor"], -8500) &&
            catalog.TryGetFightRule(rules[2], out var bot) && bot.Kind == ModFightRuleKind.Attributes &&
            bot.Target == ModRuleTarget.Opponent && bot.Attributes.Count == 1 && Near(bot.Attributes["DamageFactor"], 1000),
            "Challenger damage rules differ");
        Check(catalog.TryGetFightRule(rules[3], out var charge) && charge.Kind == ModFightRuleKind.Behavior &&
            rules[3].LocalId == "sensei_raid_charge_conditional", "Challenger RaidCharge rule is not the shared conditional rule");

        foreach (var c in challengers) { keys.Add(c.Weapon); keys.Add(c.Drop); }
        Check(keys.Count == 37, "Challenger text key set changed: " + keys.Count);
        var words = Languages.ToDictionary(language => language, language =>
        {
            var result = new Dictionary<string, string>();
            foreach (XmlElement word in Read(repository, "Assets/DExml/localizations/" + language + ".xml").GetElementsByTagName("Word"))
                if (!result.ContainsKey(word.GetAttribute("Title"))) result[word.GetAttribute("Title")] = word.InnerText;
            return result;
        });
        foreach (string key in keys)
        {
            Check(catalog.TryGetLocalization(DefinitionId.Parse(Key(key)), out var localization), "Challenger text missing: " + key);
            foreach (string language in Languages)
                Check(localization.TryGet(language, out var actual) && actual == words[language][key],
                    "Challenger translation differs: " + language + " " + key);
        }
        return _checks;
    }

    // DropWeapon_<Name>: GiveItem at 100 * player level, then GivePerk with Aspect
    // 3639 / 100 * level + 60 and the quest's other Set values.
    private static void CompareGrant(RewardDefinition reward, Challenger c, Dictionary<string, string> set, string where)
    {
        string perk = c.Grant.GetAttribute("Name") == Stranger ? StrangerBase : c.Grant.GetAttribute("Name");
        Check(set["Aspect"] == "3639 / 100 * ?Player[].Level + 60", "Archive drop aspect formula changed: " + where);
        double? Number(string name) => set.TryGetValue(name, out var v) ? double.Parse(v, CultureInfo.InvariantCulture) : (double?)null;
        var parameters = set.Where(p => p.Key != "Aspect" && p.Key != "Chance" && p.Key != "Frames").ToArray();
        Check(reward.Items[0].Item.ToString() == c.WeaponId && reward.Items[0].UpgradeNumber == 0 && reward.Items[0].UsesConfiguration,
            "Challenger weapon grant differs: " + where);
        foreach (int level in new[] { 1, 30, 52 })
        {
            var configuration = reward.Items[0].Configure(level);
            Check(configuration.Level == level && configuration.Enchantments.Count == 1, "Challenger grant level differs: " + where);
            var e = configuration.Enchantments[0];
            Check(e.Perk == CoreContentImporter.PerkId(perk) && e.Aspect.HasValue && Near(e.Aspect.Value, 3639d / 100 * level + 60) &&
                !e.ChanceFactor.HasValue && e.Chance.HasValue == Number("Chance").HasValue &&
                (!e.Chance.HasValue || Near(e.Chance.Value, Number("Chance").Value)) &&
                e.Frames.HasValue == Number("Frames").HasValue && (!e.Frames.HasValue || e.Frames.Value == (int)Number("Frames").Value) &&
                e.Parameters.Count == parameters.Length &&
                parameters.All(p => e.Parameters.TryGetValue(p.Key, out var v) && Near(v, double.Parse(p.Value, CultureInfo.InvariantCulture))),
                "Challenger grant enchantment differs: " + where + " level " + level);
        }
    }

    public static int RunStory(ModDescriptor mod, string repository,
        Func<ModDescriptor, ModContentCatalog, ModStoryEvents, ModStateRuntime, IModScriptContext> load, Action<bool, string> check)
    {
        _check = check; _checks = 0;
        var challengers = Archive(repository);
        var english = new Dictionary<string, string>();
        foreach (XmlElement word in Read(repository, "Assets/DExml/localizations/eng.xml").GetElementsByTagName("Word"))
            if (!english.ContainsKey(word.GetAttribute("Title"))) english[word.GetAttribute("Title")] = word.InnerText;

        var catalog = new ModContentCatalog();
        var errors = new List<string>();
        var bus = new ModStoryEvents((owner, message) => errors.Add(message));
        var state = new ModStateRuntime();
        var dialogs = new FakeDialogHost(catalog);
        var wins = new Dictionary<string, int>();
        var reveals = new List<string>();
        var scenes = new List<string>();
        bool mapReady = true;
        ModProfileAccess.Fight = id => new ModProfileFightSnapshot(true, wins.TryGetValue(id.ToString(), out var value) ? value : 0, 0);
        ModBattleAccess.Reveal = (id, locked) => { if (!mapReady) return false; reveals.Add(id + ":" + locked); return true; };
        ModSceneAccess.Open = scene => { scenes.Add(scene); return true; };
        ModUnderworldAccess.SetToggleVisible = visible => true;
        try
        {
            using (var context = load(mod, catalog, bus, state))
            {
                var save = new XmlDocument();
                save.LoadXml("<Warrior><EclipseMods schema='1'><Mod id='de128'/></EclipseMods></Warrior>");
                Check(state.Bind(save.DocumentElement, new[] { context }).Count == 0, "Challenger state did not bind");
                bus.BindProfile();
                string Pending() => state.TryGetValue(mod.Id, "challenger_drops_pending", out var value) ? value.String : null;
                bool Revealed() => state.TryGetValue(mod.Id, "challengers_revealed", out var value) && value.Boolean;
                void Scene(string scene)
                {
                    if (scene != "map") foreach (var view in dialogs.Views.ToArray()) view.Close(ModUiCloseReason.Scene);
                    bus.Publish(new ModStoryEvent(ModStoryEventKind.SceneEnter, null, scene: scene));
                }
                void Result(string id, string outcome)
                {
                    if (outcome == "win") wins[id] = wins.TryGetValue(id, out var count) ? count + 1 : 1;
                    bus.Publish(new ModStoryEvent(ModStoryEventKind.BattleResult, null,
                        battle: new ModBattleResultSnapshot(DefinitionId.Parse(id), outcome, false)));
                }
                void Acquire(Challenger c, int previous = 0) =>
                    bus.Publish(new ModStoryEvent(ModStoryEventKind.ItemAcquired, DefinitionId.Parse(c.WeaponId), previousCount: previous, count: previous + 1));
                void Expect(Challenger c, string where)
                {
                    var view = dialogs.Live;
                    Check(view != null && view.Read("speaker").Text == english[c.Weapon] && view.Read("body").Text == english[c.Drop] &&
                        view.Read("continue").Text == english["OK"] && view.Request.Lines.Count == 1 &&
                        view.Request.Portrait == Sprite(c.Image) && !view.Request.Mirrored,
                        where + ": drop dialog differs for " + c.Key);
                }

                // UnlockChallengerBattles: nothing before a normal Titan win.
                Scene("map");
                Check(reveals.Count == 0 && !Revealed() && dialogs.Live == null, "Challengers appeared before Titan was beaten");
                wins["core:fights/zone_7/c3_boss_titan_eclipsemode/6"] = 1;
                Scene("map");
                Check(reveals.Count == 0, "An Eclipse Titan win revealed the Challengers");
                // A refused reveal is retried on the next map entry.
                wins[Titan] = 1; mapReady = false; Scene("map");
                Check(!Revealed(), "A refused reveal was recorded");
                mapReady = true; Scene("map");
                Check(Revealed() && reveals.SequenceEqual(challengers.Select(c => c.BattleId + ":False")),
                    "Titan win did not reveal all seven Challengers unlocked: " + string.Join(",", reveals));
                Scene("map");
                Check(reveals.Count == 7, "Challengers were revealed again");

                // DropWeapon_<Name>: a win that granted the weapon shows its dialog on the map, then opens the shop.
                var first = challengers[0];
                Scene("fight"); Acquire(first); Result(first.FightId, "win");
                Check(dialogs.Live == null && Pending() == "," + first.Key + ",", "Drop dialog was not deferred to the map");
                Scene("map"); Expect(first, "Grant before result");
                Scene("shop");
                Check(Pending() == "," + first.Key + "," && scenes.Count == 0, "An interrupted drop dialog was dropped");
                Scene("map"); Expect(first, "Resumed drop");
                Check(dialogs.Live.TryClick("continue") && Pending() == "," && scenes.SequenceEqual(new[] { "shop" }),
                    "Drop dialog did not open the shop");
                Scene("shop"); Scene("map");
                Check(dialogs.Live == null, "Drop dialog repeated");

                // Result reported before the grant; two pending drops open the shop once.
                var second = challengers[1]; var third = challengers[2];
                Scene("fight"); Result(second.FightId, "win"); Acquire(second);
                Scene("fight"); Acquire(third); Result(third.FightId, "win");
                Check(Pending() == "," + second.Key + "," + third.Key + ",", "Pending drops were not both recorded");
                scenes.Clear(); Scene("map"); Expect(second, "First of two");
                Check(dialogs.Live.TryClick("continue") && scenes.Count == 0, "Shop opened before the last drop dialog");
                Expect(third, "Second of two");
                Check(dialogs.Live.TryClick("continue") && scenes.SequenceEqual(new[] { "shop" }), "Shop did not open after the last drop");

                // No dialog when nothing was granted: owned weapon, a loss, a purchase before the fight,
                // another fight's grant, or an existing copy gaining count.
                var fourth = challengers[3]; var fifth = challengers[4];
                Scene("fight"); Result(fourth.FightId, "win");
                Scene("fight"); Acquire(fourth); Result(fourth.FightId, "loss");
                Scene("shop"); Acquire(fourth); Scene("fight"); Result(fourth.FightId, "win");
                Scene("fight"); Acquire(fourth); Result(fifth.FightId, "win");
                Scene("fight"); Acquire(fifth, 1); Result(fifth.FightId, "win");
                Scene("fight"); Acquire(fourth); Result("core:fights/zone_1/boss_lynx/1", "win");
                Scene("map");
                Check(Pending() == "," && dialogs.Live == null, "A win without a new weapon showed a drop dialog: " + Pending());
            }
            Check(dialogs.Live == null, "Challenger teardown leaked a dialog");
            Check(errors.Count == 0, "Challenger story errors: " + string.Join("\n", errors));
        }
        finally
        {
            ModProfileAccess.Clear(); ModBattleAccess.Clear(); ModSceneAccess.Clear(); ModUnderworldAccess.Clear(); ModStoryDialogAccess.Clear();
        }
        return _checks;
    }
}
