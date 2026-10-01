$ErrorActionPreference='Stop'
$root=Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$fight=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Fight.cs')
$model=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Model.cs')
$prepared=[regex]::Match($fight,'(?s)    internal sealed class PreparedFormModel.*?(?=    private sealed class PendingModelTransition)').Value
$dispose=[regex]::Match($model,'(?ms)^\tpublic void IMFOFFFLGOM\(\).*?^\t\}').Value
$clear=[regex]::Match($model,'(?ms)^\tprotected void Clear\(\).*?^\t\}').Value
$loader=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ModelLoader.cs')
$require=[regex]::Match($loader,'(?ms)^    internal static void RequireModelDocuments\(.*?^    \}').Value
if(!$prepared -or !$dispose -or !$clear -or !$require){throw 'Production preparation/cleanup extraction failed.'}
$fixture=Join-Path $root ('Temp/PreparedForm-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$code=@'
using System;
using System.Collections.Generic;
using System.Xml;
enum SceneTypes{Other,SceneFight}
class ModelParameters{public List<string> ModelDocuments=new List<string>(); public SceneTypes IBBALIJOJMC; public ModelParameters(){ModelDocuments.Add("body");} public ModelParameters(ModelParameters p){IBBALIJOJMC=p.IBBALIJOJMC;ModelDocuments.AddRange(p.ModelDocuments);}}
static class SF2Paths{public static string BNHLPKEDMOM()=>"fixture";}
static class ModelLoader{
 public static bool Missing;public static string Xml="<Scene><Figures/></Scene>";
 public sealed class Cache{public XmlDocument JBJDPDOEGFO(string root,string path){if(Missing)return null;var d=new XmlDocument();d.LoadXml(Xml);return d;}}
 public static Cache FHGHPCACAKJ=new Cache();
 REQUIRE
}
class Service{public bool Active=true;public void RemoveAllEventListener(){}public void Clear(){}public void Reset(){}public void SetActive(bool value){Active=value;}}
namespace Eclipse.Multiplayer.Rollback {
 static class RollbackObjects {
  public static bool Speculative;public static int Calls;
  public static bool Hide(Service body){Calls++;if(Speculative)body.SetActive(false);return Speculative;}
 }
}
static class Object{public static int Destroyed;public static void Destroy(Service s){Destroyed++;}}
class Model{
 public static Model Last;public static bool Fail;public ModelParameters Parameters;
 Service BNFCCKBIIDB=new Service(),_UnityObject=new Service(),_ModelConditions=new Service(),FEHOHLMIEBP=new Service();
 Service _ModelObject,_Collision,_Animation,_Physics,_Strike,HJOGNGDMAKJ,KDAHHIMLJGG;
 List<Model> _Enemies=new List<Model>(),JLDBGHLBJEL=new List<Model>();
 public Model(ModelParameters p){Last=this;Parameters=p;}
 public Service MJNPBMOAFML()=>_UnityObject;
 public void CGEKLPLKIDC(){if(Fail)throw new InvalidOperationException("load failed");_ModelObject=new Service();_Animation=new Service();KDAHHIMLJGG=new Service();}
 public void RemoveAllEventListener(){}
 DISPOSE
 CLEAR
}
class Fight{
 PREPARED
 static void Check(bool yes,string why){if(!yes)throw new Exception(why);}
 public static void Main(){
  var source=new ModelParameters();
  var p=new PreparedFormModel(source);var m=p.Model;
  Check(m!=null&&!m.MJNPBMOAFML().Active,"prepared hidden");
  Check(!ReferenceEquals(source,m.Parameters)&&source.IBBALIJOJMC==SceneTypes.Other&&m.Parameters.IBBALIJOJMC==SceneTypes.SceneFight,"isolated parameters");
  p.Dispose();p.Dispose();Check(Object.Destroyed==1&&p.Model==null,"dispose once");
  bool rejected=false;try{p.Take();}catch(InvalidOperationException){rejected=true;}Check(rejected,"no take after dispose");
  p=new PreparedFormModel(source);m=p.Take();p.Dispose();Check(Object.Destroyed==1,"transferred ownership survives wrapper");
  rejected=false;try{p.Take();}catch(InvalidOperationException){rejected=true;}Check(rejected,"take once");
  m.IMFOFFFLGOM();Check(Object.Destroyed==2,"owner cleanup");
  Model.Fail=true;rejected=false;try{new PreparedFormModel(source);}catch(InvalidOperationException e){rejected=e.Message=="load failed";}
  Check(rejected&&Object.Destroyed==3&&!Model.Last.MJNPBMOAFML().Active,"partial initialization cleaned and original failure preserved");
  Check(source.IBBALIJOJMC==SceneTypes.Other,"failure did not mutate source");
  ModelLoader.Missing=true;var last=Model.Last;rejected=false;try{new PreparedFormModel(source);}catch(System.IO.FileNotFoundException){rejected=true;}
  Check(rejected&&ReferenceEquals(last,Model.Last)&&Object.Destroyed==3,"missing assets reject before constructing a model");
  ModelLoader.Missing=false;
  foreach(var paths in new[]{new List<string>(),new List<string>{"assets/models/.xml"},new List<string>{""}}){rejected=false;try{ModelLoader.RequireModelDocuments(paths);}catch(System.IO.InvalidDataException){rejected=true;}Check(rejected,"empty document list/path");}
  ModelLoader.RequireModelDocuments(new List<string>{"assets/models/.xml","body","skin"});
  foreach(var xml in new[]{"<Other/>","<Scene/>"}){ModelLoader.Xml=xml;rejected=false;try{ModelLoader.RequireModelDocuments(new List<string>{"body"});}catch(System.IO.InvalidDataException){rejected=true;}Check(rejected,"missing structural elements");}
  ModelLoader.Xml="<Scene><Figures/></Scene>";Model.Fail=false;
  p=new PreparedFormModel(source);m=p.Model;int calls=Eclipse.Multiplayer.Rollback.RollbackObjects.Calls;
  Eclipse.Multiplayer.Rollback.RollbackObjects.Speculative=true;p.Dispose();p.Dispose();
  Check(Object.Destroyed==3&&!m.MJNPBMOAFML().Active&&p.Model==null,"speculative disposal hides body without destroying it");
  Check(Eclipse.Multiplayer.Rollback.RollbackObjects.Calls==calls+1,"speculative disposal delegates exactly once");
  Eclipse.Multiplayer.Rollback.RollbackObjects.Speculative=false;m.IMFOFFFLGOM();
  Check(Object.Destroyed==4,"confirmed cleanup destroys retained body");
  rejected=false;try{new PreparedFormModel(null);}catch(ArgumentNullException){rejected=true;}Check(rejected,"null destination");
  Console.WriteLine("PASS: production prepared-form ownership and Model cleanup with controlled loading/Unity/rollback services; hidden preparation, isolated parameters, transfer, cancellation cleanup, partial-load failure and speculative hide versus confirmed destruction.");
 }
}
'@
$code=$code.Replace('DISPOSE',$dispose).Replace('CLEAR',$clear).Replace('PREPARED',$prepared).Replace('REQUIRE',$require)
[IO.File]::WriteAllText((Join-Path $fixture 'Program.cs'),$code)
[IO.File]::WriteAllText((Join-Path $fixture 'Test.csproj'),'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><NoWarn>CS0649;CS0414</NoWarn></PropertyGroup></Project>')
dotnet run --project (Join-Path $fixture 'Test.csproj') --verbosity quiet
if($LASTEXITCODE -ne 0){throw 'Prepared form checks failed.'}
