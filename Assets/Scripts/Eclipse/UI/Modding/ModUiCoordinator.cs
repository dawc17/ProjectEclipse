using System;
using System.Collections.Generic;
using Eclipse.Modding;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Eclipse.UI.Modding
{
    // Scene-owned canvas/input arbitration. The game bridge supplies native blocking
    // and routes input; this class does not poll keys or grant combat pause authority.
    public sealed class ModUiCoordinator : MonoBehaviour
    {
        private sealed class Entry
        {
            public ModUiSurface Surface;
            public GameObject Root;
            public RectTransform SafeArea;
            public CanvasGroup Group;
            public Canvas Canvas;
            public ModUiView View;
            public long Order;
        }
        private readonly ModUiLayerStack layers = new ModUiLayerStack();
        private readonly Dictionary<ModUiSurface, Entry> entries = new Dictionary<ModUiSurface, Entry>();
        private bool disposed, blocked;
        private long nextOrder;
        private EventSystem navigationOwner;
        private bool savedNavigation;
        private ModUiSurface focused;
        private Rect lastSafeArea;
        private Vector2Int lastScreen;
        public ModUiSurface Foreground => layers.Foreground;
        public bool CapturesInput => !disposed && layers.HasExclusiveInput;

        public static ModUiCoordinator Create()
        {
            var root = new GameObject("Mod UI coordinator", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 31010;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            var coordinator = root.AddComponent<ModUiCoordinator>();
            coordinator.layers.Changed += coordinator.Synchronize;
            return coordinator;
        }

        public void Attach(ModUiSurface surface)
        {
            if (disposed) throw new ObjectDisposedException(nameof(ModUiCoordinator));
            if (surface == null || surface.IsClosed) throw new ArgumentException("An open UI surface is required.");
            if (surface.IsMounted) throw new InvalidOperationException("UI surface already has a view.");
            var root = new GameObject(surface.Owner + ":" + surface.Id, typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup), typeof(GraphicRaycaster));
            root.transform.SetParent(transform, false);
            Stretch(root.GetComponent<RectTransform>());
            var entry = new Entry { Surface = surface, Root = root, Canvas = root.GetComponent<Canvas>(),
                Group = root.GetComponent<CanvasGroup>(), Order = nextOrder++ };
            try
            {
                entry.Canvas.overrideSorting = true;
                // Exclusive surfaces consume pointer input outside their content too.
                if (surface.Mount != ModUiMount.CombatHud)
                {
                    var backdrop = new GameObject("Backdrop", typeof(RectTransform), typeof(Image));
                    backdrop.transform.SetParent(root.transform, false); Stretch(backdrop.GetComponent<RectTransform>());
                    backdrop.GetComponent<Image>().color = new Color(0, 0, 0, .55f);
                }
                var safe = new GameObject("Safe area", typeof(RectTransform));
                safe.transform.SetParent(root.transform, false);
                entry.SafeArea = safe.GetComponent<RectTransform>();
                Stretch(entry.SafeArea);
                entry.View = ModUiView.Attach(surface, entry.SafeArea);
                entries.Add(surface, entry);
                layers.Add(surface);
                UpdateSafeAreas(true);
            }
            catch
            {
                entries.Remove(surface);
                root.SetActive(false); Destroy(root);
                surface.Close(ModUiCloseReason.Error);
                throw;
            }
        }

        public void SetNativeBlocked(bool value)
        {
            if (disposed) return;
            blocked = value;
            layers.SetBlocked(value);
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }

        private void Synchronize()
        {
            if (disposed) return;
            foreach (var entry in new List<Entry>(entries.Values))
            {
                if (entry.Surface.IsClosed)
                {
                    entries.Remove(entry.Surface);
                    if (entry.Root != null) { entry.Root.SetActive(false); Destroy(entry.Root); }
                    continue;
                }
                bool visible = !blocked && entry.Surface.Read(entry.Surface.Root.Id).Visible;
                entry.Root.SetActive(visible);
                bool foreground = visible && entry.Surface == layers.Foreground;
                entry.Group.interactable = entry.Group.blocksRaycasts = foreground;
            }
            // Compact ranks prevent sort-order overflow after repeated open/close.
            var ordered = new List<Entry>(entries.Values);
            ordered.Sort((a, b) => {
                int priority = ModUiLayerStack.Priority(a.Surface.Mount).CompareTo(ModUiLayerStack.Priority(b.Surface.Mount));
                return priority != 0 ? priority : a.Order.CompareTo(b.Order);
            });
            for (int i = 0; i < ordered.Count; i++) ordered[i].Canvas.sortingOrder = 31010 + i;
            UpdateNavigation();
            if (focused != layers.Foreground)
            {
                focused = layers.Foreground;
                if (CapturesInput && entries.TryGetValue(focused, out var foreground)) foreground.View.MoveFocus(1);
            }
        }

        private void UpdateNavigation()
        {
            var current = EventSystem.current;
            if (navigationOwner != null && (!CapturesInput || navigationOwner != current))
            {
                navigationOwner.sendNavigationEvents = savedNavigation;
                navigationOwner = null;
            }
            if (CapturesInput && current != null && navigationOwner == null)
            {
                navigationOwner = current; savedNavigation = current.sendNavigationEvents;
                current.sendNavigationEvents = false;
            }
        }

        public bool MoveFocus(int direction)
        {
            if (!CapturesInput || !entries.TryGetValue(layers.Foreground, out var entry)) return false;
            return entry.View.MoveFocus(direction);
        }

        public bool ActivateSelected()
        {
            if (!CapturesInput || !entries.TryGetValue(layers.Foreground, out var entry)) return false;
            return entry.View.ActivateSelected();
        }

        public bool Back() => !disposed && layers.Back();

        private void UpdateSafeAreas(bool force = false)
        {
            var screen = new Vector2Int(Screen.width, Screen.height);
            var safe = Screen.safeArea;
            if (!force && screen == lastScreen && safe == lastSafeArea) return;
            lastScreen = screen; lastSafeArea = safe;
            if (screen.x <= 0 || screen.y <= 0) return;
            foreach (var entry in entries.Values)
            {
                entry.SafeArea.anchorMin = new Vector2(Mathf.Clamp01(safe.xMin / screen.x), Mathf.Clamp01(safe.yMin / screen.y));
                entry.SafeArea.anchorMax = new Vector2(Mathf.Clamp01(safe.xMax / screen.x), Mathf.Clamp01(safe.yMax / screen.y));
                entry.SafeArea.offsetMin = entry.SafeArea.offsetMax = Vector2.zero;
            }
            Canvas.ForceUpdateCanvases();
            foreach (var entry in entries.Values)
            {
                entry.View.FitToSafeArea(entry.SafeArea.rect.width, entry.SafeArea.rect.height);
            }
        }

        private void Update()
        {
            if (disposed) return;
            UpdateSafeAreas(); UpdateNavigation();
        }

        private void OnDestroy()
        {
            disposed = true;
            layers.Changed -= Synchronize;
            layers.Dispose();
            entries.Clear(); focused = null;
            if (navigationOwner != null) navigationOwner.sendNavigationEvents = savedNavigation;
            navigationOwner = null;
        }
    }
}
