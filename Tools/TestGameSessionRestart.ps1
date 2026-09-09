$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root ('Temp/SessionRestart-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $fixture | Out-Null
$source = [Security.SecurityElement]::Escape((Join-Path $root 'Assets/Scripts/Eclipse/UI/GameSessionRestart.cs'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType></PropertyGroup><ItemGroup><Compile Include="$source"/></ItemGroup></Project>
"@ | Set-Content "$fixture/Test.csproj"
@'
using System;
using System.Collections.Generic;
using Eclipse.UI;
public static class Calls { public static readonly List<string> Log = new(); }
namespace Eclipse.Modding { }
namespace UnityEngine {
    public enum RuntimeInitializeLoadType { SubsystemRegistration }
    public class RuntimeInitializeOnLoadMethodAttribute : Attribute { public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType t) {} }
    public static class Time { public static float timeScale; }
    public static class AudioListener { public static bool pause; }
    public static class PlayerPrefs { public static void Save() => Calls.Log.Add("prefs"); }
    public static class Debug { public static void LogException(Exception e) {} }
}
namespace Eclipse.UI { public static class TitleScreen { public static void PrepareForRestart() => Calls.Log.Add("title"); } }
public class Roster { public bool Fail; public void GGGEHAGCLGC() { Calls.Log.Add("save"); if (Fail) throw new Exception("save failed"); } }
public static class ListSF { public static Roster Current; public static Roster CCDKHLAMKKO() => Current; }
public enum ScreenType { ModulePreloader }
public static class SceneManagerSF { public static void Load(ScreenType t) => Calls.Log.Add("load"); }
public static class Program {
    static void Check(bool b, string m) { if (!b) throw new Exception(m); }
    public static void Main() {
        ListSF.Current = new Roster { Fail = true };
        Check(!GameSessionRestart.TryRestart(() => Calls.Log.Add("selection"), out var error), "Save failure allowed restart");
        Check(error.Contains("save failed") && string.Join(",", Calls.Log)=="save" && !GameSessionRestart.IsRestarting, "Failure changed selection or scene");
        Calls.Log.Clear(); ListSF.Current.Fail=false; UnityEngine.Time.timeScale=0; UnityEngine.AudioListener.pause=true;
        Check(GameSessionRestart.TryRestart(() => Calls.Log.Add("selection"), out error), error);
        Check(string.Join(",",Calls.Log)=="save,selection,prefs,title,load", "Save/preferences/title/load order incorrect");
        Check(UnityEngine.Time.timeScale==1 && !UnityEngine.AudioListener.pause, "Pause state carried into title");
        Check(!GameSessionRestart.TryRestart(null,out error) && Calls.Log.Count==5, "Double click restarted twice");
        GameSessionRestart.ArrivedAtTitle(); Calls.Log.Clear(); ListSF.Current=null;
        Check(GameSessionRestart.TryRestart(null,out error) && string.Join(",",Calls.Log)=="prefs,title,load", "First-title apply requires a profile");
        Console.WriteLine("PASS: production restart coordinator save failure, ordering, paused state, double-click guard and initial title.");
    }
}
'@ | Set-Content "$fixture/Program.cs"
dotnet run --project "$fixture/Test.csproj"
if ($LASTEXITCODE -ne 0) { throw 'Session restart tests failed.' }
