using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

// Tests execute the actual DE Lua package. Archived XML supplies the expected
// constants/branches. This fake supplies observations and records host operations;
// it is not a substitute implementation of either perk's Lua behavior.
internal static class DECombatPerksTests
{
    private sealed class Fighter : IModFighterOperations, IModIncomingHitSource,
        IModCombatSnapshotSource, IModCombatActivitySource, IModFighterStatusIcons, IModFighterFlags, IModAnimationLifecycleSource
    {
        public ModAnimationLifecycleEvent AnimationEvent { get; set; }
        internal readonly HashSet<string> Flags = new HashSet<string>();
        internal int Sets, Clears;
        public bool TrySetFlag(object owner,string behavior,string name,out string error) { error="";Sets++;Flags.Add(name);return true; }
        public bool TryClearFlag(object owner,string behavior,string name,out string error) {error="";Clears++;Flags.Remove(name);return true;}
        public bool TryHasFlag(object owner,string behavior,string name,out bool exists,out string error) {error="";exists=Flags.Contains(name);return true;}
        internal int Frame = 10;
        internal double Damage;
        internal readonly Dictionary<object, (AssetId Sprite, int Frames, int Stacks)> Icons =
            new Dictionary<object, (AssetId, int, int)>();
        public ModIncomingHit IncomingHit { get; set; }
        public ModCombatActivityEvent ActivityEvent { get; set; }
        public ModCombatSnapshot CaptureCombatSnapshot() => new ModCombatSnapshot(
            new ModFighterSnapshot(1, 1, 1, 0, 0, 0), new ModFighterSnapshot(1, 1, 1, 1, 0, 0), Frame, true);
        public bool TryChangeHealth(double amount, out string error)
        { error = "The perk must modify the pending hit, not directly change health."; return false; }
        public bool TryAddMagicCharge(double amount, out string error)
        { error = "Unexpected magic operation."; return false; }
        public bool TryShowStatusIcon(object key, AssetId sprite, int frames, int stacks, out string error)
        { Icons[key] = (sprite, frames, stacks); error = ""; return true; }
        public bool TryClearStatusIcon(object key, out string error)
        { Icons.Remove(key); error = ""; return true; }
        internal void Hit(bool incoming, bool blocked, string category, double damage = 0.2)
        {
            Damage = damage;
            IncomingHit = new ModIncomingHit(() => Damage, value => Damage = value, blocked, false,
                new ModHitEvent(incoming, category == "weapon", category == "unarmed", category == "ranged", category == "magic"));
        }
    }

    private sealed class Trace
    {
        internal readonly Fighter Fighter = new Fighter();
        internal readonly XmlDocument Saved = new XmlDocument { XmlResolver = null };
        internal readonly Dictionary<string, string> Context = new Dictionary<string, string>
            { { "source", "perk" }, { "side", "player" }, { "fight_id", "xml-perk-fixture" }, { "round", "1" } };
        private readonly IModScriptContext _script;
        private readonly PerkDefinition _perk;
        private readonly IReadOnlyDictionary<string, ModParameterValue> _parameters;
        private readonly Action<bool, string> _check;

        internal Trace(IModScriptContext script, ModContentCatalog content, string id, int rank, Action<bool, string> check)
        {
            _script = script; _check = check;
            check(content.TryGetPerk(DefinitionId.Parse("de128:perks/" + id), out _perk), "Missing real DE perk: " + id);
            check(content.TryGetBehavior(_perk.Behavior, out var behavior), "Missing real DE behavior: " + id);
            Saved.LoadXml("<Perk UpgradeLevel='" + rank + "'/>");
            ModEffectSaveData.Write(Saved.DocumentElement, _perk.Id, behavior.Parameters, _perk.InitialParameters);
            check(ModEffectSaveData.TryRead(Saved.DocumentElement, _perk.Id, behavior.Parameters, out var instance, out var error), error);
            _parameters = _perk.ResolveSavedUpgradeParameters(Saved.DocumentElement, instance.Values);
        }

