using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using Eclipse.Runtime;
using Unity.PlayMode.Editor;
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEngine;

/// <summary>Creates native Unity assets through the editor, preserving existing profiles.</summary>
public static class EclipseUnity6Workflows
{
    public const string ProfileFolder = "Assets/Settings/Build Profiles";
    public const string ScenarioFolder = "Assets/Settings/Play Mode Scenarios";
    public const string Windows = "Eclipse Windows", Development = "Eclipse Windows Development";
    public const string EditableXml = "Eclipse Windows Editable XML", Android = "Eclipse Android";
    public const string DirectScenario = "Eclipse Direct Versus", RoomsScenario = "Eclipse Rooms and Spectator";
    public const string LoaderScene = "Assets/src/GUI/Scenes/GameLoaderScene/GameLoader.unity";
    private const string WindowsPlatform = "4e3c793746204150860bf175a9a41a05";
    private const string AndroidPlatform = "b9b35072a6f44c2e863f17467ea3dc13";
    private const BindingFlags Fields = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    [MenuItem("SF2/Unity 6/Set Up Workflows")]
    public static void SetUp()
    {
        Directory.CreateDirectory(ProfileFolder);
        Directory.CreateDirectory(ScenarioFolder);
        AssetDatabase.Refresh();
        EnsurePlayerTags();
        CreateProfile(Windows, WindowsPlatform, false, false);
        CreateProfile(Development, WindowsPlatform, true, false);
        CreateProfile(EditableXml, WindowsPlatform, true, true);
        CreateProfile(Android, AndroidPlatform, false, false);
        CreateScenario(DirectScenario, new[] { EditorPlayModeContext.Host, EditorPlayModeContext.Guest });
        CreateScenario(RoomsScenario, new[] { EditorPlayModeContext.RoomHost, EditorPlayModeContext.RoomGuest, EditorPlayModeContext.Observer });
        AssetDatabase.SaveAssets();
        Debug.Log("[EclipseUnity6] Build Profiles and Multiplayer Play Mode scenarios are ready.");
    }

    public static string ProfilePath(string name) => ProfileFolder + "/" + name + ".asset";
    public static string ScenarioPath(string name) => ScenarioFolder + "/" + name + ".asset";

