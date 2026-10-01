$ErrorActionPreference='Stop'
$root=Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$source=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ListSF.cs')
$method=[regex]::Match($source,'(?ms)^    internal ModelParameters CreateFormParameters.*?^    \}').Value
if(!$method){throw 'Form parameter extraction failed.'}
$fixture=Join-Path $root ('Temp/FormParameters-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$code=@'
using System;
using System.Xml;
enum SceneTypes{SceneNone,SceneFight}
class ModelParameters{public bool IsPlayer;public SceneTypes IBBALIJOJMC;public XmlNode Node;}
class TemplateUser{public ModelParameters KEJDJHAGBMK=new ModelParameters();}
class ListSF{
METHOD
TemplateUser Template=new TemplateUser();public int Parsed,Merged;public ModelParameters Base;
TemplateUser CNFBCBDPKCI(string name)=>name=="native_staff"?Template:null;
ModelParameters IAOBIMJFBMH(XmlNode node,ModelParameters baseline){Parsed++;return new ModelParameters{Node=node};}
ModelParameters CNMFNFDIOOK(ModelParameters baseline,XmlNode node){Merged++;Base=baseline;return new ModelParameters{Node=node};}
static void Check(bool value,string message){if(!value)throw new Exception(message);}
static XmlNode Node(string text){var doc=new XmlDocument();doc.LoadXml(text);return doc.DocumentElement;}
static void Main(){
 var parser=new ListSF();var source=Node("<Warrior Template='native_staff' EclipseCharacterId='sample:form'><Items><Item Name='staff'/></Items></Warrior>");
 var form=parser.CreateFormParameters(source,true);
 Check(parser.Merged==1&&parser.Parsed==0&&parser.Base==parser.Template.KEJDJHAGBMK,"native template merge selected");
 Check(form!=parser.Template.KEJDJHAGBMK&&form.IsPlayer&&form.IBBALIJOJMC==SceneTypes.SceneFight&&!parser.Template.KEJDJHAGBMK.IsPlayer,"form side does not mutate template");
 Check(form.Node!=source&&form.Node.OwnerDocument!=source.OwnerDocument&&form.Node.OuterXml==source.OuterXml,"complete definition projected to independent document");
 form.Node["Items"]["Item"].Attributes["Name"].Value="selected";Check(source["Items"]["Item"].Attributes["Name"].Value=="staff","native selection cannot annotate catalog projection");
 var direct=parser.CreateFormParameters(Node("<Warrior EclipseCharacterId='sample:custom'/>"),false);
 Check(parser.Parsed==1&&!direct.IsPlayer&&direct.IBBALIJOJMC==SceneTypes.SceneFight,"custom template-free character parsing");
 bool failed=false;try{parser.CreateFormParameters(Node("<Warrior Template='missing'/>"),false);}catch(InvalidOperationException e){failed=e.Message.Contains("missing");}Check(failed&&parser.Parsed==1,"missing template rejected without silent empty fallback");
 failed=false;try{parser.CreateFormParameters(Node("<Fight/>"),false);}catch(ArgumentException){failed=true;}Check(failed,"wrong node rejected");
 Console.WriteLine("PASS: production form-parameter projection; native merge/direct routing, independent XML, side ownership and missing-template rejection. Native parsing/merge services controlled.");
}
}
'@
[IO.File]::WriteAllText((Join-Path $fixture 'Program.cs'),$code.Replace('METHOD',$method))
[IO.File]::WriteAllText((Join-Path $fixture 'Test.csproj'),'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>')
dotnet run --project (Join-Path $fixture 'Test.csproj') --verbosity quiet
if($LASTEXITCODE -ne 0){throw 'Form parameter checks failed.'}
