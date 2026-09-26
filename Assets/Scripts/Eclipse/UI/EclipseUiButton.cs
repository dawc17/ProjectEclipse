using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // Animated focus for Eclipse menu controls. Hovering focuses the control, so mouse,
    // keyboard and gamepad share one highlight. Colours ease instead of snapping, the label
    // slides slightly and a press gives a short squash. Runs on unscaled time.
    [DisallowMultipleComponent]
    public sealed class EclipseUiButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        private const float FocusSeconds = .14f;
        private Selectable target;
        private Graphic body;
        private Text label;
        private Color normal, highlight, labelNormal, labelHighlight;
        private Vector2 labelHome;
        private float slide, grow, squash;
        private float focus, press;
        private bool selected, pressed;

        public float Focus => focus;

        public static EclipseUiButton Attach(Selectable target, Graphic body, Text label, Color normal, Color highlight,
            Color labelNormal, Color labelHighlight, float slide = 8f, float grow = .03f, float squash = .05f)
        {
            var fx = target.gameObject.GetComponent<EclipseUiButton>() ?? target.gameObject.AddComponent<EclipseUiButton>();
            fx.target = target; fx.body = body; fx.label = label;
            fx.slide = slide; fx.grow = grow; fx.squash = squash;
            target.transition = Selectable.Transition.None;
            // A Button tints its target the moment it is enabled; that tint would otherwise
            // multiply every colour set here (it made red plates render near-black).
            if (target.targetGraphic != null) target.targetGraphic.CrossFadeColor(Color.white, 0f, true, true);
            // Grow and squash about the centre; keep the control where it was authored.
            var rect = (RectTransform)target.transform;
            var shift = new Vector2(.5f, .5f) - rect.pivot;
            if (shift != Vector2.zero)
            {
                rect.pivot = new Vector2(.5f, .5f);
                rect.anchoredPosition += Vector2.Scale(shift, rect.rect.size);
            }
            if (label != null) fx.labelHome = label.rectTransform.anchoredPosition;
            fx.SetColors(normal, highlight, labelNormal, labelHighlight);
            return fx;
        }

        public void SetColors(Color normal, Color highlight, Color labelNormal, Color labelHighlight)
        {
            this.normal = normal; this.highlight = highlight;
            this.labelNormal = labelNormal; this.labelHighlight = labelHighlight;
            Apply();
        }

        public void OnSelect(BaseEventData data) { selected = true; EclipseUiAudio.Play(UiSound.Focus); }
        public void OnDeselect(BaseEventData data) { selected = false; pressed = false; }

        public void OnPointerEnter(PointerEventData data)
        {
            if (target == null || !target.IsInteractable() || EventSystem.current == null) return;
            if (EventSystem.current.currentSelectedGameObject != gameObject) target.Select();
        }

        public void OnPointerDown(PointerEventData data) { if (target != null && target.IsInteractable()) pressed = true; }
        public void OnPointerUp(PointerEventData data) { pressed = false; }

        // Call after moving the label yourself, so the focus slide starts from its new place.
        public void Rehome() { if (label != null) labelHome = label.rectTransform.anchoredPosition; }

        // Code-triggered presses (keyboard Enter) get the same squash as a click.
        public void Punch() { press = 1f; }

        private void OnDisable() { selected = pressed = false; focus = press = 0f; Apply(); }

        private void Update()
        {
            if (target == null) return;
            bool live = target.IsInteractable() && (selected ||
                (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject));
            focus = Mathf.MoveTowards(focus, live ? 1f : 0f, Time.unscaledDeltaTime / FocusSeconds);
            press = pressed ? Mathf.MoveTowards(press, 1f, Time.unscaledDeltaTime / .06f)
                : Mathf.MoveTowards(press, 0f, Time.unscaledDeltaTime / .22f);
            Apply();
        }

        private void Apply()
        {
            if (target == null) return;
            float t = focus * focus * (3f - 2f * focus);
            bool enabled = target.IsInteractable();
            if (body != null)
            {
                var color = Color.Lerp(normal, highlight, t);
                if (!enabled) color.a *= .42f;
                body.color = color;
            }
            if (label != null)
            {
                label.color = Color.Lerp(labelNormal, labelHighlight, t);
                label.rectTransform.anchoredPosition = labelHome + new Vector2(slide * t, 0f);
            }
            float scale = 1f + grow * t - squash * press;
            transform.localScale = new Vector3(scale, scale, 1f);
        }
    }
}
