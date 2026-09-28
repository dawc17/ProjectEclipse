using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

namespace Eclipse.UI
{
    // Windows release builds refuse to play once their launcher channel publishes a newer
    // version. Only a confirmed newer version blocks: an unversioned build, an unreachable
    // server or a malformed reply leaves the game playable, so installed builds work offline.
    public static class ReleaseCheck
    {
        public enum Result { Pending, Allowed, Outdated }

        // Written into <Product>_Data by EclipsePlayerBuild; PackageUpdate.ps1 checks it too.
        public const string StampFileName = "eclipse-version.txt";
        // The launcher treats this exit code as "update required" instead of a crash to roll back.
        public const int OutdatedExitCode = 3;
        public const string ReleasesPage = Repository + "/releases/latest";
        private const string Repository = "https://github.com/dawc17/ProjectEclipse";
        private const string LauncherFile = "EclipseLauncher.exe";
        private const int TimeoutSeconds = 6;
        private const int ReplyLimit = 4096;
        private static readonly Regex VersionPattern = new Regex(@"^\d{1,6}\.\d{1,6}\.\d{1,6}$");

        private static Result state = Result.Allowed;
        private static UnityWebRequest request;
        private static float deadline;
        private static string skipped;

        public static string Installed { get; private set; }
        public static string Latest { get; private set; }
        public static string Channel { get; private set; }
        // The install root holding the stable launcher bootstrap, or null when none was found.
        public static string LauncherRoot { get; private set; }
        // Started by the launcher's Play button, which waits for the game to exit.
        public static bool FromLauncher { get; private set; }

        public static Result Current
        {
            get
            {
                // Backstop for a request that outlives its own timeout.
                if (state == Result.Pending && Time.realtimeSinceStartup > deadline)
                {
                    Debug.LogWarning("[Release] Update check timed out; continuing offline.");
                    var abandoned = request;
                    request = null;
                    state = Result.Allowed;
                    if (abandoned != null) { abandoned.Abort(); abandoned.Dispose(); }
                }
                return state;
            }
        }

        [Serializable]
        private sealed class LauncherState
        {
            public string rejected;
            public string channel;
        }

        [Serializable]
        private sealed class ChannelVersion
        {
            public int format;
            public string version;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Begin()
        {
            state = Result.Allowed;
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            try { Start(); }
            catch (Exception error)
            {
                state = Result.Allowed;
                Debug.LogWarning("[Release] Update check skipped: " + error.Message);
            }
#endif
        }

        private static void Start()
        {
            string stamp = Path.Combine(Application.dataPath, StampFileName);
            if (!File.Exists(stamp))
            {
                Debug.Log("[Release] Unversioned build; update check disabled.");
                return;
            }
            Installed = File.ReadAllText(stamp).Trim();
            if (!VersionPattern.IsMatch(Installed))
            {
                Debug.LogWarning("[Release] Invalid version stamp; update check disabled.");
                return;
            }
            string launcherRoot = Environment.GetEnvironmentVariable("ECLIPSE_LAUNCHER_ROOT");
            FromLauncher = !string.IsNullOrEmpty(launcherRoot);
            LauncherRoot = FindLauncherRoot(launcherRoot);
            ReadLauncherState();
            // Same channel files as the launcher; the version file is a few bytes, unlike the manifest.
            string url = Repository + "/releases/" + (Channel == "stable" ? "latest/download/" : "download/beta/") +
                Channel + "-version.json";
            request = UnityWebRequest.Get(url);
            request.timeout = TimeoutSeconds;
            deadline = Time.realtimeSinceStartup + TimeoutSeconds + 2;
            state = Result.Pending;
            var sending = request;
            sending.SendWebRequest().completed += operation => Finish(sending);
        }

        private static string FindLauncherRoot(string fromLauncher)
        {
            if (!string.IsNullOrEmpty(fromLauncher)) return Path.GetFullPath(fromLauncher);
            string game = Path.GetDirectoryName(Application.dataPath);
            string parent = Path.GetDirectoryName(game);
            // Launcher installs live in <root>/versions/<version>. Their bundled launcher reads its
            // own folder as the root, so only the bootstrap in <root> may be started.
            if (parent != null && string.Equals(Path.GetFileName(parent), "versions", StringComparison.OrdinalIgnoreCase))
            {
                string root = Path.GetDirectoryName(parent);
                return root != null && File.Exists(Path.Combine(root, LauncherFile)) ? root : null;
            }
            // A loose build shares its folder with the bootstrap.
            return File.Exists(Path.Combine(game, LauncherFile)) ? game : null;
        }

        private static void ReadLauncherState()
        {
            Channel = "stable";
            skipped = null;
            string path = LauncherRoot == null ? null : Path.Combine(LauncherRoot, "launcher-state.json");
            if (path == null || !File.Exists(path)) return;
            try
            {
                var saved = JsonUtility.FromJson<LauncherState>(File.ReadAllText(path));
                if (saved == null) return;
                if (saved.channel == "beta") Channel = "beta";
                // A version the player rolled back from is not offered again, so it must not block.
                if (!string.IsNullOrEmpty(saved.rejected)) skipped = saved.rejected;
            }
            catch (Exception error) { Debug.LogWarning("[Release] Could not read launcher settings: " + error.Message); }
        }

        private static void Finish(UnityWebRequest done)
        {
            // A timed-out request was already abandoned and disposed.
            if (done != request) return;
            request = null;
            try
            {
                if (done.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("[Release] Update check unavailable (" + done.error + "); continuing offline.");
                    state = Result.Allowed;
                    return;
                }
                string text = done.downloadHandler.text;
                var reply = text != null && text.Length <= ReplyLimit ? JsonUtility.FromJson<ChannelVersion>(text) : null;
                if (reply == null || reply.format != 1 || reply.version == null || !VersionPattern.IsMatch(reply.version))
                {
                    Debug.LogWarning("[Release] Ignoring malformed " + Channel + " version file.");
                    state = Result.Allowed;
                    return;
                }
                Latest = reply.version;
                bool outdated = new Version(Latest) > new Version(Installed) && Latest != skipped;
                state = outdated ? Result.Outdated : Result.Allowed;
                Debug.Log("[Release] Installed " + Installed + ", latest " + Channel + " " + Latest +
                    (outdated ? ": update required." : "."));
            }
            catch (Exception error)
            {
                state = Result.Allowed;
                Debug.LogWarning("[Release] Update check failed: " + error.Message);
            }
            finally { done.Dispose(); }
        }

        // Starts the stable launcher bootstrap; the caller quits so the launcher can update.
        public static bool OpenLauncher()
        {
            if (LauncherRoot == null) return false;
            try
            {
                Process.Start(new ProcessStartInfo(Path.Combine(LauncherRoot, LauncherFile))
                {
                    WorkingDirectory = LauncherRoot,
                    UseShellExecute = true
                });
                return true;
            }
            catch (Exception error)
            {
                Debug.LogWarning("[Release] Could not start the launcher: " + error.Message);
                return false;
            }
        }
    }
}
