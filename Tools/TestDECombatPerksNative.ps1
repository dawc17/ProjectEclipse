$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root 'Temp/DECombatPerksNative'
New-Item -ItemType Directory -Force $fixture | Out-Null

function Extract-Block([string]$source, [string]$signature) {
    $start = $source.IndexOf($signature, [StringComparison]::Ordinal)
    if ($start -lt 0) { throw "Cannot find production block: $signature" }
    $lineStart = $source.LastIndexOf("`n", $start); if ($lineStart -lt 0) { $lineStart = 0 } else { $lineStart++ }
    $brace = $source.IndexOf('{', $start); if ($brace -lt 0) { throw "Cannot find opening brace: $signature" }
    $depth = 0
    for ($i = $brace; $i -lt $source.Length; $i++) {
        if ($source[$i] -eq '{') { $depth++ }
        elseif ($source[$i] -eq '}') { $depth--; if ($depth -eq 0) { return $source.Substring($lineStart, $i - $lineStart + 1) } }
    }
    throw "Cannot find closing brace: $signature"
}

$fight = Get-Content -Raw -Encoding UTF8 (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Fight.cs')
$runtime = Get-Content -Raw -Encoding UTF8 (Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/ModScripting.cs')

# Exact native ordering is part of the contract: recovered perk processing first,
# then Eclipse phase dispatch, and PostHit remains before the later DamageDealing seam.
$postCrit = Extract-Block $fight 'public void OnModelPostCrit(Model.EventModel EGHPHELLOGO)'
$postHit = Extract-Block $fight 'public void OnModelHit(Model.EventModel EGHPHELLOGO)'
if ($postCrit.IndexOf('EVENT_HIT_POSTCRIT') -lt 0 -or
    $postCrit.IndexOf('DispatchEclipseHitPhase') -le $postCrit.IndexOf('EVENT_HIT_POSTCRIT')) {
    throw 'HitPostCrit callback must follow recovered EVENT_HIT_POSTCRIT processing.'
}
if ($postHit.IndexOf('EVENT_POST_HIT') -lt 0 -or
    $postHit.IndexOf('DispatchEclipseHitPhase') -le $postHit.IndexOf('EVENT_POST_HIT')) {
    throw 'PostHit callback must follow recovered EVENT_POST_HIT processing.'
}
$nativeHit = $fight.IndexOf('DispatchEclipseHitPhase(EGHPHELLOGO, gHHCDAFIKJE, ModEffectEvent.PostHit)')
$laterDamage = $fight.IndexOf('DispatchEclipseCombatEvent(ModEffectEvent.DamageDealing', $nativeHit)
if ($nativeHit -lt 0 -or $laterDamage -le $nativeHit) { throw 'PostHit callback moved after the existing outgoing-damage seam.' }

$dispatch = Extract-Block $fight 'private void DispatchEclipseHitPhase(Model.EventModel eventModel, Model.StrikeResult strike, ModEffectEvent effectEvent)'
$show = Extract-Block $fight 'private bool TryShowEclipseStatusIcon(Model model, object key, AssetId sprite, int frames, int stacks, out string error)'
$clear = Extract-Block $fight 'private bool TryClearEclipseStatusIcon(Model model, object key, out string error)'
$update = Extract-Block $fight 'private void UpdateEclipseStatusIcons()'
$eventEnum = Extract-Block $runtime 'public enum ModEffectEvent'
$hitEvent = Extract-Block $runtime 'public sealed class ModHitEvent'
$incomingHit = Extract-Block $runtime 'public sealed class ModIncomingHit'

$source = @"
using System;
using System.Collections.Generic;
using System.Linq;
using Eclipse.Modding;
namespace Eclipse.Modding {
$eventEnum
$hitEvent
$incomingHit
public readonly struct AssetId {
    public string Namespace { get; } public string Path { get; }
    private AssetId(string ns,string path){Namespace=ns;Path=path;}
    public static AssetId Parse(string value){int i=value.IndexOf(':');return new AssetId(value.Substring(0,i),value.Substring(i+1));}
    public override string ToString()=>Namespace+":"+Path;
}
}
public sealed class InfoAnimation {
    private readonly HashSet<string> _tags; public InfoAnimation(params string[] tags){_tags=new HashSet<string>(tags);}
    public bool CNPFHBMGDFP(string name)=>_tags.Contains(name);
}
public sealed class Model {
    public string Name;
    public sealed class EventModel { public Model KJDFJPBIGJC; public Model GAIBPAGPEGK; }
    public sealed class StrikeResult { public Model GAIBPAGPEGK; public InfoAnimation PBPDKJNKFCJ; public float EEDJBBOCFNL; public bool DFOHNJEBDED; public bool DNGKOMPMPCD; }
}
public static class PerksStage { public sealed class ActionPerk { public Model KJDFJPBIGJC; public Model BIKLKJMNGKP; public string NHKMCLPOMFK=""; public bool FLNCPBKBJBL; public int KGNDJOLBBJF; public int FLNLMIHEDCI; public int EclipseStackCount; } }
public static class ModRuntime { public static object Scripts=new object(); }
public sealed class FightHarness {
    public sealed class Seen { public string Side; public Eclipse.Modding.ModEffectEvent Type; public Eclipse.Modding.ModIncomingHit Hit; }
    public readonly List<Seen> Events=new List<Seen>();
    public Model Player { get=>_playerModel; set=>_playerModel=value; } public Model Opponent { get=>CKNCPOABFBO; set=>CKNCPOABFBO=value; }
    public bool LocalVersus { get=>IsLocalVersus; set=>IsLocalVersus=value; }
    private bool IsLocalVersus; private bool _eclipseFightBeginDispatched=true; private Model _playerModel; private Model CKNCPOABFBO; private int fightTimeInFrame;
    public bool BeginDispatched { get=>_eclipseFightBeginDispatched; set=>_eclipseFightBeginDispatched=value; }
    public int VisibleAdds,VisibleRemoves;
    private sealed class EclipseStatusIcon { public PerksStage.ActionPerk Action; public int ExpiresAt; }
    private readonly Dictionary<(Model,object),EclipseStatusIcon> _eclipseStatusIcons=new Dictionary<(Model,object),EclipseStatusIcon>();
    private void DispatchEclipseCombatEvent(Eclipse.Modding.ModEffectEvent type,Eclipse.Modding.ModDamageEvent ignored=null,Eclipse.Modding.ModIncomingHit hit=null,object activity=null){Events.Add(new Seen{Side="player",Type=type,Hit=hit});}
    private void DispatchEclipseOpponent(Eclipse.Modding.ModEffectEvent type,Eclipse.Modding.ModDamageEvent ignored=null,Eclipse.Modding.ModIncomingHit hit=null,object activity=null){Events.Add(new Seen{Side="opponent",Type=type,Hit=hit});}
    private void CKCCBJKIGIO(Model model,PerksStage.ActionPerk action,bool remove){if(remove)VisibleRemoves++;else VisibleAdds++;}
$dispatch
$show
$clear
$update
    public void Hit(Model.EventModel e,Model.StrikeResult s,Eclipse.Modding.ModEffectEvent type)=>DispatchEclipseHitPhase(e,s,type);
    public bool Show(Model m,object key,Eclipse.Modding.AssetId sprite,int frames,int stacks,out string error)=>TryShowEclipseStatusIcon(m,key,sprite,frames,stacks,out error);
    public bool Clear(Model m,object key,out string error)=>TryClearEclipseStatusIcon(m,key,out error);
    public void Advance(int frames){for(int i=0;i<frames;i++){fightTimeInFrame++;UpdateEclipseStatusIcons();}}
    public bool HasIcon(Model m,object key)=>_eclipseStatusIcons.ContainsKey((m,key));
    public PerksStage.ActionPerk IconAction(Model m,object key)=>_eclipseStatusIcons.TryGetValue((m,key),out var value)?value.Action:null;
}
"@

# ModDamageEvent is only a dispatch-parameter type in the extracted helper fixture.
$source = $source.Replace('namespace Eclipse.Modding {', 'namespace Eclipse.Modding { public sealed class ModDamageEvent {}')
$source | Set-Content -Encoding UTF8 (Join-Path $fixture 'ProductionExtract.cs')
Copy-Item -LiteralPath (Join-Path $root 'Tools/DECombatPerksNativeTests.cs') -Destination (Join-Path $fixture 'Program.cs') -Force
'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><EnableDefaultCompileItems>true</EnableDefaultCompileItems><Nullable>disable</Nullable><LangVersion>latest</LangVersion></PropertyGroup></Project>' | Set-Content -Encoding UTF8 (Join-Path $fixture 'DECombatPerksNative.csproj')
dotnet run --project (Join-Path $fixture 'DECombatPerksNative.csproj') -nologo --verbosity quiet
if ($LASTEXITCODE -ne 0) { throw "DE combat perks native fixture failed: $LASTEXITCODE" }

# Reentrancy/local-versus protections remain owned by the actual dispatch methods.
$playerDispatch = Extract-Block $fight 'private void DispatchEclipseCombatEvent(ModEffectEvent effectEvent = ModEffectEvent.FightBegin'
$opponentDispatch = Extract-Block $fight 'private void DispatchEclipseOpponent(ModEffectEvent effectEvent'
if ($playerDispatch -notmatch 'if \(IsLocalVersus\) return;' -or $playerDispatch -notmatch '_eclipseCombatDispatching \|\| _eclipseOpponentDispatching' -or
    $opponentDispatch -notmatch 'if \(IsLocalVersus\) return;' -or $opponentDispatch -notmatch '_eclipseOpponentDispatching \|\| _eclipseCombatDispatching') {
    throw 'Native scripted hit dispatch lost local-versus/reentrancy guards.'
}
if ($fight -notmatch 'EclipseFighterOperations[^\r\n]*IModFighterStatusIcons' -or
    $fight -notmatch 'TryShowStatusIcon\(object key, AssetId sprite, int frames, int stacks' -or
    $fight -notmatch 'ClearEclipseStatusIcons\(\);') {
    throw 'Native fighter status-icon bridge or fight-end cleanup is missing.'
}
$perkStage = Get-Content -Raw -Encoding UTF8 (Join-Path $root 'Assets/Scripts/Assembly-CSharp/PerksStage.cs')
$activePerk = Get-Content -Raw -Encoding UTF8 (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Nekki/SF2/GUI/Fight/ActivePerkItem.cs')
if ($perkStage -notmatch 'public int EclipseStackCount;' -or
    $perkStage -notmatch 'EclipseStackCount = IBODMPMJELJ\.EclipseStackCount;' -or
    $activePerk -notmatch 'SetEclipseStackCount\(IBODMPMJELJ\.EclipseStackCount\);' -or
    $activePerk -notmatch 'void SetEclipseStackCount\(int count\)' -or
    $activePerk -notmatch 'count <= 0' -or
    $activePerk -notmatch 'LabelAlias' -or
    $activePerk -notmatch 'set_text\(count\.ToString\(System\.Globalization\.CultureInfo\.InvariantCulture\)\)') {
    throw 'Authored status-icon stack badge is not connected to the native ActivePerkItem consumer.'
}
Write-Host 'PASS: exact production hit-phase ordering and dispatch guards.'
