using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Eclipse.Multiplayer
{
    /// <summary>A blue ground highlight that identifies player two independently of camera focus.</summary>
    internal sealed class VersusGroundHighlight
    {
        private readonly SpriteRenderer renderer;

        public VersusGroundHighlight(Transform parent)
        {
            renderer = new GameObject("Player two ground highlight").AddComponent<SpriteRenderer>();
            renderer.transform.SetParent(parent, false);
            renderer.sprite = Eclipse.Rendering.FxBuilder.ShapeSprite(Eclipse.Modding.ModFxShape.Glow);
            renderer.color = new Color(.2f, .6f, 1f, .85f);
        }

        public void Update(float x, float y, float scale)
        {
            renderer.transform.localPosition = new Vector3(x, y, -40f);
            var size = renderer.sprite.bounds.size;
            renderer.transform.localScale = new Vector3(100f * scale / size.x, 12f * scale / size.y, 1f);
        }
    }

    /// <summary>Runs callbacks when a UI element gains or loses the pointer or the selection.</summary>
    public sealed class UiHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        public Action Enter, Exit;
        private bool _pointer, _selected;

        public static UiHover Attach(GameObject target, Action enter, Action exit)
        {
            var hover = Eclipse.UI.ComponentUtility.Ensure<UiHover>(target);
            hover.Enter = enter;
            hover.Exit = exit;
            return hover;
        }

        public void OnPointerEnter(PointerEventData data) { _pointer = true; Enter?.Invoke(); }
        public void OnPointerExit(PointerEventData data) { _pointer = false; if (!_selected) Exit?.Invoke(); }
        public void OnSelect(BaseEventData data) { _selected = true; Enter?.Invoke(); }
        public void OnDeselect(BaseEventData data) { _selected = false; if (!_pointer) Exit?.Invoke(); }
    }

    /// <summary>Signal bars for a ping: green, amber or red, grey when unknown or stale.</summary>
    public sealed class SignalBars : UnityEngine.UI.MaskableGraphic
    {
        private int _ping = -1;
        private bool _stale;

        public void Set(int pingMs, bool stale)
        {
            if (_ping == pingMs && _stale == stale) return;
            _ping = pingMs;
            _stale = stale;
            SetVerticesDirty();
        }

        public static int Bars(int pingMs) => pingMs < 0 ? 0 : pingMs < 60 ? 4 : pingMs < 100 ? 3 : pingMs < 160 ? 2 : 1;

        public static Color ColorFor(int pingMs, bool stale) =>
            stale || pingMs < 0 ? new Color(.55f, .52f, .48f, 1f) : pingMs < 80 ? new Color32(96, 176, 92, 255) :
            pingMs < 150 ? new Color32(214, 170, 78, 255) : new Color32(206, 72, 52, 255);

        protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper vertices)
        {
            vertices.Clear();
            var rect = GetPixelAdjustedRect();
            int lit = _stale ? 0 : Bars(_ping);
            var on = ColorFor(_ping, _stale) * color;
            var off = new Color(0, 0, 0, .28f * color.a);
            float gap = rect.width * .08f, width = (rect.width - gap * 3f) / 4f;
            for (int i = 0; i < 4; i++)
            {
                float x = rect.xMin + i * (width + gap);
                float height = rect.height * (.35f + .65f * (i + 1) / 4f);
                var tint = i < lit ? on : off;
                int start = vertices.currentVertCount;
                vertices.AddVert(new Vector3(x, rect.yMin), tint, Vector2.zero);
                vertices.AddVert(new Vector3(x, rect.yMin + height), tint, Vector2.zero);
                vertices.AddVert(new Vector3(x + width, rect.yMin + height), tint, Vector2.zero);
                vertices.AddVert(new Vector3(x + width, rect.yMin), tint, Vector2.zero);
                vertices.AddTriangle(start, start + 1, start + 2);
                vertices.AddTriangle(start, start + 2, start + 3);
            }
        }
    }
}