        internal bool Invoke(ModEffectEvent kind, out string error) =>
            ((IModInteractiveBehaviorScriptContext)_script).TryInvokeBehavior(_perk.Behavior, kind, _parameters,
                Context, new ModInstanceFighter(Fighter, Saved.DocumentElement), out error);
        internal void Call(ModEffectEvent kind)
        {
            string before = Saved.OuterXml;
            _check(Invoke(kind, out var error), kind + ": " + error);
            _check(Saved.OuterXml == before, "Transient combat state rewrote the learned perk save.");
        }
        internal void Style(int rank)
        {
            Fighter.ActivityEvent = ModCombatActivityEvent.StyleChange(rank,
                new[] { "Hard", "Normal", "Brutal", "Aggressive", "Crazy" }[rank], 0, true);
            Call(ModEffectEvent.StyleChanged);
        }
        internal void Combo(int current, int previous)
        { Fighter.ActivityEvent = ModCombatActivityEvent.ComboChange(current, previous); Call(ModEffectEvent.ComboChanged); }
        internal void Hit(ModEffectEvent phase, bool incoming, bool blocked, string category, double damage = 0.2)
        { Fighter.Hit(incoming, blocked, category, damage); Call(phase); }
        internal void Tick(int frame)
        { Fighter.Frame = frame; Call(ModEffectEvent.Tick); }
    }

    private static XmlDocument ReadXml(string path)
    {
        var document = new XmlDocument { XmlResolver = null };
        using (var reader = XmlReader.Create(path, new XmlReaderSettings
            { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null })) document.Load(reader);
        return document;
    }
    private static double Number(XmlNode node, string attribute) =>
        double.Parse(node.Attributes[attribute].Value, CultureInfo.InvariantCulture);
    private static bool Near(double first, double second) => Math.Abs(first - second) < 0.0000001;

