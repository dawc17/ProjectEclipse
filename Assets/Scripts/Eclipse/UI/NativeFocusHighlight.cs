using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // Keyboard and gamepad focus for the recovered map, shop and profile screens. Their
    // buttons take Unity navigation, but the selected state looked like the normal one, so
    // nothing showed what Enter or A would press. A gold ring now glides to the selected
    // control while navigating, and the first navigation press with nothing selected picks
    // the control nearest the screen centre. Pointer use hides the ring. The dojo and
    // fights are excluded: there the same keys move the fighter.
    public sealed class NativeFocusHighlight : MonoBehaviour
    {
        private static readonly Color Gold = new Color32(214, 170, 78, 255);
        private const float Padding = 10f;
        private static NativeFocusHighlight instance;
        private RectTransform ring;
        private Image image;
        private CanvasGroup group;
        private Rect shown;
        private GameObject target;
        private bool navigating;
        private float visible;
        private Vector3 lastMouse;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Boot()
        {
            if (instance != null || Application.isBatchMode) return;
            var host = new GameObject("Eclipse Focus Highlight", typeof(RectTransform));
            DontDestroyOnLoad(host);
            instance = host.AddComponent<NativeFocusHighlight>();
        }

        private void Awake()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 30500;
            group = gameObject.AddComponent<CanvasGroup>();
            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;
            ring = new GameObject("Ring", typeof(RectTransform)).GetComponent<RectTransform>();
            ring.SetParent(transform, false);
            ring.anchorMin = ring.anchorMax = ring.pivot = Vector2.zero;
            image = ring.gameObject.AddComponent<Image>();
            image.sprite = RingSprite();
            image.type = Image.Type.Sliced;
            image.color = Gold;
            image.raycastTarget = false;
            lastMouse = UnityEngine.Input.mousePosition;
        }

        private static bool Eligible()
        {
            if (TitleScreen.IsOpen || DojoPicker.IsOpen || ControlLayoutEditor.IsOpen || GameSessionRestart.IsRestarting) return false;
            if (Eclipse.UI.Modding.ModUiGameBridge.BlocksGameplayInput) return false;
            try
            {
                if (Nekki.SF2.GUI.Scenes.LoaderScene.get_Current() != null) return false;
                var module = Module.GetInstance();
                var screen = module == null ? ScreenType.ModuleNone : module.GetCurrentScreenType();
                return screen == ScreenType.ModuleMap || screen == ScreenType.ModuleShop || screen == ScreenType.ModuleProfile;
            }
            catch (System.Exception) { return false; }
        }

        private void Update()
        {
            var events = EventSystem.current;
            bool eligible = events != null && Eligible();
            if (eligible) ReadDevice(events);
            GameObject selected = eligible && navigating ? events.currentSelectedGameObject : null;
            var rect = selected == null ? null : selected.transform as RectTransform;
            bool live = rect != null && selected.activeInHierarchy && selected.GetComponent<EclipseUiButton>() == null &&
                        TryScreenRect(rect, out var screen);
            float dt = Time.unscaledDeltaTime;
            visible = Mathf.MoveTowards(visible, live ? 1f : 0f, dt / .12f);
            group.alpha = visible * (.75f + .25f * Mathf.Sin(Time.unscaledTime * 5f));
            if (!live) { if (visible <= 0f) target = null; return; }
            TryScreenRect(rect, out var wanted);
            wanted = new Rect(wanted.x - Padding, wanted.y - Padding, wanted.width + Padding * 2f, wanted.height + Padding * 2f);
            if (selected != target || visible < .05f) { shown = wanted; target = selected; }
            float follow = 1f - Mathf.Exp(-dt * 18f);
            shown = new Rect(Vector2.Lerp(shown.position, wanted.position, follow), Vector2.Lerp(shown.size, wanted.size, follow));
            float scale = 1f / Mathf.Max(.0001f, GetComponent<Canvas>().scaleFactor);
            ring.anchoredPosition = shown.position * scale;
            ring.sizeDelta = shown.size * scale;
        }

        // Navigation keys or a gamepad show the ring; moving or clicking the mouse hides it.
        private void ReadDevice(EventSystem events)
        {
            Vector3 mouse = UnityEngine.Input.mousePosition;
            if ((mouse - lastMouse).sqrMagnitude > 4f || UnityEngine.Input.GetMouseButtonDown(0) || UnityEngine.Input.touchCount > 0)
                navigating = false;
            lastMouse = mouse;
            bool pressed = UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) || UnityEngine.Input.GetKeyDown(KeyCode.DownArrow) ||
                           UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow) || UnityEngine.Input.GetKeyDown(KeyCode.RightArrow) ||
                           UnityEngine.Input.GetKeyDown(KeyCode.Tab);
            try
            {
                var dpad = GamePad.GetStick(GamePad.Stick.Dpad, GamePad.Player.Any);
                var stick = GamePad.GetStick(GamePad.Stick.LeftStick, GamePad.Player.Any);
                pressed |= dpad.sqrMagnitude > .36f || stick.sqrMagnitude > .36f;
            }
            catch (System.Exception) { }
            if (!pressed) return;
            navigating = true;
            var current = events.currentSelectedGameObject;
            if (current == null || !current.activeInHierarchy) SelectNearestCentre(events);
        }

        private static void SelectNearestCentre(EventSystem events)
        {
            Selectable best = null;
            float bestDistance = float.MaxValue;
            var centre = new Vector2(Screen.width * .5f, Screen.height * .5f);
            foreach (var candidate in Selectable.allSelectablesArray)
            {
                if (candidate == null || !candidate.IsInteractable() || !candidate.gameObject.activeInHierarchy) continue;
                if (candidate.navigation.mode == Navigation.Mode.None) continue;
                if (!(candidate.transform is RectTransform rect) || !TryScreenRect(rect, out var screen)) continue;
                float distance = (screen.center - centre).sqrMagnitude;
                if (distance < bestDistance) { bestDistance = distance; best = candidate; }
            }
            if (best != null) events.SetSelectedGameObject(best.gameObject);
        }

        private static readonly Vector3[] Corners = new Vector3[4];

        private static bool TryScreenRect(RectTransform rect, out Rect screen)
        {
            screen = default;
            var canvas = rect.GetComponentInParent<Canvas>();
            if (canvas == null) return false;
            var camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            rect.GetWorldCorners(Corners);
            Vector2 min = new Vector2(float.MaxValue, float.MaxValue), max = new Vector2(float.MinValue, float.MinValue);
            for (int i = 0; i < 4; i++)
            {
                Vector2 point = RectTransformUtility.WorldToScreenPoint(camera, Corners[i]);
                min = Vector2.Min(min, point); max = Vector2.Max(max, point);
            }
            screen = new Rect(min, max - min);
            return screen.width > 2f && screen.height > 2f && max.x > 0f && max.y > 0f && min.x < Screen.width && min.y < Screen.height;
        }

        private static Sprite ringSprite;

        // A rounded outline, nine-sliced so any control size keeps a crisp 5 px edge.
        private static Sprite RingSprite()
        {
            if (ringSprite != null) return ringSprite;
            const int Size = 64, Radius = 20;
            const float Width = 5f;
            var texture = new Texture2D(Size, Size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            var pixels = new Color32[Size * Size];
            for (int y = 0; y < Size; y++)
                for (int x = 0; x < Size; x++)
                {
                    float px = x + .5f, py = y + .5f;
                    float cx = Mathf.Clamp(px, Radius, Size - Radius), cy = Mathf.Clamp(py, Radius, Size - Radius);
                    float d = Radius - Mathf.Sqrt((px - cx) * (px - cx) + (py - cy) * (py - cy));
                    float a = Mathf.Clamp01(d + .5f) * Mathf.Clamp01(Width - d + .5f);
                    pixels[y * Size + x] = new Color(1f, 1f, 1f, a);
                }
            texture.SetPixels32(pixels);
            texture.Apply();
            ringSprite = Sprite.Create(texture, new Rect(0, 0, Size, Size), new Vector2(.5f, .5f), 100f, 0,
                SpriteMeshType.FullRect, new Vector4(Radius + 2, Radius + 2, Radius + 2, Radius + 2));
            return ringSprite;
        }
    }
}
