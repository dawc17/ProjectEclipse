using System;
using System.Collections;
using System.IO;
using System.Reflection;
using Eclipse.Multiplayer;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Runs the full production preview component against native cameras, MSAA textures
// and scene teardown. Fighter meshes are controlled quads, not recovered game art.
[InitializeOnLoad]
public static class VersusPreviewNative
{
    const string Pending = "Eclipse.VersusPreviewNative.Pending";
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    static int checks;

    static VersusPreviewNative()
    {
        if (SessionState.GetBool(Pending, false)) EditorApplication.update += Begin;
    }

    public static void Run()
    {
        SessionState.SetBool(Pending, true);
        EditorApplication.update += Begin;
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorApplication.EnterPlaymode();
    }

    static void Begin()
    {
        if (!EditorApplication.isPlaying || EditorApplication.isCompiling) return;
        EditorApplication.update -= Begin;
        SessionState.SetBool(Pending, false);
        new GameObject("Preview native validation").AddComponent<VersusPreviewNativeRunner>();
    }

    static T Field<T>(object owner, string name) => (T)owner.GetType().GetField(name, Hidden).GetValue(owner);
    static void Check(bool value, string reason)
    {
        checks++;
        if (!value) throw new Exception(reason);
    }

    static Color32[] Pixels(RenderTexture texture)
    {
        var previous = RenderTexture.active;
        var readback = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        try
        {
            RenderTexture.active = texture;
            readback.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            readback.Apply();
            return readback.GetPixels32();
        }
        finally { RenderTexture.active = previous; UnityEngine.Object.Destroy(readback); }
    }

    static bool EqualPixels(Color32[] before, Color32[] after)
    {
        if (before.Length != after.Length) return false;
        for (int i = 0; i < before.Length; i++) if (!before[i].Equals(after[i])) return false;
        return true;
    }

    public static IEnumerator Tests()
    {
        Check(Application.unityVersion == "6000.6.0f1", "Use the matching Unity editor.");
        Check(SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null, "Native rendering is required.");
        var outgoing = SceneManager.CreateScene("Outgoing preview scene");
        SceneManager.SetActiveScene(outgoing);
        var root = new GameObject("Persistent VS screen", typeof(RectTransform), typeof(Canvas));
        root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        UnityEngine.Object.DontDestroyOnLoad(root);
        var previews = new VersusFighterPreview[2];
        for (int i = 0; i < previews.Length; i++)
        {
            previews[i] = VersusFighterPreview.Create(root.GetComponent<RectTransform>(), i == 1, Color.white);
            previews[i].Show(new VersusLoadout(), true);
        }
        for (int frame = 0; frame < 5; frame++) yield return null;
        var worlds = new GameObject[2];
        var originals = new RenderTexture[2];
        var cameras = new UnityEngine.Camera[2];
        var containers = new Nekki.SF2.Core.Fights.ModelContainer[2];
        var ticks = new int[2];
        var pixels = new Color32[2][];
        for (int i = 0; i < previews.Length; i++)
        {
            var preview = previews[i];
            worlds[i] = Field<GameObject>(preview, "_world");
            Check(worlds[i].scene == outgoing, "Preview world belongs to outgoing scene.");
            var camera = Field<UnityEngine.Camera>(preview, "_camera");
            cameras[i] = camera;
            containers[i] = Field<Nekki.SF2.Core.Fights.ModelContainer>(preview, "_container");
            ticks[i] = containers[i].Ticks;
            originals[i] = Field<RenderTexture>(preview, "_texture");
            camera.Render();
            preview.GetType().GetMethod("KeepAliveDuringLoading", Hidden).Invoke(preview, null);
            Check(worlds[i].scene != outgoing, "Preview world preserved independently of outgoing scene.");
            Check(preview.GetComponent<RawImage>().texture == originals[i], "Live camera texture stays attached to UI.");
            Check(camera.enabled && camera.targetTexture == originals[i] && preview.enabled, "Preview continues its update and render path.");
            pixels[i] = Pixels(originals[i]);
            int painted = 0, transparent = 0;
            foreach (var pixel in pixels[i]) { if (pixel.a > 128 && pixel.r > 128) painted++; if (pixel.a == 0) transparent++; }
            Check(painted > 100, "Live preview contains the rendered fighter.");
            Check(transparent > 100, "Live preview preserves transparent background.");
            preview.GetType().GetMethod("KeepAliveDuringLoading", Hidden).Invoke(preview, null);
            Check(preview.GetComponent<RawImage>().texture == originals[i], "Repeated preservation does not allocate another texture.");
        }
        SceneManager.SetActiveScene(SceneManager.CreateScene("Incoming fight scene"));
        yield return SceneManager.UnloadSceneAsync(outgoing);
        for (int frame = 0; frame < 3; frame++) yield return new WaitForFixedUpdate();
        for (int i = 0; i < previews.Length; i++)
        {
            Check(worlds[i] != null && cameras[i] != null && containers[i] != null, "Live model and camera survive scene unloading.");
            Check(previews[i] != null && previews[i].GetComponent<RawImage>().texture == originals[i], "VS screen retains its live texture after unloading.");
            Check(containers[i].Ticks > ticks[i], "Preview animation continues ticking after unloading.");
            cameras[i].Render();
            Check(!EqualPixels(pixels[i], Pixels(originals[i])), "Rendered fighter pixels continue changing after unloading.");
        }
        root.SetActive(false);
        var stoppedTicks = new[] { containers[0].Ticks, containers[1].Ticks };
        for (int frame = 0; frame < 3; frame++) yield return new WaitForFixedUpdate();
        for (int i = 0; i < previews.Length; i++)
            Check(!worlds[i].activeSelf && containers[i].Ticks == stoppedTicks[i], "Hiding the splash stops its persistent preview world.");
        root.SetActive(true);
        yield return new WaitForFixedUpdate();
        for (int i = 0; i < previews.Length; i++)
            Check(worlds[i].activeSelf && containers[i].Ticks > stoppedTicks[i], "Reopening the UI resumes its live preview.");
        UnityEngine.Object.Destroy(root);
        yield return null;
        yield return null;
        for (int i = 0; i < previews.Length; i++)
        {
            Check(worlds[i] == null && cameras[i] == null && originals[i] == null, "Preview teardown releases its persistent world, camera and texture.");
        }
        string result = "PASS: " + checks + " native live preview rendering, animation, unload and cleanup checks (controlled fighter meshes).";
        File.WriteAllText("validation-result.txt", result);
        Debug.Log(result);
        EditorApplication.Exit(0);
    }
}

public sealed class VersusPreviewNativeRunner : MonoBehaviour
{
    IEnumerator Start()
    {
        var tests = VersusPreviewNative.Tests();
        while (true)
        {
            object current;
            try { if (!tests.MoveNext()) yield break; current = tests.Current; }
            catch (Exception error) { Debug.LogException(error); EditorApplication.Exit(1); yield break; }
            yield return current;
        }
    }
}
