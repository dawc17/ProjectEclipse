$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$fixture=Join-Path $root ('Temp/LotteryDialog-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$dialog=[Security.SecurityElement]::Escape((Join-Path $root 'Assets/Scripts/Eclipse/UI/Modding/ModLotteryDialog.cs'))
$questAction=[Security.SecurityElement]::Escape((Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModQuestLotteryAction.cs'))
$runtime=[Security.SecurityElement]::Escape((Join-Path $root 'Temp/bin/Debug/Eclipse.Runtime.dll'))
$program=@'
using System;
using Eclipse.Modding;
using Eclipse.UI.Modding;
static class LocalizationManager {public static string GetStringOrDefault(string key,string fallback,params string[] args)=>fallback;}
namespace Eclipse.UI.Modding {static class ModUiGameBridge {public static ModUiSurface Last;public static void Attach(ModUiSurface s){Last=s;s.SetInputAllowed(true);}}}
namespace UnityEngine {static class Random {public static float value=1;}static class Debug {public static void LogException(Exception e){}}}
enum ScreenType {ModuleMap,ModuleFight}
class QuestAction {public int Index;public object NOFNJFOCIMK()=>this;}
class Module {
 public enum FKHIMIAOCJL {OnOpenScene=1,OnCloseScene=3}
 public static Module Instance=new Module();public static Module ELEBLBJKDBI()=>Instance;
 public ScreenType Screen;public ScreenType NMCNDOPKFJD()=>Screen;
 System.Collections.Generic.Dictionary<int,Action<object>> handlers=new System.Collections.Generic.Dictionary<int,Action<object>>();
 public void AddEventListener(int n,Action<object> a){handlers.TryGetValue(n,out var old);handlers[n]=old+a;}
 public void RemoveEventListener(int n,Action<object> a){handlers.TryGetValue(n,out var old);handlers[n]=old-a;}
 public void Emit(int n){if(handlers.TryGetValue(n,out var a))a?.Invoke(null);}
}
namespace Eclipse.Modding {static class ModRuntime {
 internal sealed class LotteryClaim {public bool IsCurrent=true;public string PreviewText=>"Saved item";public int Grants;public bool TryClaim(){if(!IsCurrent||Grants!=0)return false;Grants++;return true;}}
 public static LotteryClaim Next;public static int Draws;
 public static AssetId? Artwork;public static AssetId? ResolveLotteryArtwork(LotteryClaim claim)=>Artwork;
 public static LotteryClaim PrepareQuestLotteryClaim(object stage,int index,string name,double sample){if(sample<0||sample>=1)throw new Exception("Invalid sample");Draws++;return Next;}
}}
class Program {
 static int checks,grants,complete,deferred,errors;
 static void Check(bool value,string text){checks++;if(!value)throw new Exception(text);}
 static ModLotteryDialog Open(Func<bool> action){grants=complete=deferred=errors=0;var d=new ModLotteryDialog("Lottery","A saved reward",()=>{grants++;return action();},()=>complete++,reason=>deferred++,error=>errors++);d.Show();return d;}
 static void Main(){
  var d=Open(()=>true);Check(d.Surface.TryClick("claim"),"Claim click rejected");Check(grants==1&&complete==1&&deferred==0&&d.Surface.IsClosed,"Successful claim did not complete once");Check(!d.Surface.TryClick("claim"),"Closed claim clickable");d.Dispose();Check(complete==1,"Disposal repeated completion");
  d=Open(()=>true);d.Surface.TryClick("later");Check(grants==0&&complete==0&&deferred==1,"Later granted or completed quest");
  d=Open(()=>false);d.Surface.TryClick("claim");Check(grants==1&&complete==0&&!d.Surface.IsClosed&&!d.Surface.Read("claim").Enabled,"Rejected claim remained repeatable");d.Dispose();Check(deferred==1,"Rejected claim not retained on close");
  d=Open(()=>throw new InvalidOperationException("disk failure"));d.Surface.TryClick("claim");Check(errors==1&&complete==0&&!d.Surface.IsClosed,"Failure completed or lost error UI");Check(d.Surface.Read("status").Text.Contains("Reload"),"Recovery guidance missing");d.Dispose();
  d=Open(()=>true);d.Surface.Close(ModUiCloseReason.Scene);Check(grants==0&&complete==0&&deferred==1,"Scene teardown completed unclaimed reward");
  ModLotteryDialog active=null;active=Open(()=>{active.Surface.Close(ModUiCloseReason.Scene);return true;});active.Surface.TryClick("claim");Check(grants==1&&complete==1&&deferred==0,"Scene teardown during successful grant lost completion");
  active=Open(()=>{active.Surface.Close(ModUiCloseReason.Scene);throw new InvalidOperationException();});active.Surface.TryClick("claim");Check(errors==1&&complete==0&&deferred==1,"Scene teardown during failed grant completed reward");
  active=Open(()=>{Check(!active.Surface.TryClick("claim"),"Reentrant claim");return true;});active.Surface.TryClick("claim");Check(grants==1&&complete==1,"Reentrant grant duplicated");
  Module.Instance=new Module();ModRuntime.Next=new ModRuntime.LotteryClaim();ModRuntime.Draws=0;complete=0;
  ModRuntime.Artwork=AssetId.Parse("core:UI/Items/reward");
  var workflow=new ModQuestLotteryAction(new QuestAction(),"fight",()=>complete++);workflow.Show();var first=ModUiGameBridge.Last;
  Check(!first.IsClosed&&complete==0&&ModRuntime.Next.Grants==0,"Quest awarded before click");
  Check(first.Root.Children[1].Children[0].Children[0].Sprite==ModRuntime.Artwork,"Quest did not include reward artwork");
  Check(first.Root.Children[1].Children[0].Height<=240,"Single reward summary starts outside visible viewport");
  Module.Instance.Emit(3);Check(first.IsClosed&&complete==0,"Scene close advanced quest");
  Module.Instance.Emit(1);var reopened=ModUiGameBridge.Last;Check(reopened!=first&&!reopened.IsClosed&&ModRuntime.Draws==1,"Scene reopen redrew reward");
  reopened.TryClick("later");Check(complete==0&&ModRuntime.Next.Grants==0,"Later advanced native quest");
  Module.Instance.Emit(1);ModUiGameBridge.Last.TryClick("claim");Check(complete==1&&ModRuntime.Next.Grants==1,"Claim did not advance native quest");
  var last=ModUiGameBridge.Last;Module.Instance.Emit(1);Check(ModUiGameBridge.Last==last&&complete==1,"Finished workflow reopened");
  Module.Instance=new Module();ModRuntime.Next=new ModRuntime.LotteryClaim();workflow=new ModQuestLotteryAction(new QuestAction(),"fight",()=>complete++);workflow.Show();
  Module.Instance.Emit(3);ModRuntime.Next.IsCurrent=false;Module.Instance.Emit(1);Check(ModUiGameBridge.Last.IsClosed&&complete==1,"Stale profile reopened or advanced quest");
  Module.Instance=new Module{Screen=ScreenType.ModuleFight};ModRuntime.Next=new ModRuntime.LotteryClaim();workflow=new ModQuestLotteryAction(new QuestAction(),"fight",()=>complete++);last=ModUiGameBridge.Last;workflow.Show();Check(ModUiGameBridge.Last==last,"Quest dialog opened over combat");workflow.Dispose();
  Module.Instance=new Module();ModRuntime.Next=null;workflow=new ModQuestLotteryAction(new QuestAction(),"fight",()=>complete++);workflow.Show();workflow.Show();Check(complete==2,"Acknowledged action did not advance exactly once");
  Console.WriteLine("PASS: "+checks+" production lottery dialog/quest presentation lifecycle checks; claim service and Unity mounting/rendering controlled.");
 }
}
'@
$program | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Program.cs')
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup><Compile Include="$dialog"/><Compile Include="$questAction"/><Reference Include="Eclipse.Runtime"><HintPath>$runtime</HintPath></Reference></ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj')
if($LASTEXITCODE -ne 0){throw 'Lottery dialog checks failed.'}
