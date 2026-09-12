# Execute recovered Run, Foreach entry and roster resume methods with observable host stubs.
$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
function ReadMethod([string]$file,[string]$signature) {
    $source=Get-Content -Raw -LiteralPath (Join-Path $root $file)
    $match=[regex]::Match($source,'(?ms)^\t'+[regex]::Escape($signature)+'\r?\n\t\{.*?^\t\}')
    if (!$match.Success) { throw "Method not found: $signature" }
    return $match.Value
}
$runSource=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/QuestAction.cs')
$run=[regex]::Match($runSource,'(?ms)^public class QuestActionRun : QuestAction\r?\n\{.*?^\}')
if (!$run.Success) { throw 'Run action not found' }
$each=ReadMethod 'Assets/Scripts/Assembly-CSharp/QuestActionForeach.cs' 'public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)'
$resume=ReadMethod 'Assets/Scripts/Assembly-CSharp/Roster.cs' 'public bool PBOFBNFALNN()'
$fixture=@'
using System;
using System.Collections.Generic;
using System.Xml;
public enum ScreenType { ModuleMap, ModuleDojo, ModuleFight }
public class QuestParameters {}
public class QuestAction {
 public int Completed;
 public virtual void Parse(XmlNode n){} public virtual void DEJMHFMLKIC(QuestParameters p){}
 public void OGIJONMKABB(){Completed++;}
}
public static class Debug { public static void LogWarning(string s){} }
public static class XmlExtensions { public static string CIPOICEEIBK(this XmlAttribute a,string d){return a==null?d:a.Value;} }
public class QuestStage {
 public string Name,File;public int Starts,Resets;public bool Unresumable;
 readonly List<Action<object>> listeners=new List<Action<object>>();
 public void AddEventListener(int id,Action<object> a){listeners.Add(a);} public void RemoveEventListener(int id,Action<object> a){listeners.Remove(a);}
 public void MHHNIPBJNAD(QuestParameters p,bool b){Starts++;foreach(var a in listeners.ToArray())a(this);}
 public bool IDGAAJAFCHC(){return Unresumable;} public void AGJGEBBLFGA(){Resets++;}
}
public class RosterQuest {
 public string Name,FileName;public int Deletes;public bool PLNNKKBPDJK;public int Scene=(int)ScreenType.ModuleMap;
 public QuestParameters Parameters=new QuestParameters();public QuestParameters get_Parameters(){return Parameters;}
 public void LCIHKPPGNPF(){Deletes++;Parameters=null;}public int ELBKKOPHLHK(){return Scene;}
}
public class Module {
 static readonly Module instance=new Module();public static int Changes;public static Module ELEBLBJKDBI(){return instance;}
 public ScreenType NMCNDOPKFJD(){return ScreenType.ModuleMap;} public static void DLOKJOHNDID(ScreenType s,int n=0){Changes++;}
}
public static class LLLOJBFMONN { public static void Error(string f,string n){throw new Exception(f+n);} }
public class ListSF {
 public static ListSF Current=new ListSF();public static ListSF ELEBLBJKDBI(){return Current;}
 public HashSet<string> Hidden=new HashSet<string>();public List<QuestStage> Definitions=new List<QuestStage>();
 public List<QuestStage> Queued=new List<QuestStage>();public Action<string> OnLoad;public int Loads;
 public bool IsEclipseQuestSuppressed(string n,string file=null){return Hidden.Contains((file??"quests.xml")+"#"+n);}
 public QuestStage PBGCEEBDBGG(string n){return Definitions.Find(q=>q.Name==n);}
 public QuestStage FindEclipseSavedQuest(string n,string f){return Definitions.Find(q=>q.Name==n&&q.File==f);}
 public void PDCHBPKOBFI(string f){Loads++;OnLoad?.Invoke(f);}public void FGAEEJBEGEJ(List<QuestStage> q){Queued.AddRange(q);}
}
/* RUN */
public class ForeachFixture:QuestAction {
 public enum PDIAEAEHHPE { FOREACH_ITEMS,FOREACH_DELIVERY_ITEMS,FOREACH_DELIVERY_UPGRADES,FOREACH_PAID_ITEMS,FOREACH_BATTLES,FOREACH_DELIVERY_ENCHANTMENTS }
 public string name="child";public PDIAEAEHHPE LFLGCDNKNJI;public int Touched;
 int index,PEEOEOMEBFG;QuestParameters NFIKJCJGMBB;QuestStage DOKAIKMLLDK;List<string> nodes=new List<string>();bool LNENABHHABO;
 void FGFAOPOODJA(){Touched++;}void RunDeliveryItems(bool b){Touched++;}void KINMIFFFGDA(){Touched++;}void AKFLMFMBPKD(){Touched++;}void OBAKPOLFCCP(){Touched++;}void AKPKHLBCOFB(){Touched++;}
 /* FOREACH */
}
public class ResumeFixture {
 public List<RosterQuest> CNFCPCJPGLM=new List<RosterQuest>();public bool Loaded=true;
 public bool JHHBKBENNNA(string f){return Loaded;}
 /* RESUME */
}
public static class ResumeTests {
 static int checks;static void Check(bool v,string s){checks++;if(!v)throw new Exception(s);}
 public static void Run(){
  var host=ListSF.Current;host.Hidden.Add("quests.xml#child");var child=new QuestStage{Name="child",File="quests.xml"};host.Definitions.Add(child);
  var doc=new XmlDocument();doc.LoadXml("<Run Name='child'/>");var run=new QuestActionRun();run.Parse(doc.DocumentElement);run.DEJMHFMLKIC(new QuestParameters());
  Check(run.Completed==1&&child.Starts==0,"suppressed Run did not complete without child execution");
  foreach(ForeachFixture.PDIAEAEHHPE kind in Enum.GetValues(typeof(ForeachFixture.PDIAEAEHHPE))){var each=new ForeachFixture{LFLGCDNKNJI=kind};each.DEJMHFMLKIC(new QuestParameters());Check(each.Completed==1&&each.Touched==0,"suppressed Foreach touched collection: "+kind);}
  var saved=new RosterQuest{Name="child",FileName="quests.xml",Scene=(int)ScreenType.ModuleFight};var parameters=saved.Parameters;child.Unresumable=true;
  var resume=new ResumeFixture();resume.CNFCPCJPGLM.Add(saved);
  Check(!resume.PBOFBNFALNN(),"suppressed saved quest resumed");Check(saved.Parameters==parameters&&saved.Deletes==0&&child.Resets==0&&!saved.PLNNKKBPDJK,"suppression changed saved checkpoint");Check(host.Queued.Count==0&&Module.Changes==0,"suppression changed queue/scene");
  host.Hidden.Clear();child.Unresumable=false;run.DEJMHFMLKIC(new QuestParameters());Check(run.Completed==2&&child.Starts==1,"cleared suppression did not run/finish child");
  Check(resume.PBOFBNFALNN()&&host.Queued.Count==1&&Module.Changes==1,"saved eligibility/scene not restored");
  host=ListSF.Current=new ListSF();host.Definitions.Add(new QuestStage{Name="shared",File="other.xml"});var correct=new QuestStage{Name="shared",File="story.xml"};host.Definitions.Add(correct);
  resume=new ResumeFixture();resume.CNFCPCJPGLM.Add(new RosterQuest{Name="shared",FileName="story.xml"});Check(resume.PBOFBNFALNN()&&object.ReferenceEquals(host.Queued[0],correct),"saved quest rebound to another file");
  host=ListSF.Current=new ListSF();host.OnLoad=f=>{host.Hidden.Add(f+"#lazy");host.Definitions.Add(new QuestStage{Name="lazy",File=f,Unresumable=true});};
  saved=new RosterQuest{Name="lazy",FileName="lazy.xml"};parameters=saved.Parameters;resume=new ResumeFixture{Loaded=false};resume.CNFCPCJPGLM.Add(saved);
  Check(!resume.PBOFBNFALNN()&&host.Loads==1&&host.Queued.Count==0,"lazy load bypassed suppression");Check(saved.Parameters==parameters&&saved.Deletes==0&&host.Definitions[0].Resets==0,"lazy suppression cleared saved progress");
  Console.WriteLine("PASS: "+checks+" recovered Run/Foreach/resume routing assertions (host services stubbed).");
 }
}
'@
$fixture=$fixture.Replace('/* RUN */',$run.Value).Replace('/* FOREACH */',$each).Replace('/* RESUME */',$resume)
Add-Type -TypeDefinition $fixture -IgnoreWarnings
[ResumeTests]::Run()
