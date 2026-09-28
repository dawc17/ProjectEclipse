using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Eclipse.UI
{
    // On-screen fight control texture packs.
    //
    // A pack is a folder of PNG files named after FightButtons atlas members, with or
    // without the atlas prefix: "Joystick_norm.png" or "FightButtons.Joystick_norm.png"
    // replaces FightButtons.Joystick_norm. Members without a PNG keep the built-in art.
    // Pack folders are looked up, first match by name wins, in:
    //   <Application.persistentDataPath>/ControlPacks/<name>/
    //   <game folder next to the data folder>/ControlPacks/<name>/
    //   <Application.streamingAssetsPath>/ControlPacks/<name>/
    // No packs ship; "Default" is the built-in art. Sprites are created at runtime with
    // the original sprite's pivot, border and on-screen size (pixels-per-unit is scaled by
    // the PNG width over the original width), so higher resolution art keeps the layout.
    // The selection is stored in PlayerPrefs "Eclipse.ControlPack" and applies to controls
    // created after the change (the next fight or dojo scene load).
    public static class ControlTexturePacks
    {
        public const string DefaultName = "Default";
        public const string AtlasPath = "UI/Atlases/FightButtons";
        private const string PrefKey = "Eclipse.ControlPack";
        private const string MemberPrefix = "FightButtons.";

        private static readonly List<string> names = new List<string>();
        private static readonly Dictionary<string, string> folders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>(StringComparer.Ordinal);
        private static bool scanned;
        private static string current;

        public static IList<string> Available
        {
            get { EnsureScanned(); return names.AsReadOnly(); }
        }

        // Selected pack name; "Default" when none is chosen or the saved pack is missing.
        public static string Current
        {
            get
            {
                EnsureScanned();
                if (current == null)
                {
                    string saved = PlayerPrefs.GetString(PrefKey, DefaultName);
                    current = folders.ContainsKey(saved) ? CanonicalName(saved) : DefaultName;
                }
                return current;
            }
            set
            {
                EnsureScanned();
                string next = !string.IsNullOrEmpty(value) && folders.ContainsKey(value) ? CanonicalName(value) : DefaultName;
                if (next == Current) return;
                current = next;
                PlayerPrefs.SetString(PrefKey, next);
                PlayerPrefs.Save();
            }
        }

        public static string Label
        {
            get { return Current == DefaultName ? "DEFAULT" : Current.ToUpperInvariant(); }
        }

        public static void Cycle()
        {
            Refresh();
            int index = names.IndexOf(Current);
            Current = names[(index + 1) % names.Count];
        }

        // Rescans the pack folders (call before showing the option). Drops a saved
        // pack whose folder was removed back to Default without forgetting the pref.
        public static void Refresh()
        {
            scanned = false;
            current = null;
            EnsureScanned();
        }

        // Called from AtlasCache after mod sprite replacement. Returns false for any
        // other atlas, for the Default pack and for members the pack does not provide.
        public static bool TryGetSprite(string atlas, string member, Func<string, string, Sprite> original, out Sprite sprite)
        {
            sprite = null;
            if (string.IsNullOrEmpty(member) || !string.Equals(Normalize(atlas), AtlasPath, StringComparison.OrdinalIgnoreCase))
                return false;
            string pack = Current;
            if (pack == DefaultName) return false;
            string key = pack + "|" + member;
            if (sprites.TryGetValue(key, out sprite)) return sprite != null;
            sprite = Load(folders[pack], atlas, member, original);
            sprites[key] = sprite;
            return sprite != null;
        }

        private static Sprite Load(string folder, string atlas, string member, Func<string, string, Sprite> original)
        {
            string shortName = member.StartsWith(MemberPrefix, StringComparison.OrdinalIgnoreCase) ? member.Substring(MemberPrefix.Length) : member;
            string file = Path.Combine(folder, shortName + ".png");
            if (!File.Exists(file)) file = Path.Combine(folder, MemberPrefix + shortName + ".png");
            if (!File.Exists(file)) return null;
            Sprite source = original != null ? original(atlas, member) : null;
            Texture2D texture = null;
            try
            {
                texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (!ImageConversion.LoadImage(texture, File.ReadAllBytes(file), false))
                {
                    UnityEngine.Object.Destroy(texture);
                    Debug.LogWarning("[ControlPacks] Could not decode " + file);
                    return null;
                }
                texture.name = member;
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.filterMode = FilterMode.Bilinear;
                Vector2 pivot = new Vector2(.5f, .5f);
                float pixelsPerUnit = 100f;
                Vector4 border = Vector4.zero;
                if (source != null && source.rect.width > 0f && source.rect.height > 0f)
                {
                    float scale = texture.width / source.rect.width;
                    pivot = new Vector2(source.pivot.x / source.rect.width, source.pivot.y / source.rect.height);
                    pixelsPerUnit = source.pixelsPerUnit * scale;
                    border = source.border * scale;
                }
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot, pixelsPerUnit,
                    0u, SpriteMeshType.FullRect, border);
                sprite.name = member;
                return sprite;
            }
            catch (Exception exception)
            {
                if (texture != null) UnityEngine.Object.Destroy(texture);
                Debug.LogWarning("[ControlPacks] Could not load " + file + ": " + exception.Message);
                return null;
            }
        }

        private static void EnsureScanned()
        {
            if (scanned) return;
            scanned = true;
            names.Clear();
            folders.Clear();
            names.Add(DefaultName);
            foreach (string root in Roots())
            {
                string[] directories;
                try
                {
                    if (!Directory.Exists(root)) continue;
                    directories = Directory.GetDirectories(root);
                }
                catch (Exception) { continue; }
                Array.Sort(directories, StringComparer.OrdinalIgnoreCase);
                foreach (string directory in directories)
                {
                    string name = Path.GetFileName(directory);
                    if (string.IsNullOrEmpty(name) || folders.ContainsKey(name) ||
                        string.Equals(name, DefaultName, StringComparison.OrdinalIgnoreCase)) continue;
                    bool hasPng;
                    try { hasPng = Directory.GetFiles(directory, "*.png").Length > 0; }
                    catch (Exception) { hasPng = false; }
                    if (!hasPng) continue;
                    folders[name] = directory;
                    names.Add(name);
                }
            }
            // Default resolves through the regular atlas path.
            folders[DefaultName] = null;
        }

        private static IEnumerable<string> Roots()
        {
            yield return Path.Combine(Application.persistentDataPath, "ControlPacks");
            string gameFolder = null;
            try { gameFolder = Path.GetDirectoryName(Application.dataPath); } catch (Exception) { }
            if (!string.IsNullOrEmpty(gameFolder)) yield return Path.Combine(gameFolder, "ControlPacks");
            yield return Path.Combine(Application.streamingAssetsPath, "ControlPacks");
        }

        private static string CanonicalName(string name)
        {
            foreach (string known in names)
                if (string.Equals(known, name, StringComparison.OrdinalIgnoreCase)) return known;
            return DefaultName;
        }

        private static string Normalize(string atlas)
        {
            return string.IsNullOrEmpty(atlas) ? string.Empty : atlas.Replace('\\', '/').Trim('/');
        }

        // Keep cached sprites alive while existing controls reference them. Refreshing
        // option labels must not discard ownership and leak a new copy on every visit.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSession()
        {
            foreach (Sprite sprite in sprites.Values)
            {
                if (sprite == null) continue;
                if (sprite.texture != null) UnityEngine.Object.Destroy(sprite.texture);
                UnityEngine.Object.Destroy(sprite);
            }
            sprites.Clear();
            scanned = false;
            current = null;
        }
    }
}
