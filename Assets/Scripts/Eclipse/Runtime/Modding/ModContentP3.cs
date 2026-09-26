using System;
using System.Collections.Generic;
namespace Eclipse.Modding
{
    public sealed class ModActScreenLine
    {
        public string Text { get; }
        public int Frames { get; }
        public ModActScreenLine(string text, int frames)
        {
            if (string.IsNullOrEmpty(text) || text.Length > 4096) throw new ArgumentException("Act-screen text requires 1..4096 characters.");
            if (frames < 1 || frames > 3600) throw new ArgumentOutOfRangeException(nameof(frames));
            Text = text; Frames = frames;
        }
    }

    public static class ModActScreenAccess
    {
        // null refuses the request. The host reports false on cancellation and
        // true only after native presentation and input cleanup have completed.
        public static Func<IReadOnlyList<ModActScreenLine>, Action<bool>, IDisposable> Open;
        public static void Clear() { Open = null; }
    }

    // One native story dialog (the archive's Dialog Type="Regular"). Strings are
    // native localization keys and a native or qualified portrait sprite name.
    public sealed class ModStoryDialogLine
    {
        public string Text { get; }
        public string Button { get; }
        public ModStoryDialogLine(string text, string button = null)
        {
            if (string.IsNullOrEmpty(text) || text.Length > 512) throw new ArgumentException("Story dialog lines require a 1..512 character key.");
            Text = text; Button = button ?? string.Empty;
        }
    }

    public sealed class ModStoryDialogRequest
    {
        public string Title { get; }
        public string Portrait { get; }
        public bool Mirrored { get; }
        public IReadOnlyList<ModStoryDialogLine> Lines { get; }
        public string Button { get; }
        public bool IgnoreBack { get; }
        public ModStoryDialogRequest(string title, string portrait, bool mirrored, IReadOnlyList<ModStoryDialogLine> lines, string button, bool ignoreBack)
        {
            if (lines == null || lines.Count < 1 || lines.Count > 16) throw new ArgumentException("Story dialogs require 1..16 lines.");
            var copy = new List<ModStoryDialogLine>(lines);
            if (copy.Contains(null)) throw new ArgumentException("Null story dialog line.");
            if (string.IsNullOrEmpty(button)) throw new ArgumentException("Story dialogs require a button key.");
            Title = title ?? string.Empty; Portrait = portrait ?? string.Empty; Mirrored = mirrored;
            Lines = copy.AsReadOnly(); Button = button; IgnoreBack = ignoreBack;
        }
    }

    public static class ModStoryDialogAccess
    {
        // null refuses. The host reports true once when the player acknowledges the
        // dialog, or false once when teardown closes it without acknowledgement.
        public static Func<ModStoryDialogRequest, Action<bool>, IDisposable> Open;
        public static void Clear() { Open = null; }
    }

    // Host service retains native scene/quest authority; Lua passes only a menu name.
    public static class ModSceneAccess
    {
        public static Func<string,bool> Open;
        public static void Clear() { Open=null; }
    }

    // Host service for owned Underworld pages. Both refuse (false) when no ready map
    // or profile exists; SetFocus also refuses battles outside Underworld pages.
    public static class ModUnderworldAccess
    {
        public static Func<bool, bool> SetToggleVisible;
        public static Func<DefinitionId, bool> SetFocus;
        public static void Clear() { SetToggleVisible = null; SetFocus = null; }
    }

    public static class ModBattleAccess
    {
        public static Func<DefinitionId,bool,bool> SetLocked;
        public static Func<DefinitionId,bool,bool> Reveal;
        public static Func<DefinitionId,bool> Focus;
        public static void Clear() { SetLocked=null; Reveal=null; Focus=null; }
    }

