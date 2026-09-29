using System;
using Eclipse.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Multiplayer
{
    /// <summary>
    /// The living scene behind the multiplayer menus: an arena drawn by the game's own
    /// location renderer (animated layers included) through a private camera into a
    /// texture, drifting and breathing slowly under an ink vignette, with leaves or
    /// petals when the arena suits them. Changing arena cross-fades.
    /// <para>
    /// The scenery is built far from anything a scene places (see <see cref="FarX"/>), so
    /// whatever scene is loaded under the lobby never shows up in it.
    /// </para>
    /// </summary>
    public sealed class VersusBackdrop : MonoBehaviour
    {
        private const float FarX = 60000f;
        private const float FadeSeconds = .7f;
        private static readonly Color Ink = new Color32(20, 15, 12, 255);

        private RectTransform _rect;
        private RawImage _image, _fading;
        private RawImage _vignette;
        private RectTransform _particles;
        private UnityEngine.Camera _camera;
        private RenderTexture _texture, _fadeTexture;
        private GameObject _root;
        private Render _render;
        private string _arena;
        private float _fade;
        private float _dim = .72f;
        private Vector2 _parallax;

        public string Arena => _arena;

        public static VersusBackdrop Create(Transform parent)
        {
            var rect = new GameObject("Versus Backdrop", typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero;
            var backdrop = rect.gameObject.AddComponent<VersusBackdrop>();
            backdrop.Build(rect);
            rect.gameObject.SetActive(false);
            return backdrop;
        }

        private void Build(RectTransform rect)
        {
            _rect = rect;
            var ink = rect.gameObject.AddComponent<Image>();
            ink.color = Ink;
            ink.raycastTarget = false;
            _image = Picture("Arena");
            _fading = Picture("Previous arena");
            _fading.enabled = false;
            _particles = new GameObject("Leaves", typeof(RectTransform)).GetComponent<RectTransform>();
            _particles.SetParent(rect, false);
            _particles.anchorMin = _particles.anchorMax = _particles.pivot = new Vector2(0, 1);
            _particles.sizeDelta = new Vector2(1280, 720);
            _vignette = Picture("Vignette");
            _vignette.texture = VignetteTexture();
            _vignette.color = Color.white;
        }

        private RawImage Picture(string name)
        {
            var child = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            child.SetParent(_rect, false);
            child.anchorMin = Vector2.zero; child.anchorMax = Vector2.one; child.offsetMin = child.offsetMax = Vector2.zero;
            var image = child.gameObject.AddComponent<RawImage>();
            image.raycastTarget = false;
            // The menu backdrop is drawn mirrored for the fight camera; show it upright.
            if (name != "Vignette") image.uvRect = new Rect(0, 1, 1, -1);
            return image;
        }

        /// <summary>How bright the arena shows through (1 = as in a fight).</summary>
        public float Dim { get => _dim; set => _dim = Mathf.Clamp01(value); }

        /// <summary>Shows the backdrop with <paramref name="arena"/> (a roster id); unknown ids show the dojo.</summary>
        public void Show(string arena)
        {
            gameObject.SetActive(true);
            if (string.IsNullOrEmpty(arena) || arena == VersusRoster.RandomArena) arena = _arena ?? "dojo";
            if (arena == _arena && _render != null) { EnsureCamera(); return; }
            EnsureCamera();
            if (_render != null)
            {
                // Keep the old arena on screen while the new one builds, then fade it away.
                EnsureTexture(ref _fadeTexture);
                Graphics.Blit(_texture, _fadeTexture);
                _fading.texture = _fadeTexture;
                _fading.enabled = true;
                _fade = 1f;
            }
            DestroyScenery();
            _arena = arena;
            try
            {
                var location = new Location(arena, string.Empty);
                location.init();
                if (location.layers == null || location.layers.Count == 0 || location.gameLayer == null) throw new InvalidOperationException("no layers");
                _root = new GameObject("Versus backdrop scenery (" + arena + ")");
                _root.transform.position = new Vector3(FarX, 0, 0);
                _render = new Render(_root);
                _render.Init(location);
                _render.UpdateMenuBackdrop(_camera, false);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("[Versus] Backdrop for " + arena + " could not be drawn: " + exception.Message);
                DestroyScenery();
            }
            ScatterParticles(arena);
        }

        public void Hide()
        {
            DestroyScenery();
            _arena = null;
            if (_camera != null) Destroy(_camera.gameObject);
            _camera = null;
            Release(ref _texture);
            Release(ref _fadeTexture);
            _fading.enabled = false;
            gameObject.SetActive(false);
        }

        private void EnsureCamera()
        {
            if (_camera == null)
            {
                var host = new GameObject("Versus Backdrop Camera");
                DontDestroyOnLoad(host);
                host.transform.position = new Vector3(FarX, 0, 0);
                _camera = host.AddComponent<UnityEngine.Camera>();
                _camera.orthographic = true;
                _camera.orthographicSize = 5f;
                _camera.nearClipPlane = .1f;
                _camera.farClipPlane = 2000f;
                _camera.clearFlags = CameraClearFlags.SolidColor;
                _camera.backgroundColor = Ink;
                _camera.depth = -100;
                _camera.allowHDR = false;
                _camera.allowMSAA = false;
            }
            EnsureTexture(ref _texture);
            _camera.targetTexture = _texture;
            _camera.aspect = (float)_texture.width / _texture.height;
            _image.texture = _texture;
        }

        private static void EnsureTexture(ref RenderTexture texture)
        {
            int width = Mathf.Max(64, Screen.width), height = Mathf.Max(64, Screen.height);
            if (texture != null && texture.width == width && texture.height == height) return;
            Release(ref texture);
            texture = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32) { name = "Versus backdrop", useMipMap = false };
            texture.Create();
        }

        private static void Release(ref RenderTexture texture)
        {
            if (texture == null) return;
            texture.Release();
            Destroy(texture);
            texture = null;
        }

        private void DestroyScenery()
        {
            try { _render?.DestroyMenuBackdrop(); }
            catch (Exception exception) { Debug.LogWarning("[Versus] Backdrop cleanup: " + exception.Message); }
            _render = null;
            if (_root != null) Destroy(_root);
            _root = null;
            for (int i = _particles.childCount - 1; i >= 0; i--) Destroy(_particles.GetChild(i).gameObject);
        }

        private void ScatterParticles(string arena)
        {
            string id = arena.ToLowerInvariant();
            bool petals = id.Contains("sakura") || id.Contains("pink") || id.Contains("flower") || id.Contains("new_year") || id.Contains("heaven");
            bool leaves = id.Contains("autumn") || id.Contains("bamboo") || id.Contains("forest") || id.Contains("fall") || id.Contains("village") ||
                id.Contains("road") || id.Contains("waterfall") || id.Contains("dojo");
            if (!petals && !leaves) return;
            TitleLeaf.Scatter(_particles, petals ? 18 : 12, petals ? TitleLeaf.Kind.Petal : TitleLeaf.Kind.Leaf, -40f, 1320f, -60f, 740f, 700f);
        }

        private void LateUpdate()
        {
            if (_camera == null) return;
            EnsureCamera();
            _render?.UpdateMenuBackdrop(_camera, false);
            // Slow drift and breathing zoom, plus a little parallax toward the pointer.
            float time = Time.unscaledTime;
            Vector2 pointer = new Vector2(UnityEngine.Input.mousePosition.x / Mathf.Max(1, Screen.width) - .5f, UnityEngine.Input.mousePosition.y / Mathf.Max(1, Screen.height) - .5f);
            _parallax = Vector2.Lerp(_parallax, pointer, 1f - Mathf.Exp(-Time.unscaledDeltaTime * 2f));
            float zoom = 1.06f + .015f * Mathf.Sin(time * .11f);
            var drift = new Vector2(Mathf.Sin(time * .05f) * 10f, Mathf.Sin(time * .07f + 1.3f) * 5f) - _parallax * 18f;
            foreach (var picture in new[] { _image, _fading })
            {
                picture.rectTransform.localScale = new Vector3(zoom, zoom, 1f);
                picture.rectTransform.anchoredPosition = drift;
            }
            _image.color = new Color(_dim, _dim * .97f, _dim * .94f, 1f);
            if (_fading.enabled)
            {
                _fade -= Time.unscaledDeltaTime / FadeSeconds;
                _fading.color = new Color(_dim, _dim * .97f, _dim * .94f, Mathf.Clamp01(_fade));
                if (_fade <= 0f) _fading.enabled = false;
            }
        }

        private void FixedUpdate()
        {
            if (_camera != null) _render?.UpdateMenuBackdrop(_camera, true);
        }

        private void OnDestroy()
        {
            DestroyScenery();
            if (_camera != null) Destroy(_camera.gameObject);
            Release(ref _texture);
            Release(ref _fadeTexture);
        }

        private static Texture2D _vignetteTexture;

        /// <summary>Clear in the middle, ink at the edges and heavier along the bottom where menus sit.</summary>
        private static Texture2D VignetteTexture()
        {
            if (_vignetteTexture != null) return _vignetteTexture;
            const int width = 128, height = 72;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
            var pixels = new Color32[width * height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    float u = x / (width - 1f) * 2f - 1f, v = y / (height - 1f) * 2f - 1f;
                    float edge = Mathf.Clamp01(Mathf.Sqrt(u * u * .8f + v * v * .9f) - .35f) / .75f;
                    float bottom = Mathf.Clamp01((-v - .2f) / .8f) * .45f;
                    float alpha = Mathf.Clamp01(edge * edge * .9f + bottom);
                    pixels[y * width + x] = new Color32(14, 10, 8, (byte)(alpha * 255));
                }
            texture.SetPixels32(pixels);
            texture.Apply(false, true);
            return _vignetteTexture = texture;
        }
    }
}
