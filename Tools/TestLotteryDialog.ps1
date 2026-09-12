$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$fixture=Join-Path $root ('Temp/LotteryDialog-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$dialog=[Security.SecurityElement]::Escape((Join-Path $root 'Assets/Scripts/Eclipse/UI/Modding/ModLotteryDialog.cs'))
$runtime=[Security.SecurityElement]::Escape((Join-Path $root 'Temp/bin/Debug/Eclipse.Runtime.dll'))
$program=@'
using System;
using Eclipse.Modding;
using Eclipse.UI.Modding;
namespace Eclipse.UI.Modding {static class ModUiGameBridge {public static void Attach(ModUiSurface s)=>s.SetInputAllowed(true);}}
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
  Console.WriteLine("PASS: "+checks+" production lottery dialog/state lifecycle checks; Unity mounting/rendering controlled.");
 }
}
'@
$program | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Program.cs')
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup><Compile Include="$dialog"/><Reference Include="Eclipse.Runtime"><HintPath>$runtime</HintPath></Reference></ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj')
if($LASTEXITCODE -ne 0){throw 'Lottery dialog checks failed.'}