    public sealed class ModProfileItemSnapshot
    {
        public bool Present { get; }
        public int Count { get; }
        public bool Owned => Present && Count > 0;
        public bool Equipped { get; }
        public int? Upgrade { get; }
        public string Type { get; }
        public string Subtype { get; }
        public ModProfileItemSnapshot(bool present, int count, bool equipped, int? upgrade, string type = null, string subtype = null)
        { Present=present; Count=present?count:0; Equipped=present&&equipped; Upgrade=present?upgrade:null; Type=type; Subtype=subtype; }
    }

    // Host-only read services. Lua receives detached values, never the roster.
    public sealed class ModProfilePerkSnapshot
    {
        public bool Learned { get; }
        public int? Upgrade { get; }
        public ModProfilePerkSnapshot(bool learned, int? upgrade)
        { Learned = learned; Upgrade = learned ? upgrade : null; }
    }

    public sealed class ModProfileEquipmentSnapshot
    {
        public DefinitionId? Item { get; }
        public ModProfileItemSnapshot State { get; }
        // Qualified perk IDs of the record's current enchantments, in native order.
        public IReadOnlyList<DefinitionId> Enchantments { get; }
        public ModProfileEquipmentSnapshot(DefinitionId? item, ModProfileItemSnapshot state, IReadOnlyList<DefinitionId> enchantments = null)
        {
            Item=item; State=state ?? throw new ArgumentNullException(nameof(state));
            Enchantments=Array.AsReadOnly(enchantments == null ? Array.Empty<DefinitionId>() : new List<DefinitionId>(enchantments).ToArray());
        }
    }

    public static class ModProfileAccess
    {
        public static Func<bool,bool> SetEclipseMode;
        public static Func<DefinitionId,ModProfileFightSnapshot> Fight;
        public static Func<int?> Level;
        public static Func<DefinitionId,ModProfileItemSnapshot> Item;
        public static Func<DefinitionId,ModProfilePerkSnapshot> Perk;
        public static Func<IReadOnlyList<ModProfileEquipmentSnapshot>> Equipment;
        public static void Clear() { Level=null; Item=null; Perk=null; Equipment=null; Fight=null; SetEclipseMode=null; }
    }

    public sealed class ModProfileFightSnapshot
    {
        public bool Present { get; }
        public int Wins { get; }
        public int Losses { get; }
        public ModProfileFightSnapshot(bool present, int wins, int losses)
        { Present=present; Wins=present?wins:0; Losses=present?losses:0; }
    }

