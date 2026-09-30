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
        // Every scene is a real fight location drawn by the game's own location renderer (see
        // StageView), floors included, with two sparring fighters standing in it. The authored
        // autumn gate remains only as the fallback (and for the in-game options overlay).
        private enum SceneId
        {
            BambooGrove, Sakura, NightBridge, Fuji, Waterfall,
            SnowyPeak, FloodedVillage, LanternsOnWater, Moon, Heaven,
        }
        private static readonly int SceneCount = Enum.GetValues(typeof(SceneId)).Length;

        // A fight location shown whole behind the menu.
        private sealed class Stage
        {
            public string Location;     // location id (gamedata/locations/<id>)
            public Color32 Accent;      // secondary menu copy on this scene
            public TitleLeaf.Kind? Particles;
        }

        private static readonly Dictionary<SceneId, Stage> Stages = new Dictionary<SceneId, Stage>
        {
            { SceneId.BambooGrove, new Stage { Location = "bamboo_grove", Accent = new Color32(196, 226, 132, 255), Particles = TitleLeaf.Kind.Leaf } },
            { SceneId.Sakura, new Stage { Location = "sakura", Accent = new Color32(255, 196, 214, 255), Particles = TitleLeaf.Kind.Petal } },
            { SceneId.NightBridge, new Stage { Location = "night_bridge", Accent = new Color32(168, 196, 255, 255) } },
            { SceneId.Fuji, new Stage { Location = "fuji", Accent = new Color32(255, 196, 214, 255), Particles = TitleLeaf.Kind.Petal } },
            { SceneId.Waterfall, new Stage { Location = "waterfall", Accent = new Color32(150, 220, 206, 255), Particles = TitleLeaf.Kind.Leaf } },
            { SceneId.SnowyPeak, new Stage { Location = "snowy_peak", Accent = new Color32(200, 224, 245, 255) } },
            { SceneId.FloodedVillage, new Stage { Location = "flooded_village", Accent = new Color32(250, 171, 62, 255), Particles = TitleLeaf.Kind.Leaf } },
            { SceneId.LanternsOnWater, new Stage { Location = "lamps_on_water", Accent = new Color32(255, 186, 98, 255) } },
            { SceneId.Moon, new Stage { Location = "moon", Accent = new Color32(214, 206, 255, 255) } },
            { SceneId.Heaven, new Stage { Location = "heaven", Accent = new Color32(255, 226, 150, 255), Particles = TitleLeaf.Kind.Petal } },
        };

        // Title music per scene (Resources paths). The ambience is an Eclipse-owned track;
        // Fuji is the game's own fight18_fuji, for the Fuji stage.
        private static string SceneMusic => scene == SceneId.Fuji && !gateShown
            ? "gamedata/music/fight18_fuji" : "EclipseTitle/the_ambience";

        private const string ScenePreference = "Eclipse.TitleSceneIndex";
        private static SceneId scene;
        private static bool sceneChosen;
        // True while the autumn gate stands in for a stage that could not be drawn.
        private static bool gateShown;

        // Opaque per-scene accents keep secondary copy distinct from the main
        // paper-colored labels; amber leaves at the fallback gate.
        private static Color SceneryAccent => gateShown ? new Color32(250, 171, 62, 255) : Stages[scene].Accent;

        private sealed class Layer { public RectTransform Rect; public Vector2 Home; public float Depth; public bool Live; }
        private readonly List<Layer> layers = new List<Layer>();
        // sceneryHost survives page rebuilds (Clear skips it), so opening Options or going
        // back to Home never reloads the stage or the fighters. It holds the current
        // `scenery` and, above it, the veil a scene change fades through.
        private RectTransform sceneryHost, scenery;
        private Image sceneVeil;
        private Coroutine sceneChange;
        private Vector2 parallax, parallaxVelocity;
        private float groundY = 662f;
        private float viewLeft, viewWidth = 1280f;

        // The live fight location behind a stage scene, and its picture in `scenery`.
        private StageView stageView;
        private RawImage stageImage;
        private const float StageOverscan = 1.04f;


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

        // Picks the scene for this launch, once. Manual cycling (CycleScene, below) reuses
        // the same persisted counter via AdvanceScene, so it also decides what the next
        // launch opens on.
        private static void ChooseScene()
        {
            if (sceneChosen) return;
            sceneChosen = true;
            AdvanceScene();
        }

        // Reads the persisted scene counter, advances it (for the next launch, or the next
        // manual cycle this run) and selects the scene it now points at.
        private static void AdvanceScene()
        {
            int index = 0;
            try { index = PlayerPrefs.GetInt(ScenePreference, 0); PlayerPrefs.SetInt(ScenePreference, index + 1); PlayerPrefs.Save(); }
            catch (Exception error) { Debug.LogWarning("[Title] Scene rotation unavailable: " + error.Message); }
            scene = (SceneId)(Mathf.Abs(index) % SceneCount);
        }

        // Manually advances to the next scene in rotation. Only reachable from Home (see
        // DrawSceneCycleButton and the Update() shortcut); the menu itself is never rebuilt.
        private void CycleScene()
        {
            if (leaving || rebuilding || currentPage != "Home" || sceneChange != null || sceneryHost == null) return;
            EclipseUiAudio.Play(UiSound.Tab);
            sceneChange = StartCoroutine(ChangeScene());
        }

        private const float SceneFadeOut = .35f, SceneFadeIn = .6f;

        // Dips only the scenery (and its leaves) to ink, swaps the scene while none of it
        // shows, then lifts the veil. The menu, logo and footer stay live throughout. The
        // costly part (loading a location and the fighter models) happens under the veil,
        // and the fades clamp their time step so a slow frame cannot skip them.
        private System.Collections.IEnumerator ChangeScene()
        {
            string music = SceneMusic;
            yield return FadeScene(0f, 1f, SceneFadeOut);
            AdvanceScene();
            RebuildScenery();
            // The leaf layer survives page rebuilds, so reseed it for the new scene here.
            if (leafLayer != null)
            {
                for (int i = leafLayer.childCount - 1; i >= 0; i--) Destroy(leafLayer.GetChild(i).gameObject);
                ScatterParticles(leafLayer);
            }
            if (versusCaption != null) versusCaption.color = SceneryAccent;
            if (SceneMusic != music) EclipseUiAudio.StartTitleMusic(SceneMusic, 1.2f);
            // Let the location and the fighter models load and draw before anything shows.
            for (int i = 0; i < 4; i++) yield return null;
            // The new scene settles in and its eclipse gathers again, as on opening.
            sceneOpened = Time.unscaledTime;
            eclipse = eclipseVelocity = 0f;
            yield return FadeScene(1f, 0f, SceneFadeIn);
            sceneChange = null;
        }

        private System.Collections.IEnumerator FadeScene(float from, float to, float duration)
        {
            CanvasGroup leaves = null;
            if (leafLayer != null)
            {
                leaves = leafLayer.GetComponent<CanvasGroup>();
                if (leaves == null) leaves = leafLayer.gameObject.AddComponent<CanvasGroup>();
            }
            float t = 0f;
            while (true)
            {
                t = Mathf.Min(1f, t + Mathf.Min(Time.unscaledDeltaTime, .05f) / duration);
                float veil = Mathf.Lerp(from, to, t * t * (3f - 2f * t));
                if (sceneVeil != null) sceneVeil.color = new Color(Ink.r, Ink.g, Ink.b, veil);
                if (leaves != null) leaves.alpha = 1f - veil;
                if (t >= 1f) yield break;
                yield return null;
            }
        }

        // --- Scene construction ------------------------------------------------------------

        // Called from every Clear(): builds the scenery the first time only.
        private void EnsureScenery()
        {
            if (sceneryHost != null) return;
            sceneryHost = Rect(page, "Scenery host", 0, 0, 1280, 720);
            sceneVeil = Box(sceneryHost, "Scene veil", -400, 0, 2080, 720, new Color(Ink.r, Ink.g, Ink.b, 0f)).GetComponent<Image>();
            sceneVeil.raycastTarget = false;
            RebuildScenery();
        }

        private void RebuildScenery()
        {
            if (scenery != null)
            {
                scenery.gameObject.SetActive(false);
                Destroy(scenery.gameObject);
            }
            DrawScenery();
            scenery.SetAsFirstSibling(); // under the veil
        }

        private void DrawScenery()
        {
            ChooseScene();
            layers.Clear();
            stageImage = null;
            skyLeft = skyRight = null;
            sunRoot = null; moonDisc = null; sunGlow = corona = null; totality = null;
            scenery = Rect(sceneryHost, "Scenery", 0, 0, 1280, 720);
            scenery.pivot = new Vector2(.5f, .5f);
            scenery.anchoredPosition = new Vector2(640, -360);
            if (sceneOpened <= 0f) sceneOpened = Time.unscaledTime;
            // The in-game options overlay stays light: no live location over a running fight.
            bool stage = !optionsOnly && TryDrawStage();
            gateShown = !stage;
            if (!stage) { ReleaseStage(); DrawAutumnGate(); }
            else DrawSparringFighters();
            // At the gate the sun hides behind the roof and trees; the world dims over it all.
            if (totality != null) totality.transform.SetAsLastSibling();
        }

        // Draws the scene's fight location through the game's own renderer (layers, floor
        // and animation) into a texture. Kept across page rebuilds; replaced on a scene change.
        private bool TryDrawStage()
        {
            var info = Stages[scene];
            if (stageView == null || stageView.Location != info.Location)
            {
                ReleaseStage();
                stageView = StageView.Open(info.Location);
            }
            if (stageView == null)
            {
                Debug.LogWarning("[Title] Could not draw stage " + info.Location + "; showing the autumn gate.");
                return false;
            }
            stageImage = Rect(scenery, "Stage", 0, 0, 1280, 720).gameObject.AddComponent<RawImage>();
            stageImage.raycastTarget = false;
            // The location renders mirrored for the fight camera; show it upright.
            stageImage.uvRect = new Rect(0, 1, 1, -1);
            LayoutStage(Vector2.zero);
            return true;
        }

        private void ReleaseStage()
        {
            if (stageView != null) stageView.Dispose();
            stageView = null;
        }

        // The stage picture fills the viewport with overscan for the parallax; the floor line
        // follows from where the renderer puts the location's own Floor.
        private void LayoutStage(Vector2 offset)
        {
            if (stageImage == null || stageView == null) return;
            float width = viewWidth * StageOverscan, height = 720f * StageOverscan;
            var rect = stageImage.rectTransform;
            rect.anchoredPosition = new Vector2(viewLeft - (width - viewWidth) * .5f, (height - 720f) * .5f) + offset;
            rect.sizeDelta = new Vector2(width, height);
            float pixels = Mathf.Clamp(Screen.height * StageOverscan, 256f, 2304f);
            stageView.Tick(Mathf.RoundToInt(pixels * width / height), Mathf.RoundToInt(pixels));
            stageImage.texture = stageView.Texture;
            groundY = Mathf.Min(stageView.GroundFraction(width / height) * height - (height - 720f) * .5f, 700f);
        }

        // The game's location renderer drawing one location into a private texture, far
        // from anything a scene places (as the versus lobby backdrop does).
        private sealed class StageView
        {
            private const float FarX = 80000f;
            public string Location { get; private set; }
            public RenderTexture Texture { get; private set; }
            private global::Location location;
            private global::Render render;
            private GameObject root;
            private UnityEngine.Camera camera;

            public static StageView Open(string name)
            {
                var view = new StageView { Location = name };
                try
                {
                    view.location = new global::Location(name, string.Empty);
                    view.location.init();
                    if (view.location.layers == null || view.location.layers.Count == 0 || view.location.gameLayer == null)
                        throw new InvalidOperationException("no layers");
                    view.root = new GameObject("Title stage (" + name + ")");
                    view.root.transform.position = new Vector3(FarX, 0, 0);
                    view.render = new global::Render(view.root);
                    view.render.Init(view.location);
                    var host = new GameObject("Title stage camera");
                    host.transform.position = new Vector3(FarX, 0, 0);
                    view.camera = host.AddComponent<UnityEngine.Camera>();
                    view.camera.orthographic = true;
                    view.camera.orthographicSize = 5f;
                    view.camera.nearClipPlane = .1f;
                    view.camera.farClipPlane = 2000f;
                    view.camera.clearFlags = CameraClearFlags.SolidColor;
                    view.camera.backgroundColor = Ink;
                    view.camera.depth = -100;
                    view.camera.allowHDR = false;
                    view.camera.allowMSAA = false;
                    return view;
                }
                catch (Exception error)
                {
                    Debug.LogWarning("[Title] Stage " + name + " could not be drawn: " + error.Message);
                    view.Dispose();
                    return null;
                }
            }

            // Keeps the texture at the picture's pixel size and the location framed on the camera.
            public void Tick(int width, int height)
            {
                if (camera == null) return;
                width = Mathf.Max(64, width); height = Mathf.Max(64, height);
                if (Texture == null || Texture.width != width || Texture.height != height)
                {
                    ReleaseTexture();
                    Texture = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32) { name = "Title stage", useMipMap = false };
                    Texture.Create();
                    camera.targetTexture = Texture;
                    camera.aspect = (float)width / height;
                }
                try { UpdateBackdrop(false); }
                catch (Exception error) { Debug.LogWarning("[Title] Stage update: " + error.Message); }
            }

            public void Advance()
            {
                if (camera == null) return;
                try
                {
                    if (sparring != null && !sparring.AdvanceTitleSparring())
                        ShowFighters(leftLoadout, rightLoadout, leftStart, rightStart);
                    UpdateBackdrop(true);
                }
                catch (Exception error)
                {
                    RemoveFighters();
                    Debug.LogWarning("[Title] Stage animation: " + error);
                }
            }

            private Fight sparring;
            public bool HasSparring => sparring != null;
            private Eclipse.Multiplayer.VersusLoadout leftLoadout, rightLoadout;
            private float leftStart, rightStart;

            private void UpdateBackdrop(bool advanceAnimation)
            {
                float? center = null;
                // Fixed updates draw the current state. LateUpdate's Tick draws
                // the same interpolated pose as the fighter meshes and capsules.
                if (sparring != null) center = sparring.GetTitleSparringCenterX(advanceAnimation
                    ? 1f : Eclipse.Rendering.Interpolation.FightInterpolation.FightAlpha);
                render.UpdateMenuBackdrop(camera, advanceAnimation, center);
            }

            // Converts a horizontal page offset from the picture's centre (in page units, at
            // the given viewport aspect) to the fight's x coordinate (0 at the left wall).
            public float PageToFightX(float pageOffset, float aspect)
            {
                float height = location.FEIHFIPFNKF, width = location.JMLAKAKDBBL;
                float visible = Mathf.Min(height, width / Mathf.Max(.01f, aspect));
                return width * .5f + pageOffset * visible / (720f * StageOverscan);
            }

            // The title owns presentation; the encounter owns detached CPU fighters and
            // native combat only. Page changes retain it; stage changes and entry dispose it.
            public void ShowFighters(Eclipse.Multiplayer.VersusLoadout left, Eclipse.Multiplayer.VersusLoadout right, float leftX, float rightX)
            {
                RemoveFighters();
                leftLoadout = left; rightLoadout = right;
                leftStart = leftX; rightStart = rightX;
                var first = Eclipse.Multiplayer.LocalVersusMatch.PrepareTitleFighter(left, true, "Standard");
                var second = Eclipse.Multiplayer.LocalVersusMatch.PrepareTitleFighter(right, false, "Aggressive");
                sparring = Fight.CreateTitleSparring(location, render, first, second, leftX, rightX,
                    location.MFAPMDDJBBL, location.JMLAKAKDBBL - location.MFAPMDDJBBL);
            }

            public void RemoveFighters()
            {
                var previous = sparring;
                sparring = null;
                previous?.DisposeTitleSparring();
            }

            // Where fighters stand, as a fraction of the picture's height from its top. The
            // renderer centres the location on the camera and covers it (Render.UpdateMenuBackdrop);
            // the fight floor sits Floor units above the location's bottom edge.
            public float GroundFraction(float aspect)
            {
                float height = location.FEIHFIPFNKF, width = location.JMLAKAKDBBL;
                float visible = Mathf.Min(height, width / Mathf.Max(.01f, aspect));
                float ground = height * .5f - location.GBNPHCHGKDO;
                return .5f + ground / visible;
            }

            private void ReleaseTexture()
            {
                if (Texture == null) return;
                if (camera != null) camera.targetTexture = null;
                Texture.Release();
                UnityEngine.Object.Destroy(Texture);
                Texture = null;
            }

            public void Dispose()
            {
                RemoveFighters();
                try { render?.DestroyMenuBackdrop(); }
                catch (Exception error) { Debug.LogWarning("[Title] Stage cleanup: " + error.Message); }
                render = null;
                if (root != null) UnityEngine.Object.Destroy(root);
                root = null;
                ReleaseTexture();
                if (camera != null) UnityEngine.Object.Destroy(camera.gameObject);
                camera = null;
            }
        }

        // Two CPU fighters in randomized loadouts sparring in the stage itself: drawn by
        // the stage camera at fight scale, on the
        // floor and among the location's layers as in a fight (StageView.ShowFighters).
        // Re-rolled on every scene change; page changes keep them (the stage persists).
        // Where they stand, as a fraction of the visible viewport's width.
        private const float SparringStand = .24f;

        private void DrawSparringFighters()
        {
            // Fighters need the game's items, animations and models, which the game loader
            // only reads after the title closes (so title mod choices apply to them).
            if (stageView == null || !Eclipse.Multiplayer.VersusRoster.GameDataLoaded) return;
            try
            {
                var random = new System.Random();
                var left = Eclipse.Multiplayer.VersusLoadout.Random(random);
                var right = Eclipse.Multiplayer.VersusLoadout.Random(random);
                float aspect = viewWidth / 720f;
                stageView.ShowFighters(left, right,
                    stageView.PageToFightX(viewLeft + viewWidth * SparringStand - 640f, aspect),
                    stageView.PageToFightX(viewLeft + viewWidth * (1f - SparringStand) - 640f, aspect));
            }
            catch (Exception error)
            {
                // The roster needs the game data; without it the stage simply stands empty.
                Debug.LogWarning("[Title] Sparring fighters unavailable: " + error.Message);
                stageView.RemoveFighters();
            }
        }

        // The game loader reads items, animations and models only after the title closes.
        // For the sparring fighters, the title runs the part of that boot they need early
        // (with the enabled mods, so Apply & Restart re-runs it with a new selection) against a
        // sandbox user directory. Entry keeps the content and replaces the sandbox profile;
        // a mod restart or failed preview still discards the entire content session.
        // It never touches a real save: CampaignSaveSession.PreviewDirectory redirects every
        // user-data path, and the save-version step (AttachFileModule) is left out.
        private static class TitleGameData
        {
            public static bool Active { get; private set; }
            public static bool PendingEntry { get; private set; }
            private static string sandbox;

            public static void Load()
            {
                if (Active || Eclipse.Multiplayer.VersusRoster.GameDataLoaded) return;
                PendingEntry = false;
                var watch = System.Diagnostics.Stopwatch.StartNew();
                try
                {
                    Active = true;
                    var timings = new System.Text.StringBuilder();
                    Action<string, long> record = (name, ms) => timings.Append("\n  ").Append(name).Append(": ").Append(ms).Append(" ms");
                    var modules = new LoadingModule[] { new PreInitializationModule(), new AntichitingModule(), new InitializationModule() };
                    foreach (var module in modules)
                    {
                        // SF2Paths is set up by PreInitializationModule (once per process).
                        if (module is AntichitingModule) CreateSandbox();
                        var moduleWatch = System.Diagnostics.Stopwatch.StartNew();
                        module.Start();
                        for (int step = 0; !module.GCHANFIHDGH(); step++)
                        {
                            if (step > 16) throw new InvalidOperationException(module.GetType().Name + " did not finish.");
                            module.JLPMOKPFECK();
                        }
                        record(module.GetType().Name, moduleWatch.ElapsedMilliseconds);
                    }
                    SeedSandboxProfile();
                    // ParseModule's steps, in its order, each timed (ListSF reports its own).
                    ListSF.StepTimer = (name, ms) => record("  ListSF " + name, ms);
                    try { Parse(record); }
                    finally { ListSF.StepTimer = null; }
                    Debug.Log("[Title] Game data preview loaded in " + watch.ElapsedMilliseconds + " ms:" + timings);
                }
                catch (Exception error)
                {
                    Debug.LogWarning("[Title] Game data preview unavailable; no sparring fighters. " + error);
                    Discard();
                }
            }

            // ParseModule.JLPMOKPFECK, step by step, minus what fighters never use: warriors,
            // zones, quests and the mod locale. Mod content stays: mods often change how
            // fighters look.
            private static void Parse(Action<string, long> record)
            {
                Action<string, Action> step = (name, action) =>
                {
                    var timer = System.Diagnostics.Stopwatch.StartNew();
                    action();
                    record(name, timer.ElapsedMilliseconds);
                };
                step("variables", GameUtils.InitVariables);
                step("settings", GameSettings.OCIPKAONMOP);
                step("animations", GameLoader.BJLLJHDFMOO);
                step("AI", GameLoader.POLKDKOOACO);
                step("ListSF", () => ListSF.GetInstance().LoadTitlePreview());
                step("perk tree", () => PerkTree.GBPBIPFIOJH().LJHPGKAOIAE());
                step("settings finish", GameSettings.LNNLDPLDABI);
                step("sound", GameLoader.SetSound);
                step("localization", LocalizationManager.Init);
                step("finish", GameUtils.OEKOKKCILAG);
            }

            // The game reads user data from disk only under its data root (XmlUtils.OpenXMLDocument
            // treats any other path as a bundled resource), so the sandbox sits beside userdata.
            private static void CreateSandbox()
            {
                string root = SF2Paths.FFKEDOBDLOL;
                if (string.IsNullOrEmpty(root)) throw new InvalidOperationException("The game data root is not set up.");
                sandbox = (root.TrimEnd('/', '\\') + "/EclipseTitlePreview").Replace('\\', '/');
                if (Directory.Exists(sandbox)) Directory.Delete(sandbox, true);
                Directory.CreateDirectory(sandbox);
                Eclipse.Saves.CampaignSaveSession.PreviewDirectory = sandbox;
            }

            // A new campaign's first boot gets its profile from the game's usersDefault.xml
            // (GameSettings.AMOMFPOENBF, run by AttachFileModule). Do the same copy into the
            // sandbox directly, leaving that module's version bookkeeping untouched.
            private static void SeedSandboxProfile()
            {
                var profile = XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI(), "usersDefault.xml", XmlUtils.EBLFEPIOMOL.Normal, true,
                    XmlCryptoUtils.NNLGALNDJCL());
                if (profile == null) throw new InvalidOperationException("usersDefault.xml is missing.");
                XmlUtils.ONLDJNLKKAL(profile, Path.Combine(sandbox, Constants.OJMIJINKBPJ).Replace('\\', '/'));
            }

            public static void PrepareForEntry()
            {
                if (!Active) return;
                if (!ListSF.CanResumeTitlePreview) { Discard(); return; }
                ListSF.GetInstance().DetachTitleProfile();
                Active = false;
                PendingEntry = true;
                ReleaseSandbox();
            }

            public static bool TryResume()
            {
                if (!PendingEntry) return false;
                if (!ListSF.CanResumeTitlePreview) { Discard(); return false; }
                try
                {
                    ListSF.GetInstance().ResumeTitlePreview();
                    PendingEntry = false;
                    Debug.Log("[Title] Reused loaded game content; selected profile and campaign data loaded.");
                    return true;
                }
                catch
                {
                    Discard();
                    throw;
                }
            }

            public static void Discard()
            {
                if (!Active && !PendingEntry) return;
                Active = false;
                PendingEntry = false;
                try { Nekki.SF2.GUI.Scenes.GameLoaderScene.DiscardTitlePreview(); }
                catch (Exception error) { Debug.LogWarning("[Title] Game data preview cleanup: " + error.Message); }
                ReleaseSandbox();
            }

            private static void ReleaseSandbox()
            {
                Eclipse.Multiplayer.VersusRoster.Reset();
                Eclipse.Saves.CampaignSaveSession.PreviewDirectory = null;
                try { if (sandbox != null && Directory.Exists(sandbox)) Directory.Delete(sandbox, true); }
                catch (Exception error) { Debug.LogWarning("[Title] Game data preview sandbox: " + error.Message); }
                sandbox = null;
            }
        }

        // Drops the fighters (their models use the preview's animation data) and the preview.
        // Called before anything that starts real game content or saves the loaded profile.
        private void DiscardGameDataPreview()
        {
            if (stageView != null) stageView.RemoveFighters();
            TitleGameData.Discard();
        }

        private void PrepareGameDataForEntry()
        {
            if (stageView != null) stageView.RemoveFighters();
            TitleGameData.PrepareForEntry();
        }

        internal static bool TryResumeGameDataPreview() => TitleGameData.TryResume();

        // Whatever suits each stage (none where nothing would drift, such as the moon), and
        // autumn leaves at the fallback gate.
        private void ScatterParticles(RectTransform parent)
        {
            if (gateShown)
            {
                TitleLeaf.Scatter(parent, 24, TitleLeaf.Kind.Leaf, -400f, 1600f, -40f, 700f, groundY);
                return;
            }
            var stage = Stages[scene];
            if (stage.Particles.HasValue)
                TitleLeaf.Scatter(parent, stage.Particles == TitleLeaf.Kind.Petal ? 26 : 20, stage.Particles.Value,
                    viewLeft - 300f, viewLeft + viewWidth + 100f, -40f, 700f, groundY);
        }

        private void AddLayer(RectTransform rect, float depth, bool live = false)
        {
            if (rect != null) layers.Add(new Layer { Rect = rect, Home = rect.anchoredPosition, Depth = depth, Live = live });
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
            float breeze = .3f * Mathf.Clamp01(1f - (now - selectionBreezeAt) / .65f);
            TitleLeaf.Gust = Mathf.Max(gust * gust * (3f - 2f * gust), breeze);
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
            // The whole stage moves with its floor, so with the fighters standing on it.
            LayoutStage(offset * GroundPlaneDepth);
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
            plaque = Rect(page, "Eclipse plaque", 640, 210, 0, 0);
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
            var wash = Rect(page, "Menu wash", 300, 232, 680, 384).gameObject.AddComponent<RawImage>();
            wash.texture = SoftDot();
            wash.color = new Color(Ink.r, Ink.g, Ink.b, .62f);
            wash.raycastTarget = false;
        }

        // A small ink chip in the top-right corner (clear of the sign, the menu column and the
        // footer) that manually advances the title's scene rotation. Reuses CycleScene's
        // persisted counter, so this also changes what the next launch opens on.
        private void DrawSceneCycleButton()
        {
            var chip = Box(page, "Stage cycle", 1120, 24, 136, 32, new Color(Ink.r, Ink.g, Ink.b, .4f));
            var edge = Box(chip, "Edge", 0, 0, 0, 0, new Color(Paper.r, Paper.g, Paper.b, .32f));
            edge.anchorMin = Vector2.zero; edge.anchorMax = new Vector2(1, 0); edge.pivot = new Vector2(.5f, 0);
            edge.anchoredPosition = Vector2.zero; edge.sizeDelta = new Vector2(0, 2);
            var label = Label(chip, "STAGE  >", 0, 0, 136, 32, 13, Paper, TextAnchor.MiddleCenter);
            label.raycastTarget = false;
            var button = chip.gameObject.AddComponent<Button>();
            button.targetGraphic = chip.GetComponent<Image>();
            button.transition = Selectable.Transition.None;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            button.onClick.AddListener(CycleScene);
        }

        // --- Footer --------------------------------------------------------------------------

        private void DrawFooter()
        {
            footerBackground = Box(page, "Footer", 0, 674, 1280, 46, new Color(.09f, .066f, .052f, .78f));
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
