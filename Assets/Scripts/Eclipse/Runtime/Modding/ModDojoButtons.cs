using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
    // A mod-owned button in the dojo side menu, below the disciple toggle. It is
    // presentation only: nothing is saved, and clicks reach Lua as a
    // `dojo_button` story event carrying the qualified name.
    public sealed class ModDojoButton
    {
        public string Name { get; }
        public AssetId Image { get; }

        internal ModDojoButton(string name, AssetId image) { Name = name; Image = image; }
    }

    public sealed partial class ModContentCatalog
    {
        private readonly List<ModDojoButton> _dojoButtons = new List<ModDojoButton>();
        public IReadOnlyList<ModDojoButton> DojoButtons => _dojoButtons.AsReadOnly();

        internal void CommitDojoButtons(IEnumerable<ModDojoButton> buttons) => _dojoButtons.AddRange(buttons);
    }

    public sealed partial class ModRegistrationTransaction
    {
        public const int MaxDojoButtonsPerMod = 4;
        private readonly List<ModDojoButton> _dojoButtons = new List<ModDojoButton>();
        private int DojoButtonRegistrationCount => _dojoButtons.Count;

        public ModDojoButton RegisterDojoButton(string localId, AssetId image)
        {
            ThrowIfCompleted();
            if (string.IsNullOrEmpty(localId) || localId.Length > 64)
                throw new ModContentException("Dojo button id must be 1..64 lowercase ASCII letters, digits, '_' or '-'.");
            foreach (char character in localId)
                if (!((character >= 'a' && character <= 'z') || (character >= '0' && character <= '9') ||
                      character == '_' || character == '-'))
                    throw new ModContentException("Dojo button id must be lowercase ASCII letters, digits, '_' or '-'.");
            string name = Mod.Id.Value + "." + localId;
            foreach (ModDojoButton existing in _dojoButtons)
                if (existing.Name == name) throw new ModContentException("Duplicate dojo button: '" + name + "'.");
            if (_dojoButtons.Count >= MaxDojoButtonsPerMod)
                throw new ModContentException("A mod may register at most " + MaxDojoButtonsPerMod + " dojo buttons.");
            EnsureCapacityForNewRegistration();
            var button = new ModDojoButton(name, image);
            _dojoButtons.Add(button);
            return button;
        }

        private void ApplyDojoButtonCommit() => _catalog.CommitDojoButtons(_dojoButtons);
        private void ClearDojoButtonPending() => _dojoButtons.Clear();
    }
}
