using System;
using System.IO;
using System.Linq;
using Eclipse.Modding;
using UnityEditor;
using UnityEngine;

// Runs only in the isolated project made by TestDE128MusicNative.py.
public static class ValidateDE128MusicNative
{
    private const string Prefix = "[DE128MusicNative] ";

    public static void RunEditor()
    {
        try
        {
            string root = Directory.GetParent(Application.dataPath).FullName;
            if (!File.Exists(Path.Combine(root, "de128-music-native-fixture.marker")))
                throw new InvalidOperationException("Music acceptance requires an isolated project copy.");

            string mods = Path.Combine(root, "Mods");
            ModDiscoveryResult discovered = ModDiscovery.DiscoverLoose(mods);
            if (discovered.HasErrors)
                throw new InvalidOperationException("Mod discovery failed: " +
                    string.Join("; ", discovered.Diagnostics));
            ModDescriptor mod = discovered.Mods.Single(value => value.Id.Value == "de128");
            var resolver = new AssetResolver(new IAssetProvider[] { new LooseModProvider(mod) });
            var groups = new[] { (Name: "underworld", Count: 29), (Name: "campaign", Count: 4) };
            foreach (var group in groups)
            {
                string directory = Path.Combine(mod.RootPath, "assets", "audio", group.Name);
                string[] paths = Directory.GetFiles(directory, "*.wav");
                if (paths.Length != group.Count)
                    throw new InvalidOperationException("Expected " + group.Count + " packaged " + group.Name +
                        " music tracks, found " + paths.Length);
                foreach (string path in paths.OrderBy(value => value, StringComparer.Ordinal))
                {
                    string name = Path.GetFileNameWithoutExtension(path);
                    var id = AssetId.Parse("de128:audio/" + group.Name + "/" + name);
                    using (var loader = new ModAssetLoader(resolver))
                    {
                        AudioClip clip = loader.LoadAudio(id);
                        if (clip == null || clip.samples <= 0 || clip.channels < 1 || clip.channels > 2 ||
                            clip.frequency <= 0 || clip.loadState == AudioDataLoadState.Failed)
                            throw new InvalidOperationException("Native audio decode failed: " + id);
                        Debug.Log(Prefix + "decoded " + group.Name + "/" + name + " " + clip.samples + " frames at " +
                            clip.frequency + " Hz / " + clip.channels + " channels");
                    }
                    GC.Collect();
                }
            }
            Debug.Log(Prefix + "PASS: all 33 packaged Underworld and campaign tracks decoded as Unity AudioClips.");
            EditorApplication.Exit(0);
        }
        catch (Exception error)
        {
            Debug.LogError(Prefix + "FAIL: " + error);
            EditorApplication.Exit(1);
        }
    }
}
