# Audio Lab

Enable this mod, Apply & Restart, then enter Act I Tournament stage 3. Its HUD
plays a generated beacon on an independent audio instance. Try both loop buttons,
pause combat, change the instance volume, and stop it. Game-clock playback pauses
with the native fight; real-clock playback continues. Both follow saved sound
volume/mute and normal listener effects. Stop affects this instance only.

The sound's `owner` is its HUD handle, so closing the HUD on round/fight end also
stops playback. Scene changes and mod shutdown stop all that scope's voices.
Without a UI owner, the voice lives until completion, stop, scene change or mod
shutdown; it does not automatically expire at round end. Close it from your
round/fight callbacks when that is the intended lifetime.

`assets/audio/beacon.wav` is an original one-second mono PCM16 22,050 Hz clip:
a 660 Hz sine with a sine-squared envelope during the first 0.2 seconds, at 18%
amplitude, followed by silence. It contains no recovered or external audio.
Real playback and device audibility are separate acceptance claims; consult the
engineering work log for the tested host and scenarios.

The mod appends one rule to normal/Eclipse stage 3. Its composition does not imply
that every other mod's audio/UI policy will be compatible. See the public audio
reference for handle ownership, limits and failures.
