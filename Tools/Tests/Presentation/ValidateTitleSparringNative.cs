using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Xml;
using Eclipse.UI;
using Eclipse.Multiplayer;
using Eclipse.Modding;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Fixture-only editor harness. Runs the production title StageView and combat
// methods against native Unity rigs/TAR art, without a campaign or input controller.
[InitializeOnLoad]
public static class ValidateTitleSparringNative
{
    const string Active = "Eclipse.TitleSparringNative.Active";
    const BindingFlags Hidden = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
    static double started;
    static object stage;
    static Type stageType;
    static int ticks, leftAttacks, rightAttacks, checks;
    static bool damaged, forcedKnockout, rematched;
    static Fight firstFight;
    static Model firstLeft, firstRight;
    static object scripts, items;
    static string profile;
    static float leftWall, rightWall;
    static Exception failure;
    static int effectSounds;
    static Action<string, float> oldEffectSound;
    static int sceneIndex;
    static readonly string[] Scenes = { "bamboo_grove", "sakura", "night_bridge", "fuji", "waterfall", "snowy_peak", "flooded_village", "lamps_on_water", "moon", "heaven" };

    static ValidateTitleSparringNative()
    {
        if (SessionState.GetBool(Active, false))
        {
            started = EditorApplication.timeSinceStartup;
            EditorApplication.update += Update;
            Application.logMessageReceived += Capture;
        }
    }

