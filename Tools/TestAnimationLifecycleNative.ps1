$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$source = Get-Content -Raw -Encoding UTF8 (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Fight.cs')
$start = $source.IndexOf('    private readonly Queue<(int Round, ModAnimationLifecycleEvent Player,')
$end = $source.IndexOf('    private void DispatchEclipseOpponent(', $start)
if ($start -lt 0 -or $end -le $start) { throw 'Native lifecycle methods not found.' }
$methods = $source.Substring($start, $end - $start)
foreach ($kind in @('Start','End')) {
    $hook = $source.IndexOf('public void OnAnimation' + $kind + '(object data)')
    $native = $source.IndexOf('PerkEvent.KNKIIEPDCPN.EVENT_ANIMATION_' + $kind.ToUpperInvariant() + ', true);', $hook)
    $notify = $source.IndexOf('NotifyEclipseAnimation(oJDOHGBGPFK.SourceModel, value, ModEffectEvent.Animation' + $kind + ');', $native)
    if ($hook -lt 0 -or $native -le $hook -or $notify -le $native -or ($notify-$native) -gt 160) {
        throw 'Lifecycle hook must immediately follow native perk notification.'
    }
}
$fixture = Join-Path $root ('Temp/AnimationLifecycle-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$code = @'
using System;
using System.Collections.Generic;
using Eclipse.Modding;
namespace UnityEngine { public static class Debug { public static int Warnings; public static void LogWarning(object message) { Warnings++; } } }
public class Model { }
public class InfoAnimation { public string Name; }
public static class ModRuntime { public static Subscription Scripts = new Subscription(); }
public class Subscription { public bool Enabled=true; public bool HasHandlers(ModEffectEvent type)=>Enabled; }
public class Round { public bool processing=true; public int round=1; }
public class Harness {
    bool IsLocalVersus, _eclipseFightEndDispatched, _eclipseCombatDispatching, _eclipseOpponentDispatching;
    bool _eclipseFightBeginDispatched=true;
    Model _playerModel=new Model(), CKNCPOABFBO=new Model();
    Round round=new Round(); int fightTimeInFrame=42;
    public List<string> Seen=new List<string>();
    public Action<string> During;
    void DispatchEclipseCombatEvent(ModEffectEvent kind, ModAnimationLifecycleEvent animation=null) {
        _eclipseCombatDispatching=true;
        try { Seen.Add("player:"+animation.AnimationName+":"+animation.Target+":"+animation.Frame); During?.Invoke("player"); }
        finally { _eclipseCombatDispatching=false; DrainEclipseAnimationEvents(); }
    }
    void DispatchEclipseOpponent(ModEffectEvent kind, ModAnimationLifecycleEvent animation=null) {
        _eclipseOpponentDispatching=true;
        try { Seen.Add("opponent:"+animation.AnimationName+":"+animation.Target+":"+animation.Frame); During?.Invoke("opponent"); }
        finally { _eclipseOpponentDispatching=false; DrainEclipseAnimationEvents(); }
    }
__METHODS__
    static int checks;
    static void Check(bool pass,string message) { checks++; if(!pass)throw new Exception(message); }
    public static void Main() {
        var h=new Harness(); var move=new InfoAnimation{Name="cast"};
        h.During=side=> { if(side=="player" && h.Seen.Count==1) {
            h.NotifyEclipseAnimation(h.CKNCPOABFBO,new InfoAnimation{Name="hit"},ModEffectEvent.AnimationStart);
            h.NotifyEclipseAnimation(new Model(),move,ModEffectEvent.AnimationEnd);
            move.Name="mutated";h.fightTimeInFrame=43;
        }};
        h.NotifyEclipseAnimation(h._playerModel,move,ModEffectEvent.AnimationStart);
        Check(string.Join("|",h.Seen)=="player:cast:self:42|opponent:cast:opponent:42|player:hit:opponent:42|opponent:hit:self:42|player:cast:other:42|opponent:cast:other:42","Reentrant FIFO, actor perspective or event snapshot lost");
        Check(!h._drainingEclipseAnimationEvents && h._eclipseAnimationEvents.Count==0,"Queue did not settle");
        foreach(var guard in new Action<Harness>[] { x=>x.IsLocalVersus=true,x=>x.round.processing=false,x=>x._eclipseFightBeginDispatched=false,x=>x._eclipseFightEndDispatched=true }) {
            h=new Harness();guard(h);h.NotifyEclipseAnimation(h._playerModel,move,ModEffectEvent.AnimationStart);
            Check(h.Seen.Count==0 && h._eclipseAnimationEvents.Count==0,"Inactive/local lifecycle delivered");
        }
        h=new Harness();ModRuntime.Scripts.Enabled=false;h.NotifyEclipseAnimation(h._playerModel,move,ModEffectEvent.AnimationStart);
        Check(h.Seen.Count==0,"Unsubscribed event delivered");ModRuntime.Scripts.Enabled=true;
        h._eclipseCombatDispatching=true;h.NotifyEclipseAnimation(h._playerModel,move,ModEffectEvent.AnimationStart);
        Check(h.Seen.Count==0 && h._eclipseAnimationEvents.Count==1,"Nested event lost or delivered recursively");
        h.round.round++;h._eclipseCombatDispatching=false;h.DrainEclipseAnimationEvents();
        Check(h.Seen.Count==0 && h._eclipseAnimationEvents.Count==0,"Old round event escaped");
        h=new Harness();h.During=side=>h.round.processing=false;
        h.NotifyEclipseAnimation(h._playerModel,move,ModEffectEvent.AnimationEnd);
        Check(h.Seen.Count==1,"Opponent received event after round ended");
        h=new Harness();h._eclipseCombatDispatching=true;
        for(int i=0;i<257;i++)h.NotifyEclipseAnimation(h._playerModel,move,ModEffectEvent.AnimationStart);
        Check(h._eclipseAnimationEvents.Count==256 && UnityEngine.Debug.Warnings==1,"Queue is unbounded");
        h=new Harness();h.During=side=>{if(side=="player")h.NotifyEclipseAnimation(h._playerModel,move,ModEffectEvent.AnimationStart);};
        h.NotifyEclipseAnimation(h._playerModel,move,ModEffectEvent.AnimationStart);
        Check(h.Seen.Count==512 && h._eclipseAnimationEvents.Count==0 && UnityEngine.Debug.Warnings==2,"Cascade did not stop at documented limit");
        Console.WriteLine("PASS: "+checks+" production lifecycle dispatch checks; native hook ordering checked. No Unity playtest.");
    }
}
'@
$code.Replace('__METHODS__', $methods) | Set-Content -Encoding UTF8 (Join-Path $fixture 'Program.cs')
# Compile production runtime DTOs and the extracted production dispatch, not a duplicated algorithm.
$runtime = [Security.SecurityElement]::Escape((Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/*.cs'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup><Compile Include="$runtime" /></ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 (Join-Path $fixture 'Lifecycle.csproj')
dotnet run --project (Join-Path $fixture 'Lifecycle.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Native lifecycle fixture failed.' }
