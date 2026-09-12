$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$source=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ListSF.cs')
$methods=@()
foreach($name in @('PBNNPBEDOOJ','NHAMDLEDOHM','Reset')){
 $match=[regex]::Match($source,'(?ms)^\t(?:private|public static) [^\r\n]+ '+$name+'\(.*?^\t\}')
 if(!$match.Success){throw "Production profile method not found: $name"}
 $methods+=$match.Value
}
$fixture=@'
using System;
using System.Xml;
public static class Extensions {
 public static string CIPOICEEIBK(this XmlAttribute a,string fallback=""){return a==null?fallback:a.Value;}
 public static int ParseInt(this XmlAttribute a){return a==null?0:int.Parse(a.Value);}
}
public static class XmlUtils {public static XmlDocument Input; public static XmlDocument AIFIAKNJMHG(string a,string b)=>Input;}
public static class SF2Paths {public static string APHDBIBDMDG()=>"";}
public static class Constants {public static string OJMIJINKBPJ="";}
public static class GameUtils {public static string GetDefaultItem(string slot)=>"default";}
public class ModelParameters {public XmlNode Node;public void NOBKKLBJFIL(){}}
public class Inventory {public bool Ready;public void HOMCPNCGPDB(object x){Ready=true;}}
public class Roster {
 public XmlNode Node;public Inventory Inventory=new Inventory();public ModelParameters Parameters;
 public Roster(XmlNode node,ModelParameters parameters){Node=node;Parameters=parameters;
  if(NativeLoader.Loading && Eclipse.Modding.ModRuntime.Bound!=null)throw new Exception("Old profile remained bound during construction");}
 public Inventory KHCNHPCPFII()=>Inventory;
 public ModelParameters get_Parameters()=>Parameters;
 public void AddEventListener(int n,Action<object> callback){}
}
public class Items {public object HCDLKHKBEPF()=>null;public object KCCDBEEKBCG(string name)=>new object();}
public class GlobalTimer {public static GlobalTimer get_Instance()=>new GlobalTimer();public void removeEventListener(int n,Action<object> callback){}}
public static class QuestsManager {public static void Reset(){}}
namespace Eclipse.Modding {
 public static class ModSaveData {public static XmlNode CreateEquipmentView(XmlNode n,Func<string,bool> exists,Func<string,string> fallback)=>n;}
 public static class ModRuntime {
  public static Roster Bound;public static int Bindings,Unbindings;
  public static void UnbindProfile(){Bound=null;Unbindings++;}
  public static void RecordSaveContext(XmlNode node,Roster roster){
   if(!ReferenceEquals(roster,NativeLoader.Active)||!roster.Inventory.Ready)throw new Exception("Binding preceded native activation/inventory preparation");
   Bound=roster;Bindings++;
  }
 }
}
public class NativeLoader {
 public static bool Loading;
 private static NativeLoader _instance;
 private static Roster ANEHEDFAPCH;
 private static Items _items=new Items();
 private XmlDocument IEDEFCBFJAD;
 private XmlNode _CurrentUserNode;
 public static Roster Active=>ANEHEDFAPCH;
 private static Items DJBOFEEKJMP()=>_items;
 private int HFPJDOEEDCA()=>int.Parse(IEDEFCBFJAD.SelectSingleNode("Root/CurrentUser/@ID").Value);
 private ModelParameters IAOBIMJFBMH(XmlNode n,object template,bool flag)=>new ModelParameters{Node=n};
 private void EJANJEEGOOE(object data){}
 private void ILFBDHDMHPD(object data){}
 private void JMDJEEFELCD(XmlNode node){}
 /* METHODS */
 public void Load(){Loading=true;try{PBNNPBEDOOJ();}finally{Loading=false;}}
 public Roster Comparison(XmlNode node)=>NHAMDLEDOHM(node);
}
public static class ActivationChecks {
 public static void Run(){
  int count=0;Action<bool,string> check=(ok,msg)=>{count++;if(!ok)throw new Exception(msg);};
  var doc=new XmlDocument();doc.LoadXml("<Root><CurrentUser ID='1'/><Warriors><Warrior ID='1'/><Warrior ID='2'/></Warriors></Root>");
  XmlUtils.Input=doc;var loader=new NativeLoader();loader.Load();
  var first=NativeLoader.Active;
  check(ReferenceEquals(first,Eclipse.Modding.ModRuntime.Bound),"Active profile not bound");
  var secondNode=doc.SelectSingleNode("Root/Warriors/Warrior[@ID='2']");string before=doc.OuterXml;
  var comparison=loader.Comparison(secondNode);
  check(!ReferenceEquals(comparison,first)&&ReferenceEquals(first,Eclipse.Modding.ModRuntime.Bound),"Comparison roster stole active binding");
  check(Eclipse.Modding.ModRuntime.Bindings==1&&doc.OuterXml==before,"Comparison recorded context or changed saves");
  doc.SelectSingleNode("Root/CurrentUser/@ID").Value="2";loader.Load();
  check(ReferenceEquals(NativeLoader.Active,Eclipse.Modding.ModRuntime.Bound)&&!ReferenceEquals(first,Eclipse.Modding.ModRuntime.Bound),"Profile switch retained old binding");
  check(Eclipse.Modding.ModRuntime.Bindings==2&&Eclipse.Modding.ModRuntime.Unbindings==2,"Activation binding count/order changed");
  XmlUtils.Input=null;loader.Load();
  check(Eclipse.Modding.ModRuntime.Bound==null,"Missing profile file retained old binding");
  XmlUtils.Input=doc;loader.Load();NativeLoader.Reset();
  check(NativeLoader.Active==null&&Eclipse.Modding.ModRuntime.Bound==null,"Reset retained active profile");
  Console.WriteLine("PASS: "+count+" production profile activation/comparison/reset checks.");
 }
}
'@
$temp=Join-Path $root ('Temp/ProfileActivation-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $temp | Out-Null
$fixture.Replace('/* METHODS */',($methods -join [Environment]::NewLine)) | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $temp 'Fixture.cs')
Add-Type -Path (Join-Path $temp 'Fixture.cs') -CompilerOptions '/nowarn:0219,0649'
[ActivationChecks]::Run()
