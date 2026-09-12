# Execute the native shortlist and priority checks; model/animation services are controlled.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$native = Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ModelAi.cs')
$filter = [regex]::Match($native, '(?ms)^\tprivate int GetPlayableAnimations\(List<InfoAnimation>.*?^\t\}')
$eligible = [regex]::Match($native, '(?ms)^\tprivate bool IsPlayableAnimations\(.*?^\t\}')
$characterSource = Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModRuntimeP1D.cs')
$character = [regex]::Match($characterSource, '(?ms)^    public sealed class ModCharacterCondition.*?^    \}')
if (!$filter.Success -or !$eligible.Success -or !$character.Success) { throw 'Native AI/character eligibility source not found' }
$fixture = @'
using System;
using System.Collections.Generic;
public class ModelConditions {
 public string EclipseCharacterId;
 public bool IDCHHGHAENM=true, Uninterrupt;
 public object PDKPGKPBBIL; public int PCAOCHAIBJC, FOIHIKCEBJF;
}
public class ConditionAnimation {
 public enum DGAGKLODADD { ECLIPSE_CHARACTER }
 public bool IsNot;
 public ConditionAnimation(DGAGKLODADD kind){}
 public virtual bool IsEqual(ModelConditions conditions){return true;}
}
namespace Eclipse.Modding { /* CHARACTER */ }
public class EventAnimation { public enum EECEJKADLCK { EVENT_KEY_PRESSED } }
public class InfoAnimation {
 public string Name, Character="example.author:warriors/fighter";
 public bool HasKeys=true, Available=true;
 public int Evaluations;
 public class CapabilityTable { public List<InfoAnimation> NINJLLDJLFI=new List<InfoAnimation>(); }
 public class Properties { public Directions ILOEBFFAEAN=new Directions(); }
 public class Directions { public int OLBDPMKCJIF; }
 public Properties ODACDCDONJE=new Properties();
 public CapabilityTable ICANLHJKKNE=new CapabilityTable();
 public object ILBCHANCOBP(){return HasKeys ? this : null;}
 public object FOLOOGCLPNE(){return Name;}
 public int CEDEDCLGJDE(ModelConditions c,int direction){return direction;}
 public EventAnimation OIGBIFNICBI(EventAnimation.EECEJKADLCK e){return new EventAnimation();}
 public bool HPPGNJJCEGF(Model model,object unused,EventAnimation e){
  Evaluations++;
  return Available && !model.Conditions.Uninterrupt && new Eclipse.Modding.ModCharacterCondition(Character).IsEqual(model.Conditions);
 }
}
public class Model {
 public List<InfoAnimation> Moves=new List<InfoAnimation>(); public ModelConditions Conditions=new ModelConditions();
 public List<InfoAnimation> MCFPDHOLNGB(){return Moves;} public ModelConditions EBABHGHPLFK(){return Conditions;}
}
public class ModelAnimation { public int Facing=1; public int KFCNPADAMHA(){return Facing;} }
public static class LLLOJBFMONN { public static void Error(string message){} }
public static class ListExtensions { public static void CPCAJIKOIEE<T>(this List<T> items,int count){items.RemoveRange(count,items.Count-count);} }
public class EligibilityFixture {
 readonly Model _Model; readonly ModelAnimation _ModelAnimation=new ModelAnimation();
 public EligibilityFixture(Model model){_Model=model;}
 Model get_Model(){return _Model;}
 bool IsTacticPlayableAnimations(InfoAnimation animation){throw new Exception("Unexpected tactic-only path");}
 public int Filter(List<InfoAnimation> moves,List<int> indices=null){return GetPlayableAnimations(moves,indices);}
 /* FILTER */
 /* ELIGIBLE */
}
public static class AiEligibilityTests {
 static int checks;
 static void Check(bool condition,string message){checks++;if(!condition)throw new Exception(message);}
 public static void Run(){
  var actor=new Model(); actor.Conditions.EclipseCharacterId="example.author:warriors/fighter";
  var primary=new InfoAnimation{Name="primary"};var kick=new InfoAnimation{Name="kick"};
  var foreign=new InfoAnimation{Name="foreign",Character="example.other:warriors/fighter"};
  var eventOnly=new InfoAnimation{Name="event",HasKeys=false};var unavailable=new InfoAnimation{Name="locked",Available=false};
  actor.Moves.AddRange(new[]{primary,eventOnly,foreign,kick,unavailable});var host=new EligibilityFixture(actor);
  var candidates=new List<InfoAnimation>(actor.Moves);var indices=new List<int>{0,1,2,3,4};
  Check(host.Filter(candidates,indices)==2,"Valid authored clips missing or invalid clips exposed");
  Check(candidates[0]==primary&&candidates[1]==kick&&indices[0]==0&&indices[1]==3,"Filtering changed action/index alignment");
  Check(actor.Moves.Count==5,"Shortlisting mutated the model's registered moves");
  Check(eventOnly.Evaluations==0,"Event-only move reached input selection");
  actor.Conditions.Uninterrupt=true;candidates=new List<InfoAnimation>(actor.Moves);
  Check(host.Filter(candidates)==0&&candidates.Count==0,"Uninterruptible state still offered a clip");
  actor.Conditions.Uninterrupt=false;actor.Conditions.EclipseCharacterId="core:warriors/other";
  candidates=new List<InfoAnimation>(actor.Moves);Check(host.Filter(candidates)==0,"Authored clips leaked to another warrior");
  actor.Conditions.EclipseCharacterId="example.author:warriors/fighter";
  var preferred=new InfoAnimation{Name="preferred"};actor.Moves.Add(preferred);primary.ICANLHJKKNE.NINJLLDJLFI.Add(preferred);
  candidates=new List<InfoAnimation>{primary,kick};Check(host.Filter(candidates)==1&&candidates[0]==kick,"Playable priority competitor did not suppress primary input");
  preferred.Available=false;candidates=new List<InfoAnimation>{primary,kick};Check(host.Filter(candidates)==2,"Unavailable competitor suppressed authored clip");
  preferred.Available=true;preferred.HasKeys=false;candidates=new List<InfoAnimation>{primary};Check(host.Filter(candidates)==1,"Event-only competitor suppressed input-driven move");
  preferred.HasKeys=true;actor.Moves.Remove(preferred);candidates=new List<InfoAnimation>{primary};Check(host.Filter(candidates)==1,"Unregistered competitor suppressed input-driven move");
  candidates=new List<InfoAnimation>{null,preferred,kick};Check(host.Filter(candidates)==1&&candidates[0]==kick,"Null/unregistered move escaped filtering");
  var condition=new Eclipse.Modding.ModCharacterCondition("example.author:warriors/fighter");
  Check(condition.IsEqual(actor.Conditions)&&!condition.IsEqual(null),"Character condition identity/null handling changed");
  condition.IsNot=true;Check(!condition.IsEqual(actor.Conditions),"Negated character restriction failed");
  actor.Conditions=null;candidates=new List<InfoAnimation>{kick};Check(host.Filter(candidates)==0,"Absent model conditions allowed a clip");
  Console.WriteLine("PASS: "+checks+" native AI shortlist/priority and production character-condition checks; animation predicates, model state and key metadata controlled.");
 }
}
'@
Add-Type -TypeDefinition $fixture.Replace('/* CHARACTER */',$character.Value).Replace('/* FILTER */',$filter.Value).Replace('/* ELIGIBLE */',$eligible.Value)
[AiEligibilityTests]::Run()
