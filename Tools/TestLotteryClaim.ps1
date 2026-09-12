$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
foreach ($relative in @('Runtime/Modding/ModQuestInvocationLedger','Runtime/Modding/ModProfileWriteJournal','Modding/ModLotteryPrizeCodec','Modding/ModQuestLotteryAction','UI/Modding/ModLotteryDialog')) {
 $meta=Get-Content -Raw -LiteralPath (Join-Path $root ('Assets/Scripts/Eclipse/'+$relative+'.cs.meta'))
 if($meta -notmatch '(?m)^guid: [0-9a-f]{32}\r?$'){throw "Invalid Unity script GUID: $relative"}
}
$fixture=Join-Path $root ('Temp/LotteryClaim-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$source=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModRuntime.cs')
$claim=[regex]::Match($source,'(?ms)^        internal sealed class LotteryClaim.*?^        \}')
$prepare=[regex]::Match($source,'(?ms)^        internal static LotteryClaim PrepareLotteryClaim\(.*?^        \}')
if(!$claim.Success -or !$prepare.Success){throw 'Lottery claim methods not found.'}
$resume=[regex]::Match($source,'(?ms)^        internal static LotteryClaim ResumeLotteryClaim\(.*?^        \}')
$defer=[regex]::Match($source,'(?ms)^        internal static bool DeferProfileSave\(.*?^        \}')
$ack=[regex]::Match($source,'(?ms)^        private static Action ResolveLotteryAcknowledgement\(.*?^        \}')
$questRun=[regex]::Match($source,'(?ms)^        internal static ModQuestInvocationLedger GetQuestLotteryInvocation\(.*?^        \}')
$questComplete=[regex]::Match($source,'(?ms)^        internal static bool CompleteQuestLotteryRun\(.*?^        \}')
$resolveQuest=[regex]::Match($source,'(?ms)^        internal static LotteryClaim PrepareQuestLotteryClaim\(.*?^        \}')
$saveContext=[regex]::Match($source,'(?ms)^        internal static void SaveQuestLotteryContext\(.*?^        \}')
$restoreContext=[regex]::Match($source,'(?ms)^        internal static void RestoreQuestLotteryContext\(.*?^        \}')
$battlePrepare=[regex]::Match($source,'(?ms)^        internal static void PrepareBattleLottery\(.*?^        \}')
$battleComplete=[regex]::Match($source,'(?ms)^        internal static void CompleteBattleLottery\(.*?^        \}')
$pendingLottery=[regex]::Match($source,'(?ms)^        internal static bool HasPendingLottery =>.*?;')
if(!$battlePrepare.Success -or !$battleComplete.Success -or !$pendingLottery.Success){throw 'Battle lottery continuation bridge missing.'}
if(!$saveContext.Success -or !$restoreContext.Success){throw 'Quest checkpoint context bridge missing.'}
if(!$resolveQuest.Success){throw 'Quest lottery resolution missing.'}
if(!$questRun.Success -or !$questComplete.Success){throw 'Native quest lottery run bridge missing.'}
if(!$resume.Success -or !$defer.Success){throw 'Lottery recovery/save gate missing.'}
$program=@'
using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using Eclipse.Modding;
static class LocalizationManager {public static string GetStringOrDefault(string key,string fallback,params string[] args)=>fallback;}
namespace UnityEngine {static class Random {public static float value=.5f;}}
class Program {
 public class Roster {public int Saves;public int PINDEKDNCNL()=>4;public void GGGEHAGCLGC(bool immediate){Saves++;}}
 static readonly ModStoryEvents StoryEvents=new ModStoryEvents();
 public class FightResult {public class ResultPrizeStruct {public object FAPDEKOMOGH;public int Token;public System.Collections.Generic.List<object> KBMDJACLAOH=new System.Collections.Generic.List<object>();}}
 public class RewardLottery {}
 public class ParametersQuest {public XmlNode Node;}
 public class QuestParameters {public bool inLottery;public int EGAPDJLHHNJ,BJIDALJIKNC;public string FOODLENBJGI,OHPHPJBMNLH,HEIADONEACH;public FightIDS JLGLBLDPAAF;public float fightAvgFps;}
 public class FightIDS {public string Name;public void SetFightIDSByString(string n){Name=n;}public override string ToString()=>Name;}
 class ModQuestLotteryAction {public void Dispose(){}}static ModQuestLotteryAction _battleLotteryPresentation;
 public class RewardPrize {public RewardLottery FAPDEKOMOGH;}
 public class RewardStruct {public RewardLottery Lottery;public int Evaluations;public RewardPrize KOBOIFJNPMO(int level){Evaluations++;return new RewardPrize{FAPDEKOMOGH=Lottery};}}
 public class FightList {public List<RewardStruct> Rewards=new List<RewardStruct>();public List<RewardStruct> APKPCGDBMEP()=>Rewards;}
 public class QuestStage {public bool allowDoubles,EclipseResumeActions;public string FileName="quests.xml",EclipseActionsDefinition="<Actions><DialogLottery/></Actions>";public ModQuestInvocationLedger EclipseLotteryInvocations;public string get_Name()=>"LotteryQuest";}
 public struct MANJCIGJPMK {public string Image=>"test";public string ViewType=>"Weapon";}
 public class ItemInfo {public int MHGODOLNDLE,OBJDGBBFJOO;public ItemInfo HIOBANJPMKF(int n)=>this;}
 public class Catalog {public ItemInfo KCCDBEEKBCG(string n)=>null;public object ICFINJLNCPM(string n)=>null;public object NDMEGBEFBPJ(string n)=>null;}
 public static class GameUtils {public static Catalog AJDKHINLIDI=new Catalog(),JNIMKHKGPHE=new Catalog();}
 public partial class ListSF {public static FightList Fight;public static FightList CHMCKGCDGCM(FightIDS id)=>id.Name=="test"?Fight:null;public static Action Grant;public static int Grants,Writes;public static string Saved;public static Catalog DJBOFEEKJMP()=>new Catalog();static ListSF current=new ListSF();public static ListSF ELEBLBJKDBI()=>current;public bool IMDGMNFHFCN(FightResult.ResultPrizeStruct p){Grants++;Grant?.Invoke();return false;}public void OnAuthenticate(bool force){if(DeferProfileSave())return;Writes++;Saved=_lotteryProfileNode.OwnerDocument.OuterXml;}}
 public partial class ListSF {public QuestParameters HAOHNNFLOGK;public static Action Queue;public static int Queued,Runs;public static bool Raid;public static QuestParameters Context;public bool QueueLotteryFightEnd(QuestParameters context,bool raid){Queued++;Context=context;Raid=raid;Queue?.Invoke();return true;}public void MHHNIPBJNAD(){Runs++;}}
 public static class ModLotteryPrizeCodec {
  public static XmlElement Write(XmlDocument d,FightResult.ResultPrizeStruct p){var e=d.CreateElement("Prize");e.SetAttribute("Token",p.Token.ToString());return e;}
  public static FightResult.ResultPrizeStruct Read(XmlElement e,Func<string,int,int,ItemInfo> i,Func<string,object> c,Func<string,object> r)=>new FightResult.ResultPrizeStruct{Token=int.Parse(e.GetAttribute("Token"))};
 }
 static XmlNode _lotteryProfileNode;static int _lotterySaveState;
 static Roster _profileRoster;static bool Available=true;static int Builds;static Action BuildAction;
 static bool TrySelectLotterySlot(RewardLottery l,int level,double sample,Func<MANJCIGJPMK,bool> predicate,out MANJCIGJPMK slot){slot=new MANJCIGJPMK();return Available&&(predicate==null||predicate(slot));}
 static FightResult.ResultPrizeStruct BuildLotteryPrize(MANJCIGJPMK slot,int level){Builds++;BuildAction?.Invoke();return new FightResult.ResultPrizeStruct{Token=Builds};}
 /* CLAIM */
 /* PREPARE */
 /* RESUME */
 /* DEFER */
 /* ACK */
 /* QUEST RUN */
 /* QUEST COMPLETE */
 /* QUEST RESOLVE */
 /* SAVE CONTEXT */
 /* RESTORE CONTEXT */
 /* BATTLE PREPARE */
 /* BATTLE COMPLETE */
 /* PENDING LOTTERY */
 static int checks;static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
 static void Reject(Action action,string message){try{action();}catch(InvalidOperationException){checks++;return;}throw new Exception(message);}
 static void Reset(){_profileRoster=new Roster();StoryEvents.Clear();StoryEvents.BindProfile();ListSF.Grant=null;ListSF.Grants=0;ListSF.Writes=0;ListSF.Saved=null;Builds=0;BuildAction=null;Available=true;_lotterySaveState=0;var d=new XmlDocument();d.LoadXml("<Warrior/>");_lotteryProfileNode=d.DocumentElement;}
 static LotteryClaim Prepare()=>PrepareLotteryClaim(new RewardLottery(),0.5,null);
 static void Main(){
  Reset();var claim=Prepare();Check(Builds==1&&ListSF.Grants==0,"Preparation granted/repeated draw");Check(claim.TryClaim()&&ListSF.Grants==1&&_profileRoster.Saves==1,"False level-up return treated as failure");Check(!claim.TryClaim()&&ListSF.Grants==1,"Repeated claim");
  Reset();claim=Prepare();ListSF.Grant=()=>Check(!claim.TryClaim(),"Reentrant claim");Check(claim.TryClaim()&&ListSF.Grants==1,"Reentrant native grant");
  Reset();claim=Prepare();_profileRoster=new Roster();Check(!claim.TryClaim()&&ListSF.Grants==0,"Cross-profile claim");
  Reset();claim=Prepare();StoryEvents.BindProfile();Check(!claim.TryClaim()&&ListSF.Grants==0,"Rebound same profile claim");
  Reset();claim=Prepare();ListSF.Grant=()=>{throw new InvalidOperationException("native failure");};Reject(()=>claim.TryClaim(),"Native error hidden");ListSF.Grant=null;Check(!claim.TryClaim()&&ListSF.Grants==1&&_profileRoster.Saves==0,"Failed grant retried/saved");
  Reset();claim=new LotteryClaim(_profileRoster,StoryEvents.ProfileGeneration,new FightResult.ResultPrizeStruct{FAPDEKOMOGH=new object()});Reject(()=>claim.TryClaim(),"Nested lottery dropped");Check(ListSF.Grants==0,"Nested lottery partially granted");
  Reset();Available=false;Check(Prepare()==null&&Builds==0,"Empty pool built reward");
  Reset();BuildAction=()=>StoryEvents.BindProfile();Reject(()=>Prepare(),"Stale preparation retained");Check(ListSF.Grants==0,"Preparation mutated inventory");
  Reset();_profileRoster=null;Reject(()=>Prepare(),"Unbound preparation");
  Reset();claim=Prepare();var owner=_profileRoster;ListSF.Grant=()=>{_profileRoster=new Roster();StoryEvents.BindProfile();};Reject(()=>claim.TryClaim(),"Mid-grant profile replacement hidden");Check(owner.Saves==0&&_profileRoster.Saves==0&&!claim.TryClaim(),"Mid-grant replacement saved/retried");
  Reset();claim=Prepare();int observed=0;
  StoryEvents.CreateScope(ModId.Parse("example.claim")).Subscribe(ModStoryEventKind.ItemAcquired,e=>{Check(_profileRoster.Saves==1,"Acquisition ran before bundle save request");observed++;Check(!claim.TryClaim(),"Deferred callback reclaimed");});
  ListSF.Grant=()=>{StoryEvents.Publish(new ModStoryEvent(ModStoryEventKind.ItemAcquired,null,previousCount:0,count:1));Check(observed==0,"Acquisition escaped grant boundary");};
  Check(claim.TryClaim()&&observed==1,"Deferred acquisition missing");
  Reset();claim=Prepare();observed=0;StoryEvents.CreateScope(ModId.Parse("example.claim")).Subscribe(ModStoryEventKind.ItemAcquired,e=>observed++);
  ListSF.Grant=()=>{StoryEvents.Publish(new ModStoryEvent(ModStoryEventKind.ItemAcquired,null,previousCount:0,count:1));throw new InvalidOperationException("partial grant");};Reject(()=>claim.TryClaim(),"Failed bundle completed");Check(observed==0,"Failed bundle published acquisition");
  Reset();claim=Prepare();Check(ListSF.Writes==1&&ListSF.Saved.Contains("prepared"),"Draw not saved before returning");
  var second=Prepare();Check(Builds==1,"Pending draw rerolled");
  var reloaded=new XmlDocument();reloaded.LoadXml(ListSF.Saved);_lotteryProfileNode=reloaded.DocumentElement;_profileRoster=new Roster();StoryEvents.BindProfile();
  var restored=ResumeLotteryClaim();Check(restored!=null&&Builds==1,"Reload rebuilt random reward");
  ListSF.Grant=()=>{ListSF.ELEBLBJKDBI().OnAuthenticate(true);Check(!ListSF.Saved.Contains("claimed"),"Native callback saved partial settlement");};
  Check(restored.TryClaim()&&ListSF.Saved.Contains("claimed")&&ResumeLotteryClaim()==null,"Completion not persisted");
  Check(!claim.TryClaim()&&!second.TryClaim(),"Old profile handles survived reload");
  Reset();claim=Prepare();string prepared=ListSF.Saved;ListSF.Grant=()=>{ListSF.ELEBLBJKDBI().OnAuthenticate(true);throw new InvalidOperationException("partial");};
  Reject(()=>claim.TryClaim(),"Failed settlement accepted");Reject(()=>ListSF.ELEBLBJKDBI().OnAuthenticate(true),"Failed settlement allowed autosave");Check(ListSF.Saved==prepared,"Failed settlement overwrote prepared save");
  Reset();var quest=_lotteryProfileNode.OwnerDocument.CreateElement("Quest");_lotteryProfileNode.AppendChild(quest);var ledger=new ModQuestInvocationLedger(quest,false);
  Check(quest.ChildNodes.Count==0,"Unused ledger eagerly changed profile");
  string operation=ledger.Operation(3);claim=PrepareLotteryClaim(new RewardLottery(),0.5,null,ledger,3);
  Check(!ledger.IsCompleted(3)&&ListSF.Saved.Contains("Pending"),"Prepared receipt missing");
  Reject(()=>new ModQuestInvocationLedger(quest,false).Operation(3),"New run replaced pending claim");
  Check(claim.TryClaim()&&ledger.IsCompleted(3)&&ListSF.Saved.Contains("Completed"),"Receipt not saved with claim");
  Check(PrepareLotteryClaim(new RewardLottery(),0.1,null,ledger,3)==null&&Builds==1,"Acknowledged action redrew prize");
  reloaded=new XmlDocument();reloaded.LoadXml(ListSF.Saved);_lotteryProfileNode=reloaded.DocumentElement;_profileRoster=new Roster();StoryEvents.BindProfile();
  var resumedLedger=new ModQuestInvocationLedger((XmlElement)_lotteryProfileNode["Quest"],true);
  Check(resumedLedger.Operation(3)==operation&&resumedLedger.IsCompleted(3),"Quest receipt lost after XML reload");
  Check(PrepareLotteryClaim(new RewardLottery(),0.8,null,resumedLedger,3)==null&&Builds==1,"Replayed quest drew twice");
  var nextRun=new ModQuestInvocationLedger((XmlElement)_lotteryProfileNode["Quest"],false);
  Check(nextRun.Operation(3)!=operation&&!nextRun.IsCompleted(3),"New completed quest run reused old receipt");
  Reset();var stage=new QuestStage();ledger=GetQuestLotteryInvocation(stage);operation=ledger.Operation(0);
  claim=PrepareLotteryClaim(new RewardLottery(),0.5,null,ledger,0);
  Reject(()=>CompleteQuestLotteryRun(stage),"Quest completed while reward pending");
  Check(GetQuestLotteryInvocation(new QuestStage()).Operation(0)==operation,"Reopened unresumable quest rerolled run");
  Check(claim.TryClaim(),"Native quest reward failed");
  reloaded=new XmlDocument();reloaded.LoadXml(ListSF.Saved);_lotteryProfileNode=reloaded.DocumentElement;_profileRoster=new Roster();StoryEvents.BindProfile();
  var reopened=new QuestStage();ledger=GetQuestLotteryInvocation(reopened);
  Check(ledger.Operation(0)==operation&&ledger.IsCompleted(0),"Crash before quest completion lost receipt");
  Reject(()=>GetQuestLotteryInvocation(stage),"Prior profile quest handle reused");
  Check(CompleteQuestLotteryRun(reopened),"Native quest run failed to close");
  Check(GetQuestLotteryInvocation(new QuestStage{EclipseResumeActions=true}).IsCompleted(0),"Checkpoint replay lost completed action");
  Check(CompleteQuestLotteryRun(reopened),"Repeated close failed");
  Check(GetQuestLotteryInvocation(new QuestStage()).Operation(0)!=operation,"New completed quest run retained old draw");
  Reject(()=>GetQuestLotteryInvocation(new QuestStage{EclipseActionsDefinition="changed"}),"Changed action ordering reused receipt");
  try {GetQuestLotteryInvocation(new QuestStage{allowDoubles=true});throw new Exception("Concurrent quest identity accepted");}catch(NotSupportedException){checks++;}
  Reset();stage=new QuestStage();ListSF.Fight=new FightList();var reward=new RewardStruct{Lottery=new RewardLottery()};ListSF.Fight.Rewards.Add(reward);
  claim=PrepareQuestLotteryClaim(stage,0,"test",0.5);Check(claim!=null&&Builds==1&&reward.Evaluations==1,"Quest did not resolve one draw");
  Check(PrepareQuestLotteryClaim(stage,0,"missing-after-reload",0.9)!=null&&Builds==1&&reward.Evaluations==1,"Pending reward re-evaluated changed source");
  Reject(()=>PrepareQuestLotteryClaim(stage,1,"test",0.5),"Another action stole pending draw");
  Check(claim.TryClaim(),"Resolved draw could not be claimed");
  Check(PrepareQuestLotteryClaim(stage,0,"missing",0.5)==null&&reward.Evaluations==1,"Completed action re-evaluated source");
  Reset();stage=new QuestStage();Reject(()=>PrepareQuestLotteryClaim(stage,0,"missing",0.5),"Missing fight silently completed");
  ListSF.Fight=new FightList();Reject(()=>PrepareQuestLotteryClaim(stage,0,"test",0.5),"Missing lottery silently completed");
  ListSF.Fight.Rewards.Add(new RewardStruct{Lottery=new RewardLottery()});ListSF.Fight.Rewards.Add(new RewardStruct{Lottery=new RewardLottery()});
  Reject(()=>PrepareQuestLotteryClaim(stage,0,"test",0.5),"Ambiguous lotteries silently selected");
  ListSF.Fight.Rewards.RemoveAt(1);Available=false;Reject(()=>PrepareQuestLotteryClaim(stage,0,"test",0.5),"Ineligible lottery silently completed");
  var checkpointXml=new XmlDocument();checkpointXml.LoadXml("<QuestParameters><Fight Value='test'/></QuestParameters>");var checkpoint=new ParametersQuest{Node=checkpointXml.DocumentElement};
  SaveQuestLotteryContext(checkpoint,new QuestParameters());Check(checkpoint.Node.ChildNodes.Count==1,"Ordinary quest gained lottery metadata");
  var context=new QuestParameters{inLottery=true,EGAPDJLHHNJ=3,FOODLENBJGI="example:item<&>",OHPHPJBMNLH="raid"};SaveQuestLotteryContext(checkpoint,context);
  checkpointXml.LoadXml(checkpointXml.OuterXml);checkpoint.Node=checkpointXml.DocumentElement;var resumed=new QuestParameters();RestoreQuestLotteryContext(checkpoint,resumed);
  Check(resumed.inLottery&&resumed.EGAPDJLHHNJ==3&&resumed.FOODLENBJGI==context.FOODLENBJGI&&resumed.OHPHPJBMNLH=="raid","Quest context lost across XML reload");
  Check(checkpoint.Node["Fight"].GetAttribute("Value")=="test","Lottery metadata damaged native checkpoint fields");
  checkpoint.Node["EclipseLotteryContext"].SetAttribute("Spin","invalid");resumed=new QuestParameters();
  try {RestoreQuestLotteryContext(checkpoint,resumed);throw new Exception("Malformed context accepted");}catch(InvalidDataException){checks++;}
  Check(!resumed.inLottery&&resumed.EGAPDJLHHNJ==0,"Invalid context partially mutated resume state");
  SaveQuestLotteryContext(checkpoint,new QuestParameters());Check(checkpoint.Node["EclipseLotteryContext"]==null,"New ordinary checkpoint retained stale lottery context");
  RestoreQuestLotteryContext(checkpoint,resumed);Check(!resumed.inLottery,"Legacy checkpoint acquired lottery state");
  Reset();ListSF.Queue=null;ListSF.Queued=ListSF.Runs=0;context=new QuestParameters{JLGLBLDPAAF=new FightIDS{Name="test"},BJIDALJIKNC=1,fightAvgFps=59.5f,OHPHPJBMNLH="boss"};
  string encounter=Guid.NewGuid().ToString("N");PrepareBattleLottery(new RewardLottery(),context,true,encounter);
  Check(HasPendingLottery&&ListSF.Saved.Contains("BattleEnd")&&Builds==1,"Battle draw/continuation not saved together");
  PrepareBattleLottery(new RewardLottery(),context,true,encounter);Check(Builds==1,"Repeated encounter rerolled draw");
  Reject(()=>PrepareBattleLottery(new RewardLottery(),context,false,Guid.NewGuid().ToString("N")),"New battle overwrote pending claim");
  Reject(()=>CompleteBattleLottery(),"Unclaimed battle dispatched");
  claim=ResumeLotteryClaim();Check(claim.TryClaim()&&HasPendingLottery,"Claim discarded unqueued fight-end context");
  Reject(()=>Prepare(),"New draw overwrote unqueued fight-end context");
  reloaded=new XmlDocument();reloaded.LoadXml(ListSF.Saved);_lotteryProfileNode=reloaded.DocumentElement;_profileRoster=new Roster();StoryEvents.BindProfile();
  ListSF.Queue=()=>{int writes=ListSF.Writes;ListSF.ELEBLBJKDBI().OnAuthenticate(true);Check(ListSF.Writes==writes,"Quest queue saved before dispatch marker");};
  CompleteBattleLottery();Check(!HasPendingLottery&&ListSF.Queued==1&&ListSF.Runs==1&&ListSF.Saved.Contains("Dispatched=\"1\""),"Battle continuation not durably accepted");
  Check(ListSF.Raid&&ListSF.Context.JLGLBLDPAAF.ToString()=="test"&&ListSF.Context.inLottery&&ListSF.Context.BJIDALJIKNC==1&&ListSF.Context.fightAvgFps==59.5f,"Battle context changed on reload");
  CompleteBattleLottery();PrepareBattleLottery(new RewardLottery(),context,true,encounter);Check(ListSF.Queued==1&&Builds==1,"Acknowledged battle replayed");
  Reset();ListSF.Queue=()=>throw new InvalidOperationException("queue failure");PrepareBattleLottery(new RewardLottery(),context,false,Guid.NewGuid().ToString("N"));ResumeLotteryClaim().TryClaim();prepared=ListSF.Saved;
  Reject(()=>CompleteBattleLottery(),"Queue failure ignored");Check(HasPendingLottery&&ListSF.Saved==prepared,"Failed queue overwrote recovery snapshot");Reject(()=>ListSF.ELEBLBJKDBI().OnAuthenticate(true),"Failed queue permitted autosave");
  Console.WriteLine("PASS: "+checks+" production lottery claim/recovery checks; selection, payload codec, quest stage, grant and disk save services controlled.");
 }
}
'@
$program.Replace('/* CLAIM */',$claim.Value).Replace('/* PREPARE */',$prepare.Value).Replace('/* RESUME */',$resume.Value).Replace('/* DEFER */',$defer.Value).Replace('/* ACK */',$ack.Value).Replace('/* QUEST RUN */',$questRun.Value).Replace('/* QUEST COMPLETE */',$questComplete.Value).Replace('/* QUEST RESOLVE */',$resolveQuest.Value).Replace('/* SAVE CONTEXT */',$saveContext.Value).Replace('/* RESTORE CONTEXT */',$restoreContext.Value).Replace('/* BATTLE PREPARE */',$battlePrepare.Value).Replace('/* BATTLE COMPLETE */',$battleComplete.Value).Replace('/* PENDING LOTTERY */',$pendingLottery.Value) | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Program.cs')
$sources=@('ModId.cs','DefinitionId.cs','ModStoryEvents.cs','ModQuestInvocationLedger.cs') | ForEach-Object {
 $path=[Security.SecurityElement]::Escape((Join-Path $root ('Assets/Scripts/Eclipse/Runtime/Modding/'+$_)))
 '<Compile Include="'+$path+'" />'
}
('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>'+($sources -join "`n")+'</ItemGroup></Project>') | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj')
if($LASTEXITCODE -ne 0){throw 'Lottery claim checks failed.'}
