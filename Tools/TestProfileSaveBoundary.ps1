$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$fixture=Join-Path $root ('Temp/ProfileSaveBoundary-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
function Read-Method($file,$signature) {
 $source=Get-Content -Raw -LiteralPath (Join-Path $root $file)
 $match=[regex]::Match($source,'(?ms)^\t'+[regex]::Escape($signature)+'.*?^\t\}')
 if(!$match.Success){throw "Method not found: $signature"}
 return $match.Value
}
$save=Read-Method 'Assets/Scripts/Assembly-CSharp/ListSF.cs' 'public void OnAuthenticate('
$write=Read-Method 'Assets/Scripts/Assembly-CSharp/XmlUtils.cs' 'public static void ONLDJNLKKAL('
$read=Read-Method 'Assets/Scripts/Assembly-CSharp/XmlUtils.cs' 'public static XmlDocument AIFIAKNJMHG('
$runtime=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModRuntime.cs')
$adapters=@('internal static bool DeferProfileSave','internal static bool IsProfileSnapshotPath','internal static bool TryWriteProfileSnapshot','internal static void RecoverProfileSnapshot','private static void ValidateProfileSnapshot') | ForEach-Object {
 $match=[regex]::Match($runtime,'(?ms)^        '+[regex]::Escape($_)+'\(.*?^        \}')
 if(!$match.Success){throw "Missing production profile adapter $_"};$match.Value
}
$program=@'
using System;
using System.IO;
using System.Text;
using System.Xml;
namespace UnityEngine {public static class Debug {public static void LogException(Exception e){throw e;}}}
namespace Nekki.SF2.Core.Exceptions {public class HackDetectedException:Exception {public HackDetectedException(string s):base(s){}}}
static class SystemProperties {public static string GLLJKPBHELE()=>"fixture-device";}
static class GameSettings {public static bool Hashes;public static bool HCAJHNKLLGB()=>Hashes;}
static class SF2Paths {public static string Folder;public static string APHDBIBDMDG()=>Folder;}
static class Constants {public const string OJMIJINKBPJ="users.xml",GHKPPHAAMBL="users_backup.xml";}
static class XmlUtils {
 public enum EBLFEPIOMOL {Normal,ForcedExternal,ForcedResourced}
 public static XmlDocument OpenXMLDocument(string directory,string name,EBLFEPIOMOL mode,bool flag){var doc=new XmlDocument();doc.Load(string.IsNullOrEmpty(name)?directory:Path.Combine(directory,name));return doc;}
 /* WRITE */
 /* READ */
}
namespace Eclipse.Modding {static class ModRuntime {static int _lotterySaveState=0; /* ADAPTERS */ }}
class Program {
 public bool GJEJCLBAPMP=true;public XmlDocument IEDEFCBFJAD;
 public Program CCDKHLAMKKO()=>this;public void KGFJPLKOABI(){} public void PMIIHIFGIIN(){}
 /* SAVE */
 static int checks;
 static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
 static XmlDocument Doc(string value){var d=new XmlDocument();d.LoadXml("<Root Claim='"+value+"'/>");return d;}
 static XmlDocument Load(string path){var d=new XmlDocument();d.Load(path);return d;}
 static void Fail(Action action){try{action();}catch(IOException){checks++;return;}throw new Exception("Expected disk sharing failure");}
 static void Main(string[] args){
  SF2Paths.Folder=args[0];var primary=Path.Combine(args[0],Constants.OJMIJINKBPJ);var backup=Path.Combine(args[0],Constants.GHKPPHAAMBL);
  foreach(bool hashes in new[]{false,true}){
   GameSettings.Hashes=hashes;var p=new Program{IEDEFCBFJAD=Doc("prepared")};p.OnAuthenticate();
   Check(!p.GJEJCLBAPMP&&Load(primary).OuterXml==Load(backup).OuterXml,"Normal save did not finish both copies");
   p.IEDEFCBFJAD=Doc("claimed");p.GJEJCLBAPMP=true;
   using(var locked=new FileStream(backup,FileMode.Open,FileAccess.Read,FileShare.None)) Fail(()=>p.OnAuthenticate());
   Check(p.GJEJCLBAPMP,"Failed second write cleared dirty flag");
   Check(Load(primary).OuterXml==Doc("claimed").OuterXml&&Load(backup).OuterXml==Doc("prepared").OuterXml,"Expected split primary/backup generations");
   p.OnAuthenticate();Check(!p.GJEJCLBAPMP&&Load(backup).OuterXml==Load(primary).OuterXml,"Explicit retry did not repair second copy");
  }
  GameSettings.Hashes=true;var q=new Program{IEDEFCBFJAD=Doc("prepared")};q.OnAuthenticate();q.GJEJCLBAPMP=true;q.IEDEFCBFJAD=Doc("claimed");
  using(var locked=new FileStream(primary+".hash",FileMode.Open,FileAccess.Read,FileShare.None)) Fail(()=>q.OnAuthenticate());
  Check(q.GJEJCLBAPMP&&Load(primary).OuterXml==Doc("claimed").OuterXml,"Hash failure did not expose new XML with dirty state");
  bool rejected=false;try{UserDataValidator.CheckFileHash(Load(primary),primary);}catch(Nekki.SF2.Core.Exceptions.HackDetectedException){rejected=true;}
  Check(rejected,"Mismatched XML/hash accepted");
  Check(UserDataValidator.CheckFileHash(Load(backup),backup)&&Load(backup).OuterXml==Doc("prepared").OuterXml,"Previous backup lost during primary hash failure");
  var recovered=XmlUtils.AIFIAKNJMHG(args[0],Constants.OJMIJINKBPJ);
  Check(UserDataValidator.CheckFileHash(recovered,primary)&&recovered.OuterXml==Doc("claimed").OuterXml&&!File.Exists(primary+".eclipse-write"),"Production reader failed to recover pair before hash validation");
  q.OnAuthenticate();Check(UserDataValidator.CheckFileHash(Load(primary),primary)&&!q.GJEJCLBAPMP,"Explicit retry failed to restore hash consistency");
  Check(!Eclipse.Modding.ModRuntime.IsProfileSnapshotPath(Path.Combine(args[0],"settings.xml")),"Unrelated XML routed through profile journal");
  Console.WriteLine("PASS: "+checks+" save-boundary characterization checks; actual disk writes, production save methods and hash implementation. Roster serialization/device/settings controlled; no crash recovery claim.");
 }
}
'@
$program.Replace('/* WRITE */',$write).Replace('/* READ */',$read).Replace('/* SAVE */',$save).Replace('/* ADAPTERS */',($adapters -join "`n")) | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Program.cs')
$sources=@('UserDataValidator.cs','MD5Utils.cs') | ForEach-Object {
 '<Compile Include="'+[Security.SecurityElement]::Escape((Join-Path $root ('Assets/Scripts/Assembly-CSharp/'+$_)))+'" />'
}
$sources += '<Compile Include="'+[Security.SecurityElement]::Escape((Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/ModProfileWriteJournal.cs'))+'" />'
('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>'+($sources -join "`n")+'</ItemGroup></Project>') | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj') -- $fixture
if($LASTEXITCODE -ne 0){throw 'Profile save boundary checks failed.'}
