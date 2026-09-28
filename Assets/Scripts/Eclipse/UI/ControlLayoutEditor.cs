using System;
using System.Collections.Generic;
using Nekki.SF2.GUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // Full-screen editor for ControlLayout, opened from Options > Controls. A dummy fight
    // (the dojo, two shadow fighters and a mock HUD) sits behind the real control art in the
    // same canvas space the fight uses, so what you arrange is what you get. Drag a control to
    // move it; the panel resizes the selected one and sets opacity, grid snapping, mirroring
    // and reset. Keyboard: Tab selects, arrows move (Shift: faster), +/- resize, Enter saves,
    // Esc cancels.
    public sealed class ControlLayoutEditor : MonoBehaviour
    {
        private static readonly Color Ink = new Color32(30, 25, 22, 255);
        private static readonly Color Paper = new Color32(223, 207, 177, 255);
        private static readonly Color Red = new Color32(147, 39, 31, 255);
        private static readonly Color Gold = new Color32(214, 170, 78, 255);
        private const float Grid = 32f, FadeSeconds = .25f;

        private static ControlLayoutEditor current;
        public static bool IsOpen => current != null;
        private static int closedFrame = -1;
        // True while open and on the frame it closed, so its Esc/Enter do not reach the page below.
        public static bool BlocksInput => current != null || Time.frameCount <= closedFrame;

        private sealed class Handle
        {
            public ControlLayout.Control Control;
            public RectTransform Rect;
            public Image Art;
            public UiDisc Ring;
        }

        private readonly List<Handle> handles = new List<Handle>();
        private readonly Dictionary<Texture2D, FilterMode> filtered = new Dictionary<Texture2D, FilterMode>();
        private ControlLayout.Layout layout;
        private Action closed;
        private RectTransform root, panel;
        private CanvasGroup group;
        private Font font;
        private Text selectedLabel, sizeLabel, opacityLabel, snapLabel, hint;
        private Slider sizeSlider, opacitySlider;
        private int selected = -1;
        private float openedAt, leaveAt = -1f;
        private bool syncing;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSession() { current = null; closedFrame = -1; }

        public static void Open(Action onClosed)
        {
            if (current != null) return;
            var host = new GameObject("Eclipse Touch Layout", typeof(RectTransform));
            current = host.AddComponent<ControlLayoutEditor>();
            current.closed = onClosed;
        }

        private void Awake()
        {
            layout = ControlLayout.Current.Clone();
            font = Resources.Load<Font>("ui/fonts/AGOpusBold") ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32765;
            // The fight's own canvas scaling: 1536 units tall, width follows the screen.
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(2048, 1536);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 1f;
            gameObject.AddComponent<GraphicRaycaster>();
            group = gameObject.AddComponent<CanvasGroup>();
            group.alpha = 0f;
            if (EventSystem.current == null)
                new GameObject("Touch Layout Input", typeof(EventSystem), typeof(StandaloneInputModule)).transform.SetParent(transform, false);

            root = Stretch(transform, "Root");
            BuildPreview();
            foreach (var control in ControlLayout.Controls) handles.Add(BuildHandle(control));
            BuildPanel();
            Select(0);
            openedAt = Time.unscaledTime;
            EclipseUiAudio.Play(UiSound.Open);
        }

        // --- Dummy fight --------------------------------------------------------------------

        private void BuildPreview()
        {
            var backdrop = Stretch(root, "Backdrop").gameObject.AddComponent<Image>();
            backdrop.color = new Color32(20, 12, 8, 255);
            Scenery("Textures/Locations/dojo/background_1");
            Scenery("Textures/Locations/dojo/layer_2_1");
            var dim = Stretch(root, "Dim").gameObject.AddComponent<Image>();
            dim.color = new Color(0f, 0f, 0f, .35f);
            dim.raycastTarget = false;
            // Floor band the fighters stand on.
            var floor = Anchored(root, "Floor", new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 250f), new Vector2(.5f, 0f));
            var floorImage = floor.gameObject.AddComponent<Image>();
            floorImage.color = new Color32(26, 15, 9, 235);
            floorImage.raycastTarget = false;
            var silhouette = SilhouetteTexture();
            if (silhouette != null)
            {
                Fighter(silhouette, .38f, false);
                Fighter(silhouette, .62f, true);
            }
            // A mock HUD: two health bars and the round timer.
            HealthBar(true); HealthBar(false);
            var timer = Label(root, "99", 0, 0, 200, 110, 96, Paper, TextAnchor.MiddleCenter);
            Place(timer.rectTransform, new Vector2(.5f, 1f), new Vector2(0f, -86f));
        }

        private void Scenery(string address)
        {
            Sprite sprite = null;
            try { sprite = Eclipse.Content.PackagedArtCatalog.Load<Sprite>(address); }
            catch (Exception) { }
            if (sprite == null || sprite.texture == null) return;
            var texture = sprite.texture;
            if (texture.filterMode != FilterMode.Bilinear && !filtered.ContainsKey(texture))
            {
                filtered.Add(texture, texture.filterMode);
                texture.filterMode = FilterMode.Bilinear;
            }
            var image = Stretch(root, "Scenery").gameObject.AddComponent<RawImage>();
            image.texture = texture;
            var r = sprite.textureRect;
            image.uvRect = new Rect(r.x / texture.width, r.y / texture.height, r.width / texture.width, r.height / texture.height);
            image.raycastTarget = false;
            // Cover the canvas at the art's aspect ratio.
            var fit = image.gameObject.AddComponent<AspectRatioFitter>();
            fit.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fit.aspectRatio = r.width / r.height;
        }

        private void Fighter(Texture2D texture, float x, bool mirror)
        {
            var rect = Anchored(root, mirror ? "Opponent" : "Player", new Vector2(x, 0f), new Vector2(x, 0f), new Vector2(0f, 190f),
                new Vector2(620f * texture.width / texture.height, 620f), new Vector2(.5f, 0f));
            var image = rect.gameObject.AddComponent<RawImage>();
            image.texture = texture;
            image.color = new Color32(10, 7, 6, 255);
            image.raycastTarget = false;
            if (mirror) rect.localScale = new Vector3(-1f, 1f, 1f);
        }

        private void HealthBar(bool left)
        {
            var anchor = new Vector2(left ? 0f : 1f, 1f);
            var frame = Anchored(root, left ? "Player health" : "Opponent health", anchor, anchor,
                new Vector2(left ? 90f : -90f, -70f), new Vector2(760f, 46f), new Vector2(left ? 0f : 1f, 1f));
            frame.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, .7f);
            var fill = Anchored(frame, "Fill", Vector2.zero, Vector2.one, Vector2.zero, new Vector2(-10f, -10f), new Vector2(.5f, .5f));
            fill.gameObject.AddComponent<Image>().color = new Color32(178, 36, 26, 255);
            var name = Label(frame, left ? "YOU" : "SHADOW", 0, 0, 400, 40, 30, Paper, left ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight);
            var nameRect = name.rectTransform;
            nameRect.anchorMin = nameRect.anchorMax = new Vector2(left ? 0f : 1f, 0f);
            nameRect.pivot = new Vector2(left ? 0f : 1f, 1f);
            nameRect.anchoredPosition = new Vector2(0f, -8f);
            foreach (var graphic in frame.GetComponentsInChildren<Graphic>()) graphic.raycastTarget = false;
        }

        // The splash's white fighter becomes a transparent-backed shadow, cropped to the figure.
        private static Texture2D silhouetteCache;

        private static Texture2D SilhouetteTexture()
        {
            if (silhouetteCache != null) return silhouetteCache;
            var source = Resources.Load<Texture2D>("EclipseTitle/splash");
            if (source == null) return null;
            int w = Mathf.Max(1, source.width / 2), h = Mathf.Max(1, source.height / 2);
            var target = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32);
            var previous = RenderTexture.active;
            Graphics.Blit(source, target);
            RenderTexture.active = target;
            var read = new Texture2D(w, h, TextureFormat.RGBA32, false);
            read.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            read.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(target);
            var pixels = read.GetPixels32();
            int minX = w, minY = h, maxX = -1, maxY = -1;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    var p = pixels[y * w + x];
                    byte a = (byte)Mathf.Clamp((p.r + p.g + p.b) / 3 * 2 - 40, 0, 255);
                    pixels[y * w + x] = new Color32(255, 255, 255, a);
                    if (a > 30) { minX = Mathf.Min(minX, x); maxX = Mathf.Max(maxX, x); minY = Mathf.Min(minY, y); maxY = Mathf.Max(maxY, y); }
                }
            Destroy(read);
            if (maxX < minX) return null;
            int cw = maxX - minX + 1, ch = maxY - minY + 1;
            var cropped = new Color32[cw * ch];
            for (int y = 0; y < ch; y++) Array.Copy(pixels, (minY + y) * w + minX, cropped, y * cw, cw);
            silhouetteCache = new Texture2D(cw, ch, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            silhouetteCache.SetPixels32(cropped);
            silhouetteCache.Apply();
            return silhouetteCache;
        }

        // --- Controls -----------------------------------------------------------------------

        private Handle BuildHandle(ControlLayout.Control control)
        {
            var rect = Anchored(root, control.Name, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one * control.BaseSize, new Vector2(.5f, .5f));
            var ring = Anchored(rect, "Selection", new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, Vector2.one * control.BaseSize * 1.08f, new Vector2(.5f, .5f))
                .gameObject.AddComponent<UiDisc>();
            ring.color = new Color(Gold.r, Gold.g, Gold.b, .16f);
            ring.SetRing(Gold, 6f);
            ring.raycastTarget = false;
            var art = rect.gameObject.AddComponent<Image>();
            art.sprite = ResolutionImage.GetSprite("UI/Atlases/", control.Sprite);
            art.preserveAspect = true;
            art.color = art.sprite == null ? new Color(Paper.r, Paper.g, Paper.b, .5f) : Color.white;
            if (control.Id == "stick")
            {
                var nub = Anchored(rect, "Nub", new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, Vector2.one * control.BaseSize * 300f / 620f, new Vector2(.5f, .5f))
                    .gameObject.AddComponent<Image>();
                nub.sprite = ResolutionImage.GetSprite("UI/Atlases/", "FightButtons.Joystick_norm");
                nub.preserveAspect = true;
                nub.raycastTarget = false;
            }
            var caption = Label(rect, control.Name.ToUpperInvariant(), 0, 0, 400, 40, 26, Paper, TextAnchor.MiddleCenter);
            var captionRect = caption.rectTransform;
            captionRect.anchorMin = captionRect.anchorMax = new Vector2(.5f, 0f);
            captionRect.pivot = new Vector2(.5f, 1f);
            captionRect.anchoredPosition = new Vector2(0f, -6f);
            caption.gameObject.AddComponent<Shadow>().effectColor = new Color(0f, 0f, 0f, .8f);
            var handle = new Handle { Control = control, Rect = rect, Art = art, Ring = ring };
            var drag = rect.gameObject.AddComponent<DragTarget>();
            drag.Owner = this; drag.Index = handles.Count;
            return handle;
        }

        // Pointer input for one control.
        private sealed class DragTarget : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
        {
            public ControlLayoutEditor Owner;
            public int Index;
            private Vector2 grab;

            public void OnPointerDown(PointerEventData data)
            {
                Owner.Select(Index);
                grab = Owner.CanvasPoint(data.position) - Owner.Centre(Index);
            }

            public void OnDrag(PointerEventData data) { Owner.MoveTo(Index, Owner.CanvasPoint(data.position) - grab, false); }
            public void OnEndDrag(PointerEventData data) { Owner.MoveTo(Index, Owner.CanvasPoint(data.position) - grab, true); }
        }

        private Vector2 CanvasSize => root.rect.size;

        private Vector2 CanvasPoint(Vector2 screen)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root, screen, null, out var local);
            return local + Vector2.Scale(CanvasSize, root.pivot);
        }

        private Vector2 Centre(int index) { return ControlLayout.ToCanvas(layout.Get(handles[index].Control.Id), CanvasSize); }

        private float VisualSize(int index)
        {
            return handles[index].Control.BaseSize * ControlLayout.ContainerScale * layout.Get(handles[index].Control.Id).size;
        }

        private void MoveTo(int index, Vector2 point, bool finished)
        {
            if (layout.snap && finished) point = new Vector2(Mathf.Round(point.x / Grid) * Grid, Mathf.Round(point.y / Grid) * Grid);
            // Keep at least half of the control on screen.
            Vector2 canvas = CanvasSize;
            point.x = Mathf.Clamp(point.x, 0f, canvas.x);
            point.y = Mathf.Clamp(point.y, 0f, canvas.y);
            ControlLayout.FromCanvas(layout.Get(handles[index].Control.Id), point, canvas);
            layout.enabled = true;
            if (finished) EclipseUiAudio.Play(UiSound.Tick);
        }

        private void Select(int index)
        {
            if (index < 0 || index >= handles.Count) return;
            if (index != selected) EclipseUiAudio.Play(UiSound.Focus);
            selected = index;
            handles[index].Rect.SetAsLastSibling();
            panel?.SetAsLastSibling();
            SyncPanel();
        }

        // --- Settings panel -----------------------------------------------------------------

        private void BuildPanel()
        {
            panel = Anchored(root, "Panel", new Vector2(.5f, 1f), new Vector2(.5f, 1f), new Vector2(0f, -180f), new Vector2(1040f, 470f), new Vector2(.5f, 1f));
            var paper = panel.gameObject.AddComponent<PaperPanel>();
            paper.color = new Color(Paper.r, Paper.g, Paper.b, .96f);
            panel.gameObject.AddComponent<PanelDrag>().Target = panel;
            Label(panel, "TOUCH CONTROLS LAYOUT", 40, 26, 960, 56, 44, Ink, TextAnchor.MiddleLeft);
            hint = Label(panel, "Drag a control to move it. Drag this card to move it out of the way.", 40, 80, 960, 34, 24, new Color(Ink.r, Ink.g, Ink.b, .75f), TextAnchor.MiddleLeft);
            selectedLabel = Label(panel, "", 40, 128, 460, 44, 30, Red, TextAnchor.MiddleLeft);
            sizeLabel = Label(panel, "", 40, 176, 300, 40, 28, Ink, TextAnchor.MiddleLeft);
            sizeSlider = Slider(panel, 330, 176, 670, value =>
            {
                if (syncing || selected < 0) return;
                layout.Get(handles[selected].Control.Id).size = value;
                layout.enabled = true;
                SyncPanel();
            }, ControlLayout.MinSize, ControlLayout.MaxSize);
            opacityLabel = Label(panel, "", 40, 232, 300, 40, 28, Ink, TextAnchor.MiddleLeft);
            opacitySlider = Slider(panel, 330, 232, 670, value =>
            {
                if (syncing) return;
                layout.opacity = value;
                layout.enabled = true;
                SyncPanel();
            }, ControlLayout.MinOpacity, 1f);
            snapLabel = PanelButton("Snap to grid: On", 40, 296, 300, () => { layout.snap = !layout.snap; SyncPanel(); }).GetComponentInChildren<Text>();
            PanelButton("Mirror (left-handed)", 360, 296, 320, Mirror);
            PanelButton("Reset to default", 700, 296, 300, ResetLayout);
            PanelButton("Cancel", 40, 380, 300, () => Close(false));
            PanelButton("Save layout", 700, 380, 300, () => Close(true));
            Label(panel, "Tab: select   Arrows: move   + / -: size   Enter: save   Esc: cancel", 360, 380, 330, 64, 20,
                new Color(Ink.r, Ink.g, Ink.b, .7f), TextAnchor.MiddleCenter);
        }

        private sealed class PanelDrag : MonoBehaviour, IDragHandler
        {
            public RectTransform Target;
            public void OnDrag(PointerEventData data)
            {
                var canvas = Target.GetComponentInParent<Canvas>();
                Target.anchoredPosition += data.delta / (canvas != null ? canvas.scaleFactor : 1f);
            }
        }

        private void SyncPanel()
        {
            if (panel == null) return;
            syncing = true;
            if (selected >= 0)
            {
                var placement = layout.Get(handles[selected].Control.Id);
                selectedLabel.text = "Selected: " + handles[selected].Control.Name;
                sizeLabel.text = "Size  " + Mathf.RoundToInt(placement.size * 100f) + "%";
                sizeSlider.value = placement.size;
            }
            opacityLabel.text = "Opacity  " + Mathf.RoundToInt(layout.opacity * 100f) + "%";
            opacitySlider.value = layout.opacity;
            snapLabel.text = "Snap to grid: " + (layout.snap ? "On" : "Off");
            syncing = false;
        }

        private void Mirror()
        {
            foreach (var placement in layout.controls) placement.x = 1f - placement.x;
            layout.enabled = true;
            EclipseUiAudio.Play(UiSound.Toggle);
        }

        private void ResetLayout()
        {
            bool snap = layout.snap;
            layout = ControlLayout.Default();
            layout.snap = snap;
            EclipseUiAudio.Play(UiSound.Toggle);
            SyncPanel();
        }

        // --- Frame --------------------------------------------------------------------------

        private void Update()
        {
            float now = Time.unscaledTime;
            if (leaveAt >= 0f)
            {
                float t = Mathf.Clamp01((now - leaveAt) / FadeSeconds);
                group.alpha = 1f - t;
                if (t >= 1f) Destroy(gameObject);
                return;
            }
            group.alpha = Mathf.Clamp01((now - openedAt) / FadeSeconds);
            ReadKeys();
            Vector2 canvas = CanvasSize;
            float pulse = .5f + .5f * Mathf.Sin(now * 5f);
            for (int i = 0; i < handles.Count; i++)
            {
                var handle = handles[i];
                handle.Rect.anchoredPosition = Centre(i);
                float size = VisualSize(i);
                handle.Rect.sizeDelta = Vector2.one * handle.Control.BaseSize;
                handle.Rect.localScale = Vector3.one * (size / handle.Control.BaseSize);
                handle.Art.color = new Color(1f, 1f, 1f, layout.opacity);
                handle.Ring.enabled = i == selected;
                if (i == selected) handle.Ring.color = new Color(Gold.r, Gold.g, Gold.b, .1f + .12f * pulse);
            }
        }

        private void ReadKeys()
        {
            if (Time.unscaledTime - openedAt < .1f) return;
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape)) { Close(false); return; }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Return) || UnityEngine.Input.GetKeyDown(KeyCode.KeypadEnter)) { Close(true); return; }
            bool shift = UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift);
            if (UnityEngine.Input.GetKeyDown(KeyCode.Tab)) Select((selected + (shift ? handles.Count - 1 : 1)) % handles.Count);
            if (selected < 0) return;
            float step = shift ? 40f : 8f;
            Vector2 move = Vector2.zero;
            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow)) move.x -= step;
            if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow)) move.x += step;
            if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow)) move.y += step;
            if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow)) move.y -= step;
            if (move != Vector2.zero)
            {
                bool snap = layout.snap; layout.snap = false;
                MoveTo(selected, Centre(selected) + move, false);
                layout.snap = snap;
            }
            float resize = UnityEngine.Input.GetKeyDown(KeyCode.Equals) || UnityEngine.Input.GetKeyDown(KeyCode.KeypadPlus) ? .05f :
                UnityEngine.Input.GetKeyDown(KeyCode.Minus) || UnityEngine.Input.GetKeyDown(KeyCode.KeypadMinus) ? -.05f : 0f;
            if (resize != 0f)
            {
                var placement = layout.Get(handles[selected].Control.Id);
                placement.size = Mathf.Clamp(placement.size + resize, ControlLayout.MinSize, ControlLayout.MaxSize);
                layout.enabled = true;
                SyncPanel();
            }
        }

        private void Close(bool save)
        {
            if (leaveAt >= 0f) return;
            if (save) ControlLayout.Save(layout);
            EclipseUiAudio.Play(save ? UiSound.Confirm : UiSound.Back);
            leaveAt = Time.unscaledTime;
            group.blocksRaycasts = false;
            if (current == this) current = null;
            closedFrame = Time.frameCount;
        }

        private void OnDestroy()
        {
            if (current == this) current = null;
            foreach (var pair in filtered) if (pair.Key != null) pair.Key.filterMode = pair.Value;
            closed?.Invoke();
        }

        // --- Construction helpers -----------------------------------------------------------

        private static RectTransform Stretch(Transform parent, string name)
        {
            return Anchored(parent, name, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(.5f, .5f));
        }

        private static RectTransform Anchored(Transform parent, string name, Vector2 min, Vector2 max, Vector2 position, Vector2 size, Vector2 pivot)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = min; rect.anchorMax = max; rect.pivot = pivot;
            rect.anchoredPosition = position; rect.sizeDelta = size;
            return rect;
        }

        private static void Place(RectTransform rect, Vector2 anchor, Vector2 position)
        {
            rect.anchorMin = rect.anchorMax = anchor;
            rect.pivot = new Vector2(.5f, 1f);
            rect.anchoredPosition = position;
        }

        // Panel-local text: x/y from the panel's top-left.
        private Text Label(Transform parent, string text, float x, float y, float w, float h, int size, Color color, TextAnchor alignment)
        {
            var rect = Anchored(parent, text, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(x, -y), new Vector2(w, h), new Vector2(0f, 1f));
            var label = rect.gameObject.AddComponent<Text>();
            label.font = font; label.text = text; label.fontSize = size; label.color = color;
            label.alignment = alignment; label.raycastTarget = false;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            return label;
        }

        private Button PanelButton(string text, float x, float y, float w, Action action)
        {
            var rect = Anchored(panel, text, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(x, -y), new Vector2(w, 64f), new Vector2(0f, 1f));
            var hit = rect.gameObject.AddComponent<Image>();
            hit.color = Color.clear;
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = hit;
            var plate = Anchored(rect, "Plate", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(.5f, .5f)).gameObject.AddComponent<InkStroke>();
            plate.raycastTarget = false;
            plate.Seed = text.GetHashCode() & 0xffff;
            var caption = Label(rect, text, 20, 0, w - 40, 64, 28, Paper, TextAnchor.MiddleCenter);
            EclipseUiButton.Attach(button, plate, caption, Ink, Red, Paper, Paper, 0f, .03f);
            button.onClick.AddListener(() => action());
            return button;
        }

        private Slider Slider(Transform parent, float x, float y, float w, Action<float> changed, float min, float max)
        {
            var rootRect = Anchored(parent, "Slider", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(x, -y), new Vector2(w, 40f), new Vector2(0f, 1f));
            rootRect.gameObject.AddComponent<Image>().color = Color.clear;
            var groove = Anchored(rootRect, "Groove", new Vector2(0f, .5f), new Vector2(1f, .5f), Vector2.zero, new Vector2(-40f, 10f), new Vector2(.5f, .5f));
            groove.gameObject.AddComponent<Image>().color = new Color(Ink.r, Ink.g, Ink.b, .25f);
            var fillArea = Anchored(rootRect, "Fill area", new Vector2(0f, .5f), new Vector2(1f, .5f), Vector2.zero, new Vector2(-40f, 10f), new Vector2(.5f, .5f));
            var fill = Anchored(fillArea, "Fill", Vector2.zero, new Vector2(0f, 1f), Vector2.zero, Vector2.zero, new Vector2(0f, .5f));
            fill.gameObject.AddComponent<Image>().color = Red;
            var handleArea = Anchored(rootRect, "Knob area", new Vector2(0f, .5f), new Vector2(1f, .5f), Vector2.zero, new Vector2(-40f, 40f), new Vector2(.5f, .5f));
            var knobRect = Anchored(handleArea, "Knob", new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(38f, 38f), new Vector2(.5f, .5f));
            var knob = knobRect.gameObject.AddComponent<UiDisc>();
            knob.color = Ink;
            knob.SetRing(Paper, 4f);
            var slider = rootRect.gameObject.AddComponent<Slider>();
            slider.fillRect = fill; slider.handleRect = knobRect; slider.targetGraphic = knob;
            slider.minValue = min; slider.maxValue = max;
            slider.onValueChanged.AddListener(v => changed(v));
            return slider;
        }
    }
}
