using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Eclipse.Modding
{
    public sealed partial class MoonSharpScriptRuntime
    {
        private sealed partial class MoonSharpScriptContext
        {
            private DynValue RegisterHotGroundRule(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.rules.hot_ground";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "frames", "nodes", "animations", "target", "mode", "rounds");
                    int frames = RequiredInt(table, "frames", function);
                    DynValue nodesValue = table.Get("nodes");
                    if (nodesValue.Type != DataType.Table || nodesValue.Table.Length < 1 || nodesValue.Table.Length > 32)
                        throw new ModContentException(function + " field 'nodes' must be a dense array of 1..32 entries.");
                    int length = nodesValue.Table.Length;
                    var nodes = new List<ModTrialNodeLimit>(length);
                    for (int i = 1; i <= length; i++)
                    {
                        DynValue value = nodesValue.Table.Get(i);
                        string where = function + " field 'nodes' entry " + i;
                        if (value.Type != DataType.Table) throw new ModContentException(where + " must be a table.");
                        Table node = value.Table;
                        ValidateFields(node, where, "name", "axis", "min", "max");
                        nodes.Add(new ModTrialNodeLimit(
                            RequiredString(node, "name", where),
                            ParseTrialAxis(RequiredString(node, "axis", where), where),
                            OptionalTrialFloat(node, "min", where), OptionalTrialFloat(node, "max", where)));
                    }
                    EnsureDenseTrialArray(nodesValue.Table, length, function + " field 'nodes'");
                    string[] animations = OptionalStringArray(table, "animations", function);
                    return NewHandle(_ruleHandles, _api.RegisterHotGroundRule(
                        RequiredString(table, "id", function), frames, nodes.ToArray(), animations,
                        ParseRuleTarget(OptionalString(table, "target", "player", function), function),
                        ParseRuleMode(OptionalString(table, "mode", "all", function), function),
                        OptionalIntArray(table, "rounds", function)).Id);
                });
            }

            private DynValue RegisterRingOutRule(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.rules.ring_out";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "node", "axis", "min", "max", "target", "mode", "rounds");
                    return NewHandle(_ruleHandles, _api.RegisterRingOutRule(
                        RequiredString(table, "id", function), RequiredString(table, "node", function),
                        ParseTrialAxis(RequiredString(table, "axis", function), function),
                        RequiredTrialFloat(table, "min", function), RequiredTrialFloat(table, "max", function),
                        ParseRuleTarget(OptionalString(table, "target", "all", function), function),
                        ParseRuleMode(OptionalString(table, "mode", "all", function), function),
                        OptionalIntArray(table, "rounds", function)).Id);
                });
            }

            private DynValue RegisterRegenerationRule(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.rules.regeneration";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "rate", "frames_after_hit", "target", "mode", "rounds");
                    return NewHandle(_ruleHandles, _api.RegisterRegenerationRule(
                        RequiredString(table, "id", function), RequiredTrialFloat(table, "rate", function),
                        RequiredInt(table, "frames_after_hit", function),
                        ParseRuleTarget(OptionalString(table, "target", "all", function), function),
                        ParseRuleMode(OptionalString(table, "mode", "all", function), function),
                        OptionalIntArray(table, "rounds", function)).Id);
                });
            }

            private DynValue RegisterNoAnimationRule(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.rules.no_animation";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "name", "mode", "rounds");
                    return NewHandle(_ruleHandles, _api.RegisterNoAnimationRule(
                        RequiredString(table, "id", function), RequiredString(table, "name", function),
                        ParseRuleMode(OptionalString(table, "mode", "all", function), function),
                        OptionalIntArray(table, "rounds", function)).Id);
                });
            }

            private DynValue RegisterRemoveIntervalRule(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.rules.remove_interval";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "type", "target", "mode", "rounds");
                    return NewHandle(_ruleHandles, _api.RegisterRemoveIntervalRule(
                        RequiredString(table, "id", function), ParseTrialInterval(RequiredString(table, "type", function), function),
                        ParseRuleTarget(OptionalString(table, "target", "all", function), function),
                        ParseRuleMode(OptionalString(table, "mode", "all", function), function),
                        OptionalIntArray(table, "rounds", function)).Id);
                });
            }

            private static ModTrialAxis ParseTrialAxis(string value, string function)
            {
                if (value == "X") return ModTrialAxis.X;
                if (value == "Y") return ModTrialAxis.Y;
                throw new ModContentException(function + " axis must be X or Y.");
            }

            private static ModTrialIntervalType ParseTrialInterval(string value, string function)
            {
                switch (value)
                {
                    case "Attack": return ModTrialIntervalType.Attack;
                    case "Block": return ModTrialIntervalType.Block;
                    case "Invulnerable": return ModTrialIntervalType.Invulnerable;
                    case "SelfUninterrupt": return ModTrialIntervalType.SelfUninterrupt;
                    case "Uninterrupt": return ModTrialIntervalType.Uninterrupt;
                    case "Unstable": return ModTrialIntervalType.Unstable;
                    default: throw new ModContentException(function + " type must be Attack, Block, Invulnerable, SelfUninterrupt, Uninterrupt, or Unstable.");
                }
            }

            private static float RequiredTrialFloat(Table table, string field, string function)
            {
                float? value = OptionalTrialFloat(table, field, function);
                if (!value.HasValue) throw new ModContentException(function + " field '" + field + "' is required.");
                return value.Value;
            }

            private static float? OptionalTrialFloat(Table table, string field, string function)
            {
                DynValue value = table.Get(field);
                if (value.IsNil()) return null;
                if (value.Type != DataType.Number || double.IsNaN(value.Number) || double.IsInfinity(value.Number) ||
                    value.Number < -float.MaxValue || value.Number > float.MaxValue)
                    throw new ModContentException(function + " field '" + field + "' must be a finite single-precision number.");
                return (float)value.Number;
            }

            private static void EnsureDenseTrialArray(Table table, int length, string function)
            {
                int count = 0;
                foreach (TablePair pair in table.Pairs)
                {
                    count++;
                    if (pair.Key.Type != DataType.Number || pair.Key.Number < 1 || pair.Key.Number > length ||
                        pair.Key.Number != Math.Truncate(pair.Key.Number))
                        throw new ModContentException(function + " must be a dense array.");
                }
                if (count != length) throw new ModContentException(function + " must be a dense array.");
            }
        }
    }
}
