using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
    public sealed class MovePerkLockRemoval
    {
        public string MoveName { get; }
        public DefinitionId Perk { get; }
        public string RuntimePerkName { get; }

        public MovePerkLockRemoval(string moveName, DefinitionId perk, string runtimePerkName)
        {
            if (string.IsNullOrWhiteSpace(moveName) || moveName != moveName.Trim() || moveName.Length > 128)
                throw new ModContentException("Move perk-lock removal requires an exact move name of 1..128 characters.");
            if (perk.Category != "perks")
                throw new ModContentException("Move perk-lock removal requires a perk definition.");
            if (string.IsNullOrWhiteSpace(runtimePerkName) || runtimePerkName != runtimePerkName.Trim())
                throw new ModContentException("Move perk-lock removal requires an exact runtime perk name.");
            MoveName = moveName;
            Perk = perk;
            RuntimePerkName = runtimePerkName;
        }

        internal string ConflictKey => MoveName + "\n" + Perk;
    }

    public sealed partial class ModContentCatalog
    {
        private readonly List<MovePerkLockRemoval> _movePerkLockRemovals = new List<MovePerkLockRemoval>();
        private readonly HashSet<string> _movePerkLockRemovalKeys = new HashSet<string>(StringComparer.Ordinal);

        public IReadOnlyList<MovePerkLockRemoval> MovePerkLockRemovals => _movePerkLockRemovals.AsReadOnly();

        internal void ValidateMovePerkLockCanAdd(MovePerkLockRemoval[] removals)
        {
            var pending = new HashSet<string>(StringComparer.Ordinal);
            foreach (MovePerkLockRemoval removal in removals)
                if (_movePerkLockRemovalKeys.Contains(removal.ConflictKey) || !pending.Add(removal.ConflictKey))
                    throw new ModContentException("Duplicate move perk-lock removal for '" + removal.MoveName +
                        "' and '" + removal.Perk + "'.");
        }

        internal void AddMovePerkLockRemovals(MovePerkLockRemoval[] removals)
        {
            foreach (MovePerkLockRemoval removal in removals)
            {
                _movePerkLockRemovals.Add(removal);
                _movePerkLockRemovalKeys.Add(removal.ConflictKey);
            }
        }
    }

    public sealed partial class ModRegistrationTransaction
    {
        private readonly List<MovePerkLockRemoval> _movePerkLockRemovals = new List<MovePerkLockRemoval>();
        private readonly HashSet<string> _movePerkLockRemovalKeys = new HashSet<string>(StringComparer.Ordinal);

        private int MovePerkLockRegistrationCount => _movePerkLockRemovals.Count;

        public void RemoveMovePerkLock(string moveName, DefinitionId perk)
        {
            ThrowIfCompleted();
            if (perk.Category != "perks" || !CanReferenceNamespace(perk.Namespace))
                throw new ModContentException("Move perk-lock removal references an inaccessible perk '" + perk + "'.");
            PerkDefinition resolved = GetPerk(perk.ToString());
            if (resolved.Id != perk)
                throw new ModContentException("Move perk-lock removal could not resolve perk '" + perk + "'.");
            string runtimePerkName = resolved.IsCore && !string.IsNullOrEmpty(resolved.LegacyName)
                ? resolved.LegacyName : resolved.Id.ToString();
            var removal = new MovePerkLockRemoval(moveName, perk, runtimePerkName);
            EnsureCapacityForNewRegistration();
            if (!_movePerkLockRemovalKeys.Add(removal.ConflictKey))
                throw new ModContentException("Duplicate move perk-lock removal for '" + moveName + "' and '" + perk + "'.");
            _movePerkLockRemovals.Add(removal);
        }

        private void ValidateMovePerkLockCommit()
            => _catalog.ValidateMovePerkLockCanAdd(_movePerkLockRemovals.ToArray());

        private void ApplyMovePerkLockCommit()
            => _catalog.AddMovePerkLockRemovals(_movePerkLockRemovals.ToArray());

        private void ClearMovePerkLockPending()
        {
            _movePerkLockRemovals.Clear();
            _movePerkLockRemovalKeys.Clear();
        }
    }

    public sealed partial class ModApiFacade
    {
        public void RemoveMovePerkLock(string moveName, DefinitionId perk)
        {
            RequireCapability("content.patch");
            RequireRegistration().RemoveMovePerkLock(moveName, perk);
        }
    }
}

