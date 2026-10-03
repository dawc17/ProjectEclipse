---
title: Audio instances
description: Play, query, update and stop sounds owned by your mod.
---

An audio **asset handle** identifies a clip. An audio **instance handle** identifies
one playback of that clip. Resolve the asset during registration with
[`sf2.assets.audio`](../assets/#sf2assetsaudio), then play it from a runtime callback.
Each instance uses an independent native audio source; stopping one never stops
another mod's voice. Declare `audio.play` for all four operations below.

## sf2.audio.play

Start one sound instance owned by the current script.

**Signature:** `sf2.audio.play(audio, options?)`

**Returns:** `instance, nil` on acceptance; `nil, error` when the host, clip, UI
owner or voice budget is unavailable. Malformed arguments, missing capability and
invalid timing raise a Lua error. Acceptance is not proof of device audibility.

**When:** After all mod registration finishes, from a combat, story, UI or other
runtime callback. Playback during registration or a UI cleanup callback is rejected,
including a loaded provider's framework handler called by a still-loading mod.

**Requires:** `audio.play`; an audio asset handle resolved by this script.
Cross-mod assets also require a declared dependency.

```lua
-- beacon was resolved during loading with sf2.assets.audio("audio/beacon").
-- hud is this script's open UI handle. Run this from a runtime callback.
local sound, error = sf2.audio.play(beacon, {
    volume = 0.5, loop = true, clock = "game", owner = hud,
})
if error then sf2.log.warn(error) end
```

| Option | Default | Contract |
| --- | --- | --- |
| `volume` | `1` | Finite number in `0..1`, multiplied by the saved game sound volume/mute. |
| `loop` | `false` | Boolean. Loops the clip until it is stopped or its lifetime ends. |
| `clock` | `"game"` | `"game"` pauses/resumes with native combat pause. `"real"` continues through combat/listener pause. |
| `owner` | omitted | An open UI handle owned by this script. Closing it stops this instance, including close caused by callback failure. |

Omit `options`, pass `nil`, or supply an options table. Unknown fields are errors.
Exactly one asset argument and at most one options argument are accepted; raw
filenames, strings and invented tables are not audio asset handles.

Both clocks play at the clip's normal rate in real time. `"game"` means **pause
following**, not sample-accurate simulation timing: slow motion, simulation steps
and `Time.timeScale` do not change pitch or playback rate. It also respects native
listener pause. `"real"` ignores listener pause, but still uses saved sound
volume/mute and normal listener effects. Sources are non-spatial, with no public
pitch, pan, position, music-channel, streaming or seek controls.

Each mod may own at most **16 active instances**; the shared native backend may
have at most **64**. Paused and muted instances count. Reaching either bound fails
the new request without evicting existing voices. Completed/stopped voices free
their slots. These bounds apply to this API's voices, independently of native
music, move sounds and screen-effect audio.

Instances end on natural completion, `stop`, active-scene change or script/mod
shutdown. A UI owner provides additional automatic cleanup. Without an owner,
round/fight end alone does not stop a sound: stop it in your lifecycle callbacks
when that is the desired lifetime. Never serialize instance handles; they cannot
resume from a save and cannot be shared through primitive-only framework services.

## sf2.audio.is_playing

Query whether an owned instance remains active, including while paused or muted.

**Signature:** `sf2.audio.is_playing(instance)`

**Returns:** Boolean. `false` after completion, stop or lifetime cleanup. This is
an instance-liveness query, not a signal-level or audibility measurement.

**When:** Runtime callbacks after obtaining the instance; querying a stopped
handle is safe while its script context remains alive.

**Requires:** `audio.play`; an instance returned to this script by `sf2.audio.play`.

```lua
if sound and not sf2.audio.is_playing(sound) then sound = nil end
```

Exactly one argument is accepted. Fake or other-context handles raise an error.

## sf2.audio.set_volume

Change one owned instance's volume multiplier.

**Signature:** `sf2.audio.set_volume(instance, volume)`

**Returns:** `true` if the active instance was updated; `false` if it has already
ended. Invalid volume or handles raise an error.

**When:** Runtime callbacks, including while playback is paused or muted.

**Requires:** `audio.play`; this script's instance and a finite numeric volume in
`0..1`. Exactly two arguments are required.

```lua
if sound then sf2.audio.set_volume(sound, 0.25) end
```

This multiplies the saved sound volume; it does not change the user's settings or
another instance. Saved volume/mute changes also affect existing voices.

## sf2.audio.stop

Stop and release one owned sound instance.

**Signature:** `sf2.audio.stop(instance)`

**Returns:** `true` if an active instance was stopped; `false` if it already ended.
Repeated stop is safe. Invalid handles or extra arguments raise an error.

**When:** Runtime and lifecycle callbacks while the owning script is alive.

**Requires:** `audio.play`; exactly one instance owned by this script.

```lua
on_round_end = function()
    if sound then sf2.audio.stop(sound); sound = nil end
end
```

The [Audio Lab example](https://github.com/dawc17/ProjectEclipse/tree/main/Mods/example.audio-lab)
supplies an original WAV, native HUD controls, both clocks, volume/stop and
round/fight cleanup. It appends a rule to Act I Tournament stage 3. Native source
behavior and physical-device listening are separate verification scopes.
