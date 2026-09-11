using System;
using System.Collections.Generic;
using System.Globalization;
using MoonSharp.Interpreter;

namespace Eclipse.Modding
{
    public sealed partial class MoonSharpScriptRuntime
    {
        private sealed partial class MoonSharpScriptContext
        {
            private static readonly Dictionary<string, ModEffectEvent> BehaviorEvents = new Dictionary<string, ModEffectEvent>
            {
                { "on_fight_begin", ModEffectEvent.FightBegin }, { "on_round_begin", ModEffectEvent.RoundBegin },
                { "on_round_end", ModEffectEvent.RoundEnd }, { "on_fight_end", ModEffectEvent.FightEnd },
                { "on_damage_received", ModEffectEvent.DamageReceived }, { "on_damage_dealt", ModEffectEvent.DamageDealt },
                { "on_damage_resolving", ModEffectEvent.DamageResolving }, { "on_damage_dealing", ModEffectEvent.DamageDealing },
                { "on_combo_changed", ModEffectEvent.ComboChanged }, { "on_style_changed", ModEffectEvent.StyleChanged },
                { "on_block", ModEffectEvent.Block }, { "on_critical", ModEffectEvent.Critical }
            };
            private sealed class BehaviorState
            {
                public string Epoch;
                public Dictionary<string, ModParameterValue> Values;
            }
            private readonly Dictionary<DefinitionId, ModBehaviorDefinition> _instanceDefinitions = new Dictionary<DefinitionId, ModBehaviorDefinition>();
            private readonly Dictionary<DefinitionId, Dictionary<int, DynValue>> _instanceMigrations = new Dictionary<DefinitionId, Dictionary<int, DynValue>>();
            private System.Runtime.CompilerServices.ConditionalWeakTable<System.Xml.XmlNode, Dictionary<DefinitionId, BehaviorState>> _instanceState =
                new System.Runtime.CompilerServices.ConditionalWeakTable<System.Xml.XmlNode, Dictionary<DefinitionId, BehaviorState>>();

            private Table PrepareBehaviorState(ModBehaviorDefinition definition, Table parameters,
                IReadOnlyDictionary<string, string> context, IModFighterOperations fighter, ModEffectEvent effectEvent, out Action commit)
            {
                if (definition.StateLifetime == "saved" && context != null && context.TryGetValue("source", out var source) && (source == "warrior" || source == "rule"))
                    throw new ModContentException("NPC behavior instances support fight/round state; saved state requires player-owned equipment or a learned perk.");
                var node = (fighter as IModBehaviorInstanceSource)?.SavedInstance;
                if (node == null) throw new ModContentException("Behavior state requires an equipped instance.");
                var instances = _instanceState.GetOrCreateValue(node);
                string fight = null, round = null;
                context?.TryGetValue("fight_id", out fight);
                context?.TryGetValue("round", out round);
                string epoch = definition.StateLifetime == "saved" ? "saved" : (fight ?? "fight") +
                    (definition.StateLifetime == "round" ? ":" + round : "");
                instances.TryGetValue(definition.Id, out var cached);
                if (cached == null || cached.Epoch != epoch)
                {
                    var values = definition.StateSchema.ResolveValues(null);
                    if (definition.StateLifetime == "saved")
                    {
                        System.Xml.XmlElement stored = null;
                        foreach (System.Xml.XmlNode child in node.ChildNodes)
                            if (child is System.Xml.XmlElement e && e.Name == "EclipseBehaviorState" && e.GetAttribute("Behavior") == definition.Id.ToString())
                            { if (stored != null) throw new ModContentException("Duplicate behavior state."); stored = e; }
                        if (stored != null)
                        {
                            if (!int.TryParse(stored.GetAttribute("Version"), out int version) || version < 1 || version > definition.StateVersion)
                                throw new ModContentException("Unsupported saved behavior state version; saved data preserved.");
                            var input = new Table(_script);
                            foreach (System.Xml.XmlNode child in stored.ChildNodes)
                            {
                                if (!(child is System.Xml.XmlElement field) || field.Name != "Value") continue;
                                string name = field.GetAttribute("Name");
                                ModParameterDefinition.ValidateName(name);
                                if (!input.Get(name).IsNil()) throw new ModContentException("Duplicate saved behavior field.");
                                if (!Enum.TryParse(field.GetAttribute("Type"), out ModParameterType type) ||
                                    !ModParameterValue.TryParse(type, field.GetAttribute("Value"), out var value))
                                    throw new ModContentException("Invalid saved behavior field.");
                                input.Set(name, ToDynValue(value));
                            }
                            while (version < definition.StateVersion)
                            {
                                if (!_instanceMigrations[definition.Id].TryGetValue(version, out var migration))
                                    throw new ModContentException("Missing behavior state migration from version " + version + ".");
                                var result = RunBounded(migration, definition.Id + ":migration", MaxStateMigrationInstructionSlices, new[] { DynValue.NewTable(input) });
                                if (result != null && !result.IsNil())
                                {
                                    if (result.Type != DataType.Table) throw new ModContentException("Migration must return a table or nil.");
                                    input = result.Table;
                                }
                                version++;
                            }
                            values = ReadInstanceState(definition, input);
                        }
                    }
                    cached = new BehaviorState { Epoch = epoch, Values = values };
                }
                var state = new Table(_script);
                foreach (var pair in cached.Values) state.Set(pair.Key, ToDynValue(pair.Value));
                var self = new Table(_script);
                self.Set("params", DynValue.NewTable(parameters)); self.Set("state", DynValue.NewTable(state));
                commit = () =>
                {
                    if (self.Get("state").Type != DataType.Table) throw new ModContentException("self.state must remain a table.");
                    var values = ReadInstanceState(definition, self.Get("state").Table);
                    if (definition.StateLifetime == "saved")
                    {
                        var replacement = node.OwnerDocument.CreateElement("EclipseBehaviorState");
                        replacement.SetAttribute("Behavior", definition.Id.ToString());
                        replacement.SetAttribute("Version", definition.StateVersion.ToString(CultureInfo.InvariantCulture));
                        foreach (var pair in values)
                        {
                            var field = node.OwnerDocument.CreateElement("Value");
                            field.SetAttribute("Name", pair.Key); field.SetAttribute("Type", pair.Value.Type.ToString());
                            field.SetAttribute("Value", pair.Value.ToWireString()); replacement.AppendChild(field);
                        }
                        System.Xml.XmlNode previous = null;
                        foreach (System.Xml.XmlNode child in node.ChildNodes)
                            if (child.Name == "EclipseBehaviorState" && child.Attributes?["Behavior"]?.Value == definition.Id.ToString()) previous = child;
                        if (previous != null) node.ReplaceChild(replacement, previous); else node.AppendChild(replacement);
                    }
                    instances[definition.Id] = new BehaviorState { Epoch = epoch, Values = values };
                };
                return self;
            }

