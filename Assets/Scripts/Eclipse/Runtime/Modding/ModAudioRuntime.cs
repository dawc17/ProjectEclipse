using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
    public enum ModAudioClock { Game, Real }

    public sealed class ModAudioOptions
    {
        public double Volume { get; }
        public bool Loop { get; }
        public ModAudioClock Clock { get; }
        public ModAudioOptions(double volume = 1, bool loop = false, ModAudioClock clock = ModAudioClock.Game)
        {
            ValidateVolume(volume);
            if (!Enum.IsDefined(typeof(ModAudioClock), clock)) throw new ArgumentException("Unsupported audio clock.");
            Volume = volume; Loop = loop; Clock = clock;
        }
        public static void ValidateVolume(double volume)
        {
            if (double.IsNaN(volume) || double.IsInfinity(volume) || volume < 0 || volume > 1)
                throw new ArgumentException("Audio volume must be finite in 0..1.");
        }
    }

    public interface IModAudioVoice : IDisposable
    {
        // Active includes paused voices; a completed or stopped voice stays inactive.
        bool IsActive { get; }
        void SetVolume(double volume);
    }
    public interface IModAudioBackend
    {
        bool TryPlay(AssetId audio, ModAudioOptions options, out IModAudioVoice voice, out string error);
    }

    // Each script owns its voices. Native/global accounting belongs to the backend.
    public sealed class ModAudioScope : IDisposable
    {
        public const int MaximumVoices = 16;
        private readonly IModAudioBackend backend;
        private readonly HashSet<ModAudioInstance> voices = new HashSet<ModAudioInstance>();
        private bool closed;
        public ModAudioScope(IModAudioBackend backend) { this.backend = backend; }
        public bool TryPlay(AssetId audio, ModAudioOptions options, ModUiSurface owner,
            out ModAudioInstance instance, out string error)
        {
            instance = null; error = null;
            if (closed) { error = "Audio scope is closed."; return false; }
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (owner != null && owner.IsClosed) { error = "Audio UI owner is closed."; return false; }
            foreach (var voice in new List<ModAudioInstance>(voices)) if (!voice.IsActive) voice.Stop();
            if (voices.Count >= MaximumVoices) { error = "This mod already has 16 active audio instances."; return false; }
            if (backend == null) { error = "Audio playback is unavailable in this host."; return false; }
            IModAudioVoice native = null;
            try
            {
                if (!backend.TryPlay(audio, options, out native, out error) || native == null)
                { native?.Dispose(); error = error ?? "Audio playback failed."; return false; }
                instance = new ModAudioInstance(native, owner, voice => voices.Remove(voice));
                voices.Add(instance);
                return true;
            }
            catch (Exception failure)
            {
                native?.Dispose(); error = "Audio playback failed: " + failure.Message; return false;
            }
        }
        public void Dispose()
        {
            if (closed) return;
            closed = true;
            foreach (var voice in new List<ModAudioInstance>(voices)) voice.Stop();
            voices.Clear();
        }
    }

    public sealed class ModAudioInstance
    {
        private IModAudioVoice native;
        private ModUiSurface owner;
        private Action<ModAudioInstance> remove;
        internal ModAudioInstance(IModAudioVoice native, ModUiSurface owner, Action<ModAudioInstance> remove)
        {
            this.native = native; this.owner = owner; this.remove = remove;
            if (owner != null) owner.Closed += OnOwnerClosed;
        }
        public bool IsActive => native != null && native.IsActive;
        public bool SetVolume(double volume)
        {
            ModAudioOptions.ValidateVolume(volume);
            if (!IsActive) { Stop(); return false; }
            native.SetVolume(volume); return true;
        }
        private void OnOwnerClosed() { Stop(); }
        public bool Stop()
        {
            if (native == null) return false;
            bool active = native.IsActive;
            var voice = native; native = null;
            if (owner != null) owner.Closed -= OnOwnerClosed;
            owner = null;
            var release = remove; remove = null; release?.Invoke(this);
            voice.Dispose();
            return active;
        }
    }
}
