# Execute the production native-to-safe AI adapter and native nominal timing formula.
# Animation storage and model services are controlled; this is not a combat playtest.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$bridge = Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModRuntimeP1D.cs')
$contracts = Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/ModScripting.cs')
$animation = Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/InfoAnimation.cs')
function Extract([string]$source, [string]$pattern) {
    $found = [regex]::Match($source, $pattern)
    if (!$found.Success) { throw "AI snapshot source not found: $pattern" }
    return $found.Value
}
$dto = @('ModAiActionTiming','ModAiActionInput','ModAiActionSnapshot','ModAnimationIntervalSnapshot','ModAnimationSnapshot') | ForEach-Object {
    Extract $contracts "(?ms)^    public sealed class $_\b.*?^    \}"
}
$adapter = Extract $bridge '(?ms)^        private static ModAiActionSnapshot AiActionSnapshot\(.*?^        \}'
$inputs = Extract $bridge '(?ms)^        private static void AppendAiInputs\(.*?^        \}'
$count = Extract $animation '(?ms)^\tpublic int PGOFHCBPLOE\(.*?^\t\}'
$duration = Extract $animation '(?ms)^\tpublic int ONLKMFOENEH\(.*?^\t\}'
$loop = Extract $animation '(?ms)^\tpublic bool NCEKKNIMHAG\(.*?^\t\}'
$enum = Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/FightCID.cs')
$playback = Extract $bridge '(?ms)^        public static ModAnimationSnapshot CaptureAnimationSnapshot\(.*?^        \}'
$controllerSource = Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ModelAnimation.cs')
$getters = @('bool NMEEPBDJHMG','int KFCNPADAMHA','InfoAnimation NNMAFFCCMHC','List<IntervalAnimation> PCKKMNHDDMP') | ForEach-Object {
    Extract $controllerSource "(?ms)^\tpublic $_\(.*?^\t\}"
}
$intervalSource = Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/IntervalAnimation.cs')
$intervalEnum = Extract $intervalSource '(?ms)^\tpublic enum NGAJJDIEDGF.*?^\t\}'
$fixture = @'
using System;
using System.Collections.Generic;
namespace AiSnapshotTestScope {
using Eclipse.Modding;
/* ENUM */
public class KeyData {
 public List<int> IGEEOAGOMEM=new List<int>(), CEPODJDDLBF=new List<int>(), HPEOJLAMIHC=new List<int>();
}
public class ConditionKeys { public KeyData FONEJOKEIEN=new KeyData(); }
public class IntervalAnimation {
 /* INTERVAL_ENUM */
 public string Name; public NGAJJDIEDGF Type;
}
public class Model {
 public ModelAnimation Controller=new ModelAnimation();
 public ModelAnimation OCPMJKIEPIG(){return Controller;}
}
public class ModelAnimation {
 public bool MDLBEBOGOGK=true; public int JMKAHNADIOI=1;
 public InfoAnimation BAOONIGFBMB=new InfoAnimation();
 public List<IntervalAnimation> KKNKJMCFIJK=new List<IntervalAnimation>();
 /* GETTERS */
}
public class InfoAnimation {
 public enum MGHNBEPCKIF { AnimationNone, AnimationMove, AnimationAttack }
 public string Name="custom"; public MGHNBEPCKIF Type=MGHNBEPCKIF.AnimationAttack;
 public int Priority=7, GOBJCKFGIPA=3, LHHAGECFIOL=12, MNHGBPOIHKG=2;
 public bool INFAGPDFGNL;
 public ConditionKeys Keys=new ConditionKeys();
 public ConditionKeys ILBCHANCOBP(){return Keys;}
 /* COUNT */
 /* DURATION */
 /* LOOP */
}
namespace Eclipse.Modding {
 public class ModContentException : Exception { public ModContentException(string message):base(message){} }
 /* DTO */
 public static class AiSnapshotFixture {
  public static ModAiActionSnapshot Snapshot(InfoAnimation action){return AiActionSnapshot(action);}
  /* ADAPTER */
  /* INPUTS */
  /* PLAYBACK */
 }
}
public static class AiSnapshotTests {
 static int checks;
 static void Check(bool condition,string message){checks++;if(!condition)throw new Exception(message);}
 static void Reject(Action action,string message){bool rejected=false;try{action();}catch(ArgumentException){rejected=true;}Check(rejected,message);}
 public static void Run(){
  var native=new InfoAnimation();native.Keys.FONEJOKEIEN.IGEEOAGOMEM.Add((int)FightCID.Kick);
  native.Keys.FONEJOKEIEN.CEPODJDDLBF.Add((int)FightCID.QuadrantBack);
  native.Keys.FONEJOKEIEN.HPEOJLAMIHC.Add((int)FightCID.Punch);
  var result=AiSnapshotFixture.Snapshot(native);
  Check(result.Name=="custom"&&result.Type=="attack"&&result.Priority==7,"Native identity metadata mismatch");
  Check(result.Timing.FirstSample==3&&result.Timing.LastSample==12&&result.Timing.MidFrames==2,"Sample bounds lost");
  Check(result.Timing.NominalFrames==native.ONLKMFOENEH()&&result.Timing.NominalFrames==30&&result.Timing.NominalSeconds==0.5,"Native nominal timing mismatch");
  Check(!result.Timing.Looped,"Non-looping clip marked looping");
  Check(result.Inputs.Count==3&&result.Inputs[0].Control=="Kick"&&result.Inputs[0].Press=="tap"&&result.Inputs[1].Control=="Back"&&result.Inputs[1].Press=="hold"&&result.Inputs[2].Control=="Punch"&&result.Inputs[2].Press=="release","Native key combination mismatch");
  native.Keys.FONEJOKEIEN.IGEEOAGOMEM.Clear();native.INFAGPDFGNL=true;native.LHHAGECFIOL=22;
  Check(result.Inputs.Count==3&&result.Timing.LastSample==12&&!result.Timing.Looped,"Native mutation changed an existing snapshot");
  Check(AiSnapshotFixture.Snapshot(native).Timing.Looped,"Native looping flag missing");
  foreach(var type in new[]{InfoAnimation.MGHNBEPCKIF.AnimationNone,InfoAnimation.MGHNBEPCKIF.AnimationMove}){
   native.Type=type;Check(AiSnapshotFixture.Snapshot(native).Type==(type==InfoAnimation.MGHNBEPCKIF.AnimationNone?"none":"move"),"Native type mapping mismatch");
  }
  string[] names={"Up","Up-Forward","Forward","Down-Forward","Down","Down-Back","Back","Up-Back","Punch","Kick","Ranged","Magic","RaidCharge","Super"};
  for(int i=0;i<names.Length;i++){
   native.Keys.FONEJOKEIEN.IGEEOAGOMEM.Clear();native.Keys.FONEJOKEIEN.IGEEOAGOMEM.Add(i+1);
   Check(AiSnapshotFixture.Snapshot(native).Inputs[0].Control==names[i],"Control mapping mismatch: "+names[i]);
  }
  native.Keys.FONEJOKEIEN.IGEEOAGOMEM.Clear();native.Keys.FONEJOKEIEN.IGEEOAGOMEM.Add(999);
  Check(AiSnapshotFixture.Snapshot(native).Inputs[0].Control=="Unknown","Unknown control leaked an unrelated enum label");
  native.Keys=null;Check(AiSnapshotFixture.Snapshot(native).Inputs.Count==0,"Missing keys did not yield empty metadata");
  foreach(int spacing in new[]{0,1,2,8}){
   native.GOBJCKFGIPA=0;native.LHHAGECFIOL=59;native.MNHGBPOIHKG=spacing;
   Check(AiSnapshotFixture.Snapshot(native).Timing.NominalFrames==native.ONLKMFOENEH(),"Nominal timing spacing mismatch");
  }
  Reject(()=>new ModAiActionTiming(-1,4,0,false),"Negative sample accepted");
  Reject(()=>new ModAiActionTiming(5,4,0,false),"Reversed sample range accepted");
  Reject(()=>new ModAiActionTiming(0,4,-1,false),"Negative spacing accepted");
  Reject(()=>new ModAiActionTiming(0,int.MaxValue,int.MaxValue,false),"Overflowing duration accepted");
  var source=new[]{new ModAiActionInput("Punch","tap")};var copied=new ModAiActionSnapshot("copied",inputs:source);
  source[0]=new ModAiActionInput("Kick","release");Check(copied.Inputs[0].Control=="Punch","Host array mutation leaked");
  bool immutable=false;try{((IList<ModAiActionInput>)copied.Inputs)[0]=source[0];}catch(NotSupportedException){immutable=true;}
  Check(immutable,"Snapshot input collection is writable");
  Reject(()=>new ModAiActionSnapshot("invalid",inputs:new ModAiActionInput[65]),"Unbounded input list accepted");
  native.Keys=new ConditionKeys();for(int i=0;i<65;i++)native.Keys.FONEJOKEIEN.IGEEOAGOMEM.Add(9);
  bool bounded=false;try{AiSnapshotFixture.Snapshot(native);}catch(ModContentException){bounded=true;}
  Check(bounded,"Adapter input list allocation was unbounded");
  var model=new Model();var active=model.Controller.KKNKJMCFIJK;
  Check(AiSnapshotFixture.CaptureAnimationSnapshot(null)==null,"Null model exposed animation");
  var attack=new IntervalAnimation{Name="blade contact",Type=IntervalAnimation.NGAJJDIEDGF.INTERVAL_ATTACK};active.Add(attack);
  var observed=AiSnapshotFixture.CaptureAnimationSnapshot(model);
  Check(observed.Name=="custom"&&observed.Type=="attack"&&observed.Facing==1&&observed.Intervals[0].Type=="attack"&&observed.Intervals[0].Name=="blade contact","Active animation metadata mismatch");
  attack.Name="changed";active.Clear();model.Controller.JMKAHNADIOI=-1;
  Check(observed.Intervals.Count==1&&observed.Intervals[0].Name=="blade contact"&&observed.Facing==1,"Native changes leaked into retained animation");
  Check(AiSnapshotFixture.CaptureAnimationSnapshot(model).Facing==-1&&AiSnapshotFixture.CaptureAnimationSnapshot(model).Intervals.Count==0,"Fresh facing/interval snapshot stale");
  string[] kinds={"none","unstable","uninterrupt","self_uninterrupt","attack","block","invulnerable","invisible"};
  for(int i=0;i<kinds.Length;i++){
   active.Clear();active.Add(new IntervalAnimation{Type=(IntervalAnimation.NGAJJDIEDGF)i});
   var mapped=AiSnapshotFixture.CaptureAnimationSnapshot(model);
   Check(mapped.Intervals[0].Type==kinds[i]&&mapped.Intervals[0].Name=="","Interval mapping mismatch: "+kinds[i]);
  }
  model.Controller.MDLBEBOGOGK=false;
  Check(AiSnapshotFixture.CaptureAnimationSnapshot(model)==null,"Stopped playback exposed stale intervals");
  model.Controller.MDLBEBOGOGK=true;model.Controller.BAOONIGFBMB=null;
  Check(AiSnapshotFixture.CaptureAnimationSnapshot(model)==null,"Missing native animation fabricated observation");
  model.Controller.BAOONIGFBMB=new InfoAnimation();active.Clear();active.Add(null);
  Check(AiSnapshotFixture.CaptureAnimationSnapshot(model)==null,"Malformed interval fabricated observation");
  active.Clear();for(int i=0;i<257;i++)active.Add(attack);
  Check(AiSnapshotFixture.CaptureAnimationSnapshot(model)==null,"Unbounded interval observation");
  active.Clear();model.Controller.JMKAHNADIOI=0;
  Check(AiSnapshotFixture.CaptureAnimationSnapshot(model)==null,"Invalid facing fabricated observation");
  var intervalCopy=new[]{new ModAnimationIntervalSnapshot("original","block")};
  var detached=new ModAnimationSnapshot("block","move",1,intervalCopy);intervalCopy[0]=new ModAnimationIntervalSnapshot("changed","attack");
  Check(detached.Intervals[0].Name=="original","Host interval array mutation leaked");
  immutable=false;try{((IList<ModAnimationIntervalSnapshot>)detached.Intervals)[0]=intervalCopy[0];}catch(NotSupportedException){immutable=true;}
  Check(immutable,"Animation interval snapshot is writable");
  Console.WriteLine("PASS: "+checks+" production AI snapshot adapter, native nominal timing, control mapping, immutable copy and invalid-data checks; animation/model services controlled.");
 }
}
}
'@
$fixture = $fixture.Replace('/* ENUM */',$enum).Replace('/* DTO */',($dto -join "`n")).Replace('/* ADAPTER */',$adapter).Replace('/* INPUTS */',$inputs).Replace('/* COUNT */',$count).Replace('/* DURATION */',$duration).Replace('/* LOOP */',$loop)
$fixture = $fixture.Replace('/* PLAYBACK */',$playback).Replace('/* GETTERS */',($getters -join "`n")).Replace('/* INTERVAL_ENUM */',$intervalEnum)
Add-Type -TypeDefinition $fixture
[AiSnapshotTestScope.AiSnapshotTests]::Run()
