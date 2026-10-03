using System;
using MoonSharp.Interpreter;

namespace Eclipse.Modding
{
    public sealed partial class MoonSharpScriptRuntime
    {
        private sealed partial class MoonSharpScriptContext
        {
            private readonly ModAudioScope _audio;
            private bool _audioReady;
            private System.Runtime.CompilerServices.ConditionalWeakTable<Table, ModAudioInstance> _audioInstances =
                new System.Runtime.CompilerServices.ConditionalWeakTable<Table, ModAudioInstance>();

            private void AddAudioModule(Table root)
            {
                var audio = new Table(_script);
                audio.Set("play", DynValue.NewCallback((ctx, args) => ApiCall("sf2.audio.play", () => PlayAudio(args))));
                audio.Set("stop", DynValue.NewCallback((ctx, args) => ApiCall("sf2.audio.stop", () =>
                    DynValue.NewBoolean(AudioInstance(args, "sf2.audio.stop", 1).Stop()))));
                audio.Set("is_playing", DynValue.NewCallback((ctx, args) => ApiCall("sf2.audio.is_playing", () =>
                    DynValue.NewBoolean(AudioInstance(args, "sf2.audio.is_playing", 1).IsActive))));
                audio.Set("set_volume", DynValue.NewCallback((ctx, args) => ApiCall("sf2.audio.set_volume", () => {
                    var instance = AudioInstance(args, "sf2.audio.set_volume", 2);
                    if (args[1].Type != DataType.Number) throw new ModContentException("Audio volume must be a number.");
                    return DynValue.NewBoolean(instance.SetVolume(args[1].Number));
                })));
                root.Set("audio", DynValue.NewTable(audio));
            }
            private ModAudioInstance AudioInstance(CallbackArguments args, string function, int count)
            {
                ThrowIfDisposed(); _api.RequireCapability("audio.play");
                if (args.Count != count || args[0].Type != DataType.Table || !_audioInstances.TryGetValue(args[0].Table, out var instance))
                    throw new ModContentException(function + " requires an audio instance owned by this script and exactly " + count + " arguments.");
                return instance;
            }
            private DynValue PlayAudio(CallbackArguments args)
            {
                const string function = "sf2.audio.play";
                ThrowIfDisposed(); _api.RequireCapability("audio.play");
                if (!_audioReady || (_api.Registration != null && !_api.Registration.IsCatalogFrozen) || _uiCloseDepth != 0)
                    throw new ModContentException("Audio playback requires a runtime callback after all mod registration, outside UI cleanup.");
                if (args.Count < 1 || args.Count > 2 || args[0].Type != DataType.Table || !_audioHandles.TryGetValue(args[0].Table, out var asset))
                    throw new ModContentException(function + " requires an audio asset handle and optional options table.");
                double volume = 1; bool loop = false; var clock = ModAudioClock.Game; ModUiSurface owner = null;
                if (args.Count == 2 && !args[1].IsNil())
                {
                    var options = args.AsType(1, function, DataType.Table, false).Table;
                    ValidateFields(options, function, "volume", "loop", "clock", "owner");
                    if (!options.Get("volume").IsNil())
                    {
                        if (options.Get("volume").Type != DataType.Number) throw new ModContentException("Audio volume must be a number.");
                        volume = options.Get("volume").Number;
                    }
                    if (!options.Get("loop").IsNil())
                    {
                        if (options.Get("loop").Type != DataType.Boolean) throw new ModContentException("Audio loop must be boolean.");
                        loop = options.Get("loop").Boolean;
                    }
                    if (!options.Get("clock").IsNil())
                    {
                        var value = options.Get("clock");
                        if (value.Type != DataType.String || (value.String != "game" && value.String != "real"))
                            throw new ModContentException("Audio clock must be game or real.");
                        clock = value.String == "real" ? ModAudioClock.Real : ModAudioClock.Game;
                    }
                    var ui = options.Get("owner");
                    if (!ui.IsNil() && (ui.Type != DataType.Table || !_uiHandles.TryGetValue(ui.Table, out owner)))
                        throw new ModContentException("Audio owner must be a UI handle owned by this script.");
                }
                if (!_audio.TryPlay(asset, new ModAudioOptions(volume, loop, clock), owner, out var instance, out var error))
                    return DynValue.NewTuple(DynValue.Nil, DynValue.NewString(error));
                var handle = new Table(_script); _audioInstances.Add(handle, instance);
                return DynValue.NewTuple(DynValue.NewTable(handle), DynValue.Nil);
            }
        }
    }
}
