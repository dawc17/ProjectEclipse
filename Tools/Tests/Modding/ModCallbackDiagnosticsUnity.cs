#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Eclipse.Diagnostics;
using Eclipse.Modding;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

[InitializeOnLoad]
public static class ModCallbackDiagnosticsUnity
{
    const string Pending = "Eclipse.ModCallbackDiagnosticsUnity.Pending";
    static int checks;
    static string root;
    static void Check(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static ModCallbackDiagnosticsUnity() { if (SessionState.GetBool(Pending, false)) EditorApplication.update += Begin; }
    public static void Run()
    {
        PlayerSettings.companyName = "EclipseAcceptance";
        PlayerSettings.productName = Path.GetFileName(Path.GetDirectoryName(Application.dataPath));
        SessionState.SetBool(Pending, true); EditorApplication.update += Begin;
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var gameView = EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));
        gameView.Show(); gameView.Focus();
        EditorApplication.EnterPlaymode();
    }
    static void Begin()
    {
        if (!EditorApplication.isPlaying || EditorApplication.isCompiling) return;
        EditorApplication.update -= Begin; SessionState.SetBool(Pending, false);
        new GameObject("Callback diagnostics acceptance").AddComponent<ModCallbackDiagnosticsRunner>();
    }
    static void Write(string id, string body, string caps, string dependency = null)
    {
        var path = Path.Combine(root, "Mods", id);
        Directory.CreateDirectory(Path.Combine(path, "scripts"));
        File.WriteAllText(Path.Combine(path, "scripts/main.lua"), "local sf2=require('sf2')\n" + body);
        File.WriteAllText(Path.Combine(path, "mod.toml"),
            "schema=1\nid=\"" + id + "\"\nname=\"Fixture\"\nversion=\"1.0.0\"\nauthors=[\"Fixture\"]\nentrypoint=\"scripts/main.lua\"\ncapabilities=[" + caps + "]\n" +
            (dependency == null ? "" : "[[dependencies]]\nid=\"" + dependency + "\"\nversion=\">=1.0.0 <2.0.0\"\n"));
    }
    public static IEnumerator Steps()
    {
        root = Path.GetDirectoryName(Application.dataPath);
        Check(Application.isPlaying && SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null, "Native graphics/Play Mode required");
        var camera = new GameObject("Diagnostics backdrop").AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = Color.black;
        Write("diagnostics.provider", "sf2.extensions.register{id='work',version=1,request={n='integer'},response={value='integer'},handler=function(r) " +
            "if r.n==2 then error('intentional provider failure') end; if r.n==3 then while true do end end; return {value=r.n} end}", "\"extensions.provide\"");
        Write("diagnostics.consumer", "local work=sf2.extensions.get('diagnostics.provider:extensions/work',1); " +
            "sf2.ui.open{id='probe',mount='menu',root={id='root',kind='column',width=120,height=160,children={{id='b1',kind='button',width=100,height=40,text='Good'},{id='b2',kind='button',width=100,height=40,text='Fail'},{id='b3',kind='button',width=100,height=40,text='Budget'}}}, " +
            "on_click=function(view,id) sf2.extensions.try_call(work,{n=tonumber(string.sub(id,2))}) end}", "\"extensions.call\",\"ui.create\"", "diagnostics.provider");
        var discovery = ModDiscovery.DiscoverLoose(Path.Combine(root, "Mods"));
        Check(!discovery.HasErrors, "Fixture discovery failed");
        var views = new List<ModUiSurface>();
        var diagnostics = new ModCallbackDiagnostics { Recording = true };
        using var session = ModScriptSession.Start(new ModHost(discovery.Mods), new MoonSharpScriptRuntime(views.Add), null, null, diagnostics);
        Check(!session.HasErrors && diagnostics.TimedCalls == 2, "Lua startup/timing failed: " + session.FormatReport());
        ModRuntime.Scripts = session;
        PerformanceOverlay.SetMode(PerformanceOverlay.Mode.Detailed);
        Check(views[0].TryClick("b1") && views[0].TryClick("b2") && views[0].TryClick("b3"), "Callback probe failed");
        Check(diagnostics.FailureCount == 2 && diagnostics.BudgetExceededCount == 1, "Native failures/budget not captured");
        var overlay = UnityEngine.Object.FindFirstObjectByType<PerformanceOverlay>();
        Check(overlay != null, "Production overlay did not initialize");
        yield return new WaitForSecondsRealtime(1.5f);
        for (int i = 0; i < 5; i++) yield return null;
        string summary = (string)typeof(PerformanceOverlay).GetField("_summary", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(overlay);
        Check(summary.Contains("Lua: 8 timed calls, 2 failures"), "Detailed overlay lacks runtime measurements: " + summary);
        Check(summary.Contains("diagnostics.provider"), "Slow provider missing from displayed attribution");
        // WaitForEndOfFrame coroutines do not advance in batch-mode editors.
        // Request the native screen write and observe its completion instead.
        string screenshotPath = Path.Combine(root, "callback-overlay.png");
        ScreenCapture.CaptureScreenshot(screenshotPath);
        float captureDeadline = Time.realtimeSinceStartup + 10;
        while (!File.Exists(screenshotPath) && Time.realtimeSinceStartup < captureDeadline) yield return null;
        Check(File.Exists(screenshotPath), "Native screen capture did not complete");
        var screenshot = new Texture2D(2, 2);
        Check(screenshot.LoadImage(File.ReadAllBytes(screenshotPath)), "Native screen capture was not a PNG");
        Check(screenshot.width > 400 && screenshot.height > 280, "Overlay screenshot too small");
        Check(screenshot.GetPixels32().Count(pixel => pixel.r > 150 && pixel.g > 150 && pixel.b > 150) > 300, "Overlay did not render visible text");
        UnityEngine.Object.Destroy(screenshot);
        // A scripted F4 key goes through the actual native Update/report path.
        Eclipse.Input.EclipseInput.Keys.Add(KeyCode.F4);
        yield return null; yield return null;
        var reports = Directory.GetFiles(Path.Combine(Eclipse.Runtime.EditorPlayModeContext.PersistentDataPath, "Diagnostics"), "performance-*.txt");
        Check(reports.Length == 1, "F4 did not save its native report");
        var report = File.ReadAllText(reports[0]);
        Check(report.Contains("format 5") && report.Contains("Active mod session (resolved load order)"), "F4 lacks session evidence");
        Check(report.Contains("diagnostics.provider:extensions/work:handler") && report.Contains("BUDGET_EXCEEDED") && report.Contains("intentional provider failure"), "F4 lacks errors, callback identity or budget evidence");
        Check(GUIUtility.systemCopyBuffer == reports[0], "Report path was not copied");
        long before = diagnostics.TimedCalls;
        PerformanceOverlay.SetMode(PerformanceOverlay.Mode.Off);
        Check(!diagnostics.Recording && views[0].TryClick("b1") && diagnostics.TimedCalls == before, "Off mode still timed callbacks");
        Check(views[0].TryClick("b2") && diagnostics.FailureCount == 3 && !diagnostics.RecentFailures.Last().Timed, "Off mode lost failure history");
        Eclipse.Input.EclipseInput.Keys.Add(KeyCode.F3);
        yield return null; yield return null;
        Check(PerformanceOverlay.CurrentMode == PerformanceOverlay.Mode.Compact && diagnostics.Recording, "F3 did not reenable recording");
        Check(views[0].TryClick("b1") && diagnostics.TimedCalls == before + 2, "Recording did not recover");
        session.Dispose(); ModRuntime.Scripts = null;
        Check(views.All(view => view.IsClosed), "Session cleanup leaked views");
        PerformanceOverlay.SetMode(PerformanceOverlay.Mode.Off);
        yield return null;
        File.WriteAllText(Path.Combine(root, "validation-result.txt"), "PASS: " + checks + " native callback diagnostics checks. Actual Lua, F3/F4 Update, IMGUI pixels and saved report; game/session/input sources controlled.");
        UnityEngine.Debug.Log("[ModCallbackDiagnosticsUnity] PASS " + checks);
        EditorApplication.Exit(0);
    }
}
public sealed class ModCallbackDiagnosticsRunner : MonoBehaviour
{
    void Start() { StartCoroutine(Run()); }
    IEnumerator Run()
    {
        var steps = ModCallbackDiagnosticsUnity.Steps();
        while (true)
        {
            object next;
            try { if (!steps.MoveNext()) yield break; next = steps.Current; }
            catch (Exception error) { UnityEngine.Debug.LogError("[ModCallbackDiagnosticsUnity] " + error); EditorApplication.Exit(1); yield break; }
            yield return next;
        }
    }
}
#endif
