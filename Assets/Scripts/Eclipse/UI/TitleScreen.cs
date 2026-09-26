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
        private static readonly Color RedBright = new Color32(186, 52, 36, 255);
        private static readonly Color Scrim = new Color(20f / 255f, 14f / 255f, 11f / 255f, .62f);
        private static readonly int[] Caps = { 0, 60, 120, 144, 165, 240, 360 };
        private int bindingAction = -1;
        private int bindingFrame;
        private readonly bool[] heldControllerInputs = new bool[12];
        private bool ControllerPage { get { return currentPage == "Controller"; } }
        private Text bindingStatus;
        private readonly Dictionary<int, Text> bindingLabels = new Dictionary<int, Text>();
        private RectTransform page;
        private RectTransform viewport;
        private RectTransform paperBackground, settingsBackground, footerBackground;
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
        // Presentation: first-open intro, the Home brush highlight and page entrances.
        private static bool introPlayed;
        private static bool splashShown;
        private bool splashing;
        private const float ContentLift = 44f;
        private int contentStart;
        private string previousPage;
        private InkStroke homeStroke;
        private RectTransform leafLayer;
        private Text versusCaption;
        private GameObject versusRow;
        // Focusable Home entries: top and height of each row.
        private readonly Dictionary<GameObject, Vector2> homeRows = new Dictionary<GameObject, Vector2>();
        private GameObject strokeTarget;
        private float strokeY, strokeVelocity, strokeFill;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSession()
        {
            enteredCampaign = false;
            IsOpen = false;
            introPlayed = false;
            seasonChosen = false;
            splashShown = false;
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
            if (!splashShown && !Application.isBatchMode)
            {
                splashShown = true;
                splashing = true;
                StartCoroutine(PlaySplash());
            }
            else Home();
        }

        // Launch splash: fade from black to the splash art, hold, fade back to black, then open
        // the title (which fades up from black with its entrance). Any key or click skips ahead.
        private System.Collections.IEnumerator PlaySplash()
        {
            // Black lead-in, fade in, hold, fade out, black rest. Time advances per rendered frame
            // and never by more than 1/20 s, so launch hitches cannot swallow the fade-in.
            const float Lead = .8f, FadeIn = 3f, Hold = 1f, FadeOut = 2.2f, Rest = .5f;
            const float Total = Lead + FadeIn + Hold + FadeOut + Rest;
            var layer = Rect(transform, "Splash", 0, 0, 1280, 720);
            layer.anchorMin = Vector2.zero; layer.anchorMax = Vector2.one; layer.offsetMin = layer.offsetMax = Vector2.zero;
            var black = layer.gameObject.AddComponent<Image>();
            black.color = Color.black;
            var art = Rect(layer, "Splash art", 0, 0, 0, 0);
            art.anchorMin = Vector2.zero; art.anchorMax = Vector2.one; art.offsetMin = art.offsetMax = Vector2.zero;
            art.pivot = new Vector2(.5f, .5f);
            var image = art.gameObject.AddComponent<RawImage>();
            image.texture = Resources.Load<Texture2D>("EclipseTitle/splash");
            image.color = new Color(1f, 1f, 1f, 0f);
            image.raycastTarget = false;
            if (image.texture != null)
            {
                var fit = art.gameObject.AddComponent<AspectRatioFitter>();
                fit.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
                fit.aspectRatio = (float)image.texture.width / image.texture.height;
            }
            // Let the first (often slow) launch frames pass on black before starting the clock.
            for (int i = 0; i < 3; i++) yield return null;
            float t = 0f;
            float fadeOutAt = Lead + FadeIn + Hold;
            while (t < Total)
            {
                t += Mathf.Min(Time.unscaledDeltaTime, .05f);
                if (t > Lead && t < fadeOutAt - .5f && (UnityEngine.Input.anyKeyDown || UnityEngine.Input.GetMouseButtonDown(0)))
                {
                    // Skip: fade out from wherever the fade-in had reached.
                    float shown = Mathf.Clamp01((t - Lead) / FadeIn);
                    fadeOutAt = t - (1f - shown) * FadeOut;
                }
                float alpha = t < fadeOutAt ? Mathf.Clamp01((t - Lead) / FadeIn) : 1f - Mathf.Clamp01((t - fadeOutAt) / FadeOut);
                alpha = alpha * alpha * (3f - 2f * alpha);
                image.color = new Color(1f, 1f, 1f, alpha);
                // A slow push-in while it is on screen.
                float zoom = 1.025f - .025f * Mathf.Clamp01((t - Lead) / (FadeIn + Hold + FadeOut));
                art.localScale = new Vector3(zoom, zoom, 1f);
                if (t >= fadeOutAt + FadeOut + Rest) break;
                yield return null;
            }
            Home();
            Destroy(layer.gameObject);
            splashing = false;
        }

        private void Start()
        {
            // ShowOptions sets optionsOnly after Awake; the in-game options overlay stays quiet.
            if (optionsOnly) return;
            // Returning to the title can leave the game's own menu or fight music loaded; stop it
            // (this also lets the map start its menu theme afresh after the title closes).
            try { SoundController.NDBJCCIBAIO(); }
            catch (Exception error) { Debug.LogWarning("[Title] Could not stop game music: " + error.Message); }
            // The track follows the scene; on a cold launch it rises slowly under the splash.
            ChooseSeason();
            EclipseUiAudio.StartTitleMusic(SceneMusic, splashing ? 4f : 1.2f);
        }

        private void Clear(string name)
        {
            previousPage = currentPage;
            bindingAction = -1;
            bindingLabels.Clear();
            currentPage = name;
            rebuilding = true;
            controls.Clear();
            selected = 0;
            foreach (Transform child in page)
            {
                if (child == leafLayer) continue;
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
            homeStroke = null;
            homeRows.Clear();
            strokeTarget = null;
            paperBackground = Box(page, "Paper", 0, 0, 1280, 720, Paper);
            DrawScenery();
            // Seasonal particles drift across the whole (up to 21:9) scene. The layer survives
            // page rebuilds so they keep moving instead of respawning, and sits behind every page.
            if (leafLayer == null)
            {
                LayoutViewport();
                LayoutPanorama(Vector2.zero);
                leafLayer = Rect(page, "Seasonal particles", 0, 0, 1280, 720);
                ScatterParticles(leafLayer);
            }
            leafLayer.SetAsLastSibling();
            settingsBackground = null;
            bool panelPage = name != "Home" && name != "Loading";
            bool fromHome = previousPage == "Home";
            if (name != "Home")
            {
                // The autumn scene stays visible behind an ink wash; content sits on a paper card.
                settingsBackground = Box(page, "Ink wash", 0, 0, 1280, 720, name == "Loading" ? Ink : Scrim);
                if (fromHome || name == "Loading") UiReveal.Play(settingsBackground, 0f, name == "Loading" ? .45f : .28f, Vector2.zero);
            }
            if (panelPage)
            {
                var card = Rect(page, "Paper card", 40, 26, 1200, 636);
                var paper = card.gameObject.AddComponent<PaperPanel>();
                paper.color = Paper;
                paper.raycastTarget = false;
                if (fromHome) UiReveal.Play(card, .02f, .34f, new Vector2(0, -16), .965f);
            }
            DrawFooter();
            contentStart = page.childCount;
            LayoutViewport();
        }

        private void LateUpdate()
        {
            LayoutViewport();
            ApplyParallax();
        }

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
            viewLeft = left;
            viewWidth = width;
            // Keep the authored gate, logo and controls at their original proportions.
            // Only the surrounding scenery, paper and footer fill the wider viewport.
            Place(paperBackground, left, top, width, height);
            Place(settingsBackground, left, top, width, height);
            Place(skyLeft, left, top, width * .5f, height - 46);
            Place(skyRight, 640, top, width * .5f, height - 46);
            Place(footerBackground, left, top + height - 46, width, 46);
        }

        private static void Place(RectTransform rect, float x, float y, float width, float height)
        {
            if (rect == null) return;
            rect.anchoredPosition = new Vector2(x, -y);
            rect.sizeDelta = new Vector2(width, height);
        }

        // The gate and the two trees are tiles of one painting (wall, ground and leaf carpet run
        // across their edges), so they must move together; only the sky behind has its own depth.
        private const float GroundPlaneDepth = .6f;

        private void DrawAutumnGate()
        {
            const string root = "Textures/Locations/autumn/";
            skyLeft = PackedPicture("Sky left", root + "autumn_bg", 0, 0, 640, 674, new Rect(0, 0, 1, .5f))?.rectTransform;
            skyRight = PackedPicture("Sky right", root + "autumn_bg", 640, 0, 640, 674, new Rect(0, .5f, 1, .5f))?.rectTransform;
            AddLayer(skyLeft, .15f, true);
            AddLayer(skyRight, .15f, true);
            // The eclipse sits in the strip of sky above the roof, behind the gate and trees.
            DrawSun(24f);
            sunRoot.anchoredPosition = new Vector2(862, -46);
            AddLayer(sunRoot, .2f);
            groundY = 662f;
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
                AddLayer(rect, GroundPlaneDepth);
            }
            AddLayer(PackedPicture("Right tree", root + "autumn_atlas_layer1", 1036, originY + 24 * scale, 792, 488 * scale,
                new Rect(3f / 1024, 533f / 1024, 512f / 1024, 488f / 1024))?.rectTransform, GroundPlaneDepth);
            AddLayer(PackedPicture("Gate", root + "autumn_atlas_layer1", 244, 22, 792, 446 * scale,
                new Rect(3f / 1024, 83f / 1024, 512f / 1024, 446f / 1024))?.rectTransform, GroundPlaneDepth);
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
            var image = Rect(scenery, name, x, y, w, h).gameObject.AddComponent<RawImage>();
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
            DrawPlaque();
            DrawMenuWash();
            // One brush stroke under the labels glides to whichever entry has focus.
            var stroke = Rect(page, "Brush highlight", 385, 280, 510, 60);
            homeStroke = stroke.gameObject.AddComponent<InkStroke>();
            homeStroke.color = new Color(Red.r, Red.g, Red.b, .92f);
            homeStroke.raycastTarget = false;
            homeStroke.Fill = 0f;
            // Campaign leads; Quit lives in the footer (and on Esc).
            var campaign = HomeButton("CAMPAIGN", 272, 74, BeginCampaign, UiSound.Begin, 38);
            string progress = ContinueLine();
            if (progress != null)
            {
                var caption = campaign.GetComponentInChildren<Text>();
                caption.rectTransform.anchoredPosition += new Vector2(0, 8);
                campaign.GetComponent<EclipseUiButton>().Rehome();
                var line = Label(campaign.transform, progress.ToUpperInvariant(), 0, 52, 470, 18, 12,
                    new Color(Paper.r, Paper.g, Paper.b, .75f), TextAnchor.MiddleCenter);
                line.horizontalOverflow = HorizontalWrapMode.Overflow;
            }
            versusRow = HomeButton("MULTIPLAYER", 360, 56, () =>
            {
                Eclipse.Multiplayer.LocalVersusSession.RequestEntry();
                BeginCampaign();
            }, UiSound.Begin, 29);
            versusCaption = Label(page, "LOCAL VERSUS", 405, 407, 470, 18, 12, new Color(Paper.r, Paper.g, Paper.b, .7f), TextAnchor.MiddleCenter);
            HomeButton("MODS", 440, 52, OpenMods, UiSound.Open, 27);
            HomeButton("OPTIONS", 496, 52, () => Settings("Display"), UiSound.Open, 27);
            FocusFirst();
        }

        private GameObject HomeButton(string text, float y, float h, Action action, UiSound sound, int size)
        {
            var button = Button(page, text, 405, y, 470, h, action, sound);
            button.GetComponentInChildren<Text>().fontSize = size;
            homeRows[button.gameObject] = new Vector2(y, h);
            return button.gameObject;
        }

        // Eases the brush stroke to the focused Home entry and repaints it on each move.
        private void UpdateHomeStroke()
        {
            if (homeStroke == null) return;
            var focused = EventSystem.current == null ? null : EventSystem.current.currentSelectedGameObject;
            Vector2 row;
            // Wait for the entry itself to finish its entrance before painting under it.
            if (focused == null || !homeRows.TryGetValue(focused, out row) || focused.GetComponent<UiReveal>() != null)
            {
                strokeFill = Mathf.MoveTowards(strokeFill, 0f, Time.unscaledDeltaTime / .12f);
                homeStroke.Fill = strokeFill;
                return;
            }
            float targetY = row.x;
            if (focused != strokeTarget)
            {
                if (strokeTarget == null) { strokeY = targetY; strokeVelocity = 0f; }
                strokeTarget = focused;
                strokeFill = 0f;
                homeStroke.Seed = UnityEngine.Random.Range(1, 999);
            }
            strokeY = Mathf.SmoothDamp(strokeY, targetY, ref strokeVelocity, .07f, Mathf.Infinity, Time.unscaledDeltaTime);
            strokeFill = Mathf.MoveTowards(strokeFill, 1f, Time.unscaledDeltaTime / .2f);
            homeStroke.Fill = 1f - (1f - strokeFill) * (1f - strokeFill);
            var rect = homeStroke.rectTransform;
            rect.anchoredPosition = new Vector2(385, -strokeY);
            rect.sizeDelta = new Vector2(510, Mathf.Lerp(rect.sizeDelta.y, row.y + 4f, Time.unscaledDeltaTime * 14f));
        }

        // Entrance for everything built after the page's backgrounds. The Home menu and
        // first page open stagger in; switching Options tabs only moves the tab body.
        private void RevealContent()
        {
            bool tabSwitch = Array.IndexOf(SettingsTabs, currentPage) >= 0 && Array.IndexOf(SettingsTabs, previousPage) >= 0
                && currentPage != previousPage;
            // Redrawing the same page (a mod toggle, paging) should update in place, not replay.
            if (currentPage == previousPage && currentPage != "Home") return;
            bool intro = currentPage == "Home" && !introPlayed && !optionsOnly;
            float start = intro ? .55f : currentPage == "Home" ? .04f : tabSwitch ? 0f : .1f;
            int order = 0;
            for (int i = contentStart; i < page.childCount; i++)
            {
                var child = page.GetChild(i) as RectTransform;
                if (child == null || child.GetComponent<InkStroke>() != null) continue;
                float top = -child.anchoredPosition.y;
                if (tabSwitch && top < 230f - ContentLift) continue;
                bool sign = currentPage == "Home" && top < 240f;
                Vector2 offset = sign ? new Vector2(0, 22) : tabSwitch ? new Vector2(18, 0) : new Vector2(0, -14);
                float delay = sign ? (intro ? .2f : 0f) : start + Mathf.Min(order++ * (tabSwitch ? .015f : .03f), .4f);
                UiReveal.Play(child, delay, sign ? .6f : .3f, offset);
            }
            if (intro)
            {
                introPlayed = true;
                // Open from black: the ink lifts off the scene.
                var veil = Box(transform, "Intro veil", 0, 0, 1280, 720, Ink);
                veil.anchorMin = Vector2.zero; veil.anchorMax = Vector2.one; veil.offsetMin = veil.offsetMax = Vector2.zero;
                veil.GetComponent<Image>().raycastTarget = false;
                StartCoroutine(FadeAway(veil.GetComponent<Image>(), .1f, 1.4f));
            }
        }

        private static System.Collections.IEnumerator FadeAway(Graphic graphic, float delay, float duration)
        {
            float start = Time.unscaledTime + delay;
            Color color = graphic.color;
            while (graphic != null)
            {
                float t = Mathf.Clamp01((Time.unscaledTime - start) / duration);
                color.a = 1f - t * t * (3f - 2f * t);
                graphic.color = color;
                if (t >= 1f) { Destroy(graphic.gameObject); yield break; }
                yield return null;
            }
        }
        private void Settings(string tab)
        {
            Clear(tab);
            Label(page, "Options", 76, 96, 550, 64, 46, Ink);
            string[] tabs = SettingsTabs;
            for (int i = 0; i < tabs.Length; i++)
            {
                string target = tabs[i];
                Button(page, target, 76 + i * 188, 180, 180, 48, () => { if (currentPage != target) Settings(target); },
                    UiSound.Tab, Look.Tab, tab == target);
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
                var apply = Button(page, "Apply display", 852, 604, 340, 48, ApplyDisplay);
                apply.interactable = !Application.isMobilePlatform;
                Label(page, "Window and resolution changes require confirmation. Rendering options save immediately.", 76, 558, 1120, 36, 15, Ink);
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
                        x + 270, y, 236, 38, () => BeginBinding(action), UiSound.Confirm, Look.Field);
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
            Button(page, "Back", 76, 604, 220, 48, Back, UiSound.Back);
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
            }, UiSound.Toggle, Look.Field);
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
            Button(page, "Revert", 650, 442, 470, 64, RevertDisplay, UiSound.Back);
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
            Button(page, "Stay", 100, 440, 470, 64, Home, UiSound.Back);
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
            // The loading screen fades in over the title (and the gong) and stays up until the
            // campaign or the local versus lobby is actually on screen.
            enterAt = Time.realtimeSinceStartup + .6f;
            PlayerPrefs.Save();
            EclipseUiAudio.StopTitleMusic();
            EclipseLoadingOverlay.Show();
        }

        private void Update()
        {
            UpdateHomeStroke();
            UpdateParallax();
            UpdateGust();
            UpdateEclipse();
            UpdateInputDevice();
            if (GameSessionRestart.IsRestarting || splashing) return;
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
            bool padConfirm, padBack;
            int padHorizontal;
            int padVertical = PadNavigation(out padConfirm, out padBack, out padHorizontal);
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) || padBack) { EclipseUiAudio.Play(UiSound.Back); Back(); return; }
            // Explicit navigation avoids dependence on the recovered EventSystem's input axes.
            int delta = UnityEngine.Input.GetKeyDown(KeyCode.DownArrow) || UnityEngine.Input.GetKeyDown(KeyCode.Tab) ? 1 :
                UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) ? -1 : padVertical;
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
                if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow) || padHorizontal < 0) slider.value -= .05f;
                if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow) || padHorizontal > 0) slider.value += .05f;
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Return) || UnityEngine.Input.GetKeyDown(KeyCode.Space) || padConfirm)
            {
                var button = controls[selected] as Button;
                if (button != null && button.interactable) button.onClick.Invoke();
            }
        }

        private void OptionSlider(string title, float y, float value, UnityEngine.Events.UnityAction<float> changed)
        {
            var label = Label(page, title + "  " + Mathf.RoundToInt(value * 100) + "%", 76, y, 480, 40, 24, Ink);
            // A transparent hit area holding a thin ink groove, a red fill and a round knob.
            var root = Box(page, title, 580, y, 580, 40, Color.clear);
            var groove = Stretched(Box(root, "Groove", 0, 0, 0, 0, new Color(Ink.r, Ink.g, Ink.b, .22f)), 0f, 1f, 8f);
            groove.GetComponent<Image>().raycastTarget = false;
            var fillArea = Stretched(Rect(root, "Fill area", 0, 0, 0, 0), 0f, 1f, 8f);
            var fill = Box(fillArea, "Fill", 0, 0, 0, 0, Red);
            fill.anchorMin = Vector2.zero; fill.anchorMax = new Vector2(0, 1); fill.pivot = new Vector2(0, .5f);
            fill.anchoredPosition = Vector2.zero; fill.sizeDelta = Vector2.zero;
            fill.GetComponent<Image>().raycastTarget = false;
            var handleArea = Stretched(Rect(root, "Knob area", 0, 0, 0, 0), 0f, 1f, 40f);
            var handle = Rect(handleArea, "Knob", 0, 0, 0, 0);
            handle.anchorMin = handle.anchorMax = handle.pivot = new Vector2(.5f, .5f);
            handle.anchoredPosition = Vector2.zero; handle.sizeDelta = new Vector2(30, 30);
            var knob = handle.gameObject.AddComponent<UiDisc>();
            knob.color = Ink;
            knob.SetRing(Paper, 3f);
            var slider = root.gameObject.AddComponent<Slider>();
            slider.fillRect = fill;
            slider.handleRect = handle;
            slider.targetGraphic = knob;
            slider.minValue = 0; slider.maxValue = 1; slider.value = value;
            int notch = Mathf.RoundToInt(value * 20);
            slider.onValueChanged.AddListener(v =>
            {
                changed(v);
                label.text = title + "  " + Mathf.RoundToInt(v * 100) + "%";
                int now = Mathf.RoundToInt(v * 20);
                if (now != notch) { notch = now; EclipseUiAudio.Play(UiSound.Tick); }
            });
            EclipseUiButton.Attach(slider, knob, null, Ink, Red, Paper, Paper, 0f, 0f, 0f);
            controls.Add(slider);
        }

        // Spans the parent horizontally (inset by the knob radius) at a fixed height, centred.
        private static RectTransform Stretched(RectTransform rect, float min, float max, float height)
        {
            rect.anchorMin = new Vector2(min, .5f);
            rect.anchorMax = new Vector2(max, .5f);
            rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(-30f, height);
            return rect;
        }

        private void FocusFirst()
        {
            rebuilding = false;
            // Pages were authored for a full-screen sheet; on the card they sit higher, closing
            // the gap above the title and keeping the bottom buttons inside the card.
            if (currentPage != "Home")
                for (int i = contentStart; i < page.childCount; i++)
                {
                    var child = page.GetChild(i) as RectTransform;
                    if (child != null) child.anchoredPosition += new Vector2(0, ContentLift);
                }
            RevealContent();
            EclipseUiAudio.SuppressFocusSound();
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
            if (!optionsOnly) EclipseUiAudio.StopTitleMusic();
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

        private InkStroke Stroke(Transform parent, string name, float x, float y, float w, float h, string seed)
        {
            var stroke = Rect(parent, name, x, y, w, h).gameObject.AddComponent<InkStroke>();
            stroke.raycastTarget = false;
            stroke.Seed = seed.GetHashCode() & 0xffff;
            return stroke;
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

        // Plate: a brush-stroke button for actions. Field: an editable value or binding shown as
        // text on an ink underline. Tab: a caption with a brush underline marking the open tab.
        private enum Look { Plate, Field, Tab }

        private Button Button(Transform parent, string text, float x, float y, float w, float h, Action action,
            UiSound sound = UiSound.Confirm, Look look = Look.Plate, bool current = false)
        {
            var rect = Rect(parent, text, x, y, w, h);
            // A transparent hit area covers the whole control; the visible parts are children.
            var hit = rect.gameObject.AddComponent<Image>();
            hit.color = Color.clear;
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = hit;
            EclipseUiButton fx;
            if (currentPage == "Home")
            {
                // Home entries have no plate: the shared brush stroke marks focus instead.
                var caption = Label(rect, text, 0, 0, w, h, 30, Paper, TextAnchor.MiddleCenter);
                var outline = caption.gameObject.AddComponent<Shadow>();
                outline.effectColor = new Color(Ink.r, Ink.g, Ink.b, .7f);
                outline.effectDistance = new Vector2(1.5f, -2f);
                fx = EclipseUiButton.Attach(button, null, caption, Color.clear, Color.clear,
                    new Color(Paper.r, Paper.g, Paper.b, .88f), Paper, 0f, .07f);
            }
            else if (look == Look.Plate)
            {
                var plate = Stroke(rect, "Plate", 0, 0, w, h, text);
                var caption = Label(rect, text, 30, 0, w - 60, h, h > 50 ? 28 : 22, Paper);
                fx = EclipseUiButton.Attach(button, plate, caption, Ink, Red, Paper, Paper, 6f, .02f);
            }
            else if (look == Look.Field)
            {
                var line = Stroke(rect, "Underline", 0, h - 9, w, 7, text);
                var caption = Label(rect, text, 14, 0, w - 28, h - 6, h > 40 ? 24 : 21, Ink);
                fx = EclipseUiButton.Attach(button, line, caption, new Color(Ink.r, Ink.g, Ink.b, .35f), Red, Ink, Red, 6f, 0f, .03f);
            }
            else
            {
                var line = Stroke(rect, "Underline", 14, h - 10, w - 28, 8, text);
                var caption = Label(rect, text, 0, 0, w, h - 8, 21, current ? Red : Ink, TextAnchor.MiddleCenter);
                fx = EclipseUiButton.Attach(button, line, caption, current ? Red : Color.clear,
                    current ? RedBright : new Color(Ink.r, Ink.g, Ink.b, .55f), current ? Red : Ink, Red, 0f, .03f);
            }
            if (action != null) button.onClick.AddListener(() =>
            {
                if (rebuilding || bindingAction >= 0) return;
                fx.Punch();
                EclipseUiAudio.Play(sound);
                action();
            });
            controls.Add(button);
            return button;
        }
    }
}
