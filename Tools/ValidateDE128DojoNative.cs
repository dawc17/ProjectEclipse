using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Eclipse.Modding;
using Eclipse.UI.Modding;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Dialogs;
using Nekki.SF2.GUI.Map;
using Nekki.SF2.GUI.Menu;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Runs only in the isolated project made by TestDE128DojoNative.py.
[InitializeOnLoad]
public static class ValidateDE128DojoNative
{
    const string Active = "Eclipse.DE128DojoNative.Active";
    const string Prefix = "[DE128DojoNative] ";
    const string Choice = "new_year_24_china_dojo";
    static readonly BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    static double started;
    static bool campaign, mapRequested, migrationChecked, clicked, selected, legacyStored;
    static double clickedAt;
    static double selectedAt;
    static double legacyStoredAt;
    static double lastCardPress;
    static int campaignCards;
    static string phase;

    static ValidateDE128DojoNative()
    {
        if (!SessionState.GetBool(Active, false)) return;
        phase = Environment.GetEnvironmentVariable("ECLIPSE_DE128_DOJO_PHASE");
        started = EditorApplication.timeSinceStartup;
        EditorApplication.update += Update;
    }

    public static void RunEditor()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        if (!File.Exists(Path.Combine(root, "de128-dojo-native-fixture.marker")))
            throw new InvalidOperationException("Dojo acceptance requires an isolated project copy.");
        Environment.SetEnvironmentVariable("ECLIPSE_MODS_ROOT", Path.Combine(root, "Mods"));
        PlayerSettings.companyName = "EclipseAcceptance";
        PlayerSettings.productName = Path.GetFileName(root);
        SessionState.SetBool(Active, true);
        EditorSceneManager.OpenScene("Assets/src/GUI/Scenes/GameLoaderScene/GameLoader.unity");
        EditorApplication.EnterPlaymode();
    }

    static void Update()
    {
        if (!EditorApplication.isPlaying) return;
        try
        {
            if (EditorApplication.timeSinceStartup - started > 300)
                throw new Exception("Timed out in " + phase + ": campaign=" + campaign +
                    " map=" + mapRequested + " clicked=" + clicked + " selected=" + selected);
            if (!campaign && Eclipse.UI.TitleScreen.IsOpen)
            {
                var title = UnityEngine.Object.FindObjectOfType<Eclipse.UI.TitleScreen>();
                if (title != null)
                {
                    typeof(Eclipse.UI.TitleScreen).GetMethod("BeginCampaign", Hidden).Invoke(title, null);
                    campaign = true;
                }
                return;
            }
            var scripts = ModRuntime.Scripts;
            var roster = ListSF.CCDKHLAMKKO();
            var module = Module.GetInstance();
            if (scripts == null || roster == null || module == null) return;
            if (scripts.Diagnostics.Count != 0 || scripts.StateDiagnostics.Count != 0)
                throw new Exception("DE128 diagnostics: " + string.Join("; ", scripts.Diagnostics) +
                    "; state=" + string.Join("; ", scripts.StateDiagnostics));
            if (phase == "reload")
            {
                if (module.NMCNDOPKFJD() != ScreenType.ModuleDojo &&
                    module.NMCNDOPKFJD() != ScreenType.ModuleMap) return;
                var button = MapButtonController.ELEBLBJKDBI().MEPCBPIJLGB()
                    .SingleOrDefault(value => value.Name == "de128.dojo_changer");
                if (button == null || button.BIJFFONMDBC.x != 240f ||
                    button.BIJFFONMDBC.y != -650f || button.AnchorMinX != 0f)
                    throw new Exception("The saved off-screen map button was not repositioned after restart.");
                string resolved = ModRuntime.ResolveDojoLocation("dojo");
                string entry = Location.ResolveEntryLocation(BattleType.FightNone, "dojo");
                if (resolved != Choice || entry != Choice)
                {
                    var selection = typeof(ModRuntime).GetField("DojoSelection",
                        BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as ModDojoSelection;
                    throw new Exception("The saved native dojo choice was not rebound after restart: " +
                        "bound=" + selection?.IsBound + " saved=" + selection?.SavedLocation +
                        " resolved=" + resolved + " entry=" + entry);
                }
                Debug.Log(Prefix + "PASS reload: saved installed dojo restored after Unity restart.");
                Finish(0);
                return;
            }
            if (phase != "select") throw new Exception("Unknown phase: " + phase);
            if (!mapRequested && module.NMCNDOPKFJD() == ScreenType.ModuleDojo)
            {
                var menu = MainMenu.get_Instance();
                if (menu == null) return;
                menu.SkipTutorial();
                mapRequested = true;
                return;
            }
            if (!clicked)
            {
                if (module.NMCNDOPKFJD() != ScreenType.ModuleMap ||
                    UnityEngine.Object.FindObjectOfType<MapScene>() == null) return;
                if (NativeBlocked())
                {
                    var active = typeof(DialogsManager).GetField("OALIPPPOHCL",
                        BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null) as BaseDialog;
                    if (active is StoryDialog story && story.IsQuestDialog &&
                        EditorApplication.timeSinceStartup - lastCardPress > 0.2)
                    {
                        if (++campaignCards > 40)
                            throw new Exception("Campaign quest cards did not settle.");
                        lastCardPress = EditorApplication.timeSinceStartup;
                        // The fresh-profile tutorial advances into a fight-only
                        // movement action even after SkipTutorial moved to Map.
                        // Retire that fixture card without executing its quest
                        // callback; the acceptance target is the dojo map button.
                        DialogsManager.ELEBLBJKDBI().StopDialog(story);
                        UnityEngine.Object.Destroy(story.gameObject);
                        Debug.Log(Prefix + "dismissed tutorial fixture card " + campaignCards);
                    }
                    return;
                }
                var info = MapButtonController.ELEBLBJKDBI().MEPCBPIJLGB()
                    .SingleOrDefault(value => value.Name == "de128.dojo_changer");
                if (info == null) return;
                if (info.NHKMCLPOMFK != "de128:sprites/dojo_changer/credits" ||
                    info.AnchorMinX != 0f || info.AnchorMaxX != 0f ||
                    info.BIJFFONMDBC.x != 240f || info.BIJFFONMDBC.y != -650f ||
                    info.EDMILHNJFAA() != MapButtonInfo.HNEJAKIGDBA.Both)
                    throw new Exception("Native map-button presentation differs from the visible DE128 placement.");
                if (!migrationChecked)
                {
                    var buttons = MapButtonController.ELEBLBJKDBI();
                    buttons.GKIOOABOBFL(ArchivedButton(info));
                    buttons.GKIOOABOBFL(info);
                    buttons.GKIOOABOBFL(info);
                    var saved = typeof(MapButtonController).GetField("_node", Hidden)
                        .GetValue(buttons) as XmlNode;
                    var entries = saved?.SelectNodes("Button[@Name='de128.dojo_changer']");
                    if (buttons.MEPCBPIJLGB().Count(value => value.Name == info.Name) != 1 ||
                        entries == null || entries.Count != 1 ||
                        entries[0].Attributes?["X"]?.Value != "240" ||
                        entries[0].Attributes?["Y"]?.Value != "-650")
                        throw new Exception("A previously saved off-screen button was not replaced in the profile.");
                    migrationChecked = true;
                    return;
                }
                var button = UnityEngine.Object.FindObjectsOfType<MapButton>()
                    .SingleOrDefault(value => value.get_MapButtonInfo() == info);
                if (button == null) return;
                var image = typeof(MapButton).GetField("_image", Hidden).GetValue(button) as ResolutionImageLE;
                if (image == null || image.sprite == null || image.sprite.texture == null)
                    throw new Exception("The installed map button did not render its DE128-owned sprite.");
                var corners = new Vector3[4];
                ((RectTransform)button.transform).GetWorldCorners(corners);
                var canvas = button.GetComponentInParent<Canvas>();
                var camera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
                    ? canvas.worldCamera : null;
                var lower = RectTransformUtility.WorldToScreenPoint(camera, corners[0]);
                var upper = RectTransformUtility.WorldToScreenPoint(camera, corners[2]);
                Debug.Log(Prefix + "button screen rect " + lower + " .. " + upper +
                    " of " + Screen.width + "x" + Screen.height);
                if (lower.x < 0 || upper.x > Screen.width || lower.y < 0 || upper.y > Screen.height)
                    throw new Exception("The dojo map button is clipped by the screen despite loading its sprite.");
                if (upper.y > Screen.height * 0.18f || upper.x > Screen.width * 0.2f)
                    throw new Exception("The dojo map button is outside the bottom-left map slot.");
                var events = EventSystem.current;
                if (events == null) throw new Exception("The map has no EventSystem for button input.");
                var pointer = new PointerEventData(events) { position = (lower + upper) * 0.5f };
                var hits = new List<RaycastResult>();
                events.RaycastAll(pointer, hits);
                if (hits.Count == 0 || hits[0].gameObject.GetComponentInParent<MapButton>() != button)
                    throw new Exception("The bottom-left dojo button is covered or cannot receive pointer input.");
                ExecuteEvents.ExecuteHierarchy(hits[0].gameObject, pointer, ExecuteEvents.pointerClickHandler);
                clicked = true;
                clickedAt = EditorApplication.timeSinceStartup;
                Debug.Log(Prefix + "native map button clicked");
                return;
            }
            if (!selected)
            {
                var coordinator = UnityEngine.Object.FindObjectOfType<ModUiCoordinator>();
                var surface = coordinator == null ? null : coordinator.Foreground;
                if (surface == null || surface.IsClosed)
                {
                    if (EditorApplication.timeSinceStartup - clickedAt > 20)
                    {
                        var bridge = UnityEngine.Object.FindObjectOfType<ModUiGameBridge>();
                        var entries = coordinator == null ? null :
                            typeof(ModUiCoordinator).GetField("entries", Hidden).GetValue(coordinator) as System.Collections.IDictionary;
                        var gate = NativeBlocked();
                        var bus = typeof(ModRuntime).GetField("StoryEvents",
                            BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null) as ModStoryEvents;
                        throw new Exception("Selector unavailable after native click: bridge=" + (bridge != null) +
                            " coordinator=" + (coordinator != null) + " entries=" + (entries?.Count ?? -1) +
                            " nativeBlocked=" + gate + " subscribers=" +
                            (bus != null && bus.HasSubscribers(ModStoryEventKind.MapButton)));
                    }
                    return;
                }
                if (surface.Id != "dojo_changer" || surface.Mount != ModUiMount.Modal ||
                    surface.WidgetCount != 76 || surface.Root.Style.Frame != "scroll" ||
                    surface.Read("choice_2").Text != "" ||
                    surface.Read("preview_2").Sprite?.ToString() !=
                        "de128:sprites/dojo_changer/new_year_24_china_dojo")
                    throw new Exception("The mounted dojo selector is not the captioned medallion scroll gallery.");
                var view = UnityEngine.Object.FindObjectOfType<ModUiView>();
                var scroll = view == null ? null : view.GetComponentInChildren<SFScrollRect>();
                var artwork = view == null ? null : view.GetComponentsInChildren<Image>(true)
                    .SingleOrDefault(value => value.name == "preview_2");
                var frame = view == null ? null : view.GetComponentsInChildren<Image>(true)
                    .FirstOrDefault(value => value.name == "Upper roll center");
                if (scroll == null || scroll.get_verticalScrollbar() == null ||
                    scroll.get_content().rect.height <= scroll.get_viewport().rect.height ||
                    artwork == null || artwork.sprite == null || artwork.sprite.texture == null ||
                    frame == null || frame.sprite == null || frame.sprite.name != "CommonScrolls.Roll_center")
                    throw new Exception("The dojo gallery is missing native scroll behavior, preview art or rolled frame.");
                Canvas.ForceUpdateCanvases();
                float top = scroll.get_content().anchoredPosition.y;
                scroll.set_verticalNormalizedPosition(0);
                Canvas.ForceUpdateCanvases();
                if (Mathf.Abs(scroll.get_content().anchoredPosition.y - top) < 100f)
                    throw new Exception("The dojo preview grid cannot scroll through all ten images.");
                scroll.set_verticalNormalizedPosition(1);
                Canvas.ForceUpdateCanvases();
                var artCorners = new Vector3[4];
                artwork.rectTransform.GetWorldCorners(artCorners);
                var artCanvas = artwork.GetComponentInParent<Canvas>();
                var artCamera = artCanvas != null && artCanvas.renderMode != RenderMode.ScreenSpaceOverlay
                    ? artCanvas.worldCamera : null;
                var artLower = RectTransformUtility.WorldToScreenPoint(artCamera, artCorners[0]);
                var artUpper = RectTransformUtility.WorldToScreenPoint(artCamera, artCorners[2]);
                var artPointer = new PointerEventData(EventSystem.current) { position = (artLower + artUpper) * .5f };
                var artHits = new List<RaycastResult>();
                EventSystem.current.RaycastAll(artPointer, artHits);
                if (artHits.Count == 0 || artHits[0].gameObject.GetComponentInParent<Button>()?.name != "choice_2")
                    throw new Exception("The Chinese dojo image does not receive pointer clicks.");
                ExecuteEvents.ExecuteHierarchy(artHits[0].gameObject, artPointer, ExecuteEvents.pointerClickHandler);
                if (ModRuntime.ResolveDojoLocation("dojo") != Choice)
                    throw new Exception("Clicking the Chinese dojo did not update the profile choice.");
                selected = true;
                selectedAt = EditorApplication.timeSinceStartup;
                Debug.Log(Prefix + "Chinese dojo selected; waiting for the native profile save cycle");
                return;
            }
            if (module.NMCNDOPKFJD() != ScreenType.ModuleDojo ||
                EditorApplication.timeSinceStartup - selectedAt < 2) return;
            var fight = Fight.GetCurrentFight();
            if (fight != null)
            {
                var location = typeof(Fight).GetField("_location", Hidden).GetValue(fight) as Location;
                if (location != null && location.name != Choice)
                    throw new Exception("The dojo fight loaded a different location: " + location.name);
            }
            if (!legacyStored)
            {
                var savedButton = MapButtonController.ELEBLBJKDBI().MEPCBPIJLGB()
                    .Single(value => value.Name == "de128.dojo_changer");
                MapButtonController.ELEBLBJKDBI().GKIOOABOBFL(ArchivedButton(savedButton));
                legacyStored = true;
                legacyStoredAt = EditorApplication.timeSinceStartup;
                return;
            }
            if (EditorApplication.timeSinceStartup - legacyStoredAt < 2) return;
            Debug.Log(Prefix + "PASS select: native map button, mounted UI, profile preference and dojo transition.");
            Finish(0);
        }
        catch (Exception error) { Debug.LogError(Prefix + "FAIL: " + error); Finish(1); }
    }

    static void Finish(int code)
    {
        EditorApplication.update -= Update;
        SessionState.SetBool(Active, false);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(code);
    }

    static bool NativeBlocked() => (bool)typeof(ModUiGameBridge).GetProperty("NativeInputBlocked",
        BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

    static MapButtonInfo ArchivedButton(MapButtonInfo current) =>
        new MapButtonInfo(current.Name, current.NHKMCLPOMFK, "",
            new Vector2(-3095f, -645f), anchorMinX: 1f, anchorMaxX: 1f,
            BFBFKHHANJG: "Both");
}