    internal static void Run(ModDescriptor mod, string repository,
        Func<ModDescriptor, ModContentCatalog, IModScriptContext> load, Action<bool, string> check)
    {
        var xmlPerks = ReadXml(Path.Combine(repository, "Assets/DExml/perks.xml"));
        var xmlProgress = ReadXml(Path.Combine(repository, "Assets/DExml/CharacterProgress.xml"));
        var xmlEnglish = ReadXml(Path.Combine(repository, "Assets/DExml/localizations/eng.xml"));
        var catalog = new ModContentCatalog();
        using (var script = load(mod, catalog))
        {
            var mind=new Trace(script,catalog,"mind_throw",0,check);
            Action<string,string> animation=(target,name)=> {
                mind.Fighter.AnimationEvent=new ModAnimationLifecycleEvent(ModEffectEvent.AnimationStart,name,target,10);
                mind.Call(ModEffectEvent.AnimationStart);
            };
            check(xmlPerks.SelectNodes("//Perk[@Name='MindThrowNormal']/Trigger").Count==3,"Archived MindThrow trigger count changed");
            animation("opponent","de128:moves/mind_throw_player");
            check(mind.Fighter.Flags.Count==0,"Opponent cast set own flag");
            animation("self","de128:moves/mind_throw_player");animation("self","de128:moves/mind_throw_player");
            check(mind.Fighter.Sets==1 && mind.Fighter.Flags.Contains("de128:behaviors/mind_throw:pending"),"Cast flag duplicated or wrong name");
            animation("self","de128:moves/mind_throw_hit");
            check(mind.Fighter.Flags.Count==1,"Own hit cleared caster flag");
            animation("opponent","de128:moves/mind_throw_hit");animation("opponent","de128:moves/mind_throw_hit");
            check(mind.Fighter.Clears==1 && mind.Fighter.Flags.Count==0,"Victim reaction handoff differs");
            animation("self","de128:moves/mind_throw_player");animation("other","de128:moves/mind_throw_wall");
            check(mind.Fighter.Clears==2 && mind.Fighter.Flags.Count==0,"Projectile wall cleanup differs");
            animation("other","de128:moves/mind_throw_wall");
            check(mind.Fighter.Clears==3,"Archived unconditional wall clear lost");
            check(catalog.Modes.Count == 0 && catalog.Quests.Count == 0 && catalog.Warriors.All(value => value.Id.LocalId.StartsWith("sensei_")),
                "Ascension remains active while loading the replacement package.");
            var ids = new[] { "master_of_style", "relentless" };
            foreach (string id in ids)
            {
                string native = "PERK_" + id.ToUpperInvariant();
                var source = xmlPerks.SelectSingleNode("/Perks/Perk[@Name='" + native + "']");
                var upgrades = xmlProgress.SelectNodes("/Progress/Perks/Perk[@Name='" + native + "']/UpgradeLevel");
                check(catalog.TryGetPerk(DefinitionId.Parse("de128:perks/" + id), out var perk), "Missing XML perk " + native);
                check(source != null && upgrades.Count == 5 && perk.Upgrades.Count == upgrades.Count && perk.InitialUpgradeLevel == 1,
                    "Rank count or first-unlock rank differs from archive.");
                check(catalog.TryGetBehavior(perk.Behavior, out var behavior), "Missing behavior " + id);
                string nativeDuration = id == "master_of_style" ? "ActiveFrames" : "ActiveTime";
                string parameterDuration = id == "master_of_style" ? "active_frames" : "active_time";
                check(perk.InitialParameters[parameterDuration].Integer == Number(source["Set"], nativeDuration),
                    "Buff duration differs from archive.");
                string field = id == "master_of_style" ? "drain" : "damage_per_stack";
                string sourceField = id == "master_of_style" ? "Drain" : "DamagePerStack";
                string english = xmlEnglish.SelectSingleNode("//Word[@Title='PERKDESCRIPTION_" + id.ToUpperInvariant() + "']").InnerText;
                for (int rank = 1; rank <= upgrades.Count; rank++)
                {
                    var sourceRank = upgrades[rank - 1];
                    var expected = Number(sourceRank["Set"], sourceField);
                    var saved = new XmlDocument(); saved.LoadXml("<Perk UpgradeLevel='" + rank + "'/>");
                    var actual = perk.ResolveSavedUpgradeParameters(saved.DocumentElement, perk.InitialParameters);
                    check(Near(actual[field].Number, expected), "Rank " + rank + " payload differs from " + native);
                    check(actual[parameterDuration].Integer == 300, "Upgrade discarded inherited duration.");
                    if (id == "relentless")
                    {
                        check(actual["combo_min_count"].Integer == Number(source["Set"], "ComboMinCount") &&
                            actual["combo_max_count"].Integer == Number(source["Set"], "ComboMaxCount") &&
                            actual["combo_max_count"].Integer == Number(sourceRank["Set"], "MaxStacks"),
                            "Combo thresholds/cap differ from the XML.");
                    }
                    check(catalog.TryGetLocalization(perk.Upgrades[rank - 1].Description, out var description), "Missing rank description.");
                    string expectedText = english.Replace("{0}", (id == "master_of_style" ? rank * 2 : rank).ToString(CultureInfo.InvariantCulture))
                        .Replace("%%", "%");
                    check(description.GetOrEnglish("eng") == expectedText, "Rank description was invented instead of matching XML text.");
                    if (id == "master_of_style") TestMaster(script, catalog, rank, expected, check);
                    else TestRelentless(script, catalog, rank, expected, check);
                }
            }

            var relevantRows = xmlProgress.SelectNodes("/Progress/PerkTree/Level[Perk[@Name='PERK_MASTER_OF_STYLE'] or Upgrade[@Name='PERK_MASTER_OF_STYLE']]");
            check(relevantRows.Count == 5, "Archived progression slots changed.");
            foreach (XmlNode sourceRow in relevantRows)
            {
                int level = int.Parse(sourceRow.Attributes["Value"].Value, CultureInfo.InvariantCulture);
                check(catalog.TryGetProgressionBranch(level, out var branch) && branch.Entries.Count == sourceRow.ChildNodes.Count,
                    "Progression branch differs from XML at " + level);
                for (int i = 0; i < branch.Entries.Count; i++)
                {
                    check(branch.Entries[i].Perk.LocalId == ids[i] &&
                        (branch.Entries[i].Action == ModProgressionPerkAction.Upgrade) == (sourceRow.ChildNodes[i].Name == "Upgrade"),
                        "Wrong perk order or operation at XML level " + level);
                }
            }
            TestIsolation(script, catalog, check);
            CheckMoveLocks(catalog, repository, check);
        }
        TestDamageBounds(check);
    }