namespace Eclipse.Modding
{
    public sealed class MoveItemLockExtension
    {
        public string MoveName { get; }
        public string ItemType { get; }
        public string SourceSubtype { get; }
        public string Subtype { get; }
        public MoveItemLockExtension(string moveName, string itemType, string sourceSubtype, string subtype)
        {
            foreach (var value in new[]{moveName,itemType,sourceSubtype,subtype})
                if (string.IsNullOrWhiteSpace(value) || value != value.Trim() || value.Length > 128 || value.IndexOf('\n') >= 0 || value.IndexOf('\r') >= 0)
                    throw new ModContentException("Item lock extension requires exact names of 1..128 characters.");
            if (Array.IndexOf(new[]{"Weapon","Ranged","Magic","Armor","Helm","Skeleton"}, itemType) < 0)
                throw new ModContentException("Unsupported item lock type.");
            if (sourceSubtype == subtype) throw new ModContentException("Item lock extension must add a different subtype.");
            MoveName=moveName; ItemType=itemType; SourceSubtype=sourceSubtype; Subtype=subtype;
        }
        internal string ConflictKey => MoveName + "\n" + ItemType + "\n" + Subtype;
    }

    public sealed partial class ModContentCatalog
    {
        private readonly List<MoveItemLockExtension> _moveItemLockExtensions = new List<MoveItemLockExtension>();
        public IReadOnlyList<MoveItemLockExtension> MoveItemLockExtensions => _moveItemLockExtensions.AsReadOnly();
        internal void ValidateItemLockExtensions(IReadOnlyList<MoveItemLockExtension> extensions)
        {
            var keys = new HashSet<string>(StringComparer.Ordinal);
            foreach (var entry in _moveItemLockExtensions) keys.Add(entry.ConflictKey);
            foreach (var entry in extensions)
                if (!keys.Add(entry.ConflictKey)) throw new ModContentException("Duplicate item lock extension for '" + entry.MoveName + "'.");
        }
        internal void AddItemLockExtensions(IEnumerable<MoveItemLockExtension> extensions) => _moveItemLockExtensions.AddRange(extensions);
    }

    public sealed partial class ModRegistrationTransaction
    {
        private readonly List<MoveItemLockExtension> _moveItemLockExtensions = new List<MoveItemLockExtension>();
        public void ExtendMoveItemLock(string moveName, string itemType, string sourceSubtype, string subtype)
        {
            ThrowIfCompleted();
            var value = new MoveItemLockExtension(moveName,itemType,sourceSubtype,subtype);
            foreach (var prior in _moveItemLockExtensions)
                if (prior.ConflictKey == value.ConflictKey) throw new ModContentException("Duplicate item lock extension for '" + moveName + "'.");
            EnsureCapacityForNewRegistration();
            _moveItemLockExtensions.Add(value);
        }
    }

    public sealed partial class ModApiFacade
    {
        public void ExtendMoveItemLock(string moveName, string itemType, string sourceSubtype, string subtype)
        {
            RequireCapability("content.patch");
            RequireRegistration().ExtendMoveItemLock(moveName,itemType,sourceSubtype,subtype);
        }
    }
}

namespace Eclipse.Modding
{
    public sealed class ModMoveFramePatch
    {
        public string Name { get; }
        public int Expected { get; }
        public int Value { get; }
        public ModMoveFramePatch(string name, int expected, int value)
        {
            MoveCombatPatch.ValidateName(name);
            if (expected < 0 || expected > 100000 || value < 0 || value > 100000 || expected == value)
                throw new ModContentException("Move frame patches require distinct expected/value integers in 0..100000.");
            Name = name; Expected = expected; Value = value;
        }
    }

    public sealed class ModMoveHitPatch
    {
        public string Expected { get; }
        public string Value { get; }
        public ModMoveHitPatch(string expected, string value)
        {
            var names = new[] { "High", "Middle", "Low", "Spinning", "HighHeavy", "MiddleShortPlus", "Physycal", "HighLong", "NoReaction" };
            if (Array.IndexOf(names, expected) < 0 || Array.IndexOf(names, value) < 0 || expected == value)
                throw new ModContentException("Move hit patch requires distinct supported expected/value reactions.");
            Expected = expected; Value = value;
        }
    }

    public sealed class ModMoveInputPatch
    {
        public ModMoveKey Expected { get; }
        public ModMoveKey Value { get; }
        public ModMoveInputPatch(string expected, string value)
        {
            Expected = new ModMoveKey(expected);
            Value = new ModMoveKey(value);
            if (expected == value) throw new ModContentException("Move input patch requires distinct Tap keys.");
        }
    }

    public sealed class ModMovePriorityPatch
    {
        public int Expected { get; }
        public int Value { get; }
        public ModMovePriorityPatch(int expected, int value)
        {
            if (expected < 0 || expected > 100000 || value < 0 || value > 100000 || expected == value)
                throw new ModContentException("Move priority patch requires distinct values in 0..100000.");
            Expected = expected; Value = value;
        }
    }

