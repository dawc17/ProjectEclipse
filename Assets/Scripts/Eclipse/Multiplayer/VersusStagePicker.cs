using System;
using System.Collections.Generic;
using Eclipse.Input;
using Eclipse.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Multiplayer
{
    /// <summary>
    /// The arena picker: the dojo picker's modal (title, large name, gliding strip,
    /// arrows, Enter/Esc) over every roster arena plus Random. The live backdrop behind
    /// it follows the focused arena, so the preview is the arena itself.
    /// </summary>
    public sealed class VersusStagePicker : MonoBehaviour
    {
        private const float Spacing = 214f;
        private const float StripY = -236f;
        private static readonly Color Ink = new Color32(14, 10, 8, 255);
        private static readonly Color Paper = new Color32(223, 207, 177, 255);
        private static readonly Color Red = new Color32(147, 39, 31, 255);
        private static readonly Color Gold = new Color32(214, 170, 78, 255);
        private static VersusStagePicker _open;

        private sealed class Choice
        {
            public string Id, Name;
            public RawImage Picture;
            public bool Requested;
            public RectTransform Card;
        }

        private readonly List<Choice> _choices = new List<Choice>();
        private Action<string> _picked;
        private Action _cancelled;
        private string _current;
        private int _focus;
        private float _glide;
        private float _previewAt = -1f;
        private Font _font;
        private RectTransform _root, _strip;
        private Text _name, _count;
        private GameObject _badge;
        private CanvasGroup _group;
        private float _openedAt;
        private bool _closing;
        private int _held;
        private bool _navigation = true;
        private float _repeatAt;

        public static bool IsOpen => _open != null;

        /// <summary>Opens the picker on <paramref name="current"/>; <paramref name="allowRandom"/> adds a Random entry.</summary>
        public static void Open(string current, bool allowRandom, Action<string> picked, Action cancelled = null)
        {
            if (_open != null) Destroy(_open.gameObject);
            var host = new GameObject("Eclipse Arena Picker");
            DontDestroyOnLoad(host);
            _open = host.AddComponent<VersusStagePicker>();
            // Only the arena itself shows behind the picker.
            LocalVersusMenu.Ensure().SetPageHidden(true);
            _open.Build(current, allowRandom, picked, cancelled);
        }

        private void Build(string current, bool allowRandom, Action<string> picked, Action cancelled)
        {
            _picked = picked;
            _cancelled = cancelled;
            _current = current;
            _font = Resources.Load<Font>("ui/fonts/AGOpusBold") ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var canvas = gameObject.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 32758;
            var scaler = gameObject.AddComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720); scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            gameObject.AddComponent<GraphicRaycaster>();
            _root = Node(transform, "Picker", Vector2.zero, Vector2.zero, true);
            _group = _root.gameObject.AddComponent<CanvasGroup>();
            _group.alpha = 0f;
            var scrim = _root.gameObject.AddComponent<Image>(); scrim.color = new Color(Ink.r, Ink.g, Ink.b, .38f);
            // Darker toward the strip, so cards read over any arena.
            var foot = Node(_root, "Foot shade", new Vector2(0, 0), new Vector2(0, 330), false, new Vector2(.5f, 0), new Vector2(0, 0), new Vector2(1, 0));
            var footImage = foot.gameObject.AddComponent<Image>(); footImage.color = new Color(Ink.r, Ink.g, Ink.b, .55f); footImage.raycastTarget = false;

            var title = Label(_root, "CHOOSE ARENA", 34, Paper, TextAnchor.MiddleCenter, new Vector2(0, 286), new Vector2(700, 50));
            var underline = Node(_root, "Underline", new Vector2(0, 256), new Vector2(360, 16), false);
            var stroke = underline.gameObject.AddComponent<InkStroke>(); stroke.color = Red; stroke.raycastTarget = false; stroke.Seed = 404;
            _name = Label(_root, "", 64, Paper, TextAnchor.MiddleCenter, new Vector2(0, 40), new Vector2(1100, 90));
            _count = Label(_root, "", 18, new Color(Paper.r, Paper.g, Paper.b, .7f), TextAnchor.MiddleCenter, new Vector2(0, -16), new Vector2(600, 28));
            var badge = Node(_root, "Current", new Vector2(0, 104), new Vector2(170, 30), false);
            var badgeDisc = badge.gameObject.AddComponent<Image>(); badgeDisc.color = new Color(Gold.r, Gold.g, Gold.b, .92f); badgeDisc.raycastTarget = false;
            Label(badge, "CURRENT ARENA", 15, Ink, TextAnchor.MiddleCenter, Vector2.zero, new Vector2(170, 30));
            _badge = badge.gameObject;

            _strip = Node(_root, "Strip", new Vector2(0, StripY), new Vector2(10, 10), false);
            if (allowRandom) _choices.Add(new Choice { Id = VersusRoster.RandomArena, Name = "Random" });
            foreach (var arena in VersusRoster.Arenas) _choices.Add(new Choice { Id = arena.Id, Name = arena.Name });
            for (int i = 0; i < _choices.Count; i++) BuildCard(i);
            _focus = Math.Max(0, _choices.FindIndex(choice => choice.Id == current));
            _glide = _focus;

            Arrow(new Vector2(-560, StripY), "<", () => Move(-1));
            Arrow(new Vector2(560, StripY), ">", () => Move(1));
            Label(_root, "<color=#D6AA4E>A  D</color>  Browse      <color=#D6AA4E>Enter</color>  Choose      <color=#D6AA4E>Esc</color>  Cancel", 16,
                new Color(Paper.r, Paper.g, Paper.b, .8f), TextAnchor.MiddleCenter, new Vector2(0, -330), new Vector2(900, 26)).supportRichText = true;
            _openedAt = Time.unscaledTime;
            // The menu underneath must not act on the same keys and pad buttons.
            var events = UnityEngine.EventSystems.EventSystem.current;
            if (events != null)
            {
                _navigation = events.sendNavigationEvents;
                events.SetSelectedGameObject(null);
                events.sendNavigationEvents = false;
            }
            EclipseUiAudio.Play(UiSound.Open);
            Focus(_focus, silent: true);
            StartCoroutine(Paint(stroke));
        }

        /// <summary>Asks for a card's picture the first time it comes into view.</summary>
        private static void RequestPicture(Choice choice)
        {
            if (choice.Requested || choice.Picture == null) return;
            choice.Requested = true;
            var picture = choice.Picture;
            var texture = ArenaThumbnails.Get(choice.Id, ready => { if (picture != null) { picture.texture = ready; picture.enabled = true; } });
            if (texture != null) { picture.texture = texture; picture.enabled = true; }
        }

        private void BuildCard(int index)
        {
            var choice = _choices[index];
            var card = Node(_strip, choice.Id, Vector2.zero, new Vector2(196, 128), false);
            var frame = card.gameObject.AddComponent<PaperPanel>(); frame.color = Paper; frame.raycastTarget = true;
            var picture = Node(card, "Picture", new Vector2(0, 10), new Vector2(176, 92), false);
            // A name plate until the arena's picture has been drawn over it.
            var plate = picture.gameObject.AddComponent<Image>(); plate.color = choice.Id == VersusRoster.RandomArena ? Red : new Color(Ink.r, Ink.g, Ink.b, .9f); plate.raycastTarget = false;
            Label(picture, choice.Id == VersusRoster.RandomArena ? "?" : choice.Name.ToUpperInvariant(), choice.Id == VersusRoster.RandomArena ? 48 : 16, Paper,
                TextAnchor.MiddleCenter, Vector2.zero, new Vector2(170, 88)).resizeTextForBestFit = true;
            if (choice.Id != VersusRoster.RandomArena)
            {
                var art = Node(picture, "Art", Vector2.zero, new Vector2(176, 92), false);
                choice.Picture = art.gameObject.AddComponent<RawImage>();
                choice.Picture.raycastTarget = false;
                choice.Picture.enabled = false;
            }
            Label(card, choice.Name, 15, Ink, TextAnchor.MiddleCenter, new Vector2(0, -50), new Vector2(190, 22));
            var button = card.gameObject.AddComponent<Button>(); button.targetGraphic = frame;
            button.transition = Selectable.Transition.None;
            int captured = index;
            button.onClick.AddListener(() => { if (captured == _focus) Choose(); else Focus(captured); });
            choice.Card = card;
        }

        private void Arrow(Vector2 position, string text, Action click)
        {
            var rect = Node(_root, "Arrow " + text, position, new Vector2(56, 56), false);
            var disc = rect.gameObject.AddComponent<UiDisc>(); disc.color = new Color(Ink.r, Ink.g, Ink.b, .85f);
            disc.SetRing(Gold, 2f);
            Label(rect, text, 30, Gold, TextAnchor.MiddleCenter, new Vector2(0, 2), new Vector2(56, 56));
            var button = rect.gameObject.AddComponent<Button>(); button.targetGraphic = disc;
            button.onClick.AddListener(() => click());
            PressBounce.Attach(rect.gameObject);
        }

        private void Move(int step)
        {
            if (_choices.Count == 0) return;
            Focus((_focus + step + _choices.Count) % _choices.Count);
        }

        private void Focus(int index, bool silent = false)
        {
            _focus = Mathf.Clamp(index, 0, _choices.Count - 1);
            var choice = _choices[_focus];
            _name.text = choice.Name.ToUpperInvariant();
            _count.text = (_focus + 1) + " / " + _choices.Count + (choice.Id == VersusRoster.RandomArena ? "   ·   a different arena every fight" : "");
            _badge.SetActive(choice.Id == _current);
            if (!silent) EclipseUiAudio.Play(UiSound.Focus);
            // Building an arena takes a moment; only preview where the player settles.
            _previewAt = Time.unscaledTime + .28f;
        }

        private void Choose()
        {
            if (_closing) return;
            _closing = true;
            EclipseUiAudio.Play(UiSound.Begin);
            string id = _choices[_focus].Id;
            var picked = _picked;
            Close();
            picked?.Invoke(id);
        }

        private void Cancel()
        {
            if (_closing) return;
            _closing = true;
            EclipseUiAudio.Play(UiSound.Back);
            var cancelled = _cancelled;
            Close();
            LocalVersusMenu.Ensure().SetBackdropArena(_current);
            cancelled?.Invoke();
        }

        private void Close()
        {
            if (_open == this) _open = null;
            LocalVersusMenu.Ensure().SetPageHidden(false);
            var events = UnityEngine.EventSystems.EventSystem.current;
            if (events != null)
            {
                // Focus is left empty: the key that closed the picker must not also press a menu button.
                events.sendNavigationEvents = _navigation;
                events.SetSelectedGameObject(null);
            }
            Destroy(gameObject);
        }

        private void Update()
        {
            if (_closing) return;
            _group.alpha = Mathf.Clamp01((Time.unscaledTime - _openedAt) / .22f);
            float now = Time.unscaledTime;
            if (now - _openedAt < .15f) return;
            int keyboard = Eclipse.Input.EclipseInput.GetKey(KeyCode.LeftArrow) || Eclipse.Input.EclipseInput.GetKey(KeyCode.A) ? -1 :
                Eclipse.Input.EclipseInput.GetKey(KeyCode.RightArrow) || Eclipse.Input.EclipseInput.GetKey(KeyCode.D) ? 1 : 0;
            float pad = 0f;
            bool choose = false, cancel = false;
            try
            {
                var dpad = GamePad.GetStick(GamePad.Stick.Dpad, GamePad.Player.Any);
                var stick = GamePad.GetStick(GamePad.Stick.LeftStick, GamePad.Player.Any);
                pad = Mathf.Abs(stick.x) > Mathf.Abs(dpad.x) ? stick.x : dpad.x;
                choose = GamePad.GetButtonDown(GamePad.Button.A, GamePad.Player.Any);
                cancel = GamePad.GetButtonDown(GamePad.Button.B, GamePad.Player.Any);
            }
            catch (Exception) { }
            int direction = keyboard != 0 ? keyboard : Mathf.Abs(pad) > .6f ? (pad > 0f ? 1 : -1) : 0;
            // First press moves at once; holding repeats.
            if (direction != 0 && (direction != _held || now >= _repeatAt))
            {
                _repeatAt = now + (direction != _held ? .38f : .1f);
                Move(direction);
            }
            _held = direction;
            float wheel = Eclipse.Input.EclipseInput.mouseScrollDelta.y;
            if (Mathf.Abs(wheel) > .1f) Move(wheel > 0 ? -1 : 1);
            if (choose || Eclipse.Input.EclipseInput.GetKeyDown(KeyCode.Return) || Eclipse.Input.EclipseInput.GetKeyDown(KeyCode.KeypadEnter) || Eclipse.Input.EclipseInput.GetKeyDown(KeyCode.Space)) { Choose(); return; }
            if (cancel || Eclipse.Input.EclipseInput.GetKeyDown(KeyCode.Escape) || Eclipse.Input.EclipseInput.GetKeyDown(KeyCode.Backspace)) { Cancel(); return; }
            if (_previewAt >= 0f && Time.unscaledTime >= _previewAt)
            {
                _previewAt = -1f;
                string id = _choices[_focus].Id;
                if (id != VersusRoster.RandomArena) LocalVersusMenu.Ensure().SetBackdropArena(id);
            }
            _glide = Mathf.Lerp(_glide, _focus, 1f - Mathf.Exp(-Time.unscaledDeltaTime * 12f));
            for (int i = 0; i < _choices.Count; i++)
            {
                var card = _choices[i].Card;
                float offset = i - _glide;
                // Wrap so the strip reads as a loop around the focused card.
                if (offset > _choices.Count / 2f) offset -= _choices.Count;
                if (offset < -_choices.Count / 2f) offset += _choices.Count;
                float distance = Mathf.Abs(offset);
                card.gameObject.SetActive(distance < 3.4f);
                if (distance >= 3.4f) continue;
                RequestPicture(_choices[i]);
                float scale = Mathf.Lerp(1.18f, .78f, Mathf.Clamp01(distance / 2.5f));
                card.anchoredPosition = new Vector2(offset * Spacing, -Mathf.Clamp01(distance) * 8f);
                card.localScale = new Vector3(scale, scale, 1f);
                var group = ComponentUtility.Ensure<CanvasGroup>(card.gameObject);
                group.alpha = Mathf.Lerp(1f, .35f, Mathf.Clamp01(distance / 3f));
                if (distance < .5f) card.SetAsLastSibling();
            }
        }

        private static System.Collections.IEnumerator Paint(InkStroke stroke)
        {
            float start = Time.unscaledTime + .1f;
            while (stroke != null)
            {
                float t = Mathf.Clamp01((Time.unscaledTime - start) / .3f);
                stroke.Fill = 1f - (1f - t) * (1f - t);
                if (t >= 1f) yield break;
                yield return null;
            }
        }

        private static RectTransform Node(Transform parent, string name, Vector2 position, Vector2 size, bool stretch,
            Vector2? pivot = null, Vector2? anchorMin = null, Vector2? anchorMax = null)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            if (stretch) { rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero; return rect; }
            rect.anchorMin = anchorMin ?? new Vector2(.5f, .5f);
            rect.anchorMax = anchorMax ?? new Vector2(.5f, .5f);
            rect.pivot = pivot ?? new Vector2(.5f, .5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        private Text Label(Transform parent, string text, int size, Color color, TextAnchor alignment, Vector2 position, Vector2 box)
        {
            var rect = Node(parent, "Label", position, box, false);
            var label = rect.gameObject.AddComponent<Text>();
            label.font = _font; label.fontSize = size; label.text = text; label.color = color; label.alignment = alignment; label.raycastTarget = false;
            var shadow = rect.gameObject.AddComponent<Shadow>(); shadow.effectColor = new Color(0, 0, 0, .7f); shadow.effectDistance = new Vector2(1.5f, -2f);
            return label;
        }
    }
}
