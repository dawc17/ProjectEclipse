using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.UI
{
    public enum UiSound { Focus, Confirm, Back, Tab, Toggle, Tick, Begin, Open, Gust }

    // Sound effects and title music for Eclipse-owned menus. Clips are the game's own
    // packaged sounds (Resources/gamedata). This plays on its own sources so menus work
    // before the recovered AudioManager starts, and follows the saved volume settings.
    public sealed class EclipseUiAudio : MonoBehaviour
    {
        // Clip names and base volumes, tuned together. Several names pick one at random.
        private static readonly Dictionary<UiSound, (string[] Clips, float Volume)> Table = new Dictionary<UiSound, (string[], float)>
        {
            { UiSound.Focus, (new[] { "snd_swish1", "snd_swish2", "snd_swish3" }, .18f) },
            { UiSound.Confirm, (new[] { "snd_shopshurikencatch" }, .45f) },
            { UiSound.Back, (new[] { "snd_gust_whoosh_1", "snd_gust_whoosh_2" }, .3f) },
            { UiSound.Tab, (new[] { "snd_swish_sword1", "snd_swish_sword2" }, .22f) },
            { UiSound.Toggle, (new[] { "snd_coin_hit1", "snd_coin_hit3" }, .35f) },
            { UiSound.Tick, (new[] { "snd_coin_hit2" }, .12f) },
            { UiSound.Begin, (new[] { "snd_gong" }, .55f) },
            { UiSound.Open, (new[] { "snd_gust_whoosh_3" }, .25f) },
            { UiSound.Gust, (new[] { "snd_gust_whoosh_1", "snd_gust_whoosh_2", "snd_gust_whoosh_3" }, .12f) },
        };
        private const float MusicFadeSeconds = 1.2f;
        private float musicFade = MusicFadeSeconds;
        // Fight tracks are mastered hot; the title plays this one well under the saved music level.
        private const float TitleMusicVolume = .4f;

        private static EclipseUiAudio instance;
        private static int muteFocusUntilFrame;
        private static float lastFocusAt = -1f;
        private readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
        private AudioSource effects, music;
        private float musicLevel, musicTarget;
        private float titleFightFocus;
        private sealed class FightVoice
        {
            public AudioSource Source;
            public string Name;
            public float Volume;
        }
        private readonly List<FightVoice> fightVoices = new List<FightVoice>();
        private readonly Dictionary<string, AudioClip> fightClips = new Dictionary<string, AudioClip>();

        internal static void SetTitleFightFocus(float focus)
        {
            if (instance == null && focus <= 0f) return;
            var self = Instance;
            self.titleFightFocus = Mathf.Clamp01(focus);
            self.UpdateFightVolumes();
            if (focus <= 0f) StopTitleFightSounds();
        }

        internal static void PlayTitleFightSound(string name, bool loop, float volume, System.Func<string, AudioClip> load)
        {
            // Muted menu fights neither load clips nor queue sounds for later playback.
            if (instance == null || instance.titleFightFocus <= 0f || SoundController.GetSoundVolume() <= 0f
                || string.IsNullOrEmpty(name)) return;
            var self = instance;
            AudioClip clip;
            if (!self.fightClips.TryGetValue(name, out clip))
            {
                clip = load(name);
                self.fightClips.Add(name, clip);
            }
            if (clip == null) return;
            if (loop && self.fightVoices.Exists(v => v.Name == name && v.Source.loop && v.Source.isPlaying)) return;
            var voice = self.fightVoices.Find(v => !v.Source.isPlaying);
            if (voice == null && self.fightVoices.Count < 10)
            {
                voice = new FightVoice { Source = self.gameObject.AddComponent<AudioSource>() };
                voice.Source.playOnAwake = false;
                voice.Source.spatialBlend = 0f;
                self.fightVoices.Add(voice);
            }
            // Prefer replacing a one-shot to interrupting an active loop.
            if (voice == null) voice = self.fightVoices.Find(v => !v.Source.loop);
            if (voice == null) return;
            voice.Source.Stop();
            voice.Name = name;
            voice.Volume = Mathf.Clamp01(volume);
            voice.Source.clip = clip;
            voice.Source.loop = loop;
            voice.Source.volume = voice.Volume * self.titleFightFocus * SoundController.GetSoundVolume();
            voice.Source.Play();
        }

        internal static void StopTitleFightSound(string name)
        {
            if (instance == null) return;
            foreach (var voice in instance.fightVoices)
                if (voice.Name == name) voice.Source.Stop();
        }

        internal static void StopTitleFightSounds(bool clearClips = false)
        {
            if (instance == null) return;
            foreach (var voice in instance.fightVoices)
            {
                voice.Source.Stop();
                voice.Source.clip = null;
                voice.Name = null;
            }
            if (clearClips) instance.fightClips.Clear();
        }

        private void UpdateFightVolumes()
        {
            float level = titleFightFocus * SoundController.GetSoundVolume();
            foreach (var voice in fightVoices) voice.Source.volume = voice.Volume * level;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSession() { instance = null; muteFocusUntilFrame = 0; lastFocusAt = -1f; }

        private static EclipseUiAudio Instance
        {
            get
            {
                if (instance != null) return instance;
                var host = new GameObject("Eclipse UI Audio");
                DontDestroyOnLoad(host);
                instance = host.AddComponent<EclipseUiAudio>();
                instance.effects = host.AddComponent<AudioSource>();
                instance.effects.playOnAwake = false;
                instance.effects.ignoreListenerPause = true;
                instance.music = host.AddComponent<AudioSource>();
                instance.music.playOnAwake = false;
                instance.music.loop = true;
                instance.music.ignoreListenerPause = true;
                return instance;
            }
        }

        // Code-driven selection after a rebuild should not sound like the player moved.
        public static void SuppressFocusSound() { muteFocusUntilFrame = Time.frameCount + 1; }

        public static void Play(UiSound sound)
        {
            if (sound == UiSound.Focus)
            {
                if (Time.frameCount <= muteFocusUntilFrame || Time.unscaledTime - lastFocusAt < .045f) return;
                lastFocusAt = Time.unscaledTime;
            }
            var self = Instance;
            var entry = Table[sound];
            var clip = self.Clip("gamedata/sounds/" + entry.Clips[Random.Range(0, entry.Clips.Length)]);
            if (clip == null) return;
            self.effects.pitch = sound == UiSound.Focus || sound == UiSound.Tick ? Random.Range(.94f, 1.08f) : 1f;
            self.effects.PlayOneShot(clip, entry.Volume * SoundController.GetSoundVolume());
        }

        // Plays a Resources music path; a different track than the one playing starts fresh.
        public static void StartTitleMusic(string path, float fadeSeconds = MusicFadeSeconds)
        {
            var self = Instance;
            var clip = self.Clip(path);
            if (clip == null) return;
            if (self.music.clip != clip)
            {
                self.music.Stop();
                self.music.clip = clip;
            }
            self.musicTarget = 1f;
            self.musicFade = Mathf.Max(.1f, fadeSeconds);
            if (!self.music.isPlaying) { self.musicLevel = 0f; self.music.Play(); }
        }

        public static void StopTitleMusic()
        {
            if (instance == null) return;
            instance.musicTarget = 0f;
            instance.musicFade = MusicFadeSeconds;
        }

        private AudioClip Clip(string path)
        {
            AudioClip clip;
            if (!clips.TryGetValue(path, out clip))
            {
                clip = Resources.Load<AudioClip>(path);
                if (clip == null) Debug.LogWarning("[Eclipse UI] Missing sound " + path);
                clips.Add(path, clip);
            }
            return clip;
        }

        private void Update()
        {
            UpdateFightVolumes();
            if (music == null || !music.isPlaying) return;
            // Capped per frame so a launch hitch cannot jump the fade.
            musicLevel = Mathf.MoveTowards(musicLevel, musicTarget, Mathf.Min(Time.unscaledDeltaTime, .05f) / musicFade);
            // Follows the Audio slider live; the curve keeps fades even at low volumes.
            music.volume = musicLevel * musicLevel * TitleMusicVolume * SoundController.GetMusicVolume();
            if (musicLevel <= 0f && musicTarget <= 0f) music.Stop();
        }
    }
}
