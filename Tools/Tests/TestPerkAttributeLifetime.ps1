$ErrorActionPreference='Stop'
$root=Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$source=Get-Content -Raw (Join-Path $root 'Assets/Scripts/Assembly-CSharp/InfoPerk.cs')
$method=[regex]::Match($source,'(?ms)^\tprivate void NMIGELMNBDF\(.*?^\t\}').Value
if(!$method){throw 'Attribute effect extraction failed.'}
$transfer=[regex]::Match($source,'(?ms)^    internal System.Action TransferAttributeEffect\(.*?^    \}').Value
if(!$transfer){throw 'Attribute transfer extraction failed.'}
$stageSource=Get-Content -Raw (Join-Path $root 'Assets/Scripts/Assembly-CSharp/PerksStage.cs')
$stageTransfer=[regex]::Match($stageSource,'(?ms)^    internal System.Action TransferFormEffects\(.*?^    \}').Value
if(!$stageTransfer){throw 'Stage attribute transfer extraction failed.'}
$stageTransfer += [regex]::Match($stageSource,'(?ms)^    private static bool IsFormBodyModifier\(.*?^    \}').Value
$fixture=Join-Path $root ('Temp/PerkAttributes-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $fixture | Out-Null
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ActionType.cs') -Destination $fixture
$code=@'
using System;
using System.Collections.Generic;
static class ActionKinds{public static ActionType get_Type(this object action)=>default;}
class Attributes{public Dictionary<string,int> Values=new Dictionary<string,int>();public Attributes(){}public Attributes(Attributes source){AddRange(source);}public void Clear(){Values.Clear();}public void AddRange(Attributes source){foreach(var pair in source.Values)Values[pair.Key]=pair.Value;}public void Set(string key,int value,bool raw=false){Values[key]=value;}public bool Get(string key,ref int value,bool a=false,bool b=false){return Values.TryGetValue(key,out value);}}
class FunctionResult{public int Value;public int ToInt()=>Value;}
class FunctionExtension{public int Value,Calls;public bool Fail;public FunctionResult IBCPKBBAFNH(){Calls++;if(Fail)throw new Exception("expression");return new FunctionResult{Value=Value};}}
class PerkActionSetAttributes{public Dictionary<string,FunctionExtension> Values=new Dictionary<string,FunctionExtension>();public Dictionary<string,FunctionExtension> NNBFJDJAAGI()=>Values;}
class Parameters{public Attributes IBLHIAHECLK=new Attributes();}
class Model{public Action CopyFormModifiersFrom(Model old){return()=>{};}public Parameters Parameters=new Parameters();public class StrikeResult{public void GGENIBPJPAG(int id){}}public StrikeResult GHHCDAFIKJE=new StrikeResult();}
class Registration{public List<InfoPerk> Perks=new List<InfoPerk>();public List<InfoPerk> HIPOGANEPMI()=>Perks;}
class PerksStage{public List<Registration> MPJMCCGKEOD=new List<Registration>();List<ActionPerk> JLAKGOEOHMN=new List<ActionPerk>();static Dictionary<string,List<ActionPerk>> PNAALKAHAKG=new Dictionary<string,List<ActionPerk>>();
 STAGE_TRANSFER
 public class ActionPerk{public object AMKJNPOCODK;public Model KJDFJPBIGJC=new Model(),BIKLKJMNGKP=new Model();public Dictionary<string,int> AppliedAttributes;}}
class ModHealthChange{}
class Definition{public int Id=1;}class Data{public Definition MBDDKGIOOGD=new Definition();}
class InfoPerk{
 List<PerksStage.ActionPerk> NBFBBDHELEJ=new List<PerksStage.ActionPerk>();
 public List<PerksStage.ActionPerk> HIPOGANEPMI()=>NBFBBDHELEJ;
 public System.Action TransferHealthEffect(PerksStage.ActionPerk action,Model old,Model next){throw new Exception("not used by attribute fixture");}
 Data DCMHONAFOGI=new Data();bool IHAHGIHPNIG()=>false;
 METHOD
 TRANSFER
 static void Check(bool x,string why){if(!x)throw new Exception(why);}
 static void Main(){
 var info=new InfoPerk();var expression=new FunctionExtension{Value=12};var definition=new PerkActionSetAttributes();definition.Values.Add("DamageFactor",expression);
 var action=new PerksStage.ActionPerk{AMKJNPOCODK=definition};var stats=action.KJDFJPBIGJC.Parameters.IBLHIAHECLK;stats.Set("DamageFactor",100);
 info.NMIGELMNBDF(action,false);Check(stats.Values["DamageFactor"]==112&&action.AppliedAttributes["DamageFactor"]==12,"applied normalized value recorded");
 expression.Value=30;stats.Set("DamageFactor",119);info.NMIGELMNBDF(action,true);
 Check(stats.Values["DamageFactor"]==107&&expression.Calls==1,"expiry removes original amount and preserves unrelated changes without reevaluation");
 var broken=new FunctionExtension{Fail=true};definition.Values.Add("Defense",broken);bool failed=false;
 try{info.NMIGELMNBDF(action,false);}catch(Exception){failed=true;}
 Check(failed&&stats.Values["DamageFactor"]==107&&!stats.Values.ContainsKey("Defense"),"later expression failure cannot leave partial attributes");
 var legacy=new PerksStage.ActionPerk{AMKJNPOCODK=new PerkActionSetAttributes()};((PerkActionSetAttributes)legacy.AMKJNPOCODK).Values.Add("DamageFactor",new FunctionExtension{Value=5});legacy.KJDFJPBIGJC.Parameters.IBLHIAHECLK.Set("DamageFactor",15);
 info.NMIGELMNBDF(legacy,true);Check(legacy.KJDFJPBIGJC.Parameters.IBLHIAHECLK.Values["DamageFactor"]==10,"unrecorded legacy action retains fallback expiry");
 definition.Values.Remove("Defense");expression.Value=12;info.NMIGELMNBDF(action,false);info.NBFBBDHELEJ.Add(action);
 var old=action.KJDFJPBIGJC;action.BIKLKJMNGKP=old;var replacement=new Model();var destination=replacement.Parameters.IBLHIAHECLK;destination.Set("DamageFactor",200);
 var rollback=info.TransferAttributeEffect(action,old,replacement);
 Check(stats.Values["DamageFactor"]==107&&destination.Values["DamageFactor"]==212&&action.KJDFJPBIGJC==replacement&&action.BIKLKJMNGKP==replacement,"effect removed from old body and applied to destination");
 rollback();Check(stats.Values["DamageFactor"]==119&&destination.Values["DamageFactor"]==200&&action.KJDFJPBIGJC==old&&action.BIKLKJMNGKP==old,"transfer rollback restores both bodies and references");
 destination.Set("DamageFactor",int.MaxValue);failed=false;try{info.TransferAttributeEffect(action,old,replacement);}catch(OverflowException){failed=true;}
 Check(failed&&stats.Values["DamageFactor"]==119&&destination.Values["DamageFactor"]==int.MaxValue&&action.KJDFJPBIGJC==old,"overflow rejects before live mutation");
 destination.Set("DamageFactor",200);info.TransferAttributeEffect(action,old,replacement);expression.Value=99;info.NMIGELMNBDF(action,true);
 Check(destination.Values["DamageFactor"]==200&&stats.Values["DamageFactor"]==107,"later expiry undoes original delta on new body only");
 var owner=new Model();owner.Parameters.IBLHIAHECLK.Set("Defense",112);var next=new Model();next.Parameters.IBLHIAHECLK.Set("Defense",int.MaxValue-6);
 var a=new PerksStage.ActionPerk{AMKJNPOCODK=new PerkActionSetAttributes(),KJDFJPBIGJC=owner,AppliedAttributes=new Dictionary<string,int>{{"Defense",5}}};
 var b=new PerksStage.ActionPerk{AMKJNPOCODK=new PerkActionSetAttributes(),KJDFJPBIGJC=owner,AppliedAttributes=new Dictionary<string,int>{{"Defense",7}}};
 var effects=new InfoPerk();effects.NBFBBDHELEJ.AddRange(new[]{a,b});var registration=new Registration();registration.Perks.Add(effects);var stage=new PerksStage();stage.MPJMCCGKEOD.Add(registration);
 failed=false;try{stage.TransferFormEffects(owner,next);}catch(OverflowException){failed=true;}
 Check(failed&&owner.Parameters.IBLHIAHECLK.Values["Defense"]==112&&next.Parameters.IBLHIAHECLK.Values["Defense"]==int.MaxValue-6&&a.KJDFJPBIGJC==owner&&b.KJDFJPBIGJC==owner,"later transfer failure restores earlier attribute effect");
 next.Parameters.IBLHIAHECLK.Set("Defense",200);var undoAll=stage.TransferFormEffects(owner,next);
 Check(owner.Parameters.IBLHIAHECLK.Values["Defense"]==100&&next.Parameters.IBLHIAHECLK.Values["Defense"]==212,"multiple effects compose on new body");undoAll();
 Check(owner.Parameters.IBLHIAHECLK.Values["Defense"]==112&&next.Parameters.IBLHIAHECLK.Values["Defense"]==200,"multiple effect snapshots restore in reverse order");
 Console.WriteLine("PASS: production attribute perk lifetime; original delta expiry, independent stat changes, expression failure before mutation and legacy fallback. Attribute normalization/services controlled.");
 }
}
'@
$code=$code.Replace('METHOD',$method)
$code=$code.Replace('STAGE_TRANSFER',$stageTransfer).Replace('TRANSFER',$transfer)
[IO.File]::WriteAllText((Join-Path $fixture 'Program.cs'),$code)
[IO.File]::WriteAllText((Join-Path $fixture 'Test.csproj'),'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>')
dotnet run --project (Join-Path $fixture 'Test.csproj') --verbosity quiet
if($LASTEXITCODE -ne 0){throw 'Attribute lifetime checks failed.'}
