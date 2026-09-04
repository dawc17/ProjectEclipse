using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class EclipsePlayerBuild
{
    private const string WindowsOutputVariable = "ECLIPSE_WINDOWS_OUTPUT";
    private const string AndroidOutputVariable = "ECLIPSE_ANDROID_OUTPUT";

    [MenuItem("SF2/Build/Windows x86_64")]
    public static void BuildWindows()
    {
        BuildPlayer(
            BuildTarget.StandaloneWindows64,
            ResolveOutputPath(WindowsOutputVariable, "Builds/Windows/Eclipse.exe"));
    }

    [MenuItem("SF2/Build/Android ARM64 APK")]
    public static void BuildAndroid()
    {
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        EditorUserBuildSettings.buildAppBundle = false;

        BuildPlayer(
            BuildTarget.Android,
            ResolveOutputPath(AndroidOutputVariable, "Builds/Android/Eclipse.apk"));
    }

    private static void BuildPlayer(BuildTarget target, string outputPath)
    {
        if (EditorUserBuildSettings.activeBuildTarget != target)
            throw new BuildFailedException("Select " + target + " in Build Settings first, or use " +
                "BuildScripts/BuildPlayers.ps1 (which launches Unity with an explicit -buildTarget).");

        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        if (scenes.Length == 0)
            throw new BuildFailedException("No enabled scenes are configured in Build Settings.");

        string missingScene = scenes.FirstOrDefault(scene => !File.Exists(scene));
        if (!string.IsNullOrEmpty(missingScene))
            throw new BuildFailedException("Configured build scene is missing: " + missingScene);

        string outputDirectory = Path.GetDirectoryName(outputPath);
        if (string.IsNullOrEmpty(outputDirectory))
            throw new BuildFailedException("Build output has no parent directory: " + outputPath);
        Directory.CreateDirectory(outputDirectory);

        BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = target,
            // Raw recovered Resources plus the streaming archives exceed Gradle's
            // intermediate AAR limit. Compress Unity data without dropping content.
            options = target == BuildTarget.Android ? BuildOptions.CompressWithLz4HC : BuildOptions.None
        });

        BuildSummary summary = report.summary;
        if (summary.result != BuildResult.Succeeded)
            throw new BuildFailedException(target + " build failed with " + summary.totalErrors +
                " errors and " + summary.totalWarnings + " warnings.");

        Debug.Log("[EclipseBuild] PASS: " + target + " -> " + outputPath +
            " (" + summary.totalSize + " bytes, " + summary.totalWarnings + " warnings)");
    }

    private static string ResolveOutputPath(string variableName, string defaultRelativePath)
    {
        string configuredPath = Environment.GetEnvironmentVariable(variableName);
        string path = string.IsNullOrWhiteSpace(configuredPath) ? defaultRelativePath : configuredPath;
        return Path.GetFullPath(path);
    }
}
