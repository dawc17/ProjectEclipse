using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Eclipse.Input;
using Eclipse.Runtime;
using Unity.PlayMode.Editor;
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;

// Run in the isolated Unity project prepared by TestUnity6Workflows.ps1.
[InitializeOnLoad]
public static class ValidateUnity6Workflows
{
    private static int checks;
    private const BindingFlags Fields = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    private static void Check(bool condition, string message)
    {
        checks++;
        if (!condition) throw new Exception(message);
    }
    private static object Field(object owner, string name) => owner.GetType().GetField(name, Fields).GetValue(owner);
    private static void PrimeEdges(InputDevice device)
    {
        foreach (var control in device.allControls)
            if (control is UnityEngine.InputSystem.Controls.ButtonControl button)
            { _ = button.wasPressedThisFrame; _ = button.wasReleasedThisFrame; }
    }
    static ValidateUnity6Workflows()
    {
        if (SessionState.GetBool("Unity6Fixture.Pending", false)) EditorApplication.update += PlayTests;
    }

    public static void Run()
    {
        try
        {
            Check(Application.unityVersion == "6000.6.0f1", "Use the project's matching editor.");
            TestProfilesAndScenarios();
            SessionState.SetInt("Unity6Fixture.Checks", checks);
            SessionState.SetBool("Unity6Fixture.Pending", true);
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorApplication.EnterPlaymode();
        }
        catch (Exception error) { Debug.LogException(error); EditorApplication.Exit(1); }
    }

    public static void VerifyXmlProfileDefines()
    {
#if ECLIPSE_EDITABLE_XML
        Check(AssetDatabase.GetAssetPath(BuildProfile.GetActiveBuildProfile()) == EclipseUnity6Workflows.ProfilePath(EclipseUnity6Workflows.EditableXml), "Wrong active XML profile.");
        Debug.Log("[Unity6Workflows] PASS: active Build Profile define is compiled into the editor build postprocessor.");
#else
        Debug.LogError("[Unity6Workflows] The active XML profile did not supply ECLIPSE_EDITABLE_XML.");
        EditorApplication.Exit(1);
#endif
    }

    private static void PlayTests()
    {
        if (!EditorApplication.isPlaying || EditorApplication.isCompiling) return;
        EditorApplication.update -= PlayTests;
        SessionState.SetBool("Unity6Fixture.Pending", false);
        checks = SessionState.GetInt("Unity6Fixture.Checks", 0);
        new GameObject("Unity 6 Native Input Fixture").AddComponent<Unity6WorkflowTestRunner>();
    }

    public static IEnumerator PlayRoutine()
    {
        var input = TestInput();
        while (input.MoveNext()) yield return input.Current;
        TestUi();
        Debug.Log("[Unity6Workflows] PASS: " + checks + " native device, UI, profile and scenario checks.");
        File.WriteAllText("validation-result.txt", "PASS: " + checks);
        EditorApplication.Exit(0);
    }

