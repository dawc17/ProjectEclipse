using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Eclipse.Multiplayer;
using Eclipse.UI;
using Nekki.SF2.Core.Fights.Renders.Model;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Runs only in the isolated project prepared by TestTitleSparringNative.py.
// Exercises the real model loader, title combat, interpolation and Unity meshes.
[InitializeOnLoad]
public static class ValidateTitleMeshNative
{
    const string Active = "Eclipse.TitleMeshNative.Active";
    const BindingFlags Hidden = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
    static object stage;
    static int ticks, vertices;
    static double started;
    static string failure;
    static bool finished;

    static ValidateTitleMeshNative()
    {
        if (!SessionState.GetBool(Active, false)) return;
        started = EditorApplication.timeSinceStartup;
        EditorApplication.update += Update;
        Application.logMessageReceived += Capture;
    }

    public static void RunEditor()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        if (!File.Exists(Path.Combine(root, "title-sparring-fixture.marker")))
            throw new InvalidOperationException("An isolated title fixture is required.");
        Environment.SetEnvironmentVariable("ECLIPSE_MODS_ROOT", Path.Combine(root, "Mods"));
        PlayerSettings.companyName = "EclipseAcceptance";
        PlayerSettings.productName = Path.GetFileName(root);
        SessionState.SetBool(Active, true);
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorApplication.EnterPlaymode();
    }

    static object Call(object target, string method, params object[] args) =>
        target.GetType().GetMethod(method, Hidden).Invoke(target, args);

    static void Capture(string message, string stack, LogType type)
    {
        // Unity 6's editor search index can throw during isolated-project boot.
        // This is editor-only; leave its log visible, but keep checking game errors.
        if (type == LogType.Exception && stack.Contains("UnityEditor.Search.SearchInit.IndexationOnStartup")) return;
        if (type == LogType.Exception || type == LogType.Assert || type == LogType.Error ||
            message.Contains("abnormal mesh bounds")) failure = failure ?? message + "\n" + stack;
    }

    static void Update()
    {
        if (finished || !EditorApplication.isPlaying || EditorApplication.isCompiling) return;
        EditorApplication.QueuePlayerLoopUpdate();
        var previousCulture = CultureInfo.CurrentCulture;
        try
        {
            if (failure != null) throw new Exception(failure);
            if (EditorApplication.timeSinceStartup - started > 180) throw new TimeoutException("Title mesh test timed out.");
            // Set it after domain reload and before parsing any content. Unity's own
            // editor loop can reset thread culture between callbacks.
            var args = Environment.GetCommandLineArgs();
            int cultureArg = Array.IndexOf(args, "-titleMeshCulture");
            string culture = cultureArg >= 0 ? args[cultureArg + 1] : "hr-HR";
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            if (stage == null)
            {
                typeof(TitleScreen).GetNestedType("TitleGameData", Hidden).GetMethod("Load", Hidden).Invoke(null, null);
                var stageType = typeof(TitleScreen).GetNestedType("StageView", Hidden);
                stage = stageType.GetMethod("Open", Hidden).Invoke(null, new object[] { "night_bridge" });
                if (stage == null) throw new Exception("Title stage could not open.");
                Call(stage, "Tick", 1280, 720);
                // Equipment/model families from the reported Croatian player log.
                var left = VersusLoadout.Default.With(LoadoutSlot.Weapon, "WEAPON_KATANA")
                    .With(LoadoutSlot.Armor, "ARMOR_BRONZE").With(LoadoutSlot.Helm, "HELM_ADVANCED_CONICAL");
                var right = VersusLoadout.Default.With(LoadoutSlot.Weapon, "WEAPON_SHARP_TONFA")
                    .With(LoadoutSlot.Armor, "ARMOR_HW15_WICKED_VEIL").With(LoadoutSlot.Helm, "HELM_RAID_FUNGUS_SET");
                if (!left.IsValid || !right.IsValid) throw new Exception("Regression loadouts are missing from the roster.");
                Call(stage, "ShowFighters", left, right,
                    (float)Call(stage, "PageToFightX", -332.8f, 1280f / 720f),
                    (float)Call(stage, "PageToFightX", 332.8f, 1280f / 720f));
            }
            for (int i = 0; i < 4; i++)
            {
                Call(stage, "Advance");
                var fight = Fight.GetCurrentFight();
                if (fight == null) throw new Exception("Title sparring stopped.");
                foreach (var model in new[] { fight.GetPlayerModel(), fight.GetEnemyModel() })
                {
                    foreach (var renderer in model.MJNPBMOAFML().GetComponentsInChildren<MeshRender>())
                    {
                        var mesh = renderer.get_Base();
                        if (mesh.Vertices == null) continue; // Unity Start has not run yet.
                        foreach (float alpha in new[] { 0f, .5f, 1f })
                        {
                            mesh.Render(alpha);
                            foreach (var vertex in mesh.Vertices)
                            {
                                if (float.IsNaN(vertex.x) || float.IsNaN(vertex.y) ||
                                    Mathf.Abs(vertex.x) > 10000f || Mathf.Abs(vertex.y) > 10000f)
                                    throw new Exception("Unbounded title vertex at tick " + ticks + " (" + culture + "): " + vertex);
                                vertices++;
                            }
                        }
                    }
                }
                ticks++;
            }
            if (failure != null) throw new Exception(failure);
            if (ticks < 600) return;
            if (vertices == 0) throw new Exception("No native fighter meshes were checked.");
            Debug.Log("[TitleMeshNative] PASS: " + culture + ", " + ticks + " title ticks, " + vertices + " bounded interpolated vertices.");
            Finish(0);
        }
        catch (Exception error)
        {
            Debug.LogError("[TitleMeshNative] FAIL: " + error);
            Finish(1);
        }
        finally { CultureInfo.CurrentCulture = previousCulture; }
    }

    static void Finish(int code)
    {
        finished = true;
        SessionState.SetBool(Active, false);
        EditorApplication.update -= Update;
        Application.logMessageReceived -= Capture;
        try { if (stage != null) Call(stage, "Dispose"); }
        finally { EditorApplication.Exit(code); }
    }
}
