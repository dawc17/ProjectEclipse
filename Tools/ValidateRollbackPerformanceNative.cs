using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using Eclipse.Multiplayer;
using Eclipse.Multiplayer.Online;
using Eclipse.Multiplayer.Rollback;
using Eclipse.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using Debug = UnityEngine.Debug;

// Copy into Assets/Scripts in an isolated title fixture: the benchmark exercises
// production internal policy and real rigs, never the user's saves/editor scene.
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public static class ValidateRollbackPerformanceNative
{
    const string Active = "Eclipse.RollbackPerformanceNative.Active";
    const BindingFlags Hidden = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    static object stage;
    static ObjectGraphSnapshotter snapshotter, reference;
    static readonly StateSnapshot[] Ring = new StateSnapshot[10];
    static readonly StateSnapshot Before = new StateSnapshot(), After = new StateSnapshot();
    static readonly object[] Roots = new object[1];
    static readonly Stopwatch Watch = new Stopwatch();
    static int ticks, samples, checks;
    static double elapsed, maximum, started;
    static long allocated;
    static string failure;
    static bool finished;
    static double Now => (double)Stopwatch.GetTimestamp() / Stopwatch.Frequency;

    sealed class ReferencePolicy : ISnapshotPolicy
    {
        readonly FightSnapshotPolicy policy = new FightSnapshotPolicy();
        public bool IsOpaque(Type type) => policy.IsOpaque(type);
        public bool Captures(FieldInfo field) => policy.Captures(field);
        public bool NeedsFieldCopy(Type type) => policy.NeedsFieldCopy(type);
        public SnapshotCodec CodecFor(Type type) => null;
        public PropertyInfo[] ExtraProperties(Type type) => policy.ExtraProperties(type);
    }

    static ValidateRollbackPerformanceNative()
    {
#if UNITY_EDITOR
        if (!SessionState.GetBool(Active, false)) return;
        started = Now;
        EditorApplication.update += Update;
        Application.logMessageReceived += Capture;
#endif
    }

#if UNITY_EDITOR
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

    public static void BuildPlayer()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        if (!File.Exists(Path.Combine(root, "title-sparring-fixture.marker"))) throw new Exception("Isolated fixture required.");
        var args = Environment.GetCommandLineArgs();
        int at = Array.IndexOf(args, "-benchmarkOutput");
        string output = at >= 0 ? Path.GetFullPath(args[at + 1]) : Path.Combine(root, "BenchmarkPlayer");
        if (!output.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) throw new Exception("Output must stay inside fixture.");
        Directory.CreateDirectory(output);
        Directory.CreateDirectory(Path.Combine(output, "Mods"));
        File.WriteAllText(Path.Combine(output, "rollback-benchmark.marker"), "Isolated rollback benchmark");
        // Use the exact same source XML as the editor fixture. Its copied gameplay
        // archive can be stale because this minimal project omits build processors.
        string source = Eclipse.Content.GameplayContentArchive.NormalizeSourceRoot(Path.Combine(root, "Assets/vanillaXml"));
        string content = Path.Combine(output, Eclipse.Content.GameplayContentArchive.EditableDirectoryName);
        foreach (string file in Eclipse.Content.GameplayContentArchive.GetSourceFiles(source))
        {
            string target = Path.Combine(content, file.Substring(source.Length));
            Directory.CreateDirectory(Path.GetDirectoryName(target));
            File.Copy(file, target, true);
        }
        File.WriteAllText(Path.Combine(content, Eclipse.Content.GameplayContentArchive.EditableMarkerFileName), "Isolated benchmark content");
        PlayerSettings.companyName = "EclipseAcceptance";
        PlayerSettings.productName = Path.GetFileName(root);
        PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone, ScriptingImplementation.IL2CPP);
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, "Assets/RollbackBenchmark.unity");
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = new[] { "Assets/RollbackBenchmark.unity" }, target = BuildTarget.StandaloneWindows64,
            locationPathName = Path.Combine(output, "RollbackBenchmark.exe"), options = BuildOptions.None });
        Debug.Log("[RollbackPerformanceNative] Build " + report.summary.result);
        EditorApplication.Exit(report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded ? 0 : 1);
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void RunPlayer()
    {
        if (Application.isEditor || Array.IndexOf(Environment.GetCommandLineArgs(), "-rollback-benchmark") < 0) return;
        string root = Directory.GetParent(Application.dataPath).FullName;
        if (!File.Exists(Path.Combine(root, "rollback-benchmark.marker"))) throw new Exception("Isolated benchmark player required.");
        Environment.SetEnvironmentVariable("ECLIPSE_MODS_ROOT", Path.Combine(root, "Mods"));
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
        started = Now;
        Application.logMessageReceived += Capture;
        new GameObject("Rollback benchmark").AddComponent<Driver>();
    }

    sealed class Driver : MonoBehaviour { void Update() => ValidateRollbackPerformanceNative.Update(); }

    static object Call(object target, string method, params object[] args) =>
        target.GetType().GetMethod(method, Hidden).Invoke(target, args);

    static void Capture(string message, string stack, LogType type)
    {
        if (type == LogType.Exception && stack.Contains("UnityEditor.Search.SearchInit.IndexationOnStartup")) return;
        if (type == LogType.Exception || type == LogType.Assert || type == LogType.Error || message.Contains("abnormal mesh bounds"))
            failure = failure ?? message + "\n" + stack;
    }

    static void Update()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying || EditorApplication.isCompiling) return;
        EditorApplication.QueuePlayerLoopUpdate();
