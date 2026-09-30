using System;
using System.IO;
using System.Reflection;
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
                Check(firstLeft.Parameters.AiControlled && firstRight.Parameters.AiControlled && !firstLeft.Parameters.UserControlled && !firstRight.Parameters.UserControlled, "Both fighters must be CPUs.");
                firstLeft.AddEventListener(0, LeftInterval); firstRight.AddEventListener(0, RightInterval);
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
                    if (firstLeft.Parameters.HABJPOFCIHA() < .999f || firstRight.Parameters.HABJPOFCIHA() < .999f) damaged = true;
                    if (!forcedKnockout && ticks >= 1500)
                    {
                        Check(leftAttacks > 0 && rightAttacks > 0 && damaged, "CPUs failed to trade native attacks/damage: " + leftAttacks + "/" + rightAttacks + ", damage=" + damaged + "; moves=" + firstLeft.GetCurrentAnimation()?.Name + "/" + firstRight.GetCurrentAnimation()?.Name);
                        firstFight.UpdateLife(firstRight, -firstRight.Parameters.CIDCNCDFONA * 2f);
                        forcedKnockout = true;
                    }
                }
                else
                {
                    rematched = true;
                    Check(Fight.GetCurrentFight() != null && Fight.GetCurrentFight().GetPlayerModel() != firstLeft, "Rematch did not create fresh fighters.");
                    FinishChecks(); return;
                }
            }
            if (ticks > 2700 && !rematched) throw new Exception("Title did not rematch after knockout.");
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
        Check(leftAttacks > 0 && rightAttacks > 0 && damaged, "Both CPUs must trade attacks and damage before rematching.");
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
        Debug.Log("[TitleSparringNative] PASS: " + checks + " native CPU attacks, damage, rematch, save isolation, content reuse and cleanup checks across all ten title locations.");
        Finish(0);
    }

    static void CaptureStage()
    {
        var camera = (UnityEngine.Camera)stageType.GetField("camera", Hidden).GetValue(stage);
        camera.Render();
        var texture = (RenderTexture)stageType.GetProperty("Texture", Hidden).GetValue(stage);
        var old = RenderTexture.active;
        var location = (Location)stageType.GetField("location", Hidden).GetValue(stage);
        var render = (global::Render)stageType.GetField("render", Hidden).GetValue(stage);
        var container = render.PFELMKLNBMC.MJNPBMOAFML().transform;
        Check(Mathf.Abs(container.parent.localPosition.x) < .001f && container.parent.localScale == Vector3.one, "Gameplay camera moved/scaled the title game layer.");
        Check(GameUtils.CKOPPGCIHPL() == location.MFAPMDDJBBL && GameUtils.FBOGLADLJML() == location.JMLAKAKDBBL - location.MFAPMDDJBBL, "Title uses artificial walls instead of the native stage walls.");
        foreach (var model in new[] { firstLeft, firstRight })
        {
            var point = model.PLBNCDCFPML();
            var projected = camera.WorldToViewportPoint(container.TransformPoint(new Vector3(point.GetX(), point.GetY(), 0)));
            Check(projected.x >= .05f && projected.x <= .95f, "CPU is outside title view: " + projected);
            Check(model.MJNPBMOAFML().GetComponentsInChildren<MeshRenderer>().Length > 0, "CPU has no native rendered mesh.");
        }
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
            File.WriteAllBytes(Path.Combine(Directory.GetParent(Application.dataPath).FullName, Scenes[sceneIndex] + ".png"), picture.EncodeToPNG());
        }
        finally { RenderTexture.active = old; UnityEngine.Object.Destroy(picture); }
    }

    static void Finish(int code)
    {
        SessionState.SetBool(Active, false);
        EditorApplication.update -= Update;
        Application.logMessageReceived -= Capture;
        if (stage != null) { try { Call(stage, "Dispose"); } catch { } stage = null; }
        EditorApplication.Exit(code);
    }
}