    private static IEnumerator TestInput()
    {
        // A headless fixture has no focused Game View. Route only its synthetic
        // devices to the player buffer; the production project keeps normal focus rules.
        Application.runInBackground = true;
        InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
        // Remove fixture devices so installed hardware cannot influence assertions.
        var devices = new List<InputDevice>();
        foreach (var device in InputSystem.devices) devices.Add(device);
        foreach (var device in devices) InputSystem.RemoveDevice(device);
        typeof(EclipseInput).GetMethod("Reset", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
        var keyboard = InputSystem.AddDevice<Keyboard>();
        PrimeEdges(keyboard);
        yield return null;
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.O, Key.UpArrow, Key.Numpad5, Key.Semicolon));
        yield return null;
        Check(EclipseInput.GetKey(KeyCode.O) && EclipseInput.GetKeyDown(KeyCode.O), "Keyboard press was lost: mapped=" + EclipseInput.MapKey(KeyCode.O) + ", native=" + keyboard.oKey.isPressed + ", down=" + keyboard.oKey.wasPressedThisFrame + ", current=" + (Keyboard.current == keyboard));
        Check(EclipseInput.GetKey(KeyCode.UpArrow) && EclipseInput.GetKey(KeyCode.Keypad5) && EclipseInput.GetKey(KeyCode.Semicolon), "Saved key codes changed meaning.");
        Check(!EclipseInput.GetKey(KeyCode.P) && EclipseInput.anyKeyDown, "Keyboard leaked an unpressed binding.");
        InputSystem.QueueStateEvent(keyboard, new KeyboardState());
        yield return null;
        Check(!EclipseInput.GetKey(KeyCode.O) && EclipseInput.GetKeyUp(KeyCode.O), "Keyboard release was lost.");
        var mouse = InputSystem.AddDevice<Mouse>();
        PrimeEdges(mouse);
        yield return null;
        InputSystem.QueueStateEvent(mouse, new MouseState { position = new Vector2(130, 260), scroll = new Vector2(0, 120), buttons = 1 });
        yield return null;
        Check(EclipseInput.GetMouseButtonDown(0) && EclipseInput.mousePosition.x == 130 && EclipseInput.mousePosition.y == 260, "Pointer click/position changed.");
        Check(EclipseInput.mouseScrollDelta.y == 1, "Wheel units changed.");
        InputSystem.QueueStateEvent(mouse, new MouseState());
        yield return null;
        Check(EclipseInput.GetMouseButtonUp(0), "Pointer release was lost.");
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.A));
        InputSystem.QueueStateEvent(mouse, new MouseState { position = new Vector2(70, 80), scroll = new Vector2(0, 120) }.WithButton(MouseButton.Left));
        yield return null;
        Check(EclipseInput.GetKey(KeyCode.A) && EclipseInput.mousePosition.x == 70, "Disabled-device fixture did not start with active input.");
        InputSystem.DisableDevice(keyboard);
        InputSystem.DisableDevice(mouse);
        Check(!EclipseInput.GetKey(KeyCode.A) && !EclipseInput.anyKey, "Disabled keyboard/mouse leaked retained button state.");
        Check(EclipseInput.mousePosition == Vector3.zero && EclipseInput.mouseScrollDelta == Vector2.zero && !EclipseInput.GetMouseButtonDown(0), "Disabled mouse leaked retained pointer state.");
        InputSystem.EnableDevice(keyboard);
        InputSystem.EnableDevice(mouse);
        InputSystem.QueueStateEvent(keyboard, new KeyboardState());
        InputSystem.QueueStateEvent(mouse, new MouseState());
        yield return null;
        Check(!EclipseInput.IsGamepadConnected(1), "Missing pad reported connected.");
        var one = InputSystem.AddDevice<Gamepad>();
        var two = InputSystem.AddDevice<Gamepad>();
        var three = InputSystem.AddDevice<Gamepad>();
        var four = InputSystem.AddDevice<Gamepad>();
        PrimeEdges(one); PrimeEdges(two); PrimeEdges(three); PrimeEdges(four);
        yield return null;
        InputSystem.QueueStateEvent(one, new UnityEngine.InputSystem.LowLevel.GamepadState { leftStick = new Vector2(.1f, .4f), rightTrigger = .8f }.WithButton(GamepadButton.South).WithButton(GamepadButton.North).WithButton(GamepadButton.RightShoulder));
        InputSystem.QueueStateEvent(two, new UnityEngine.InputSystem.LowLevel.GamepadState().WithButton(GamepadButton.East).WithButton(GamepadButton.West));
        InputSystem.QueueStateEvent(four, new UnityEngine.InputSystem.LowLevel.GamepadState { leftStick = new Vector2(0, .9f), leftTrigger = .9f });
        yield return null;
        Check(GamePad.GetButton(GamePad.Button.A, GamePad.Player.One) && GamePad.GetButtonDown(GamePad.Button.A, GamePad.Player.One), "South/A semantic mapping failed.");
        Check(GamePad.GetButton(GamePad.Button.Y, GamePad.Player.One) && !GamePad.GetButton(GamePad.Button.X, GamePad.Player.One), "North/Y semantic mapping failed.");
        Check(GamePad.GetButton(GamePad.Button.B, GamePad.Player.Two) && GamePad.GetButton(GamePad.Button.X, GamePad.Player.Two), "P2 or west/X mapping failed.");
        Check(GamePad.GetButton(GamePad.Button.RightShoulder, GamePad.Player.One), "Saved shoulder binding changed.");
        Check(!GamePad.GetButton(GamePad.Button.A, GamePad.Player.Two), "P1 input leaked into P2.");
        Check(GamePad.GetStick(GamePad.Stick.LeftStick, GamePad.Player.One, true).y > 0, "Stick up was inverted.");
        Check(Mathf.Abs(GamePad.GetStick(GamePad.Stick.LeftStick, GamePad.Player.Any, true).y - .9f) < .001f, "Any omitted the fourth pad.");
        Check(GamePad.GetTrigger(GamePad.Trigger.LeftTrigger, GamePad.Player.Any, true) > .8f, "Any trigger omitted the fourth pad.");
        Check(EclipseInput.GetKey(KeyCode.Joystick1Button0) && !EclipseInput.GetKey(KeyCode.Joystick2Button0), "Legacy modality scan changed pad ownership.");
        InputSystem.QueueStateEvent(one, new UnityEngine.InputSystem.LowLevel.GamepadState());
        yield return null;
        Check(GamePad.GetButtonUp(GamePad.Button.A, GamePad.Player.One), "Gamepad release was lost.");
        InputSystem.RemoveDevice(one);
        Check(!EclipseInput.IsGamepadConnected(1) && EclipseInput.IsGamepadConnected(2), "Disconnect compacted player slots.");
        Check(GamePad.GetButton(GamePad.Button.B, GamePad.Player.Two), "Disconnect transferred P2 controls.");
        var replacement = InputSystem.AddDevice<Gamepad>();
        Check(EclipseInput.IsGamepadConnected(1) && GamePad.GetButton(GamePad.Button.B, GamePad.Player.Two), "Replacement pad displaced P2.");
        InputSystem.DisableDevice(two);
        Check(!EclipseInput.IsGamepadConnected(2) && !GamePad.GetButton(GamePad.Button.B, GamePad.Player.Two), "Disabled pad retained input.");
        InputSystem.EnableDevice(two);
        Check(EclipseInput.IsGamepadConnected(2), "Re-enabled pad lost its slot.");
    }

    private static void TestUi()
    {
        var host = new GameObject("Fixture EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        var events = host.GetComponent<EventSystem>();
        EclipseUiInput.Ensure(events);
        EclipseUiInput.Ensure(events);
        Check(!host.GetComponent<StandaloneInputModule>().enabled, "Recovered UI module still polls legacy axes.");
        var modules = host.GetComponents<InputSystemUIInputModule>();
        Check(modules.Length == 1 && modules[0].enabled && modules[0].point != null && modules[0].leftClick != null, "Modern UI defaults or idempotency failed.");
        UnityEngine.Object.DestroyImmediate(host);
    }

    private static void TestProfilesAndScenarios()
    {
        EclipseUnity6Workflows.SetUp();
        var guids = new Dictionary<string, string>();
        foreach (var name in new[] { EclipseUnity6Workflows.Windows, EclipseUnity6Workflows.Development, EclipseUnity6Workflows.EditableXml, EclipseUnity6Workflows.Android })
        {
            var path = EclipseUnity6Workflows.ProfilePath(name);
            var profile = AssetDatabase.LoadAssetAtPath<BuildProfile>(path);
            Check(profile != null && !profile.overrideGlobalScenes, "Profile is missing or replaced the canonical scenes: " + name);
            var serialized = new SerializedObject(profile);
            Check(serialized.FindProperty("m_BuildTarget").intValue == (int)(name == EclipseUnity6Workflows.Android ? BuildTarget.Android : BuildTarget.StandaloneWindows64), "Profile platform mismatch.");
            if (name != EclipseUnity6Workflows.Android) Check(serialized.FindProperty("m_Subtarget").intValue == (int)StandaloneBuildSubtarget.Player, "Windows profile is not a player build.");
            else Check(serialized.FindProperty("m_PlatformBuildProfile").FindPropertyRelative("m_CompressionType").intValue == Convert.ToInt32(Enum.Parse(typeof(EditorUserBuildSettings).Assembly.GetType("UnityEditor.Compression", true), "Lz4HC")), "Android profile lost content compression.");
            Check(serialized.FindProperty("m_PlatformBuildProfile").FindPropertyRelative("m_Development").boolValue == (name == EclipseUnity6Workflows.Development || name == EclipseUnity6Workflows.EditableXml), "Development setting mismatch.");
            Check(Array.IndexOf(profile.scriptingDefines, "ECLIPSE_EDITABLE_XML") >= 0 == (name == EclipseUnity6Workflows.EditableXml), "Editable XML define mismatch.");
            guids[path] = AssetDatabase.AssetPathToGUID(path);
        }
        Scenario(EclipseUnity6Workflows.DirectScenario, new[] { EditorPlayModeContext.Host, EditorPlayModeContext.Guest }, guids);
        Scenario(EclipseUnity6Workflows.RoomsScenario, new[] { EditorPlayModeContext.RoomHost, EditorPlayModeContext.RoomGuest, EditorPlayModeContext.Observer }, guids);
        var tagsType = Type.GetType("Unity.Multiplayer.PlayMode.Editor.ProjectDataStore, UnityEditor.MultiplayerModule", true);
        var tagStore = tagsType.GetMethod("GetMain", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
        var tagList = (string[])tagsType.GetMethod("GetAllPlayerTags", Fields).Invoke(tagStore, null);
        foreach (var tag in new[] { EditorPlayModeContext.Host, EditorPlayModeContext.Guest, EditorPlayModeContext.RoomHost, EditorPlayModeContext.RoomGuest, EditorPlayModeContext.Observer })
            Check(Array.IndexOf(tagList, tag) >= 0, "Scenario tag is not registered.");
        EclipseUnity6Workflows.SetUp();
        foreach (var pair in guids) Check(AssetDatabase.AssetPathToGUID(pair.Key) == pair.Value, "Set Up changed an existing GUID.");
        Check(EditorPlayModeContext.Role == null && EditorPlayModeContext.PersistentDataPath == Application.persistentDataPath, "Untagged editor data path changed.");
        var dataPath = typeof(EditorPlayModeContext).GetMethod("TaggedDataPath", BindingFlags.Static | BindingFlags.NonPublic);
        foreach (var role in new[] { EditorPlayModeContext.Host, EditorPlayModeContext.Guest })
        {
            var root = (string)dataPath.Invoke(null, new object[] { "C:/Fixture/Eclipse", role });
            foreach (var directory in new[] { "userdata", "EclipseTitlePreview" })
            {
                // These paths follow the production SF2Paths/preview spelling.
                var profile = Path.Combine(root, directory, "users.xml").Replace('\\', '/');
                Check(profile.StartsWith(root, StringComparison.Ordinal), "Tagged profile is misclassified as a bundled resource.");
            }
        }
        var previous = PlayModeScenarioManager.ActiveScenario;
        try
        {
            PlayModeScenarioManager.ActiveScenario = AssetDatabase.LoadAssetAtPath<PlayModeScenario>(EclipseUnity6Workflows.ScenarioPath(EclipseUnity6Workflows.DirectScenario));
            Check(PlayModeScenarioManager.ActiveScenario != null, "Scenario selection failed.");
        }
        finally { PlayModeScenarioManager.ActiveScenario = previous; }
    }

    private static void Scenario(string name, string[] tags, Dictionary<string, string> guids)
    {
        string path = EclipseUnity6Workflows.ScenarioPath(name);
        var scenario = AssetDatabase.LoadAssetAtPath<PlayModeScenario>(path);
        Check(scenario != null && (bool)Field(scenario, "m_EnableEditors"), "Scenario missing/disabled.");
        var instances = (IList)Field(Field(scenario, "m_Settings"), "m_InstanceItems");
        Check(instances.Count == tags.Length, "Scenario player count mismatch.");
        for (int i = 0; i < tags.Length; i++)
        {
            var settings = Field(instances[i], "m_Settings");
            Check((string)Field(settings, "PlayerTag") == tags[i], "Scenario player tag was not serialized.");
            if (i == 0) Check(AssetDatabase.GetAssetPath((UnityEngine.Object)Field(settings, "InitialScene")) == EclipseUnity6Workflows.LoaderScene, "Scenario starts in the wrong scene.");
            else Check((int)Field(settings, "PlayerInstanceIndex") == i && (bool)Field(settings, "StreamLogsToMainEditor"), "Clone slot/log streaming mismatch.");
        }
        guids[path] = AssetDatabase.AssetPathToGUID(path);
    }
}

public sealed class Unity6WorkflowTestRunner : MonoBehaviour
{
    private IEnumerator Start()
    {
        var routine = ValidateUnity6Workflows.PlayRoutine();
        while (true)
        {
            bool more; object current;
            try { more = routine.MoveNext(); current = routine.Current; }
            catch (Exception error) { Debug.LogException(error); EditorApplication.Exit(1); yield break; }
            if (!more) yield break;
            yield return current;
        }
    }
}