    private static void EnsurePlayerTags()
    {
        // Use the same native store as Unity's player-tag editor. Merge rather
        // than replacing tags belonging to other scenarios in this project.
        var type = Type.GetType("Unity.Multiplayer.PlayMode.Editor.ProjectDataStore, UnityEditor.MultiplayerModule", true);
        var store = type.GetMethod("GetMain", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
        var contains = type.GetMethod("Contains", Fields);
        var add = type.GetMethod("Add", Fields);
        foreach (var tag in new[] { EditorPlayModeContext.Host, EditorPlayModeContext.Guest, EditorPlayModeContext.RoomHost, EditorPlayModeContext.RoomGuest, EditorPlayModeContext.Observer })
        {
            if ((bool)contains.Invoke(store, new object[] { tag })) continue;
            var arguments = new object[] { tag, null };
            if (!(bool)add.Invoke(store, arguments)) throw new InvalidOperationException("Could not register player tag " + tag + ": " + arguments[1]);
        }
    }

    private static void CreateProfile(string name, string platform, bool development, bool editableXml)
    {
        if (AssetDatabase.LoadAssetAtPath<BuildProfile>(ProfilePath(name)) != null) return;
        if (!BuildProfile.GetInstalledPlatformModules().Any(info => info.platformGuid.ToString() == platform))
        {
            Debug.LogWarning("[EclipseUnity6] Install the platform module before creating " + name + ".");
            return;
        }
        var profile = BuildProfile.CreateBuildProfile(new GUID(platform), name);
        profile.overrideGlobalScenes = false; // Follow Eclipse's canonical scene list.
        profile.scriptingDefines = editableXml ? new[] { "ECLIPSE_EDITABLE_XML" } : Array.Empty<string>();
        // These are native 6.6 managed-reference properties, verified by the fixture.
        var serialized = new SerializedObject(profile);
        var settings = serialized.FindProperty("m_PlatformBuildProfile");
        settings.FindPropertyRelative("m_Development").boolValue = development;
        settings.FindPropertyRelative("m_AllowDebugging").boolValue = development;
        if (platform == AndroidPlatform)
            settings.FindPropertyRelative("m_CompressionType").intValue = Convert.ToInt32(Enum.Parse(
                typeof(EditorUserBuildSettings).Assembly.GetType("UnityEditor.Compression", true), "Lz4HC"));
        serialized.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(profile);
        string createdPath = AssetDatabase.GetAssetPath(profile);
        if (createdPath != ProfilePath(name))
        {
            string error = AssetDatabase.MoveAsset(createdPath, ProfilePath(name));
            if (!string.IsNullOrEmpty(error)) throw new IOException(error);
        }
    }

    private static object Field(object owner, string name) => owner.GetType().GetField(name, Fields)?.GetValue(owner)
        ?? throw new MissingFieldException(owner.GetType().FullName, name);
    private static void SetField(object owner, string name, object value)
    {
        var field = owner.GetType().GetField(name, Fields) ?? throw new MissingFieldException(owner.GetType().FullName, name);
        field.SetValue(owner, value);
    }

    private static void CreateScenario(string name, string[] tags)
    {
        if (AssetDatabase.LoadAssetAtPath<PlayModeScenario>(ScenarioPath(name)) != null) return;
        var loader = AssetDatabase.LoadAssetAtPath<SceneAsset>(LoaderScene);
        if (loader == null) throw new FileNotFoundException("The Eclipse loader scene is missing.", LoaderScene);
        // Unity 6.6 exposes the scenario manager publicly, but its built-in scenario
        // authoring types are internal. Keep this version-specific adapter here.
        var type = Type.GetType("Unity.Multiplayer.PlayMode.Editor.OrchestratedScenario, UnityEditor.MultiplayerModule", true);
        var scenario = (PlayModeScenario)ScriptableObject.CreateInstance(type);
        var settings = Field(scenario, "m_Settings");
        var instances = (IList)Field(settings, "m_InstanceItems");
        var main = Field(instances[0], "m_Settings");
        SetField(main, "PlayerTag", tags[0]);
        SetField(main, "InitialScene", loader);
        SetField(instances[0], "m_Settings", main); // InstanceSettings is a boxed value type.
        SetField(scenario, "m_EnableEditors", true);
        var controller = type.Assembly.GetType("Unity.Multiplayer.PlayMode.Editor.CloneEditorController", true);
        var instanceType = controller.GetNestedType("InstanceSettings", BindingFlags.Public | BindingFlags.NonPublic);
        var add = settings.GetType().GetMethods(Fields).Single(method => method.Name == "AddInstance" && method.IsGenericMethodDefinition && method.GetParameters().Length == 2);
        for (int i = 1; i < tags.Length; i++)
        {
            var client = Activator.CreateInstance(instanceType, true);
            SetField(client, "PlayerTag", tags[i]);
            SetField(client, "PlayerInstanceIndex", i);
            SetField(client, "StreamLogsToMainEditor", true);
            SetField(client, "LogsColor", i == 1 ? new Color(.3f, .8f, 1f) : new Color(1f, .7f, .3f));
            add.MakeGenericMethod(controller, instanceType).Invoke(settings, new[] { (object)tags[i], client });
        }
        SetField(scenario, "m_Settings", settings);
        AssetDatabase.CreateAsset(scenario, ScenarioPath(name));
    }

    [MenuItem("SF2/Unity 6/Select Direct Versus Scenario")]
    public static void SelectDirect()
    {
        SetUp();
        PlayModeScenarioManager.ActiveScenario = AssetDatabase.LoadAssetAtPath<PlayModeScenario>(ScenarioPath(DirectScenario));
    }

    [MenuItem("SF2/Unity 6/Select Rooms and Spectator Scenario")]
    public static void SelectRooms()
    {
        SetUp();
        PlayModeScenarioManager.ActiveScenario = AssetDatabase.LoadAssetAtPath<PlayModeScenario>(ScenarioPath(RoomsScenario));
    }
}
