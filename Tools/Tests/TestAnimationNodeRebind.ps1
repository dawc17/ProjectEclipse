$ErrorActionPreference='Stop'
$root=Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$source=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/DistancePoint.cs')
$fields=[regex]::Match($source,'(?s)public class DistancePoint\s*\{.*?(?=\tpublic DistancePoint\(\))').Value
$fields=[regex]::Replace($fields,'^public class DistancePoint\s*\{','')
$methods=[regex]::Match($source,'(?ms)^\tpublic void UpdateNode\(.*?^\t\}').Value
$methods+=[regex]::Match($source,'(?ms)^\tprotected PointNode MHIDGNCKHON\(.*?^\t\}').Value
$methods+=[regex]::Match($source,'(?ms)^\tprivate PointNode PAPCNMHMBOO\(.*?^\t\}').Value
if(!$fields -or !$methods){throw 'Native cache fields/methods missing.'}
$fixture=Join-Path $root ('Temp/AnimationNodeRebind-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$code=@'
using System;
using System.Collections.Generic;
class ModelNode{}
class ModelObject{public ModelNode Node=new ModelNode(),Ranged;public ModelNode EGHIDHMENEF(string name)=>name=="foot"?Node:name=="Ranged-Node2_1"?Ranged:null;}
class ModelType{public enum KEIDBIOIFGA{MODEL_NULL,MODEL_THIS,MODEL_OTHER,MODEL_OTHER_CHILD,MODEL_PARENT}}
class ModelConditions{public bool FDELMAHAAJD,IsPlayer;public Position IHJJBIDMEMB=new Position();public class Position{public ModelObject CBAECAAKAIA;}}
static class LLLOJBFMONN{public static void Error(string s,params object[] args){throw new Exception(s);}}
class DistancePoint{
 FIELDS
 METHODS
 static void Check(bool x,string why){if(!x)throw new Exception(why);}
 public static void Main(){
  var p=new DistancePoint{HLGJJGHDEAP=JJIAEPLMBFF.OBJECT_NODES,Part="foot",OOFFOILONLO=ModelType.KEIDBIOIFGA.MODEL_THIS};
  var left=new ModelObject();var right=new ModelObject();var next=new ModelObject();var pivot=new ModelNode();
  var l=new ModelConditions{IsPlayer=true};var r=new ModelConditions{IsPlayer=false};
  p.UpdateNode(left,true,pivot,false,left);p.UpdateNode(right,false,null,false,right);
  Check(p.MHIDGNCKHON(l).Node==left.Node&&p.MHIDGNCKHON(r).Node==right.Node,"initial sides");
  p.UpdateNode(next,true,null,false,next);
  Check(p.MHIDGNCKHON(l).Node==next.Node&&p.MHIDGNCKHON(r).Node==right.Node,"replacement changes only its side");
  p.OOFFOILONLO=ModelType.KEIDBIOIFGA.MODEL_OTHER;
  Check(p.MHIDGNCKHON(r).Node==next.Node&&p.MHIDGNCKHON(l).Node==right.Node,"opponent lookup follows new side");
  p.UpdateNode(left,true,pivot,false,left);
  Check(p.MHIDGNCKHON(r).Node==left.Node&&p.MHIDGNCKHON(r).CHEKEGGJDBL==pivot,"old node and pivot restored");
  var child1=new ModelObject();var child2=new ModelObject();
  p.UpdateNode(child1,true,null,true,child1);p.UpdateNode(child2,true,null,true,child2);
  p.OOFFOILONLO=ModelType.KEIDBIOIFGA.MODEL_THIS;l.FDELMAHAAJD=true;l.IHJJBIDMEMB.CBAECAAKAIA=child1;
  Check(p.MHIDGNCKHON(l).Node==child1.Node,"child identity 1");l.IHJJBIDMEMB.CBAECAAKAIA=child2;Check(p.MHIDGNCKHON(l).Node==child2.Node,"child identity 2");
  p.UpdateNode(next,true,null,false,next);Check(p.MHIDGNCKHON(l).Node==child2.Node,"main form change does not replace child cache");
  l.FDELMAHAAJD=false;
  var ranged=new DistancePoint{HLGJJGHDEAP=JJIAEPLMBFF.OBJECT_NODES,Part="Ranged-Node2_1",OOFFOILONLO=ModelType.KEIDBIOIFGA.MODEL_THIS};
  left.Ranged=new ModelNode();right.Ranged=new ModelNode();
  ranged.UpdateNode(left,true,pivot,false,left);ranged.UpdateNode(right,false,null,false,right);
  ranged.UpdateNode(child1,true,pivot,true,child1);
  ranged.UpdateNode(next,true,null,false,next);
  Check(ranged.MHIDGNCKHON(l).Node==null&&ranged.MHIDGNCKHON(l).CHEKEGGJDBL==null,"absent optional weapon point clears retired node and pivot instead of rejecting the form");
  Check(ranged.MHIDGNCKHON(r).Node==right.Ranged&&p.MHIDGNCKHON(l).Node==next.Node,"optional node absence leaves opponent and present body node bindings intact");
  ranged.UpdateNode(left,true,pivot,false,left);
  Check(ranged.MHIDGNCKHON(l).Node==left.Ranged&&ranged.MHIDGNCKHON(l).CHEKEGGJDBL==pivot,"rollback restores an optional weapon binding");
  l.FDELMAHAAJD=true;l.IHJJBIDMEMB.CBAECAAKAIA=child1;
  Check(ranged.MHIDGNCKHON(l).Node==null&&ranged.MHIDGNCKHON(l).CHEKEGGJDBL==pivot,"missing optional child point stays isolated from main form and opponent");
  l.FDELMAHAAJD=false;
  p.UpdateNode(new ModelObject{Node=null},true,null,false,null);Check(p.MHIDGNCKHON(l).Node==null,"native missing-node update is silent");
  p.UpdateNode(left,true,pivot,false,left);Check(p.MHIDGNCKHON(l).Node==left.Node,"restore after missing node");
  Console.WriteLine("PASS: production DistancePoint cache fields/update/lookup; optional ranged-node absence clears retired caches, present body/opponent/child bindings stay isolated, reverse restoration and pivot identity. Named points may be absent for unused/shared moves; validation of a selected animation's required rig remains separate.");
 }
}
'@
$code=$code.Replace('FIELDS',$fields).Replace('METHODS',$methods)
[IO.File]::WriteAllText((Join-Path $fixture 'Program.cs'),$code)
[IO.File]::WriteAllText((Join-Path $fixture 'Test.csproj'),'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><NoWarn>CS0649</NoWarn></PropertyGroup></Project>')
dotnet run --project (Join-Path $fixture 'Test.csproj') --verbosity quiet
if($LASTEXITCODE -ne 0){throw 'Native animation cache checks failed.'}
