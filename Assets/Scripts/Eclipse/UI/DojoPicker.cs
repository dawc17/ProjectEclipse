using System.Collections.Generic;
using Eclipse.Modding;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // The dojo picker a mod declares with sf2.locations.dojo_picker. A large medallion of the
    // focused dojo with its name, over an ink scrim and a soft aura of the same art, and a
    // strip of medallions below that glides with the focus. Arrows, A/D, the mouse wheel and
    // the gamepad move the focus; Enter, A or a click on the focused medallion enters that
    // dojo; Esc or B closes. Everything animates on unscaled time.
    public sealed class DojoPicker : MonoBehaviour
    {
        private static readonly Color Ink = new Color32(14, 10, 8, 255);
        private static readonly Color Paper = new Color32(223, 207, 177, 255);
        private static readonly Color Red = new Color32(147, 39, 31, 255);
        private static readonly Color Gold = new Color32(214, 170, 78, 255);
        private const float OpenSeconds = .45f, CloseSeconds = .25f, SwapSeconds = .3f, Spacing = 104f;
        private const float StripY = -250f, HeroY = 36f, HeroSize = 330f;

        private sealed class Entry
        {
            public DefinitionId Location;
            public string Name;
            public Sprite Preview;
            public RectTransform Item;
            public Image Image;
        }

        private static DojoPicker current;
        private readonly List<Entry> entries = new List<Entry>();
        private System.IDisposable inputLock;
        private Font font;
        private CanvasGroup group;
        private Image scrim, heroIn, heroOut, auraIn, auraOut;
        private RawImage halo;
        private Text title, nameIn, nameOut, badge;
        private InkStroke underline;
        private RectTransform root, strip;
        private int focus, selected = -1, swapDirection;
        private float openedAt, swapAt = -99f, scroll, scrollVelocity, leaveAt = -1f, chooseAt = -1f, badgeAlpha;
        private float repeatAt;
        private bool restoreNavigation;
        private int heldDirection;
        private bool choosing;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSession() { current = null; }

        public static bool IsOpen => current != null;

        // Opens the picker behind a dojo button; false when the button has no declared picker.
        public static bool TryOpen(string buttonName)
        {
            ModDojoPicker picker;
            try { picker = ModRuntime.FindDojoPicker(buttonName); }
            catch (System.Exception) { return false; }
            if (picker == null) return false;
            if (current != null) return true;
            var host = new GameObject("Eclipse Dojo Picker", typeof(RectTransform));
            var view = host.AddComponent<DojoPicker>();
            if (!view.Build(picker)) { Destroy(host); return true; }
            current = view;
            return true;
        }

        // For editor validators: the open picker's choices, and a scripted pick.
        public static string[] OpenChoices
        {
            get
            {
                if (current == null) return new string[0];
                var result = new string[current.entries.Count];
                for (int i = 0; i < result.Length; i++) result[i] = current.entries[i].Location.ToString();
                return result;
            }
        }

        public static bool PickForValidation(int index)
        {
            if (current == null || index < 0 || index >= current.entries.Count) return false;
            current.openedAt = Mathf.Min(current.openedAt, Time.unscaledTime - 1f);
            current.Focus(index, 1);
            current.Choose();
            return current.choosing;
        }

        // Esc from BackKeyManager (through ModUiGameBridge) closes the picker first.
        public static bool HandleBack()
        {
            if (current == null) return false;
            current.Close();
            return true;
        }

        private bool Build(ModDojoPicker picker)
        {
            var saved = ModRuntime.SelectedDojo;
            foreach (var choice in picker.Choices)
            {
                if (!ModRuntime.CanChooseDojo(choice.Location)) continue;
                Sprite sprite = null;
                try { sprite = ModRuntime.Host.TypedAssets.LoadSprite(choice.Preview); }
                catch (System.Exception error) { Debug.LogWarning("[DojoPicker] Preview failed for " + choice.Location + ": " + error.Message); }
                if (sprite == null) continue;
                if (choice.Location.Equals(saved)) selected = entries.Count;
                entries.Add(new Entry { Location = choice.Location, Preview = sprite,
                    Name = ModRuntime.LocalizedText(choice.Name, choice.Location.LocalId) });
            }
            if (entries.Count == 0) { Debug.LogWarning("[DojoPicker] No installed dojo choices to show."); return false; }
            focus = Mathf.Max(0, selected);
            scroll = focus;

            font = Resources.Load<Font>("ui/fonts/AGOpusBold") ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 31500;
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            gameObject.AddComponent<GraphicRaycaster>();
            group = gameObject.AddComponent<CanvasGroup>();
            group.alpha = 0f;

            scrim = Stretch("Scrim").gameObject.AddComponent<Image>();
            scrim.color = new Color(Ink.r, Ink.g, Ink.b, .84f);

            root = Node(transform, "Content", 0, 0, 1280, 720);
            // A faint, oversized copy of the focused medallion tints the room around it.
            auraOut = Picture(root, "Aura out", 0, HeroY, 980, 980);
            auraIn = Picture(root, "Aura in", 0, HeroY, 980, 980);
            halo = Node(root, "Halo", 0, HeroY, HeroSize * 1.34f, HeroSize * 1.34f).gameObject.AddComponent<RawImage>();
            halo.texture = HaloTexture();
            halo.raycastTarget = false;
            heroOut = Picture(root, "Medallion out", 0, HeroY, HeroSize, HeroSize);
            heroIn = Picture(root, "Medallion", 0, HeroY, HeroSize, HeroSize);
            heroIn.raycastTarget = true;
            var heroButton = heroIn.gameObject.AddComponent<Button>();
            heroButton.transition = Selectable.Transition.None;
            heroButton.onClick.AddListener(Choose);

            string heading = picker.Title.HasValue ? ModRuntime.LocalizedText(picker.Title.Value, "Choose your dojo") : "Choose your dojo";
            title = Label(root, heading.ToUpperInvariant(), 0, 300, 900, 56, 40, Paper);
            underline = Node(root, "Underline", 0, 266, 380, 14).gameObject.AddComponent<InkStroke>();
            underline.color = new Color(Red.r, Red.g, Red.b, .95f);
            underline.raycastTarget = false;
            underline.Seed = 4127;
            underline.Fill = 0f;

            nameOut = Label(root, "", 0, -162, 900, 50, 34, Paper);
            nameIn = Label(root, "", 0, -162, 900, 50, 34, Paper);
            badge = Label(root, "CURRENT DOJO", 0, -196, 600, 26, 16, Gold);

            Arrow("<", -268, -1);
            Arrow(">", 268, 1);

            strip = Node(root, "Strip", 0, StripY, 1280, 110);
            for (int i = 0; i < entries.Count; i++)
            {
                int index = i;
                var item = Picture(strip, entries[i].Name, 0, 0, 76, 76);
                item.sprite = entries[i].Preview;
                item.raycastTarget = true;
                var button = item.gameObject.AddComponent<Button>();
                button.transition = Selectable.Transition.None;
                button.onClick.AddListener(() => { if (index == focus) Choose(); else Focus(index, index > focus ? 1 : -1); });
                entries[i].Item = item.rectTransform;
                entries[i].Image = item;
            }

            Hint("ESC", "BACK", -150, Close);
            Hint("ENTER", "ENTER DOJO", 150, Choose);

            heroIn.sprite = auraIn.sprite = entries[focus].Preview;
            nameIn.text = entries[focus].Name.ToUpperInvariant();
            heroOut.enabled = auraOut.enabled = false;
            nameOut.text = string.Empty;
            badgeAlpha = focus == selected ? 1f : 0f;
            inputLock = Eclipse.UI.Modding.ModUiGameBridge.AcquirePresentationBlock();
            // Keys are read here; the event system must not also submit a hovered hint.
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                restoreNavigation = EventSystem.current.sendNavigationEvents;
                EventSystem.current.sendNavigationEvents = false;
            }
            openedAt = Time.unscaledTime;
            EclipseUiAudio.Play(UiSound.Open);
            return true;
        }

        private void Update()
        {
            float now = Time.unscaledTime, dt = Time.unscaledDeltaTime;
            if (leaveAt >= 0f)
            {
                float t = Mathf.Clamp01((now - leaveAt) / CloseSeconds);
                group.alpha = 1f - Smooth(t);
                root.localScale = Vector3.one * (1f + .03f * t);
                if (t >= 1f) Destroy(gameObject);
                return;
            }
            float open = Mathf.Clamp01((now - openedAt) / OpenSeconds);
            group.alpha = Smooth(open);
            group.blocksRaycasts = !choosing;
            if (!choosing) ReadInput(now);

            // The strip glides to the focus; each medallion grows and brightens near it.
            scroll = Mathf.SmoothDamp(scroll, focus, ref scrollVelocity, .12f, Mathf.Infinity, dt);
            for (int i = 0; i < entries.Count; i++)
            {
                float offset = i - scroll, distance = Mathf.Abs(offset);
                float stagger = Mathf.Clamp01((now - openedAt - .12f - Mathf.Min(distance, 6f) * .04f) / .3f);
                float rise = 1f - Back(stagger);
                var item = entries[i].Item;
                item.anchoredPosition = new Vector2(offset * Spacing, -26f * rise);
                float scale = Mathf.Lerp(1.3f, .78f, Mathf.Clamp01(distance)) * Mathf.Lerp(1f, .9f, Mathf.Clamp01(distance - 1f));
                item.localScale = Vector3.one * scale;
                var color = i == selected ? Color.Lerp(Color.white, Gold, .35f) : Color.white;
                color.a = Mathf.Clamp01(1f - Mathf.Max(0f, distance - .4f) * .2f) * Mathf.Clamp01(stagger * 1.5f) *
                          Mathf.Clamp01(5.6f - distance);
                entries[i].Image.color = color;
                entries[i].Image.raycastTarget = distance < 5f;
            }

            // Hero: the incoming medallion slides in from the side it came from, the old one out.
            float swap = Mathf.Clamp01((now - swapAt) / SwapSeconds);
            float ease = 1f - (1f - swap) * (1f - swap) * (1f - swap);
            float entrance = Back(open);
            float pulse = chooseAt >= 0f ? Mathf.Sin(Mathf.Clamp01((now - chooseAt) / .3f) * Mathf.PI) * .1f : 0f;
            float breathe = 1f + .015f * Mathf.Sin(now * 1.7f);
            Place(heroIn, swapDirection * 90f * (1f - ease), Mathf.Lerp(.9f, 1f, ease) * Mathf.Lerp(.82f, 1f, entrance) * (1f + pulse), ease);
            Place(heroOut, -swapDirection * 90f * ease, Mathf.Lerp(1f, .88f, ease), 1f - ease);
            SetAlpha(auraIn, .11f * ease);
            SetAlpha(auraOut, .11f * (1f - ease));
            auraIn.rectTransform.localEulerAngles = auraOut.rectTransform.localEulerAngles = new Vector3(0f, 0f, now * 2.5f);
            halo.rectTransform.localScale = Vector3.one * breathe * (1f + pulse * 1.6f);
            halo.color = new Color(Paper.r, Paper.g, Paper.b, (.28f + pulse * 2.4f) * Smooth(open));
            Slide(nameIn, swapDirection * 40f * (1f - ease), ease);
            Slide(nameOut, -swapDirection * 40f * ease, 1f - ease);
            badgeAlpha = Mathf.MoveTowards(badgeAlpha, focus == selected ? 1f : 0f, dt / .2f);
            badge.color = new Color(Gold.r, Gold.g, Gold.b, badgeAlpha);

            float heading = Smooth(Mathf.Clamp01((now - openedAt - .05f) / .4f));
            title.rectTransform.anchoredPosition = new Vector2(0f, 300f + 18f * (1f - heading));
            title.color = new Color(Paper.r, Paper.g, Paper.b, heading);
            underline.Fill = Smooth(Mathf.Clamp01((now - openedAt - .18f) / .45f));

            if (chooseAt >= 0f && now - chooseAt >= .3f && leaveAt < 0f) Enter();
        }

        private void ReadInput(float now)
        {
            if (now - openedAt < .15f) return;
            var keyboard = UnityEngine.Input.GetKey(KeyCode.LeftArrow) || UnityEngine.Input.GetKey(KeyCode.A) ? -1 :
                UnityEngine.Input.GetKey(KeyCode.RightArrow) || UnityEngine.Input.GetKey(KeyCode.D) ? 1 : 0;
            float pad = 0f;
            try
            {
                var stick = GamePad.GetStick(GamePad.Stick.Dpad, GamePad.Player.Any);
                var left = GamePad.GetStick(GamePad.Stick.LeftStick, GamePad.Player.Any);
                pad = Mathf.Abs(left.x) > Mathf.Abs(stick.x) ? left.x : stick.x;
                if (GamePad.GetButtonDown(GamePad.Button.A, GamePad.Player.Any)) { Choose(); return; }
                if (GamePad.GetButtonDown(GamePad.Button.B, GamePad.Player.Any)) { Close(); return; }
            }
            catch (System.Exception) { }
            int direction = keyboard != 0 ? keyboard : Mathf.Abs(pad) > .6f ? (pad > 0f ? 1 : -1) : 0;
            // First press moves at once; holding repeats.
            if (direction != 0 && (direction != heldDirection || now >= repeatAt))
            {
                repeatAt = now + (direction != heldDirection ? .38f : .12f);
                Step(direction);
            }
            heldDirection = direction;
            float wheel = UnityEngine.Input.mouseScrollDelta.y;
            if (Mathf.Abs(wheel) > .01f) Step(wheel > 0f ? -1 : 1);
            if (UnityEngine.Input.GetKeyDown(KeyCode.Return) || UnityEngine.Input.GetKeyDown(KeyCode.KeypadEnter) ||
                UnityEngine.Input.GetKeyDown(KeyCode.Space)) Choose();
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Backspace)) Close();
        }

        private void Step(int direction)
        {
            int next = Mathf.Clamp(focus + direction, 0, entries.Count - 1);
            if (next != focus) Focus(next, direction);
        }

        private void Focus(int index, int direction)
        {
            if (choosing || index == focus || index < 0 || index >= entries.Count) return;
            heroOut.sprite = heroIn.sprite; auraOut.sprite = auraIn.sprite; nameOut.text = nameIn.text;
            heroOut.enabled = auraOut.enabled = true;
            focus = index;
            heroIn.sprite = auraIn.sprite = entries[index].Preview;
            nameIn.text = entries[index].Name.ToUpperInvariant();
            swapDirection = direction;
            swapAt = Time.unscaledTime;
            EclipseUiAudio.Play(UiSound.Focus);
        }

        private void Choose()
        {
            if (choosing || leaveAt >= 0f || Time.unscaledTime - openedAt < .15f) return;
            // The current dojo needs no reload.
            if (focus == selected) { Close(); return; }
            choosing = true;
            chooseAt = Time.unscaledTime;
            EclipseUiAudio.Play(UiSound.Begin);
        }

        // After the medallion's pulse: save and reload. The menu scene fade captures this
        // frame, so the picker dissolves into the transition.
        private void Enter()
        {
            // Scene navigation refuses while any presentation holds the input lock, this one included.
            Release();
            if (!ModRuntime.TryChooseDojo(entries[focus].Location)) Debug.LogWarning("[DojoPicker] The dojo could not be opened now.");
            Close(false);
        }

        private void Close() { Close(true); }

        private void Close(bool sound)
        {
            if (leaveAt >= 0f) return;
            if (sound) EclipseUiAudio.Play(UiSound.Back);
            leaveAt = Time.unscaledTime;
            group.blocksRaycasts = false;
            if (current == this) current = null;
            Release();
        }

        private void Release()
        {
            inputLock?.Dispose();
            inputLock = null;
            if (restoreNavigation && EventSystem.current != null) EventSystem.current.sendNavigationEvents = true;
            restoreNavigation = false;
        }

        private void OnDestroy()
        {
            if (current == this) current = null;
            Release();
        }

        // --- Construction helpers ---------------------------------------------------------

        private RectTransform Stretch(string name)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(transform, false);
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            return rect;
        }

        // Centre-anchored node; x/y are offsets from the screen centre, up positive.
        private static RectTransform Node(Transform parent, string name, float x, float y, float w, float h)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(w, h);
            return rect;
        }

        private static Image Picture(Transform parent, string name, float x, float y, float w, float h)
        {
            var image = Node(parent, name, x, y, w, h).gameObject.AddComponent<Image>();
            image.preserveAspect = true;
            image.raycastTarget = false;
            return image;
        }

        private Text Label(Transform parent, string text, float x, float y, float w, float h, int size, Color color)
        {
            var label = Node(parent, text, x, y, w, h).gameObject.AddComponent<Text>();
            label.font = font; label.text = text; label.fontSize = size; label.color = color;
            label.alignment = TextAnchor.MiddleCenter; label.raycastTarget = false;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            var shadow = label.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, .7f);
            shadow.effectDistance = new Vector2(1.5f, -2f);
            return label;
        }

        private void Arrow(string glyph, float x, int direction)
        {
            var rect = Node(root, "Arrow " + glyph, x, HeroY, 80, 120);
            var hit = rect.gameObject.AddComponent<Image>();
            hit.color = Color.clear;
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = hit;
            var label = Label(rect, glyph, 0, 0, 80, 120, 64, Paper);
            EclipseUiButton.Attach(button, null, label, Color.clear, Color.clear,
                new Color(Paper.r, Paper.g, Paper.b, .55f), Paper, 5f * direction, .14f, .12f);
            button.onClick.AddListener(() => Step(direction));
        }

        private void Hint(string key, string action, float x, System.Action click)
        {
            var rect = Node(root, action, x, -318, 260, 40);
            var hit = rect.gameObject.AddComponent<Image>();
            hit.color = Color.clear;
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = hit;
            var label = Label(rect, key + "   " + action, 0, 0, 260, 40, 19, Paper);
            label.supportRichText = true;
            label.text = "<color=#D6AA4E>" + key + "</color>   " + action;
            EclipseUiButton.Attach(button, null, label, Color.clear, Color.clear,
                new Color(Paper.r, Paper.g, Paper.b, .7f), Paper, 0f, .06f);
            button.onClick.AddListener(() => click());
        }

        private static void Place(Image image, float x, float scale, float alpha)
        {
            var rect = image.rectTransform;
            rect.anchoredPosition = new Vector2(x, HeroY);
            rect.localScale = Vector3.one * scale;
            SetAlpha(image, alpha);
        }

        private static void Slide(Text text, float x, float alpha)
        {
            text.rectTransform.anchoredPosition = new Vector2(x, -162f);
            var color = text.color; color.a = alpha; text.color = color;
        }

        private static void SetAlpha(Graphic graphic, float alpha)
        {
            var color = graphic.color; color.a = alpha; graphic.color = color;
            graphic.enabled = alpha > .001f;
        }

        private static float Smooth(float t) { return t * t * (3f - 2f * t); }

        // Ease-out with a small overshoot.
        private static float Back(float t)
        {
            const float s = 1.6f;
            t -= 1f;
            return t * t * ((s + 1f) * t + s) + 1f;
        }

        private static Texture2D haloTexture;

        // A soft ring behind the medallion, generated once.
        private static Texture2D HaloTexture()
        {
            if (haloTexture != null) return haloTexture;
            const int Size = 128;
            haloTexture = new Texture2D(Size, Size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            var pixels = new Color32[Size * Size];
            for (int y = 0; y < Size; y++)
                for (int x = 0; x < Size; x++)
                {
                    float dx = (x + .5f) / Size * 2f - 1f, dy = (y + .5f) / Size * 2f - 1f;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    float a = Mathf.Exp(-Mathf.Pow((d - .74f) / .2f, 2f)) * Mathf.Clamp01((1f - d) * 6f);
                    pixels[y * Size + x] = new Color(1f, 1f, 1f, a);
                }
            haloTexture.SetPixels32(pixels);
            haloTexture.Apply();
            return haloTexture;
        }
    }
}