    public sealed class ModCounterDefinition
    {
        public DefinitionId Id { get; }
        public int Maximum { get; }
        internal ModCounterDefinition(DefinitionId id, int maximum)
        {
            if (maximum < 1 || maximum > 1000000000) throw new ModContentException("Counter maximum must be 1..1000000000.");
            Id = id; Maximum = maximum;
        }
    }
    public sealed class ModAchievementDefinition
    {
        public DefinitionId Id { get; }
        public DefinitionId Counter { get; }
        public DefinitionId Title { get; }
        public DefinitionId Description { get; }
        public AssetId Icon { get; }
        public int Threshold { get; }
        public bool Hidden { get; }
        internal ModAchievementDefinition(DefinitionId id, DefinitionId counter, DefinitionId title, DefinitionId description, AssetId icon, int threshold, bool hidden)
        { Id=id; Counter=counter; Title=title; Description=description; Icon=icon; Threshold=threshold; Hidden=hidden; }
    }
    public sealed class ModAssetReplacement
    {
        public ModId Owner { get; }
        public AssetId Target { get; }
        public AssetId Replacement { get; }
        public AssetKind Kind { get; }
        internal ModAssetReplacement(ModId owner, AssetId target, AssetId replacement, AssetKind kind)
        { Owner=owner; Target=target; Replacement=replacement; Kind=kind; }
    }
    // Native roster access is installed by the host, never exposed to Lua.
    public static class ModProgressionAccess
    {
        public static Func<DefinitionId,int,int> Advance;
        public static Func<DefinitionId,int> Read;
        public static void Clear() { Advance=null; Read=null; }
    }
    public sealed partial class ModContentCatalog
    {
        internal readonly Dictionary<DefinitionId,ModCounterDefinition> CounterDefinitions = new Dictionary<DefinitionId,ModCounterDefinition>();
        internal readonly Dictionary<DefinitionId,ModAchievementDefinition> AchievementDefinitions = new Dictionary<DefinitionId,ModAchievementDefinition>();
        internal readonly Dictionary<AssetId,ModAssetReplacement> ReplacementDefinitions = new Dictionary<AssetId,ModAssetReplacement>();
        public IReadOnlyCollection<ModCounterDefinition> Counters => CounterDefinitions.Values;
        public IReadOnlyCollection<ModAchievementDefinition> Achievements => AchievementDefinitions.Values;
        public IReadOnlyCollection<ModAssetReplacement> AssetReplacements => ReplacementDefinitions.Values;
        // Validated before any registry changes so a conflicting commit leaves the catalog untouched.
        internal void ValidateBattlePositionPatch(ModContentPatchRecord record)
        {
            ModContentPolicies.RequirePatchAllowed(record.Target, record.Field, record.Operation);
            if (!_battles.TryGet(record.Target, out _)) throw new ModContentException("Battle patch target is not registered: '" + record.Target + "'.");
            if (_patchByKey.TryGetValue(new ModContentPatchKey(record.Target, record.Field), out var existing)) throw PatchConflict(existing, record);
        }
        internal void ApplyBattlePositionPatch(ModContentPatchRecord record, int? x, int? y)
        {
            _battles.TryGet(record.Target, out var battle);
            _battles.Replace(record.Target, battle.WithPosition(x ?? battle.X, y ?? battle.Y));
            _patchByKey.Add(new ModContentPatchKey(record.Target, record.Field), record);
            _patches.Add(record);
        }
    }
    public sealed partial class ModRegistrationTransaction
    {
        private readonly Dictionary<DefinitionId,ModCounterDefinition> _counters = new Dictionary<DefinitionId,ModCounterDefinition>();
        private readonly Dictionary<DefinitionId,ModAchievementDefinition> _achievements = new Dictionary<DefinitionId,ModAchievementDefinition>();
        private readonly Dictionary<AssetId,ModAssetReplacement> _replacements = new Dictionary<AssetId,ModAssetReplacement>();
        // Staged sf2.battles.patch map placements, keyed by committed target battle.
        private readonly Dictionary<DefinitionId,BattlePositionPatch> _battlePositions = new Dictionary<DefinitionId,BattlePositionPatch>();
        private sealed class BattlePositionPatch
        {
            public ModContentPatchRecord Record; public int? X; public int? Y;
        }
        public const int BattlePositionLimit = 10000;
        public DefinitionId PatchBattlePosition(string reference, int? x, int? y)
        {
            ThrowIfCompleted();
            if (string.IsNullOrWhiteSpace(reference)) throw new ModContentException("Battle patch target must not be empty.");
            if (!x.HasValue && !y.HasValue) throw new ModContentException("Battle patch needs x or y.");
            foreach (int? value in new[] { x, y })
                if (value.HasValue && (value.Value < -BattlePositionLimit || value.Value > BattlePositionLimit))
                    throw new ModContentException("Battle position must be within -" + BattlePositionLimit + ".." + BattlePositionLimit + ".");
            DefinitionId id;
            try { id = DefinitionId.Parse(reference); }
            catch (FormatException exception) { throw new ModContentException(exception.Message, exception); }
            if (id.Category != "battles") throw new ModContentException("Battle patch target must use the battles category: '" + id + "'.");
            if (id.Namespace == Mod.Id) throw new ModContentException("Set x and y when registering your own battle instead of patching '" + id + "'.");
            if (!CanReferenceNamespace(id.Namespace))
                throw new ModContentException("Mod '" + Mod.Id + "' cannot patch undeclared namespace '" + id.Namespace + "'.");
            if (!_catalog.TryGetBattle(id, out _)) throw new ModContentException("Battle patch target is not registered: '" + id + "'.");
            ModContentPolicies.RequirePatchAllowed(id, ModContentPolicies.BattlePosition, ModContentPatchOperation.Replace);
            if (_battlePositions.ContainsKey(id))
                throw new ModContentException("Duplicate battle patch for '" + id + "' field '" + ModContentPolicies.BattlePosition + "'.");
            EnsureCapacityForNewRegistration();
            _battlePositions.Add(id, new BattlePositionPatch {
                Record = new ModContentPatchRecord(Mod.Id, id, ModContentPolicies.BattlePosition, ModContentPatchOperation.Replace), X = x, Y = y });
            return id;
        }
        public ModCounterDefinition RegisterCounter(string localId, int maximum)
        {
            ThrowIfCompleted(); EnsureCapacityForNewRegistration();
            var value=new ModCounterDefinition(Qualify("counters",localId),maximum);
            if (_counters.ContainsKey(value.Id)) throw new ModContentException("Duplicate counter: "+value.Id);
            _counters.Add(value.Id,value); return value;
        }
        public ModAchievementDefinition RegisterAchievement(string localId, DefinitionId counter, DefinitionId title, DefinitionId description, AssetId icon, int threshold, bool hidden)
        {
            ThrowIfCompleted(); EnsureCapacityForNewRegistration();
            if (counter.Namespace!=Mod.Id || !_counters.TryGetValue(counter,out var definition)) throw new ModContentException("Achievement counter must be registered by this mod.");
            if (threshold<1 || threshold>definition.Maximum) throw new ModContentException("Achievement threshold exceeds counter bounds.");
            if (title.Category!="localization" || description.Category!="localization" || !CanReferenceNamespace(title.Namespace) || !CanReferenceNamespace(description.Namespace)) throw new ModContentException("Invalid achievement localization references.");
            if (!CanReferenceNamespace(icon.Namespace)) throw new ModContentException("Achievement icon needs a declared dependency.");
            var value=new ModAchievementDefinition(Qualify("achievements",localId),counter,title,description,icon,threshold,hidden);
            if (_achievements.ContainsKey(value.Id)) throw new ModContentException("Duplicate achievement: "+value.Id);
            _achievements.Add(value.Id,value); return value;
        }
        public void ReplaceAsset(AssetId target, AssetId replacement, AssetKind kind)
        {
            ThrowIfCompleted(); EnsureCapacityForNewRegistration();
            if (target.Namespace==Mod.Id || !CanReferenceNamespace(target.Namespace) || replacement.Namespace!=Mod.Id) throw new ModContentException("Replacement target must be a declared dependency; replacement must belong to this mod.");
            if (kind != AssetKind.Sprite && kind != AssetKind.Texture && kind != AssetKind.Audio && kind != AssetKind.Model) throw new ModContentException("Unsupported replacement type.");
            if (_replacements.ContainsKey(target)) throw new ModContentException("Duplicate asset replacement: "+target);
            _replacements.Add(target,new ModAssetReplacement(Mod.Id,target,replacement,kind));
        }
        private void ValidateP3Commit()
        {
            foreach(var value in _battlePositions.Values) _catalog.ValidateBattlePositionPatch(value.Record);
            foreach(var value in _counters.Values) if (_catalog.CounterDefinitions.ContainsKey(value.Id)) throw new ModContentException("Counter conflict: "+value.Id);
            foreach(var value in _achievements.Values)
            {
                if (_catalog.AchievementDefinitions.ContainsKey(value.Id)) throw new ModContentException("Achievement conflict: "+value.Id);
                if ((!_localizations.ContainsKey(value.Title) && !_catalog.TryGetLocalization(value.Title,out _)) || (!_localizations.ContainsKey(value.Description) && !_catalog.TryGetLocalization(value.Description,out _))) throw new ModContentException("Missing achievement localization.");
            }
            foreach(var value in _replacements.Values)
                if (_catalog.ReplacementDefinitions.TryGetValue(value.Target, out var existing))
                    throw new ModContentException("Asset replacement conflict: " + value.Target + " is already claimed by " + existing.Owner);
            // A dependency may redirect to an asset that another mod targets; reject cycles.
            var graph=new Dictionary<AssetId,AssetId>();
            foreach(var value in _catalog.AssetReplacements) graph.Add(value.Target,value.Replacement);
            foreach(var value in _replacements.Values) graph.Add(value.Target,value.Replacement);
            foreach(var start in graph.Keys)
            {
                var seen=new HashSet<AssetId>(); var current=start;
                while(graph.TryGetValue(current,out var next)) { if(!seen.Add(current)) throw new ModContentException("Asset replacement cycle: "+start); current=next; }
            }
        }
        private void ApplyP3Commit()
        {
            foreach(var p in _counters) _catalog.CounterDefinitions.Add(p.Key,p.Value);
            foreach(var p in _achievements) _catalog.AchievementDefinitions.Add(p.Key,p.Value);
            foreach(var p in _replacements) _catalog.ReplacementDefinitions.Add(p.Key,p.Value);
            foreach(var p in _battlePositions) _catalog.ApplyBattlePositionPatch(p.Value.Record, p.Value.X, p.Value.Y);
        }
        private void ClearP3Pending() { _counters.Clear(); _achievements.Clear(); _replacements.Clear(); _battlePositions.Clear(); }
    }
    public sealed partial class ModApiFacade
    {
        public DefinitionId PatchBattlePosition(string target, int? x, int? y)
        {
            RequireCapability("content.patch");
            return RequireRegistration().PatchBattlePosition(target, x, y);
        }
        public ModCounterDefinition RegisterCounter(string id,int maximum) { RequireCapability("content.register"); return RequireRegistration().RegisterCounter(id,maximum); }
        public ModAchievementDefinition RegisterAchievement(string id,DefinitionId counter,DefinitionId title,DefinitionId description,AssetId icon,int threshold,bool hidden)
        { RequireCapability("content.register"); return RequireRegistration().RegisterAchievement(id,counter,title,description,icon,threshold,hidden); }
        public int ReadCounter(DefinitionId id)
        {
            RequireCapability("progression.read");
            if(id.Namespace!=Mod.Id || id.Category!="counters") throw new ModContentException("Counter belongs to another mod.");
            if(ModProgressionAccess.Read==null) throw new ModContentException("Counter state is unavailable before profile load.");
            return ModProgressionAccess.Read(id);
        }
        public int AdvanceCounter(DefinitionId id,int amount)
        {
            RequireCapability("progression.write");
            if(id.Namespace!=Mod.Id || id.Category!="counters" || amount<0 || amount>1000000000) throw new ModContentException("Invalid counter increment or ownership.");
            if(ModProgressionAccess.Advance==null) throw new ModContentException("Counter state is unavailable before profile load.");
            return ModProgressionAccess.Advance(id,amount);
        }
        public void ReplaceAsset(string target,string replacement)
        {
            RequireCapability("assets.replace");
            var from=QualifyAsset(target); var to=QualifyAsset(replacement);
            if(!Assets.TryDescribe(from,out var a) || !Assets.TryDescribe(to,out var b)) throw new ModContentException("Replacement asset is missing.");
            if(a.Kind!=b.Kind || (a.Kind!=AssetKind.Sprite && a.Kind!=AssetKind.Texture && a.Kind!=AssetKind.Audio && a.Kind!=AssetKind.Model)) throw new ModContentException("Replacement asset types do not match a supported runtime loader.");
            RequireRegistration().ReplaceAsset(from,to,a.Kind);
        }
    }
}
