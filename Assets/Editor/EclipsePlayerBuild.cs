using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Eclipse.Content;
using Eclipse.UI;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Build.Profile;
using UnityEngine;

public static class EclipsePlayerBuild
{
    private const string WindowsOutputVariable = "ECLIPSE_WINDOWS_OUTPUT";
    private const string AndroidOutputVariable = "ECLIPSE_ANDROID_OUTPUT";
    private const string EditableXmlOutputVariable = "ECLIPSE_WINDOWS_EDITABLE_XML_OUTPUT";
    // Release version from BuildPlayers.ps1 -Version. Without it the build is an unversioned dev build.
    private const string VersionVariable = "ECLIPSE_BUILD_VERSION";

    [MenuItem("SF2/Build/Windows x86_64")]
    public static void BuildWindows()
    {
        BuildPlayer(
            BuildTarget.StandaloneWindows64,
            ResolveOutputPath(WindowsOutputVariable, "Builds/Windows/Eclipse.exe"), EclipseUnity6Workflows.Windows);
    }

    [MenuItem("SF2/Build/Windows x86_64 (Development)")]
    public static void BuildWindowsDevelopment()
    {
        BuildPlayer(BuildTarget.StandaloneWindows64,
            ResolveOutputPath("ECLIPSE_WINDOWS_DEVELOPMENT_OUTPUT", "Builds/WindowsDevelopment/Eclipse.exe"),
            EclipseUnity6Workflows.Development);
    }

    // Tester build: gameplay XML is also written loose beside the executable and
    // the player reads it from there on every launch instead of the packaged copy.
    [MenuItem("SF2/Build/Windows x86_64 (Editable XML)")]
    public static void BuildWindowsEditableXml()
    {
        string outputPath = ResolveOutputPath(EditableXmlOutputVariable, "Builds/WindowsEditableXml/Eclipse.exe");
        BuildPlayer(BuildTarget.StandaloneWindows64, outputPath, EclipseUnity6Workflows.EditableXml);
    }

    internal static void WriteEditableXml(string gameDirectory)
    {
        string source = GameplayContentArchive.NormalizeSourceRoot(
            Path.Combine(Application.dataPath, GameplayContentArchive.EditorSourceDirectoryName));
        string target = Path.Combine(gameDirectory, GameplayContentArchive.EditableDirectoryName);
        // Replace the folder wholesale so files deleted from vanillaXml do not linger.
        if (Directory.Exists(target))
        {
            if (!File.Exists(Path.Combine(target, GameplayContentArchive.EditableMarkerFileName)))
                throw new BuildFailedException("Refusing to replace " + target + ": it was not written by this build step.");
            Directory.Delete(target, true);
        }
        string[] files = GameplayContentArchive.GetSourceFiles(source);
        foreach (string file in files)
        {
            string destination = Path.Combine(target, file.Substring(source.Length));
            Directory.CreateDirectory(Path.GetDirectoryName(destination));
            File.Copy(file, destination);
        }
        File.WriteAllText(Path.Combine(target, GameplayContentArchive.EditableMarkerFileName),
            "This Eclipse build loads gameplay XML from this folder instead of its packaged copy.\r\n" +
            "Edit any file here and restart the game to apply the change.\r\n" +
            "Delete this marker file to go back to the packaged XML.\r\n" +
            "Source: Assets/" + GameplayContentArchive.EditorSourceDirectoryName + " at build time.\r\n");
        Debug.Log("[EclipseBuild] Editable XML: " + files.Length + " files -> " + target);
    }

    [MenuItem("SF2/Build/Android ARM64 APK")]
    public static void BuildAndroid()
    {
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        EditorUserBuildSettings.buildAppBundle = false;

        BuildPlayer(
            BuildTarget.Android,
            ResolveOutputPath(AndroidOutputVariable, "Builds/Android/Eclipse.apk"), EclipseUnity6Workflows.Android);
    }

