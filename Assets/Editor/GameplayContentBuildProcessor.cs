using System;
using System.IO;
using Eclipse.Content;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public sealed class GameplayContentBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return -1000; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.Android &&
            (PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android) != ScriptingImplementation.IL2CPP ||
             PlayerSettings.Android.targetArchitectures != AndroidArchitecture.ARM64))
            throw new BuildFailedException("Android playtests require IL2CPP and ARM64 only. Run SF2 > Configure Android ARM64 before building.");
        Package();
    }

    [MenuItem("SF2/Configure Android ARM64")]
    public static void ConfigureAndroidArm64()
    {
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        AssetDatabase.SaveAssets();
        Debug.Log("[OfflineContent] Android configured: IL2CPP, ARM64 only.");
    }

    [MenuItem("SF2/Validate Offline Services")]
    public static void ValidateOfflineServices()
    {
        int callbacks = 0;
        var request = new HTTPRequest(new Uri("https://example.invalid/offline-check"),
            (completed, response) => { callbacks++; });
        HTTPManager.EMPGOCGHMBI(request);
        if (request.FLBBFDNHJAJ() != CFGBMHKCENK.Error ||
            !(request.IEFGFKFHNMD() is NotSupportedException) || callbacks != 1)
            throw new InvalidOperationException("Offline HTTP failure/completion contract failed.");
        TextAsset content = Resources.Load<TextAsset>(GameplayContentArchive.ResourcePath);
        if (content == null || content.bytes.Length == 0)
            throw new InvalidOperationException("Packaged gameplay resource is missing.");
        Debug.Log("[OfflineContent] PASS: HTTP rejected locally with one completion callback; packaged resource loads in Unity.");
    }

    [MenuItem("SF2/Package Offline Gameplay XML")]
    public static void Package()
    {
        const string assetPath = "Assets/Resources/SF2Content/gameplay.bytes";
        string source = Path.Combine(Application.dataPath, GameplayContentArchive.EditorSourceDirectoryName);
        foreach (string required in new[] { "stages.xml", "raid_stages_default.xml", "internalSettings.xml", "list.xml", "quests.xml" })
            if (!File.Exists(Path.Combine(source, required)))
                throw new BuildFailedException("Missing gameplay XML: " + required);
        try
        {
            byte[] bytes = GameplayContentArchive.CreateArchive(source);
            Directory.CreateDirectory(Path.GetDirectoryName(assetPath));
            File.WriteAllBytes(assetPath, bytes);
            // Generated alongside the ignored resource; preserve its identity on
            // fresh checkouts without leaving an orphan .meta in source control.
            if (!File.Exists(assetPath + ".meta"))
                File.WriteAllText(assetPath + ".meta", "fileFormatVersion: 2\nguid: 061035895c6091948b6b2de752f90edb\nTextScriptImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n");
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            if (AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath) == null)
                throw new BuildFailedException("Gameplay XML resource could not be imported.");
            Debug.Log("[OfflineContent] Packaged gameplay XML: " + bytes.Length + " bytes.");
        }
        catch (Exception exception)
        {
            throw new BuildFailedException("Gameplay XML packaging failed: " + exception);
        }
    }
}
