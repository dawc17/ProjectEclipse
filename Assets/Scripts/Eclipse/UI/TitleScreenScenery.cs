using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // The living title scene: a seasonal backdrop chosen per launch, parallax depth with a
    // slow breathing zoom, an eclipse in the sky, periodic wind gusts, and the footer
    // with device-aware hints.
    public sealed partial class TitleScreen
    {
        private enum Season { Autumn, Spring }

        // Title music per scene (Resources paths). The ambience is an Eclipse-owned track;
        // Fuji is the game's own fight18_fuji.
        private static string SceneMusic => season == Season.Spring ? "gamedata/music/fight18_fuji" : "EclipseTitle/the_ambience";

        // A packaged seasonal dojo panorama. Rects are in sprite pixels, top-left origin.
        private sealed class Panorama
        {
            public string Address;
            public Rect Art;          // the painted area inside the sprite
            public float FocusX;      // horizontal centre to keep in view
            public float Ground;      // floor line, where leaves settle
            public Vector2? Sun;      // sky position for the eclipse, when the scene has sky
            public float SunRadius;
        }

        private static readonly Dictionary<Season, Panorama> Panoramas = new Dictionary<Season, Panorama>
        {
            // Lunar New Year village at night, lanterns lit; the eclipse rises over the peaks.
            { Season.Spring, new Panorama { Address = "Textures/Locations/dojo_cny25/StaticImage/BG",
                Art = new Rect(0, 0, 1960, 653), FocusX = 918, Ground = 552, Sun = new Vector2(1290, 96), SunRadius = 30 } },
        };

        private const string SeasonPreference = "Eclipse.TitleSceneIndex";
        private static Season season;
        private static bool seasonChosen;

        private sealed class Layer { public RectTransform Rect; public Vector2 Home; public float Depth; public bool Live; }
        private readonly List<Layer> layers = new List<Layer>();
        private RectTransform scenery;
        private RawImage panoramaImage;
        private Sprite panoramaSprite;
        private Vector2 parallax, parallaxVelocity;
        private float groundY = 662f;
        private float viewLeft, viewWidth = 1280f;

        // Eclipse
        private RectTransform sunRoot;
        private UiDisc moonDisc;
        private RawImage sunGlow, corona;
        private Image totality;
        private float sunRadius, eclipse, eclipseVelocity, sceneOpened;

        // Wind
        private float nextGust = 6f, gustAt = -99f;

        // Plaque
        private RectTransform plaque;
        private UiDisc sealMoon;

        // Footer
        private RectTransform footerHints;
        private Text footerBrand;
        private bool padMode;
        private Vector2 padStickLast;

        private static Texture2D softDot, ringTexture;

        private static void ChooseSeason()
        {
            if (seasonChosen) return;
            seasonChosen = true;
            int index = 0;
            try { index = PlayerPrefs.GetInt(SeasonPreference, 0); PlayerPrefs.SetInt(SeasonPreference, index + 1); PlayerPrefs.Save(); }
            catch (Exception error) { Debug.LogWarning("[Title] Scene rotation unavailable: " + error.Message); }
            season = (Season)(Mathf.Abs(index) % 2);
        }

        // --- Scene construction (called from Clear) ---------------------------------------

        private void DrawScenery()
        {
            ChooseSeason();
            layers.Clear();
            panoramaImage = null;
            sunRoot = null; moonDisc = null; sunGlow = corona = null; totality = null;
            scenery = Rect(page, "Scenery", 0, 0, 1280, 720);
            scenery.pivot = new Vector2(.5f, .5f);
            scenery.anchoredPosition = new Vector2(640, -360);
            if (sceneOpened <= 0f) sceneOpened = Time.unscaledTime;
            bool panorama = season != Season.Autumn && TryDrawPanorama();
            if (!panorama) DrawAutumnGate();
            if (totality != null) totality.transform.SetAsLastSibling();
            // On a panorama the sun sits over the painted sky; at the gate it hides behind the roof and trees.
            if (panorama && sunRoot != null) sunRoot.SetAsLastSibling();
        }

        private bool TryDrawPanorama()
        {
            var info = Panoramas[season];
            if (panoramaSprite == null) panoramaSprite = Eclipse.Content.PackagedArtCatalog.Load<Sprite>(info.Address);
            if (panoramaSprite == null)
            {
                Debug.LogWarning("[Title] Missing seasonal scene " + info.Address + "; showing the autumn gate.");
                season = Season.Autumn;
                return false;
            }
            panoramaImage = Rect(scenery, "Panorama", 0, 0, 1280, 720).gameObject.AddComponent<RawImage>();
            panoramaImage.texture = panoramaSprite.texture;
            panoramaImage.raycastTarget = false;
            if (info.Sun.HasValue) DrawSun(info.SunRadius);
            return true;
        }

        // Autumn leaves at the gate, cherry petals over the spring village.
        private void ScatterParticles(RectTransform parent)
        {
            if (season == Season.Spring)
                TitleLeaf.Scatter(parent, 30, TitleLeaf.Kind.Petal, viewLeft - 300f, viewLeft + viewWidth + 100f, -40f, 700f, groundY);
            else
                TitleLeaf.Scatter(parent, 24, TitleLeaf.Kind.Leaf, -400f, 1600f, -40f, 700f, groundY);
        }

        private void AddLayer(RectTransform rect, float depth, bool live = false)
        {
            if (rect != null) layers.Add(new Layer { Rect = rect, Home = rect.anchoredPosition, Depth = depth, Live = live });
        }

        // Where the floor should land on screen: above the footer, below the menu.
        private const float GroundTarget = 648f;

        // The visible part of the panorama: fills the viewport (with 4% overscan for the
        // parallax), keeps FocusX centred and puts the floor near GroundTarget when it can.
        private void Crop(Vector2 offset, out float cover, out float cropX, out float cropY, out float cropW, out float cropH)
        {
            var info = Panoramas[season];
            cover = Mathf.Max(viewWidth / info.Art.width, 720f / info.Art.height) * 1.04f;
            cropW = viewWidth / cover;
            cropH = 720f / cover;
            float slackX = info.Art.width - cropW, slackY = info.Art.height - cropH;
            cropX = Mathf.Clamp(info.FocusX - info.Art.x - cropW * .5f - offset.x / cover, 0f, slackX);
            cropY = Mathf.Clamp(info.Ground - info.Art.y - GroundTarget / cover - offset.y / cover, 0f, slackY);
        }

        // Maps a point in the panorama's sprite pixels to page coordinates for the current crop.
        private Vector2 PanoramaToPage(Vector2 sprite, out float scale)
        {
            var info = Panoramas[season];
            float cropX, cropY, cropW, cropH;
            Crop(Vector2.zero, out scale, out cropX, out cropY, out cropW, out cropH);
            return new Vector2(viewLeft + (sprite.x - info.Art.x - cropX) * scale, (sprite.y - info.Art.y - cropY) * scale);
        }

        private void LayoutPanorama(Vector2 offset)
        {
            if (panoramaImage == null || panoramaSprite == null) return;
            var info = Panoramas[season];
            float cover, cropX, cropY, cropW, cropH;
            Crop(offset, out cover, out cropX, out cropY, out cropW, out cropH);
            var texture = panoramaSprite.texture;
            var sprite = panoramaSprite.textureRect;
            // Sprite rect is bottom-left in texture pixels; Art is top-left inside the sprite.
            float u = (sprite.x + info.Art.x + cropX) / texture.width;
            float vTop = sprite.y + sprite.height - (info.Art.y + cropY);
            float v = (vTop - cropH) / texture.height;
            panoramaImage.uvRect = new Rect(u, v, cropW / texture.width, cropH / texture.height);
            var rect = panoramaImage.rectTransform;
            rect.anchoredPosition = new Vector2(viewLeft, 0);
            rect.sizeDelta = new Vector2(viewWidth, 720);
            float unused;
            groundY = Mathf.Min(PanoramaToPage(new Vector2(0, info.Ground), out unused).y, 700f);
            if (sunRoot != null && info.Sun.HasValue)
                sunRoot.anchoredPosition = PanoramaToPage(info.Sun.Value, out unused) + new Vector2(offset.x * .3f, -offset.y * .3f);
        }

        // --- The eclipse -------------------------------------------------------------------

        private void DrawSun(float radius)
        {
            sunRadius = radius;
            sunRoot = Rect(scenery, "Eclipse", 0, 0, 0, 0);
            sunRoot.pivot = new Vector2(.5f, .5f);
            sunGlow = Soft(sunRoot, "Sun glow", radius * 7f, SoftDot(), new Color(1f, .86f, .55f, .55f));
            var sun = Centered(sunRoot, "Sun", radius * 2f).gameObject.AddComponent<UiDisc>();
            sun.color = new Color32(255, 243, 214, 255);
            sun.raycastTarget = false;
            corona = Soft(sunRoot, "Corona", radius * 3.6f, Ring(), new Color(1f, .93f, .78f, 0f));
            moonDisc = Centered(sunRoot, "Moon", radius * 2.06f).gameObject.AddComponent<UiDisc>();
            moonDisc.color = new Color32(26, 18, 15, 255);
            moonDisc.raycastTarget = false;
            // The world dims slightly at totality.
            totality = Rect(scenery, "Totality", -400, 0, 2080, 720).gameObject.AddComponent<Image>();
            totality.color = new Color(.05f, .03f, .06f, 0f);
            totality.raycastTarget = false;
        }

        private static RectTransform Centered(Transform parent, string name, float size)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
            rect.sizeDelta = new Vector2(size, size);
            return rect;
        }

        private static RawImage Soft(Transform parent, string name, float size, Texture2D texture, Color color)
        {
            var image = Centered(parent, name, size).gameObject.AddComponent<RawImage>();
            image.texture = texture;
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static Texture2D SoftDot()
        {
            if (softDot != null) return softDot;
            softDot = Radial(64, d => { float a = Mathf.Clamp01(1f - d); return a * a; });
            return softDot;
        }

        private static Texture2D Ring()
        {
            if (ringTexture != null) return ringTexture;
            ringTexture = Radial(128, d =>
            {
                float ring = Mathf.Exp(-Mathf.Pow((d - .56f) / .09f, 2f));
                float halo = Mathf.Clamp01(1f - d) * Mathf.Clamp01((d - .5f) / .1f) * .45f;
                return Mathf.Clamp01(ring + halo);
            });
            return ringTexture;
        }

        private static Texture2D Radial(int size, Func<float, float> alpha)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            var pixels = new Color32[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float dx = (x + .5f) / size * 2f - 1f, dy = (y + .5f) / size * 2f - 1f;
                    pixels[y * size + x] = new Color(1f, 1f, 1f, alpha(Mathf.Sqrt(dx * dx + dy * dy)));
                }
            texture.SetPixels32(pixels);
            texture.Apply();
            return texture;
        }

        private void UpdateEclipse()
        {
            float target = 0f;
            float since = Time.unscaledTime - sceneOpened;
            if (since > 1.2f) target = .62f;
            var events = UnityEngine.EventSystems.EventSystem.current;
            var focused = events != null ? events.currentSelectedGameObject : null;
            // Totality while the way into the story is chosen.
            if (focused != null && (focused.name == "CAMPAIGN" || focused.name == "MULTIPLAYER") && since > 1.2f) target = 1f;
            eclipse = Mathf.SmoothDamp(eclipse, target, ref eclipseVelocity, .9f, Mathf.Infinity, Time.unscaledDeltaTime);
            float cover = Mathf.Clamp01(eclipse);
            float total = Mathf.Pow(cover, 8f);
            if (moonDisc != null)
            {
                moonDisc.rectTransform.anchoredPosition = new Vector2(-2.3f, .55f) * sunRadius * (1f - cover);
                sunGlow.color = new Color(1f, .86f, .55f, .55f * (1f - .8f * total));
                corona.color = new Color(1f, .93f, .78f, .95f * total);
                corona.rectTransform.localScale = Vector3.one * (1f + .03f * Mathf.Sin(Time.unscaledTime * 1.7f));
                totality.color = new Color(.05f, .03f, .06f, .2f * total);
            }
            if (sealMoon != null)
                sealMoon.rectTransform.anchoredPosition = new Vector2(-9f, 2.5f) * (1f - cover);
        }

        // --- Wind --------------------------------------------------------------------------

        private void UpdateGust()
        {
            float now = Time.unscaledTime;
            if (now >= nextGust)
            {
                gustAt = now;
                nextGust = now + UnityEngine.Random.Range(11f, 19f);
                EclipseUiAudio.Play(UiSound.Gust);
            }
            float t = now - gustAt;
            // Rise over .7 s, hold 1.2 s, die away over 2.4 s.
            float gust = t < 0f ? 0f : t < .7f ? t / .7f : t < 1.9f ? 1f : Mathf.Clamp01(1f - (t - 1.9f) / 2.4f);
            TitleLeaf.Gust = gust * gust * (3f - 2f * gust);
        }

        // --- Parallax ----------------------------------------------------------------------

        private void UpdateParallax()
        {
            float t = Time.unscaledTime;
            Vector2 target = new Vector2(Mathf.Sin(t * .11f) * .35f, Mathf.Sin(t * .083f) * .25f);
            if (Application.isFocused && Screen.width > 0 && Screen.height > 0)
            {
                Vector2 mouse = UnityEngine.Input.mousePosition;
                if (mouse.x >= 0 && mouse.y >= 0 && mouse.x <= Screen.width && mouse.y <= Screen.height)
                    target += new Vector2(mouse.x / Screen.width - .5f, mouse.y / Screen.height - .5f) * 1.4f;
            }
            parallax = Vector2.SmoothDamp(parallax, Vector2.ClampMagnitude(target, 1.2f), ref parallaxVelocity, .55f,
                Mathf.Infinity, Time.unscaledDeltaTime);
        }

        // Applied after LayoutViewport so live layers (the sky halves) build on its placement.
        private void ApplyParallax()
        {
            if (scenery == null) return;
            // Move opposite the pointer; deeper layers move less.
            Vector2 offset = new Vector2(-parallax.x, parallax.y) * 16f;
            foreach (var layer in layers)
            {
                if (layer.Rect == null) continue;
                var home = layer.Live ? layer.Rect.anchoredPosition : layer.Home;
                layer.Rect.anchoredPosition = home + offset * layer.Depth;
            }
            LayoutPanorama(offset * .35f);
            // Slow breathing zoom of the whole scene.
            float breath = 1.012f + .012f * Mathf.Sin(Time.unscaledTime * .09f);
            // On opening, the scene settles in from slightly closer over about three seconds.
            float settle = 1f - Mathf.Clamp01((Time.unscaledTime - sceneOpened) / 3.2f);
            breath += .07f * settle * settle * settle;
            scenery.localScale = new Vector3(breath, breath, 1f);
            if (plaque != null)
                plaque.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.unscaledTime * .8f) * .9f + TitleLeaf.Gust * 2.6f);
        }

        // --- Home decorations ----------------------------------------------------------------

        // The name plate hanging under the logo sign, with a red hanko seal whose small sun
        // is eclipsed in step with the sky.
        private void DrawPlaque()
        {
            plaque = Rect(page, "Eclipse plaque", 640, 192, 0, 0);
            plaque.pivot = new Vector2(.5f, 1f);
            var wood = (Color)new Color32(73, 43, 29, 255);
            Box(plaque, "Cord left", -72, 0, 2, 12, Ink);
            Box(plaque, "Cord right", 70, 0, 2, 12, Ink);
            Box(plaque, "Plate border", -118, 11, 236, 34, Ink);
            Box(plaque, "Plate", -115, 14, 230, 28, wood);
            Box(plaque, "Grain", -115, 27, 230, 1, new Color32(103, 66, 43, 255));
            var caption = Label(plaque, "P R O J E C T    E C L I P S E", -108, 14, 190, 28, 13, Paper, TextAnchor.MiddleCenter);
            caption.horizontalOverflow = HorizontalWrapMode.Overflow;
            var seal = Centered(plaque, "Hanko", 30).gameObject.AddComponent<UiDisc>();
            seal.color = Red;
            seal.SetRing(new Color32(120, 24, 18, 255), 2f);
            seal.raycastTarget = false;
            seal.rectTransform.anchorMin = seal.rectTransform.anchorMax = new Vector2(0, 1);
            seal.rectTransform.anchoredPosition = new Vector2(98, -28);
            var sun = Centered(seal.rectTransform, "Seal sun", 12).gameObject.AddComponent<UiDisc>();
            sun.color = Paper; sun.raycastTarget = false;
            sealMoon = Centered(seal.rectTransform, "Seal moon", 12.5f).gameObject.AddComponent<UiDisc>();
            sealMoon.color = Red; sealMoon.raycastTarget = false;
        }

        // A soft ink wash behind the menu keeps the labels legible on every scene.
        private void DrawMenuWash()
        {
            var wash = Rect(page, "Menu wash", 300, 232, 680, 360).gameObject.AddComponent<RawImage>();
            wash.texture = SoftDot();
            wash.color = new Color(Ink.r, Ink.g, Ink.b, .62f);
            wash.raycastTarget = false;
        }

        // "Act VI  ·  Level 52" from the local save, shown under Campaign. Absent on a new install.
        private static string ContinueLine()
        {
            try
            {
                string path = Path.Combine(Application.persistentDataPath, "userdata", "users.xml");
                if (!File.Exists(path)) return null;
                var document = new XmlDocument { XmlResolver = null };
                using (var reader = XmlReader.Create(path, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null }))
                    document.Load(reader);
                var warrior = document.SelectSingleNode("//Warrior[@ID='1']") as XmlElement;
                if (warrior == null) return null;
                string zone = warrior.GetAttribute("CurrentZone");
                string level = warrior.GetAttribute("Level");
                int act;
                string actText = zone.StartsWith("ZONE_", StringComparison.Ordinal) && int.TryParse(zone.Substring(5), out act) && act > 0
                    ? "Act " + Roman(act) : null;
                if (string.IsNullOrEmpty(level)) return actText;
                return actText == null ? "Level " + level : actText + "   ·   Level " + level;
            }
            catch (Exception error)
            {
                Debug.Log("[Title] No readable save summary: " + error.Message);
                return null;
            }
        }

        private static string Roman(int value)
        {
            string[] numerals = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" };
            return value < numerals.Length ? numerals[value] : value.ToString();
        }

        // --- Footer --------------------------------------------------------------------------

        private void DrawFooter()
        {
            footerBackground = Box(page, "Footer", 0, 674, 1280, 46, new Color(.09f, .066f, .052f, .94f));
            var rule = Box(footerBackground, "Rule", 0, 0, 0, 2, Red);
            rule.anchorMin = new Vector2(0, 1); rule.anchorMax = new Vector2(1, 1); rule.sizeDelta = new Vector2(0, 2); rule.anchoredPosition = Vector2.zero;
            footerBrand = Label(footerBackground, Brand(), 28, 9, 640, 30, 14, new Color(Paper.r, Paper.g, Paper.b, .6f));
            footerHints = Rect(footerBackground, "Hints", 0, 0, 0, 46);
            footerHints.anchorMin = footerHints.anchorMax = footerHints.pivot = new Vector2(1, 1);
            footerHints.anchoredPosition = new Vector2(-24, 0);
            BuildHints();
        }

        private static string Brand()
        {
            string brand = "PROJECT ECLIPSE  " + Application.version;
            try
            {
                if (Eclipse.Modding.ModRuntime.IsInitialized && Eclipse.Modding.ModRuntime.Host != null)
                {
                    int count = Eclipse.Modding.ModRuntime.Host.EnabledMods.Count;
                    brand += "   ·   " + count + (count == 1 ? " MOD" : " MODS");
                }
            }
            catch (Exception) { }
            return brand;
        }

        private void BuildHints()
        {
            if (footerHints == null) return;
            for (int i = footerHints.childCount - 1; i >= 0; i--) Destroy(footerHints.GetChild(i).gameObject);
            bool home = currentPage == "Home";
            var hints = padMode
                ? new[] { ("D-PAD", "Select", (Action)null), ("A", "Confirm", null), ("B", home ? "Quit" : "Back", (Action)FooterBack) }
                : new[] { ("ARROWS", "Select", (Action)null), ("ENTER", "Confirm", null), ("ESC", home ? "Quit" : "Back", (Action)FooterBack) };
            float x = 0f;
            for (int i = hints.Length - 1; i >= 0; i--) x = Hint(hints[i].Item1, hints[i].Item2, hints[i].Item3, x);
        }

        private void FooterBack()
        {
            if (leaving || rebuilding) return;
            EclipseUiAudio.Play(UiSound.Back);
            Back();
        }

        // Lays out one "[KEY] action" pair leftwards from the right edge; returns the next edge.
        private float Hint(string key, string action, Action click, float right)
        {
            var actionLabel = Label(footerHints, action, 0, 12, 10, 24, 15, new Color(Paper.r, Paper.g, Paper.b, .85f));
            actionLabel.horizontalOverflow = HorizontalWrapMode.Overflow;
            float actionWidth = actionLabel.preferredWidth;
            var keyLabel = Label(footerHints, key, 0, 12, 10, 24, 12, Paper, TextAnchor.MiddleCenter);
            keyLabel.horizontalOverflow = HorizontalWrapMode.Overflow;
            float capWidth = Mathf.Max(26f, keyLabel.preferredWidth + 14f);
            float actionX = right - actionWidth;
            float capX = actionX - 8f - capWidth;
            Anchor(actionLabel.rectTransform, actionX, 12, actionWidth, 24);
            var cap = Box(footerHints, "Key", 0, 0, 0, 0, new Color(Paper.r, Paper.g, Paper.b, .13f));
            Anchor(cap, capX, 11, capWidth, 25);
            cap.SetSiblingIndex(keyLabel.transform.GetSiblingIndex());
            var edge = Box(cap, "Edge", 0, 0, 0, 0, new Color(Paper.r, Paper.g, Paper.b, .35f));
            edge.anchorMin = Vector2.zero; edge.anchorMax = new Vector2(1, 0); edge.pivot = new Vector2(.5f, 0);
            edge.anchoredPosition = Vector2.zero; edge.sizeDelta = new Vector2(0, 2);
            Anchor(keyLabel.rectTransform, capX, 11, capWidth, 25);
            if (click != null)
            {
                var hit = Box(footerHints, key + " button", 0, 0, 0, 0, Color.clear);
                Anchor(hit, capX - 4, 6, right - capX + 8, 34);
                var button = hit.gameObject.AddComponent<Button>();
                button.targetGraphic = hit.GetComponent<Image>();
                button.transition = Selectable.Transition.None;
                button.navigation = new Navigation { mode = Navigation.Mode.None };
                button.onClick.AddListener(() => click());
            }
            return capX - 26f;
        }

        // Positions relative to the hints group's top-right corner (x grows leftwards negative).
        private static void Anchor(RectTransform rect, float x, float y, float w, float h)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(x, -y);
            rect.sizeDelta = new Vector2(w, h);
        }

        // Switches the hints between keyboard and controller wording, following the last input.
        private void UpdateInputDevice()
        {
            bool pad = padMode;
            var stick = GamePad.GetStick(GamePad.Stick.Dpad, GamePad.Player.Any) + GamePad.GetStick(GamePad.Stick.LeftStick, GamePad.Player.Any);
            if (GamePad.GetButtonDown(GamePad.Button.A, GamePad.Player.Any) || GamePad.GetButtonDown(GamePad.Button.B, GamePad.Player.Any) ||
                GamePad.GetButtonDown(GamePad.Button.Start, GamePad.Player.Any) || stick.sqrMagnitude > .25f) pad = true;
            else if (UnityEngine.Input.anyKeyDown && !JoystickKeyDown() || UnityEngine.Input.GetAxisRaw("Mouse X") != 0f) pad = false;
            if (pad != padMode) { padMode = pad; BuildHints(); }
        }

        private static bool JoystickKeyDown()
        {
            for (var key = KeyCode.JoystickButton0; key <= KeyCode.Joystick8Button19; key++)
                if (UnityEngine.Input.GetKeyDown(key)) return true;
            return false;
        }

        // D-pad or left stick moves focus (edge-triggered); A confirms; B goes back.
        private int PadNavigation(out bool confirm, out bool back, out int horizontal)
        {
            var stick = GamePad.GetStick(GamePad.Stick.Dpad, GamePad.Player.Any);
            var left = GamePad.GetStick(GamePad.Stick.LeftStick, GamePad.Player.Any);
            if (left.sqrMagnitude > stick.sqrMagnitude) stick = left;
            int vertical = 0; horizontal = 0;
            if (Mathf.Abs(stick.y) > .6f && Mathf.Abs(padStickLast.y) <= .6f) vertical = stick.y > 0 ? -1 : 1;
            if (Mathf.Abs(stick.x) > .6f && Mathf.Abs(padStickLast.x) <= .6f) horizontal = stick.x > 0 ? 1 : -1;
            padStickLast = stick;
            confirm = GamePad.GetButtonDown(GamePad.Button.A, GamePad.Player.Any);
            back = GamePad.GetButtonDown(GamePad.Button.B, GamePad.Player.Any);
            return vertical;
        }
    }
}
