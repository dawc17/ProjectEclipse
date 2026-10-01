$ErrorActionPreference='Stop'
$root=Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$source=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Fight.cs')
$method=[regex]::Match($source,'(?ms)^    internal bool QueuePreparedFighterForm.*?^    \}').Value
if(!$method){throw 'Prepared request extraction failed.'}
$fixture=Join-Path $root ('Temp/FormRequest-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$code=@'
using System;
using System.Collections.Generic;
class Vector3f{public float X;public Vector3f(float x){X=x;}public Vector3f(Vector3f other){X=other.X;}}
class Parameters{public bool IsPlayer,IsWinner;public int RoundsWon;public float CIDCNCDFONA=100,Health=100;public void GFNCMLFKBGP(float value){Health=value;}}
class Model{public Parameters Parameters=new Parameters();public List<Model> _Enemies=new List<Model>();public Model Owner;public Vector3f Position=new Vector3f(0);public int NFOOGKCGFAB=1;public object KDAHHIMLJGG=new object();public float KKMCHCNOHMB()=>Parameters.Health;public Vector3f PLBNCDCFPML()=>Position;public int KFCNPADAMHA()=>NFOOGKCGFAB;public void SetModelPosition(Vector3f value){Position=value;}public Model BDJBNOPNCNB()=>Owner??this;public void CJNGMIMHFCC(Model enemy){_Enemies.Add(enemy);}}
class Selector{public object Pending;public void PrepareFormAnimation(Model model){Pending=model.KDAHHIMLJGG;}}
class Fight{
METHOD
internal class PreparedFormModel:IDisposable{public Model Model;public int Disposals;public void Dispose(){if(Model!=null){Disposals++;Model=null;}}}
internal class FormRenderBindings:IDisposable{Fight fight;public bool Committed;public FormRenderBindings(Fight f,Model old,Model next){fight=f;f.Bound=true;}public void Dispose(){if(!Committed){fight.Bound=false;fight._SelectAnimation.Pending=null;}}}
bool Reject,Bound,FailCommit;int Commits;Action Apply;Action<Exception> Complete;Selector _SelectAnimation=new Selector();
bool QueueModelTransition(Model model,Action apply,Action<Exception> complete){if(Reject)return false;Apply=apply;Complete=complete;return true;}
void CommitPreparedForm(Model old,PreparedFormModel prepared,FormRenderBindings binding){if(FailCommit)throw new InvalidOperationException("effect transfer incomplete");Commits++;binding.Committed=true;prepared.Model=null;}
void Drain(){Exception error=null;try{Apply();}catch(Exception e){error=e;}Complete(error);}
static void Check(bool value,string message){if(!value)throw new Exception(message);}
static void Main(){
 var f=new Fight();var old=new Model();var next=new Model();next.Parameters.CIDCNCDFONA=200;
 var enemy=new Model();var child=new Model{Owner=enemy};old._Enemies.AddRange(new[]{enemy,enemy,child});
 var p=new PreparedFormModel{Model=next};int callbacks=0;Exception result=null;
 Check(f.QueuePreparedFighterForm(old,p,e=>{callbacks++;result=e;}),"accepted");
 Check(!f.Bound&&callbacks==0&&p.Model==next,"request is deferred");
 old.Parameters.Health=25;old.Parameters.RoundsWon=2;old.Position.X=73;old.NFOOGKCGFAB=-1;
 f.Drain();
 Check(callbacks==1&&result==null&&f.Commits==1&&p.Disposals==0,"committed body survives request completion");
 Check(next.Parameters.Health==50&&next.Parameters.RoundsWon==2&&next.Position.X==73&&next.Position!=old.Position&&next.NFOOGKCGFAB==-1,"boundary health fraction, round wins, position and facing");
 Check(next._Enemies.Count==1&&next._Enemies[0]==enemy&&f._SelectAnimation.Pending==next.KDAHHIMLJGG,"root enemy deduplication and deferred animation entry");
 f=new Fight{Reject=true};p=new PreparedFormModel{Model=new Model()};callbacks=0;
 Check(!f.QueuePreparedFighterForm(old,p,e=>callbacks++)&&p.Model!=null&&p.Disposals==0&&callbacks==0,"queue rejection leaves caller ownership");
 f=new Fight{FailCommit=true};p=new PreparedFormModel{Model=new Model()};result=null;
 f.QueuePreparedFighterForm(old,p,e=>result=e);f.Drain();
 Check(result is InvalidOperationException&&p.Disposals==1&&!f.Bound&&f._SelectAnimation.Pending==null,"failed commit restores registrations and drops animation entry before disposal");
 f=new Fight();p=new PreparedFormModel{Model=new Model()};result=null;
 f.QueuePreparedFighterForm(old,p,e=>result=e);f.Complete(new OperationCanceledException());
 Check(result is OperationCanceledException&&p.Disposals==1&&f.Commits==0,"boundary cancellation releases unclaimed body");
 Console.WriteLine("PASS: production prepared form request; deferred live-state capture, health ratio/wins/position, enemy links, animation entry, ownership, rejection, rollback and cancellation. Boundary/registration/animation services controlled.");
}
}
'@
[IO.File]::WriteAllText((Join-Path $fixture 'Program.cs'),$code.Replace('METHOD',$method))
[IO.File]::WriteAllText((Join-Path $fixture 'Test.csproj'),'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>')
dotnet run --project (Join-Path $fixture 'Test.csproj') --verbosity quiet
if($LASTEXITCODE -ne 0){throw 'Prepared form request checks failed.'}
