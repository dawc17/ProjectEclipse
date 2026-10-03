using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eclipse.Multiplayer.Balance;
using UnityEngine;

namespace Eclipse.Multiplayer
{
    /// <summary>Bundled and local JSON presets; active fights retain their own immutable rules.</summary>
    public static class PvpBalanceProfiles
    {
        public const string ResourcePath = "EclipseVersus/Balance";
        private static readonly Dictionary<string, PvpBalanceSnapshot> history = new Dictionary<string, PvpBalanceSnapshot>(StringComparer.Ordinal);
        private static PvpBalanceSnapshot[] profiles;
        private static string selectedId;
        public static string LocalDirectory => Path.Combine(Eclipse.Runtime.EditorPlayModeContext.PersistentDataPath, "PvpBalance");
        private static string SelectionPath => Path.Combine(LocalDirectory, "selected.txt");
        public static IReadOnlyList<PvpBalanceSnapshot> Available { get { Ensure(); return profiles; } }
        public static PvpBalanceSnapshot Selected { get { Ensure(); return profiles.FirstOrDefault(x => x.Id == selectedId) ?? profiles.First(x => x.Id == "default"); } }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset() { profiles = null; selectedId = null; history.Clear(); }
        private static void Ensure() { if (profiles == null) Refresh(); }

        public static void Refresh()
        {
            var found = new Dictionary<string, PvpBalanceSnapshot>(StringComparer.Ordinal);
            var fallback = new PvpBalanceProfile().Compile();
            found[fallback.Id] = fallback;
            foreach (var asset in Resources.LoadAll<TextAsset>(ResourcePath).OrderBy(x => x.name, StringComparer.Ordinal))
                Load(found, asset.text, "Resources/" + ResourcePath + "/" + asset.name);
            if (Directory.Exists(LocalDirectory))
                foreach (var path in Directory.GetFiles(LocalDirectory, "*.json").OrderBy(x => x, StringComparer.Ordinal).Take(128))
                {
                    try
                    {
                        if (new FileInfo(path).Length > PvpBalanceProfile.MaxJsonBytes) throw new ArgumentException("File exceeds 32 KiB.");
                        Load(found, File.ReadAllText(path), path);
                    }
                    catch (Exception error) { Debug.LogWarning("[PvP Balance] " + path + ": " + error.Message); }
                }
            profiles = found.Values.OrderBy(x => x.Id == "default" ? "" : x.Name, StringComparer.Ordinal).ToArray();
            foreach (var snapshot in profiles) history[snapshot.Hash] = snapshot;
            if (selectedId == null)
            {
                selectedId = "default";
                try { if (File.Exists(SelectionPath)) selectedId = File.ReadAllText(SelectionPath).Trim(); }
                catch (IOException error) { Debug.LogWarning("[PvP Balance] Could not read selection: " + error.Message); }
            }
        }

        private static void Load(Dictionary<string, PvpBalanceSnapshot> found, string json, string source)
        {
            try
            {
                var snapshot = PvpBalanceProfile.Parse(json).Compile();
                found[snapshot.Id] = snapshot;
            }
            catch (Exception error) { Debug.LogWarning("[PvP Balance] Ignoring invalid profile " + source + ": " + error.Message); }
        }

        public static bool TryFind(string hash, out PvpBalanceSnapshot snapshot)
        {
            Ensure();
            if (hash != null && history.TryGetValue(hash, out snapshot)) return true;
            Refresh();
            snapshot = null;
            return hash != null && history.TryGetValue(hash, out snapshot);
        }

        public static void Select(string id)
        {
            Ensure();
            var snapshot = profiles.FirstOrDefault(x => x.Id == id) ?? throw new ArgumentException("Unknown PvP balance profile: " + id);
            selectedId = snapshot.Id;
            Directory.CreateDirectory(LocalDirectory);
            File.WriteAllText(SelectionPath, selectedId);
            OnlineVersusSession.Current?.SetBalance(snapshot);
        }

        public static void Cycle()
        {
            Ensure();
            int current = Array.FindIndex(profiles, x => x.Id == Selected.Id);
            Select(profiles[(current + 1) % profiles.Length].Id);
        }
    }
}