    public static void RunEditor()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        if (!File.Exists(Path.Combine(root, "title-sparring-fixture.marker")))
            throw new InvalidOperationException("This validator requires its isolated fixture.");
        Environment.SetEnvironmentVariable("ECLIPSE_MODS_ROOT", Path.Combine(root, "Mods"));
        PlayerSettings.companyName = "EclipseAcceptance";
        PlayerSettings.productName = Path.GetFileName(root);
        SessionState.SetBool(Active, true);
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorApplication.EnterPlaymode();
    }

    static object Call(object instance, string method, params object[] args) => instance.GetType().GetMethod(method, Hidden).Invoke(instance, args);
    static void Check(bool ok, string message) { checks++; if (!ok) throw new Exception(message); }
    static void Capture(string message, string stack, LogType type)
    {
        if (type == LogType.Exception && !stack.Contains("UnityEditor.Search")) failure = new Exception(message + "\n" + stack);
    }

    static void Update()
    {
        if (!EditorApplication.isPlaying) return;
        EditorApplication.QueuePlayerLoopUpdate();
        try
        {
            if (failure != null) throw failure;
            if (EditorApplication.timeSinceStartup - started > 180) throw new Exception("Native sparring timed out.");
            if (stage == null)
            {
                typeof(TitleScreen).GetNestedType("TitleGameData", Hidden).GetMethod("Load", Hidden).Invoke(null, null);
                Check(VersusRoster.GameDataLoaded, "Title data failed to load.");
                scripts = ModRuntime.Scripts; items = ListSF.GetItems();
                if (sceneIndex == 0)
                {
                    Check(ModRuntime.Scripts.Content.Effects.Any(effect => effect.Sounds.Count > 0), "Fixture needs active screen effects with sounds.");
                    oldEffectSound = ModVisuals.PlayEffectSound;
                    ModVisuals.PlayEffectSound = (sound, volume) => effectSounds++;
                    ModVisuals.ResetTriggers();
                }
                profile = ListSF.CCDKHLAMKKO().get_Parameters().Node.OuterXml;
                leftWall = GameUtils.CKOPPGCIHPL(); rightWall = GameUtils.FBOGLADLJML();
                stageType = typeof(TitleScreen).GetNestedType("StageView", Hidden);
                stage = stageType.GetMethod("Open", Hidden).Invoke(null, new object[] { Scenes[sceneIndex] });
                Check(stage != null, "Stage failed to open.");
                Call(stage, "Tick", 1280, 720);
                var random = new System.Random(47);
                float leftX = (float)Call(stage, "PageToFightX", -332.8f, 1280f / 720f);
                float rightX = (float)Call(stage, "PageToFightX", 332.8f, 1280f / 720f);
                Call(stage, "ShowFighters", VersusLoadout.Random(random), VersusLoadout.Random(random), leftX, rightX);
                firstFight = Fight.GetCurrentFight();
                Check(firstFight != null && (bool)typeof(Fight).GetProperty("IsTitleSparring", Hidden).GetValue(firstFight), "Title fight missing.");
                Check(firstFight.Controller == null && firstFight.preFight == null, "Title created HUD/input controller.");
                firstLeft = firstFight.GetPlayerModel(); firstRight = firstFight.GetEnemyModel();
                if (Environment.GetCommandLineArgs().Contains("-titleShowcaseOnly"))
                {
                    CheckShowcase();
                    Debug.Log("[TitleShowcaseNative] PASS: " + checks + " checks.");
                    Finish(0);
                    return;
                }
                Check(firstLeft.Parameters.AiControlled && firstRight.Parameters.AiControlled && !firstLeft.Parameters.UserControlled && !firstRight.Parameters.UserControlled, "Both fighters must be CPUs.");
                Eclipse.Rendering.FighterParticles.Hit(null, new Vector3f(), true, false, true);
                var particles = firstLeft.MJNPBMOAFML().GetComponent<Eclipse.Rendering.FighterParticles>();
                foreach (var trigger in new[] { ModFxTrigger.Land, ModFxTrigger.Knockdown, ModFxTrigger.Slide, ModFxTrigger.Wall })
                    Call(particles, "Fire", trigger, Vector3.zero);
                firstLeft.AddEventListener(0, LeftInterval); firstRight.AddEventListener(0, RightInterval);
                if (sceneIndex == 0) CheckMagicEffects();
                Debug.Log("[TitleSparringNative] Native CPU bodies ready: " + firstLeft.Parameters.Weapon.Name + ", " + firstRight.Parameters.Weapon.Name);
            }
            // Several native simulation steps per editor update, still using the production
            // title Advance path (including its knockout delay and automatic rematch).
            for (int i = 0; i < 8; i++)
            {
                Call(stage, "Advance"); ticks++;
                if (ticks == 120) CaptureStage();
                if (Fight.GetCurrentFight() == firstFight)
                {
                    if (ticks >= 1500 && !forcedKnockout)
                    {
                        Check(leftAttacks > 0 && rightAttacks > 0, "CPUs failed to trade native attacks.");
                        firstFight.UpdateLife(firstRight, -firstRight.Parameters.MaxLife * 2f);
                        firstFight.SetLife(firstLeft, 0f);
                        Check(firstLeft.Parameters.HABJPOFCIHA() == 1f && firstRight.Parameters.HABJPOFCIHA() == 1f,
                            "Title fighters lost health under lethal damage/direct life assignment.");
                        Check(!firstLeft.Parameters.PCALDKCJGCK && !firstRight.Parameters.PCALDKCJGCK, "Immortal title fighter entered knockout state.");
                        forcedKnockout = true;
                    }
                    if (ticks >= 2600) { FinishChecks(); return; }
                }
                else
                {
                    throw new Exception("Infinite-health sparring unexpectedly restarted.");
                }
            }
        }
        catch (Exception error)
        {
            Debug.LogError("[TitleSparringNative] FAIL: " + error);
            Finish(1);
        }
    }

    static void LeftInterval(object data) { if (((Model.EventModel)data).Data is IntervalAttack) leftAttacks++; }
    static void RightInterval(object data) { if (((Model.EventModel)data).Data is IntervalAttack) rightAttacks++; }

    static void FinishChecks()
    {
        Check(leftAttacks > 0 && rightAttacks > 0 && Fight.GetCurrentFight() == firstFight,
            "Both CPUs must keep sparring beyond the former round timeout.");
        Check(effectSounds == 0, "Title sparring played a mod screen effect sound.");
        Check(((float[])typeof(ModVisuals).GetField("_triggerTimes", Hidden).GetValue(null)).All(time => time < 0), "Title hit or movement triggered a global screen effect.");
        Check(ModVisuals.CurrentTimeScale() == 1f && ModVisuals.CurrentMuffle() == 0f, "Title sparring slowed/muffled the menu.");
        Check(ReferenceEquals(items, ListSF.GetItems()) && ReferenceEquals(scripts, ModRuntime.Scripts), "Sparring reloaded content.");
        Check(profile == ListSF.CCDKHLAMKKO().get_Parameters().Node.OuterXml, "Sparring changed preview progress.");
        Check(typeof(ModRuntime).GetField("_profileRoster", Hidden).GetValue(null) == null, "Sparring bound a mod save profile.");
        Call(stage, "RemoveFighters");
        Check(Fight.GetCurrentFight() == null, "Title fight survived disposal.");
        Check(GameUtils.CKOPPGCIHPL() == leftWall && GameUtils.FBOGLADLJML() == rightWall, "Title combat bounds leaked.");
        Call(stage, "Dispose"); stage = null;
        Debug.Log("[TitleSparringNative] Scene passed: " + Scenes[sceneIndex] + "; ticks=" + ticks + ", attacks=" + leftAttacks + "/" + rightAttacks);
        if (++sceneIndex < Scenes.Length)
        {
            ticks = leftAttacks = rightAttacks = 0;
            damaged = forcedKnockout = rematched = false;
            return;
        }
        // Positive control: gameplay hit/KO screen triggers and sounds still work.
        Eclipse.Rendering.FighterParticles.Hit(null, new Vector3f(), true, false, true);
        Check(effectSounds > 0, "Gameplay hit/KO effect sounds were disabled.");
        Check(((float[])typeof(ModVisuals).GetField("_triggerTimes", Hidden).GetValue(null))[(int)ModFxTrigger.Ko] >= 0, "Gameplay KO trigger was disabled.");
        ModVisuals.ResetTriggers();
        Debug.Log("[TitleSparringNative] PASS: " + checks + " native CPU attacks, infinite health, save isolation, content reuse and cleanup checks across all ten title locations.");
        Finish(0);
    }

    static void CaptureStage()
    {
        CheckCameraInterpolation();
        var camera = (UnityEngine.Camera)stageType.GetField("camera", Hidden).GetValue(stage);
        camera.Render();
        var texture = (RenderTexture)stageType.GetProperty("Texture", Hidden).GetValue(stage);
        var old = RenderTexture.active;
        var location = (Location)stageType.GetField("location", Hidden).GetValue(stage);
        var render = (global::Render)stageType.GetField("render", Hidden).GetValue(stage);
        var container = render.PFELMKLNBMC.MJNPBMOAFML().transform;
        Check(container.parent.localScale == Vector3.one, "Gameplay camera scaled the title game layer.");
        Check(GameUtils.CKOPPGCIHPL() == location.MFAPMDDJBBL && GameUtils.FBOGLADLJML() == location.JMLAKAKDBBL - location.MFAPMDDJBBL, "Title uses artificial walls instead of the native stage walls.");
        foreach (var model in new[] { firstLeft, firstRight })
        {
            var point = model.PLBNCDCFPML();
            var projected = camera.WorldToViewportPoint(container.TransformPoint(new Vector3(point.GetX(), point.GetY(), 0)));
            Check(projected.x >= .05f && projected.x <= .95f, "CPU is outside title view: " + projected);
            Check(model.MJNPBMOAFML().GetComponentsInChildren<MeshRenderer>().Length > 0, "CPU has no native rendered mesh.");
        }
        WritePicture(Scenes[sceneIndex]);
        foreach (var side in new[] { "left", "right" })
        {
            float center = side == "left" ? location.MFAPMDDJBBL : location.JMLAKAKDBBL - location.MFAPMDDJBBL;
            render.UpdateMenuBackdrop(camera, false, center);
            bool behind = true;
            float height = camera.orthographicSize * 2f;
            float scale = Mathf.Max(height / location.FEIHFIPFNKF, height * camera.aspect / location.JMLAKAKDBBL);
            float halfVisible = height * camera.aspect / scale * .5f;
            float offset = location.JMLAKAKDBBL * .5f - Mathf.Clamp(center, halfVisible, location.JMLAKAKDBBL - halfVisible);
            foreach (var layer in location.layers)
            {
                if (layer == location.gameLayer) behind = false;
                float factor = behind ? ModVisuals.BackgroundLayerFactor(layer.Factor) : layer.Factor;
                Check(Mathf.Abs(layer.MJNPBMOAFML().transform.localPosition.x - offset * factor) < .01f, "Title ignored the stage layer's parallax factor.");
            }
            WritePicture(Scenes[sceneIndex] + "-" + side);
        }
        Call(stage, "Tick", 1280, 720);
    }

    static void WritePicture(string name)
    {
        var camera = (UnityEngine.Camera)stageType.GetField("camera", Hidden).GetValue(stage);
        camera.Render();
        var texture = (RenderTexture)stageType.GetProperty("Texture", Hidden).GetValue(stage);
        var old = RenderTexture.active;
        var picture = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
        try
        {
            RenderTexture.active = texture;
            picture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            // The menu RawImage flips the native location texture vertically.
            var pixels = picture.GetPixels();
            for (int y = 0; y < texture.height / 2; y++)
                for (int x = 0; x < texture.width; x++)
                {
                    int a = y * texture.width + x, b = (texture.height - 1 - y) * texture.width + x;
                    var swap = pixels[a]; pixels[a] = pixels[b]; pixels[b] = swap;
                }
            picture.SetPixels(pixels); picture.Apply();
            File.WriteAllBytes(Path.Combine(Directory.GetParent(Application.dataPath).FullName, name + ".png"), picture.EncodeToPNG());
        }
        finally { RenderTexture.active = old; UnityEngine.Object.Destroy(picture); }
    }

    static void CheckCameraInterpolation()
    {
        var interpolation = typeof(Eclipse.Rendering.Interpolation.FightInterpolation);
        var frameField = interpolation.GetField("_cachedFrame", Hidden);
        var alphaField = interpolation.GetField("_cachedAlpha", Hidden);
        var drawField = interpolation.GetField("_drawStep", Hidden);
        var fightField = interpolation.GetField("_fightStep", Hidden);
        object oldFrame = frameField.GetValue(null), oldAlpha = alphaField.GetValue(null);
        object oldDraw = drawField.GetValue(null), oldFight = fightField.GetValue(null);
        bool oldEnabled = SF2DisplayFrameRate.InterpolationEnabled;
        var render = (global::Render)stageType.GetField("render", Hidden).GetValue(stage);
        var camera = (UnityEngine.Camera)stageType.GetField("camera", Hidden).GetValue(stage);
        var location = (Location)stageType.GetField("location", Hidden).GetValue(stage);
        try
        {
            SF2DisplayFrameRate.SetInterpolationEnabled(true);
            drawField.SetValue(null, double.NaN); fightField.SetValue(null, double.NaN);
            foreach (float alpha in new[] { 0f, .5f, 1f })
            {
                frameField.SetValue(null, Time.frameCount); alphaField.SetValue(null, alpha);
                float left = Mathf.Lerp(firstLeft.CLDMEJKGLBA().HOFFDCFEBGA().GetEnd().GetX(), firstLeft.PLBNCDCFPML().GetX(), alpha);
                float right = Mathf.Lerp(firstRight.CLDMEJKGLBA().HOFFDCFEBGA().GetEnd().GetX(), firstRight.PLBNCDCFPML().GetX(), alpha);
                float center = (left + right) * .5f;
                Check(Mathf.Abs((float)Call(firstFight, "GetTitleSparringCenterX", alpha) - center) < .001f, "Title camera center does not interpolate fighter pivots.");
                Call(stage, "Tick", 1280, 720);
                float height = camera.orthographicSize * 2f;
                float scale = Mathf.Max(height / location.FEIHFIPFNKF, height * camera.aspect / location.JMLAKAKDBBL);
                float halfVisible = height * camera.aspect / scale * .5f;
                float expected = (location.JMLAKAKDBBL * .5f - Mathf.Clamp(center, halfVisible, location.JMLAKAKDBBL - halfVisible)) * location.gameLayer.Factor;
                Check(Mathf.Abs(location.gameLayer.MJNPBMOAFML().transform.localPosition.x - expected) < .01f, "Rendered title camera ignores interpolation alpha.");
                Check(render.MJNPBMOAFML().transform.position.x == camera.transform.position.x, "Title panned the entire stage instead of its layers.");
            }
            SF2DisplayFrameRate.SetInterpolationEnabled(false);
            frameField.SetValue(null, Time.frameCount); alphaField.SetValue(null, 0f);
            Call(stage, "Tick", 1280, 720);
            Check(Eclipse.Rendering.Interpolation.FightInterpolation.FightAlpha == 1f, "Disabled interpolation must use current state.");
        }
        finally
        {
            SF2DisplayFrameRate.SetInterpolationEnabled(oldEnabled);
            frameField.SetValue(null, oldFrame); alphaField.SetValue(null, oldAlpha);
            drawField.SetValue(null, oldDraw); fightField.SetValue(null, oldFight);
            Call(stage, "Tick", 1280, 720);
        }
    }

    static CurrentEffect[] MagicEffects()
    {
        var render = (global::Render)stageType.GetField("render", Hidden).GetValue(stage);
        var background = render.GOCPBKNDKMC().NFCBNLKLPBK();
        var front = render.GDBMKMFFOCF().NFCBNLKLPBK();
        var field = typeof(EffectsRunning).GetField("IGLOMLIOOBM", Hidden);
        return ((System.Collections.Generic.List<CurrentEffect>)field.GetValue(background))
            .Concat((System.Collections.Generic.List<CurrentEffect>)field.GetValue(front)).ToArray();
    }

    static void CheckMagicEffects()
    {
        bool oldAi = ModelAi.get_AiOn();
        ModelAi.set_AiOn(false);
        try
        {
            foreach (var spec in new[] { ("TitleMagicFront", false, false), ("TitleMagicBack", true, false), ("TitleMagicLoop", false, true) })
            {
                var document = new XmlDocument();
                document.LoadXml("<Effect Name='" + spec.Item1 + "' Sequence='mgc_magic_mass_bomb_start' TimeScale='1' OnBackground='" + (spec.Item2 ? "1" : "0") + "' Looped='" + (spec.Item3 ? "1" : "0") + "'/>");
                new ActionEffect(document.DocumentElement).Visit(firstLeft);
            }
            var effects = MagicEffects().Where(effect => effect.LLOLBKJMKNC.get_Name().StartsWith("TitleMagic")).ToArray();
            Check(effects.Length == 3, "Native effects did not reach both title effect containers.");
            Check(effects.All(effect => effect.BHHCMELOEJF.get_TotalFrames() > 2 && effect.BHHCMELOEJF.GetComponentInChildren<SpriteRenderer>().sprite != null), "Native magic art is missing.");
            var frames = effects.Select(effect => effect.BHHCMELOEJF.GetComponentInChildren<SpriteRenderer>().sprite).ToArray();
            for (int i = 0; i < 3; i++) Call(stage, "Advance");
            for (int i = 0; i < effects.Length; i++)
                Check(effects[i].BHHCMELOEJF.GetComponentInChildren<SpriteRenderer>().sprite != frames[i], "Title magic stayed on its first frame.");
            int duration = effects.Max(effect => effect.BHHCMELOEJF.get_TotalFrames()) + 4;
            for (int i = 0; i < duration; i++) Call(stage, "Advance");
            Check(!MagicEffects().Any(effect => effect.LLOLBKJMKNC.get_Name() == "TitleMagicFront" || effect.LLOLBKJMKNC.get_Name() == "TitleMagicBack"), "Finished title magic was not removed.");
            Check(MagicEffects().Any(effect => effect.LLOLBKJMKNC.get_Name() == "TitleMagicLoop"), "Looping title magic ended early.");
            Call(stage, "ShowFighters", leftLoadoutForTest(), rightLoadoutForTest(), firstLeft.Parameters.JJCKADKCDIF.GetX(), firstRight.Parameters.JJCKADKCDIF.GetX());
            Check(MagicEffects().Length == 0, "Old magic effects survived a title rematch.");
            firstFight = Fight.GetCurrentFight(); firstLeft = firstFight.GetPlayerModel(); firstRight = firstFight.GetEnemyModel();
            firstLeft.AddEventListener(0, LeftInterval); firstRight.AddEventListener(0, RightInterval);
        }
        finally { ModelAi.set_AiOn(oldAi); }
    }

    static void CheckShowcase()
    {
        // Build the production Home UI against the already-running native encounter.
        // The inactive component bypasses startup/splash; its UI has a live, separate canvas.
        var owner = new GameObject("Showcase test owner", typeof(RectTransform));
        owner.SetActive(false);
        var screen = owner.AddComponent<TitleScreen>();
        ((RectTransform)owner.transform).sizeDelta = new Vector2(1280, 720);
        var canvas = new GameObject("Showcase test canvas", typeof(RectTransform), typeof(Canvas));
        canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var viewport = (RectTransform)Call(screen, "Rect", canvas.transform, "Viewport", 0f, 0f, 1280f, 720f);
        var page = (RectTransform)Call(screen, "Rect", viewport, "Page", 0f, 0f, 1280f, 720f);
        var scenery = (RectTransform)Call(screen, "Rect", page, "Scenery host", 0f, 0f, 1280f, 720f);
        var leaves = (RectTransform)Call(screen, "Rect", page, "Seasonal particles", 0f, 0f, 1280f, 720f);
        var image = new GameObject("Stage picture", typeof(RectTransform), typeof(UnityEngine.UI.RawImage)).GetComponent<UnityEngine.UI.RawImage>();
        image.transform.SetParent(scenery, false);
        void Set(string name, object value) => typeof(TitleScreen).GetField(name, Hidden).SetValue(screen, value);
        object Get(string name) => typeof(TitleScreen).GetField(name, Hidden).GetValue(screen);
        bool Advance(float now, bool activity = false, bool focused = true) => (bool)Call(screen, "AdvanceShowcase", now, .05f, activity, focused);
        AudioSource[] Voices() => UnityEngine.Object.FindObjectsByType<EclipseUiAudio>(FindObjectsSortMode.None)
            .SelectMany(audio => audio.GetComponents<AudioSource>()).Where(source => source.clip != null && source.clip.name.StartsWith("snd_swish")).ToArray();
        void Play(string name, bool loop = false)
        {
            var doc = new XmlDocument();
            doc.LoadXml("<Sound Name='" + name + "' Looped='" + (loop ? "1" : "0") + "' Volume='.5'/>");
            new ActionSound(doc.DocumentElement).Visit(firstLeft);
        }
        float oldSound = SoundController.GetSoundVolume();
        try
        {
            Set("font", Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));
            typeof(TitleScreen).GetField("introPlayed", Hidden).SetValue(null, true);
            Set("viewport", viewport); Set("page", page); Set("sceneryHost", scenery);
            Set("leafLayer", leaves); Set("stageView", stage); Set("stageImage", image);
            Call(screen, "Home");
            var group = (CanvasGroup)Get("homePresentation");
            Check(group != null && group.transform.childCount > 10, "Production Home UI was not grouped.");
            Check(scenery.parent == page && leaves.parent == page && image.transform.parent == scenery,
                "Home fade incorrectly captured the scenery or seasonal particles.");
            var buttons = ((System.Collections.Generic.List<UnityEngine.UI.Selectable>)Get("controls"))
                .OfType<UnityEngine.UI.Button>().ToArray();
            Check(buttons.Length == 5 && buttons.All(button => button.transform.parent == group.transform), "Home controls lost their presentation parent.");
            var campaign = buttons[0];
            Check(campaign.GetComponent<RectTransform>().anchoredPosition.x == 405f, "Grouping moved the menu layout.");
            Advance(0f, true);
            Check(!Advance(7.9f) && group.alpha == 1f && group.blocksRaycasts, "Showcase began before its idle delay.");
            Play("snd_swish1");
            Check(Voices().Length == 0, "Menu-mode combat played or queued audio.");
            SoundController.SetSoundVolume(.6f);
            for (int i = 0; i < 26; i++) Advance(8.1f + i * .05f);
            Check(group.alpha == 0f && !group.blocksRaycasts && !group.interactable && !Cursor.visible,
                "Idle showcase did not hide/disable the menu and cursor.");
            Check(image.color == Color.white, "Showcase did not lift the backdrop dimming.");
            campaign.onClick.Invoke();
            Check((string)Get("currentPage") == "Home", "Hidden menu input activated a campaign entry.");
            Play("snd_swish1"); Play("snd_swish2", true);
            var voices = Voices();
            Check(voices.Length == 2 && voices.All(source => source.isPlaying), "Native animation sound did not reach showcase audio sources.");
            Check(voices.All(source => Mathf.Abs(source.volume - .3f) < .001f), "Showcase ignored native clip/player volume.");
            for (int i = 0; i < 20; i++) Play("snd_swish2", true);
            Check(Voices().Length == 2, "Repeated loop actions multiplied showcase audio sources.");
            SoundController.SetSoundVolume(0f);
            var audio = UnityEngine.Object.FindFirstObjectByType<EclipseUiAudio>();
            Call(audio, "UpdateFightVolumes");
            Check(Voices().All(source => source.volume == 0f), "Live sound muting did not silence showcase loops.");
            SoundController.SetSoundVolume(.6f);
            Check(Advance(10f, true), "Wake input was not consumed.");
            Check(!group.blocksRaycasts && Cursor.visible && Voices().Length == 0, "Wake input left hidden buttons or audio active.");
            campaign.onClick.Invoke();
            Check((string)Get("currentPage") == "Home", "Wake click also opened a menu destination.");
            for (int i = 0; i < 6; i++) Advance(10.1f + i * .05f);
            Check(group.alpha == 1f && !group.blocksRaycasts, "Menu did not restore or lost its same-frame wake guard.");
            Set("showcaseWakeFrame", Time.frameCount - 1);
            Check(!Advance(10.5f) && group.blocksRaycasts && group.interactable, "Restored menu did not become interactive on the next frame.");
            Check(Mathf.Abs(image.color.r - .78f) < .001f, "Menu mode did not restore backdrop dimming.");
            for (int i = 0; i < 26; i++) Advance(19f + i * .05f);
            Play("snd_swish2", true);
            Check(Voices().Length == 1, "Idle re-entry replayed stale sounds.");
            Advance(21f, false, false);
            Check(Voices().Length == 0 && Cursor.visible, "Focus loss left showcase audio playing.");
            for (int i = 0; i < 30; i++) Advance(40f + i * .05f, false, false);
            Check(group.alpha == 1f, "An unfocused window entered showcase mode.");
            Set("optionsOnly", true);
            for (int i = 0; i < 30; i++) Advance(60f + i * .05f);
            Check(group.alpha == 1f, "In-game options entered title showcase mode.");
            Set("optionsOnly", false);
            Call(screen, "Settings", "Audio");
            Check(Get("homePresentation") == null && Voices().Length == 0, "Page change retained showcase UI/audio.");
            Check(Fight.GetCurrentFight() == firstFight && Get("stageView") == stage, "UI mood changes reloaded the fight/content.");
            Call(screen, "Home");
            Check(((CanvasGroup)Get("homePresentation")).alpha == 1f, "Returning Home preserved a hidden menu.");
            firstFight.UpdateLife(firstRight, -firstRight.Parameters.MaxLife * 2f);
            firstFight.SetLife(firstLeft, 0f);
            Check(firstLeft.Parameters.HABJPOFCIHA() == 1f && firstRight.Parameters.HABJPOFCIHA() == 1f
                && !firstLeft.Parameters.PCALDKCJGCK && !firstRight.Parameters.PCALDKCJGCK,
                "Title immortality failed for lethal damage or direct health assignment.");
            firstLeft.AddEventListener(0, LeftInterval); firstRight.AddEventListener(0, RightInterval);
            for (int i = 0; i < 2700; i++) Call(stage, "Advance");
            Check(Fight.GetCurrentFight() == firstFight && leftAttacks > 0 && rightAttacks > 0,
                "Title fight stopped attacking or restarted at its old timeout.");
            Check(firstLeft.Parameters.HABJPOFCIHA() == 1f && firstRight.Parameters.HABJPOFCIHA() == 1f,
                "Native combat reduced infinite-health title fighters' life.");
        }
        finally
        {
            typeof(EclipseUiAudio).GetMethod("SetTitleFightFocus", Hidden).Invoke(null, new object[] { 0f });
            SoundController.SetSoundVolume(oldSound);
            Set("stageView", null);
            UnityEngine.Object.DestroyImmediate(canvas);
            UnityEngine.Object.DestroyImmediate(owner);
            Cursor.visible = true;
        }
    }

    static VersusLoadout leftLoadoutForTest() => (VersusLoadout)stageType.GetField("leftLoadout", Hidden).GetValue(stage);
    static VersusLoadout rightLoadoutForTest() => (VersusLoadout)stageType.GetField("rightLoadout", Hidden).GetValue(stage);

    static void Finish(int code)
    {
        SessionState.SetBool(Active, false);
        EditorApplication.update -= Update;
        Application.logMessageReceived -= Capture;
        if (oldEffectSound != null) ModVisuals.PlayEffectSound = oldEffectSound;
        if (stage != null) { try { Call(stage, "Dispose"); } catch { } stage = null; }
        EditorApplication.Exit(code);
    }
}