    public sealed class MoveCombatPatch
    {
        public ModId Owner { get; }
        public string MoveName { get; }
        public IReadOnlyList<ModMoveCondition> Conditions { get; }
        public ModMoveFramePatch IntervalEnd { get; }
        public ModMoveFramePatch IntervalStart { get; }
        public ModMoveHitPatch Hit { get; }
        public ModMoveFramePatch SoundFrame { get; }
        public ModMoveInputPatch Input { get; }
        public ModMovePriorityPatch Priority { get; }
        public bool Disable { get; }
        public MoveCombatPatch(ModId owner, string moveName, ModMoveCondition[] conditions = null,
            ModMoveFramePatch intervalEnd = null, ModMoveHitPatch hit = null, ModMoveFramePatch soundFrame = null,
            bool disable = false, ModMoveInputPatch input = null, ModMovePriorityPatch priority = null,
            ModMoveFramePatch intervalStart = null)
        {
            ValidateName(moveName);
            conditions = conditions ?? new ModMoveCondition[0];
            if (conditions.Length > 32) throw new ModContentException("Move patch accepts at most 32 added conditions.");
            foreach (var condition in conditions)
                if (condition == null) throw new ModContentException("Move patch conditions cannot contain null.");
            if (conditions.Length == 0 && intervalEnd == null && intervalStart == null && hit == null && soundFrame == null &&
                input == null && priority == null && !disable)
                throw new ModContentException("Move patch must change at least one supported field.");
            if (disable && (conditions.Length != 0 || intervalEnd != null || intervalStart != null || hit != null || soundFrame != null ||
                input != null || priority != null))
                throw new ModContentException("A disabled move cannot also receive combat field patches.");
            if (intervalEnd != null && Array.IndexOf(new[] { "Uninterrupt", "SelfUninterrupt", "Unstable" }, intervalEnd.Name) < 0)
                throw new ModContentException("interval_end requires Uninterrupt, SelfUninterrupt or Unstable.");
            if (intervalStart != null && Array.IndexOf(new[] { "Uninterrupt", "SelfUninterrupt", "Unstable" }, intervalStart.Name) < 0)
                throw new ModContentException("interval_start requires Uninterrupt, SelfUninterrupt or Unstable.");
            Owner = owner; MoveName = moveName; Conditions = Array.AsReadOnly((ModMoveCondition[])conditions.Clone());
            IntervalEnd = intervalEnd; IntervalStart = intervalStart; Hit = hit; SoundFrame = soundFrame;
            Input = input; Priority = priority; Disable = disable;
        }
        internal static void ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length > 128)
                throw new ModContentException("Move patch names require 1..128 ASCII letters, digits, underscores, dots or hyphens.");
            foreach (char c in name)
                if (!(c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9' || c == '_' || c == '.' || c == '-'))
                    throw new ModContentException("Move patch requires an exact native name, not a content ID or pattern.");
        }
    }

    public sealed partial class ModContentCatalog
    {
        private readonly List<MoveCombatPatch> _moveCombatPatches = new List<MoveCombatPatch>();
        public IReadOnlyList<MoveCombatPatch> MoveCombatPatches => _moveCombatPatches.AsReadOnly();
        internal void ValidateCombatPatches(IReadOnlyList<MoveCombatPatch> patches)
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var patch in _moveCombatPatches) names.Add(patch.MoveName);
            foreach (var patch in patches)
                if (!names.Add(patch.MoveName)) throw new ModContentException("Move combat patch already owned: " + patch.MoveName);
        }
        internal void AddCombatPatches(IEnumerable<MoveCombatPatch> patches) => _moveCombatPatches.AddRange(patches);
    }

    public sealed partial class ModRegistrationTransaction
    {
        private readonly List<MoveCombatPatch> _moveCombatPatches = new List<MoveCombatPatch>();
        public void PatchMove(string moveName, ModMoveCondition[] conditions = null,
            ModMoveFramePatch intervalEnd = null, ModMoveHitPatch hit = null, ModMoveFramePatch soundFrame = null,
            bool disable = false, ModMoveInputPatch input = null, ModMovePriorityPatch priority = null,
            ModMoveFramePatch intervalStart = null)
        {
            ThrowIfCompleted();
            var patch = new MoveCombatPatch(Mod.Id, moveName, conditions, intervalEnd, hit, soundFrame,
                disable, input, priority, intervalStart);
            foreach (var prior in _moveCombatPatches)
                if (prior.MoveName == patch.MoveName) throw new ModContentException("Duplicate move combat patch: " + moveName);
            EnsureCapacityForNewRegistration();
            _moveCombatPatches.Add(patch);
        }
    }

    public sealed partial class ModApiFacade
    {
        public void PatchMove(string moveName, ModMoveCondition[] conditions = null,
            ModMoveFramePatch intervalEnd = null, ModMoveHitPatch hit = null, ModMoveFramePatch soundFrame = null,
            bool disable = false, ModMoveInputPatch input = null, ModMovePriorityPatch priority = null,
            ModMoveFramePatch intervalStart = null)
        {
            RequireCapability("content.patch");
            RequireRegistration().PatchMove(moveName, conditions, intervalEnd, hit, soundFrame,
                disable, input, priority, intervalStart);
        }
    }
}
