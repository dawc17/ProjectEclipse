$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
& (Join-Path $PSScriptRoot 'TestPhase1ShowcaseRuntime.ps1')
$fixture=Join-Path $root ('Temp/ProfileApi-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidateProfileApi.cs') -Destination (Join-Path $fixture 'Program.cs')
$source=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModRuntime.cs')
$level=[regex]::Match($source,'(?m)^        private static int\? ReadProfileLevel\(\).*;$')
$item=[regex]::Match($source,'(?ms)^        private static ModProfileItemSnapshot ReadProfileItem\(.*?^        \}')
$perk=[regex]::Match($source,'(?ms)^        private static ModProfilePerkSnapshot ReadProfilePerk\(.*?^        \}')
if(!$level.Success -or !$item.Success -or !$perk.Success){throw 'Native profile query methods not found.'}
$equipment=[regex]::Match($source,'(?ms)^        private static IReadOnlyList<ModProfileEquipmentSnapshot> ReadProfileEquipment\(.*?^        \}')
if(!$equipment.Success){throw 'Native equipment query not found.'}
$native=@'
using System;
using System.Collections.Generic;
using Eclipse.Modding;
public static class NativeProfileFixture {
 public class ItemMetadata {public string Type="Weapon",MDPPNGIEJGD="Nunchaku";public System.Xml.XmlNode NodeXML;}
 public class Catalog {public ItemMetadata Item=new ItemMetadata();public string Name;public ItemMetadata KCCDBEEKBCG(string name){Name=name;return Item;}}
 public static class ListSF {public static Catalog Items=new Catalog();public static Catalog DJBOFEEKJMP()=>Items;}

