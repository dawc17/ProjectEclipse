using System;
using System.Collections.Generic;
using Eclipse.Modding;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Eclipse.UI.Modding
{
    // Renders one owned surface within a coordinator-supplied mount. The coordinator
    // remains responsible for scene/pause/input priority; this view never polls input.
    public sealed class ModUiView : MonoBehaviour
    {
        private sealed class WidgetView
        {
            public RectTransform Rect;
            public CanvasGroup Group;
            public Text Label;
            public RectTransform Fill;
            public Button Button;
            public Selectable Control;
            public Toggle Toggle;
            public Slider Slider;
        }
        private readonly Dictionary<string, WidgetView> widgets = new Dictionary<string, WidgetView>(StringComparer.Ordinal);
        private readonly List<string> buttons = new List<string>();
        private ModUiSurface surface;
        private Font font;
        private GameObject previousSelection;
        private bool disposed;

        public static ModUiView Attach(ModUiSurface surface, RectTransform mount)
        {
            if (surface == null || surface.IsClosed) throw new ArgumentException("An open UI surface is required.");
            if (mount == null) throw new ArgumentNullException(nameof(mount));
            var root = new GameObject("Mod UI " + surface.Owner + "/" + surface.Id, typeof(RectTransform));
            root.transform.SetParent(mount, false);
            var view = root.AddComponent<ModUiView>();
            view.surface = surface;
            try
            {
                view.font = Resources.Load<Font>("ui/fonts/AGOpusBold") ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                view.previousSelection = EventSystem.current == null ? null : EventSystem.current.currentSelectedGameObject;
                var rect = root.GetComponent<RectTransform>();
                rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2((float)surface.Placement.AnchorX, (float)surface.Placement.AnchorY);
                rect.sizeDelta = new Vector2((float)surface.Root.Width, (float)surface.Root.Height);
                if (surface.Mount != ModUiMount.CombatHud)
                {
                    var paper = root.AddComponent<Image>();
                    Skin(paper,"DialogScroll.Background_Center",new Color32(203,171,120,255));
                    paper.color = ColorOf(surface.Root.Style.BackgroundColor,paper.color);
                    paper.raycastTarget = false;
                }
                view.Build(surface.Root, rect);
                surface.Changed += view.UpdateWidget;
                surface.Closed += view.Release;
                return view;
            }
            catch
            {
                view.Release();
                surface.Close(ModUiCloseReason.Error);
                throw;
            }
        }

        private RectTransform Rect(string name, Transform parent, double width, double height)
        {
            var item = new GameObject(name, typeof(RectTransform));
            var rect = item.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
            rect.sizeDelta = new Vector2((float)width, (float)height);
            return rect;
        }

        private static Color ColorOf(ModUiColor value, Color fallback) => value == null ? fallback : new Color32(value.R,value.G,value.B,value.A);

        private static void Skin(Image image, string name, Color fallback)
        {
            image.sprite = Nekki.SF2.GUI.ResolutionImage.GetSprite("UI/Atlases/", name);
            image.type = Image.Type.Sliced;
            image.color = image.sprite == null ? fallback : Color.white;
        }

        private Text Label(RectTransform rect, ModUiNode node)
        {
            var label = rect.gameObject.AddComponent<Text>();
            label.font = font; label.fontSize = node.Style.FontSize ?? 22; label.text = node.Text;
            label.supportRichText = false;
            Color normal = node.Kind == ModUiKind.Button ? new Color32(50,50,50,255) :
                surface.Mount == ModUiMount.CombatHud ? new Color32(223,207,177,255) : new Color32(47,37,27,255);
            label.color = ColorOf(node.Style.TextColor, normal);
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.alignment = node.Style.TextAlign == "left" ? TextAnchor.MiddleLeft :
                node.Style.TextAlign == "right" ? TextAnchor.MiddleRight : TextAnchor.MiddleCenter;
            label.raycastTarget = false;
            return label;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }

        private RectTransform Build(ModUiNode node, RectTransform parent)
        {
            var rect = Rect(node.Id, parent, node.Width, node.Height);
            var layout = rect.gameObject.AddComponent<LayoutElement>();
            layout.preferredWidth = (float)node.Width; layout.preferredHeight = (float)node.Height;
            layout.flexibleWidth = node.Width == 0 ? 1 : 0;
            layout.flexibleHeight = node.Height == 0 ? 1 : 0;
            var view = new WidgetView { Rect = rect, Group = rect.gameObject.AddComponent<CanvasGroup>() };
            widgets.Add(node.Id, view);
            if (node.Kind == ModUiKind.Row || node.Kind == ModUiKind.Column)
            {
                HorizontalOrVerticalLayoutGroup group = node.Kind == ModUiKind.Row
                    ? (HorizontalOrVerticalLayoutGroup)rect.gameObject.AddComponent<HorizontalLayoutGroup>()
                    : rect.gameObject.AddComponent<VerticalLayoutGroup>();
                group.spacing = (float)node.Gap; group.childAlignment = TextAnchor.UpperLeft;
                group.childControlWidth = group.childControlHeight = true;
                group.childForceExpandWidth = group.childForceExpandHeight = false;
            }
            bool container = node.Kind == ModUiKind.Stack || node.Kind == ModUiKind.Row || node.Kind == ModUiKind.Column || node.Kind == ModUiKind.Scroll;
            if (container && node.Style.BackgroundColor != null && !(node == surface.Root && surface.Mount != ModUiMount.CombatHud))
            {
                var paper = rect.gameObject.AddComponent<Image>();
                Skin(paper,"DialogScroll.Background_Center",new Color32(203,171,120,255));
                paper.color = ColorOf(node.Style.BackgroundColor,paper.color); paper.raycastTarget = false;
            }
            if (node.Kind == ModUiKind.Text) view.Label = Label(rect, node);
            if (node.Kind == ModUiKind.Button)
            {
                var background = rect.gameObject.AddComponent<Image>();
                Skin(background,"CommonButtons.BtnWhite",new Color32(223,207,177,255));
                if (background.sprite != null && node.Height > 0)
                    background.pixelsPerUnitMultiplier = background.sprite.rect.height / (float)node.Height;
                background.color = ColorOf(node.Style.BackgroundColor,background.color);
                view.Button = rect.gameObject.AddComponent<Button>();
                view.Control = view.Button;
                view.Button.targetGraphic = background;
                view.Button.navigation = new Navigation { mode = Navigation.Mode.None };
                var colors = view.Button.colors;
                colors.normalColor = Color.white;
                colors.highlightedColor = colors.selectedColor = new Color32(245,245,245,255);
                colors.pressedColor = new Color32(200,200,200,255);
                colors.disabledColor = new Color32(200,200,200,128);
                view.Button.colors = colors;
                view.Button.onClick.AddListener(() => surface.TryClick(node.Id));
                buttons.Add(node.Id);
                var label = Rect("Label", rect, 0, 0); Stretch(label);
                label.offsetMin = new Vector2(8, 4); label.offsetMax = new Vector2(-8, -4);
                view.Label = Label(label, node);
            }
            if (node.Kind == ModUiKind.Toggle)
            {
                float size = Mathf.Min(40, (float)node.Height);
                var box = Rect("Box", rect, size, size);
                box.anchorMin = box.anchorMax = new Vector2(0, .5f);
                box.anchoredPosition = new Vector2(size / 2, 0);
                var background = box.gameObject.AddComponent<Image>();
                Skin(background, "MiscSprites.checkboxOff", new Color32(48,31,20,255));
                background.color = ColorOf(node.Style.BackgroundColor, background.color);
                var mark = Rect("Check", box, 0, 0); Stretch(mark);
                var check = mark.gameObject.AddComponent<Image>();
                Skin(check, "MiscSprites.checkboxOn", new Color32(213,165,62,255));
                check.raycastTarget = false;
                view.Toggle = rect.gameObject.AddComponent<Toggle>();
                view.Control = view.Toggle;
                view.Toggle.targetGraphic = background; view.Toggle.graphic = check;
                view.Toggle.toggleTransition = Toggle.ToggleTransition.None;
                var hit = rect.gameObject.AddComponent<Image>(); hit.color = Color.clear;
                var label = Rect("Label", rect, 0, 0); Stretch(label);
                label.offsetMin = new Vector2(size + 8, 0);
                view.Label = Label(label, node);
                view.Toggle.onValueChanged.AddListener(value => { surface.TryChange(node.Id, value ? 1 : 0); UpdateWidget(node.Id); });
                view.Control.navigation = new Navigation { mode = Navigation.Mode.None };
                buttons.Add(node.Id);
            }
            if (node.Kind == ModUiKind.Slider)
            {
                var hit = rect.gameObject.AddComponent<Image>(); hit.color = Color.clear;
                var track = Rect("Track", rect, 0, 0); Stretch(track);
                track.anchorMin = new Vector2(0, .35f); track.anchorMax = new Vector2(1, .65f);
                track.offsetMin = new Vector2(12, 0); track.offsetMax = new Vector2(-12, 0);
                var background = track.gameObject.AddComponent<Image>();
                Skin(background, "SlidersSettings.SettingsEmpty", new Color32(48,31,20,255));
                background.color = ColorOf(node.Style.BackgroundColor, background.color);
                var fillRect = Rect("Fill", track, 0, 0); Stretch(fillRect);
                var fill = fillRect.gameObject.AddComponent<Image>();
                Skin(fill, "SlidersSettings.full", new Color32(213,165,62,255));
                fill.color = ColorOf(node.Style.FillColor, fill.color); fill.raycastTarget = false;
                var handleArea = Rect("HandleArea", rect, 0, 0); Stretch(handleArea);
                handleArea.offsetMin = new Vector2(12, 0); handleArea.offsetMax = new Vector2(-12, 0);
                var handle = Rect("Handle", handleArea, 24, 0); Stretch(handle); handle.sizeDelta = new Vector2(24, 0);
                var thumb = handle.gameObject.AddComponent<Image>();
                Skin(thumb, "SlidersSettings.slider", new Color32(213,165,62,255));
                view.Slider = rect.gameObject.AddComponent<Slider>(); view.Control = view.Slider;
                view.Slider.fillRect = fillRect; view.Slider.handleRect = handle; view.Slider.targetGraphic = thumb;
                view.Slider.minValue = 0; view.Slider.maxValue = 1;
                view.Slider.onValueChanged.AddListener(value => { surface.TryChange(node.Id, value); UpdateWidget(node.Id); });
                view.Control.navigation = new Navigation { mode = Navigation.Mode.None };
                buttons.Add(node.Id);
            }
            if (node.Kind == ModUiKind.Progress)
            {
                var background = rect.gameObject.AddComponent<Image>();
                Skin(background,"FightUI.HealthBar_Empty",new Color32(48,31,20,255));
                background.color = ColorOf(node.Style.BackgroundColor,background.color); background.raycastTarget = false;
                view.Fill = Rect("Fill", rect, 0, 0);
                Stretch(view.Fill);
                var fill = view.Fill.gameObject.AddComponent<Image>();
                Skin(fill,"FightUI.HealthBar_Full",new Color32(213,165,62,255));
                fill.color = ColorOf(node.Style.FillColor,fill.color); fill.raycastTarget = false;
            }
            if (node.Kind == ModUiKind.Scroll)
            {
                var viewport = Rect("Viewport", rect, 0, 0); Stretch(viewport);
                viewport.gameObject.AddComponent<RectMask2D>();
                // Transparent but raycastable so wheel/drag reaches ScrollRect.
                var hit = viewport.gameObject.AddComponent<Image>(); hit.color = Color.clear;
                var content = Build(node.Children[0], viewport);
                content.anchorMin = content.anchorMax = content.pivot = new Vector2(.5f, 1);
                content.anchoredPosition = Vector2.zero;
                var scroll = rect.gameObject.AddComponent<ScrollRect>();
                scroll.viewport = viewport; scroll.content = content;
                scroll.horizontal = false; scroll.vertical = true;
                scroll.movementType = ScrollRect.MovementType.Clamped;
            }
            else foreach (var child in node.Children) Build(child, rect);
            UpdateWidget(node.Id);
            return rect;
        }

        private void UpdateWidget(string id)
        {
            if (disposed || surface.IsClosed) return;
            var view = widgets[id]; var state = surface.Read(id);
            if (id == surface.Root.Id && GetComponent<Image>() is Image paper) paper.enabled = state.Visible;
            view.Rect.gameObject.SetActive(state.Visible);
            view.Group.interactable = state.Enabled;
            if (view.Label != null) view.Label.text = state.Text;
            if (view.Fill != null) view.Fill.anchorMax = new Vector2((float)state.Value, 1);
            if (view.Control != null) view.Control.interactable = state.Enabled;
            if (view.Toggle != null) view.Toggle.SetIsOnWithoutNotify(state.Value != 0);
            if (view.Slider != null) view.Slider.SetValueWithoutNotify((float)state.Value);
        }

        public void FitToSafeArea(float width, float height)
        {
            if (disposed || surface.IsClosed) return;
            width = Mathf.Max(0,width); height = Mathf.Max(0,height);
            var rect = GetComponent<RectTransform>();
            float scale = Mathf.Min(1,width/(float)surface.Root.Width,height/(float)surface.Root.Height);
            rect.localScale = Vector3.one * scale;
            float scaledWidth=(float)surface.Root.Width*scale, scaledHeight=(float)surface.Root.Height*scale;
            float minX=rect.pivot.x*scaledWidth, minY=rect.pivot.y*scaledHeight;
            float pivotX=Mathf.Clamp(rect.anchorMin.x*width+(float)surface.Placement.X,minX,Mathf.Max(minX,width-(1-rect.pivot.x)*scaledWidth));
            float pivotY=Mathf.Clamp(rect.anchorMin.y*height-(float)surface.Placement.Y,minY,Mathf.Max(minY,height-(1-rect.pivot.y)*scaledHeight));
            rect.anchoredPosition=new Vector2(pivotX-rect.anchorMin.x*width,pivotY-rect.anchorMin.y*height);
        }

        // Input is routed here only for the foreground surface by its coordinator.
        public bool MoveFocus(int direction)
        {
            if (disposed || surface.IsClosed || EventSystem.current == null || buttons.Count == 0) return false;
            var selected = EventSystem.current.currentSelectedGameObject;
            int start = buttons.FindIndex(id => widgets[id].Control.gameObject == selected);
            int step = direction < 0 ? -1 : 1;
            if (start < 0) start = step > 0 ? -1 : 0;
            for (int offset = 1; offset <= buttons.Count; offset++)
            {
                int index = (start + step * offset + buttons.Count * 2) % buttons.Count;
                string id = buttons[index];
                if (surface.CanInteract(id)) { widgets[id].Control.Select(); return true; }
            }
            return false;
        }

        public bool ActivateSelected()
        {
            if (disposed || surface.IsClosed || EventSystem.current == null) return false;
            var selected = EventSystem.current.currentSelectedGameObject;
            foreach (string id in buttons)
                if (widgets[id].Control.gameObject == selected)
                    return widgets[id].Toggle != null ? surface.TryChange(id, surface.Read(id).Value == 0 ? 1 : 0) : surface.TryClick(id);
            return false;
        }

        public bool AdjustSelected(int direction)
        {
            if (disposed || surface.IsClosed || EventSystem.current == null || direction == 0) return false;
            var selected = EventSystem.current.currentSelectedGameObject;
            foreach (string id in buttons)
                if (widgets[id].Control.gameObject == selected && widgets[id].Slider != null)
                    return surface.TryChange(id, Math.Max(0, Math.Min(1, surface.Read(id).Value + (direction < 0 ? -.05 : .05))));
            return false;
        }

        private void Release()
        { ReleaseView(true); }

        private void ReleaseView(bool destroy)
        {
            if (disposed) return;
            disposed = true;
            surface.Changed -= UpdateWidget; surface.Closed -= Release;
            if (EventSystem.current != null)
            {
                var selected = EventSystem.current.currentSelectedGameObject;
                if (selected != null && selected.transform.IsChildOf(transform))
                    EventSystem.current.SetSelectedGameObject(previousSelection != null && previousSelection.activeInHierarchy ? previousSelection : null);
            }
            widgets.Clear(); buttons.Clear(); previousSelection = null;
            gameObject.SetActive(false);
            if (destroy) Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (surface == null) return;
            ReleaseView(false);
            surface.Close(ModUiCloseReason.Destroyed);
        }
    }
}
