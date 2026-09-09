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
public static class Sound { public static void FAJONFGJBPD() => Calls.Log.Add("music-stop"); public static void GKMINHHAMAK() => Calls.Log.Add("effects-stop"); }
public enum ScreenType { ModulePreloader }
public static class SceneManagerSF { public static void Load(ScreenType t) => Calls.Log.Add("load"); }
public static class Program {
    static void Check(bool b, string m) { if (!b) throw new Exception(m); }
    public static void Main() {
        ResetProbe.Test();
        ListSF.Current = new Roster { Fail = true };
        Check(!GameSessionRestart.TryRestart(() => Calls.Log.Add("selection"), out var error), "Save failure allowed restart");
        Check(error.Contains("save failed") && string.Join(",", Calls.Log)=="save" && !GameSessionRestart.IsRestarting, "Failure changed selection or scene");
        Calls.Log.Clear(); ListSF.Current.Fail=false; UnityEngine.Time.timeScale=0; UnityEngine.AudioListener.pause=true;
        Check(GameSessionRestart.TryRestart(() => Calls.Log.Add("selection"), out error), error);
        Check(string.Join(",",Calls.Log)=="save,selection,prefs,music-stop,effects-stop,title,load", "Save/preferences/title/load order incorrect");
        Check(UnityEngine.Time.timeScale==1 && !UnityEngine.AudioListener.pause, "Pause state carried into title");
        Check(!GameSessionRestart.TryRestart(null,out error) && Calls.Log.Count==7, "Double click restarted twice");
        GameSessionRestart.ArrivedAtTitle(); Calls.Log.Clear(); ListSF.Current=null;
        Check(GameSessionRestart.TryRestart(null,out error) && string.Join(",",Calls.Log)=="prefs,music-stop,effects-stop,title,load", "First-title apply requires a profile");
        Console.WriteLine("PASS: production restart coordinator save failure, ordering, paused state, double-click guard and initial title.");
    }
}
'@ | Set-Content "$fixture/Program.cs"
$listSource = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ListSF.cs')
$reset = [regex]::Match($listSource, '(?s)public static void Reset\(\)\s*\{.*?\n\t\}').Value
if (!$reset) { throw 'Production ListSF.Reset not found' }
$probe = @'
public class Items { }
public static class QuestsManager { public static void Reset() {} }
public class GlobalTimer {
    static readonly GlobalTimer Timer = new();
    public static GlobalTimer get_Instance() => Timer;
    public event Action<object> Tick;
    public void removeEventListener(int id, Action<object> callback) { Tick -= callback; }
    public void Fire() => Tick?.Invoke(null);
}
public class ResetProbe {
    static ResetProbe _instance;
    static object ANEHEDFAPCH;
    static Items _items;
    static int callbacks;
    void ILFBDHDMHPD(object data) { if (ANEHEDFAPCH == null) throw new Exception("Stale timer callback after roster reset"); callbacks++; }
    RESET_METHOD
    public static void Test() {
        int unrelated = 0;
        GlobalTimer.get_Instance().Tick += _ => unrelated++;
        for (int i=0; i<3; i++) {
            _instance = new ResetProbe(); ANEHEDFAPCH = new object();
            GlobalTimer.get_Instance().Tick += _instance.ILFBDHDMHPD;
            GlobalTimer.get_Instance().Fire();
            Reset(); Reset(); GlobalTimer.get_Instance().Fire();
        }
        if (callbacks != 3 || unrelated != 6) throw new Exception("Timer cleanup leaked callbacks or removed other listeners");
        Console.WriteLine("PASS: production ListSF.Reset detaches old profile timer across repeated sessions; other listeners survive.");
    }
}
'@
$probe.Replace('RESET_METHOD', $reset) | Add-Content "$fixture/Program.cs"
dotnet run --project "$fixture/Test.csproj"
if ($LASTEXITCODE -ne 0) { throw 'Session restart tests failed.' }
