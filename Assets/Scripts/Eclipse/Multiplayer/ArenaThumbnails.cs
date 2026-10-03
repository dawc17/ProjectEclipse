using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Eclipse.Multiplayer
{
    /// <summary>
    /// Pictures of arenas for pickers and cards, drawn by the game's own location
    /// renderer (the same view as the menu backdrop), one arena at a time on request,
    /// and cached in memory and as PNG files per game version.
    /// </summary>
    public sealed class ArenaThumbnails : MonoBehaviour
    {
        public const int Width = 384, Height = 216;
        private const float FarX = 120000f;
        private static ArenaThumbnails _instance;

        private readonly Dictionary<string, Texture2D> _cache = new Dictionary<string, Texture2D>();
        private readonly Dictionary<string, List<Action<Texture2D>>> _waiting = new Dictionary<string, List<Action<Texture2D>>>();
        private readonly List<string> _queue = new List<string>();
        private readonly HashSet<string> _failed = new HashSet<string>();
        private UnityEngine.Camera _camera;
        private RenderTexture _render, _upright;

        private static string Folder => Path.Combine(Eclipse.Runtime.EditorPlayModeContext.PersistentDataPath, "ArenaThumbnails", Safe(Application.version), "2");

        /// <summary>
        /// The arena's picture now if it is ready, otherwise null; <paramref name="ready"/> runs
        /// once it has been drawn. Random and unknown arenas never get one.
        /// </summary>
        public static Texture2D Get(string arena, Action<Texture2D> ready)
        {
            if (string.IsNullOrEmpty(arena) || arena == VersusRoster.RandomArena) return null;
            var self = Instance;
            if (self._cache.TryGetValue(arena, out var texture)) return texture;
            if (self._failed.Contains(arena)) return null;
            if (ready != null)
            {
                if (!self._waiting.TryGetValue(arena, out var list)) self._waiting[arena] = list = new List<Action<Texture2D>>();
                list.Add(ready);
            }
            // The newest request goes first: it is what the player is looking at.
            self._queue.Remove(arena);
            self._queue.Insert(0, arena);
            return null;
        }

        private static ArenaThumbnails Instance
        {
            get
            {
                if (_instance != null) return _instance;
                var host = new GameObject("Eclipse Arena Thumbnails");
                DontDestroyOnLoad(host);
                return _instance = host.AddComponent<ArenaThumbnails>();
            }
        }

        private void Update()
        {
            // One arena per frame at most: building a location takes a moment.
            if (_queue.Count == 0) return;
            string arena = _queue[0];
            _queue.RemoveAt(0);
            if (_cache.ContainsKey(arena)) { Deliver(arena, _cache[arena]); return; }
            Texture2D texture = null;
            try { texture = LoadSaved(arena) ?? Draw(arena); }
            catch (Exception exception) { Debug.LogWarning("[Versus] Arena picture for " + arena + " failed: " + exception.Message); }
            if (texture == null) { _failed.Add(arena); _waiting.Remove(arena); return; }
            _cache[arena] = texture;
            Deliver(arena, texture);
        }

        private void Deliver(string arena, Texture2D texture)
        {
            if (!_waiting.TryGetValue(arena, out var list)) return;
            _waiting.Remove(arena);
            foreach (var ready in list)
            {
                try { ready(texture); }
                catch (Exception exception) { Debug.LogException(exception); }
            }
        }

        private static Texture2D LoadSaved(string arena)
        {
            string path = Path.Combine(Folder, Safe(arena) + ".png");
            if (!File.Exists(path)) return null;
            var texture = new Texture2D(2, 2, TextureFormat.RGB24, false) { wrapMode = TextureWrapMode.Clamp, name = "Arena " + arena };
            return texture.LoadImage(File.ReadAllBytes(path)) ? texture : null;
        }

        private Texture2D Draw(string arena)
        {
            if (!VersusRoster.IsArena(arena)) return null;
            EnsureCamera();
            var location = new Location(arena, string.Empty);
            location.init();
            if (location.layers == null || location.layers.Count == 0 || location.gameLayer == null) return null;
            var root = new GameObject("Arena picture scenery (" + arena + ")");
            root.transform.position = new Vector3(FarX, 0, 0);
            var render = new Render(root);
            try
            {
                render.Init(location);
                render.UpdateMenuBackdrop(_camera, false);
                _camera.Render();
                // The menu backdrop is drawn mirrored for the fight camera; turn it upright.
                Graphics.Blit(_render, _upright, new Vector2(1, -1), new Vector2(0, 1));
                var previous = RenderTexture.active;
                RenderTexture.active = _upright;
                var texture = new Texture2D(Width, Height, TextureFormat.RGB24, false) { wrapMode = TextureWrapMode.Clamp, name = "Arena " + arena };
                texture.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
                texture.Apply(false, false);
                RenderTexture.active = previous;
                try
                {
                    Directory.CreateDirectory(Folder);
                    File.WriteAllBytes(Path.Combine(Folder, Safe(arena) + ".png"), texture.EncodeToPNG());
                }
                catch (Exception exception) { Debug.LogWarning("[Versus] Could not save the picture of " + arena + ": " + exception.Message); }
                return texture;
            }
            finally
            {
                try { render.DestroyMenuBackdrop(); } catch (Exception) { }
                Destroy(root);
            }
        }

        private void EnsureCamera()
        {
            if (_camera != null) return;
            var host = new GameObject("Arena picture camera");
            host.transform.SetParent(transform, false);
            host.transform.position = new Vector3(FarX, 0, 0);
            _camera = host.AddComponent<UnityEngine.Camera>();
            _camera.enabled = false;
            _camera.orthographic = true;
            _camera.orthographicSize = 5f;
            _camera.nearClipPlane = .1f;
            _camera.farClipPlane = 2000f;
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.backgroundColor = new Color32(20, 15, 12, 255);
            _camera.aspect = (float)Width / Height;
            _render = new RenderTexture(Width, Height, 16, RenderTextureFormat.ARGB32) { name = "Arena picture", antiAliasing = 2 };
            _upright = new RenderTexture(Width, Height, 0, RenderTextureFormat.ARGB32) { name = "Arena picture upright" };
            _render.Create();
            _upright.Create();
            _camera.targetTexture = _render;
        }

        private static string Safe(string text)
        {
            var builder = new System.Text.StringBuilder(text.Length);
            foreach (char c in text) builder.Append(char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.' ? c : '_');
            return builder.ToString();
        }

        private void OnDestroy()
        {
            if (_render != null) { _render.Release(); Destroy(_render); }
            if (_upright != null) { _upright.Release(); Destroy(_upright); }
        }
    }
}