    private static void BuildPlayer(BuildTarget target, string outputPath, string profileName)
    {
        var profile = AssetDatabase.LoadAssetAtPath<BuildProfile>(EclipseUnity6Workflows.ProfilePath(profileName));
        if (profile == null || BuildProfile.GetActiveBuildProfile() != profile)
            throw new BuildFailedException("Activate " + profileName + " in File > Build Profiles first, or use BuildScripts/BuildPlayers.ps1.");
        if (EditorUserBuildSettings.activeBuildTarget != target)
            throw new BuildFailedException("Select " + target + " in Build Profiles first, or use " +
                "BuildScripts/BuildPlayers.ps1 (which launches Unity with an explicit -buildTarget).");

        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        if (scenes.Length == 0)
            throw new BuildFailedException("No enabled scenes are configured in Build Profiles.");

        string missingScene = scenes.FirstOrDefault(scene => !File.Exists(scene));
        if (!string.IsNullOrEmpty(missingScene))
            throw new BuildFailedException("Configured build scene is missing: " + missingScene);

        string outputDirectory = Path.GetDirectoryName(outputPath);
        if (string.IsNullOrEmpty(outputDirectory))
            throw new BuildFailedException("Build output has no parent directory: " + outputPath);
        Directory.CreateDirectory(outputDirectory);

        string version = Environment.GetEnvironmentVariable(VersionVariable);
        if (string.IsNullOrWhiteSpace(version)) version = null;
        else if (!Regex.IsMatch(version, @"^\d{1,6}\.\d{1,6}\.\d{1,6}$"))
            throw new BuildFailedException(VersionVariable + " must be major.minor.patch: " + version);

        // Stamp the release version for this build only; ProjectSettings keeps its committed value.
        string committedVersion = PlayerSettings.bundleVersion;
        if (version != null) PlayerSettings.bundleVersion = version;
        BuildReport report;
        try
        {
            report = BuildPipeline.BuildPlayer(new BuildPlayerWithProfileOptions
            {
                buildProfile = profile,
                locationPathName = outputPath,
                // Raw recovered Resources plus the streaming archives exceed Gradle's
                // intermediate AAR limit. Compress Unity data without dropping content.
                options = target == BuildTarget.Android ? BuildOptions.CompressWithLz4HC : BuildOptions.None
            });
        }
        finally { PlayerSettings.bundleVersion = committedVersion; }

        BuildSummary summary = report.summary;
        if (summary.result != BuildResult.Succeeded)
            throw new BuildFailedException(target + " build failed with " + summary.totalErrors +
                " errors and " + summary.totalWarnings + " warnings.");
        if (target == BuildTarget.StandaloneWindows64) WriteReleaseStamp(outputPath, version);

        Debug.Log("[EclipseBuild] PASS: " + target + " -> " + outputPath +
            " (" + summary.totalSize + " bytes, " + summary.totalWarnings + " warnings)");
    }

    // The player's update check and PackageUpdate.ps1 read this file. A dev build must not
    // keep a stamp left in the output folder by an earlier release build.
    private static void WriteReleaseStamp(string outputPath, string version)
    {
        string stamp = Path.Combine(Path.GetDirectoryName(outputPath),
            Path.GetFileNameWithoutExtension(outputPath) + "_Data", ReleaseCheck.StampFileName);
        if (version != null)
        {
            File.WriteAllText(stamp, version);
            Debug.Log("[EclipseBuild] Release version " + version + " -> " + stamp);
        }
        else
        {
            if (File.Exists(stamp)) File.Delete(stamp);
            Debug.Log("[EclipseBuild] Unversioned build: the player's update check is disabled.");
        }
    }

    private static string ResolveOutputPath(string variableName, string defaultRelativePath)
    {
        string configuredPath = Environment.GetEnvironmentVariable(variableName);
        string path = string.IsNullOrWhiteSpace(configuredPath) ? defaultRelativePath : configuredPath;
        return Path.GetFullPath(path);
    }
}

// Also applies when building directly through Unity's Build Profiles window.
public sealed class EclipseEditableXmlBuildProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder => 100;
    public void OnPostprocessBuild(BuildReport report)
    {
#if ECLIPSE_EDITABLE_XML
        if (report.summary.platform == BuildTarget.StandaloneWindows64)
            EclipsePlayerBuild.WriteEditableXml(Path.GetDirectoryName(report.summary.outputPath));
#endif
    }
}
