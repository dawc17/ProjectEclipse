using System;
using System.Collections.Generic;
namespace Eclipse.Modding
{
    // Host service retains native scene/quest authority; Lua passes only a menu name.
    public static class ModSceneAccess
    {
        public static Func<string,bool> Open;
        public static void Clear() { Open=null; }
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
        public ModProfileEquipmentSnapshot(DefinitionId? item, ModProfileItemSnapshot state)
        { Item=item; State=state ?? throw new ArgumentNullException(nameof(state)); }
    }

    public static class ModProfileAccess
    {
        public static Func<int?> Level;
        public static Func<DefinitionId,ModProfileItemSnapshot> Item;
        public static Func<DefinitionId,ModProfilePerkSnapshot> Perk;
        public static Func<IReadOnlyList<ModProfileEquipmentSnapshot>> Equipment;
        public static void Clear() { Level=null; Item=null; Perk=null; Equipment=null; }
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
    }
    public sealed partial class ModRegistrationTransaction
    {
        private readonly Dictionary<DefinitionId,ModCounterDefinition> _counters = new Dictionary<DefinitionId,ModCounterDefinition>();
        private readonly Dictionary<DefinitionId,ModAchievementDefinition> _achievements = new Dictionary<DefinitionId,ModAchievementDefinition>();
        private readonly Dictionary<AssetId,ModAssetReplacement> _replacements = new Dictionary<AssetId,ModAssetReplacement>();
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
        }
        private void ClearP3Pending() { _counters.Clear(); _achievements.Clear(); _replacements.Clear(); }
    }
    public sealed partial class ModApiFacade
    {
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