    private static void CheckMoveLocks(ModContentCatalog catalog, string repository, Action<bool, string> check)
    {
        var canonical = ReadXml(Path.Combine(repository, "Assets/vanillaXml/animations/moves.xml"));
        var archived = ReadXml(Path.Combine(repository, "Assets/DExml/animations/moves.xml"));
        var replaced = new HashSet<string>(StringComparer.Ordinal)
            { "PERK_DOUBLE_JUMP_KICK", "PERK_ELBOW_STRIKE", "PERK_TWO_FOOT_JUMP_KICK", "PERK_BACK_FLIP_KICK", "PERK_SUPLEX" };
        var expected = new HashSet<string>(StringComparer.Ordinal);
        foreach (XmlElement move in canonical.SelectNodes("/Movesxml/Moves/Move"))
            foreach (XmlElement perk in move.SelectNodes("Locks/Perk"))
                if (replaced.Contains(perk.GetAttribute("Name")))
                {
                    string name = move.GetAttribute("Name");
                    var archivedMove = archived.SelectNodes("/Movesxml/Moves/Move").Cast<XmlElement>()
                        .Single(candidate => candidate.GetAttribute("Name") == name);
                    check(!archivedMove.SelectNodes("Locks/Perk").Cast<XmlElement>().Any(candidate =>
                        candidate.GetAttribute("Name") == perk.GetAttribute("Name")), "DE archive retains a supposedly removed move lock.");
                    expected.Add(name + "|" + CoreContentImporter.PerkId(perk.GetAttribute("Name")));
                }
        check(expected.Count == 6 && expected.SetEquals(catalog.MovePerkLockRemovals.Select(removal => removal.MoveName + "|" + removal.Perk)),
            "Lua move-lock removals do not exactly cover the six XML moves, including both Suplex variants.");
    }

