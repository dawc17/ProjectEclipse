$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root ('Temp/GameplayArchiveCache-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$source = [Security.SecurityElement]::Escape((Join-Path $root 'Assets/Scripts/Eclipse/Content/GameplayContentArchive.cs'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup><Compile Include="$source" /></ItemGroup></Project>
"@ | Set-Content -LiteralPath (Join-Path $fixture 'Test.csproj')
@'
using System;
using System.IO;
using System.Reflection;
using Eclipse.Content;

// Execute the production archive/cache code with controlled Unity environment
// and resource access. Extraction, hashing and all file operations are real.
namespace UnityEngine
{
    public enum RuntimePlatform { WindowsPlayer, LinuxPlayer, Android }
    public enum RuntimeInitializeLoadType { SubsystemRegistration }
    public sealed class RuntimeInitializeOnLoadMethodAttribute : Attribute
    { public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType kind) {} }
    public static class Application
    {
        public static bool isEditor;
        public static string dataPath, persistentDataPath;
        public static RuntimePlatform platform = RuntimePlatform.WindowsPlayer;
    }
    public sealed class TextAsset { public byte[] bytes; }
    public static class Resources
    {
        public static TextAsset Asset;
        public static int Loads, Unloads;
        public static T Load<T>(string path) where T : class
        {
            if (path != GameplayContentArchive.ResourcePath) throw new Exception("Wrong resource path");
            Loads++; return Asset as T;
        }
        public static void UnloadAsset(TextAsset asset) { Unloads++; }
    }
    public static class Debug { public static void LogWarning(string text) {} }
}
static class Program
{
    static int checks;
    static void Check(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static void Reset() => typeof(GameplayContentArchive).GetMethod("ResetRootCache", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
    static void Main(string[] args)
    {
        string fixture = args[0], source = Path.Combine(fixture, "Source"); Directory.CreateDirectory(source);
        File.WriteAllText(Path.Combine(source, "stages.xml"), "<Stages/>");
        byte[] archive = GameplayContentArchive.CreateArchive(source);
        UnityEngine.Application.dataPath = Path.Combine(fixture, "Player_Data");
        UnityEngine.Application.persistentDataPath = Path.Combine(fixture, "Save");
        UnityEngine.Resources.Asset = new UnityEngine.TextAsset { bytes = archive };
        Reset(); string first = GameplayContentArchive.GetXmlRoot();
        Check(File.ReadAllText(Path.Combine(first, "stages.xml")) == "<Stages/>", "Initial extraction changed");
        for (int i = 0; i < 10; i++) Check(GameplayContentArchive.GetXmlRoot() == first, "Cached directory changed");
        Check(UnityEngine.Resources.Loads == 1 && UnityEngine.Resources.Unloads == 1, "Archive resource loaded more than once per run");
        // A new Unity subsystem initialization must not inherit the previous run.
        Reset(); Check(GameplayContentArchive.GetXmlRoot() == first && UnityEngine.Resources.Loads == 2, "New run retained static archive state");
        // Missing or invalid resources must not turn a failed resolution into a cache hit.
        Reset(); UnityEngine.Resources.Asset = null;
        bool failed = false; try { GameplayContentArchive.GetXmlRoot(); } catch (InvalidDataException) { failed = true; }
        Check(failed, "Missing resource accepted");
        UnityEngine.Resources.Asset = new UnityEngine.TextAsset { bytes = archive };
        Check(GameplayContentArchive.GetXmlRoot() == first && UnityEngine.Resources.Loads == 4, "Missing resource failure prevented retry");
        Reset(); UnityEngine.Resources.Asset = new UnityEngine.TextAsset { bytes = new byte[] { 1, 2, 3 } };
        failed = false; try { GameplayContentArchive.GetXmlRoot(); } catch (InvalidDataException) { failed = true; }
        Check(failed && UnityEngine.Resources.Unloads == 4, "Invalid resource did not fail and unload");
        UnityEngine.Resources.Asset = new UnityEngine.TextAsset { bytes = archive };
        Check(GameplayContentArchive.GetXmlRoot() == first && UnityEngine.Resources.Loads == 6, "Extraction failure prevented retry");
        // Editor source directories bypass the player cache, including a changed dataPath.
        UnityEngine.Application.isEditor = true;
        Check(GameplayContentArchive.GetXmlRoot() == Path.Combine(fixture, "Player_Data", "vanillaXml"), "Editor used player extraction");
        UnityEngine.Application.dataPath = Path.Combine(fixture, "Editor_Data");
        Check(GameplayContentArchive.GetXmlRoot() == Path.Combine(fixture, "Editor_Data", "vanillaXml") && UnityEngine.Resources.Loads == 6, "Editor loaded archive or retained stale source root");
        UnityEngine.Application.isEditor = false; UnityEngine.Application.dataPath = Path.Combine(fixture, "Player_Data");
        string editable = Path.Combine(fixture, GameplayContentArchive.EditableDirectoryName); Directory.CreateDirectory(editable);
        File.WriteAllText(Path.Combine(editable, GameplayContentArchive.EditableMarkerFileName), "Fixture");
        File.WriteAllText(Path.Combine(editable, "stages.xml"), "<Edited/>");
        Reset(); Check(GameplayContentArchive.GetXmlRoot() == editable && UnityEngine.Resources.Loads == 6, "Editable player loaded packaged archive");
        File.WriteAllText(Path.Combine(editable, "stages.xml"), "<Changed/>");
        Check(File.ReadAllText(Path.Combine(GameplayContentArchive.GetXmlRoot(), "stages.xml")) == "<Changed/>", "Directory cache hid edits to loose XML");
        UnityEngine.Application.platform = UnityEngine.RuntimePlatform.Android;
        Reset(); Check(GameplayContentArchive.GetXmlRoot() == first && UnityEngine.Resources.Loads == 7, "Android accepted desktop editable XML");
        Console.WriteLine("PASS: " + checks + " production gameplay archive cache checks: one resource load per run, reset, failed-load retry, extraction, editor bypass and live loose XML.");
    }
}
'@ | Set-Content -LiteralPath (Join-Path $fixture 'Program.cs')
dotnet run --project (Join-Path $fixture 'Test.csproj') --verbosity quiet -- $fixture
if ($LASTEXITCODE -ne 0) { throw 'Gameplay archive cache checks failed.' }
