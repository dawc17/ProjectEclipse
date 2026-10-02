using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Eclipse.Input
{
    public static class FightControllerBindings
    {
        // 0..9 are the existing GamePad button enum; 10 and 11 are analog triggers.
        public static readonly int[] Defaults = { 3, 0, 1, 2, 4 };
        public static readonly string[] Names = { "Punch", "Kick", "Ranged weapon", "Magic", "Raid charge" };
        private static readonly string[] Labels = { "A / Cross", "B / Circle", "Y / Triangle", "X / Square",
            "RB / R1", "LB / L1", "Right stick click", "Left stick click", "Back / Share", "Start / Options", "LT / L2", "RT / R2" };
        private const string Prefix = "Eclipse.Controller.";
        public static int Get(int action)
        {
            int value = FightBindingPreferences.GetInt(Prefix + Names[action], Defaults[action]);
            return value >= 0 && value < Labels.Length ? value : Defaults[action];
        }
        public static string Display(int value) { return Labels[value]; }
        public static bool IsPressed(int value)
        {
            return IsPressed(value, GamePad.Player.One);
        }
        public static bool IsPressed(int value, GamePad.Player player)
        {
            return value < 10 ? GamePad.GetButton((GamePad.Button)value, player)
                : GamePad.GetTrigger((GamePad.Trigger)(value - 10), player, true) > .5f;
        }
        public static bool TrySet(int action, int value, out string message)
        {
            if (action < 0 || action >= Names.Length || value < 0 || value >= Labels.Length)
                throw new ArgumentOutOfRangeException();
            for (int i = 0; i < Names.Length; i++)
                if (i != action && Get(i) == value)
                { message = Display(value) + " is assigned to " + Names[i] + ". Choose another input."; return false; }
            if (!FightBindingPreferences.TrySetInt(Prefix + Names[action], value, out message)) return false;
            message = Names[action] + " saved.";
            return true;
        }
        public static GamePad.Stick MovementStick { get { return FightBindingPreferences.GetInt(Prefix + "RightStick", 0) == 1
            ? GamePad.Stick.RightStick : GamePad.Stick.LeftStick; } }
        public static void ToggleStick()
        {
            if (!FightBindingPreferences.TrySetInt(Prefix + "RightStick", MovementStick == GamePad.Stick.LeftStick ? 1 : 0, out var message))
                Debug.LogWarning(message);
        }
        public static void Reset()
        {
            foreach (var name in Names) FightBindingPreferences.DeleteKey(Prefix + name);
            FightBindingPreferences.DeleteKey(Prefix + "RightStick");
            FightBindingPreferences.Save();
        }
    }

    public static class FightKeyBindings
    {
        public static readonly KeyCode[] Defaults = {
            KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D,
            KeyCode.O, KeyCode.P, KeyCode.K, KeyCode.L, KeyCode.J
        };
        public static readonly string[] Names = {
            "Jump", "Crouch", "Move left", "Move right",
            "Punch", "Kick", "Ranged weapon", "Magic", "Raid charge"
        };
        private const string Prefix = "Eclipse.Key.";

        public static KeyCode Get(KeyCode original)
        {
            var key = (KeyCode)FightBindingPreferences.GetInt(Prefix + original, (int)original);
            return IsAllowed(key) ? key : original;
        }

        // Numpad movement and the recovered debug shortcuts remain reserved.
        public static bool IsAllowed(KeyCode key)
        {
            return (key >= KeyCode.A && key <= KeyCode.Z && key != KeyCode.M && key != KeyCode.B && key != KeyCode.U)
                || key == KeyCode.Space || key == KeyCode.Tab
                || key == KeyCode.LeftShift || key == KeyCode.RightShift
                || key == KeyCode.LeftControl || key == KeyCode.RightControl
                || key == KeyCode.UpArrow || key == KeyCode.DownArrow || key == KeyCode.LeftArrow
                || key == KeyCode.Comma || key == KeyCode.Period || key == KeyCode.Slash
                || key == KeyCode.Semicolon || key == KeyCode.Quote
                || key == KeyCode.LeftBracket || key == KeyCode.RightBracket;
        }

        public static bool TrySet(int action, KeyCode key, out string message)
        {
            if (action < 0 || action >= Defaults.Length) throw new ArgumentOutOfRangeException("action");
            if (!IsAllowed(key)) { message = "That key is reserved. Choose another key, or Esc to cancel."; return false; }
            for (int i = 0; i < Defaults.Length; i++)
                if (i != action && Get(Defaults[i]) == key)
                { message = Display(key) + " is assigned to " + Names[i] + ". Choose another key."; return false; }
            if (!FightBindingPreferences.TrySetInt(Prefix + Defaults[action], (int)key, out message)) return false;
            message = Names[action] + " saved.";
            return true;
        }

        public static void Reset()
        {
            foreach (var key in Defaults) FightBindingPreferences.DeleteKey(Prefix + key);
            FightBindingPreferences.Save();
        }

        public static string Display(KeyCode key)
        {
            return key.ToString().Replace("Left", "Left ").Replace("Right", "Right ").Replace("Arrow", " Arrow").Trim();
        }
    }

    // Bindings are installation settings, independent of the recovered profile
    // and PlayerPrefs reset/reinitialization. Keep a durable copy and import old
    // PlayerPrefs bindings once so existing custom controls are retained.
    internal static class FightBindingPreferences
    {
        [Serializable] private sealed class Entry { public string key; public int value; }
        [Serializable] private sealed class Data { public int version = 1; public Entry[] entries; }
        private static Dictionary<string, int> values;
        private static string FilePath => Path.Combine(Application.persistentDataPath, "controls.json");

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetCache() => values = null;

        private static void Load()
        {
            if (values != null) return;
            values = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var key in FightKeyBindings.Defaults) Import("Eclipse.Key." + key);
            foreach (var name in FightControllerBindings.Names) Import("Eclipse.Controller." + name);
            Import("Eclipse.Controller.RightStick");
            try
            {
                if (!File.Exists(FilePath)) return;
                var data = JsonUtility.FromJson<Data>(File.ReadAllText(FilePath));
                if (data == null || data.version != 1 || data.entries == null) return;
                foreach (var entry in data.entries)
                    if (entry != null && KnownKey(entry.key)) values[entry.key] = entry.value;
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException || error is ArgumentException)
            { Debug.LogWarning("[Controls] Could not read saved bindings: " + error.Message); }
        }

        private static bool KnownKey(string key)
        {
            if (key == "Eclipse.Controller.RightStick") return true;
            foreach (var original in FightKeyBindings.Defaults) if (key == "Eclipse.Key." + original) return true;
            foreach (var name in FightControllerBindings.Names) if (key == "Eclipse.Controller." + name) return true;
            return false;
        }

        private static void Import(string key)
        {
            if (PlayerPrefs.HasKey(key)) values[key] = PlayerPrefs.GetInt(key);
        }

        internal static int GetInt(string key, int fallback)
        {
            Load();
            return values.TryGetValue(key, out int value) ? value : fallback;
        }

        internal static bool TrySetInt(string key, int value, out string message)
        {
            Load();
            bool existed = values.TryGetValue(key, out int previous);
            values[key] = value;
            try { Save(); }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException)
            {
                if (existed) values[key] = previous; else values.Remove(key);
                message = "Could not save controls: " + error.Message;
                return false;
            }
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
            message = string.Empty;
            return true;
        }

        internal static void DeleteKey(string key)
        {
            Load();
            values.Remove(key);
            PlayerPrefs.DeleteKey(key);
        }

        internal static void Save()
        {
            Load();
            var entries = new List<Entry>();
            foreach (var pair in values) entries.Add(new Entry { key = pair.Key, value = pair.Value });
            string path = FilePath, temporary = path + ".tmp";
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            try
            {
                using (var stream = new FileStream(temporary, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(JsonUtility.ToJson(new Data { entries = entries.ToArray() }, true));
                    writer.Flush(); stream.Flush(true);
                }
                if (File.Exists(path)) File.Replace(temporary, path, null); else File.Move(temporary, path);
                PlayerPrefs.Save();
            }
            finally { if (File.Exists(temporary)) File.Delete(temporary); }
        }
    }
}
