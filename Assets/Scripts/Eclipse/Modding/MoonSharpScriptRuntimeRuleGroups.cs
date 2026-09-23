using System;
using MoonSharp.Interpreter;

namespace Eclipse.Modding
{
    public sealed partial class MoonSharpScriptRuntime
    {
        private sealed partial class MoonSharpScriptContext
        {
            private DynValue RegisterNoHealthBarRule(ScriptExecutionContext context, CallbackArguments args) =>
                RegisterFlagRule(args, "sf2.rules.no_health_bar", ModFightRuleKind.NoHealthBar);

            private DynValue RegisterInvertJoystickRule(ScriptExecutionContext context, CallbackArguments args) =>
                RegisterFlagRule(args, "sf2.rules.invert_joystick", ModFightRuleKind.InvertJoystick);

            private DynValue RegisterFlagRule(CallbackArguments args, string function, ModFightRuleKind kind)
            {
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "target", "mode", "rounds");
                    return NewHandle(_ruleHandles, _api.RegisterFlagRule(RequiredString(table, "id", function), kind,
                        ParseRuleTarget(OptionalString(table, "target", "all", function), function),
                        ParseRuleMode(OptionalString(table, "mode", "all", function), function),
                        OptionalIntArray(table, "rounds", function)).Id);
                });
            }

            private DynValue RegisterRandomAreaRule(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.rules.random_area";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "image", "icon", "width", "fade_in", "frames_on", "fade_out",
                        "frames_off", "target", "mode", "rounds");
                    return NewHandle(_ruleHandles, _api.RegisterRandomAreaRule(RequiredString(table, "id", function),
                        RequiredString(table, "image", function), OptionalString(table, "icon", string.Empty, function),
                        RequiredTrialFloat(table, "width", function), RequiredInt(table, "fade_in", function),
                        RequiredInt(table, "frames_on", function), RequiredInt(table, "fade_out", function),
                        RequiredInt(table, "frames_off", function),
                        ParseRuleTarget(OptionalString(table, "target", "all", function), function),
                        ParseRuleMode(OptionalString(table, "mode", "all", function), function),
                        OptionalIntArray(table, "rounds", function)).Id);
                });
            }

            private DynValue RegisterGroupRule(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.rules.group";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "description", "rules", "mode", "rounds");
                    DefinitionId? description = table.Get("description").IsNil() ? (DefinitionId?)null
                        : RequiredHandle(table, "description", _localizationHandles, "localization", function);
                    return NewHandle(_ruleHandles, _api.RegisterGroupRule(RequiredString(table, "id", function), description,
                        RequiredRuleArray(table, function),
                        ParseRuleMode(OptionalString(table, "mode", "all", function), function),
                        OptionalIntArray(table, "rounds", function)).Id);
                });
            }

            private DynValue RegisterRandomRule(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.rules.random";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "refresh", "no_doubles", "rules", "mode", "rounds");
                    string refresh = OptionalString(table, "refresh", "each_fight", function);
                    ModRuleRefresh parsed = refresh == "each_round" ? ModRuleRefresh.EachRound
                        : refresh == "each_fight" ? ModRuleRefresh.EachFight
                        : throw new ModContentException(function + " field 'refresh' must be \"each_round\" or \"each_fight\".");
                    return NewHandle(_ruleHandles, _api.RegisterRandomRule(RequiredString(table, "id", function), parsed,
                        OptionalBool(table, "no_doubles", false, function), RequiredRuleArray(table, function),
                        ParseRuleMode(OptionalString(table, "mode", "all", function), function),
                        OptionalIntArray(table, "rounds", function)).Id);
                });
            }

            private DefinitionId[] RequiredRuleArray(Table table, string function)
            {
                DynValue value = table.Get("rules");
                if (value.Type != DataType.Table || value.Table.Length < 1 || value.Table.Length > 64)
                    throw new ModContentException(function + " field 'rules' must be a dense array of 1..64 rule handles.");
                int length = value.Table.Length;
                var result = new DefinitionId[length];
                for (int i = 1; i <= length; i++)
                {
                    DynValue entry = value.Table.Get(i);
                    if (entry.Type != DataType.Table || !_ruleHandles.TryGetValue(entry.Table, out var id))
                        throw new ModContentException(function + " field 'rules' entry " + i + " must be a rule handle.");
                    result[i - 1] = id;
                }
                EnsureDenseTrialArray(value.Table, length, function + " field 'rules'");
                return result;
            }
        }
    }
}
