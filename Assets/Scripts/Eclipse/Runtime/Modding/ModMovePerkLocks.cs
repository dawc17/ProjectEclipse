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
