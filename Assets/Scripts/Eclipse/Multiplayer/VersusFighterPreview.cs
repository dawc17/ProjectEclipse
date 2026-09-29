using System;
using System.Collections.Generic;
using Nekki.SF2.Core.Fights;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Multiplayer
{
    /// <summary>
    /// A live fighter wearing a versus loadout, idling, drawn into a UI RawImage. The
    /// fighter is built with the same menu model path as the profile screen
    /// (<see cref="ModelContainer.ShowParameters"/>) far away from any scene content, and a
    /// private camera frames whatever it renders, so the model's world scale never matters.
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public sealed class VersusFighterPreview : MonoBehaviour
    {
        private const float FarX = -60000f;
        private const float RebuildDelay = .12f;
        private static int _nextSlot;
        /// <summary>The game's shadow-fighter ink; reads well on paper cards.</summary>
        public static readonly Color DefaultTint = new Color32(40, 20, 9, 255);

        private RawImage _image;
        private GameObject _world;
        private ModelContainer _container;
        private UnityEngine.Camera _camera;
        private RenderTexture _texture;
        private VersusLoadout _shown, _wanted;
        private float _rebuildAt = -1f;
        private int _slot;
        private bool _mirror;
        private Color _tint = DefaultTint;
        private readonly List<Renderer> _renderers = new List<Renderer>();
        private float _nextRendererScan;
        private Vector3 _center;
        private float _size = -1f;

        public VersusLoadout Loadout => _wanted ?? _shown;

        /// <summary>Adds a preview filling <paramref name="parent"/>; <paramref name="mirror"/> faces it left.</summary>
        public static VersusFighterPreview Create(RectTransform parent, bool mirror, Color? tint = null)
        {
            var rect = new GameObject("Fighter preview", typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero;
            var image = rect.gameObject.AddComponent<RawImage>();
            image.raycastTarget = false;
            image.color = new Color(1, 1, 1, 0);
            var preview = rect.gameObject.AddComponent<VersusFighterPreview>();
            preview._image = image;
            preview._mirror = mirror;
            if (tint.HasValue) preview._tint = tint.Value;
            preview._slot = _nextSlot++ % 64;
            image.uvRect = mirror ? new Rect(1, 0, -1, 1) : new Rect(0, 0, 1, 1);
            return preview;
        }

        /// <summary>Shows <paramref name="loadout"/>; quick successive calls rebuild once.</summary>
        public void Show(VersusLoadout loadout, bool immediately = false)
        {
            if (loadout == null) return;
            _wanted = loadout;
            if (_shown != null && _shown.Equals(loadout)) { _wanted = null; return; }
            _rebuildAt = immediately ? 0f : Time.unscaledTime + RebuildDelay;
        }

        private void Rebuild()
        {
            var loadout = _wanted;
            _wanted = null;
            _rebuildAt = -1f;
            if (loadout == null) return;
            EnsureWorld();
            try
            {
                var parameters = LocalVersusMatch.PrepareFighter(loadout, true, string.Empty);
                _container.ShowParameters(parameters, StageType.FDBBPEGEGMK.STAGE_SHOP_START, "Profile", _tint);
                _shown = loadout;
                _renderers.Clear();
                _nextRendererScan = 0f;
                _size = -1f;
            }
            catch (Exception exception)
            {
                Debug.LogWarning("[Versus] Fighter preview for " + loadout + " failed: " + exception.Message);
            }
        }

        private void EnsureWorld()
        {
            if (_world != null) return;
            var origin = new Vector3(FarX - _slot * 500f, 0f, 0f);
            _world = new GameObject("Versus fighter preview " + _slot);
            _world.transform.position = origin;
            _container = _world.AddComponent<ModelContainer>();
            _container.Init();
            var host = new GameObject("Versus fighter preview camera " + _slot);
            host.transform.SetParent(_world.transform, false);
            host.transform.localPosition = new Vector3(0, 0, -100f);
            _camera = host.AddComponent<UnityEngine.Camera>();
            _camera.orthographic = true;
            _camera.orthographicSize = 3f;
            _camera.nearClipPlane = .1f;
            _camera.farClipPlane = 600f;
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.backgroundColor = new Color(0, 0, 0, 0);
            _camera.depth = -99;
            _camera.allowHDR = false;
            var rect = ((RectTransform)transform).rect;
            int width = Mathf.Clamp(Mathf.RoundToInt(rect.width * 1.5f), 128, 1024), height = Mathf.Clamp(Mathf.RoundToInt(rect.height * 1.5f), 128, 1024);
            _texture = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32) { name = "Versus fighter preview", antiAliasing = 4 };
            _texture.Create();
            _camera.targetTexture = _texture;
            _camera.aspect = (float)width / height;
            _image.texture = _texture;
        }

        private void LateUpdate()
        {
            if (_rebuildAt >= 0f && Time.unscaledTime >= _rebuildAt) Rebuild();
            if (_camera == null) return;
            if (Time.unscaledTime >= _nextRendererScan)
            {
                _nextRendererScan = Time.unscaledTime + .5f;
                _renderers.Clear();
                _world.GetComponentsInChildren(false, _renderers);
                _renderers.RemoveAll(renderer => renderer == null || renderer.gameObject == _camera.gameObject);
            }
            if (!TryBounds(out var bounds)) return;
            // Frame the pose; grow at once, shrink slowly, so attacks never jitter the framing.
            float needed = Mathf.Max(bounds.extents.y, bounds.extents.x / Mathf.Max(.1f, _camera.aspect)) * 1.18f;
            _size = _size < 0 ? needed : needed > _size ? needed : Mathf.Lerp(_size, needed, 1f - Mathf.Exp(-Time.unscaledDeltaTime * .8f));
            var center = bounds.center;
            _center = _center == Vector3.zero ? center : Vector3.Lerp(_center, center, 1f - Mathf.Exp(-Time.unscaledDeltaTime * 4f));
            _camera.orthographicSize = Mathf.Max(.01f, _size);
            _camera.transform.position = new Vector3(_center.x, _center.y - _size * .04f, bounds.min.z - 50f);
            float alpha = Mathf.MoveTowards(_image.color.a, 1f, Time.unscaledDeltaTime * 4f);
            _image.color = new Color(1, 1, 1, alpha);
        }

        private bool TryBounds(out Bounds bounds)
        {
            bounds = default;
            bool any = false;
            foreach (var renderer in _renderers)
            {
                if (renderer == null || !renderer.enabled || !renderer.gameObject.activeInHierarchy) continue;
                var b = renderer.bounds;
                if (b.size.sqrMagnitude <= 0f || float.IsNaN(b.center.x)) continue;
                if (!any) { bounds = b; any = true; }
                else bounds.Encapsulate(b);
            }
            return any;
        }

        private void OnDestroy()
        {
            if (_world != null) Destroy(_world);
            if (_texture != null) { _texture.Release(); Destroy(_texture); }
        }
    }
}
