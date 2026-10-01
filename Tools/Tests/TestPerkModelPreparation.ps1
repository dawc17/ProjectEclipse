$ErrorActionPreference='Stop'
$root=Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$source=Get-Content -Raw (Join-Path $root 'Assets/Scripts/Assembly-CSharp/PerksStage.cs')
$methods=@('(?ms)^\tpublic void AddModel\(.*?^\t\}', '(?ms)^\tpublic void RemoveModel\(.*?^\t\}', '(?ms)^    internal PerkModelStruct PrepareModelRegistration\(.*?^    \}') | ForEach-Object { $m=[regex]::Match($source,$_).Value;if(!$m){throw 'Perk method extraction failed.'};$m }
$fixture=Join-Path $root ('Temp/PerkPreparation-'+[Guid]::NewGuid().ToString('N'))
$queued=[regex]::Match($source,'(?ms)^    internal System.Action RebindQueuedFormActions\(.*?^    \}').Value
if(!$queued){throw 'Queued perk method extraction failed.'}
New-Item -ItemType Directory $fixture | Out-Null
$code=@'
using System;
using System.Collections.Generic;
class PerkInfoItem{public bool Fail;}
class Parameters{public List<PerkInfoItem> Perks=new List<PerkInfoItem>();}
class Model{public Parameters Parameters=new Parameters();}
class InfoPerk{public List<PerksStage.ActionPerk> Pending=new List<PerksStage.ActionPerk>(),Active=new List<PerksStage.ActionPerk>();public List<PerksStage.ActionPerk> MNLNLKOJPHO()=>Pending;public List<PerksStage.ActionPerk> HIPOGANEPMI()=>Active;}
class PerkModelStruct{public Model Model;public List<InfoPerk> Effects=new List<InfoPerk>();public List<InfoPerk> HIPOGANEPMI()=>Effects;public List<PerkInfoItem> Perks=new List<PerkInfoItem>();public void set_Model(Model value){Model=value;}public Model get_Model()=>Model;}
class PerksStage{
 public class ActionPerk{public Model KJDFJPBIGJC,BIKLKJMNGKP;public int KGNDJOLBBJF,FLNLMIHEDCI;}
 List<ActionPerk> JLAKGOEOHMN=new List<ActionPerk>();static Dictionary<string,List<ActionPerk>> PNAALKAHAKG=new Dictionary<string,List<ActionPerk>>();
 List<PerkModelStruct> MPJMCCGKEOD=new List<PerkModelStruct>();
 void OPACOCIKEOL(PerkModelStruct prepared,PerkInfoItem perk){if(perk==null)return;prepared.Perks.Add(perk);if(perk.Fail)throw new InvalidOperationException("invalid trigger");}
 METHODS
 QUEUED
 static void Check(bool x,string why){if(!x)throw new Exception(why);}
 static void Main(){
 var stage=new PerksStage();var model=new Model();var other=new Model();var first=new PerkInfoItem();var broken=new PerkInfoItem{Fail=true};
 model.Parameters.Perks.Add(first);stage.AddModel(model);stage.AddModel(other);var original=stage.MPJMCCGKEOD[0];
 var prepared=stage.PrepareModelRegistration(model);Check(stage.MPJMCCGKEOD.Count==2&&stage.MPJMCCGKEOD[0]==original&&prepared!=original&&prepared.get_Model()==model&&prepared.Perks[0]==first,"preparation builds detached registration without replacing active identity");
 model.Parameters.Perks.Add(broken);bool failed=false;try{stage.AddModel(model);}catch(InvalidOperationException){failed=true;}
 Check(failed&&stage.MPJMCCGKEOD.Count==2&&stage.MPJMCCGKEOD[0]==original&&original.Perks.Count==1,"later invalid perk preserves previous registration and trigger table");
 var fresh=new Model();fresh.Parameters.Perks.Add(broken);failed=false;try{stage.AddModel(fresh);}catch(InvalidOperationException){failed=true;}Check(failed&&stage.MPJMCCGKEOD.Count==2,"failed fresh preparation leaves no registration");
 broken.Fail=false;stage.AddModel(model);Check(stage.MPJMCCGKEOD.Count==2&&stage.MPJMCCGKEOD[0].get_Model()==other&&stage.MPJMCCGKEOD[1].Perks.Count==2,"successful replacement preserves native append ordering");
 failed=false;try{stage.PrepareModelRegistration(null);}catch(ArgumentNullException){failed=true;}Check(failed,"null preparation rejects");
 var effect=new InfoPerk();stage.MPJMCCGKEOD[0].Effects.Add(effect);var replacement=new Model();
 var target=new ActionPerk{KJDFJPBIGJC=model,BIKLKJMNGKP=other,KGNDJOLBBJF=7,FLNLMIHEDCI=35};var source=new ActionPerk{KJDFJPBIGJC=other,BIKLKJMNGKP=model};var both=new ActionPerk{KJDFJPBIGJC=model,BIKLKJMNGKP=model};
 var active=new ActionPerk{KJDFJPBIGJC=model};var indexed=new ActionPerk{BIKLKJMNGKP=model};var recent=new ActionPerk{KJDFJPBIGJC=model};
 effect.Pending.AddRange(new[]{target,source,both,target,active,indexed,recent});effect.Active.Add(active);PNAALKAHAKG["active"]=new List<ActionPerk>{indexed};stage.JLAKGOEOHMN.Add(recent);
 var undo=stage.RebindQueuedFormActions(model,replacement);
 Check(target.KJDFJPBIGJC==replacement&&target.BIKLKJMNGKP==other&&source.BIKLKJMNGKP==replacement&&source.KJDFJPBIGJC==other&&both.KJDFJPBIGJC==replacement&&both.BIKLKJMNGKP==replacement,"queued source and target references transferred independently");
 Check(target.KGNDJOLBBJF==7&&target.FLNLMIHEDCI==35&&effect.Pending.Count==7&&effect.Pending[0]==effect.Pending[3],"timing, queue order and shared action identity preserved");
 Check(active.KJDFJPBIGJC==model&&indexed.BIKLKJMNGKP==model&&recent.KJDFJPBIGJC==model,"active aliases excluded across all registries");
 undo();Check(target.KJDFJPBIGJC==model&&source.BIKLKJMNGKP==model&&both.KJDFJPBIGJC==model&&both.BIKLKJMNGKP==model,"queued reference rollback");
 Console.WriteLine("PASS: production perk registration preparation/AddModel/RemoveModel; detached construction, later trigger failure, preserved live registration and successful replacement. Trigger construction controlled; active-effect migration not covered.");
 }
}
'@
$code=$code.Replace('METHODS',($methods -join "`n"))
$code=$code.Replace('QUEUED',$queued)
[IO.File]::WriteAllText((Join-Path $fixture 'Program.cs'),$code)
[IO.File]::WriteAllText((Join-Path $fixture 'Test.csproj'),'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>')
dotnet run --project (Join-Path $fixture 'Test.csproj') --verbosity quiet
if($LASTEXITCODE -ne 0){throw 'Perk preparation checks failed.'}
