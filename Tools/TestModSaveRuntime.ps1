$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$source = Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/UserItems.cs')
# Execute the recovered inventory parse/add methods, with unrelated Unity/timer services stubbed.
$parse = [regex]::Match($source, '(?ms)^\tpublic void Parse\(XmlNode.*?^\t\}').Value
$add = [regex]::Match($source, '(?ms)^\tpublic UserItem GEFDJDIINND\(.*?^\t\}').Value
if (!$parse -or !$add) { throw 'Could not extract recovered inventory methods.' }
$testRoot = Join-Path $root 'Temp/ModSaveRuntime'
New-Item -ItemType Directory -Force -Path $testRoot | Out-Null
$code = @'
using System;
using System.Collections.Generic;
using System.Xml;
using Eclipse.Modding;

namespace UnityEngine { public static class Debug { public static void LogWarning(object value) {} } }
public sealed class ItemInfo { public string Type = "Weapon"; public void BEBDMOEIEJN(bool value) {} }
public sealed class Items
{
    public readonly Dictionary<string, ItemInfo> Definitions = new Dictionary<string, ItemInfo>();
    public ItemInfo KCCDBEEKBCG(string name) { ItemInfo item; return Definitions.TryGetValue(name, out item) ? item : null; }
}
public static class ListSF { public static readonly Items Items = new Items(); public static Items DJBOFEEKJMP() { return Items; } }
public sealed class UserItem
{
    public static int Constructions;
    private readonly XmlElement _node;
    public UserItem(XmlNode node) { Constructions++; _node = (XmlElement)node; }
    public string get_Name() { return _node.GetAttribute("Name"); }
    public ItemInfo BHKHOJPANHE() { return null; } // Binding occurs later in HOMCPNCGPDB.
    public long IJGAOHJNLAH() { long value; return long.TryParse(_node.GetAttribute("DeliveryTime"), out value) ? value : 0; }
    public void JBLKCIBKMKB(bool value) { _node.SetAttribute("Equipped", value ? "1" : "0"); }
}
public sealed class UserItems
{
    private readonly List<string> _missingModItemIds = new List<string>();
    private readonly List<UserItem> _items = new List<UserItem>();
    private readonly List<UserItem> HBLLBGLBDGI = new List<UserItem>();
    public int Visible => _items.Count;
    public int Deliveries => HBLLBGLBDGI.Count;
    public int Missing => _missingModItemIds.Count;
    __PARSE__
    __ADD__
}
public static class Program
{
    private static void Assert(bool value, string message) { if (!value) throw new Exception(message); }
    public static int Main()
    {
        const string id = "example.weapon:items/weapon/example_blade";
        var save = new XmlDocument();
        save.LoadXml("<Warrior Weapon='" + id + "'><Items><Item Name='Fists' Count='1'/>" +
            "<Item Name='" + id + "' Count='1' UpgradeLevel='1201' Equipped='1' DeliveryTime='1234' Future='opaque'>" +
            "<Enchantments><Unrecognized data='preserve'/></Enchantments></Item></Items></Warrior>");
        XmlElement item = (XmlElement)save.DocumentElement["Items"].LastChild;
        string original = item.OuterXml;
        ListSF.Items.Definitions.Add("Fists", new ItemInfo());
        var missing = new UserItems();
        missing.Parse(save.DocumentElement["Items"]);
        Assert(missing.Visible == 1 && missing.Deliveries == 0 && missing.Missing == 1 && UserItem.Constructions == 1,
            "Missing mod item entered the constructor, active inventory, or delivery queue.");
        Assert(item.OuterXml == original, "Missing mod ownership was normalized or lost opaque children.");
        var reloaded = new XmlDocument();
        reloaded.LoadXml(save.OuterXml);
        var stillMissing = new UserItems();
        stillMissing.Parse(reloaded.DocumentElement["Items"]);
        Assert(stillMissing.Missing == 1 && reloaded.DocumentElement["Items"].LastChild.OuterXml == original,
            "Missing item changed across another save/load.");
        ListSF.Items.Definitions.Add(id, new ItemInfo());
        var restored = new UserItems();
        restored.Parse(reloaded.DocumentElement["Items"]);
        Assert(restored.Visible == 2 && restored.Missing == 0 && restored.Deliveries == 1,
            "Restored mod item did not return to inventory and pending deliveries.");
        Assert(reloaded.DocumentElement["Items"].LastChild.OuterXml == original,
            "Restoring the original equipped mod changed saved ownership or upgrades.");
        // The player chose a different weapon while this mod was absent. Do not re-equip it.
        reloaded.DocumentElement.SetAttribute("Weapon", "Fists");
        new UserItems().Parse(reloaded.DocumentElement["Items"]);
        item = (XmlElement)reloaded.DocumentElement["Items"].LastChild;
        Assert(item.GetAttribute("Equipped") == "0" && item.GetAttribute("Count") == "1" &&
            item.GetAttribute("UpgradeLevel") == "1201" && item["Enchantments"].FirstChild.Name == "Unrecognized",
            "Restoration overwrote a newer equipment choice or lost ownership state.");
        Console.WriteLine("Recovered inventory dispatch: PASS (missing, reload, restore, delivery exclusion, equipment choice).");
        return 0;
    }
}
'@
$code = $code.Replace('__PARSE__', $parse).Replace('__ADD__', $add)
$harness = Join-Path $testRoot 'Program.cs'
[IO.File]::WriteAllText($harness, $code, [Text.UTF8Encoding]::new($false))
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$csc = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\Roslyn\csc.exe' | Select-Object -First 1
if (!$csc) { throw 'Could not locate the Visual Studio Roslyn compiler.' }
$sources = @(Get-ChildItem -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding') -Filter '*.cs' | Select-Object -ExpandProperty FullName)
$exe = Join-Path $testRoot 'ModSaveRuntime.exe'
& $csc /nologo /langversion:9.0 /target:exe "/out:$exe" @sources $harness
if ($LASTEXITCODE -ne 0) { throw 'Inventory regression compilation failed.' }
& $exe
if ($LASTEXITCODE -ne 0) { throw 'Inventory regression failed.' }
