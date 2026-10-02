$ErrorActionPreference='Stop'
$root=Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$source=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Model.cs')
$methods=[regex]::Match($source,'(?s)    internal System.Action TransferFormCombatState.*?(?=\tpublic ModelController DEGJJOMLJGM)').Value
if(!$methods){throw 'Combat state extraction failed.'}
$perkSource=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/PerkInfoItem.cs')
$lookup=[regex]::Match($perkSource,'(?ms)^\tprivate void JADKFPGJAJP\(.*?^\t\}').Value
if(!$lookup){throw 'Perk variable lookup extraction failed.'}
$magic=[regex]::Match($source,'(?s)\tpublic void OGHAMAGPFLF.*?(?=\tpublic int CKAKLHDLHJO)').Value
$magic+=[regex]::Match($source,'(?ms)^\tpublic void JJHLOKBPBLD\(.*?^\t\}').Value
$magic+=[regex]::Match($source,'(?ms)^\tpublic void IPGBFKOCOCK\(.*?^\t\}').Value
$magic+=[regex]::Match($source,'(?ms)^\tpublic void BFBFNKMLOJA\(.*?^\t\}').Value
if(!$magic){throw 'Native magic charge extraction failed.'}
$fixture=Join-Path $root ('Temp/FormCombatState-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
foreach($file in @('ModelController.cs','KeyData.cs','FightCID.cs','ModelConditions.cs')) {
 Copy-Item -LiteralPath (Join-Path $root "Assets/Scripts/Assembly-CSharp/$file") -Destination $fixture
}
$code=@'
using System;
using System.Collections.Generic;
using System.Linq;
namespace UnityEngine { public struct Vector2 {} }
public class ItemInfo {} public class IntervalAnimation {} public class ModelObject {} public class ModelNode {}
public class EventAnimation {} public enum EndRoundType { None } public enum SceneTypes { SceneFight }
public class PerksStage { public class ActionPerk {} }
public class FunctionResult { public string DCJLKCFKCOM; }
public class PerkObject {}
public class FunctionExtension {
 public class Argument { public string body; }
 public class GLBAFLLMOOH { public List<Argument> EIALKNELNMB=new List<Argument>(); }
}
public class PerkInfoItem {
LOOKUP
 public string Read(Model model,string name) {
  var arguments=new FunctionExtension.GLBAFLLMOOH();
  arguments.EIALKNELNMB.Add(new FunctionExtension.Argument{body=name});
  var result=new FunctionResult();JADKFPGJAJP(model,arguments,null,result);return result.DCJLKCFKCOM;
 }
}
public class EventDispatcher<T>{readonly Dictionary<int,Action<T>> events=new Dictionary<int,Action<T>>();public void AddEventListener(int key,Action<T> action){events.TryGetValue(key,out var previous);events[key]=previous+action;}public void CallEvent(int key,T data){if(events.TryGetValue(key,out var action))action(data);}}
static class Extensions{public static bool ANNPHPHLNEH<T>(this List<T> a,List<T>b)=>a.SequenceEqual(b);}
static class LLLOJBFMONN{public static void Error(string message,int value){throw new Exception(message+": "+value);}}
class EventActBtnSettings{public float Value;public EventActBtnSettings(FightCID button,float value){Value=value;}}
class Stats {public Model Owner;public int Hits;public void RebindFormOwner(Model model){Owner=model;}}
public class Model {
METHODS
MAGIC
ModelController FEHOHLMIEBP=new ModelController();
ModelConditions _ModelConditions=new ModelConditions();
public ModelConditions EBABHGHPLFK()=>_ModelConditions;
Stats _Statistics=new Stats(),DKFGOHCNIKL=new Stats();
object MDFEHKBOHEL=new object();bool HCPHOJKFIDM;
int JMHJDHLBHLK,LGLIHLJPDIO,DJOKGDICHAJ,AIAKAAECMEH,AAEFMEJBMLH,PACHBHGEIGN;
float NJDNNFJAFBG;int MJEJFBHOJKB;Model BFFLLGHDPEB;
readonly List<float> magicEvents=new List<float>();
bool EPCNJLEHJCB()=>true;
void CallEvent(int type,EventActBtnSettings data){Check(type==12,"native magic event channel");magicEvents.Add(data.Value);}
static void Check(bool value,string message){if(!value)throw new Exception(message);}
static void Main(){
 CheckMagicTransfer();
 var old=new Model();var next=new Model();
 old.DKFGOHCNIKL.Owner=old;next.DKFGOHCNIKL.Owner=next;
 old._Statistics.Hits=17;old.DKFGOHCNIKL.Hits=29;
 old.HCPHOJKFIDM=true;old.JMHJDHLBHLK=2;old.LGLIHLJPDIO=3;old.DJOKGDICHAJ=4;old.AIAKAAECMEH=5;old.AAEFMEJBMLH=6;old.PACHBHGEIGN=7;
 var originalConditions=old._ModelConditions;var destinationConditions=next._ModelConditions;
 originalConditions.ModelName="staff";originalConditions.EclipseCharacterId="example:staff";
 destinationConditions.ModelName="baton";destinationConditions.EclipseCharacterId="example:baton";
 originalConditions.AFLPHBDFMGA=new ModelNode();destinationConditions.AFLPHBDFMGA=new ModelNode();
 var originalNode=originalConditions.AFLPHBDFMGA;var destinationNode=destinationConditions.AFLPHBDFMGA;
 var numbers=originalConditions.PerkVariables;var strings=originalConditions.PerkStringVariables;
 var preparedNumbers=destinationConditions.PerkVariables;var preparedStrings=destinationConditions.PerkStringVariables;
 numbers["Counter"]=7;strings["Phase"]="guard";numbers["Shadowed"]=2;strings["Shadowed"]="text";
 preparedNumbers["Counter"]=99;preparedNumbers["PreparedOnly"]=1;preparedStrings["Phase"]="prepared";
 var lookup=new PerkInfoItem();
 int oldEvents=0,newEvents=0;
 old.FEHOHLMIEBP.AddEventListener(0,_=>oldEvents++);old.FEHOHLMIEBP.AddEventListener(1,_=>oldEvents++);
 next.FEHOHLMIEBP.AddEventListener(0,_=>newEvents++);next.FEHOHLMIEBP.AddEventListener(1,_=>newEvents++);
 old.FEHOHLMIEBP.OnPressAnyKey(3);
 for(int i=0;i<17;i++)old.FEHOHLMIEBP.Render();
 var keys=old.FEHOHLMIEBP.ANALKHBJKIO();var cooldown=old.MDFEHKBOHEL;
 var undo=old.TransferFormCombatState(next);
 Check(lookup.Read(next,"Counter")=="7"&&lookup.Read(next,"Phase")=="guard"&&lookup.Read(next,"Shadowed")=="text"&&lookup.Read(next,"PreparedOnly")=="","native variable lookup follows the fighter without stale prepared defaults");
 Check(next._ModelConditions.PerkVariables==numbers&&next._ModelConditions.PerkStringVariables==strings&&old._ModelConditions.PerkVariables==preparedNumbers&&old._ModelConditions.PerkStringVariables==preparedStrings,"variable dictionaries transfer ownership without sharing retired state");
 Check(old._ModelConditions==originalConditions&&next._ModelConditions==destinationConditions&&originalConditions.AFLPHBDFMGA==originalNode&&destinationConditions.AFLPHBDFMGA==destinationNode&&destinationConditions.ModelName=="baton"&&destinationConditions.EclipseCharacterId=="example:baton","body-specific conditions and rig identities stay on their model");
 Check(next.FEHOHLMIEBP.ANALKHBJKIO()==keys&&keys.CEPODJDDLBF.Contains(3),"held input and buffer identity");
 Check(oldEvents==1&&newEvents==0,"transfer emits no input");
 Check(next._Statistics.Hits==17&&next.DKFGOHCNIKL.Hits==29&&next.DKFGOHCNIKL.Owner==next,"history and owner");
 Check(old.DKFGOHCNIKL.Owner==old&&next.MDFEHKBOHEL==cooldown,"retired owner and cooldown");
 Check(next.HCPHOJKFIDM&&next.JMHJDHLBHLK==2&&next.LGLIHLJPDIO==3&&next.DJOKGDICHAJ==4&&next.AIAKAAECMEH==5&&next.AAEFMEJBMLH==6&&next.PACHBHGEIGN==7,"control, round and counters");
 undo();undo();
 Check(originalConditions.PerkVariables==numbers&&originalConditions.PerkStringVariables==strings&&destinationConditions.PerkVariables==preparedNumbers&&destinationConditions.PerkStringVariables==preparedStrings&&lookup.Read(old,"Counter")=="7"&&lookup.Read(next,"Counter")=="99","rollback restores both variable owners and prepared values exactly once");
 Check(old.FEHOHLMIEBP.ANALKHBJKIO()==keys&&old._Statistics.Hits==17&&old.DKFGOHCNIKL.Owner==old&&old.MDFEHKBOHEL==cooldown,"idempotent restore");
 old.TransferFormCombatState(next);
 originalConditions.Reset();
 Check(lookup.Read(next,"Counter")=="7"&&lookup.Read(next,"Phase")=="guard"&&originalConditions.PerkVariables.Count==0&&originalConditions.PerkStringVariables.Count==0,"native retired-condition reset cannot erase active variable state");
 next.FEHOHLMIEBP.OnReleaseAnyKey(3);
 Check(newEvents==1&&oldEvents==1&&!keys.CEPODJDDLBF.Contains(3),"release goes to replacement subscribers only");
 next.FEHOHLMIEBP.OnPressAnyKey(9);
 Check(newEvents==2&&oldEvents==1,"new press reaches new body");
 numbers["Counter"]=8;strings["Phase"]="attack";
 var third=new Model();next.TransferFormCombatState(third);destinationConditions.Reset();
 Check(lookup.Read(third,"Counter")=="8"&&lookup.Read(third,"Phase")=="attack"&&third._ModelConditions.PerkVariables==numbers,"repeated form changes preserve updates and exclusive ownership");
 foreach(int invalid in Enumerable.Range(0,6)) {
  var current=new Model();var target=new Model();var currentStats=current._Statistics;
  var currentKeys=current.FEHOHLMIEBP.ANALKHBJKIO();var targetKeys=target.FEHOHLMIEBP.ANALKHBJKIO();
  if(invalid==0)current._ModelConditions=null;
  if(invalid==1)target._ModelConditions=null;
  if(invalid==2)target._ModelConditions=current._ModelConditions;
  if(invalid==3)target._ModelConditions.PerkVariables=current._ModelConditions.PerkVariables;
  if(invalid==4)target._ModelConditions.PerkStringVariables=current._ModelConditions.PerkStringVariables;
  if(invalid==5)target._ModelConditions.PerkVariables=null;
  bool rejected=false;try{current.TransferFormCombatState(target);}catch(ArgumentException){rejected=true;}
  Check(rejected&&current._Statistics==currentStats&&current.FEHOHLMIEBP.ANALKHBJKIO()==currentKeys&&target.FEHOHLMIEBP.ANALKHBJKIO()==targetKeys,"invalid variable ownership rejects before input/history mutation");
 }
 // Compare uninterrupted input with a transferred controller at every tick.
 var control=new ModelController();var moving=new ModelController();var destination=new ModelController();
 control.OnPressAnyKey(9);moving.OnPressAnyKey(9);
 for(int i=0;i<11;i++){control.Render();moving.Render();}
 moving.ExchangeFormInput(destination);
 for(int i=0;i<40;i++){control.Render();destination.Render();Check(control.ANALKHBJKIO().IGEEOAGOMEM.SequenceEqual(destination.ANALKHBJKIO().IGEEOAGOMEM)&&control.ANALKHBJKIO().CEPODJDDLBF.SequenceEqual(destination.ANALKHBJKIO().CEPODJDDLBF),"combo expiry retains original timing");}
 Console.WriteLine("PASS: production combat-state transfer, native magic charging/cast accounting, complete native controller/key data and ModelConditions reset, native perk variable lookup; partial/ready charge, continued charging and casting, numeric/text state, repeated changes, retirement isolation, rollback and preflight plus held input/history. Statistics, expression arguments, rig services and UI/event sinks controlled.");
}
static void CheckMagicTransfer(){
 foreach(float fraction in new[]{0f,0.375f,1f})foreach(int casts in new[]{0,1}){
  var current=new Model();var next=new Model();
  current.OGHAMAGPFLF(fraction);current.FLBDBIHFJAI(casts);
  next.OGHAMAGPFLF(0.125f);next.FLBDBIHFJAI(1);
  var undo=current.TransferFormCombatState(next);
  Check(next.GetMagicChargeFraction()==fraction&&next.GetMagicCharges()==casts,"partial magic charge and ready cast follow the fighter");
  Check(current.GetMagicChargeFraction()==0.125f&&current.GetMagicCharges()==1,"retired body receives prepared magic state");
  Check(current.magicEvents.Count==0&&next.magicEvents.Count==0&&next.DJOKGDICHAJ==0,"handover does not normalize, cast or emit magic events");
  undo();undo();
  Check(current.GetMagicChargeFraction()==fraction&&current.GetMagicCharges()==casts&&next.GetMagicChargeFraction()==0.125f&&next.GetMagicCharges()==1,"rollback restores both magic states exactly once");
 }
 // Compare normal charging/casting on an uninterrupted body against two swaps.
 var control=new Model();var moving=new Model();
 control.OGHAMAGPFLF(0.375f);moving.OGHAMAGPFLF(0.375f);
 var destination=new Model();moving.TransferFormCombatState(destination);
 moving.OGHAMAGPFLF(0);moving.FLBDBIHFJAI(0);
 Check(destination.GetMagicChargeFraction()==0.375f,"clearing retired magic state cannot clear the active form");
 foreach(float amount in new[]{0.125f,0.25f,0.5f}){
  control.JJHLOKBPBLD(amount);control.BFBFNKMLOJA();
  destination.JJHLOKBPBLD(amount);destination.BFBFNKMLOJA();
  Check(control.GetMagicChargeFraction()==destination.GetMagicChargeFraction()&&control.GetMagicCharges()==destination.GetMagicCharges(),"native charging matches uninterrupted fighter");
 }
 Check(destination.GetMagicCharges()==1&&destination.GetMagicChargeFraction()==0&&destination.magicEvents.SequenceEqual(control.magicEvents),"native charge threshold grants one cast and matching UI events");
 var third=new Model();destination.TransferFormCombatState(third);
 control.JJHLOKBPBLD(0.5f);third.JJHLOKBPBLD(0.5f);
 Check(third.GetMagicCharges()==1&&third.GetMagicChargeFraction()==0,"a ready cast cannot gain another charge after repeated swaps");
 control.IPGBFKOCOCK(-1);control.BFBFNKMLOJA();
 third.IPGBFKOCOCK(-1);third.BFBFNKMLOJA();
 Check(third.GetMagicCharges()==0&&third.DJOKGDICHAJ==1&&third.DJOKGDICHAJ==control.DJOKGDICHAJ,"cast consumption and usage count happen exactly once");
 control.JJHLOKBPBLD(0.25f);control.BFBFNKMLOJA();
 third.JJHLOKBPBLD(0.25f);third.BFBFNKMLOJA();
 Check(third.GetMagicChargeFraction()==control.GetMagicChargeFraction()&&third.magicEvents.Last()==control.magicEvents.Last(),"charging and native UI output continue after the cast");
}
}
'@
[IO.File]::WriteAllText((Join-Path $fixture 'Program.cs'),$code.Replace('METHODS',$methods).Replace('LOOKUP',$lookup).Replace('MAGIC',$magic))
[IO.File]::WriteAllText((Join-Path $fixture 'Test.csproj'),'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>')
dotnet run --project (Join-Path $fixture 'Test.csproj') --verbosity quiet
if($LASTEXITCODE -ne 0){throw 'Form combat state checks failed.'}
