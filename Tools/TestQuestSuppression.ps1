# Compile the complete production manager; stub scene/roster effects, not routing.
$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$manager=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Nekki/SF2/Core/Quests/QuestsManager.cs')
$events=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/QuestEvent.cs')
$enum=[regex]::Match($events,'(?s)public enum PMDPDMFLCIJ\s*\{.*?\}')
if (!$enum.Success) { throw 'Native quest enum not found' }
$compatibility=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Content/QuestCompatibility.cs')
$stamp=[regex]::Match($compatibility,'(?ms)^\t\tpublic static void StampQuestSource\(.*?^\t\t\}')
$merge=[regex]::Match($compatibility,'(?ms)^\t\tpublic static void AddQuestWithConditions\(.*?^\t\t\}')
if (!$stamp.Success -or !$merge.Success) { throw 'Quest provenance methods not found' }
$stageSource=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/QuestStage.cs')
$prepare=[regex]::Match($stageSource,'(?ms)^\tpublic void MHNEBBGMOLA\(.*?^\t\}')
if (!$prepare.Success) { throw 'Quest context capture method not found' }
$stubs=@'
using System;
using System.Collections.Generic;
namespace UnityEngine {
 public class Object { public static void Destroy(object x){} public static void DontDestroyOnLoad(object x){} }
 public class GameObject { public GameObject(string n){} public T AddComponent<T>() where T:new(){return new T();} }
 public class SerializeField:Attribute {}
}
namespace Nekki.SF2.GUI { public enum ScreenType { ModuleFight, Map } }
public class SFMonoBehaviour<T> { public void CallEvent(int e,object data){} }
public class ItemInfo {}
public class FightList {}
public static class LocalizationManager { public class Language {} }
public class ParametersQuest { public QuestParameters Context=new QuestParameters(); }
public class RosterQuest { public enum NOKCOAHJIPB { None } public int Deletes; public ParametersQuest Saved=new ParametersQuest(); public ParametersQuest get_Parameters(){return Saved;} public void LCIHKPPGNPF(){Deletes++;} }
public class Module { public static Module ELEBLBJKDBI(){return new Module();} public Nekki.SF2.GUI.ScreenType NMCNDOPKFJD(){return Nekki.SF2.GUI.ScreenType.Map;} }
public class NGOFBFGBICM { public static NGOFBFGBICM ELEBLBJKDBI(){return new NGOFBFGBICM();} public void HIHDEKHLHKP(string n){} }
public class ListSF { public static ListSF ELEBLBJKDBI(){return new ListSF();} public void EJANJEEGOOE(){} public static FightList CHMCKGCDGCM(FightIDS id){return null;} }
public class SystemProperties { public static bool DBBOCENKMGD(){return true;} }
public class LLLOJBFMONN { public static void Error(string s){throw new Exception(s);} }
public class QuestStage:IComparable<QuestStage> {
 public enum HPOLGFKCOOE { QUEST_UNCOMPLETE, QUEST_ACTIONS }
 public string Source="quests.xml", Container; public string EclipseSourceFile { get { return Source; } } public string EPDMGFELIMC(){return Container ?? Source;} public string Name; public bool allowDoubles; public int index, Compared, Prepared, Started, Restored;
 public RosterQuest Roster=new RosterQuest(); public HashSet<QuestEvent.PMDPDMFLCIJ> Events=new HashSet<QuestEvent.PMDPDMFLCIJ>();
 public string get_Name(){return Name;} public bool IsEvent(QuestEvent.PMDPDMFLCIJ e){return Events.Contains(e);}
 public QuestParameters EclipseQueuedParameters, StartedWith; public bool EclipseQueuedResume, StartedAsResume;
 private Checkpoint JAPJJHBDLKB; public QuestStage(){JAPJJHBDLKB=new Checkpoint(this);}
 private class Checkpoint { readonly QuestStage owner; public Checkpoint(QuestStage q){owner=q;} public void OIPDKFAJILO(QuestParameters p){owner.Prepared++;owner.Roster.Saved.Context=p.SnapshotForQueue();} }
 public bool Compare(QuestParameters p){Compared++;return true;}
 /* CAPTURE */
 public RosterQuest LBIPHHIJEFP(){return Roster;} public QuestParameters JMHGHCAGFDI(ParametersQuest p){Restored++;return p.Context.SnapshotForQueue();}
 public void MHHNIPBJNAD(QuestParameters p,bool b){Started++;StartedWith=p;StartedAsResume=b;}
 private Action<object> completed; public void AddEventListener(int n,Action<object> a){completed+=a;} public void RemoveEventListener(int n,Action<object> a){completed-=a;} public void Complete(){completed?.Invoke(this);}
 public HPOLGFKCOOE MHFPGCBLGIP(){return HPOLGFKCOOE.QUEST_UNCOMPLETE;} public bool IsGroup(List<string> g){return g.Contains(Name);}
 public int CompareTo(QuestStage q){return string.CompareOrdinal(Name,q.Name);}
}
public static class QuestSuppressionTests {
 static int checks; static void Check(bool b,string s){checks++;if(!b)throw new Exception(s);}
 static QuestStage Q(string name){return new QuestStage{Name=name};}
 public static void Run(){
  foreach(QuestEvent.PMDPDMFLCIJ e in Enum.GetValues(typeof(QuestEvent.PMDPDMFLCIJ))){
   if(e==QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_NONE)continue;
   var m=new Nekki.SF2.Core.Quests.QuestsManager(); var q=Q("original");q.Events.Add(e);m.AddQuest(q);
   m.SetEclipseSuppressedQuests(new[]{"quests.xml#original"});
   Check(!m.ActionQuest(e),"suppressed event returned active: "+e);
   Check(q.Compared==0 && q.Prepared==0,"suppressed quest touched native state: "+e);
   Check(object.ReferenceEquals(m.GetQuestByName("original"),q),"definition removed");
   Check(!m.AddQuestToStek("original",true),"explicit name bypass");
   Check(!m.AddQuestToStek(q),"explicit object bypass");
   var saved=new List<QuestStage>{q};Check(!m.AddActionQuest(saved),"saved queue bypass");
   Check(saved.Count==1 && q.Roster.Deletes==0 && q.Restored==0,"saved progress changed");
   m.SetEclipseSuppressedQuests(null);Check(m.ActionQuest(e),"disabled suppression not restored: "+e);
   Check(q.Compared==1 && q.Prepared==1,"restored quest did not queue");m.RunActionsAll();Check(q.Started==1,"restored quest did not run");
   try{m.SetEclipseSuppressedQuests(new[]{"quests.xml#original"});throw new Exception("active replacement accepted");}catch(InvalidOperationException){checks++;}
  }
  var same=new Nekki.SF2.Core.Quests.QuestsManager();var first=Q("shared");var second=Q("shared");second.Source="quest_extensions/other.xml";
  same.AddQuest(first);same.AddQuest(second);same.SetEclipseSuppressedQuests(new[]{"quests.xml#shared"});
  Check(!same.AddQuestToStek(first) && same.AddQuestToStek(second),"same-name file identity collision");
  Check(same.IsEclipseQuestSuppressed("shared","quests.xml") && !same.IsEclipseQuestSuppressed("shared","quest_extensions/other.xml"),"saved file identity lost");
  Check(object.ReferenceEquals(same.GetQuestByName("shared","quest_extensions/other.xml"),second),"saved lookup selected another source's same-name quest");
  Check(same.GetQuestByName("shared","missing.xml")==null,"saved lookup rebound a missing source by name");
  var included=new Nekki.SF2.Core.Quests.QuestsManager();var imported=Q("included");imported.Source="quest_extensions/story.xml";imported.Container="quests.xml";included.AddQuest(imported);
  included.SetEclipseSuppressedQuests(new[]{"quest_extensions/story.xml#included"});
  Check(included.IsEclipseQuestSuppressed("included","quests.xml") && !included.AddQuestToStek(imported),"flattened include provenance lost");
  Check(imported.EPDMGFELIMC()=="quests.xml","saved loader identity changed");
  var source=new System.Xml.XmlDocument();source.LoadXml("<Quests><Quest Name='included'><Actions/></Quest></Quests>");
  Provenance.StampQuestSource(source,"quest_extensions/story.xml");
  var output=new System.Xml.XmlDocument();output.LoadXml("<Quests/>");
  Provenance.AddQuestWithConditions(output,output.DocumentElement,source.DocumentElement.FirstChild,null);
  Check(output.DocumentElement.FirstChild.Attributes["EclipseSourceFile"].Value=="quest_extensions/story.xml","include clone lost provenance");
  Check(output.DocumentElement.FirstChild.Attributes["Name"].Value=="included","source annotation renamed quest");
  var mixed=new Nekki.SF2.Core.Quests.QuestsManager();var hidden=Q("hidden");var visible=Q("visible");
  mixed.SetEclipseSuppressedQuests(new[]{"quests.xml#hidden"});var incoming=new List<QuestStage>{hidden,visible};
  Check(mixed.AddActionQuest(incoming),"mixed queue failed");Check(visible.Restored==1 && visible.Prepared==0 && hidden.Prepared==0 && hidden.Restored==0,"wrong saved parameters restored or checkpoint rewritten");
  Check(incoming.Count==2 && hidden.Roster.Deletes==0,"caller queue mutated");
  mixed.ClearActions();try{mixed.SetEclipseSuppressedQuests(new[]{"new", ""});throw new Exception("empty accepted");}catch(ArgumentException){checks++;}
  Check(!mixed.AddQuestToStek(hidden),"failed configuration was not atomic");Check(mixed.AddQuestToStek(Q("Hidden")),"names are not ordinal");
  mixed.ClearEclipseQuestSuppression();Check(mixed.AddQuestToStek(hidden),"teardown did not restore eligibility alongside another queued quest");
  var queue=new Nekki.SF2.Core.Quests.QuestsManager();var a=Q("a");var b=Q("b");
  var original=new QuestParameters{JLGLBLDPAAF=new FightIDS("act|boss|1"),inLottery=true,FOODLENBJGI="reward",fightAvgFps=59.5f};
  original.DPLEGFCHOCE.OHCGEEEKEJH="purchase";original.DPLEGFCHOCE.BMNFPNBAMAF=17;
  queue.QuestParameters=original;queue.AddQuestToStek(a);
  original.JLGLBLDPAAF.SetFightIDSByString("act|other|2");original.FOODLENBJGI="changed";original.inLottery=false;
  original.DPLEGFCHOCE.OHCGEEEKEJH="changed";original.DPLEGFCHOCE.BMNFPNBAMAF=99;
  queue.QuestParameters=new QuestParameters{FOODLENBJGI="second"};queue.AddQuestToStek(b);queue.RunActionsAll();
  Check(a.StartedWith.inLottery&&a.StartedWith.FOODLENBJGI=="reward"&&a.StartedWith.JLGLBLDPAAF.ToString()=="act|boss|1","queued event overwritten before execution");
  Check(a.StartedWith.DPLEGFCHOCE.OHCGEEEKEJH=="purchase"&&a.StartedWith.DPLEGFCHOCE.BMNFPNBAMAF==17,"purchase payload aliased caller mutation");
  queue.QuestParameters.FOODLENBJGI="third";a.Complete();queue.Update();
  Check(b.StartedWith.FOODLENBJGI=="second","waiting quest consumed a later event");
  queue=new Nekki.SF2.Core.Quests.QuestsManager();a=Q("a");b=Q("b");
  a.Roster.Saved.Context=new QuestParameters{FOODLENBJGI="saved-a",inLottery=true};
  b.Roster.Saved.Context=new QuestParameters{FOODLENBJGI="saved-b",fightAvgFps=58};
  var latest=queue.QuestParameters;queue.AddActionQuest(new List<QuestStage>{a,b});
  Check(ReferenceEquals(queue.QuestParameters,latest),"resume replaced unrelated event context");
  Check(a.Roster.Saved.Context.FOODLENBJGI=="saved-a"&&b.Roster.Saved.Context.FOODLENBJGI=="saved-b","resume rewrote different checkpoints with first context");
  Check(a.Prepared==0&&b.Prepared==0,"restoring queue reran initial checkpoint");
  queue.RunActionsAll();Check(a.StartedAsResume&&a.StartedWith.inLottery&&a.StartedWith.FOODLENBJGI=="saved-a","first saved quest context/resume flag lost");
  a.Complete();queue.Update();Check(b.StartedAsResume&&b.StartedWith.FOODLENBJGI=="saved-b"&&b.StartedWith.fightAvgFps==58,"second saved quest used first context or restarted");
  var empty=new QuestParameters{JLGLBLDPAAF=null,DPLEGFCHOCE=null}.SnapshotForQueue();
  Check(empty.JLGLBLDPAAF==null&&empty.DPLEGFCHOCE==null,"null optional payload snapshot failed");
  Console.WriteLine("PASS: "+checks+" production quest routing checks; event/name/object/saved entry, restoration, atomic config and progress preservation.");
 }
}
'@
$fixture=Join-Path $root ('Temp/QuestSuppression-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$manager | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Manager.cs')
('public class QuestEvent { '+$enum.Value+' }') | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Events.cs')
$stubs.Replace('/* CAPTURE */',$prepare.Value) | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.cs')
('using System; using System.Xml; public static class Provenance {'+$stamp.Value+$merge.Value+'}') | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Provenance.cs')
Add-Type -Path @((Join-Path $fixture 'Manager.cs'),(Join-Path $fixture 'Events.cs'),(Join-Path $fixture 'Fixture.cs'),(Join-Path $fixture 'Provenance.cs'),(Join-Path $root 'Assets/Scripts/Assembly-CSharp/QuestParameters.cs'),(Join-Path $root 'Assets/Scripts/Assembly-CSharp/FightIDS.cs')) -IgnoreWarnings
[QuestSuppressionTests]::Run()
