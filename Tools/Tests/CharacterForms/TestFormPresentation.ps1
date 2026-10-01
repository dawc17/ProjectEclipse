$ErrorActionPreference='Stop'
$root=Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$source=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Fight.cs')
$method=[regex]::Match($source,'(?ms)^    internal Action BindFormPresentation.*?^    \}').Value
if(!$method){throw 'Presentation extraction failed.'}
$attach=[regex]::Match($source,'(?ms)^\tprivate void SetModelOnListening.*?^\t\}').Value
$detach=[regex]::Match($source,'(?ms)^    private void StopModelListening.*?^    \}').Value
$adds=@([regex]::Matches($attach,'AddEventListener\((\d+), (\w+)\)') | ForEach-Object {$_.Groups[1].Value+':'+$_.Groups[2].Value})
$removes=@([regex]::Matches($detach,'RemoveEventListener\((\d+), (\w+)\)') | ForEach-Object {$_.Groups[1].Value+':'+$_.Groups[2].Value})
if($adds.Count -ne 13 -or (Compare-Object $adds $removes)){throw 'Fight listener handover is not symmetric.'}
$fixture=Join-Path $root ('Temp/FormPresentation-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$code=@'
using System;
class Model{public object Parameters=new object();public bool Listening;}
class Panel{public object Parameters;public bool FailOnce;public int Refreshes;public bool RefreshForm(object old,object next){if(Parameters!=old)return false;Parameters=next;Refreshes++;if(FailOnce){FailOnce=false;throw new Exception("render failure");}return true;}}
class Viewer{public Panel Left=new Panel(),Right=new Panel();public Panel get_LeftModel()=>Left;public Panel get_RightModel()=>Right;}
class PreFight{public Viewer Viewer=new Viewer();public Viewer get_ViewerFight()=>Viewer;}
class Fight{
METHOD
PreFight preFight=new PreFight();
void SetModelOnListening(Model model){model.Listening=true;}
void StopModelListening(Model model){model.Listening=false;}
static void Check(bool value,string message){if(!value)throw new Exception(message);}
static void Main(){
 foreach(bool player in new[]{true,false}){
  var f=new Fight();var old=new Model{Listening=true};var next=new Model();var panel=player?f.preFight.Viewer.Left:f.preFight.Viewer.Right;
  panel.Parameters=old.Parameters;var undo=f.BindFormPresentation(old,next,player);
  Check(!old.Listening&&next.Listening&&panel.Parameters==next.Parameters,"HUD and listeners transferred");
  undo();undo();Check(old.Listening&&!next.Listening&&panel.Parameters==old.Parameters&&panel.Refreshes==2,"idempotent presentation rollback");
  panel.FailOnce=true;bool failed=false;try{f.BindFormPresentation(old,next,player);}catch(Exception){failed=true;}
  Check(failed&&old.Listening&&!next.Listening&&panel.Parameters==old.Parameters,"render failure after parameter assignment restores both systems");
  var unrelated=new object();panel.Parameters=unrelated;failed=false;try{f.BindFormPresentation(old,next,player);}catch(InvalidOperationException){failed=true;}
  Check(failed&&panel.Parameters==unrelated&&old.Listening&&!next.Listening,"stale HUD precondition preserves unrelated panel");
 }
 var absent=new Fight{preFight=null};var a=new Model{Listening=true};var b=new Model();var restore=absent.BindFormPresentation(a,b,true);Check(b.Listening&&!a.Listening,"headless event transfer");restore();
 Console.WriteLine("PASS: production presentation transaction; both panels, listener ownership, post-assignment render failure, stale HUD, headless and idempotent rollback. Panel/events controlled; all 13 production listener pairs audited.");
}
}
'@
[IO.File]::WriteAllText((Join-Path $fixture 'Program.cs'),$code.Replace('METHOD',$method))
[IO.File]::WriteAllText((Join-Path $fixture 'Test.csproj'),'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>')
dotnet run --project (Join-Path $fixture 'Test.csproj') --verbosity quiet
if($LASTEXITCODE -ne 0){throw 'Form presentation checks failed.'}
