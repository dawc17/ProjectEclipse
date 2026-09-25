using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Eclipse.Modding
{
    public sealed partial class MoonSharpScriptRuntime
    {
        private sealed partial class MoonSharpScriptContext
        {
            private sealed class StorySequence
            {
                public readonly List<Func<Action<bool>, IDisposable>> Steps = new List<Func<Action<bool>, IDisposable>>();
                public string Position;
                public int Next;
                public long Binding;
                public DynValue Complete, Cancel, Step;
                public IDisposable Lease;
                public bool Pumping, Waiting;
            }
            private void OnSequenceBindingChanged() => StopSequence(false);
            private StorySequence _sequence;
            private int _sequenceCallbackDepth;

            private DynValue PlaySequence(CallbackArguments args)
            {
                const string function = "sf2.story.play_sequence";
                ThrowIfDisposed();
                _api.RequireCapability("ui.create");
                _api.RequireCapability("story.events");
                if (_uiCloseDepth != 0 || _sequenceCallbackDepth >= 8)
                    throw new ModContentException("Sequences cannot start during cleanup or recursively beyond eight completion callbacks.");
                var definition = args.AsType(0, function, DataType.Table, false).Table;
                ValidateFields(definition, function, "steps", "position", "on_complete", "on_cancel", "on_step");
                var sequence = new StorySequence { Binding = _api.State.BindingVersion, Next = 1,
                    Complete = definition.Get("on_complete"), Cancel = definition.Get("on_cancel"), Step = definition.Get("on_step") };
                foreach (var callback in new[] { sequence.Complete, sequence.Cancel, sequence.Step })
                    if (!callback.IsNil() && callback.Type != DataType.Function)
                        throw new ModContentException("Sequence callbacks must be Lua functions.");
                Table steps = RequireArray(definition.Get("steps"), function + ".steps");
                if (steps.Length < 1 || steps.Length > 64) throw new ModContentException("Sequences require 1..64 steps.");
                EnsureDenseArray(steps, steps.Length, function + ".steps");
                for (int i = 1; i <= steps.Length; i++)
                {
                    var value = steps.Get(i);
                    if (value.Type != DataType.Table) throw new ModContentException("Sequence steps must be tables.");
                    var step = value.Table;
                    ValidateFields(step, function, "dialog", "act_screen");
                    bool dialog = !step.Get("dialog").IsNil();
                    if (dialog == !step.Get("act_screen").IsNil()) throw new ModContentException("Each step requires exactly one dialog or act_screen.");
                    var payload = step.Get(dialog ? "dialog" : "act_screen");
                    if (payload.Type != DataType.Table) throw new ModContentException("Sequence step payload must be a table.");
                    if (dialog)
                    {
                        ValidateFields(payload.Table, function, "title", "portrait", "mirrored", "lines", "button", "ignore_back");
                        var request = ReadStoryDialogRequest(payload.Table);
                        sequence.Steps.Add(done => {
                            if (ModStoryDialogAccess.Open == null) throw new ModContentException("Story dialogs are unavailable in this host.");
                            return ModStoryDialogAccess.Open(request, done);
                        });
                    }
                    else
                    {
                        ValidateFields(payload.Table, function, "lines");
                        var lines = ReadActScreenLines(payload.Table);
                        sequence.Steps.Add(done => {
                            if (ModActScreenAccess.Open == null) throw new ModContentException("Act screens are unavailable in this host.");
                            return ModActScreenAccess.Open(lines, done);
                        });
                    }
                }
                if (!definition.Get("position").IsNil())
                {
                    _api.RequireCapability("state.read"); _api.RequireCapability("state.write");
                    sequence.Position = RequiredString(definition, "position", function);
                    if (!_api.TryGetState(sequence.Position, out var saved) || saved.Type != ModParameterType.Integer || saved.Integer < 1 || saved.Integer > steps.Length + 1)
                        throw new ModContentException("Sequence position must name a declared integer state field in 1..steps+1 (default 1).");
                    sequence.Next = (int)saved.Integer;
                }
                if (_sequence != null && _sequence.Binding != _api.State.BindingVersion) StopSequence(false);
                if (_sequence != null || _storyDialog != null || _actScreen != null) return DynValue.False;
                _sequence = sequence;
                try { return DynValue.NewBoolean(PumpSequence(sequence)); }
                catch { if (_sequence == sequence) StopSequence(false); throw; }
            }

            private bool PumpSequence(StorySequence sequence)
            {
                if (sequence.Pumping) return true;
                sequence.Pumping = true;
                try
                {
                    while (_sequence == sequence && !sequence.Waiting)
                    {
                        if (_disposed || sequence.Binding != _api.State.BindingVersion) { StopSequence(false); return false; }
                        if (sequence.Next > sequence.Steps.Count)
                        {
                            _sequence = null;
                            SequenceCallback(sequence.Complete);
                            return true;
                        }
                        var proceed = SequenceCallback(sequence.Step, DynValue.NewNumber(sequence.Next));
                        if (proceed.Type == DataType.Boolean && !proceed.Boolean)
                        {
                            if (_sequence == sequence) StopSequence(true);
                            return false;
                        }
                        if (_sequence != sequence) return false;
                        sequence.Waiting = true;
                        bool ended = false, opening = true;
                        bool? immediate = null;
                        Action<bool> finish = acknowledged => {
                            if (opening) { if (!immediate.HasValue) immediate = acknowledged; return; }
                            if (ended) return;
                            ended = true;
                            if (_sequence != sequence) return;
                            sequence.Lease = null;
                            sequence.Waiting = false;
                            if (_disposed || sequence.Binding != _api.State.BindingVersion) { StopSequence(false); return; }
                            if (!acknowledged) { StopSequence(true); return; }
                            try
                            {
                                int next = sequence.Next + 1;
                                if (sequence.Position != null) _api.SetState(new Dictionary<string, ModParameterValue> {
                                    [sequence.Position] = ModParameterValue.FromInteger(next) });
                                sequence.Next = next;
                                PumpSequence(sequence);
                            }
                            catch (Exception error)
                            {
                                if (_sequence == sequence) StopSequence(false);
                                _api.Log(ModLogLevel.Error, "Sequence callback failed: " + error.Message);
                            }
                        };
                        IDisposable lease = sequence.Steps[sequence.Next - 1](finish);
                        opening = false;
                        if (lease == null) { if (_sequence == sequence) StopSequence(true); return false; }
                        if (immediate.HasValue) finish(immediate.Value);
                        if (ended || _sequence != sequence) lease?.Dispose();
                        else if (lease == null) { StopSequence(true); return false; }
                        else sequence.Lease = lease;
                    }
                    return true;
                }
                finally { sequence.Pumping = false; }
            }

            private void StopSequence(bool notify)
            {
                var sequence = _sequence;
                if (sequence == null) return;
                _sequence = null;
                _uiCloseDepth++;
                try
                {
                    sequence.Lease?.Dispose();
                    if (notify && !_disposed && sequence.Binding == _api.State.BindingVersion)
                        SequenceCallback(sequence.Cancel);
                }
                catch (Exception error) { _api.Log(ModLogLevel.Error, "Sequence cancellation failed: " + error.Message); }
                finally { _uiCloseDepth--; }
            }

            private DynValue SequenceCallback(DynValue callback, params DynValue[] arguments)
            {
                if (callback.IsNil() || _disposed) return DynValue.Nil;
                _sequenceCallbackDepth++;
                try { return RunBounded(callback, Mod.Id + ":story/sequence", MaxBehaviorInstructionSlices, arguments); }
                finally { _sequenceCallbackDepth--; }
            }
        }
    }
}
