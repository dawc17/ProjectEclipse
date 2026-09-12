$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
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
if(!$questRun.Success -or !$questComplete.Success){throw 'Native quest lottery run bridge missing.'}
if(!$resume.Success -or !$defer.Success){throw 'Lottery recovery/save gate missing.'}
$program=@'
using System;
using System.Xml;
using System.IO;
using Eclipse.Modding;
class Program {
 public class Roster {public int Saves;public int PINDEKDNCNL()=>4;public void GGGEHAGCLGC(bool immediate){Saves++;}}
 static readonly ModStoryEvents StoryEvents=new ModStoryEvents();
 public class FightResult {public class ResultPrizeStruct {public object FAPDEKOMOGH;public int Token;public System.Collections.Generic.List<object> KBMDJACLAOH=new System.Collections.Generic.List<object>();}}
 public class RewardLottery {}
 public class QuestStage {public bool allowDoubles,EclipseResumeActions;public string FileName="quests.xml",EclipseActionsDefinition="<Actions><DialogLottery/></Actions>";public ModQuestInvocationLedger EclipseLotteryInvocations;public string get_Name()=>"LotteryQuest";}
 public struct MANJCIGJPMK {public string Image=>"test";public string ViewType=>"Weapon";}
 public class ItemInfo {public int MHGODOLNDLE,OBJDGBBFJOO;public ItemInfo HIOBANJPMKF(int n)=>this;}
 public class Catalog {public ItemInfo KCCDBEEKBCG(string n)=>null;public object ICFINJLNCPM(string n)=>null;public object NDMEGBEFBPJ(string n)=>null;}
 public static class GameUtils {public static Catalog AJDKHINLIDI=new Catalog(),JNIMKHKGPHE=new Catalog();}
 public class ListSF {public static Action Grant;public static int Grants,Writes;public static string Saved;public static Catalog DJBOFEEKJMP()=>new Catalog();public static ListSF ELEBLBJKDBI()=>new ListSF();public bool IMDGMNFHFCN(FightResult.ResultPrizeStruct p){Grants++;Grant?.Invoke();return false;}public void OnAuthenticate(bool force){if(DeferProfileSave())return;Writes++;Saved=_lotteryProfileNode.OwnerDocument.OuterXml;}}
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
  Console.WriteLine("PASS: "+checks+" production lottery claim/recovery checks; selection, payload codec, quest stage, grant and disk save services controlled.");
 }
}
'@
$program.Replace('/* CLAIM */',$claim.Value).Replace('/* PREPARE */',$prepare.Value).Replace('/* RESUME */',$resume.Value).Replace('/* DEFER */',$defer.Value).Replace('/* ACK */',$ack.Value).Replace('/* QUEST RUN */',$questRun.Value).Replace('/* QUEST COMPLETE */',$questComplete.Value) | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Program.cs')
$sources=@('ModId.cs','DefinitionId.cs','ModStoryEvents.cs','ModQuestInvocationLedger.cs') | ForEach-Object {
 $path=[Security.SecurityElement]::Escape((Join-Path $root ('Assets/Scripts/Eclipse/Runtime/Modding/'+$_)))
 '<Compile Include="'+$path+'" />'
}
('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>'+($sources -join "`n")+'</ItemGroup></Project>') | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj')
if($LASTEXITCODE -ne 0){throw 'Lottery claim checks failed.'}