            private Dictionary<string, ModParameterValue> ReadInstanceState(ModBehaviorDefinition definition, Table state)
            {
                var wrapper = new Table(_script); wrapper.Set("state", DynValue.NewTable(state));
                return definition.StateSchema.ResolveValues(OptionalTypedParameterMap(wrapper, "state", definition.StateSchema, "behavior"));
            }

            private void AddP2Modules(Table root)
            {
                foreach (string name in new[] { "modes", "events", "raids" })
                {
                    string category = name;
                    var module = new Table(_script);
                    module.Set("register", DynValue.NewCallback((ctx, args) => RegisterMode(args, category)));
                    root.Set(name, DynValue.NewTable(module));
                }
                var timers = new Table(_script);
                timers.Set("set", DynValue.NewCallback((ctx, args) => ApiCall("sf2.timers.set", () =>
                {
                    var table = args.AsType(0, "sf2.timers.set", DataType.Table, false).Table;
                    ValidateFields(table, "sf2.timers.set", "subsystem", "seconds", "skip_enabled");
                    _api.SetTimer(RequiredString(table, "subsystem", "timer"), RequiredInt(table, "seconds", "timer"),
                        OptionalBool(table, "skip_enabled", true, "timer"));
                    return DynValue.Nil;
                })));
                root.Set("timers", DynValue.NewTable(timers));
                var services = new Table(_script);
                services.Set("disable", DynValue.NewCallback((ctx, args) => ApiCall("sf2.services.disable", () =>
                {
                    _api.DisableFeature(args.AsType(0, "sf2.services.disable", DataType.String, false).String);
                    return DynValue.Nil;
                })));
                root.Set("services", DynValue.NewTable(services));
            }

            private static long ReadUnixTime(Table table, string field)
            {
                var value = table.Get(field);
                if (value.IsNil()) return 0;
                long time = ParameterValue(ModParameterType.Integer, value, field).Integer;
                if (time < 0 || time > 253402300799L) throw new ModContentException(field + " must be a UTC Unix timestamp from 0 through year 9999.");
                return time;
            }

            private DynValue RegisterMode(CallbackArguments args, string category)
            {
                string function = "sf2." + category + ".register";
                return ApiCall(function, () =>
                {
                    Table table = args.AsType(0, function, DataType.Table, false).Table;
                    ValidateFields(table, function, "id", "fights", "repeatable", "reset_on_loss", "minimum_level",
                        "starts_at", "ends_at", "entry_item", "entry_count", "hard_mode");
                    var list = table.Get("fights");
                    if (list.Type != DataType.Table || list.Table.Length == 0) throw new ModContentException("Mode requires fights.");
                    int length = list.Table.Length;
                    if (length > 100) throw new ModContentException("Mode supports at most 100 fights.");
                    foreach (var pair in list.Table.Pairs)
                        if (pair.Key.Type != DataType.Number || pair.Key.Number < 1 || pair.Key.Number > length || pair.Key.Number != Math.Floor(pair.Key.Number))
                            throw new ModContentException("Mode fights must be a dense array.");
                    var fights = new List<DefinitionId>();
                    for (int i = 1; i <= list.Table.Length; i++)
                    {
                        var wrapper = new Table(_script); wrapper.Set("fight", list.Table.Get(i));
                        fights.Add(RequiredHandle(wrapper, "fight", _fightHandles, "fight", function));
                    }
                    DefinitionId item = default;
                    int count = 0;
                    if (!table.Get("entry_item").IsNil())
                    {
                        item = RequiredHandle(table, "entry_item", _itemHandles, "item", function);
                        count = RequiredInt(table, "entry_count", function);
                        if (count <= 0) throw new ModContentException("Entry count must be positive.");
                    }
                    else if (!table.Get("entry_count").IsNil()) throw new ModContentException("Entry count requires an entry item.");
                    var mode = _api.RegisterMode(RequiredString(table, "id", function), fights.ToArray(),
                        OptionalBool(table, "repeatable", false, function), OptionalBool(table, "reset_on_loss", false, function),
                        category == "raids", OptionalBool(table, "hard_mode", false, function),
                        table.Get("minimum_level").IsNil() ? 1 : RequiredInt(table, "minimum_level", function),
                        ReadUnixTime(table, "starts_at"),
                        ReadUnixTime(table, "ends_at"), item, count);
                    return DynValue.Nil;
                });
            }
        }
    }
}
