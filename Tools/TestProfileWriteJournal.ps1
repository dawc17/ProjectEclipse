$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$fixture=Join-Path $root ('Temp/ProfileWriteJournal-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$source=[Security.SecurityElement]::Escape((Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/ModProfileWriteJournal.cs'))
$program=@'
using System;
using System.IO;
using System.Text;
using Eclipse.Modding;
class Program {
 static int checks;static byte[] B(string text)=>Encoding.UTF8.GetBytes(text);
 static void Check(bool result,string message){checks++;if(!result)throw new Exception(message);}
 static void Fail<T>(Action action)where T:Exception{try{action();}catch(T){checks++;return;}throw new Exception("Expected "+typeof(T).Name);}
 static void Validate(byte[] data,byte[] hash){if(data.Length==0)throw new InvalidDataException();}
 static void Main(string[] args){
  if(args.Length==2&&args[0]=="recover"){ModProfileWriteJournal.Recover(args[1],Validate);return;}
  string path=Path.Combine(args[0],"users.xml"),journal=path+".eclipse-write";
  ModProfileWriteJournal.Write(path,B("old"),B("old-hash"),Validate);
  Check(File.ReadAllText(path)=="old"&&File.ReadAllText(path+".hash")=="old-hash"&&!File.Exists(journal),"Normal write failed");
  Check(!ModProfileWriteJournal.Recover(path,Validate),"Spurious recovery");
  using(var locked=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.None))
   Fail<IOException>(()=>ModProfileWriteJournal.Write(path,B("new"),B("new-hash"),Validate));
  Check(File.ReadAllText(path)=="old"&&File.Exists(journal),"Failed first install lost old snapshot or pending write");
  Fail<InvalidDataException>(()=>ModProfileWriteJournal.Recover(path,(data,hash)=>{throw new InvalidDataException("Host rejects snapshot");}));
  Check(File.ReadAllText(path)=="old"&&File.Exists(journal),"Rejected recovery mutated destination");
  Check(ModProfileWriteJournal.Recover(path,Validate)&&File.ReadAllText(path)=="new"&&File.ReadAllText(path+".hash")=="new-hash","First-write recovery failed");
  using(var locked=new FileStream(path+".hash",FileMode.Open,FileAccess.Read,FileShare.None))
   Fail<IOException>(()=>ModProfileWriteJournal.Write(path,B("third"),B("third-hash"),Validate));
  Check(File.ReadAllText(path)=="third"&&File.ReadAllText(path+".hash")=="new-hash"&&File.Exists(journal),"Expected recoverable split pair");
  var start=new System.Diagnostics.ProcessStartInfo(Environment.ProcessPath){UseShellExecute=false,CreateNoWindow=true};
  start.ArgumentList.Add("recover");start.ArgumentList.Add(path);
  using(var child=System.Diagnostics.Process.Start(start)){
   if(!child.WaitForExit(10000)){child.Kill();throw new Exception("Fresh-process recovery timed out");}
   Check(child.ExitCode==0&&File.ReadAllText(path+".hash")=="third-hash"&&!File.Exists(journal),"Fresh-process hash recovery failed");
  }
  ModProfileWriteJournal.Write(path,B("no-hash"),null,Validate);
  Check(File.ReadAllText(path)=="no-hash"&&File.ReadAllText(path+".hash")=="third-hash","Disabled-hash policy changed");
  ModProfileWriteJournal.Write(path,B("isolated"),B("hash"),(data,hash)=>{data[0]=0;hash[0]=0;});
  Check(File.ReadAllText(path)=="isolated"&&File.ReadAllText(path+".hash")=="hash","Validator mutated stored bytes");
  using(var locked=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.None))
   Fail<IOException>(()=>ModProfileWriteJournal.Write(path,B("pending"),null,Validate));
  var bytes=File.ReadAllBytes(journal);bytes[12]^=1;File.WriteAllBytes(journal,bytes);
  Fail<InvalidDataException>(()=>ModProfileWriteJournal.Recover(path,Validate));
  Fail<InvalidDataException>(()=>ModProfileWriteJournal.Write(path,B("replacement"),null,Validate));
  Check(File.ReadAllText(path)=="isolated"&&File.Exists(journal),"Corruption was overwritten or installed");
  // Only this fixture's intentionally corrupt journal is removed.
  File.Delete(journal);
  using(var locked=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.None))
   Fail<IOException>(()=>ModProfileWriteJournal.Write(path,B("discarded"),null,Validate));
  ModProfileWriteJournal.Discard(path);
  Check(!ModProfileWriteJournal.Recover(path,Validate)&&File.ReadAllText(path)=="isolated","Discarded snapshot resurrected");
  using(var locked=new FileStream(path+".eclipse-write.lock",FileMode.Open,FileAccess.ReadWrite,FileShare.None))
   Fail<IOException>(()=>ModProfileWriteJournal.Write(path,B("concurrent"),null,Validate));
  Fail<InvalidDataException>(()=>ModProfileWriteJournal.Write(path,Array.Empty<byte>(),null,Validate));
  Check(File.ReadAllText(path)=="isolated"&&!File.Exists(journal),"Rejected writes changed snapshot");
  Check(Directory.GetFiles(args[0],"*.tmp").Length==0,"Temporary files leaked after handled failures");
  Console.WriteLine("PASS: "+checks+" disk-backed journal checks; no live profiles touched. Power-loss/filesystem durability not simulated.");
 }
}
'@
$program | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Program.cs')
"<Project Sdk=`"Microsoft.NET.Sdk`"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup><Compile Include=`"$source`"/></ItemGroup></Project>" | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj') -- $fixture
if($LASTEXITCODE -ne 0){throw 'Profile write journal checks failed.'}
