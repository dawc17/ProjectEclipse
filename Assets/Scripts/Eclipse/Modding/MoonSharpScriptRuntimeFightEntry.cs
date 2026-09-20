using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Eclipse.Modding
{
    public sealed partial class MoonSharpScriptRuntime
    {
        private sealed partial class MoonSharpScriptContext
        {
            private readonly List<IDisposable> _fightEntryBindings = new List<IDisposable>();
            private readonly System.Runtime.CompilerServices.ConditionalWeakTable<Table, ModFightEntryRequest> _fightEntryRequests =
                new System.Runtime.CompilerServices.ConditionalWeakTable<Table, ModFightEntryRequest>();
            private void AddFightEntryFunctions(Table story)
            {
                story.Set("before_fight", DynValue.NewCallback((ctx, args) => ApiCall("sf2.story.before_fight", () => {
                    ThrowIfDisposed(); _api.RequireCapability("story.progression");
                    if (_fightEntries == null) throw new ModContentException("Fight-entry callbacks are unavailable in this host.");
                    var handle = UiArgument(args, 0, DataType.Table, "sf2.story.before_fight").Table;
                    if (!_fightHandles.TryGetValue(handle, out var fight)) throw new ModContentException("Expected an owned fight handle.");
                    var callback = UiArgument(args, 1, DataType.Function, "sf2.story.before_fight");
                    _fightEntryBindings.Add(_fightEntries.Register(Mod.Id, fight, request => {
                        var table = new Table(_script); table.Set("fight", DynValue.NewString(fight.ToString()));
                        _fightEntryRequests.Add(table, request);
                        var result = RunBounded(callback, fight + ":on_before_fight", MaxBehaviorInstructionSlices, new[] { DynValue.NewTable(table) });
                        if (result.IsNil()) return null;
                        if (result.Type != DataType.Boolean) throw new ModContentException("on_before_fight must return true, false or nil.");
                        return (bool?)result.Boolean;
                    }));
                    return DynValue.Nil;
                })));
                story.Set("resume_fight", DynValue.NewCallback((ctx, args) => ApiCall("sf2.story.resume_fight", () => {
                    if (_uiCloseDepth != 0) throw new ModContentException("Fights cannot resume during UI cleanup.");
                    return DynValue.NewBoolean(FightEntryRequest(args, "sf2.story.resume_fight").Resume());
                })));
                story.Set("cancel_fight", DynValue.NewCallback((ctx, args) => ApiCall("sf2.story.cancel_fight", () => {
                    FightEntryRequest(args, "sf2.story.cancel_fight").Cancel(); return DynValue.Nil;
                })));
                story.Set("fight_pending", DynValue.NewCallback((ctx, args) => ApiCall("sf2.story.fight_pending", () =>
                    DynValue.NewBoolean(FightEntryRequest(args, "sf2.story.fight_pending").IsPending))));
            }
            private ModFightEntryRequest FightEntryRequest(CallbackArguments args, string function)
            {
                ThrowIfDisposed(); _api.RequireCapability("story.progression");
                var table = UiArgument(args, 0, DataType.Table, function).Table;
                if (!_fightEntryRequests.TryGetValue(table, out var request)) throw new ModContentException("Expected a fight-entry request from this script context.");
                return request;
            }
        }
    }
}
