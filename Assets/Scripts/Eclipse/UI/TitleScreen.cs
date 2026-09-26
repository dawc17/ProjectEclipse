using System;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Scenes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // Built independently of the recovered canvas: no scene or sprite GUID changes.
    public sealed partial class TitleScreen : MonoBehaviour
    {
        private static bool enteredCampaign;
        public static bool IsOpen { get; private set; }
        private static readonly Color Ink = new Color32(30, 25, 22, 255);
        private static readonly Color Paper = new Color32(223, 207, 177, 255);
        private static readonly Color Red = new Color32(147, 39, 31, 255);
        private static readonly int[] Caps = { 0, 60, 120, 144, 165, 240, 360 };
        private int bindingAction = -1;
        private int bindingFrame;
        private readonly bool[] heldControllerInputs = new bool[12];
        private bool ControllerPage { get { return currentPage == "Controller"; } }
        private Text bindingStatus;
        private readonly Dictionary<int, Text> bindingLabels = new Dictionary<int, Text>();
        private RectTransform page;
        private RectTransform viewport;
        private RectTransform paperBackground, settingsBackground, footerBackground, footerHint;
        private RectTransform skyLeft, skyRight;
        private Font font;
        private Material logoInk;
        private readonly Dictionary<string, Texture2D> autumnTextures = new Dictionary<string, Texture2D>();
        private EventSystem ownedEventSystem;
        private GameObject previousSelection;
        private bool previousNavigation;
        private readonly List<Selectable> controls = new List<Selectable>();
        private string currentPage = "Home";
        private int selected;
        private bool rebuilding;
        private bool leaving;
        private Action optionsClosed;
        private bool optionsOnly;
        private float enterAt;
        private Vector2Int resolution;
        private FullScreenMode mode;
        private Vector2Int oldResolution;
        private FullScreenMode oldMode;
        private float confirmUntil;
        private Text countdown;
        private readonly List<Vector2Int> resolutions = new List<Vector2Int>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSession()
        {
            enteredCampaign = false;
            IsOpen = false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ApplyPreferences()
        {
            AudioListener.volume = 1f; // Retire the title-only volume override; campaign audio keeps its own settings.
            if (!Application.isEditor && PlayerPrefs.HasKey("Eclipse.DisplayWidth"))
                Screen.SetResolution(PlayerPrefs.GetInt("Eclipse.DisplayWidth"),
                    PlayerPrefs.GetInt("Eclipse.DisplayHeight"),
                    PlayerPrefs.GetInt("Eclipse.Fullscreen", 1) == 1 ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
        }

        public static void ShowAtStartup()
        {
            if (enteredCampaign || IsOpen || Application.isBatchMode) return;
            new GameObject("Eclipse Title Screen", typeof(RectTransform)).AddComponent<TitleScreen>();
        }

        public static void ShowOptions(Action closed)
        {
            if (IsOpen) return;
            var screen = new GameObject("Eclipse Options", typeof(RectTransform)).AddComponent<TitleScreen>();
            screen.optionsOnly = true;
            screen.optionsClosed = closed;
            screen.Settings("Display");
        }

        public static void PrepareForRestart() { enteredCampaign = false; }

        private void Awake()
        {
            IsOpen = true;
            SoundController.ApplySavedVolumes();
            font = Resources.Load<Font>("ui/fonts/AGOpusBold");
            if (font == null) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var inkShader = Resources.Load<Shader>("shaders/EclipseTitleInk");
            if (inkShader != null) { logoInk = new Material(inkShader); logoInk.SetFloat("_WhiteInk", 1f); }
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32760;
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            gameObject.AddComponent<GraphicRaycaster>();
            if (EventSystem.current == null)
                ownedEventSystem = new GameObject("Title Input", typeof(EventSystem), typeof(StandaloneInputModule)).GetComponent<EventSystem>();
            previousSelection = EventSystem.current.currentSelectedGameObject;
            previousNavigation = EventSystem.current.sendNavigationEvents;
            EventSystem.current.sendNavigationEvents = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            var backdrop = Box(transform, "Backdrop", 0, 0, 1280, 720, Ink);
            backdrop.anchorMin = Vector2.zero;
            backdrop.anchorMax = Vector2.one;
            backdrop.offsetMin = backdrop.offsetMax = Vector2.zero;
            viewport = Rect(transform, "Title viewport", 0, 0, 1280, 720);
            viewport.anchorMin = viewport.anchorMax = viewport.pivot = new Vector2(.5f, .5f);
            viewport.anchoredPosition = Vector2.zero;
            viewport.gameObject.AddComponent<RectMask2D>();
            page = Rect(viewport, "Page", 0, 0, 1280, 720);
            page.anchorMin = page.anchorMax = new Vector2(.5f, .5f);
            page.pivot = new Vector2(.5f, .5f);
            page.anchoredPosition = Vector2.zero;
            foreach (var item in Screen.resolutions)
            {
                var size = new Vector2Int(item.width, item.height);
                if (size.x >= 800 && size.y >= 600 && !resolutions.Contains(size)) resolutions.Add(size);
            }
            resolution = new Vector2Int(Screen.width, Screen.height);
            if (!resolutions.Contains(resolution)) resolutions.Add(resolution);
            mode = Screen.fullScreenMode == FullScreenMode.Windowed ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow;
            Home();
        }

        private void Clear(string name)
        {
            bindingAction = -1;
            bindingLabels.Clear();
            currentPage = name;
            rebuilding = true;
            controls.Clear();
            selected = 0;
            foreach (Transform child in page) { child.gameObject.SetActive(false); Destroy(child.gameObject); }
            paperBackground = Box(page, "Paper", 0, 0, 1280, 720, Paper);
            DrawAutumnGate();
            settingsBackground = null;
            if (name != "Home")
                settingsBackground = Box(page, "Settings paper", 0, 0, 1280, 720, new Color(Paper.r, Paper.g, Paper.b, .96f));
            footerBackground = Box(page, "Footer", 0, 674, 1280, 46, new Color32(132, 40, 14, 245));
            footerHint = Label(page, "ARROWS  Select     ENTER  Confirm     ESC  Back", 620, 680, 610, 30, 16, Paper, TextAnchor.MiddleRight).rectTransform;
            LayoutViewport();
        }

        private void LateUpdate() { LayoutViewport(); }

        private void LayoutViewport()
        {
            if (viewport == null || page == null) return;
            var available = ((RectTransform)transform).rect.size;
            if (available.x <= 0 || available.y <= 0) return;
            float width = Mathf.Min(available.x, available.y * (21f / 9f));
            float height = available.y;
            viewport.sizeDelta = new Vector2(width, height);
            float left = (1280f - width) * .5f;
            float top = (720f - height) * .5f;
            // Keep the authored gate, logo and controls at their original proportions.
            // Only the surrounding scenery, paper and footer fill the wider viewport.
            Place(paperBackground, left, top, width, height);
            Place(settingsBackground, left, top, width, height);
            Place(skyLeft, left, top, width * .5f, height - 46);
            Place(skyRight, 640, top, width * .5f, height - 46);
            Place(footerBackground, left, top + height - 46, width, 46);
            Place(footerHint, left + width - 660, top + height - 40, 610, 30);
        }

        private static void Place(RectTransform rect, float x, float y, float width, float height)
        {
            if (rect == null) return;
            rect.anchoredPosition = new Vector2(x, -y);
            rect.sizeDelta = new Vector2(width, height);
        }

        private void DrawAutumnGate()
        {
            const string root = "Textures/Locations/autumn/";
            skyLeft = PackedPicture("Sky left", root + "autumn_bg", 0, 0, 640, 674, new Rect(0, 0, 1, .5f))?.rectTransform;
            skyRight = PackedPicture("Sky right", root + "autumn_bg", 640, 0, 640, 674, new Rect(0, .5f, 1, .5f))?.rectTransform;
            // Original TexturePacker sourceSize is 512 square for all three tiles.
            // sourceColorRect trims 24 pixels above the trees and 66 above the gate.
            // Preserve those offsets and one scale so roof, wall and ground edges meet.
            const float scale = 792f / 512f;
            const float originY = 22f - 66f * scale;
            var left = PackedPicture("Left tree", root + "autumn_atlas_layer1",
                244 - 792, originY + 24 * scale, 488 * scale, 512 * scale,
                new Rect(519f / 1024, 509f / 1024, 488f / 1024, 512f / 1024));
            if (left != null)
            {
                // The left tile is packed clockwise. Rotate its UI quad, not the source sprite.
                var rect = left.rectTransform;
                rect.pivot = new Vector2(.5f, .5f);
                rect.anchoredPosition = new Vector2(244 - 396, -(originY + 24 * scale + 244 * scale));
                rect.localRotation = Quaternion.Euler(0, 0, 90);
            }
            PackedPicture("Right tree", root + "autumn_atlas_layer1", 1036, originY + 24 * scale, 792, 488 * scale,
                new Rect(3f / 1024, 533f / 1024, 512f / 1024, 488f / 1024));
            PackedPicture("Gate", root + "autumn_atlas_layer1", 244, 22, 792, 446 * scale,
                new Rect(3f / 1024, 83f / 1024, 512f / 1024, 446f / 1024));
        }

        private RawImage PackedPicture(string name, string address, float x, float y, float w, float h, Rect uv)
        {
            Texture2D texture;
            if (!autumnTextures.TryGetValue(address, out texture))
            {
                var source = Eclipse.Content.PackagedArtCatalog.Load<Texture2D>(address);
                if (source == null) { Debug.LogError("[Title] Missing autumn artwork: " + address); return null; }
                texture = Instantiate(source);
                texture.filterMode = FilterMode.Bilinear;
                autumnTextures.Add(address, texture);
            }
            var image = Rect(page, name, x, y, w, h).gameObject.AddComponent<RawImage>();
            image.texture = texture;
            image.uvRect = uv;
            image.raycastTarget = false;
            return image;
        }

        private void Home()
        {
            Clear("Home");
            Box(page, "Sign frame", 522, 80, 236, 112, new Color32(164, 120, 66, 255));
            Box(page, "Sign border", 527, 85, 226, 102, Ink);
            Box(page, "Sign wood", 531, 89, 218, 94, new Color32(73, 43, 29, 255));
            for (int i = 1; i < 4; i++)
                Box(page, "Wood grain", 531, 89 + i * 23, 218, 1, new Color32(103, 66, 43, 255));
            const float logoScale = 1f / 6f;
            const float logoX = 640 - (618 + 610) * logoScale / 2;
            const float logoY = 136 - 489 * logoScale / 2;
            Picture(page, "Logo left", "ui/fullscreen/startLoading_left", logoX, logoY, 618 * logoScale, 489 * logoScale,
                new Rect(292f / 1024, 368f / 1024, 618f / 1024, 489f / 1024));
            Picture(page, "Logo right", "ui/fullscreen/startLoading_right", logoX + 618 * logoScale, logoY, 610 * logoScale, 489 * logoScale,
                new Rect(0, 368f / 1024, 610f / 1024, 489f / 1024));
            Label(page, "PROJECT ECLIPSE", 405, 211, 470, 28, 16, Paper, TextAnchor.MiddleCenter);
            Button(page, "CAMPAIGN", 405, 280, 470, 60, BeginCampaign);
            Button(page, "MULTIPLAYER", 405, 355, 470, 60, () =>
            {
                Eclipse.Multiplayer.LocalVersusSession.RequestEntry();
                BeginCampaign();
            });
            Label(page, "LOCAL VERSUS", 405, 408, 470, 20, 12, Ink, TextAnchor.MiddleCenter);
            Button(page, "MODS", 405, 433, 470, 56, OpenMods);
            Button(page, "OPTIONS", 405, 494, 470, 56, () => Settings("Display"));
            Button(page, "QUIT GAME", 405, 555, 470, 56, QuitPrompt);
            FocusFirst();
        }
        private void Settings(string tab)
        {
            Clear(tab);
            Label(page, "Options", 76, 96, 550, 64, 46, Ink);
            string[] tabs = SettingsTabs;
            for (int i = 0; i < tabs.Length; i++)
            {
                string target = tabs[i];
                var button = Button(page, target, 76 + i * 188, 180, 180, 48, () => { if (currentPage != target) Settings(target); });
                if (tab == target) { var tint = button.colors; tint.normalColor = Red; button.colors = tint; }
            }
            if (tab == "Display")
            {
                Row("Window mode", () => mode == FullScreenMode.Windowed ? "Windowed" : "Borderless fullscreen", 244, () =>
                { mode = mode == FullScreenMode.Windowed ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed; });
                Row("Resolution", () => resolution.x + " x " + resolution.y, 296, () =>
                { resolution = resolutions[(resolutions.IndexOf(resolution) + 1) % resolutions.Count]; });
                Row("Frame limit", () => SF2DisplayFrameRate.MaxFrameRate == 0 ? "Display / VSync" : SF2DisplayFrameRate.MaxFrameRate + " FPS", 348, () =>
                { SF2DisplayFrameRate.SetMaxFrameRate(Caps[(Array.IndexOf(Caps, SF2DisplayFrameRate.MaxFrameRate) + 1) % Caps.Length]); });
                Row("Frame interpolation", () => OnOff(SF2DisplayFrameRate.InterpolationEnabled), 400, () =>
                { SF2DisplayFrameRate.ToggleInterpolation(); });
                Row("Motion blur", () => OnOff(SF2DisplayFrameRate.MotionBlurEnabled), 452, () =>
                { SF2DisplayFrameRate.ToggleMotionBlur(); });
                Row("Anti-aliasing", () => SF2DisplayFrameRate.AntiAliasingLabel(SF2DisplayFrameRate.AntiAliasing), 504, () =>
                { SF2DisplayFrameRate.CycleAntiAliasing(); });
                var apply = Button(page, "Apply display", 852, 612, 340, 48, ApplyDisplay);
                apply.interactable = !Application.isMobilePlatform;
                Label(page, "Window and resolution changes require confirmation. Rendering options save immediately.", 76, 612, 740, 48, 15, Ink);
            }
            else if (tab == "Accessibility")
            {
                OptionSlider("Critical hit pause", 290, AccessibilitySettings.CriticalPause, AccessibilitySettings.SetCriticalPause);
                OptionSlider("Critical hit shake", 390, AccessibilitySettings.CriticalShake, AccessibilitySettings.SetCriticalShake);
                Row("Control size", () => GraphicsController.LargeControlsEnabled() ? "Large" : "Small", 490, () =>
                {
                    GraphicsController.ToggleControlSize();
                    var dojo = Scene<DojoScene>.get_Current();
                    if (dojo != null) dojo.fight.RefreshControllerLayout();
                });
                Label(page, "0% disables the effect. 100% restores the original intensity. Changes save immediately.", 76, 550, 1120, 40, 17, Ink);
            }
            else if (tab == "Mod settings")
            {
                ModSettingsPage();
            }
            else if (tab == "Audio")
            {
                OptionSlider("Music volume", 290, SoundController.GetMusicVolume(), SoundController.SetMusicVolume);
                OptionSlider("Sound volume", 390, SoundController.GetSoundVolume(), SoundController.SetSoundVolume);

            }
            else
            {
                Label(page, ControllerPage ? "CONTROLLER REMAPPING" : "KEYBOARD REMAPPING", 76, 240, 1050, 30, 17, Red);
                for (int i = 0; i < (ControllerPage ? Eclipse.Input.FightControllerBindings.Defaults.Length : Eclipse.Input.FightKeyBindings.Defaults.Length); i++)
                {
                    int action = i;
                    float x = i < 5 ? 76 : 686;
                    float y = 280 + (i % 5) * 48;
                    Label(page, ControllerPage ? Eclipse.Input.FightControllerBindings.Names[i] : Eclipse.Input.FightKeyBindings.Names[i], x, y, 260, 34, 21, Ink);
                    var keyButton = Button(page, BindingLabel(i),
                        x + 270, y, 236, 34, () => BeginBinding(action));
                    bindingLabels.Add(i, keyButton.GetComponentInChildren<Text>());
                }
                Text movementLabel = null;
                if (ControllerPage)
                    movementLabel = Row("Movement: D-pad +", () => Eclipse.Input.FightControllerBindings.MovementStick == GamePad.Stick.LeftStick ? "Left stick" : "Right stick",
                        520, Eclipse.Input.FightControllerBindings.ToggleStick);
                else
                    Row("Battle touch controls", () => OnOff(BattleTouchControls.Visible), 520, BattleTouchControls.Toggle);
                bindingStatus = Label(page, ControllerPage ? "Select an action, then press a controller button or trigger. Esc cancels. Controller 1."
                    : "Combine movement keys for diagonals (e.g. W+D). Select an action to rebind; Esc cancels.", 76, 562, 1120, 38, 16, Ink);
                Button(page, "Restore defaults", 852, 604, 340, 48, () =>
                {
                    if (ControllerPage) Eclipse.Input.FightControllerBindings.Reset();
                    else Eclipse.Input.FightKeyBindings.Reset();
                    RefreshBindings();
                    if (movementLabel != null) movementLabel.text = "Left stick   >";
                    bindingStatus.text = ControllerPage ? "Default controller controls restored." : "Default keyboard controls restored.";
                });
            }
            Button(page, "Back", 76, 604, 220, 48, Back);
            FocusFirst();
        }

        private Text Row(string name, Func<string> value, float y, Action action)
        {
            Label(page, name, 76, y, 500, 48, 25, Ink);
            Text label = null;
            var button = Button(page, value() + "   >", 686, y, 506, 48, () =>
            {
                action();
                label.text = value() + "   >";
                PlayerPrefs.Save();
            });
            label = button.GetComponentInChildren<Text>();
            return label;
        }

        private void BeginBinding(int action)
        {
            bindingAction = action;
            bindingFrame = Time.frameCount;
            for (int i = 0; i < heldControllerInputs.Length; i++)
                heldControllerInputs[i] = ControllerPage && Eclipse.Input.FightControllerBindings.IsPressed(i);
            bindingLabels[action].text = ControllerPage ? "Press an input..." : "Press a key...";
            bindingStatus.text = "Binding " + (ControllerPage ? Eclipse.Input.FightControllerBindings.Names[action] : Eclipse.Input.FightKeyBindings.Names[action]) + ". Press Esc to cancel.";
        }

        private void RefreshBindings()
        {
            foreach (var pair in bindingLabels)
                pair.Value.text = BindingLabel(pair.Key);
        }

        private string BindingLabel(int action)
        {
            return ControllerPage ? Eclipse.Input.FightControllerBindings.Display(Eclipse.Input.FightControllerBindings.Get(action))
                : Eclipse.Input.FightKeyBindings.Display(Eclipse.Input.FightKeyBindings.Get(Eclipse.Input.FightKeyBindings.Defaults[action]));
        }

        private void CaptureControllerBinding()
        {
            if (!ControllerPage || Time.frameCount <= bindingFrame) return;
            for (int i = 0; i < heldControllerInputs.Length; i++)
            {
                bool pressed = Eclipse.Input.FightControllerBindings.IsPressed(i);
                bool rising = pressed && !heldControllerInputs[i];
                heldControllerInputs[i] = pressed;
                if (!rising) continue;
                string message;
                if (Eclipse.Input.FightControllerBindings.TrySet(bindingAction, i, out message))
                {
                    bindingAction = -1;
                    RefreshBindings();
                }
                bindingStatus.text = message;
                bindingFrame = Time.frameCount;
                break;
            }
        }

        private void OnGUI()
        {
            if (bindingAction < 0 || Time.frameCount <= bindingFrame) return;
            var input = Event.current;
            if (input.type != EventType.KeyDown || input.keyCode == KeyCode.None) return;
            if (input.keyCode == KeyCode.Escape)
            {
                bindingAction = -1;
                bindingStatus.text = "Binding cancelled.";
                RefreshBindings();
            }
            else if (!ControllerPage)
            {
                string message;
                if (Eclipse.Input.FightKeyBindings.TrySet(bindingAction, input.keyCode, out message))
                {
                    bindingAction = -1;
                    RefreshBindings();
                }
                bindingStatus.text = message;
            }
            bindingFrame = Time.frameCount;
            input.Use();
        }

        private static string OnOff(bool value) { return value ? "On" : "Off"; }

        private static readonly string[] SettingsTabs = { "Display", "Controls", "Controller", "Audio", "Accessibility", "Mod settings" };

        private const int ModSettingsPerPage = 6;
        private int modSettingsPage;

        // Toggles registered by enabled mods through sf2.settings.toggle, grouped
        // by mod in load order. Values are stored per installation and save immediately.
        private void ModSettingsPage()
        {
            var toggles = Eclipse.Modding.ModVisuals.Settings;
            if (toggles.Count == 0)
            {
                Label(page, "No enabled mod provides settings. Enable a mod in the Mods menu, then Apply & Restart.", 76, 260, 1120, 48, 20, Ink);
                return;
            }
            int pages = (toggles.Count + ModSettingsPerPage - 1) / ModSettingsPerPage;
            modSettingsPage = Mathf.Clamp(modSettingsPage, 0, pages - 1);
            for (int i = 0; i < ModSettingsPerPage; i++)
            {
                int index = modSettingsPage * ModSettingsPerPage + i;
                if (index >= toggles.Count) break;
                var toggle = toggles[index];
                string owner = ModDisplayName(toggle.Owner);
                Row(owner + ": " + toggle.Label, () => OnOff(Eclipse.Modding.ModSettingsStore.Get(toggle)), 244 + i * 52,
                    () => Eclipse.Modding.ModSettingsStore.Set(toggle, !Eclipse.Modding.ModSettingsStore.Get(toggle)));
            }
            if (pages > 1)
            {
                Button(page, "< Previous", 76, 566, 220, 48, () => { modSettingsPage = (modSettingsPage + pages - 1) % pages; Settings("Mod settings"); });
                Button(page, "Next >", 306, 566, 220, 48, () => { modSettingsPage = (modSettingsPage + 1) % pages; Settings("Mod settings"); });
                Label(page, "Page " + (modSettingsPage + 1) + " of " + pages, 540, 566, 300, 48, 17, Ink);
            }
            Label(page, "Settings from enabled mods. Changes save immediately and apply to this installation.", 76, 620, 1120, 30, 15, Ink);
        }

        private static string ModDisplayName(Eclipse.Modding.ModId id)
        {
            var host = Eclipse.Modding.ModRuntime.IsInitialized ? Eclipse.Modding.ModRuntime.Host : null;
            if (host != null)
                foreach (var mod in host.EnabledMods)
                    if (mod.Id == id) return mod.Manifest.Name;
            return id.Value;
        }

        private void ApplyDisplay()
        {
            oldResolution = new Vector2Int(Screen.width, Screen.height);
            oldMode = Screen.fullScreenMode;
            Screen.SetResolution(resolution.x, resolution.y, mode);
            confirmUntil = Time.realtimeSinceStartup + 15;
            Clear("Confirm");
            Label(page, "Keep these display settings?", 100, 200, 1080, 75, 42, Ink);
            countdown = Label(page, "", 100, 300, 1080, 80, 26, Ink);
            Button(page, "Keep changes", 100, 442, 470, 64, () =>
            {
                confirmUntil = 0;
                PlayerPrefs.SetInt("Eclipse.DisplayWidth", resolution.x);
                PlayerPrefs.SetInt("Eclipse.DisplayHeight", resolution.y);
                PlayerPrefs.SetInt("Eclipse.Fullscreen", mode == FullScreenMode.Windowed ? 0 : 1);
                PlayerPrefs.Save(); Settings("Display");
            });
            Button(page, "Revert", 650, 442, 470, 64, RevertDisplay);
            FocusFirst();
        }

        private void RevertDisplay()
        {
            confirmUntil = 0;
            Screen.SetResolution(oldResolution.x, oldResolution.y, oldMode);
            resolution = oldResolution; mode = oldMode;
            Settings("Display");
        }

        private void QuitPrompt()
        {
            Clear("Quit");
            Label(page, "Leave the shadows?", 100, 220, 1080, 70, 46, Ink);
            Label(page, "Close the game and return to your desktop.", 100, 310, 1080, 50, 26, Ink);
            Button(page, "Stay", 100, 440, 470, 64, Home);
            Button(page, "Quit game", 650, 440, 470, 64, () =>
            {
                PlayerPrefs.Save();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
            FocusFirst();
        }

        private void Back()
        {
            PlayerPrefs.Save();
            if (currentPage == "Confirm") RevertDisplay();
            else if (optionsOnly) { IsOpen = false; Destroy(gameObject); }
            else if (currentPage == "Home") QuitPrompt();
            else if (currentPage == "Mod details") DrawMods();
            else if (currentPage == "Mod ZIP") CancelModZip();
            else Home();
        }

        private void BeginCampaign()
        {
            if (leaving) return;
            leaving = true;
            enterAt = Time.realtimeSinceStartup + .15f;
            PlayerPrefs.Save();
            Clear("Loading");
            Label(page, "Entering the shadows...", 100, 290, 1080, 100, 44, Ink, TextAnchor.MiddleCenter);
        }

        private void Update()
        {
            if (GameSessionRestart.IsRestarting) return;
            if (bindingAction >= 0) { CaptureControllerBinding(); return; }
            if (Time.frameCount == bindingFrame) return;
            if (leaving)
            {
                if (Time.realtimeSinceStartup >= enterAt)
                { enteredCampaign = true; IsOpen = false; Destroy(gameObject); }
                return;
            }
            if (confirmUntil > 0)
            {
                countdown.text = "Reverting in " + Mathf.CeilToInt(confirmUntil - Time.realtimeSinceStartup) + " seconds.";
                if (Time.realtimeSinceStartup >= confirmUntil) { RevertDisplay(); return; }
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape)) { Back(); return; }
            // Explicit navigation avoids dependence on the recovered EventSystem's input axes.
            int delta = UnityEngine.Input.GetKeyDown(KeyCode.DownArrow) || UnityEngine.Input.GetKeyDown(KeyCode.Tab) ? 1 :
                UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) ? -1 : 0;
            if (controls.Count == 0) return;
            var active = EventSystem.current == null ? null : EventSystem.current.currentSelectedGameObject;
            int activeIndex = controls.FindIndex(c => c != null && c.gameObject == active);
            if (activeIndex >= 0) selected = activeIndex;
            if (delta != 0)
            {
                selected = (selected + delta + controls.Count) % controls.Count;
                controls[selected].Select();
            }
            var slider = controls[selected] as Slider;
            if (slider != null)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow)) slider.value -= .05f;
                if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow)) slider.value += .05f;
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Return) || UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                var button = controls[selected] as Button;
                if (button != null && button.interactable) button.onClick.Invoke();
            }
        }

        private void OptionSlider(string title, float y, float value, UnityEngine.Events.UnityAction<float> changed)
        {
            var label = Label(page, title + "  " + Mathf.RoundToInt(value * 100) + "%", 76, y, 480, 40, 24, Ink);
            var track = Box(page, title, 580, y, 580, 40, new Color32(120, 105, 84, 255));
            var handle = Box(track, "Handle", 0, 0, 24, 40, Red);
            var slider = track.gameObject.AddComponent<Slider>();
            slider.targetGraphic = handle.GetComponent<Image>();
            slider.handleRect = handle;
            slider.minValue = 0; slider.maxValue = 1; slider.value = value;
            slider.onValueChanged.AddListener(v => { changed(v); label.text = title + "  " + Mathf.RoundToInt(v * 100) + "%"; });
            controls.Add(slider);
        }

        private void FocusFirst()
        {
            rebuilding = false;
            if (controls.Count > 0)
            {
                int tabIndex = Array.IndexOf(SettingsTabs, currentPage);
                selected = Mathf.Max(0, tabIndex);
                controls[selected].Select();
            }
        }

        private void OnDestroy()
        {
            ClearPendingModZip();
            foreach (var texture in autumnTextures.Values) Destroy(texture);
            if (logoInk != null) Destroy(logoInk);
            if (confirmUntil > 0) Screen.SetResolution(oldResolution.x, oldResolution.y, oldMode);
            IsOpen = false;
            PlayerPrefs.Save();
            if (EventSystem.current != null)
            {
                EventSystem.current.sendNavigationEvents = previousNavigation;
                EventSystem.current.SetSelectedGameObject(previousSelection);
            }
            if (ownedEventSystem != null) Destroy(ownedEventSystem.gameObject);
            optionsClosed?.Invoke();
        }

        private static RectTransform Rect(Transform parent, string name, float x, float y, float w, float h)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(x, -y);
            rect.sizeDelta = new Vector2(w, h);
            return rect;
        }

        private static RectTransform Box(Transform parent, string name, float x, float y, float w, float h, Color color)
        {
            var rect = Rect(parent, name, x, y, w, h);
            rect.gameObject.AddComponent<Image>().color = color;
            return rect;
        }

        private void Picture(Transform parent, string name, string resource, float x, float y, float w, float h, Rect uv)
        {
            var image = Rect(parent, name, x, y, w, h).gameObject.AddComponent<RawImage>();
            image.texture = Resources.Load<Texture2D>(resource);
            image.uvRect = uv;
            if (name.StartsWith("Logo", StringComparison.Ordinal) && logoInk != null) image.material = logoInk;
            image.raycastTarget = false;
            image.enabled = image.texture != null;
        }

        private Text Label(Transform parent, string text, float x, float y, float w, float h, int size, Color color,
            TextAnchor alignment = TextAnchor.MiddleLeft)
        {
            var label = Rect(parent, text, x, y, w, h).gameObject.AddComponent<Text>();
            label.font = font; label.text = text; label.fontSize = size; label.color = color;
            label.alignment = alignment; label.raycastTarget = false;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            return label;
        }

        private Button Button(Transform parent, string text, float x, float y, float w, float h, Action action)
        {
            var rect = Rect(parent, text, x, y, w, h);
            rect.gameObject.AddComponent<CanvasRenderer>();
            Graphic graphic = currentPage == "Home"
                ? (Graphic)rect.gameObject.AddComponent<Image>()
                : rect.gameObject.AddComponent<TitleRibbon>();
            graphic.color = Color.white;
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = graphic;
            var colors = button.colors;
            colors.normalColor = Ink;
            colors.highlightedColor = colors.selectedColor = Red;
            colors.pressedColor = new Color32(105, 30, 24, 255);
            colors.disabledColor = new Color(Ink.r, Ink.g, Ink.b, .42f);
            if (currentPage == "Home")
            {
                colors.normalColor = Color.clear;
                colors.highlightedColor = colors.selectedColor = new Color32(255, 106, 52, 255);
                colors.pressedColor = new Color32(230, 79, 28, 255);
                colors.disabledColor = Color.clear;
            }
            colors.fadeDuration = 0f;
            button.colors = colors;
            if (currentPage == "Home")
                Label(rect, text, 0, 0, w, h, 30, Ink, TextAnchor.MiddleCenter);
            else Label(rect, text, 30, 0, w - 70, h, h > 50 ? 28 : 22, Paper);
            if (action != null) button.onClick.AddListener(() => { if (!rebuilding && bindingAction < 0) action(); });
            controls.Add(button);
            return button;
        }
    }
}
