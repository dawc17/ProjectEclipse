$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root ('Temp/CombatActivity-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force -Path $fixture | Out-Null
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ComboCounter.cs') -Destination $fixture
@'
using System;
using System.Collections.Generic;
public class EventDispatcher<T> {
    public readonly List<object> Events = new List<object>();
    public void CallEvent(int type,object data){Events.Add(data);}
}
public static class GameUtils {
    public static int KCBHAMHLGBC()=>2;
    public static int NPDOLGNNINO()=>3;
}
public static class Program {
    static int checks;
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static void Main(){
        var combo=new ComboCounter();
        combo.INNGMENHNEL();combo.INNGMENHNEL();
        Check(combo.Events.Count==0 && combo.NPDOLGNNINO()==0,"Subthreshold hits emitted a combo");
        combo.INNGMENHNEL();
        Check(combo.Events.Count==1 && (int)combo.Events[0]==3 && combo.CLPDEPPPJFE()==3,"Threshold combo missing");
        combo.INNGMENHNEL();
        Check(combo.Events.Count==2 && (int)combo.Events[1]==4,"Combo increment missing");
        combo.HHHDLDIHKBJ();combo.HHHDLDIHKBJ();
        Check(combo.NPDOLGNNINO()==4,"Combo expired too early");
        combo.HHHDLDIHKBJ();
        Check(combo.Events.Count==3 && (int)combo.Events[2]==0 && combo.CLPDEPPPJFE()==4,"Expiry lost completed combo");
        combo.HHHDLDIHKBJ();Check(combo.Events.Count==3,"Expiry emitted twice");
        combo.INNGMENHNEL();combo.Reset();Check(combo.Events.Count==3,"Direct reset unexpectedly notifies");
        Console.WriteLine("PASS: "+checks+" production ComboCounter threshold/increment/expiry/reset checks.");
    }
}
'@ | Set-Content -Encoding UTF8 (Join-Path $fixture 'Program.cs')
'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>' | Set-Content -Encoding UTF8 (Join-Path $fixture 'Activity.csproj')
dotnet run --project (Join-Path $fixture 'Activity.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Native combat activity fixture failed.' }
$fight = Get-Content -Raw -Encoding UTF8 (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Fight.cs')
$styleUpdate=$fight.IndexOf('fGCODGKLHED.OnStyleChanged(kNBKAELNFDD')
$styleNotify=$fight.IndexOf('ModCombatActivityEvent.StyleChange(', $styleUpdate)
$styleRule=$fight.IndexOf('CheckFightRules(FightEvent.CrazyEvent', $styleNotify)
if ($styleUpdate -lt 0 -or $styleNotify -le $styleUpdate -or $styleRule -le $styleNotify) { throw 'Style notification moved outside its documented boundary.' }
$comboNative=$fight.IndexOf('PerkEvent.KNKIIEPDCPN.EVENT_COMBO);')
$comboNotify=$fight.IndexOf('ModCombatActivityEvent.ComboChange(', $comboNative)
if ($comboNative -lt 0 -or $comboNotify -le $comboNative) { throw 'Combo notification must follow native perk bookkeeping.' }
$renderStart=$fight.IndexOf('private void RenderFight()')
$renderEnd=$fight.IndexOf('private void RenderCamera()', $renderStart)
$renderBody=$fight.Substring($renderStart,$renderEnd-$renderStart)
if ($renderBody -notmatch 'if \(round.processing\)\s*\{\s*fightTimeInFrame\+\+;' -or
    $renderBody -notmatch '_eclipseFightBeginDispatched && ModRuntime.Scripts != null &&\s*ModRuntime.Scripts.HasHandlers\(ModEffectEvent.Tick\)' -or
    $renderBody.IndexOf('DispatchEclipseCombatEvent(ModEffectEvent.Tick)') -gt $renderBody.IndexOf('EPBDEDGLHJE.Render()') -or
    $renderBody -notmatch 'if \(round.processing\) DispatchEclipseOpponent\(ModEffectEvent.Tick\)') {
    throw 'Tick dispatch must use the active clock before model updates, with subscriptions and opponent boundary recheck.'
}
if ($fight -notmatch '(?s)private void MOFKFJCIBGC\(object data\).*?isRenderFight = false;' -or
    $fight -notmatch '(?s)public void Render\(\)\s*\{\s*if \(isRenderFight\)\s*\{\s*RenderFight\(\);') {
    throw 'Native pause must prevent simulation ticks.'
}
Write-Host 'PASS: native activity/tick dispatch source-order and pause guards (not a Unity playtest).'
