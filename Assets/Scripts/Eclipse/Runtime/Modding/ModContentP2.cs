using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Eclipse.Modding
{
    public sealed class ModDamageShields
    {
        private readonly Dictionary<object, (int End, double Fraction)> _entries = new Dictionary<object, (int, double)>();
        public bool TrySet(object key, double fraction, int frames, int now, out string error)
        {
            error = "";
            if (key == null || double.IsNaN(fraction) || double.IsInfinity(fraction) || fraction < 0 || fraction > 1 || frames < 1 || frames > 3600)
            { error = "Shield fraction must be 0..1 and duration 1..3600 simulation frames."; return false; }
            Expire(now);
            if (!_entries.ContainsKey(key) && _entries.Count >= 128) { error = "Too many active damage shields."; return false; }
            _entries[key] = (checked(now + frames), fraction); return true;
        }
        public void Remove(object key) { if (key != null) _entries.Remove(key); }
        public void Clear() => _entries.Clear();
        public void Expire(int now)
        {
            var expired = new List<object>();
            foreach (var pair in _entries) if (pair.Value.End <= now) expired.Add(pair.Key);
            foreach (var key in expired) _entries.Remove(key);
        }
        public double Scale(int now)
        {
            Expire(now); double scale = 1;
            foreach (var pair in _entries) scale *= 1 - pair.Value.Fraction;
            return scale;
        }
    }

    public sealed class ModModeDefinition
    {
        public DefinitionId Id { get; }
        public IReadOnlyList<DefinitionId> Fights { get; }
        public bool Repeatable { get; }
        public bool ResetOnLoss { get; }
        public bool Raid { get; }
        public bool HardMode { get; }
        public int MinimumLevel { get; }
        public long StartsAt { get; }
        public long EndsAt { get; }
        public DefinitionId EntryItem { get; }
        public int EntryCount { get; }
        public bool HasEntryItem => EntryCount > 0;
        internal ModModeDefinition(DefinitionId id, DefinitionId[] fights, bool repeatable, bool resetOnLoss,
            bool raid, bool hardMode, int minimumLevel, long startsAt, long endsAt, DefinitionId entryItem, int entryCount)
        {
            if (fights == null || fights.Length < 1 || fights.Length > 100) throw new ModContentException("Mode requires 1..100 fights.");
            if (minimumLevel < 1 || minimumLevel > 1000 || startsAt < 0 || endsAt < 0 || (endsAt > 0 && endsAt <= startsAt))
                throw new ModContentException("Invalid mode level or UTC schedule.");
            if (entryCount < 0 || entryCount > 100000) throw new ModContentException("Entry item count must be 0..100000.");
            if (hardMode && !raid) throw new ModContentException("Hard mode metadata requires a raid.");
            Id = id; Fights = Array.AsReadOnly((DefinitionId[])fights.Clone()); Repeatable = repeatable;
            ResetOnLoss = resetOnLoss; Raid = raid; HardMode = hardMode; MinimumLevel = minimumLevel;
            StartsAt = startsAt; EndsAt = endsAt; EntryItem = entryItem; EntryCount = entryCount;
        }
        public bool IsAvailable(int level, long now) => level >= MinimumLevel && now >= StartsAt && (EndsAt == 0 || now < EndsAt);
    }

    public sealed class ModTimerPolicy
    {
        public ModId Owner { get; }
        public string Subsystem { get; }
        public int Seconds { get; }
        public bool SkipEnabled { get; }
        internal ModTimerPolicy(ModId owner, string subsystem, int seconds, bool skipEnabled)
        {
            if (subsystem != "forge") throw new ModContentException("Unsupported timer subsystem: " + subsystem);
            if (seconds < 0 || seconds > 31536000) throw new ModContentException("Timer seconds must be 0..31536000.");
            Owner = owner; Subsystem = subsystem; Seconds = seconds; SkipEnabled = skipEnabled;
        }
    }

    public sealed partial class ModContentCatalog
    {
        internal readonly Dictionary<DefinitionId, ModModeDefinition> ModeDefinitions = new Dictionary<DefinitionId, ModModeDefinition>();
        internal readonly Dictionary<string, ModTimerPolicy> TimerDefinitions = new Dictionary<string, ModTimerPolicy>(StringComparer.Ordinal);
        internal readonly HashSet<string> DisabledFeatures = new HashSet<string>(StringComparer.Ordinal);
        public IReadOnlyCollection<ModModeDefinition> Modes => ModeDefinitions.Values;
        public IReadOnlyCollection<ModTimerPolicy> TimerPolicies => TimerDefinitions.Values;
        public bool FeatureEnabled(string feature) => !DisabledFeatures.Contains(feature);
        public bool TryGetMode(DefinitionId id, out ModModeDefinition mode) => ModeDefinitions.TryGetValue(id, out mode);
        public bool TryGetTimer(string subsystem, out ModTimerPolicy policy) => TimerDefinitions.TryGetValue(subsystem, out policy);
        public string RuntimeFightId(DefinitionId id)
        {
            if (!TryGetFight(id, out var fight) || !TryGetBattle(fight.Battle, out var battle) || !TryGetZone(battle.Zone, out var zone))
                throw new ModContentException("Missing mode fight graph: " + id);
            return zone.LegacyName + "|" + battle.LegacyName + "|" + fight.LegacyName;
        }
    }

    public sealed partial class ModRegistrationTransaction
    {
        private readonly Dictionary<DefinitionId, ModModeDefinition> _modes = new Dictionary<DefinitionId, ModModeDefinition>();
        private readonly Dictionary<string, ModTimerPolicy> _timers = new Dictionary<string, ModTimerPolicy>();
        private readonly HashSet<string> _disabledFeatures = new HashSet<string>();
        public ModModeDefinition RegisterMode(string id, DefinitionId[] fights, bool repeatable, bool resetOnLoss,
            bool raid, bool hardMode, int level, long starts, long ends, DefinitionId item, int count)
        {
            ThrowIfCompleted(); EnsureCapacityForNewRegistration();
            var mode = new ModModeDefinition(Qualify("modes", id), fights, repeatable, resetOnLoss, raid, hardMode, level, starts, ends, item, count);
            if (_modes.ContainsKey(mode.Id)) throw new ModContentException("Duplicate mode: " + mode.Id);
            _modes.Add(mode.Id, mode); return mode;
        }
        public void SetTimer(string subsystem, int seconds, bool skipEnabled)
        {
            ThrowIfCompleted(); EnsureCapacityForNewRegistration();
            if (_timers.ContainsKey(subsystem)) throw new ModContentException("Duplicate timer policy: " + subsystem);
            _timers.Add(subsystem, new ModTimerPolicy(Mod.Id, subsystem, seconds, skipEnabled));
        }
        public void DisableFeature(string feature)
        {
            ThrowIfCompleted(); EnsureCapacityForNewRegistration();
            switch (feature)
            {
                case "paid_offers": case "battle_pass": case "ads": case "rewarded_video": case "online_services": case "payments": break;
                default: throw new ModContentException("Unsupported service surface: " + feature);
            }
            _disabledFeatures.Add(feature);
        }
        private void ValidateP2Commit()
        {
            foreach (var timer in _timers)
                if (_catalog.TimerDefinitions.ContainsKey(timer.Key)) throw new ModContentException("Timer policy already owned: " + timer.Key);
            foreach (var battle in _battles.Values)
                if (battle.Kind == ModBattleKind.Raid)
                    foreach (var id in _fightOrder)
                        if (_fights[id].Battle == battle.Id)
                        {
                            bool owned = false;
                            foreach (var mode in _modes.Values) foreach (var fight in mode.Fights) if (fight == id && mode.Raid) owned = true;
                            if (!owned) throw new ModContentException("Raid fights require a registered offline raid mode.");
                        }
            var assigned = new HashSet<DefinitionId>();
            foreach (var mode in _catalog.Modes) foreach (var fight in mode.Fights) assigned.Add(fight);
            foreach (var mode in _modes.Values)
            {
                if (_catalog.ModeDefinitions.ContainsKey(mode.Id)) throw new ModContentException("Duplicate mode: " + mode.Id);
                foreach (var id in mode.Fights)
                {
                    if (id.Namespace != Mod.Id || id.Category != "fights" ||
                        (!_fights.TryGetValue(id, out var fight) && !_catalog.TryGetFight(id, out fight)))
                        throw new ModContentException("Mode must own its registered fights: " + id);
                    if (!assigned.Add(id)) throw new ModContentException("Fight belongs to multiple mode steps: " + id);
                    if (!_battles.TryGetValue(fight.Battle, out var battle) && !_catalog.TryGetBattle(fight.Battle, out battle))
                        throw new ModContentException("Missing mode battle: " + fight.Battle);
                    if (mode.Raid != (battle.Kind == ModBattleKind.Raid)) throw new ModContentException("Mode raid metadata must match its battle kind.");
                }
                if (mode.HasEntryItem)
                {
                    if (mode.EntryItem.Namespace != Mod.Id ||
                        (!_p1cItems.ContainsKey(mode.EntryItem) && !_catalog.TryGetItem(mode.EntryItem, out var ignored)))
                        throw new ModContentException("Mode entry item must be a consumable owned by the mode's mod.");
                    ItemDefinition entry;
                    if (_p1cItems.TryGetValue(mode.EntryItem, out var pending)) entry = pending; else _catalog.TryGetItem(mode.EntryItem, out entry);
                    if (!(entry is NonEquipmentItemDefinition item) || item.Kind != ModNonEquipmentItemKind.Consumable) throw new ModContentException("Entry item must be consumable.");
                }
            }
        }
        private void ApplyP2Commit()
        {
            foreach (var value in _modes) _catalog.ModeDefinitions.Add(value.Key, value.Value);
            foreach (var value in _timers) _catalog.TimerDefinitions.Add(value.Key, value.Value);
            foreach (var value in _disabledFeatures) _catalog.DisabledFeatures.Add(value);
        }
        private void ClearP2Pending() { _modes.Clear(); _timers.Clear(); _disabledFeatures.Clear(); }
    }

    public sealed partial class ModApiFacade
    {
        public ModModeDefinition RegisterMode(string id, DefinitionId[] fights, bool repeatable, bool resetOnLoss,
            bool raid, bool hardMode, int level, long starts, long ends, DefinitionId item, int count)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterMode(id, fights, repeatable, resetOnLoss, raid, hardMode, level, starts, ends, item, count);
        }
        public void SetTimer(string subsystem, int seconds, bool skip) { RequireCapability("policy.timers"); RequireRegistration().SetTimer(subsystem, seconds, skip); }
        public void DisableFeature(string feature) { RequireCapability("policy.services"); RequireRegistration().DisableFeature(feature); }
    }

    // Shared policy boundary: recovered assemblies consume semantic decisions without holding script objects.
    public static class ModPolicies
    {
        public static ModContentCatalog Content { get; set; }
        public static int DeliverySeconds(string subsystem, int original) => Content != null && Content.TryGetTimer(subsystem, out var policy) ? policy.Seconds : original;
        public static bool SkipEnabled(string subsystem) => Content == null || !Content.TryGetTimer(subsystem, out var policy) || policy.SkipEnabled;
        public static bool FeatureEnabled(string feature) => Content == null || Content.FeatureEnabled(feature);
        public static bool TryRaidBattle(string name, out bool hardMode)
        {
            hardMode = false;
            if (Content == null) return false;
            foreach (var mode in Content.Modes)
                if (mode.Raid) foreach (var id in mode.Fights)
                    if (Content.TryGetFight(id, out var fight) && Content.TryGetBattle(fight.Battle, out var battle) && battle.LegacyName == name)
                    { hardMode = mode.HardMode; return true; }
            return false;
        }
        public static bool IsRaidZone(string name)
        {
            if (Content == null) return false;
            foreach (var mode in Content.Modes)
                if (mode.Raid) foreach (var id in mode.Fights)
                    if (Content.TryGetFight(id, out var fight) && Content.TryGetBattle(fight.Battle, out var battle) &&
                        Content.TryGetZone(battle.Zone, out var zone) && zone.LegacyName == name) return true;
            return false;
        }
    }

    public sealed class ModModeProgress
    {
        private readonly XmlElement _node;
        public int Step => Read("Step");
        public int Completions => Read("Completions");
        public bool Entered => _node.GetAttribute("Entered") == "1";
        public ModModeProgress(XmlNode warrior, ModModeDefinition definition)
        {
            if (warrior == null) throw new ModContentException("Mode progress requires a loaded save.");
            XmlElement root = warrior["EclipseModes"];
            if (root == null) { root = warrior.OwnerDocument.CreateElement("EclipseModes"); root.SetAttribute("Version", "1"); warrior.AppendChild(root); }
            if (root.GetAttribute("Version") != "1") throw new ModContentException("Unsupported mode save version; data preserved.");
            foreach (XmlNode child in root.ChildNodes)
                if (child is XmlElement element && element.Name == "Mode" && element.GetAttribute("Id") == definition.Id.ToString())
                { if (_node != null) throw new ModContentException("Duplicate mode save entry."); _node = element; }
            if (_node == null)
            {
                _node = warrior.OwnerDocument.CreateElement("Mode"); _node.SetAttribute("Id", definition.Id.ToString()); root.AppendChild(_node);
            }
            var sequence = new List<string>(); foreach (var id in definition.Fights) sequence.Add(id.ToString());
            string signature = string.Join("|", sequence);
            string savedSignature = _node.GetAttribute("Sequence");
            if (savedSignature != "" && savedSignature != signature) throw new ModContentException("Mode sequence changed; saved progression preserved.");
            if (savedSignature == "") _node.SetAttribute("Sequence", signature);
            if (Step > definition.Fights.Count) throw new ModContentException("Saved mode step exceeds its current sequence; data preserved.");
        }
        private int Read(string name)
        {
            string text = _node.GetAttribute(name);
            if (text == "") return 0;
            if (!int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out int value)) throw new ModContentException("Invalid saved mode " + name);
            return value;
        }
        private void Write(string name, int value) => _node.SetAttribute(name, value.ToString(CultureInfo.InvariantCulture));
        public void Enter() { _node.SetAttribute("Entered", "1"); }
        public void CancelEnter() { _node.SetAttribute("Entered", "0"); }
        public void Complete(ModModeDefinition mode, bool won)
        {
            if (!Entered) return;
            _node.SetAttribute("Entered", "0");
            if (!won) { if (mode.ResetOnLoss) Write("Step", 0); return; }
            int next = Step + 1;
            if (next == mode.Fights.Count) { Write("Completions", checked(Completions + 1)); if (mode.Repeatable) next = 0; }
            Write("Step", next);
        }
    }
}
