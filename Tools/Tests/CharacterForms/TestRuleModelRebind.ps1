$ErrorActionPreference='Stop'
$root=Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$methods=@{}
foreach($name in @('RingOutRule','LoseFallRule','HotGroundRule','RulesInspector')){
 $source=Get-Content -Raw -LiteralPath (Join-Path $root "Assets/Scripts/Assembly-CSharp/$name.cs")
 $methods[$name]=[regex]::Match($source,'(?ms)^    internal (?:override )?System.Action PrepareModelRebind\(.*?^    \}').Value
 if(!$methods[$name]){throw "Rebind method missing: $name"}
}
$fixture=Join-Path $root ('Temp/RuleRebind-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$code=@'
using System;
using System.Collections.Generic;
class ModelNode{}
class ModelObject{public Dictionary<string,ModelNode> Nodes=new Dictionary<string,ModelNode>();public ModelNode EGHIDHMENEF(string name)=>Nodes.TryGetValue(name,out var n)?n:null;}
class Model{public ModelObject Body=new ModelObject();public ModelObject CLDMEJKGLBA()=>Body;}
class InFightRule{internal virtual Action PrepareModelRebind(Model a,Model b)=>null;}
class RingOutRule:InFightRule{public ModelNode _node;public string _nodeName="foot"; RING }
class LoseFallRule:InFightRule{public ModelNode _node;public string _nodeName="foot"; FALL }
class HotGroundRule:InFightRule{public class LimitedNode{public ModelNode node;public string name;}public List<LimitedNode> CFPIOKDFJCH=new List<LimitedNode>(); HOT }
class RulesInspector{public List<InFightRule> _inFightRules=new List<InFightRule>(); INSPECTOR }
class Program{
 static void Check(bool x,string why){if(!x)throw new Exception(why);}
 static void Main(){
  var old=new Model();old.Body.Nodes["foot"]=new ModelNode();old.Body.Nodes["hand"]=new ModelNode();
  var next=new Model();next.Body.Nodes["foot"]=new ModelNode();
  var other=new Model();other.Body.Nodes["foot"]=new ModelNode();
  var ring=new RingOutRule{_node=old.Body.Nodes["foot"]};var fall=new LoseFallRule{_node=old.Body.Nodes["foot"]};var untouched=new RingOutRule{_node=other.Body.Nodes["foot"]};
  var hot=new HotGroundRule();hot.CFPIOKDFJCH.Add(new HotGroundRule.LimitedNode{name="foot",node=old.Body.Nodes["foot"]});hot.CFPIOKDFJCH.Add(new HotGroundRule.LimitedNode{name="hand",node=old.Body.Nodes["hand"]});
  var inspector=new RulesInspector();inspector._inFightRules.AddRange(new InFightRule[]{ring,fall,untouched,hot});
  bool failed=false;try{inspector.PrepareModelRebind(old,next);}catch(InvalidOperationException e){failed=e.Message.Contains("hand");}
  Check(failed&&ring._node==old.Body.Nodes["foot"]&&fall._node==old.Body.Nodes["foot"]&&hot.CFPIOKDFJCH[0].node==old.Body.Nodes["foot"],"missing later node leaves all earlier rule bindings untouched");
  next.Body.Nodes["hand"]=new ModelNode();var commit=inspector.PrepareModelRebind(old,next);
  Check(ring._node==old.Body.Nodes["foot"],"preparation is read-only");commit();
  Check(ring._node==next.Body.Nodes["foot"]&&fall._node==next.Body.Nodes["foot"],"single-node rules rebound");
  Check(hot.CFPIOKDFJCH[0].node==next.Body.Nodes["foot"]&&hot.CFPIOKDFJCH[1].node==next.Body.Nodes["hand"],"all hot-ground nodes rebound");
  Check(untouched._node==other.Body.Nodes["foot"],"other fighter untouched");
  inspector.PrepareModelRebind(next,old)();Check(ring._node==old.Body.Nodes["foot"]&&hot.CFPIOKDFJCH[1].node==old.Body.Nodes["hand"],"reverse rebind");
  Console.WriteLine("PASS: production rule rebind preparation/commit; no partial changes on missing nodes, all three cached-node rules, other-fighter isolation and reverse binding. Native node services controlled.");
 }
}
'@
$code=$code.Replace('RING',$methods.RingOutRule).Replace('FALL',$methods.LoseFallRule).Replace('HOT',$methods.HotGroundRule).Replace('INSPECTOR',$methods.RulesInspector)
[IO.File]::WriteAllText((Join-Path $fixture 'Program.cs'),$code)
[IO.File]::WriteAllText((Join-Path $fixture 'Test.csproj'),'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>')
dotnet run --project (Join-Path $fixture 'Test.csproj') --verbosity quiet
if($LASTEXITCODE -ne 0){throw 'Rule rebind checks failed.'}
