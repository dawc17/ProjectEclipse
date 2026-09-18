using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

internal static class DE128AscensionTests
{
    private static int _checks;
    private static void Check(bool value, string message) { _checks++; if (!value) throw new Exception(message); }

    private sealed class CoreMetadata : IAssetProvider
    {
        public ModId Namespace => ModId.Parse("core");
        public bool TryDescribe(AssetId id, out AssetMetadata metadata)
        {
            metadata = null;
            AssetKind kind;
            if (id.Path == "gamedata/models/mdl_weapon_giant_sword") kind = AssetKind.Model;
            else if (id.Path == "ui/items/weapon17.img_weapon_boss_giant_sword") kind = AssetKind.Sprite;
            else return false;
            metadata = new AssetMetadata(id, kind, AssetSourceKind.Core, string.Empty, -1, "DE128 Ascension fixture exact core metadata");
            return true;
        }
    }

    private static XmlDocument ReadXml(string path)
    {
        var document = new XmlDocument { XmlResolver = null };
        using (var reader = XmlReader.Create(path, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null }))
            document.Load(reader);
        return document;
    }

    private static void ImportCore(ModContentCatalog catalog, string repository)
    {
        XmlDocument items = ReadXml(Path.Combine(repository, "Assets", "vanillaXml", "list.xml"));
        XmlDocument perks = ReadXml(Path.Combine(repository, "Assets", "vanillaXml", "perks.xml"));
        XmlDocument stages = ReadXml(Path.Combine(repository, "Assets", "vanillaXml", "stages.xml"));
        var languages = CoreContentImporter.ReadLocalizations(Path.Combine(repository, "Assets", "vanillaXml", "localizations"));
        var nodes = items.SelectNodes("/List/Items/Item").Cast<XmlNode>();
        CoreContentImporter.ImportWeapons(catalog, nodes, languages);
        CoreContentImporter.ImportArmors(catalog, items.SelectNodes("/List/Items/Item").Cast<XmlNode>(), languages);
        CoreContentImporter.ImportHelms(catalog, items.SelectNodes("/List/Items/Item").Cast<XmlNode>(), languages);
        CoreContentImporter.ImportRanged(catalog, items.SelectNodes("/List/Items/Item").Cast<XmlNode>(), languages);
        CoreContentImporter.ImportMagic(catalog, items.SelectNodes("/List/Items/Item").Cast<XmlNode>(), languages);
        CoreContentImporter.ImportPerks(catalog, perks.DocumentElement.ChildNodes.Cast<XmlNode>());
        CoreContentImporter.ImportStages(catalog, stages.DocumentElement["Zones"]);
        CoreContentImporter.ImportWarriorTemplates(catalog, stages.SelectSingleNode("Stages/Warriors/Templates"));
    }

    private static ModDescriptor Discover(string modsRoot)
    {
        var discovered = ModDiscovery.DiscoverLoose(modsRoot);
        Check(!discovered.HasErrors, "DE128 discovery failed: " + string.Join(" | ", discovered.Diagnostics));
        var resolved = DependencyResolver.Resolve(discovered.Mods, ModPlatformVersions.Core);
        Check(!resolved.HasErrors, "DE128 dependency resolution failed: " + string.Join(" | ", resolved.Diagnostics));
        return resolved.OrderedMods.Single(m => m.Id.Value == "de128");
    }

    private sealed class Harness : IDisposable
    {
        internal readonly ModContentCatalog Catalog = new ModContentCatalog();
        internal readonly ModStateRuntime State = new ModStateRuntime();
        internal readonly ModDescriptor Mod;
        internal readonly IModScriptContext Script;
        internal readonly ModModeDefinition Mode;
        internal readonly XmlDocument Save = new XmlDocument { XmlResolver = null };
        internal readonly DE128Projection Projection;
        internal XmlElement LastEncounter;
        internal ModModeRequest LastRequest;
        internal Action Ready;

        internal Harness(string repository, string modsRoot)
        {
            Mod = Discover(modsRoot);
            ImportCore(Catalog, repository);
            var assets = new AssetResolver(new IAssetProvider[] { new CoreMetadata(), new LooseModProvider(Mod) });
            var tx = Catalog.BeginRegistration(Mod);
            Script = new MoonSharpScriptRuntime().CreateContext(Mod, new ModApiFacade(Mod, assets, tx, State, null));
            ModLocalizationLoader.Load(Mod, assets, tx);
            Script.ExecuteEntrypoint();
            tx.Commit();
            Catalog.Freeze();
            Mode = Catalog.Modes.Single(m => m.Id.ToString() == "de128:modes/ascension_trials");
            Check(Mode.Fights.Count == 5 && Mode.Repeatable && Mode.ResetOnLoss && !Mode.HasEntryItem && Mode.UsesPrepareCallback,
                "DE128 Ascension mode metadata changed or gained an entry charge.");
            foreach (DefinitionId id in Mode.Fights)
            {
                Check(Catalog.TryGetFight(id, out var fight), "Missing Ascension fight " + id);
                Check(fight.Power == 0 && fight.Rounds == 1 && fight.RoundTime == 150 && fight.Rewards.Count == 2,
                    "Ascension blueprint must stay Power=0, 1 round, 150 seconds and two empty result rewards.");
                foreach (DefinitionId rewardId in fight.Rewards)
                {
                    Catalog.TryGetReward(rewardId, out var reward);
                    Check(reward != null && reward.Items.Count == 0 && reward.Choices.Count == 0 && reward.Gems == 0,
                        "Ascension preview unexpectedly pays out a reward.");
                }
                Catalog.TryGetBattle(fight.Battle, out var battle);
                string runtime = Catalog.RuntimeFightId(id);
                ListSF.Fights[runtime] = new FightList { FightId = new FightIDS(runtime), Battle = new Battle { Name = battle.LegacyName } };
            }
            Save.LoadXml("<Warrior/>");
            ModSaveData.RecordContext(Save.DocumentElement, new[] { Mod }, Catalog, State);
            State.Bind(Save.DocumentElement, new[] { Script });
            ModPolicies.Content = Catalog;
            ModModeRuntime.Bind(Save.DocumentElement);
            Check(new ModModeProgress(Save.DocumentElement, Mode).ReadPlan() == null,
                "Fresh DE128 Ascension save unexpectedly contains a prepared encounter.");
            Projection = new DE128Projection(Catalog);
            ModModeRuntime.SchedulePreparation = (request, ready, cancel) => { LastRequest = request; Ready = ready; };
            ModModeRuntime.Prepare = (mode, step, completions, request) => {
                Check(((IModModePrepareScriptContext)Script).TryPrepareMode(mode, step, completions, request, out var error), error);
            };
            ModModeRuntime.SelectNext = (mode, won, step, completions) => {
                Check(((IModModeScriptContext)Script).TryChooseModeNext(mode, won, step, completions, out var next, out var error), error);
                return next;
            };
            ModModeRuntime.BuildEncounter = (mode, step, plan) => {
                Catalog.TryGetFight(mode.Fights[step], out var fight);
                LastEncounter = Projection.Encounter(fight, plan);
                return ListSF.Fights[Catalog.RuntimeFightId(mode.Fights[step])];
            };
        }

        internal ModEncounterPlan PrepareCurrent(bool finishReady = true)
        {
            var progress = new ModModeProgress(Save.DocumentElement, Mode);
            var entry = ListSF.Fights[Catalog.RuntimeFightId(Mode.Fights[progress.Step])];
            Check(ModModeRuntime.TryFind(entry.FightId.ToString(), out var found) && ReferenceEquals(found, Mode),
                "Mode host could not resolve the current Ascension runtime fight.");
            var existingPlan = progress.ReadPlan();
            Check(!progress.Entered && existingPlan == null,
                "Ascension prepare helper was called with existing reservation/plan. Entered=" + progress.Entered +
                " Plan=" + (existingPlan == null ? "null" : PlanIdentity(existingPlan)) + " Save=" + Save.OuterXml);
            LastRequest = null; Ready = null; LastEncounter = null;
            Check(!ModModeRuntime.PrepareEntry(entry, () => { }), "Unprepared Ascension encounter entered synchronously.");
            Check(LastRequest != null && !LastRequest.IsPending && LastRequest.Plan != null && Ready != null,
                "Ascension on_prepare did not resolve a plan.");
            ModEncounterPlan generated = LastRequest.Plan;
            if (finishReady) Ready();
            return generated;
        }

        internal FightList BeginCurrent()
        {
            var progress = new ModModeProgress(Save.DocumentElement, Mode);
            FightList fight = ListSF.Fights[Catalog.RuntimeFightId(Mode.Fights[progress.Step])];
            Check(ModModeRuntime.ResolveEntry(ref fight) && ModModeRuntime.Begin(fight), "Prepared Ascension encounter could not begin.");
            return fight;
        }

        public void Dispose()
        {
            ModModeRuntime.Clear();
            Script.Dispose();
            ModPolicies.Content = null;
            ListSF.Fights.Clear();
        }
    }

    private static string PlanIdentity(ModEncounterPlan plan) =>
        string.Join(";", plan.Warriors) + "|" + string.Join(";", plan.Rules ?? Array.Empty<DefinitionId>()) + "|" + plan.Description;

    private static void CheckPlan(Harness h, ModEncounterPlan plan, HashSet<string> opponents,
        HashSet<string> challenges, HashSet<string> buffs, HashSet<string> rangedVariants)
    {
        Check(plan.Warriors.Count == 1 && plan.Rules != null && plan.Rules.Count >= 4 && plan.Rounds == 1 && plan.RoundTime == 150,
            "Ascension generated plan lost warrior/rule/timing data.");
        string opponent = plan.Warriors[0].ToString(); opponents.Add(opponent);
        Check(opponent.StartsWith("de128:warriors/ascension_opponent_", StringComparison.Ordinal), "Unexpected Ascension opponent " + opponent);
        Check(h.Catalog.TryGetWarrior(plan.Warriors[0], out var selectedWarrior), "Selected Ascension warrior is missing from catalog.");
        foreach (DefinitionId item in selectedWarrior.Items)
            if (item == CoreContentImporter.RangedId("RANGED_SHURIKENS") || item == CoreContentImporter.RangedId("RANGED_KUNAI"))
                rangedVariants.Add(item.ToString());
        var kinds = new HashSet<ModFightRuleKind>();
        foreach (DefinitionId id in plan.Rules)
        {
            Check(h.Catalog.TryGetFightRule(id, out var rule), "Plan references missing rule " + id);
            kinds.Add(rule.Kind);
            if (id.ToString().Contains("ascension_buff_"))
            {
                buffs.Add(id.ToString());
                Check(rule.Kind == ModFightRuleKind.Perk && rule.PerkAspect == 100000d && rule.Target == ModRuleTarget.Opponent,
                    "Ascension buff lost aspect 100000/opponent target.");
            }
        }
        foreach (var pair in new[] {
            (ModFightRuleKind.EquipItem,"No weapons"), (ModFightRuleKind.HotGround,"Hot ground"),
            (ModFightRuleKind.RingOut,"Ring out"), (ModFightRuleKind.RemoveInterval,"No blocks"),
            (ModFightRuleKind.Regeneration,"Enemy regenerates"), (ModFightRuleKind.NoAnimation,"No jumps") })
            if (kinds.Contains(pair.Item1)) challenges.Add(pair.Item2);
        Check(plan.Description != null && plan.Description.StartsWith("de128:localization/ascension.trial.", StringComparison.Ordinal),
            "Ascension selected description was not persisted in the plan.");
    }

    private static void CheckNativeProjection(Harness h, ModEncounterPlan plan)
    {
        var progress = new ModModeProgress(h.Save.DocumentElement, h.Mode);
        h.Catalog.TryGetFight(h.Mode.Fights[progress.Step], out var blueprint);
        XmlElement xml = h.Projection.Encounter(blueprint, plan);
        Check(xml.GetAttribute("Power") == "0" && xml.GetAttribute("Rounds") == "1" && xml.GetAttribute("RoundTime") == "150",
            "Projected Ascension fight changed Power/rounds/time.");
        Check(xml.SelectNodes("Rewards/Reward").Count == 2 && xml.SelectNodes("Rewards/Reward/*").Count == 0,
            "Projected Ascension fight gained a payout.");
        Check(xml.SelectSingleNode("Warriors/Warrior")?.Attributes?["EclipseCharacterId"]?.Value == plan.Warriors[0].ToString(),
            "Projected Ascension opponent differs from persisted plan.");
        foreach (DefinitionId id in plan.Rules)
        {
            h.Catalog.TryGetFightRule(id, out var rule);
            if (rule.Kind == ModFightRuleKind.Behavior) continue;
            string expected = rule.Kind == ModFightRuleKind.EquipItem ? "EquipItem" :
                rule.Kind == ModFightRuleKind.HotGround ? "HotGround" :
                rule.Kind == ModFightRuleKind.RingOut ? "Ringout" :
                rule.Kind == ModFightRuleKind.Regeneration ? "Regeneration" :
                rule.Kind == ModFightRuleKind.NoAnimation ? "NoAnimation" :
                rule.Kind == ModFightRuleKind.RemoveInterval ? "RemoveInterval" : null;
            if (expected != null) Check(xml.SelectSingleNode("Rules/" + expected) != null, "Projected plan lost native challenge node " + expected);
        }
    }

    private static void CheckSaveFormats(Harness h, ModEncounterPlan live)
    {
        var doc = new XmlDocument { XmlResolver = null }; doc.LoadXml("<Warrior/>");
        var p = new ModModeProgress(doc.DocumentElement, h.Mode);
        p.SavePlan(new ModEncounterPlan(live.Warriors, rounds: 1, roundTime: 150));
        XmlElement v1 = (XmlElement)doc.SelectSingleNode("Warrior/EclipseModes/Mode/Encounter");
        Check(v1.GetAttribute("Version") == "1" && p.ReadPlan().Rules == null, "Encounter save v1 backward compatibility changed.");

        var doc2 = new XmlDocument { XmlResolver = null }; doc2.LoadXml("<Warrior/>");
        var p2 = new ModModeProgress(doc2.DocumentElement, h.Mode); p2.SavePlan(live);
        XmlElement saved = (XmlElement)doc2.SelectSingleNode("Warrior/EclipseModes/Mode/Encounter");
        Check(saved.GetAttribute("Version") == "2" && p2.ReadPlan().Rules.SequenceEqual(live.Rules) && p2.ReadPlan().Description == live.Description,
            "Encounter save v2 rules/description roundtrip failed.");
        foreach (string mutation in new[] { "version", "duplicate" })
        {
            var bad = new XmlDocument { XmlResolver = null }; bad.LoadXml(doc2.OuterXml);
            XmlElement encounter = (XmlElement)bad.SelectSingleNode("Warrior/EclipseModes/Mode/Encounter");
            if (mutation == "version") encounter.SetAttribute("Version", "99");
            else encounter.AppendChild(encounter.SelectSingleNode("Rules").CloneNode(true));
            string before = bad.OuterXml; bool rejected = false;
            try { new ModModeProgress(bad.DocumentElement, h.Mode).ReadPlan(); } catch (ModContentException) { rejected = true; }
            Check(rejected && bad.OuterXml == before, "Corrupt encounter save was mutated or accepted: " + mutation);
        }
    }

    private static void CheckBehaviorSelection(ModDescriptor mod)
    {
        var catalog = new ModContentCatalog();
        using (var tx = catalog.BeginRegistration(mod))
        {
            var behavior = tx.RegisterBehavior("fixture_behavior", new ModParameterSchema(Array.Empty<ModParameterDefinition>()));
            var blueprint = tx.RegisterBehaviorRule("blueprint", behavior.Id, ModRuleTarget.All, ModRuleMode.All, Array.Empty<int>(), null);
            var selected = tx.RegisterBehaviorRule("selected", behavior.Id, ModRuleTarget.All, ModRuleMode.All, Array.Empty<int>(), null);
            var zone = tx.RegisterZone("fixture", "fixture", false);
            var battle = tx.RegisterBattle("fixture", zone.Id, ModBattleKind.Story);
            var warrior = tx.RegisterWarrior("fixture", "", "", "", "", 1, "Standard",
                Array.Empty<DefinitionId>(), Array.Empty<DefinitionId>());
            var fight = tx.RegisterFight("fixture", battle.Id, 0, 0, 0, 1, 60, "", "", -1, 1, "", false, "",
                new[] { warrior.Id }, new[] { blueprint.Id }, Array.Empty<DefinitionId>());
            tx.Commit(); catalog.Freeze();
            var instances = new ModBattleRuleInstances();
            Check(instances.Applicable(catalog, catalog.RuntimeFightId(fight.Id), true, 1, false, new[] { selected.Id }).Single().Id == selected.Id,
                "Explicit encounter behavior rules did not replace blueprint behavior rules.");
            instances = new ModBattleRuleInstances();
            Check(instances.Applicable(catalog, catalog.RuntimeFightId(fight.Id), true, 1, false, null).Single().Id == blueprint.Id,
                "Null encounter rules did not inherit blueprint behavior rules.");
            instances = new ModBattleRuleInstances();
            Check(!instances.Applicable(catalog, catalog.RuntimeFightId(fight.Id), true, 1, false, Array.Empty<DefinitionId>()).Any(),
                "Explicit empty encounter rules did not suppress blueprint behavior rules.");
        }
    }

    public static int Main(string[] args)
    {
        if (args.Length != 2) throw new ArgumentException("Expected repository root and Mods root.");
        string repository = args[0], modsRoot = args[1];
        var opponents = new HashSet<string>();
        var challenges = new HashSet<string>();
        var buffs = new HashSet<string>();
        var rangedVariants = new HashSet<string>();
        using (var h = new Harness(repository, modsRoot))
        {
            ModEncounterPlan first = h.PrepareCurrent();
            CheckPlan(h, first, opponents, challenges, buffs, rangedVariants); CheckNativeProjection(h, first); CheckSaveFormats(h, first);
            string identity = PlanIdentity(first), saveAfterPrepare = h.Save.OuterXml;
            var reloaded = new XmlDocument { XmlResolver = null }; reloaded.LoadXml(saveAfterPrepare);
            ModModeRuntime.Bind(reloaded.DocumentElement); h.State.Bind(reloaded.DocumentElement, new[] { h.Script });
            var restored = new ModModeProgress(reloaded.DocumentElement, h.Mode).ReadPlan();
            Check(PlanIdentity(restored) == identity, "Reload after preparation rerolled the persisted plan.");
            ModModeRuntime.Bind(h.Save.DocumentElement); h.State.Bind(h.Save.DocumentElement, new[] { h.Script });

            for (int step = 0; step < 5; step++)
            {
                ModEncounterPlan plan = step == 0 ? first : h.PrepareCurrent();
                CheckPlan(h, plan, opponents, challenges, buffs, rangedVariants); CheckNativeProjection(h, plan);
                FightList fight = h.BeginCurrent();
                Check(ModModeRuntime.ActiveRules(fight.FightId.ToString()) != null &&
                    ModModeRuntime.ActiveRules(fight.FightId.ToString()).SequenceEqual(plan.Rules), "ActiveRules did not expose selected plan rules.");
                int before = new ModModeProgress(h.Save.DocumentElement, h.Mode).Step;
                ModModeRuntime.Complete(fight, true);
                int after = new ModModeProgress(h.Save.DocumentElement, h.Mode).Step;
                if (step == 0)
                {
                    var afterWinReload = new XmlDocument { XmlResolver = null }; afterWinReload.LoadXml(h.Save.OuterXml);
                    ModModeRuntime.Bind(afterWinReload.DocumentElement); h.State.Bind(afterWinReload.DocumentElement, new[] { h.Script });
                    var restoredAfterWin = new ModModeProgress(afterWinReload.DocumentElement, h.Mode);
                    Check(restoredAfterWin.Step == 1 && restoredAfterWin.ReadPlan() == null,
                        "Reload after first Ascension win lost progression or retained a stale plan.");
                    ModModeRuntime.Bind(h.Save.DocumentElement); h.State.Bind(h.Save.DocumentElement, new[] { h.Script });
                }
                ModModeRuntime.Complete(fight, true);
                Check(new ModModeProgress(h.Save.DocumentElement, h.Mode).Step == after, "One native result advanced Ascension more than once.");
                Check(step == 4 ? after == 0 : after == before + 1, "Ascension success progression is wrong at step " + (step + 1));
                Check(ModModeRuntime.ActiveRules(fight.FightId.ToString()) == null, "Complete retained active encounter rules.");
            }
            var completed = new ModModeProgress(h.Save.DocumentElement, h.Mode);
            Check(completed.Step == 0 && completed.Completions == 1, "Five wins did not complete/repeat exactly once.");
            Check(opponents.Count == 5 && challenges.Count == 5, "First run did not select five distinct opponents and five distinct challenge families.");

            // A loss from a newly prepared run resets immediately and clears active rules.
            ModEncounterPlan lossPlan = h.PrepareCurrent(); FightList lossFight = h.BeginCurrent();
            ModModeRuntime.Complete(lossFight, false);
            Check(new ModModeProgress(h.Save.DocumentElement, h.Mode).Step == 0 && ModModeRuntime.ActiveRules(lossFight.FightId.ToString()) == null,
                "Ascension loss did not reset to trial one/clear active rules.");

            // Failed launch reservation teardown also clears active rules without paying or charging anything.
            ModEncounterPlan cancelPlan = h.PrepareCurrent(); FightList cancelFight = h.BeginCurrent();
            ModModeRuntime.CancelLaunch(cancelFight);
            var cancelledProgress = new ModModeProgress(h.Save.DocumentElement, h.Mode);
            Check(ModModeRuntime.ActiveRules(cancelFight.FightId.ToString()) == null && !cancelledProgress.Entered &&
                PlanIdentity(cancelledProgress.ReadPlan()) == PlanIdentity(cancelPlan),
                "CancelLaunch retained active rules or reservation.");

            // Repeat deterministic runs until every challenge family and registered 100000-aspect buff has appeared.
            for (int run = 0; run < 24 && (challenges.Count < 6 || buffs.Count < 16); run++)
            {
                var runOpponents = new HashSet<string>(); var runChallenges = new HashSet<string>();
                for (int step = 0; step < 5; step++)
                {
                    var progress = new ModModeProgress(h.Save.DocumentElement, h.Mode);
                    ModEncounterPlan plan = progress.ReadPlan() ?? h.PrepareCurrent();
                    CheckPlan(h, plan, runOpponents, runChallenges, buffs, rangedVariants);
                    CheckNativeProjection(h, plan);
                    FightList fight = h.BeginCurrent(); ModModeRuntime.Complete(fight, true);
                }
                Check(runOpponents.Count == 5 && runChallenges.Count == 5, "Repeated Ascension run lost distinct opponent/challenge selection.");
                foreach (string c in runChallenges) challenges.Add(c);
            }
            Check(challenges.Count == 6, "Repeated deterministic runs never exercised all six Ascension challenge families.");
            Check(buffs.Count == 16, "Repeated deterministic runs never exercised all sixteen Ascension buffs.");
            Check(rangedVariants.SetEquals(new[] {
                CoreContentImporter.RangedId("RANGED_SHURIKENS").ToString(),
                CoreContentImporter.RangedId("RANGED_KUNAI").ToString()
            }), "Repeated Ascension runs did not exercise both archived ranged variants.");
            Check(h.Catalog.FightRules.Count(r => r.Id.ToString().Contains("ascension_buff_")) == 16 &&
                h.Catalog.FightRules.Where(r => r.Id.ToString().Contains("ascension_buff_")).All(r => r.PerkAspect == 100000d),
                "DE128 did not register exactly sixteen 100000-aspect Ascension buff rules.");

            // Adapter must reject foreign/missing generated rule identities before projection.
            h.Catalog.TryGetFight(h.Mode.Fights[0], out var blueprint);
            foreach (DefinitionId bad in new[] { DefinitionId.Parse("core:rules/missing"), DefinitionId.Parse("de128:rules/missing") })
            {
                bool rejected = false;
                try { h.Projection.Encounter(blueprint, new ModEncounterPlan(first.Warriors, rounds: 1, roundTime: 150, rules: new[] { bad })); }
                catch (ModContentException) { rejected = true; }
                Check(rejected, "Generated projection accepted foreign/missing rule " + bad);
            }
            CheckBehaviorSelection(h.Mod);
        }
        Console.WriteLine("DE128 Ascension PASS: " + _checks + " checks; actual package/Lua, five-trial progression, persisted plans/RNG, native projection, all challenge families/buffs, save v1/v2 and behavior-rule selection. Host fight/catalog services are controlled stubs; no Unity fight scene executed.");
        return 0;
    }
}