#endif
        if (finished) return;
        try
        {
            if (failure != null) throw new Exception(failure);
            if (Now - started > 240) throw new TimeoutException("Rollback benchmark timed out.");
            if (stage == null)
            {
                UnityEngine.Random.InitState(271828);
                typeof(TitleScreen).GetNestedType("TitleGameData", Hidden).GetMethod("Load", Hidden).Invoke(null, null);
                stage = typeof(TitleScreen).GetNestedType("StageView", Hidden).GetMethod("Open", Hidden).Invoke(null, new object[] { "skyport" });
                if (stage == null) throw new Exception("Stage failed to open.");
                Call(stage, "Tick", 1280, 720);
                var left = VersusLoadout.Default.With(LoadoutSlot.Weapon, "WEAPON_AXES")
                    .With(LoadoutSlot.Armor, "ARMOR_ALLOY").With(LoadoutSlot.Helm, "HELM_FACELESS_MASK");
                var right = VersusLoadout.Default.With(LoadoutSlot.Weapon, "WEAPON_AXES")
                    .With(LoadoutSlot.Armor, "ARMOR_SPACE_GOWN").With(LoadoutSlot.Helm, "HELM_VISOR");
                if (!left.IsValid || !right.IsValid) throw new Exception("Benchmark loadouts are missing.");
                Call(stage, "ShowFighters", left, right,
                    (float)Call(stage, "PageToFightX", -332.8f, 1280f / 720f),
                    (float)Call(stage, "PageToFightX", 332.8f, 1280f / 720f));
                snapshotter = FightRollback.CreateSnapshotter();
                reference = new ObjectGraphSnapshotter(new ReferencePolicy());
                for (int i = 0; i < Ring.Length; i++) Ring[i] = new StateSnapshot();
                if (Array.IndexOf(Environment.GetCommandLineArgs(), "-rollbackBenchmarkOnly") < 0) CheckCodecs();
                return; // Let Unity initialize renderers before measuring or restoring.
            }
            for (int step = 0; step < 4; step++)
            {
                Call(stage, "Advance");
                Roots[0] = Fight.GetCurrentFight();
                if (Roots[0] == null) throw new Exception("Fight stopped.");
                var saved = Ring[ticks % Ring.Length];
                long allocationStart = GC.GetAllocatedBytesForCurrentThread();
                Watch.Restart();
                snapshotter.Capture(saved, ticks, Roots);
                Watch.Stop();
                if (ticks >= 120)
                {
                    samples++;
                    elapsed += Watch.Elapsed.TotalMilliseconds;
                    maximum = Math.Max(maximum, Watch.Elapsed.TotalMilliseconds);
                    allocated += GC.GetAllocatedBytesForCurrentThread() - allocationStart;
                }
                if (ticks % 60 == 0 && Array.IndexOf(Environment.GetCommandLineArgs(), "-rollbackBenchmarkOnly") < 0)
                {
                    // The independent reference walk has NO production codecs: it
                    // catches fields, aliases and array slots omitted by a fast codec.
                    reference.Capture(Before, ticks, Roots);
                    Call(stage, "Advance");
                    Call(stage, "Advance");
                    snapshotter.Restore(saved);
                    reference.Capture(After, ticks, Roots);
                    string difference = reference.FirstDifference(Before, After);
                    if (difference != null) throw new Exception("Restore at tick " + ticks + ": " + difference);
                    checks++;
                }
                ticks++;
            }
            if (ticks < 1200) return;
            if (failure != null) throw new Exception(failure);
            Debug.Log(string.Format(CultureInfo.InvariantCulture,
                "[RollbackPerformanceNative] PASS: {0} captures, {1} full reference restore checks; {2} objects; save {3:F3} ms mean / {4:F3} max; {5:F1} bytes/capture; raw={6}, fallbacks={7}.",
                ticks, checks, snapshotter.LastObjectCount, elapsed / samples, maximum, (double)allocated / samples,
                snapshotter.UsesRawMemory, snapshotter.RawFallbacks));
            Finish(0);
        }
        catch (Exception error) { Debug.LogError("[RollbackPerformanceNative] FAIL: " + error); Finish(1); }
    }

    sealed class ExtendedWeight : Pair<ModelNode, float>
    {
        public int Extra = 42;
        public ExtendedWeight(ModelNode node) : base(node, .75f) { }
    }

    static void CheckCodecs()
    {
        var node = new ModelNode("codec point");
        var macro = new ModelMacroNode("codec macro", new Vector3f(3, 4, 5));
        var pair = new Pair<ModelNode, float>(node, .25f);
        var extended = new ExtendedWeight(macro);
        var weights = new Pair<ModelNode, float>[] { pair, null, pair, extended };
        var triangle = new Triangle(node, macro, node, "codec triangle");
        var triangles = new Triangle[] { triangle, null, triangle };
        var frame = new KeyFrames.Frame { Size = 1, Data = new List<Vector3f>(8) { node.GetStart() } };
        var frames = new KeyFrames.Frame[] { frame, null, frame };
        object[] testRoots = { node, macro, weights, pair, triangles, frames };
        var saved = new StateSnapshot();
        // Flip every declared scalar/reference field, including private cached flags.
        // Reflection's independent snapshot catches newly added fields a codec omits.
        for (int pass = 0; pass < 2; pass++)
        {
            reference.Capture(Before, pass, testRoots);
            snapshotter.Capture(saved, pass, testRoots);
            foreach (var target in new ModelNode[] { node, macro })
            {
                for (var type = target.GetType(); type != typeof(object); type = type.BaseType)
                    foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                    {
                        if (field.FieldType == typeof(bool)) field.SetValue(target, !(bool)field.GetValue(target));
                        else if (field.FieldType == typeof(float)) field.SetValue(target, 87.125f);
                        else if (field.FieldType == typeof(int)) field.SetValue(target, -711);
                        else if (field.FieldType.IsEnum) field.SetValue(target, Enum.ToObject(field.FieldType, 31));
                        else if (field.FieldType == typeof(string)) field.SetValue(target, "changed");
                        else if (field.FieldType == typeof(Vector3f))
                        {
                            var point = (Vector3f)field.GetValue(target);
                            point?.Set(71, -83, 93);
                            field.SetValue(target, new Vector3f(99));
                        }
                        else field.SetValue(target, null);
                    }
            }
            pair.First = macro; pair.Second = -.5f; weights[0] = null;
            extended.Extra = -42; extended.Second = 99;
            triangle.set_Name("changed"); triangle.Nodes[0] = null;
            triangle.Nodes = new ModelNode[] { macro }; triangles[1] = triangle;
            frame.Data.Add(new Vector3f(42)); frame.Data.Capacity = 32;
            frame.Data = new List<Vector3f>(); frame.Size = 31; frames[2] = null;
            snapshotter.Restore(saved);
            reference.Capture(After, pass, testRoots);
            string difference = reference.FirstDifference(Before, After);
            if (difference != null) throw new Exception("Codec fields/aliases/capacity: " + difference);
            checks++;
        }
        Before.Clear(); After.Clear();
    }

    static void Finish(int code)
    {
        finished = true;
#if UNITY_EDITOR
        SessionState.SetBool(Active, false);
        EditorApplication.update -= Update;
#endif
        Application.logMessageReceived -= Capture;
        try { if (stage != null) Call(stage, "Dispose"); }
        finally
        {
#if UNITY_EDITOR
            EditorApplication.Exit(code);
#else
            Application.Quit(code);
#endif
        }
    }
}
