$ErrorActionPreference='Stop'
$root=Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$source=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Fight.cs')
$method=[regex]::Match($source,'(?ms)^    internal void CommitPreparedForm.*?^    \}').Value
if(!$method){throw 'Form commit extraction failed.'}
$fixture=Join-Path $root ('Temp/FormCommit-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$code=@'
using System;
using System.Collections.Generic;
namespace UnityEngine{static class Debug{public static int Errors;public static void LogException(Exception e){Errors++;}}}
class Surface{public bool activeSelf=true,FailNext;public void SetActive(bool value){if(FailNext){FailNext=false;throw new Exception("visibility");}activeSelf=value;}}
class Model{public Model Owner;public Surface Surface=new Surface();public List<Model> Children=new List<Model>();public int Disposals;public bool FailDispose;public Model BDJBNOPNCNB()=>Owner==null?this:Owner.BDJBNOPNCNB();public List<Model>KGGIDBLBMDJ()=>Children;public Surface MJNPBMOAFML()=>Surface;public void FKIBECCHIJC(){}public void IMFOFFFLGOM(){Disposals++;if(FailDispose)throw new Exception("cleanup");}}
class Perks{public bool Reject;public HashSet<Model> Seen;public void RequireFormReferencesTransferred(ISet<Model> retired){Seen=new HashSet<Model>(retired);if(Reject)throw new InvalidOperationException("effect still references old body");}}
class Fight{
METHOD
internal class PreparedFormModel:IDisposable{public Model Model;public Model Take(){var model=Model;Model=null;return model;}public void Dispose(){if(Model!=null){Model.IMFOFFFLGOM();Model=null;}}}
internal class FormRenderBindings{public Fight Fight;public Model Old,Next;public bool Committed;public bool Owns(Fight fight,Model old,Model next)=>!Committed&&Fight==fight&&Old==old&&Next==next;public void Commit(){Committed=true;}}
Model _playerModel,CKNCPOABFBO=new Model();HashSet<Model> _retiredFormBodies=new HashSet<Model>();
List<Model> LNDLFINJHDB=new List<Model>(),HCPGFOCGDAA=new List<Model>(),JLEFIKJODGG=new List<Model>();Perks EPBDEDGLHJE=new Perks();
void RemoveModel(Model model){model.IMFOFFFLGOM();}
static void Check(bool value,string message){if(!value)throw new Exception(message);}
static void Main(){
 foreach(bool visible in new[]{true,false}){
  var f=new Fight();var old=new Model();old.Surface.activeSelf=visible;var next=new Model();next.Surface.activeSelf=false;f._playerModel=next;
  var child=new Model{Owner=old};var nested=new Model{Owner=child};old.Children.Add(child);child.Children.Add(nested);
  f.LNDLFINJHDB.AddRange(new[]{next,f.CKNCPOABFBO,child});f.HCPGFOCGDAA.Add(nested);f.JLEFIKJODGG.Add(child);
  var p=new PreparedFormModel{Model=next};var bindings=new FormRenderBindings{Fight=f,Old=old,Next=next};
  f.EPBDEDGLHJE.Reject=true;bool failed=false;try{f.CommitPreparedForm(old,p,bindings);}catch(InvalidOperationException){failed=true;}
  Check(failed&&!bindings.Committed&&p.Model==next&&old.Disposals==0&&old.Surface.activeSelf==visible,"reference rejection before visibility/ownership/disposal");
  f.EPBDEDGLHJE.Reject=false;next.Surface.FailNext=true;failed=false;try{f.CommitPreparedForm(old,p,bindings);}catch(Exception){failed=true;}
  Check(failed&&p.Model==next&&!bindings.Committed&&!next.Surface.activeSelf&&old.Surface.activeSelf==visible,"visibility failure keeps preparation and old body intact");
  f.CommitPreparedForm(old,p,bindings);p.Dispose();
  Check(bindings.Committed&&p.Model==null&&next.Disposals==0&&next.Surface.activeSelf==visible,"active body ownership and invisibility");
  Check(old.Disposals==1&&child.Disposals==1&&nested.Disposals==1,"body and nested helpers cleaned once");
  Check(f.LNDLFINJHDB.Count==2&&f.HCPGFOCGDAA.Count==0&&f.JLEFIKJODGG.Count==0,"retired entries removed from simulation queues");
  Check(f.EPBDEDGLHJE.Seen.Count==3,"reference preflight covers nested helpers");
  failed=false;try{f.CommitPreparedForm(old,p,bindings);}catch(InvalidOperationException){failed=true;}
  Check(failed&&old.Disposals==1&&next.Disposals==0,"duplicate commit rejected");
 }
 var cleanup=new Fight();var retired=new Model{FailDispose=true};var active=new Model();cleanup._playerModel=active;
 var prepared=new PreparedFormModel{Model=active};var registration=new FormRenderBindings{Fight=cleanup,Old=retired,Next=active};
 cleanup.CommitPreparedForm(retired,prepared,registration);prepared.Dispose();
 Check(UnityEngine.Debug.Errors==1&&registration.Committed&&active.Disposals==0,"post-commit cleanup failure cannot dispose or reject replacement");
 Console.WriteLine("PASS: production form commit; reference/visibility rejection, invisibility, ownership transfer, helper/queue retirement, duplicate commit and cleanup failure. Native object and registration services controlled.");
}
}
'@
[IO.File]::WriteAllText((Join-Path $fixture 'Program.cs'),$code.Replace('METHOD',$method))
[IO.File]::WriteAllText((Join-Path $fixture 'Test.csproj'),'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>')
dotnet run --project (Join-Path $fixture 'Test.csproj') --verbosity quiet
if($LASTEXITCODE -ne 0){throw 'Form commit checks failed.'}
