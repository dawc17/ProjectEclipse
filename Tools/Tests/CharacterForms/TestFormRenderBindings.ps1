$ErrorActionPreference='Stop'
$root=Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$source=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Fight.cs')
$stage=[regex]::Match($source,'(?s)    internal sealed class FormRenderBindings.*?(?=    private readonly Dictionary<Model, PendingModelTransition>)').Value
if(!$stage){throw 'Form binding stage extraction failed.'}
$fixture=Join-Path $root ('Temp/FormBindings-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$code=@'
using System;
using System.Collections.Generic;
class Model{public Action TransferFormCombatState(Model next){return()=>{};}public Model Owner;public List<Model> _Enemies=new List<Model>(),Weapons=new List<Model>();public bool RejectEnemy,RejectEnemyRestore;public int Exchanges;public Model BDJBNOPNCNB()=>Owner??this;public List<Model> KGGIDBLBMDJ()=>Weapons;public Action ReplaceEnemyForm(Model old,Model next){if(RejectEnemy)throw new InvalidOperationException("enemy exchange");int index=_Enemies.IndexOf(old);_Enemies[index]=next;Exchanges++;return()=>{if(RejectEnemyRestore)throw new InvalidOperationException("enemy restore");_Enemies[index]=old;};}}
class Binding{public Model Current,Original;public bool Reject,RejectRestore;public int Calls,EventRestores;public Action CapturePendingEvents(){var captured=Current;return()=>{if(Current!=captured)throw new Exception("events restored before registration");EventRestores++;};}public bool ReplaceModel(Model old,Model next,bool player=false){Calls++;if(Reject||(RejectRestore&&next==Original)||Current!=old)return false;Current=next;return true;}}
class Rules{public Model Current;public bool Reject;public Action PrepareModelRebind(Model old,Model next){if(Reject||Current!=old)throw new InvalidOperationException("rules");return()=>Current=next;}}
class Perks{public Model Current,AttributeTarget,Registration;public Action ReplaceFormRegistration(Model old,Model next){Registration=next;return()=>Registration=old;}public Action TransferFormEffects(Model old,Model next){AttributeTarget=next;return()=>AttributeTarget=old;}public Action RebindQueuedFormActions(Model old,Model next){Current=next;return()=>Current=old;}}
class Fight{
 Model _playerModel=new Model(),CKNCPOABFBO=new Model();Binding _Camera=new Binding(),_SelectAnimation=new Binding();Rules _rulesInspector=new Rules();
 List<Model> LNDLFINJHDB=new List<Model>();
 Perks EPBDEDGLHJE=new Perks();
 Action BindFormPresentation(Model expected,Model replacement,bool player){return()=>{};} Action BindFormParticipant(Model expected,Model replacement){_playerModel=replacement;return()=>_playerModel=expected;}
 public Fight(){_Camera.Current=_Camera.Original=_SelectAnimation.Current=_SelectAnimation.Original=_rulesInspector.Current=_playerModel;LNDLFINJHDB.AddRange(new[]{_playerModel,CKNCPOABFBO});CKNCPOABFBO._Enemies.Add(_playerModel);}
 STAGE
 static void Check(bool x,string why){if(!x)throw new Exception(why);}
 public static void Main(){
  var f=new Fight();var next=new Model();
  var stage=new FormRenderBindings(f,f._playerModel,next);
  Check(f._Camera.Current==next&&f._SelectAnimation.Current==next&&f._rulesInspector.Current==next,"all registrations staged");
  Check(f._playerModel==next,"participant identity follows staged registrations");
  Check(f.EPBDEDGLHJE.Current==next,"queued perks staged");
  Check(f.EPBDEDGLHJE.AttributeTarget==next&&f.EPBDEDGLHJE.Registration==next,"attribute effects and registration staged");
  stage.Dispose();stage.Dispose();Check(f._Camera.Current==f._playerModel&&f._SelectAnimation.Current==f._playerModel&&f._rulesInspector.Current==f._playerModel,"rollback and idempotent dispose");
  Check(f._SelectAnimation.EventRestores==1,"events restored once after reversing selector registration");
  Check(f.EPBDEDGLHJE.Current==f._playerModel,"queued perks restored");
  Check(f.EPBDEDGLHJE.AttributeTarget==f._playerModel&&f.EPBDEDGLHJE.Registration==f._playerModel,"attribute effects and registration restored");
  f._rulesInspector.Reject=true;bool failed=false;try{new FormRenderBindings(f,f._playerModel,next);}catch(InvalidOperationException){failed=true;}
  Check(failed&&f._Camera.Current==f._playerModel,"rule validation precedes mutation");f._rulesInspector.Reject=false;
  f._SelectAnimation.Reject=true;failed=false;try{new FormRenderBindings(f,f._playerModel,next);}catch(InvalidOperationException){failed=true;}
  Check(failed&&f._Camera.Current==f._playerModel,"animation rejection restores camera");f._SelectAnimation.Reject=false;
  stage=new FormRenderBindings(f,f._playerModel,next);stage.Commit();stage.Dispose();Check(f._Camera.Current==next&&f._rulesInspector.Current==next,"commit retains bindings");
  Check(f._SelectAnimation.EventRestores==1,"commit and rejected exchange do not restore stale events");
  f=new Fight();stage=new FormRenderBindings(f,f._playerModel,next);f._SelectAnimation.RejectRestore=true;failed=false;
  try{stage.Dispose();}catch(AggregateException){failed=true;}
  Check(failed&&f._Camera.Current==f._playerModel&&f._rulesInspector.Current==f._playerModel,"restoration attempts remaining systems after failure");
  Check(f._SelectAnimation.EventRestores==0,"failed selector restoration does not install old events on replacement");
  f=new Fight();f._Camera.RejectRestore=true;f._SelectAnimation.Reject=true;failed=false;
  try{new FormRenderBindings(f,f._playerModel,next);}catch(AggregateException e){failed=e.InnerExceptions.Count==2;}
  Check(failed,"original plus rollback failures reported");
  f=new Fight();var weapon=new Model{Owner=f.CKNCPOABFBO};weapon._Enemies.Add(f._playerModel);f.CKNCPOABFBO.Weapons.Add(weapon);f.LNDLFINJHDB.Add(weapon);
  var retiredWeapon=new Model{Owner=f._playerModel};retiredWeapon._Enemies.Add(f._playerModel);f._playerModel.Weapons.Add(retiredWeapon);f.LNDLFINJHDB.Add(retiredWeapon);
  stage=new FormRenderBindings(f,f._playerModel,next);
  Check(f.CKNCPOABFBO._Enemies[0]==next&&weapon._Enemies[0]==next&&weapon.Exchanges==1,"surviving fighter and weapon targeting exchanged once");
  Check(retiredWeapon.Exchanges==0,"retired weapon targeting is not mutated");
  stage.Dispose();Check(f.CKNCPOABFBO._Enemies[0]==f._playerModel&&weapon._Enemies[0]==f._playerModel,"targeting restored with registrations");
  weapon.RejectEnemy=true;failed=false;try{new FormRenderBindings(f,f._playerModel,next);}catch(InvalidOperationException){failed=true;}
  Check(failed&&f.CKNCPOABFBO._Enemies[0]==f._playerModel&&f._Camera.Current==f._playerModel,"later observer rejection restores earlier observers and camera");weapon.RejectEnemy=false;
  f._SelectAnimation.Reject=true;failed=false;try{new FormRenderBindings(f,f._playerModel,next);}catch(InvalidOperationException){failed=true;}
  Check(failed&&weapon._Enemies[0]==f._playerModel&&f.CKNCPOABFBO._Enemies[0]==f._playerModel,"selector rejection restores all enemy targets");f._SelectAnimation.Reject=false;
  stage=new FormRenderBindings(f,f._playerModel,next);stage.Commit();stage.Dispose();Check(weapon._Enemies[0]==next,"commit retains enemy targeting");
  f=new Fight();stage=new FormRenderBindings(f,f._playerModel,next);f.CKNCPOABFBO.RejectEnemyRestore=true;failed=false;try{stage.Dispose();}catch(AggregateException){failed=true;}
  Check(failed&&f._Camera.Current==f._playerModel&&f._SelectAnimation.Current==f._playerModel,"target rollback failure still attempts other registrations");
  Console.WriteLine("PASS: production form registration orchestration; preparation, staged exchange, commit, rollback, partial rejection and rollback-failure reporting. Camera/selector/rule services controlled.");
 }
}
'@
$code=$code.Replace('STAGE',$stage)
[IO.File]::WriteAllText((Join-Path $fixture 'Program.cs'),$code)
[IO.File]::WriteAllText((Join-Path $fixture 'Test.csproj'),'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>')
dotnet run --project (Join-Path $fixture 'Test.csproj') --verbosity quiet
if($LASTEXITCODE -ne 0){throw 'Form registration checks failed.'}
