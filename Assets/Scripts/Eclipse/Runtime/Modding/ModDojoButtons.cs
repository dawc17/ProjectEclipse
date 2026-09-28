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

    // One choice of a dojo picker: a location (a core location or a dojo registered by the
    // same mod), its display name and a preview sprite.
    public sealed class ModDojoPickerChoice
    {
        public DefinitionId Location { get; }
        public DefinitionId Name { get; }
        public AssetId Preview { get; }

        public ModDojoPickerChoice(DefinitionId location, DefinitionId name, AssetId preview)
        {
            Location = location; Name = name; Preview = preview;
        }
    }

    // A dojo picker declared by a mod: the engine hosts its dojo-menu button and draws the
    // picker itself. Selecting a choice saves the dojo selection and reloads the dojo.
    public sealed class ModDojoPicker
    {
        public ModId Owner { get; }
        public ModDojoButton Button { get; }
        public DefinitionId? Title { get; }
        public IReadOnlyList<ModDojoPickerChoice> Choices { get; }

        internal ModDojoPicker(ModId owner, ModDojoButton button, DefinitionId? title, IReadOnlyList<ModDojoPickerChoice> choices)
        {
            Owner = owner; Button = button; Title = title; Choices = choices;
        }
    }

    public sealed partial class ModContentCatalog
    {
        private readonly List<ModDojoButton> _dojoButtons = new List<ModDojoButton>();
        private readonly List<ModDojoPicker> _dojoPickers = new List<ModDojoPicker>();
        public IReadOnlyList<ModDojoButton> DojoButtons => _dojoButtons.AsReadOnly();
        public IReadOnlyList<ModDojoPicker> DojoPickers => _dojoPickers.AsReadOnly();

        internal void CommitDojoButtons(IEnumerable<ModDojoButton> buttons) => _dojoButtons.AddRange(buttons);
        internal void CommitDojoPickers(IEnumerable<ModDojoPicker> pickers) => _dojoPickers.AddRange(pickers);
    }

    public sealed partial class ModRegistrationTransaction
    {
        public const int MaxDojoButtonsPerMod = 4;
        public const int MaxDojoPickerChoices = 64;
        private readonly List<ModDojoButton> _dojoButtons = new List<ModDojoButton>();
        private readonly List<ModDojoPicker> _dojoPickers = new List<ModDojoPicker>();
        private int DojoButtonRegistrationCount => _dojoButtons.Count + _dojoPickers.Count;

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

        // One picker per mod. Its button is an ordinary dojo button named "<mod>.<id>".
        public ModDojoPicker RegisterDojoPicker(string localId, AssetId button, DefinitionId? title, IList<ModDojoPickerChoice> choices)
        {
            ThrowIfCompleted();
            if (_dojoPickers.Count != 0) throw new ModContentException("A mod may register only one dojo picker.");
            if (choices == null || choices.Count == 0 || choices.Count > MaxDojoPickerChoices)
                throw new ModContentException("A dojo picker needs 1.." + MaxDojoPickerChoices + " choices.");
            var seen = new HashSet<DefinitionId>();
            foreach (ModDojoPickerChoice choice in choices)
            {
                if (choice == null) throw new ModContentException("Dojo picker choices must not be empty.");
                if (choice.Location.Category != "locations" ||
                    (choice.Location.Namespace.Value != "core" && choice.Location.Namespace != Mod.Id))
                    throw new ModContentException("Dojo picker locations must be core locations or this mod's dojos: " + choice.Location);
                if (!seen.Add(choice.Location)) throw new ModContentException("Duplicate dojo picker choice: " + choice.Location);
            }
            ModDojoButton entry = RegisterDojoButton(localId, button);
            var picker = new ModDojoPicker(Mod.Id, entry, title, new List<ModDojoPickerChoice>(choices).AsReadOnly());
            _dojoPickers.Add(picker);
            return picker;
        }

        private void ApplyDojoButtonCommit()
        {
            _catalog.CommitDojoButtons(_dojoButtons);
            _catalog.CommitDojoPickers(_dojoPickers);
        }

        private void ClearDojoButtonPending() { _dojoButtons.Clear(); _dojoPickers.Clear(); }
    }
}
