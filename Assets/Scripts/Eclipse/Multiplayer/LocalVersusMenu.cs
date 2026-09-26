using System;
using Eclipse.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Eclipse.Multiplayer
{
    public sealed class LocalVersusMenu : MonoBehaviour
    {
        private static readonly Color Ink = new Color32(30, 25, 22, 255);
        private static readonly Color Paper = new Color32(223, 207, 177, 255);
        private static readonly Color Red = new Color32(147, 39, 31, 255);
        private static LocalVersusMenu instance;
        private Font font;
        private RectTransform panel;
        private UnityEngine.UI.Text status;
        private EventSystem ownedEventSystem;
        private int p1Weapon, p2Weapon, arena;
        private bool keyboardPlayerOne = true;
        private int winsRequired = 2;
        private int roundTime = 99;
        private float nextDeviceCheck;
        private bool padsWereReady;
        private int inputFrame;
        private Page page;
        public bool IsShowing { get; private set; }
        // Lets the loading overlay step aside once the local versus lobby is up.
        public static bool LobbyVisible => instance != null && instance.IsShowing && instance.page == Page.Lobby;

        private enum Page { Hidden, Lobby, Pause, Result }

        public static LocalVersusMenu Ensure()
        {
            if (instance != null) return instance;
            instance = FindFirstObjectByType<LocalVersusMenu>();
            if (instance != null) return instance;
            var host = new GameObject("Eclipse Local Versus Menu", typeof(RectTransform));
            instance = host.AddComponent<LocalVersusMenu>();
            DontDestroyOnLoad(host);
            return instance;
        }

        private void Awake()
        {
            if (instance != null && instance != this) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
            font = Resources.Load<Font>("ui/fonts/AGOpusBold") ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var canvas = gameObject.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 32755;
            var scaler = gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1280, 720);
            scaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand;
            gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            panel = Rect(transform, "Local Versus Overlay"); Stretch(panel);
            SceneManager.sceneLoaded += OnSceneLoaded;
            Hide();
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (ownedEventSystem != null) Destroy(ownedEventSystem.gameObject);
            if (instance == this) instance = null;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (IsShowing) EnsureEventSystem();
        }

        private void Update()
        {
            if (!LocalVersusSession.IsActive) return;
            if (page == Page.Lobby && Time.unscaledTime >= nextDeviceCheck)
            {
                nextDeviceCheck = Time.unscaledTime + .5f;
                if (PadsReady(keyboardPlayerOne) != padsWereReady) RefreshDeviceStatus();
            }
            var fight = Fight.GetCurrentFight();
            if (fight == null || !fight.IsLocalVersus) return;
            bool pausePressed = UnityEngine.Input.GetKeyDown(KeyCode.Escape);
            if (!pausePressed && (GamePad.GetButtonDown(GamePad.Button.Start, GamePad.Player.One) ||
                (!CurrentKeyboardPlayerOne() && GamePad.GetButtonDown(GamePad.Button.Start, GamePad.Player.Two))))
            {
                pausePressed = true;
                // A custom combat binding owns its button. Escape and the HUD's
                // pause button remain available when Start is assigned to combat.
                for (int action = 0; action < FightControllerBindings.Names.Length; action++)
                    if (FightControllerBindings.Get(action) == (int)GamePad.Button.Start) pausePressed = false;
            }
            if (!pausePressed) return;
            if (IsShowing)
            {
                if (page == Page.Pause && fight.IsPaused() && PadsReady(CurrentKeyboardPlayerOne())) Resume();
            }
            else if (!fight.IsPaused()) Pause("Match paused.");
        }

        public void ShowLobby()
        {
            EnsureEventSystem(); ReadSettings(LocalVersusSession.Settings);
            page = Page.Lobby;
            Rebuild("LOCAL VERSUS", "Choose a matchup", body =>
            {
                AddChoice(body, "PLAYER 1 WEAPON", () => LocalVersusMatch.WeaponLabels[p1Weapon], () => p1Weapon = (p1Weapon + 1) % LocalVersusMatch.WeaponIds.Length);
                AddChoice(body, "PLAYER 2 WEAPON", () => LocalVersusMatch.WeaponLabels[p2Weapon], () => p2Weapon = (p2Weapon + 1) % LocalVersusMatch.WeaponIds.Length);
                AddChoice(body, "ARENA", () => LocalVersusMatch.ArenaLabels[arena], () => arena = (arena + 1) % LocalVersusMatch.ArenaIds.Length);
                AddChoice(body, "CONTROLS", SchemeLabel, () => { keyboardPlayerOne = !keyboardPlayerOne; RefreshDeviceStatus(); });
                AddChoice(body, "FIRST TO", () => winsRequired + (winsRequired == 1 ? " WIN" : " WINS"), () => winsRequired = winsRequired % 3 + 1);
                AddButton(body, "START MATCH", () =>
                {
                    if (!PadsReady(keyboardPlayerOne)) { SetStatus(PadReason(keyboardPlayerOne)); return; }
                    TryStart(new LocalVersusSettings(LocalVersusMatch.WeaponIds[p1Weapon], LocalVersusMatch.WeaponIds[p2Weapon], LocalVersusMatch.ArenaIds[arena], keyboardPlayerOne, winsRequired, roundTime));
                });
                AddButton(body, "RETURN TO TITLE", LocalVersusSession.ReturnToTitle);
            });
            RefreshDeviceStatus();
        }

        public void ShowResult(int winner, int playerOneWins, int playerTwoWins)
        {
            EnsureEventSystem();
            page = Page.Result;
            Rebuild(winner == 0 ? "PLAYER 1 WINS" : winner == 1 ? "PLAYER 2 WINS" : "MATCH ENDED",
                "Player 1  " + playerOneWins + "  :  " + playerTwoWins + "  Player 2", body =>
            {
                AddButton(body, "REMATCH", () => { if (LocalVersusSession.Settings == null) SetStatus("No matchup is configured."); else TryStart(LocalVersusSession.Settings); });
                AddButton(body, "CHANGE MATCHUP", LocalVersusSession.ShowLobby);
                AddButton(body, "RETURN TO TITLE", LocalVersusSession.ReturnToTitle);
            });
        }

        public void ShowPause(string reason)
        {
            EnsureEventSystem();
            page = Page.Pause;
            Rebuild("PAUSED", string.IsNullOrEmpty(reason) ? "Local versus" : reason, body =>
            {
                AddButton(body, "RESUME", Resume);
                AddButton(body, "CHANGE MATCHUP", LocalVersusSession.ShowLobby);
                AddButton(body, "RETURN TO TITLE", LocalVersusSession.ReturnToTitle);
            });
        }

        public void Hide()
        {
            IsShowing = false; page = Page.Hidden;
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null && panel != null &&
                EventSystem.current.currentSelectedGameObject.transform.IsChildOf(panel)) EventSystem.current.SetSelectedGameObject(null);
            if (panel != null) panel.gameObject.SetActive(false);
        }

        private void Pause(string reason)
        {
            if (inputFrame == Time.frameCount) return;
            inputFrame = Time.frameCount;
            LocalVersusSession.Pause(reason);
        }

        private void Resume()
        {
            if (inputFrame == Time.frameCount) return;
            bool keyboard = CurrentKeyboardPlayerOne();
            if (!PadsReady(keyboard)) { SetStatus(PadReason(keyboard)); return; }
            inputFrame = Time.frameCount;
            LocalVersusSession.Resume();
        }

        private void TryStart(LocalVersusSettings settings)
        {
            try { LocalVersusSession.StartMatch(settings); }
            catch (Exception exception) { Debug.LogException(exception); SetStatus(string.IsNullOrEmpty(exception.Message) ? "The match could not start." : exception.Message); panel.gameObject.SetActive(true); IsShowing = true; }
        }

        private void ReadSettings(LocalVersusSettings settings)
        {
            if (settings == null) return;
            int i = Array.IndexOf(LocalVersusMatch.WeaponIds, settings.PlayerOneWeapon); if (i >= 0) p1Weapon = i;
            i = Array.IndexOf(LocalVersusMatch.WeaponIds, settings.PlayerTwoWeapon); if (i >= 0) p2Weapon = i;
            i = Array.IndexOf(LocalVersusMatch.ArenaIds, settings.Location); if (i >= 0) arena = i;
            keyboardPlayerOne = settings.KeyboardPlayerOne; winsRequired = Mathf.Clamp(settings.WinsRequired, 1, 3); roundTime = settings.RoundTimeSeconds;
        }

        private bool CurrentKeyboardPlayerOne() { return LocalVersusSession.Settings != null ? LocalVersusSession.Settings.KeyboardPlayerOne : keyboardPlayerOne; }
        private string SchemeLabel() { return keyboardPlayerOne ? "KEYBOARD + GAMEPAD" : "2 GAMEPADS"; }
        private static bool PadsReady(bool keyboard) { return FightGamepadInput.IsConnected(GamePad.Player.One) && (keyboard || FightGamepadInput.IsConnected(GamePad.Player.Two)); }
        private static string PadReason(bool keyboard)
        {
            if (!FightGamepadInput.IsConnected(GamePad.Player.One)) return "Connect Gamepad 1 to continue.";
            return !keyboard && !FightGamepadInput.IsConnected(GamePad.Player.Two) ? "Connect Gamepad 2 to continue." : "Ready.";
        }

        private static string InputHint(bool keyboard)
        {
            if (!keyboard) return "Player 1: Gamepad 1     Player 2: Gamepad 2";
            return "P1 move " + FightKeyBindings.Display(FightKeyBindings.Get(KeyCode.W)) + "/" + FightKeyBindings.Display(FightKeyBindings.Get(KeyCode.A)) + "/" +
                FightKeyBindings.Display(FightKeyBindings.Get(KeyCode.S)) + "/" + FightKeyBindings.Display(FightKeyBindings.Get(KeyCode.D)) +
                "  Punch " + FightKeyBindings.Display(FightKeyBindings.Get(KeyCode.O)) + "  Kick " + FightKeyBindings.Display(FightKeyBindings.Get(KeyCode.P)) +
                "     P2: Gamepad 1";
        }

        private void RefreshDeviceStatus()
        {
            padsWereReady = PadsReady(keyboardPlayerOne);
            SetStatus(padsWereReady ? InputHint(keyboardPlayerOne) : PadReason(keyboardPlayerOne));
        }

        private void Rebuild(string title, string subtitle, Action<RectTransform> build)
        {
            panel.gameObject.SetActive(true);
            for (int i = panel.childCount - 1; i >= 0; i--) { panel.GetChild(i).gameObject.SetActive(false); Destroy(panel.GetChild(i).gameObject); }
            // The lobby has no match behind it; pause and results keep the fight visible under an ink wash.
            SetImage(panel, page == Page.Lobby ? Ink : new Color(20f / 255f, 14f / 255f, 11f / 255f, .7f));
            var paper = Rect(panel, "Paper"); paper.anchorMin = paper.anchorMax = paper.pivot = new Vector2(.5f, .5f); paper.sizeDelta = new Vector2(760, 660);
            var card = paper.gameObject.AddComponent<Eclipse.UI.PaperPanel>(); card.color = Paper; card.raycastTarget = true;
            // Title painted on a red brush stroke rather than a flat band.
            var header = Rect(paper, "Header"); header.anchorMin = new Vector2(0, 1); header.anchorMax = new Vector2(1, 1); header.pivot = new Vector2(.5f, 1); header.anchoredPosition = new Vector2(0, -14); header.sizeDelta = new Vector2(-40, 82);
            var stroke = header.gameObject.AddComponent<Eclipse.UI.InkStroke>(); stroke.color = Red; stroke.raycastTarget = false; stroke.Seed = title.Length * 31;
            Label(header, title, 38, Paper, TextAnchor.MiddleCenter);
            Eclipse.UI.UiReveal.Play(paper, 0f, .34f, new Vector2(0, -18), .96f);
            StartCoroutine(PaintStroke(stroke));
            var sub = Label(paper, subtitle, 21, Ink, TextAnchor.MiddleCenter); sub.rectTransform.anchorMin = new Vector2(0, 1); sub.rectTransform.anchorMax = new Vector2(1, 1); sub.rectTransform.pivot = new Vector2(.5f, 1); sub.rectTransform.anchoredPosition = new Vector2(0, -105); sub.rectTransform.sizeDelta = new Vector2(-40, 48);
            var body = Rect(paper, "Options"); body.anchorMin = body.anchorMax = body.pivot = new Vector2(.5f, 1); body.anchoredPosition = new Vector2(0, -150); body.sizeDelta = new Vector2(650, 400);
            var layout = body.gameObject.AddComponent<UnityEngine.UI.VerticalLayoutGroup>(); layout.spacing = 9; layout.childControlHeight = layout.childControlWidth = true; layout.childForceExpandHeight = false; layout.childForceExpandWidth = true;
            build(body);
            status = Label(paper, "", 16, Red, TextAnchor.MiddleCenter); status.rectTransform.anchorMin = new Vector2(0, 0); status.rectTransform.anchorMax = new Vector2(1, 0); status.rectTransform.pivot = new Vector2(.5f, 0); status.rectTransform.anchoredPosition = new Vector2(0, 14); status.rectTransform.sizeDelta = new Vector2(-40, 52);
            IsShowing = true; Cursor.visible = true; Cursor.lockState = CursorLockMode.None;
            Eclipse.UI.EclipseUiAudio.SuppressFocusSound();
            Eclipse.UI.EclipseUiAudio.Play(Eclipse.UI.UiSound.Open);
            FocusFirst(body);
        }

        private static System.Collections.IEnumerator PaintStroke(Eclipse.UI.InkStroke stroke)
        {
            float start = Time.unscaledTime + .12f;
            while (stroke != null)
            {
                float t = Mathf.Clamp01((Time.unscaledTime - start) / .32f);
                stroke.Fill = 1f - (1f - t) * (1f - t);
                if (t >= 1f) yield break;
                yield return null;
            }
        }

        private void AddChoice(RectTransform parent, string caption, Func<string> value, Action cycle)
        {
            var row = Rect(parent, caption); row.gameObject.AddComponent<UnityEngine.UI.LayoutElement>().preferredHeight = 46;
            var horizontal = row.gameObject.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>(); horizontal.spacing = 12; horizontal.childControlWidth = horizontal.childControlHeight = true; horizontal.childForceExpandWidth = false;
            var left = Label(row, caption, 22, Ink, TextAnchor.MiddleLeft); left.gameObject.AddComponent<UnityEngine.UI.LayoutElement>().flexibleWidth = 1;
            UnityEngine.UI.Text valueLabel = null;
            var button = AddButton(row, value() + "   >", () => { cycle(); valueLabel.text = value() + "   >"; }, 340, Eclipse.UI.UiSound.Toggle);
            valueLabel = button.GetComponentInChildren<UnityEngine.UI.Text>();
        }

        private UnityEngine.UI.Button AddButton(RectTransform parent, string text, Action action, float width = -1,
            Eclipse.UI.UiSound? sound = null)
        {
            var rect = Rect(parent, text); var element = rect.gameObject.AddComponent<UnityEngine.UI.LayoutElement>(); element.preferredHeight = 46; if (width > 0) { element.minWidth = element.preferredWidth = width; element.flexibleWidth = 0; }
            // A brush-stroke plate; the stroke itself is also the hit area.
            var plate = rect.gameObject.AddComponent<Eclipse.UI.InkStroke>(); plate.color = Ink; plate.Seed = text.GetHashCode() & 0xffff;
            var button = rect.gameObject.AddComponent<UnityEngine.UI.Button>(); button.targetGraphic = plate;
            var label = Label(rect, text, 22, Paper, TextAnchor.MiddleCenter);
            var fx = Eclipse.UI.EclipseUiButton.Attach(button, plate, label, Ink, Red, Paper, Paper, 6f, .02f);
            var cue = sound ?? (text == "START MATCH" || text == "REMATCH" ? Eclipse.UI.UiSound.Begin
                : text == "RETURN TO TITLE" || text == "RESUME" ? Eclipse.UI.UiSound.Back : Eclipse.UI.UiSound.Confirm);
            button.onClick.AddListener(() => { fx.Punch(); Eclipse.UI.EclipseUiAudio.Play(cue); action(); });
            return button;
        }

        private UnityEngine.UI.Text Label(Transform parent, string text, int size, Color color, TextAnchor alignment)
        {
            var rect = Rect(parent, "Label"); Stretch(rect); var label = rect.gameObject.AddComponent<UnityEngine.UI.Text>(); label.font = font; label.fontSize = size; label.text = text; label.color = color; label.alignment = alignment; label.raycastTarget = false; return label;
        }

        private static RectTransform Rect(Transform parent, string name) { var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>(); rect.SetParent(parent, false); return rect; }
        private static void Stretch(RectTransform rect) { rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero; }
        private static void AddImage(RectTransform rect, Color color) { var image = rect.gameObject.AddComponent<UnityEngine.UI.Image>(); image.color = color; image.raycastTarget = true; }
        private static void SetImage(RectTransform rect, Color color) { var image = rect.GetComponent<UnityEngine.UI.Image>() ?? rect.gameObject.AddComponent<UnityEngine.UI.Image>(); image.color = color; image.raycastTarget = true; }
        private void SetStatus(string text) { if (status != null) status.text = text ?? string.Empty; }

        private void EnsureEventSystem()
        {
            if (EventSystem.current != null) return;
            ownedEventSystem = null;
            ownedEventSystem = new GameObject("Local Versus Input", typeof(EventSystem), typeof(StandaloneInputModule)).GetComponent<EventSystem>();
        }

        private static void FocusFirst(RectTransform body)
        {
            if (EventSystem.current == null) return;
            var buttons = body.GetComponentsInChildren<UnityEngine.UI.Button>(true); if (buttons.Length > 0) EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
        }
    }
}
