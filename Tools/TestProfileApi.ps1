$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
& (Join-Path $PSScriptRoot 'TestPhase1ShowcaseRuntime.ps1')
$fixture=Join-Path $root ('Temp/ProfileApi-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidateProfileApi.cs') -Destination (Join-Path $fixture 'Program.cs')
$source=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModRuntime.cs')
$level=[regex]::Match($source,'(?m)^        private static int\? ReadProfileLevel\(\).*;$')
$item=[regex]::Match($source,'(?ms)^        private static ModProfileItemSnapshot ReadProfileItem\(.*?^        \}')
if(!$level.Success -or !$item.Success){throw 'Native profile query methods not found.'}
$native=@'
using System;
using Eclipse.Modding;
public static class NativeProfileFixture {
 public class InventoryItem { public int Count=2; public bool EFMFGEPDAOP()=>true; public int DHNNCAEEMLL()=>3; }
 public class Inventory { public InventoryItem Item=new InventoryItem(); public string Name; public InventoryItem CMGOCLGHNLH(string name){Name=name;return Item;} }
 public class Roster { public int Level=12; public Inventory Items=new Inventory(); public Inventory KHCNHPCPFII()=>Items; }
 public class Scripts {public ModContentCatalog Content;}
 private static Roster _profileRoster;
 private static Scripts _scripts;
 /* METHODS */
 public static void Run(ModContentCatalog catalog){
  _scripts=new Scripts{Content=catalog};_profileRoster=null;
  if(ReadProfileLevel()!=null||ReadProfileItem(CoreContentImporter.WeaponId("WEAPON_NUNCHAKU"))!=null)throw new Exception("Unavailable roster returned data");
  _profileRoster=new Roster();var result=ReadProfileItem(CoreContentImporter.WeaponId("WEAPON_NUNCHAKU"));
  if(!result.Owned||!result.Equipped||result.Count!=2||result.Upgrade!=3||_profileRoster.Items.Name!="WEAPON_NUNCHAKU")throw new Exception("Native item mapping/read failed");
  _profileRoster.Items.Item.Count=4;
  if(result.Count!=2||ReadProfileItem(CoreContentImporter.WeaponId("WEAPON_NUNCHAKU")).Count!=4)throw new Exception("Snapshot not detached or query stale");
  _profileRoster=new Roster{Level=20};_profileRoster.Items.Item=null;
  if(ReadProfileLevel()!=20||ReadProfileItem(CoreContentImporter.WeaponId("WEAPON_NUNCHAKU")).Present)throw new Exception("Roster switch retained old inventory");
  bool rejected=false;try{ReadProfileItem(DefinitionId.Parse("core:items/weapon/missing"));}catch(ModContentException){rejected=true;}
  if(!rejected)throw new Exception("Unavailable definition accepted");
  _profileRoster=null;
  if(ReadProfileLevel()!=null)throw new Exception("Roster unbind retained level");
  Console.WriteLine("PASS: 6 production profile query method checks with controlled roster services.");
 }
}
'@
$native.Replace('/* METHODS */',$level.Value+[Environment]::NewLine+$item.Value) | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Native.cs')
$production=[Security.SecurityElement]::Escape((Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference>
<Reference Include="MoonSharp.Interpreter"><HintPath>$root/Library/ScriptAssemblies/MoonSharp.Interpreter.dll</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj') -- (Join-Path $fixture 'Mods') $root
if ($LASTEXITCODE -ne 0) { throw 'Profile API checks failed.' }
