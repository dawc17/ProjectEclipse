using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Eclipse.Modding
{
    public sealed partial class MoonSharpScriptRuntime
    {
        // Move short form. Each short table names its kind with one key
        // ({ not_mod = "Stun" }, { node = "NPivot", player = "Enemy" }) and is
        // expanded here into the typed move definition the long form produced.
        // Public input accepts only compact declarations. Normalized tables below
        // are private intermediate values, never a second public syntax.
        private sealed partial class MoonSharpScriptContext
        {
            private static readonly string[] ShortConditionKinds =
            {
                "key", "keys", "mod", "interval", "animation", "stage", "round_result", "screen", "actor",
                "character", "perk", "item", "bullets", "distance", "direction", "all", "any",
            };

            private static readonly string[] ShortPointObjects = { "node", "pivot", "wall", "animation", "floor", "com" };

            // Timeline event keys in emission order; frames come first, ascending.
            private static readonly string[,] ShortTimelineEvents =
            {
                { "birth", "Birth" }, { "round_stage", "RoundStage" }, { "round_start", "RoundStart" },
                { "key_pressed", "KeyPressed" }, { "key_released", "KeyReleased" },
                { "animation_start", "AnimationStart" }, { "interval_start", "IntervalStart" },
                { "every_frame", "EveryFrame" }, { "strike", "Strike" }, { "hit", "Hit" }, { "wall_hit", "WallHit" },
                { "interval_end", "IntervalEnd" }, { "animation_end", "AnimationEnd" },
                { "mod_expires", "ModExpires" }, { "round_end", "RoundEnd" },
            };

            private static readonly string[] ShortActionKinds =
            {
                "sound", "stop_sound", "play_sound", "effect", "stop_effect", "stop_follow_effect", "projectile",
                "add_bullets", "delete_actor", "play_animation", "shake", "try_on_end",
            };

            private static readonly string[] ShortEventKinds =
            {
                "interval_end", "interval_start", "round_stage_start", "mod_expires", "animation_start",
                "animation_end", "hit", "strike", "every_frame", "birth", "key_pressed",
            };

            private static readonly string[] DamageTermOrder = { "WeaponDamage", "RangedDamage", "MagicDamage", "UnarmedDamage" };

            private Table NewShortTable(params object[] pairs)
            {
                var table = new Table(_script);
                for (int i = 0; i < pairs.Length; i += 2)
                    if (pairs[i + 1] != null) table.Set((string)pairs[i], ShortValue(pairs[i + 1]));
                return table;
            }

            private static DynValue ShortValue(object value)
            {
                if (value is DynValue dyn) return dyn;
                if (value is string text) return DynValue.NewString(text);
                if (value is bool flag) return DynValue.NewBoolean(flag);
                if (value is Table table) return DynValue.NewTable(table);
                if (value is double number) return DynValue.NewNumber(number);
                if (value is int integer) return DynValue.NewNumber(integer);
                throw new InvalidOperationException("Unsupported short-form value.");
            }

            private Table ShortArray(IEnumerable<Table> entries)
            {
                var array = new Table(_script);
                int index = 1;
                foreach (Table entry in entries) array.Set(index++, DynValue.NewTable(entry));
                return array;
            }

            private Table SingleEntry(DynValue value)
            {
                var array = new Table(_script);
                array.Set(1, value);
                return array;
            }

            private static bool IsArrayTable(Table table) => !table.Get(1).IsNil();

            // Returns the single kind key of a short table from the allowed set.
            private static string ShortKind(Table table, string[] kinds, bool allowNegated, string function, string what)
            {
                string found = null;
                foreach (TablePair pair in table.Pairs)
                {
                    if (pair.Value.IsNil() || pair.Key.Type != DataType.String) continue;
                    string key = pair.Key.String;
                    string bare = allowNegated && key.StartsWith("not_", StringComparison.Ordinal) ? key.Substring(4) : key;
                    if (Array.IndexOf(kinds, bare) < 0) continue;
                    if (found != null)
                        throw new ModContentException(function + " " + what + " names two kinds: '" + found + "' and '" + key + "'.");
                    found = key;
                }
                if (found == null)
                    throw new ModContentException(function + " " + what + " needs one kind key: " + string.Join(", ", kinds) + ".");
                return found;
            }

            private static string ShortString(Table table, string key, string function)
            {
                DynValue value = table.Get(key);
                if (value.Type != DataType.String || value.String.Length == 0)
                    throw new ModContentException(function + " field '" + key + "' must be a non-empty string.");
                return value.String;
            }

            private ModMoveCondition[] ReadMoveConditions(DynValue value, string function)
            {
                if (value.IsNil()) return Array.Empty<ModMoveCondition>();
                var result = new List<ModMoveCondition>();
                AppendConditions(RequireArray(value, function), function, result, 0);
                return result.ToArray();
            }

            private void AppendConditions(Table array, string function, List<ModMoveCondition> result, int depth)
            {
                if (depth > 8) throw new ModContentException(function + " nests condition lists too deeply.");
                int count = 0;
                for (int i = 1; !array.Get(i).IsNil(); i++)
                {
                    count++;
                    var entry = array.Get(i);
                    string where = function + "[" + i + "]";
                    if (entry.Type != DataType.Table) throw new ModContentException(where + " must be a condition table.");
                    Table table = entry.Table;
                    if (IsArrayTable(table)) { AppendConditions(table, where, result, depth + 1); continue; }
                    if (!table.Get("controllable").IsNil())
                    {
                        ValidateFields(table, where, "controllable");
                        if (table.Get("controllable").Type != DataType.Boolean || !table.Get("controllable").Boolean)
                            throw new ModContentException(where + " field 'controllable' must be true.");
                        result.Add(new ModMoveCondition(ModMoveConditionKind.CurrentInterval, "SemiUninterrupt", not: true));
                        result.Add(new ModMoveCondition(ModMoveConditionKind.CurrentInterval, "Uninterrupt", not: true));
                        result.Add(new ModMoveCondition(ModMoveConditionKind.RoundStage, "Fight"));
                        result.Add(new ModMoveCondition(ModMoveConditionKind.All, not: true, children: new[] {
                            new ModMoveCondition(ModMoveConditionKind.CurrentAnimation, "$Move"),
                            new ModMoveCondition(ModMoveConditionKind.CurrentInterval, "SemiUninterrupt") }));
                        result.Add(new ModMoveCondition(ModMoveConditionKind.CurrentAnimation, "Physical", not: true));
                        continue;
                    }
                    result.Add(ReadCompactCondition(table, where, depth));
                }
                EnsureDenseArray(array, count, function);
            }

            private ModMoveCondition ReadCompactCondition(Table table, string function, int depth)
            {
                string key = ShortKind(table, ShortConditionKinds, true, function, "condition");
                bool negated = key.StartsWith("not_", StringComparison.Ordinal);
                string kind = negated ? key.Substring(4) : key;
                string type = kind == "mod" ? "mod_exists" : kind == "interval" ? "current_interval" :
                    kind == "animation" ? "current_animation" : kind == "stage" ? "round_stage" :
                    kind == "actor" ? "actor_name" : kind == "key" ? "keys" : kind;
                var parsed = ParseMoveConditionKind(type, function);
                if (kind == "key" || kind == "keys")
                {
                    ValidateFields(table, function, kind == "key" ? new[] { key, "press" } : new[] { key });
                    var keys = new List<ModMoveKey>();
                    if (kind == "key") keys.Add(new ModMoveKey(ShortString(table, key, function), OptionalString(table, "press", "Tap", function)));
                    else
                    {
                        Table array = RequireArray(table.Get(key), function + "." + key);
                        for (int i = 1; !array.Get(i).IsNil(); i++)
                        {
                            var item = array.Get(i);
                            if (item.Type == DataType.String) keys.Add(new ModMoveKey(item.String, "Tap"));
                            else
                            {
                                if (item.Type != DataType.Table || item.Table.Get(1).Type != DataType.String)
                                    throw new ModContentException(function + "." + key + " requires key names or { name, press = ... }.");
                                foreach (var pair in item.Table.Pairs)
                                    if (!pair.Value.IsNil() && !(pair.Key.Type == DataType.Number && pair.Key.Number == 1) && !(pair.Key.Type == DataType.String && pair.Key.String == "press"))
                                        throw new ModContentException(function + "." + key + " accepts only a key name and press.");
                                keys.Add(new ModMoveKey(item.Table.Get(1).String, OptionalString(item.Table, "press", "Tap", function)));
                            }
                        }
                        EnsureDenseArray(array, keys.Count, function + "." + key);
                    }
                    return new ModMoveCondition(parsed, not: negated, keys: keys.ToArray());
                }
                if (kind == "all" || kind == "any")
                {
                    ValidateFields(table, function, key);
                    var children = new List<ModMoveCondition>();
                    AppendConditions(RequireArray(table.Get(key), function + "." + key), function + "." + key, children, depth + 1);
                    return new ModMoveCondition(parsed, not: negated, children: children.ToArray());
                }
                if (kind == "distance")
                {
                    ValidateFields(table, function, key, "min", "max", "from", "to");
                    return new ModMoveCondition(parsed, not: negated, distance: new ModMoveDistance(ShortString(table, key, function),
                        ReadMovePoint(table.Get("from"), function + ".from"), ReadMovePoint(table.Get("to"), function + ".to"),
                        OptionalFloat(table, "min", -1000000, function), OptionalFloat(table, "max", 1000000, function)));
                }
                if (kind == "direction")
                {
                    ValidateFields(table, function, key, "from", "to");
                    return new ModMoveCondition(parsed, player: ShortString(table, key, function), not: negated,
                        direction: new ModMoveDirection(ReadMovePoint(table.Get("from"), function + ".from"), ReadMovePoint(table.Get("to"), function + ".to")));
                }
                if (kind == "character")
                {
                    ValidateFields(table, function, key);
                    return new ModMoveCondition(parsed, RequiredHandle(table, key, _warriorHandles, "warrior", function).ToString(), not: negated);
                }
                if (kind == "perk")
                {
                    ValidateFields(table, function, key, "player");
                    return new ModMoveCondition(parsed, RequiredHandle(table, key, _perkHandles, "perk", function).ToString(), OptionalStringAllowEmpty(table, "player", string.Empty, function), not: negated);
                }
                if (kind == "item")
                {
                    ValidateFields(table, function, key, "subtype", "name", "player");
                    return new ModMoveCondition(parsed, OptionalStringAllowEmpty(table, "name", string.Empty, function),
                        OptionalStringAllowEmpty(table, "player", string.Empty, function), ShortString(table, key, function),
                        OptionalStringAllowEmpty(table, "subtype", string.Empty, function), negated);
                }
                if (kind == "bullets")
                {
                    ValidateFields(table, function, key, "min", "max", "player");
                    return new ModMoveCondition(parsed, player: OptionalStringAllowEmpty(table, "player", string.Empty, function), not: negated,
                        bullets: new ModMoveBulletRange(ShortString(table, key, function), OptionalInt(table, "min", 0, function), OptionalInt(table, "max", int.MaxValue, function)));
                }
                ValidateFields(table, function, key, "player");
                return new ModMoveCondition(parsed, ShortString(table, key, function), OptionalStringAllowEmpty(table, "player", string.Empty, function), not: negated);
            }

            private ModMovePoint ReadMovePoint(DynValue value, string function)
            {
                if (value.Type != DataType.Table) throw new ModContentException(function + " requires a point table.");
                Table table = value.Table;
                string key = ShortKind(table, ShortPointObjects, false, function, "point");
                ValidateFields(table, function, key, "player", "x", "y");
                string obj = key == "node" ? "Nodes" : key == "pivot" ? "Pivot" : key == "wall" ? "Wall" :
                    key == "animation" ? "Animation" : key == "floor" ? "Floor" : "COM";
                var part = table.Get(key);
                string partName = null, player = table.Get("player").IsNil() ? null : ShortString(table, "player", function);
                if (key == "node" || key == "wall") partName = ShortString(table, key, function);
                else if (part.Type == DataType.String)
                {
                    if (player != null) throw new ModContentException(function + " gives the player twice.");
                    player = part.String;
                }
                else if (part.Type != DataType.Boolean || !part.Boolean) throw new ModContentException(function + " field '" + key + "' must be a player name or true.");
                return new ModMovePoint(obj, player, partName, UiNumber(table, "x"), UiNumber(table, "y"));
            }

            // Frames ascending, then events in ShortTimelineEvents order. Entries under
            // one key keep their written order.
            private Table ExpandTimeline(DynValue value, string function)
            {
                if (value.Type != DataType.Table) throw new ModContentException(function + " must be a table.");
                Table timeline = value.Table;
                var frames = new SortedDictionary<int, DynValue>();
                var events = new Dictionary<string, DynValue>(StringComparer.Ordinal);
                foreach (TablePair pair in timeline.Pairs)
                {
                    if (pair.Value.IsNil()) continue;
                    if (pair.Key.Type == DataType.Number)
                    {
                        double frame = pair.Key.Number;
                        if (frame != Math.Floor(frame) || frame < 0 || frame > 100000)
                            throw new ModContentException(function + " frame keys must be whole numbers 0..100000.");
                        frames.Add((int)frame, pair.Value);
                    }
                    else if (pair.Key.Type == DataType.String && EventName(pair.Key.String) != null) events.Add(pair.Key.String, pair.Value);
                    else throw new ModContentException(function + " has unsupported key '" + pair.Key.ToPrintString() +
                        "'; use frame numbers or event names such as strike, hit and animation_end.");
                }
                var actions = new List<Table>();
                foreach (var frame in frames)
                    AppendTimelineActions(frame.Value, function + "[" + frame.Key + "]", "frame", DynValue.NewNumber(frame.Key), actions);
                for (int i = 0; i < ShortTimelineEvents.GetLength(0); i++)
                    if (events.TryGetValue(ShortTimelineEvents[i, 0], out DynValue entries))
                        AppendTimelineActions(entries, function + "." + ShortTimelineEvents[i, 0], "event",
                            DynValue.NewString(ShortTimelineEvents[i, 1]), actions);
                return ShortArray(actions);
            }

            private static string EventName(string key)
            {
                for (int i = 0; i < ShortTimelineEvents.GetLength(0); i++)
                    if (ShortTimelineEvents[i, 0] == key) return ShortTimelineEvents[i, 1];
                return null;
            }

            private void AppendTimelineActions(DynValue value, string function, string triggerField, DynValue trigger, List<Table> actions)
            {
                if (value.Type != DataType.Table) throw new ModContentException(function + " must be an action table or a list of them.");
                Table table = value.Table;
                if (!IsArrayTable(table)) { actions.Add(ExpandAction(table, function, triggerField, trigger)); return; }
                int count = 0;
                for (int i = 1; !table.Get(i).IsNil(); i++)
                {
                    count++;
                    DynValue entry = table.Get(i);
                    if (entry.Type != DataType.Table) throw new ModContentException(function + "[" + i + "] must be an action table.");
                    actions.Add(ExpandAction(entry.Table, function + "[" + i + "]", triggerField, trigger));
                }
                EnsureDenseArray(table, count, function);
            }

            private Table ExpandAction(Table table, string function, string triggerField, DynValue trigger)
            {
                string kind = ShortKind(table, ShortActionKinds, false, function, "action");
                DynValue value = table.Get(kind);
                Table action;
                switch (kind)
                {
                    case "sound":
                        ValidateFields(table, function, kind);
                        action = NewShortTable("type", "random_sound", "core_sounds", value.Type == DataType.String ? SingleEntry(value) : (object)value);
                        break;
                    case "stop_sound":
                        ValidateFields(table, function, kind);
                        action = NewShortTable("type", "stop_sound", "core_sound", ShortString(table, kind, function));
                        break;
                    case "play_sound":
                        ValidateFields(table, function, kind, "voice");
                        action = NewShortTable("type", "sound", "sound", NewShortTable("core_sound", ShortString(table, kind, function),
                            "voice", table.Get("voice").IsNil() ? null : (object)ShortString(table, "voice", function)));
                        break;
                    case "effect":
                        ValidateFields(table, function, kind);
                        action = NewShortTable("type", "effect", "effect", value);
                        break;
                    case "stop_effect":
                    case "stop_follow_effect":
                        ValidateFields(table, function, kind);
                        action = NewShortTable("type", kind, "effect_name", ShortString(table, kind, function));
                        break;
                    case "projectile":
                        ValidateFields(table, function, kind);
                        action = NewShortTable("type", "create_projectile", "projectile", value);
                        break;
                    case "add_bullets":
                        ValidateFields(table, function, kind, "amount");
                        action = NewShortTable("type", "add_bullets", "bullets",
                            NewShortTable("type", ShortString(table, kind, function), "value", table.Get("amount")));
                        break;
                    case "delete_actor":
                        ValidateFields(table, function, kind);
                        action = NewShortTable("type", "delete_actor", "player", ShortString(table, kind, function));
                        break;
                    case "play_animation":
                        ValidateFields(table, function, kind, "player", "child_name");
                        action = NewShortTable("type", "play_animation",
                            value.Type == DataType.String ? "core_animation" : "move", value,
                            "player", table.Get("player"), "child_name", table.Get("child_name").IsNil() ? null : (object)table.Get("child_name"));
                        break;
                    case "shake":
                        ValidateFields(table, function, kind);
                        action = NewShortTable("type", "shake_screen", "shake", value);
                        break;
                    default:
                        ValidateFields(table, function, kind);
                        if (value.Type != DataType.Boolean || !value.Boolean) throw new ModContentException(function + " field 'try_on_end' must be true.");
                        action = NewShortTable("type", "try_on_end");
                        break;
                }
                action.Set(triggerField, trigger);
                return action;
            }

            private DynValue ExpandDirection(DynValue value, string function)
            {
                if (value.Type != DataType.String) return value;
                if (value.String != "face_enemy") throw new ModContentException(function + " string must be \"face_enemy\".");
                return DynValue.NewTable(NewShortTable(
                    "from", NewShortTable("node", "NPivot", "player", "Me"),
                    "to", NewShortTable("node", "NPivot", "player", "Enemy")));
            }

            // animation = "animations/fireball_player" names a binary in this mod.
            private AssetId MoveAnimationAsset(Table table, string function)
            {
                DynValue value = table.Get("animation");
                if (value.Type == DataType.String) return _api.RequireAsset(value.String, AssetKind.Binary);
                return RequiredHandle(table, "animation", _binaryHandles, "binary", function);
            }
        }
    }

}