    private static void TestMaster(IModScriptContext script, ModContentCatalog catalog, int rank, double drain, Action<bool, string> check)
    {
        var trace = new Trace(script, catalog, "master_of_style", rank, check);
        trace.Style(1);
        check(trace.Fighter.Icons.Count == 0, "Master activated below the XML Brutal threshold.");
        trace.Style(2);
        check(trace.Fighter.Icons.Single().Value.Frames == 300 &&
            trace.Fighter.Icons.Single().Value.Sprite.Path == "ui/skills/iconmasterofstyle_blue", "Master icon differs from XML.");
        trace.Hit(ModEffectEvent.PostHit, false, true, "weapon");
        check(Near(trace.Fighter.Damage, 0.2) && trace.Fighter.Icons.Count == 1, "Blocked melee consumed Master.");
        trace.Hit(ModEffectEvent.HitPostCrit, true, true, "weapon");
        check(trace.Fighter.Icons.Count == 1, "Blocking an incoming hit consumed Master.");
        trace.Hit(ModEffectEvent.PostHit, false, false, "weapon");
        check(Near(trace.Fighter.Damage, 0.2 + drain) && trace.Fighter.Icons.Count == 0, "Master did not add the exact normalized Drain once.");
        trace.Hit(ModEffectEvent.PostHit, false, false, "unarmed");
        check(Near(trace.Fighter.Damage, 0.2), "Master applied to a second hit.");
        trace.Style(3);
        trace.Hit(ModEffectEvent.PostHit, false, false, "unarmed", 0);
        check(Near(trace.Fighter.Damage, drain), "Master multiplied damage instead of adding flat health-fraction damage.");
        foreach (string category in new[] { "ranged", "magic" })
        {
            trace.Style(2);
            trace.Hit(ModEffectEvent.HitPostCrit, false, true, category);
            check(trace.Fighter.Icons.Count == 0 && Near(trace.Fighter.Damage, 0.2), "Projectile did not cancel Master at postcrit.");
            trace.Hit(ModEffectEvent.PostHit, false, false, "weapon");
            check(Near(trace.Fighter.Damage, 0.2), "Canceled projectile bonus survived until next melee.");
        }
        trace.Style(2);
        trace.Hit(ModEffectEvent.HitPostCrit, true, false, "weapon");
        check(trace.Fighter.Icons.Count == 0 && Near(trace.Fighter.Damage, 0.2), "Incoming unblocked hit did not cancel Master.");
        trace.Style(2);
        trace.Tick(309);
        check(trace.Fighter.Icons.Count == 1, "Master expired before 300 active frames.");
        trace.Tick(310);
        check(trace.Fighter.Icons.Count == 0, "Master lasted beyond 300 active frames.");
        trace.Hit(ModEffectEvent.PostHit, false, false, "weapon");
        check(Near(trace.Fighter.Damage, 0.2), "Expired Master changed damage.");
        trace.Style(3);
        trace.Fighter.Frame = 500;
        trace.Style(4);
        trace.Tick(799);
        check(trace.Fighter.Icons.Count == 1, "Style activation failed to refresh duration.");
        trace.Call(ModEffectEvent.RoundEnd);
        check(trace.Fighter.Icons.Count == 0, "Master icon survived round end.");
        trace.Context["round"] = "2";
        trace.Hit(ModEffectEvent.PostHit, false, false, "weapon");
        check(Near(trace.Fighter.Damage, 0.2), "Master state leaked to next round.");
    }

    private static void TestRelentless(IModScriptContext script, ModContentCatalog catalog, int rank, double perStack, Action<bool, string> check)
    {
        var trace = new Trace(script, catalog, "relentless", rank, check);
        trace.Call(ModEffectEvent.RoundBegin);
        trace.Combo(2, 1); trace.Combo(0, 2);
        check(trace.Fighter.Icons.Count == 0, "Relentless armed for a combo below three.");
        trace.Combo(3, 2); trace.Combo(4, 3);
        trace.Hit(ModEffectEvent.PostHit, false, false, "weapon");
        check(Near(trace.Fighter.Damage, 0.2) && trace.Fighter.Icons.Count == 0, "Relentless armed before combo expiry.");
        trace.Combo(0, 4);
        check(trace.Fighter.Icons.Single().Value.Stacks == 4 && trace.Fighter.Icons.Single().Value.Frames == 300 &&
            trace.Fighter.Icons.Single().Value.Sprite.Path == "ui/skills/iconcrackedapple_blue", "Relentless stack icon differs from XML.");
        trace.Combo(10, 9);
        trace.Hit(ModEffectEvent.PostHit, true, false, "weapon");
        check(Near(trace.Fighter.Damage, 0.2) && trace.Fighter.Icons.Count == 1, "Relentless changed an incoming hit.");
        trace.Hit(ModEffectEvent.PostHit, false, true, "ranged");
        check(Near(trace.Fighter.Damage, 0.2 * (1 + 4 * perStack)) && trace.Fighter.Icons.Count == 0,
            "Relentless did not consume on a blocked outgoing ranged hit or grew stacks while active.");
        trace.Hit(ModEffectEvent.PostHit, false, false, "weapon");
        check(Near(trace.Fighter.Damage, 0.2), "Relentless became an all-hits timed buff.");
        // Literal XML uses its retained counter + 1, not the current/last reported combo size.
        trace.Combo(11, 10); trace.Combo(0, 11);
        check(trace.Fighter.Icons.Single().Value.Stacks == 5, "Relentless ignored the archived retained-counter semantics.");
        trace.Tick(310);
        check(trace.Fighter.Icons.Count == 0, "Relentless did not expire at 300 active frames.");
        trace.Hit(ModEffectEvent.PostHit, false, false, "magic");
        check(Near(trace.Fighter.Damage, 0.2), "Expired Relentless changed damage.");
        trace.Combo(3, 2);
        for (int combo = 4; combo <= 19; combo++) trace.Combo(combo, combo - 1);
        trace.Combo(0, 19);
        check(trace.Fighter.Icons.Single().Value.Stacks == 15, "Relentless exceeded the archived cap of 15.");
        trace.Hit(ModEffectEvent.PostHit, false, false, "magic");
        check(Near(trace.Fighter.Damage, 0.2 * (1 + 15 * perStack)), "Relentless cap/rank damage differs from XML.");
        trace.Combo(3, 2); trace.Combo(0, 3);
        trace.Call(ModEffectEvent.RoundEnd);
        check(trace.Fighter.Icons.Count == 0, "Relentless icon survived round end.");
        trace.Context["round"] = "2"; trace.Call(ModEffectEvent.RoundBegin); trace.Combo(0, 0);
        check(trace.Fighter.Icons.Count == 0, "Relentless stacks survived round reset.");
    }

