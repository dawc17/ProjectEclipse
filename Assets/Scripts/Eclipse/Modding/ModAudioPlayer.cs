using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Modding
{
    internal sealed class ModAudioBackend : IModAudioBackend
    {
        public bool TryPlay(AssetId audio, ModAudioOptions options, out IModAudioVoice voice, out string error) =>
            ModAudioPlayer.Instance.TryPlay(audio, options, out voice, out error);
    }

    // Independent sources allow one mod to stop/update its own voice. They use the
    // saved sound volume, normal listener effects and a scene-bound lifetime.
    internal sealed class ModAudioPlayer : MonoBehaviour
    {
        internal const int MaximumVoices = 64;
        private static ModAudioPlayer instance;
        private readonly HashSet<Voice> voices = new HashSet<Voice>();
        internal static ModAudioPlayer Instance
        {
            get
            {
                if (instance != null) return instance;
                var host = new GameObject("Eclipse mod audio");
                DontDestroyOnLoad(host);
                return instance = host.AddComponent<ModAudioPlayer>();
            }
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset() { instance = null; }
        private void OnEnable() { SceneManager.activeSceneChanged += OnSceneChanged; }
        private void OnDisable() { SceneManager.activeSceneChanged -= OnSceneChanged; StopAll(); }
        private void OnSceneChanged(Scene previous, Scene current) { StopAll(); }
        private void StopAll() { foreach (var voice in new List<Voice>(voices)) voice.Dispose(); }

        internal bool TryPlay(AssetId audio, ModAudioOptions options, out IModAudioVoice result, out string error)
        {
            result = null; error = null;
            foreach (var old in new List<Voice>(voices)) old.Refresh();
            if (voices.Count >= MaximumVoices) { error = "The session already has 64 active audio instances."; return false; }
            AudioClip clip = ModRuntime.Host?.TypedAssets.LoadAudio(audio);
            if (clip == null || clip.samples <= 0) { error = "Audio clip is unavailable: " + audio; return false; }
            var source = gameObject.AddComponent<AudioSource>();
            try
            {
                source.playOnAwake = false; source.clip = clip; source.loop = options.Loop;
                source.spatialBlend = 0; source.dopplerLevel = 0;
                source.ignoreListenerPause = options.Clock == ModAudioClock.Real;
                var voice = new Voice(this, source, options); voices.Add(voice);
                source.volume = (float)options.Volume * SoundController.GetSoundVolume();
                source.Play(); voice.Refresh(); result = voice; return true;
            }
            catch { Destroy(source); throw; }
        }
        private void Update() { foreach (var voice in new List<Voice>(voices)) voice.Refresh(); }

        private sealed class Voice : IModAudioVoice
        {
            private readonly ModAudioPlayer player;
            private AudioSource source;
            private readonly ModAudioClock clock;
            private bool paused;
            private readonly int startedFrame;
            private double volume;
            public Voice(ModAudioPlayer player, AudioSource source, ModAudioOptions options)
            { this.player = player; this.source = source; clock = options.Clock; volume = options.Volume; startedFrame = Time.frameCount; }
            public bool IsActive { get { Refresh(); return source != null; } }
            public void SetVolume(double value) { ModAudioOptions.ValidateVolume(value); volume = value; Refresh(); }
            public void Refresh()
            {
                if (source == null) { player.voices.Remove(this); return; }
                // External listener pause also reports isPlaying=false, without
                // completing the source. Keep it until the listener resumes.
                bool listenerPaused = AudioListener.pause && !source.ignoreListenerPause;
                if (!paused && !listenerPaused && !source.isPlaying && Time.frameCount > startedFrame &&
                    source.clip.loadState != AudioDataLoadState.Loading) { Dispose(); return; }
                var fight = Fight.GetCurrentFight();
                bool shouldPause = clock == ModAudioClock.Game && fight != null && fight.IsPaused();
                if (shouldPause && !paused) { source.Pause(); paused = true; }
                else if (!shouldPause && paused) { source.UnPause(); paused = false; }
                source.volume = (float)volume * SoundController.GetSoundVolume();
            }
            public void Dispose()
            {
                player.voices.Remove(this);
                if (source == null) return;
                var old = source; source = null; player.voices.Remove(this);
                old.Stop(); old.clip = null; Destroy(old);
            }
        }
    }
}
