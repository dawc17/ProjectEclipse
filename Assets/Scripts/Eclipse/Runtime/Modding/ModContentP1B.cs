using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
    public enum ModQuestEventKind
    {
        FightEnter, FightEnd, LevelUp, GotItem, Dialog, Session, Activate, Purchase, Delivery, TimerEnd,
        MapButtonPress, Enchantment, ActivatePerk, DeactivatePerk, SetItemAcquired, SceneLoaded, ShopEnter
    }

    public enum ModQuestCompareOperator { Equal, Greater, GreaterEqual, Less, LessEqual }
    public enum ModQuestConditionKind { Compare, All, Any }
    public enum ModQuestOperandKind { Literal, UserVariable, EventFight, EventFightResult, CurrentFightBattle, FightWinCount, FightId }
    public enum ModQuestActionKind
    {
        Dialog, StoryScreen, SetUserVariable, ShowBattle, ToggleBattle, SetMapFocus, StartFight,
        StartCurrentFight, ToggleEclipseMode, UpdateEclipseBattles, GiveItem
    }
    public enum ModQuestActionPlace { Map, Fight, Dojo }

    public sealed class ModQuestOperand
    {
        public ModQuestOperandKind Kind { get; }
        public string Value { get; }
        public DefinitionId Reference { get; }
        public bool HasReference { get; }

        public ModQuestOperand(ModQuestOperandKind kind, string value = null)
        {
            Kind = kind; Value = value ?? string.Empty; Reference = default(DefinitionId); HasReference = false;
        }

        public ModQuestOperand(ModQuestOperandKind kind, DefinitionId reference)
        {
            Kind = kind; Value = string.Empty; Reference = reference; HasReference = true;
        }
    }

    public sealed class ModQuestCondition
    {
        private readonly ModQuestCondition[] _children;
        public ModQuestConditionKind Kind { get; }
        public ModQuestCompareOperator Operator { get; }
        public ModQuestOperand Left { get; }
        public ModQuestOperand Right { get; }
        public bool Not { get; }
        public IReadOnlyList<ModQuestCondition> Children => _children;

        public ModQuestCondition(ModQuestCompareOperator op, ModQuestOperand left, ModQuestOperand right, bool not = false)
        {
            Kind = ModQuestConditionKind.Compare; Operator = op;
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
            Not = not; _children = Array.Empty<ModQuestCondition>();
        }

        public ModQuestCondition(ModQuestConditionKind kind, ModQuestCondition[] children, bool not = false)
        {
            if (kind == ModQuestConditionKind.Compare) throw new ArgumentException("Use comparison constructor.", nameof(kind));
            Kind = kind; Operator = ModQuestCompareOperator.Equal; Left = null; Right = null; Not = not;
            _children = children == null ? Array.Empty<ModQuestCondition>() : (ModQuestCondition[])children.Clone();
        }
    }

    public sealed class ModQuestDialogLine
    {
        public string Text { get; }
        public string ButtonText { get; }
        public int Frames { get; }
        public ModQuestDialogLine(string text, string buttonText = null, int frames = 0)
        { Text = text ?? string.Empty; ButtonText = buttonText ?? string.Empty; Frames = frames; }
    }

    public sealed class ModQuestDialogButton
    {
        private readonly ModQuestAction[] _actions;
        public string Text { get; }
        public string Color { get; }
        public IReadOnlyList<ModQuestAction> Actions => _actions;

        public ModQuestDialogButton(string text, ModQuestAction[] actions, string color = null)
        {
            Text = text ?? string.Empty;
            Color = color ?? "Beige";
            _actions = actions == null ? Array.Empty<ModQuestAction>() : (ModQuestAction[])actions.Clone();
        }
    }

    public sealed class ModQuestAction
    {
        private readonly ModQuestDialogLine[] _lines;
        private readonly ModQuestDialogButton _button;
        public ModQuestActionKind Kind { get; }
        public string Name { get; }
        public string Value { get; }
        public string Title { get; }
        public string Image { get; }
        public bool Flag { get; }
        public DefinitionId Reference { get; }
        public bool HasReference { get; }
        public IReadOnlyList<ModQuestDialogLine> Lines => _lines;
        public ModQuestDialogButton Button => _button;

        private ModQuestAction(ModQuestActionKind kind, string name, string value, string title, string image,
            bool flag, DefinitionId reference, bool hasReference, ModQuestDialogLine[] lines,
            ModQuestDialogButton button = null)
        {
            Kind = kind; Name = name ?? string.Empty; Value = value ?? string.Empty; Title = title ?? string.Empty;
            Image = image ?? string.Empty; Flag = flag; Reference = reference; HasReference = hasReference;
            _lines = lines == null ? Array.Empty<ModQuestDialogLine>() : (ModQuestDialogLine[])lines.Clone();
            _button = button;
        }

        public static ModQuestAction Dialog(string title, string image, ModQuestDialogLine[] lines) =>
            new ModQuestAction(ModQuestActionKind.Dialog, null, null, title, image, false, default(DefinitionId), false, lines);
        public static ModQuestAction Dialog(string title, string image, ModQuestDialogLine[] lines,
            ModQuestDialogButton button) =>
            new ModQuestAction(ModQuestActionKind.Dialog, null, null, title, image, false,
                default(DefinitionId), false, lines, button);
        public static ModQuestAction StoryScreen(ModQuestDialogLine[] lines) =>
            new ModQuestAction(ModQuestActionKind.StoryScreen, null, null, null, null, false, default(DefinitionId), false, lines);
        public static ModQuestAction SetUserVariable(string name, string value) =>
            new ModQuestAction(ModQuestActionKind.SetUserVariable, name, value, null, null, false, default(DefinitionId), false, null);
        public static ModQuestAction ShowBattle(DefinitionId battle, bool locked) =>
            new ModQuestAction(ModQuestActionKind.ShowBattle, null, null, null, null, locked, battle, true, null);
        public static ModQuestAction ToggleBattle(DefinitionId battle, bool visible) =>
            new ModQuestAction(ModQuestActionKind.ToggleBattle, null, null, null, null, visible, battle, true, null);
        public static ModQuestAction SetMapFocus(DefinitionId battle) =>
            new ModQuestAction(ModQuestActionKind.SetMapFocus, null, null, null, null, false, battle, true, null);
        public static ModQuestAction StartFight(DefinitionId fight) =>
            new ModQuestAction(ModQuestActionKind.StartFight, null, null, null, null, false, fight, true, null);
        public static ModQuestAction StartCurrentFight() =>
            new ModQuestAction(ModQuestActionKind.StartCurrentFight, null, null, null, null, false, default(DefinitionId), false, null);
        public static ModQuestAction ToggleEclipseMode(bool enabled) =>
            new ModQuestAction(ModQuestActionKind.ToggleEclipseMode, null, null, null, null, enabled, default(DefinitionId), false, null);
        public static ModQuestAction UpdateEclipseBattles() =>
            new ModQuestAction(ModQuestActionKind.UpdateEclipseBattles, null, null, null, null, false, default(DefinitionId), false, null);
        public static ModQuestAction GiveItem(DefinitionId item) =>
            new ModQuestAction(ModQuestActionKind.GiveItem, null, null, null, null, false, item, true, null);
    }

    public sealed class QuestDefinition
    {
        private readonly string[] _groups, _marks;
        private readonly ModQuestEventKind[] _events;
        private readonly ModQuestCondition[] _conditions;
        private readonly ModQuestAction[] _actions;
        public DefinitionId Id { get; }
        public int Priority { get; }
        public bool Unresumable { get; }
        public bool AllowDoubles { get; }
        public ModQuestActionPlace Place { get; }
        public IReadOnlyList<string> Groups => _groups;
        public IReadOnlyList<string> Marks => _marks;
        public IReadOnlyList<ModQuestEventKind> Events => _events;
        public IReadOnlyList<ModQuestCondition> Conditions => _conditions;
        public IReadOnlyList<ModQuestAction> Actions => _actions;

        internal QuestDefinition(DefinitionId id, int priority, bool unresumable, bool allowDoubles,
            ModQuestActionPlace place, string[] groups, string[] marks, ModQuestEventKind[] events,
            ModQuestCondition[] conditions, ModQuestAction[] actions)
        {
            Id = id; Priority = priority; Unresumable = unresumable; AllowDoubles = allowDoubles; Place = place;
            _groups = groups ?? Array.Empty<string>(); _marks = marks ?? Array.Empty<string>();
            _events = events ?? Array.Empty<ModQuestEventKind>(); _conditions = conditions ?? Array.Empty<ModQuestCondition>();
            _actions = actions ?? Array.Empty<ModQuestAction>();
        }
    }

    public sealed partial class ModContentCatalog
    {
        private readonly Dictionary<DefinitionId, QuestDefinition> _quests = new Dictionary<DefinitionId, QuestDefinition>();
        private readonly List<QuestDefinition> _questValues = new List<QuestDefinition>();
        public IReadOnlyList<QuestDefinition> Quests => _questValues.AsReadOnly();
        public bool TryGetQuest(DefinitionId id, out QuestDefinition value) => _quests.TryGetValue(id, out value);
        internal void CommitP1B(IEnumerable<QuestDefinition> quests)
        { foreach (QuestDefinition quest in quests) { _quests.Add(quest.Id, quest); _questValues.Add(quest); } }
    }

    public sealed partial class ModRegistrationTransaction
    {
        private readonly Dictionary<DefinitionId, QuestDefinition> _p1bQuests = new Dictionary<DefinitionId, QuestDefinition>();
        private int P1BRegistrationCount => _p1bQuests.Count;

        public QuestDefinition RegisterQuest(string localId, int priority, bool unresumable, bool allowDoubles,
            ModQuestActionPlace place, string[] groups, string[] marks, ModQuestEventKind[] events,
            ModQuestCondition[] conditions, ModQuestAction[] actions)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("quests", localId);
            if (_p1bQuests.ContainsKey(id)) throw new ModContentException("Duplicate quest definition: '" + id + "'.");
            if (!Enum.IsDefined(typeof(ModQuestActionPlace), place)) throw new ModContentException("Unsupported quest action place.");
            events = events ?? Array.Empty<ModQuestEventKind>(); conditions = conditions ?? Array.Empty<ModQuestCondition>();
            actions = actions ?? Array.Empty<ModQuestAction>(); groups = groups ?? Array.Empty<string>(); marks = marks ?? Array.Empty<string>();
            if (events.Length == 0 || actions.Length == 0) throw new ModContentException("Quest requires events and actions.");
            for (int i = 0; i < events.Length; i++) if (!Enum.IsDefined(typeof(ModQuestEventKind), events[i]))
                throw new ModContentException("Unsupported quest event.");
            for (int i = 0; i < conditions.Length; i++) ValidateQuestCondition(conditions[i], 0);
            for (int i = 0; i < actions.Length; i++) ValidateQuestAction(actions[i], 0);
            EnsureCapacityForNewRegistration();
            var definition = new QuestDefinition(id, priority, unresumable, allowDoubles, place,
                (string[])groups.Clone(), (string[])marks.Clone(), (ModQuestEventKind[])events.Clone(),
                (ModQuestCondition[])conditions.Clone(), (ModQuestAction[])actions.Clone());
            _p1bQuests.Add(id, definition); return definition;
        }

        private void ValidateQuestCondition(ModQuestCondition condition, int depth)
        {
            if (condition == null || depth > 16) throw new ModContentException("Invalid or excessively nested quest condition.");
            if (condition.Kind == ModQuestConditionKind.Compare)
            { ValidateQuestOperand(condition.Left); ValidateQuestOperand(condition.Right); return; }
            if (condition.Kind != ModQuestConditionKind.All && condition.Kind != ModQuestConditionKind.Any)
                throw new ModContentException("Unsupported quest condition group.");
            if (condition.Children.Count == 0) throw new ModContentException("Quest condition group cannot be empty.");
            for (int i = 0; i < condition.Children.Count; i++) ValidateQuestCondition(condition.Children[i], depth + 1);
        }

        private void ValidateQuestOperand(ModQuestOperand operand)
        {
            if (operand == null || !Enum.IsDefined(typeof(ModQuestOperandKind), operand.Kind)) throw new ModContentException("Invalid quest operand.");
            if (operand.Kind != ModQuestOperandKind.FightWinCount && operand.Kind != ModQuestOperandKind.FightId)
            { if (operand.HasReference) throw new ModContentException("Unexpected quest reference."); return; }
            FightDefinition fight;
            if (!operand.HasReference || operand.Reference.Category != "fights" || !CanReferenceNamespace(operand.Reference.Namespace) ||
                (!_fights.TryGetValue(operand.Reference, out fight) && !_catalog.TryGetFight(operand.Reference, out fight)))
                throw new ModContentException("FightWinCount references an unavailable fight.");
        }

        private void ValidateQuestAction(ModQuestAction action, int depth)
        {
            if (action == null || depth > 16 || !Enum.IsDefined(typeof(ModQuestActionKind), action.Kind))
                throw new ModContentException("Invalid or excessively nested quest action.");
            DefinitionId id = action.Reference;
            if (action.Kind == ModQuestActionKind.ShowBattle || action.Kind == ModQuestActionKind.ToggleBattle ||
                action.Kind == ModQuestActionKind.SetMapFocus)
            {
                BattleDefinition value;
                if (!action.HasReference || id.Category != "battles" || !CanReferenceNamespace(id.Namespace) ||
                    (!_battles.TryGetValue(id, out value) && !_catalog.TryGetBattle(id, out value)))
                    throw new ModContentException("Quest action references an unavailable battle.");
            }
            else if (action.Kind == ModQuestActionKind.StartFight)
            {
                FightDefinition value;
                if (!action.HasReference || id.Category != "fights" || !CanReferenceNamespace(id.Namespace) ||
                    (!_fights.TryGetValue(id, out value) && !_catalog.TryGetFight(id, out value)))
                    throw new ModContentException("Quest action references an unavailable fight.");
            }
            else if (action.Kind == ModQuestActionKind.GiveItem)
            {
                ItemDefinition value;
                if (!action.HasReference || id.Category != "items" || !CanReferenceNamespace(id.Namespace) ||
                    (!TryGetPendingItem(id, out value) && !_catalog.TryResolveItem(id, out value)))
                    throw new ModContentException("Quest action references an unavailable item.");
            }
            if ((action.Kind == ModQuestActionKind.Dialog || action.Kind == ModQuestActionKind.StoryScreen) && action.Lines.Count == 0)
                throw new ModContentException("Quest presentation requires at least one line.");
            if (action.Kind == ModQuestActionKind.Dialog && action.Button != null)
            {
                if (string.IsNullOrWhiteSpace(action.Button.Text))
                    throw new ModContentException("Quest dialog button text must not be empty.");
                if (action.Button.Actions.Count == 0)
                    throw new ModContentException("Quest dialog button requires at least one action.");
                for (int i = 0; i < action.Button.Actions.Count; i++) ValidateQuestAction(action.Button.Actions[i], depth + 1);
            }
            if (action.Kind == ModQuestActionKind.SetUserVariable && string.IsNullOrWhiteSpace(action.Name))
                throw new ModContentException("SetUserVariable requires a name.");
        }

        private void ValidateP1BCommit()
        { foreach (DefinitionId id in _p1bQuests.Keys) if (_catalog.TryGetQuest(id, out QuestDefinition ignored)) throw new ModContentException("Duplicate quest definition: '" + id + "'."); }
        private void ApplyP1BCommit() => _catalog.CommitP1B(_p1bQuests.Values);
        private void ClearP1BPending() => _p1bQuests.Clear();
    }
}
