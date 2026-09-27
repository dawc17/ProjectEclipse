using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Modding
{
    // Plays sf2.fx.screen sounds. A native name loads from the packaged game sounds
    // (after any mod replacement of that sound); a qualified reference loads the mod's
    // own audio. The source bypasses listener effects, so an effect's sound stays
    // clear while its own muffle filters the rest of the fight. It follows the saved
    // sound volume and mute, and runs on real time, unaffected by slow motion.
    internal sealed class ModFxSoundPlayer : MonoBehaviour
    {
        private const string NativeSoundPath = "gamedata/sounds/";
        private const string CorePrefix = "core:";

        private static ModFxSoundPlayer instance;
        private readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
        private readonly HashSet<string> missing = new HashSet<string>();
        private AudioSource source;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Install()
        {
            instance = null;
            ModVisuals.PlayEffectSound = Play;
            SceneManager.sceneLoaded -= Preload;
            SceneManager.sceneLoaded += Preload;
        }

        // Loads and decodes every effect sound as a fight starts, so a sound plays on
        // the very frame its hit lands instead of waiting on a first load.
        private static void Preload(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "Fight" && scene.name != "Dojo") return;
            List<string> references = ModVisuals.EffectSoundReferences();
            if (references.Count == 0) return;
            ModFxSoundPlayer player = Instance;
            foreach (string reference in references)
            {
                AudioClip clip = player.Clip(reference);
                if (clip != null && clip.loadState == AudioDataLoadState.Unloaded) clip.LoadAudioData();
            }
        }

        private static ModFxSoundPlayer Instance
        {
            get
            {
                if (instance != null) return instance;
                var host = new GameObject("Eclipse effect sounds");
                DontDestroyOnLoad(host);
                instance = host.AddComponent<ModFxSoundPlayer>();
                instance.source = host.AddComponent<AudioSource>();
                instance.source.playOnAwake = false;
                instance.source.bypassListenerEffects = true;
                return instance;
            }
        }

        private static void Play(string sound, float volume)
        {
            float level = volume * SoundController.GetSoundVolume();
            if (level <= 0f) return;
            ModFxSoundPlayer player = Instance;
            AudioClip clip = player.Clip(sound);
            if (clip != null) player.source.PlayOneShot(clip, level);
        }

        private AudioClip Clip(string sound)
        {
            AudioClip clip;
            if (clips.TryGetValue(sound, out clip) && clip != null) return clip;
            // A core handle ("core:gamedata/sounds/...") names its Resources path directly.
            if (sound.StartsWith(CorePrefix, System.StringComparison.Ordinal))
                clip = ResourcesAndBundles.Load<AudioClip>(sound.Substring(CorePrefix.Length));
            else if (!ModAssetBinding.TryLoadAudio(sound, out clip) && !ModAssetBinding.IsQualified(sound))
                clip = ResourcesAndBundles.Load<AudioClip>(NativeSoundPath + sound);
            if (clip == null)
            {
                if (missing.Add(sound)) Debug.LogWarning("[Eclipse] Missing screen effect sound '" + sound + "'; skipping it.");
                return null;
            }
            clips[sound] = clip;
            return clip;
        }
    }
}