 public class InventoryItem { public string Name="WEAPON_NUNCHAKU";public string get_Name()=>Name;public ItemMetadata Metadata=new ItemMetadata();public ItemMetadata BHKHOJPANHE()=>Metadata; public int Count=2; public bool EFMFGEPDAOP()=>true; public int DHNNCAEEMLL()=>3; }
 public class Inventory { public List<InventoryItem> Equipped=new List<InventoryItem>();public List<InventoryItem> JCMOHPFKPBO()=>Equipped; public InventoryItem Item=new InventoryItem(); public string Name; public InventoryItem CMGOCLGHNLH(string name){Name=name;return Item;} }
 public class Perk { public int Upgrade=2; public string get_Name()=>"TEST_PERK";public int DHNNCAEEMLL()=>Upgrade; }
 public class Perks { public System.Collections.Generic.List<Perk> Values=new System.Collections.Generic.List<Perk>();public System.Collections.Generic.List<Perk> KEHFPLBNDHI()=>Values; }
 public class Roster { public Perks Perks=new Perks();public Perks JLBDOBLHHAF()=>Perks;public int Level=12; public Inventory Items=new Inventory(); public Inventory KHCNHPCPFII()=>Items; }
 public class Scripts {public ModContentCatalog Content;}
 private static Roster _profileRoster;
 private static Scripts _scripts;
 /* METHODS */
 public static void Bind(ModContentCatalog catalog){
  _scripts=new Scripts{Content=catalog};_profileRoster=new Roster();
  _profileRoster.Perks.Values.Add(new Perk());ListSF.Items=new Catalog();
  ModProfileAccess.Item=ReadProfileItem;ModProfileAccess.Perk=ReadProfilePerk;ModProfileAccess.Equipment=ReadProfileEquipment;
  _profileRoster.Items.Equipped.Add(_profileRoster.Items.Item);
  _profileRoster.Items.Equipped.Add(new InventoryItem{Name="UNREGISTERED",Count=0,Metadata=null});
 }
 public static void Unbind(){_profileRoster=null;}
 public static void Run(ModContentCatalog catalog){
  _scripts=new Scripts{Content=catalog};_profileRoster=null;
  if(ReadProfileLevel()!=null||ReadProfileItem(CoreContentImporter.WeaponId("WEAPON_NUNCHAKU"))!=null)throw new Exception("Unavailable roster returned data");
  _profileRoster=new Roster();var result=ReadProfileItem(CoreContentImporter.WeaponId("WEAPON_NUNCHAKU"));
  if(!result.Owned||!result.Equipped||result.Count!=2||result.Upgrade!=3||_profileRoster.Items.Name!="WEAPON_NUNCHAKU")throw new Exception("Native item mapping/read failed");
  _profileRoster.Items.Item.Count=4;
  if(result.Count!=2||ReadProfileItem(CoreContentImporter.WeaponId("WEAPON_NUNCHAKU")).Count!=4)throw new Exception("Snapshot not detached or query stale");
  _profileRoster=new Roster{Level=20};_profileRoster.Items.Item=null;
  if(ReadProfileLevel()!=20||ReadProfileItem(CoreContentImporter.WeaponId("WEAPON_NUNCHAKU")).Present)throw new Exception("Roster switch retained old inventory");
  var classified=ReadProfileItem(CoreContentImporter.WeaponId("WEAPON_NUNCHAKU"));
  if(classified.Type!="Weapon"||classified.Subtype!="Nunchaku"||ListSF.Items.Name!="WEAPON_NUNCHAKU")throw new Exception("Unowned item classification unavailable");
  ListSF.Items.Item.MDPPNGIEJGD="Changed";
  if(classified.Subtype!="Nunchaku"||ReadProfileItem(CoreContentImporter.WeaponId("WEAPON_NUNCHAKU")).Subtype!="Changed")throw new Exception("Classification snapshot stale/aliased");
  ListSF.Items.Item=null;
  if(ReadProfileItem(CoreContentImporter.WeaponId("WEAPON_NUNCHAKU")).Type!=null)throw new Exception("Missing metadata invented classification");
  bool rejected=false;try{ReadProfileItem(DefinitionId.Parse("core:items/weapon/missing"));}catch(ModContentException){rejected=true;}
  if(!rejected)throw new Exception("Unavailable definition accepted");
  var perkId=CoreContentImporter.PerkId("TEST_PERK");
  if(ReadProfilePerk(perkId).Learned)throw new Exception("Missing perk reported learned");
  var perk=new Perk();_profileRoster.Perks.Values.Add(perk);
  var learned=ReadProfilePerk(perkId);perk.Upgrade=4;
  if(!learned.Learned||learned.Upgrade!=2||ReadProfilePerk(perkId).Upgrade!=4)throw new Exception("Perk snapshot stale or aliased");
  _profileRoster=new Roster();
  if(ReadProfilePerk(perkId).Learned)throw new Exception("Perk retained across profile replacement");
  bool badPerk=false;try{ReadProfilePerk(DefinitionId.Parse("core:perks/missing"));}catch(ModContentException){badPerk=true;}
  if(!badPerk)throw new Exception("Unknown perk accepted");
  _profileRoster=null;
  if(ReadProfilePerk(perkId)!=null)throw new Exception("Unloaded perk query retained data");
  if(ReadProfileLevel()!=null)throw new Exception("Roster unbind retained level");
  Bind(catalog);var equipment=ReadProfileEquipment();
  if(equipment.Count!=2||equipment[0].Item!=CoreContentImporter.WeaponId("WEAPON_NUNCHAKU")||equipment[0].State.Subtype!="Nunchaku")throw new Exception("Equipped identity/metadata missing");
  if(equipment[1].Item!=null||equipment[1].State.Owned||equipment[1].State.Type!=null)throw new Exception("Unknown equipped record invented identity/ownership");
  _profileRoster.Items.Equipped.Clear();
  if(equipment.Count!=2||ReadProfileEquipment().Count!=0)throw new Exception("Equipment snapshot stale or aliased");
  Unbind();if(ReadProfileEquipment()!=null)throw new Exception("Unloaded equipment remained available");
  Console.WriteLine("PASS: 18 production profile query method checks with controlled roster services.");
 }
}
'@
$native.Replace('/* METHODS */',$level.Value+[Environment]::NewLine+$item.Value+[Environment]::NewLine+$perk.Value+[Environment]::NewLine+$equipment.Value) | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Native.cs')
$production=[Security.SecurityElement]::Escape((Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference>
<Reference Include="MoonSharp.Interpreter"><HintPath>$root/Library/ScriptAssemblies/MoonSharp.Interpreter.dll</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj') -- (Join-Path $fixture 'Mods') $root
if ($LASTEXITCODE -ne 0) { throw 'Profile API checks failed.' }