    private static void TestIsolation(IModScriptContext script, ModContentCatalog catalog, Action<bool, string> check)
    {
        var first = new Trace(script, catalog, "master_of_style", 1, check);
        var second = new Trace(script, catalog, "master_of_style", 5, check);
        first.Style(2);
        second.Hit(ModEffectEvent.PostHit, false, false, "weapon");
        check(Near(second.Fighter.Damage, 0.2), "A fighter borrowed another instance's armed perk.");
        first.Hit(ModEffectEvent.PostHit, false, false, "weapon");
        check(Near(first.Fighter.Damage, 0.22), "Instance isolation corrupted the original perk.");
    }

    private static void TestDamageBounds(Action<bool, string> check)
    {
        double damage = 0.2;
        var hit = new ModIncomingHit(() => damage, amount => damage = amount);
        foreach (double value in new[] { -0.01, 1.01, double.NaN, double.PositiveInfinity })
            check(!hit.TryAddOutgoing(value, out _) && Near(damage, 0.2), "Invalid additive damage changed the pending hit.");
        check(hit.TryAddOutgoing(0.1, out _) && Near(damage, 0.3), "Valid additive damage failed.");
    }

    internal static void CheckMissingCapability(ModDescriptor restricted, string missing,
        Func<ModDescriptor, ModContentCatalog, IModScriptContext> load, Action<bool, string> check)
    {
        var catalog = new ModContentCatalog();
        using (var script = load(restricted, catalog))
        {
            var trace = new Trace(script, catalog, "master_of_style", 1, check);
            trace.Fighter.ActivityEvent = ModCombatActivityEvent.StyleChange(2, "Brutal", 0, true);
            if (missing == "combat.effects")
            {
                check(!trace.Invoke(ModEffectEvent.StyleChanged, out var error) && error.Contains(missing) &&
                    trace.Fighter.Icons.Count == 0, "Status icon capability was bypassed: " + error);
            }
            else
            {
                trace.Call(ModEffectEvent.StyleChanged);
                trace.Fighter.Hit(false, false, "weapon");
                check(!trace.Invoke(ModEffectEvent.PostHit, out var error) && error.Contains(missing) &&
                    Near(trace.Fighter.Damage, 0.2) && trace.Fighter.Icons.Count == 1,
                    "Outgoing capability was bypassed or failed hit consumed the effect: " + error);
            }
        }
    }
}
