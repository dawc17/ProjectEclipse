using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
    public enum ModNonEquipmentItemKind { Consumable = 0, Free = 1, Seal = 2 }
    public enum ModItemVisibility { Inherit = 0, ForceVisible = 1, ForceHidden = 2 }
    public enum ModProgressionPerkAction { Unlock = 0, Upgrade = 1 }

    public sealed class NonEquipmentItemDefinition : ItemDefinition
    {
        public ModNonEquipmentItemKind Kind { get; }
        public string SubType { get; }
        public string PackLabel { get; }
        public bool SilentReceive { get; }
        public bool SpendAfterUse { get; }

        internal NonEquipmentItemDefinition(DefinitionId id, DefinitionId displayName, AssetId icon, AssetId model,
            ModNonEquipmentItemKind kind, string subType, string packLabel, bool silentReceive, bool spendAfterUse,
            string legacyName = null, string legacyItemXml = null)
            : base(id, displayName, icon, model, legacyName, legacyItemXml)
        {
            Kind = kind;
            SubType = subType ?? string.Empty;
            PackLabel = packLabel ?? string.Empty;
            SilentReceive = silentReceive;
            SpendAfterUse = spendAfterUse;
        }
    }

    public sealed class ItemAvailabilityPolicyDefinition
    {
        public ModId Owner { get; }
        public DefinitionId Item { get; }
        public ModItemVisibility Visibility { get; }
        public string RequiredGroup { get; }

        internal ItemAvailabilityPolicyDefinition(ModId owner, DefinitionId item, ModItemVisibility visibility,
            string requiredGroup)
        {
            Owner = owner;
            Item = item;
            Visibility = visibility;
            RequiredGroup = requiredGroup ?? string.Empty;
        }
    }

    public sealed class ModItemSetMember
    {
        public DefinitionId Item { get; }
        public float Scale { get; }
        public float Rotate { get; }
        public float X { get; }
        public float Y { get; }
        public float IconsY { get; }

        public ModItemSetMember(DefinitionId item, float scale = 1f, float rotate = 0f, float x = 0f,
            float y = 0f, float iconsY = 0f)
        {
            Item = item;
            Scale = scale;
            Rotate = rotate;
            X = x;
            Y = y;
            IconsY = iconsY;
        }
    }

    public sealed class ItemSetDefinition
    {
        private readonly ModItemSetMember[] _members;
        public DefinitionId Id { get; }
        public DefinitionId Title { get; }
        public DefinitionId Text { get; }
        public DefinitionId Brief { get; }
        public IReadOnlyList<ModItemSetMember> Members => _members;
        public bool IsCore => Id.Namespace.Value == "core";

        internal ItemSetDefinition(DefinitionId id, DefinitionId title, DefinitionId text, DefinitionId brief,
            ModItemSetMember[] members)
        {
            Id = id;
            Title = title;
            Text = text;
            Brief = brief;
            _members = members ?? Array.Empty<ModItemSetMember>();
        }
    }

    public sealed class ModProgressionPerkEntry
    {
        public DefinitionId Perk { get; }
        public ModProgressionPerkAction Action { get; }

        public ModProgressionPerkEntry(DefinitionId perk, ModProgressionPerkAction action)
        {
            Perk = perk;
            Action = action;
        }
    }

    public sealed class ProgressionBranchOverlayDefinition
    {
        private readonly ModProgressionPerkEntry[] _entries;
        public ModId Owner { get; }
        public int Level { get; }
        public IReadOnlyList<ModProgressionPerkEntry> Entries => _entries;

        internal ProgressionBranchOverlayDefinition(ModId owner, int level, ModProgressionPerkEntry[] entries)
        {
            Owner = owner;
            Level = level;
            _entries = entries ?? Array.Empty<ModProgressionPerkEntry>();
        }
    }

    public sealed class ForgeEconomicProfileDefinition
    {
        public DefinitionId Id { get; }
        public string RuntimeRecipeName { get; }
        internal ForgeEconomicProfileDefinition(DefinitionId id, string runtimeRecipeName)
        {
            Id = id;
            RuntimeRecipeName = runtimeRecipeName ?? string.Empty;
        }
    }

    public sealed class ModForgeRecipeItem
    {
        public ModEquipmentKind Equipment { get; }
        public int Enchantments { get; }
        public string BarScale { get; }
        public int MinDeviation { get; }
        public int MaxDeviation { get; }
        public bool RandomAspect { get; }

        public ModForgeRecipeItem(ModEquipmentKind equipment, int enchantments, string barScale,
            int minDeviation, int maxDeviation, bool randomAspect)
        {
            Equipment = equipment;
            Enchantments = enchantments;
            BarScale = barScale ?? string.Empty;
            MinDeviation = minDeviation;
            MaxDeviation = maxDeviation;
            RandomAspect = randomAspect;
        }
    }

    public sealed class ModForgeRecipeCandidate
    {
        public DefinitionId Perk { get; }
        public ModEquipmentKind Equipment { get; }
        public int MinLevel { get; }
        public int MaxLevel { get; }

        public ModForgeRecipeCandidate(DefinitionId perk, ModEquipmentKind equipment,
            int minLevel = int.MinValue, int maxLevel = int.MaxValue)
        {
            Perk = perk;
            Equipment = equipment;
            MinLevel = minLevel;
            MaxLevel = maxLevel;
        }
    }

    public sealed class ForgeRecipeFamilyDefinition
    {
        private readonly ModForgeRecipeItem[] _items;
        private readonly ModForgeRecipeCandidate[] _candidates;
        public DefinitionId Id { get; }
        public string Alias { get; }
        public DefinitionId EconomicProfile { get; }
        public IReadOnlyList<ModForgeRecipeItem> Items => _items;
        public IReadOnlyList<ModForgeRecipeCandidate> Candidates => _candidates;

        internal ForgeRecipeFamilyDefinition(DefinitionId id, string alias, DefinitionId economicProfile,
            ModForgeRecipeItem[] items, ModForgeRecipeCandidate[] candidates)
        {
            Id = id;
            Alias = alias ?? string.Empty;
            EconomicProfile = economicProfile;
            _items = items ?? Array.Empty<ModForgeRecipeItem>();
            _candidates = candidates ?? Array.Empty<ModForgeRecipeCandidate>();
        }
    }

    public sealed partial class ModContentCatalog
    {
        private readonly Dictionary<DefinitionId, NonEquipmentItemDefinition> _nonEquipmentItems =
            new Dictionary<DefinitionId, NonEquipmentItemDefinition>();
        private readonly List<NonEquipmentItemDefinition> _nonEquipmentItemValues = new List<NonEquipmentItemDefinition>();
        private readonly Dictionary<DefinitionId, ItemSetDefinition> _itemSets = new Dictionary<DefinitionId, ItemSetDefinition>();
        private readonly List<ItemSetDefinition> _itemSetValues = new List<ItemSetDefinition>();
        private readonly Dictionary<DefinitionId, ForgeEconomicProfileDefinition> _forgeProfiles =
            new Dictionary<DefinitionId, ForgeEconomicProfileDefinition>();
        private readonly List<ForgeEconomicProfileDefinition> _forgeProfileValues = new List<ForgeEconomicProfileDefinition>();
        private readonly Dictionary<DefinitionId, ForgeRecipeFamilyDefinition> _forgeRecipes =
            new Dictionary<DefinitionId, ForgeRecipeFamilyDefinition>();
        private readonly List<ForgeRecipeFamilyDefinition> _forgeRecipeValues = new List<ForgeRecipeFamilyDefinition>();
        private readonly Dictionary<DefinitionId, ItemAvailabilityPolicyDefinition> _availabilityPolicies =
            new Dictionary<DefinitionId, ItemAvailabilityPolicyDefinition>();
        private readonly Dictionary<int, ProgressionBranchOverlayDefinition> _progressionBranches =
            new Dictionary<int, ProgressionBranchOverlayDefinition>();

        public IReadOnlyList<NonEquipmentItemDefinition> NonEquipmentItems => _nonEquipmentItemValues.AsReadOnly();
        public IReadOnlyList<ItemSetDefinition> ItemSets => _itemSetValues.AsReadOnly();
        public IReadOnlyList<ForgeEconomicProfileDefinition> ForgeEconomicProfiles => _forgeProfileValues.AsReadOnly();
        public IReadOnlyList<ForgeRecipeFamilyDefinition> ForgeRecipeFamilies => _forgeRecipeValues.AsReadOnly();
        internal IEnumerable<ItemAvailabilityPolicyDefinition> ItemAvailabilityPolicies => _availabilityPolicies.Values;
        internal IEnumerable<ProgressionBranchOverlayDefinition> ProgressionBranches => _progressionBranches.Values;

        private bool TryGetP1CItem(DefinitionId id, out ItemDefinition value)
        {
            NonEquipmentItemDefinition item;
            if (_nonEquipmentItems.TryGetValue(id, out item)) { value = item; return true; }
            value = null;
            return false;
        }

        public bool TryGetItemSet(DefinitionId id, out ItemSetDefinition value) => _itemSets.TryGetValue(id, out value);
        public bool TryGetForgeEconomicProfile(DefinitionId id, out ForgeEconomicProfileDefinition value) =>
            _forgeProfiles.TryGetValue(id, out value);
        public bool TryGetForgeRecipeFamily(DefinitionId id, out ForgeRecipeFamilyDefinition value) =>
            _forgeRecipes.TryGetValue(id, out value);
        public bool TryGetItemAvailability(DefinitionId item, out ItemAvailabilityPolicyDefinition value) =>
            _availabilityPolicies.TryGetValue(item, out value);
        public bool TryGetProgressionBranch(int level, out ProgressionBranchOverlayDefinition value) =>
            _progressionBranches.TryGetValue(level, out value);

        public bool TryResolveRuntimeItem(string runtimeName, string runtimeXml, out DefinitionId id)
        {
            id = default(DefinitionId);
            if (string.IsNullOrEmpty(runtimeName)) return false;
            DefinitionId qualified;
            ItemDefinition external;
            if (DefinitionId.TryParse(runtimeName, out qualified) && TryResolveItem(qualified, out external))
            {
                id = external.Id;
                return true;
            }
            ItemDefinition match = FindCoreRuntimeItem(_weapons.Values, runtimeName, runtimeXml) ??
                FindCoreRuntimeItem(_armors.Values, runtimeName, runtimeXml) ??
                FindCoreRuntimeItem(_helms.Values, runtimeName, runtimeXml) ??
                FindCoreRuntimeItem(_ranged.Values, runtimeName, runtimeXml) ??
                FindCoreRuntimeItem(_magic.Values, runtimeName, runtimeXml) ??
                FindCoreRuntimeItem(_nonEquipmentItemValues, runtimeName, runtimeXml);
            if (match == null) return false;
            id = match.Id;
            return true;
        }

        private static ItemDefinition FindCoreRuntimeItem<T>(IReadOnlyList<T> items, string runtimeName,
            string runtimeXml) where T : ItemDefinition
        {
            ItemDefinition fallback = null;
            for (int i = 0; i < items.Count; i++)
            {
                ItemDefinition item = items[i];
                if (!item.IsCore || !string.Equals(item.LegacyName, runtimeName, StringComparison.Ordinal)) continue;
                if (!string.IsNullOrEmpty(runtimeXml) && !string.IsNullOrEmpty(item.LegacyItemXml) &&
                    string.Equals(item.LegacyItemXml, runtimeXml, StringComparison.Ordinal)) return item;
                if (fallback == null) fallback = item;
            }
            return fallback;
        }

        internal void ImportCoreNonEquipment(NonEquipmentItemDefinition[] items)
        {
            if (IsFrozen) throw new InvalidOperationException("Definition registries are frozen.");
            for (int i = 0; i < items.Length; i++)
            {
                if (_nonEquipmentItems.ContainsKey(items[i].Id) || TryGetItem(items[i].Id, out ItemDefinition ignored))
                    throw new ModContentException("Duplicate core item definition: '" + items[i].Id + "'.");
                _nonEquipmentItems.Add(items[i].Id, items[i]);
                _nonEquipmentItemValues.Add(items[i]);
            }
        }

        internal void ImportCoreForgeProfiles(ForgeEconomicProfileDefinition[] profiles)
        {
            if (IsFrozen) throw new InvalidOperationException("Definition registries are frozen.");
            for (int i = 0; i < profiles.Length; i++)
            {
                if (_forgeProfiles.ContainsKey(profiles[i].Id)) continue;
                _forgeProfiles.Add(profiles[i].Id, profiles[i]);
                _forgeProfileValues.Add(profiles[i]);
            }
        }

        internal void ValidateP1CPatches(IEnumerable<ItemAvailabilityPolicyDefinition> availability,
            IEnumerable<ProgressionBranchOverlayDefinition> progression)
        {
            foreach (ItemAvailabilityPolicyDefinition policy in availability)
            {
                var key = new ModContentPatchKey(policy.Item, "item/availability");
                if (_patchByKey.ContainsKey(key))
                    throw new ModContentException("Item availability already patched: '" + policy.Item + "'.");
            }
            DefinitionId target = DefinitionId.Parse("core:progression/perk-tree");
            foreach (ProgressionBranchOverlayDefinition overlay in progression)
            {
                var key = new ModContentPatchKey(target, "branch/" + overlay.Level);
                if (_patchByKey.ContainsKey(key))
                    throw new ModContentException("Progression branch already patched at level " + overlay.Level + ".");
            }
        }

        internal void CommitP1C(IEnumerable<NonEquipmentItemDefinition> items, IEnumerable<ItemSetDefinition> sets,
            IEnumerable<ForgeRecipeFamilyDefinition> recipes, IEnumerable<ItemAvailabilityPolicyDefinition> availability,
            IEnumerable<ProgressionBranchOverlayDefinition> progression)
        {
            foreach (NonEquipmentItemDefinition item in items)
            {
                _nonEquipmentItems.Add(item.Id, item);
                _nonEquipmentItemValues.Add(item);
            }
            foreach (ItemSetDefinition set in sets)
            {
                _itemSets.Add(set.Id, set);
                _itemSetValues.Add(set);
            }
            foreach (ForgeRecipeFamilyDefinition recipe in recipes)
            {
                _forgeRecipes.Add(recipe.Id, recipe);
                _forgeRecipeValues.Add(recipe);
            }
            foreach (ItemAvailabilityPolicyDefinition policy in availability)
            {
                _availabilityPolicies.Add(policy.Item, policy);
                var record = new ModContentPatchRecord(policy.Owner, policy.Item, "item/availability",
                    ModContentPatchOperation.Replace);
                var key = new ModContentPatchKey(policy.Item, record.Field);
                _patchByKey.Add(key, record);
                _patches.Add(record);
            }
            DefinitionId target = DefinitionId.Parse("core:progression/perk-tree");
            foreach (ProgressionBranchOverlayDefinition overlay in progression)
            {
                _progressionBranches.Add(overlay.Level, overlay);
                var record = new ModContentPatchRecord(overlay.Owner, target, "branch/" + overlay.Level,
                    ModContentPatchOperation.Replace);
                var key = new ModContentPatchKey(target, record.Field);
                _patchByKey.Add(key, record);
                _patches.Add(record);
            }
        }
    }

    public sealed partial class ModRegistrationTransaction
    {
        private readonly Dictionary<DefinitionId, NonEquipmentItemDefinition> _p1cItems =
            new Dictionary<DefinitionId, NonEquipmentItemDefinition>();
        private readonly Dictionary<DefinitionId, ItemSetDefinition> _p1cSets = new Dictionary<DefinitionId, ItemSetDefinition>();
        private readonly Dictionary<DefinitionId, ForgeRecipeFamilyDefinition> _p1cForgeRecipes =
            new Dictionary<DefinitionId, ForgeRecipeFamilyDefinition>();
        private readonly Dictionary<DefinitionId, ItemAvailabilityPolicyDefinition> _p1cAvailability =
            new Dictionary<DefinitionId, ItemAvailabilityPolicyDefinition>();
        private readonly Dictionary<int, ProgressionBranchOverlayDefinition> _p1cProgression =
            new Dictionary<int, ProgressionBranchOverlayDefinition>();

        private int P1CRegistrationCount => _p1cItems.Count + _p1cSets.Count + _p1cForgeRecipes.Count +
            _p1cAvailability.Count + _p1cProgression.Count;

        public NonEquipmentItemDefinition RegisterNonEquipmentItem(string localId, ModNonEquipmentItemKind kind,
            DefinitionId displayName, AssetId icon, AssetId model, string subType, string packLabel,
            bool silentReceive, bool spendAfterUse)
        {
            ThrowIfCompleted();
            if (!Enum.IsDefined(typeof(ModNonEquipmentItemKind), kind))
                throw new ModContentException("Unsupported non-equipment item kind: " + kind + ".");
            string prefix = kind == ModNonEquipmentItemKind.Consumable ? "consumable/" :
                kind == ModNonEquipmentItemKind.Free ? "free/" : "seal/";
            DefinitionId id = Qualify("items", prefix + localId);
            EnsureItemIdAvailable(id);
            ValidateExternalItem(id, displayName, "Non-equipment item");
            if (!icon.Equals(default(AssetId)) && !CanReferenceNamespace(icon.Namespace))
                throw new ModContentException("Item icon belongs to an undeclared namespace.");
            if (!model.Equals(default(AssetId)) && !CanReferenceNamespace(model.Namespace))
                throw new ModContentException("Item model belongs to an undeclared namespace.");
            EnsureCapacityForNewRegistration();
            var definition = new NonEquipmentItemDefinition(id, displayName, icon, model, kind, subType,
                packLabel, silentReceive, spendAfterUse);
            _p1cItems.Add(id, definition);
            return definition;
        }

        public ItemAvailabilityPolicyDefinition SetItemAvailability(DefinitionId item, ModItemVisibility visibility,
            string requiredGroup = null)
        {
            ThrowIfCompleted();
            if (!Enum.IsDefined(typeof(ModItemVisibility), visibility))
                throw new ModContentException("Unsupported item visibility policy: " + visibility + ".");
            if (!CanReferenceNamespace(item.Namespace))
                throw new ModContentException("Availability target belongs to an undeclared namespace: '" + item + "'.");
            ItemDefinition ignored;
            if (!TryGetPendingItem(item, out ignored) && !_catalog.TryResolveItem(item, out ignored))
                throw new ModContentException("Availability target is not registered: '" + item + "'.");
            if (_p1cAvailability.ContainsKey(item)) throw new ModContentException("Availability policy already staged for '" + item + "'.");
            var key = new ModContentPatchKey(item, "item/availability");
            if (!_patchKeys.Add(key)) throw new ModContentException("Duplicate availability patch for '" + item + "'.");
            EnsureCapacityForNewRegistration();
            var policy = new ItemAvailabilityPolicyDefinition(Mod.Id, item, visibility, requiredGroup);
            _p1cAvailability.Add(item, policy);
            return policy;
        }

        public ItemSetDefinition RegisterItemSet(string localId, DefinitionId title, DefinitionId text,
            DefinitionId brief, ModItemSetMember[] members)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("itemsets", localId);
            if (_p1cSets.ContainsKey(id)) throw new ModContentException("Duplicate item set: '" + id + "'.");
            if (title.Namespace != Mod.Id || text.Namespace != Mod.Id || brief.Namespace != Mod.Id ||
                title.Category != "localization" || text.Category != "localization" || brief.Category != "localization")
                throw new ModContentException("External item-set presentation must use localizations owned by the registering mod.");
            if (members == null || members.Length == 0)
                throw new ModContentException("External item set requires at least one member. Empty pseudo-sets have no recovered completion semantics.");
            var seen = new HashSet<DefinitionId>();
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i] == null || !seen.Add(members[i].Item)) throw new ModContentException("Invalid or duplicate item-set member.");
                if (!CanReferenceNamespace(members[i].Item.Namespace)) throw new ModContentException("Item-set member belongs to an undeclared namespace.");
                ItemDefinition ignored;
                if (!TryGetPendingItem(members[i].Item, out ignored) && !_catalog.TryResolveItem(members[i].Item, out ignored))
                    throw new ModContentException("Item-set member is not registered: '" + members[i].Item + "'.");
            }
            EnsureCapacityForNewRegistration();
            var definition = new ItemSetDefinition(id, title, text, brief, (ModItemSetMember[])members.Clone());
            _p1cSets.Add(id, definition);
            return definition;
        }

        public ProgressionBranchOverlayDefinition ReplaceProgressionBranch(int level, ModProgressionPerkEntry[] entries)
        {
            ThrowIfCompleted();
            if (level < 1 || level > MaxEquipmentLevel) throw new ModContentException("Progression branch level must be 1..52.");
            if (entries == null || entries.Length == 0) throw new ModContentException("Progression branch requires at least one perk entry.");
            if (_p1cProgression.ContainsKey(level)) throw new ModContentException("Progression branch already staged for level " + level + ".");
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i] == null || !Enum.IsDefined(typeof(ModProgressionPerkAction), entries[i].Action))
                    throw new ModContentException("Invalid progression perk entry.");
                PerkDefinition perk;
                if (!_perks.TryGetValue(entries[i].Perk, out perk) && !_catalog.TryGetPerk(entries[i].Perk, out perk))
                    throw new ModContentException("Progression branch references missing perk '" + entries[i].Perk + "'.");
                if (!CanReferenceNamespace(entries[i].Perk.Namespace))
                    throw new ModContentException("Progression perk belongs to an undeclared namespace.");
            }
            DefinitionId target = DefinitionId.Parse("core:progression/perk-tree");
            var key = new ModContentPatchKey(target, "branch/" + level);
            if (!_patchKeys.Add(key)) throw new ModContentException("Duplicate progression branch patch for level " + level + ".");
            EnsureCapacityForNewRegistration();
            var overlay = new ProgressionBranchOverlayDefinition(Mod.Id, level, (ModProgressionPerkEntry[])entries.Clone());
            _p1cProgression.Add(level, overlay);
            return overlay;
        }

        public ForgeEconomicProfileDefinition GetForgeEconomicProfile(string reference)
        {
            ThrowIfCompleted();
            DefinitionId id;
            try { id = DefinitionId.Parse(reference); }
            catch (FormatException exception) { throw new ModContentException(exception.Message, exception); }
            if (id.Category != "forge-profiles" || !CanReferenceNamespace(id.Namespace))
                throw new ModContentException("Forge profile belongs to an invalid or undeclared namespace: '" + id + "'.");
            ForgeEconomicProfileDefinition profile;
            if (_catalog.TryGetForgeEconomicProfile(id, out profile)) return profile;
            throw new ModContentException("Forge economic profile is not registered: '" + id + "'.");
        }

        public ForgeRecipeFamilyDefinition RegisterForgeRecipeFamily(string localId, string alias,
            DefinitionId economicProfile, ModForgeRecipeItem[] items, ModForgeRecipeCandidate[] candidates)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("forge-recipes", localId);
            if (_p1cForgeRecipes.ContainsKey(id)) throw new ModContentException("Duplicate forge recipe family: '" + id + "'.");
            ForgeEconomicProfileDefinition profile;
            if (!_catalog.TryGetForgeEconomicProfile(economicProfile, out profile) || !CanReferenceNamespace(economicProfile.Namespace))
                throw new ModContentException("Forge recipe must reference an immutable host/dependency economic profile.");
            if (items == null || items.Length == 0) throw new ModContentException("Forge recipe family requires at least one item binding.");
            var seen = new HashSet<ModEquipmentKind>();
            for (int i = 0; i < items.Length; i++)
            {
                ModForgeRecipeItem item = items[i];
                if (item == null || !Enum.IsDefined(typeof(ModEquipmentKind), item.Equipment) || !seen.Add(item.Equipment))
                    throw new ModContentException("Forge recipe contains an invalid or duplicate equipment binding.");
                if (item.Enchantments < 1 || item.Enchantments > 8) throw new ModContentException("Forge enchantment count must be 1..8.");
                if (item.MinDeviation > item.MaxDeviation) throw new ModContentException("Forge deviation minimum exceeds maximum.");
            }
            if (candidates == null || candidates.Length == 0)
                throw new ModContentException("Forge recipe family requires at least one candidate.");
            for (int i = 0; i < candidates.Length; i++)
            {
                ModForgeRecipeCandidate candidate = candidates[i];
                if (candidate == null || !seen.Contains(candidate.Equipment) || candidate.MinLevel > candidate.MaxLevel)
                    throw new ModContentException("Forge candidate has invalid equipment or level eligibility.");
                PerkDefinition perk;
                if (!_perks.TryGetValue(candidate.Perk, out perk) && !_catalog.TryGetPerk(candidate.Perk, out perk))
                    throw new ModContentException("Forge candidate references missing perk '" + candidate.Perk + "'.");
                if (!CanReferenceNamespace(candidate.Perk.Namespace))
                    throw new ModContentException("Forge candidate perk belongs to an undeclared namespace.");
            }
            EnsureCapacityForNewRegistration();
            var definition = new ForgeRecipeFamilyDefinition(id, alias, profile.Id, (ModForgeRecipeItem[])items.Clone(),
                (ModForgeRecipeCandidate[])candidates.Clone());
            _p1cForgeRecipes.Add(id, definition);
            return definition;
        }

        private bool TryGetPendingP1CItem(DefinitionId id, out ItemDefinition value)
        {
            NonEquipmentItemDefinition item;
            if (_p1cItems.TryGetValue(id, out item)) { value = item; return true; }
            value = null;
            return false;
        }

        private void ValidateP1CCommit()
        {
            foreach (DefinitionId id in _p1cItems.Keys)
                if (_catalog.TryGetItem(id, out ItemDefinition ignored)) throw new ModContentException("Duplicate item definition: '" + id + "'.");
            foreach (DefinitionId id in _p1cSets.Keys)
                if (_catalog.TryGetItemSet(id, out ItemSetDefinition ignored)) throw new ModContentException("Duplicate item set: '" + id + "'.");
            foreach (DefinitionId id in _p1cForgeRecipes.Keys)
                if (_catalog.TryGetForgeRecipeFamily(id, out ForgeRecipeFamilyDefinition ignored)) throw new ModContentException("Duplicate forge recipe family: '" + id + "'.");
            foreach (DefinitionId item in _p1cAvailability.Keys)
                if (_catalog.TryGetItemAvailability(item, out ItemAvailabilityPolicyDefinition ignored))
                    throw new ModContentException("Item availability already patched: '" + item + "'.");
            foreach (int level in _p1cProgression.Keys)
                if (_catalog.TryGetProgressionBranch(level, out ProgressionBranchOverlayDefinition ignored))
                    throw new ModContentException("Progression branch already patched at level " + level + ".");
            _catalog.ValidateP1CPatches(_p1cAvailability.Values, _p1cProgression.Values);
        }

        private void ApplyP1CCommit()
        {
            _catalog.CommitP1C(_p1cItems.Values, _p1cSets.Values, _p1cForgeRecipes.Values,
                _p1cAvailability.Values, _p1cProgression.Values);
        }

        private void ClearP1CPending()
        {
            _p1cItems.Clear();
            _p1cSets.Clear();
            _p1cForgeRecipes.Clear();
            _p1cAvailability.Clear();
            _p1cProgression.Clear();
        }
    }
}
