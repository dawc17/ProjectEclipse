#if UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using System.Linq;
using Eclipse.Modding;
using Eclipse.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoad]
public static class ValidateModMenuUI
{
    static ValidateModMenuUI() { EditorApplication.update += Update; }
    public static void Run()
    {
        SessionState.SetBool("ModMenuUITest", true);
        EditorApplication.isPlaying = true;
    }
    static void Update()
    {
        if (!SessionState.GetBool("ModMenuUITest", false) || !EditorApplication.isPlaying || !Application.isPlaying) return;
        SessionState.SetBool("ModMenuUITest", false);
        new GameObject("Mod UI validation").AddComponent<ModMenuUIRunner>();
    }
}
public class ModMenuUIRunner : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        try { Validate(); }
        catch (Exception exception) { Debug.LogException(exception); EditorApplication.Exit(1); }
    }
    private void Require(bool value, string message) { if (!value) throw new Exception(message); }
    private void Validate()
    {
        string mods = Path.Combine(Application.dataPath, "ModMenuFixtures");
        Environment.SetEnvironmentVariable("ECLIPSE_MODS_ROOT", mods);
        string settings = ModHost.GetSelectionPath(mods);
        if (File.Exists(settings)) File.Delete(settings); // Isolated fixture's own selection only.
        var title = new GameObject("Title", typeof(RectTransform)).AddComponent<TitleScreen>();
        var homeButtons = title.GetComponentsInChildren<Button>();
        Require(homeButtons.Count(b => b.name == "MODS") == 1, "Dedicated Mods home entry missing.");
        Capture(title.GetComponent<Canvas>(), "mod-menu-home.png");
        homeButtons.Single(b => b.name == "MODS").onClick.Invoke();
        Require(title.GetComponentsInChildren<Text>().Any(t => t.text.Contains("Always enabled")), "Core lock label missing.");
        var toggle = title.GetComponentsInChildren<Button>().First(b => b.name == "Enabled");
        toggle.onClick.Invoke();
        Require(!File.Exists(settings), "UI persisted draft before Apply.");
        Require(title.GetComponentsInChildren<Button>().Any(b => b.name == "Disabled"), "Toggle did not update display.");
        Capture(title.GetComponent<Canvas>(), "mod-menu-list.png");
        title.GetComponentsInChildren<Button>().Single(b => b.name == "Apply & Restart").onClick.Invoke();
        Require(SceneManagerSF.Loads == 1 && File.Exists(settings), "Apply failed to persist or request reload.");
        Require(ModSelection.Load(settings).Filter(ModDiscovery.DiscoverLoose(mods).Mods).Count < ModDiscovery.DiscoverLoose(mods).Mods.Count,
            "Saved selection did not disable a mod.");
        DestroyImmediate(title.gameObject);
        GameSessionRestart.ArrivedAtTitle();
        var menuObject = new GameObject("In-game menu");
        var menu = menuObject.AddComponent<Nekki.SF2.GUI.Menu.MainMenu>();
        menu.Scroll = menuObject.AddComponent<Nekki.SF2.GUI.Menu.MenuScroll>();
        ReturnToTitleButton.Attach(menu);
        var component = menu.GetComponent<ReturnToTitleButton>();
        var update = typeof(ReturnToTitleButton).GetMethod("Update", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        menu.Scroll.CurScrollState = Nekki.SF2.GUI.Menu.MenuScroll.ANJKEGGALAG.ScrollOpen;
        update.Invoke(component, null);
        var returnButton = menu.GetComponentsInChildren<Button>().Single();
        Require(returnButton.gameObject.activeInHierarchy, "Return entry not visible when menu opens.");
        Capture(menu.GetComponentInChildren<Canvas>(), "mod-menu-return.png");
        menu.Scroll.CurScrollState = Nekki.SF2.GUI.Menu.MenuScroll.ANJKEGGALAG.ScrollClose;
        update.Invoke(component, null);
        Require(!returnButton.gameObject.activeInHierarchy, "Return entry visible outside menu.");
        File.Delete(settings);
        Debug.Log("[ModMenuUI] PASS: title entry, core lock, draft toggle, persistence/reload request, menu open/close visibility and rendered captures.");
        EditorApplication.Exit(0);
    }
    private void Capture(Canvas canvas, string name)
    {
        var cameraObject = new GameObject("Capture camera");
        var camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color32(70, 55, 40, 255);
        var texture = new RenderTexture(1280, 720, 24);
        camera.targetTexture = texture;
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = camera;
        canvas.planeDistance = 1;
        Canvas.ForceUpdateCanvases();
        camera.Render();
        RenderTexture.active = texture;
        var image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); image.Apply();
        File.WriteAllBytes(Path.Combine(Application.dataPath, "../../" + name), image.EncodeToPNG());
        RenderTexture.active = null;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.worldCamera = null;
        DestroyImmediate(image); DestroyImmediate(texture); DestroyImmediate(cameraObject);
    }
}
#endif
