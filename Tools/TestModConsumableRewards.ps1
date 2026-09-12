$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root ('Temp/ConsumableRewards-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$source = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Assembly-CSharp/FightResult.cs')
$method = [regex]::Match($source, '(?ms)^\t\tpublic void KFJABAMAKOD\(RewardItem.*?^\t\t\}').Value
if (!$method) { throw 'Cannot extract recovered reward-item selection.' }
$code = @'
using System;
using System.Collections.Generic;
public class UserItem {}
public class UpgradeData {}
public class ItemInfo {
    public string Name, Type; public int MHGODOLNDLE = 1;
    public ItemInfo GetUpdateItemByLevel(int level, bool flag) => this;
    public ItemInfo HIOBANJPMKF(int level) => this;
    public List<UpgradeData> DNFDAGFAANJ(bool flag, int level) => new List<UpgradeData>();
    public ItemInfo MPADIPJLMLH(UpgradeData data) => this;
}
public class RewardItem {
    public string Name; public uint UpgradeNumber; public bool IDGKPLBKDIB = true;
    public string UpgradeLevelExpression; public int EvaluateUpgradeLevel() => 0;
    public int CMEFKONFDKN() => 1;
}
public class Roster {
    public HashSet<string> Owned = new HashSet<string>();
    public Dictionary<string, ItemInfo> Items = new Dictionary<string, ItemInfo>();
    public Roster KHCNHPCPFII() => this;
    public UserItem CMGOCLGHNLH(string name) => Owned.Contains(name) ? new UserItem() : null;
    public ItemInfo KCCDBEEKBCG(string name) => Items.TryGetValue(name, out var item) ? item : null;
    public int PINDEKDNCNL() => 1;
}
public static class ListSF {
    public static Roster Value = new Roster();
    public static Roster CCDKHLAMKKO() => Value;
    public static Roster DJBOFEEKJMP() => Value;
}
public class Result {
    public class LJFFIBFBGID { public ItemInfo DLKPBAJDHBO; public RewardItem NAIEGGHELIH; public bool IDGKPLBKDIB; }
    public List<LJFFIBFBGID> HELFDCAIJNE = new List<LJFFIBFBGID>();
    /* METHOD */
}
public static class Program {
    static void Check(string name, string type, bool owned, int expected) {
        ListSF.Value = new Roster();
        ListSF.Value.Items[name] = new ItemInfo { Name = name, Type = type };
        if (owned) ListSF.Value.Owned.Add(name);
        var result = new Result(); result.KFJABAMAKOD(new RewardItem { Name = name });
        if (result.HELFDCAIJNE.Count != expected) throw new Exception("Incorrect reward eligibility: " + name);
        if (expected == 1 && !result.HELFDCAIJNE[0].IDGKPLBKDIB) throw new Exception("Drop visibility lost.");
    }
    public static void Main() {
        Check("example.phase1:items/consumable/phase_token", "Consumable", false, 1);
        Check("example.phase1:items/consumable/phase_token", "Consumable", true, 1);
        Check("other.mod:items/weapon/sword", "Weapon", true, 0);
        Check("VANILLA_SWORD", "Weapon", true, 0);
        Check("VANILLA_TOKEN", "Consumable", true, 0);
        Check("core:items/consumable/token", "Consumable", true, 0);
        Console.WriteLine("PASS: mod consumable rewards repeat; owned equipment and core reward behavior preserved.");
    }
}
'@
$code.Replace('/* METHOD */', $method) | Set-Content -Encoding UTF8 (Join-Path $fixture 'Program.cs')
foreach ($name in @('DefinitionId.cs','ModId.cs')) {
    Copy-Item -LiteralPath (Join-Path $root ('Assets/Scripts/Eclipse/Runtime/Modding/' + $name)) -Destination $fixture
}
'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>' |
    Set-Content -Encoding UTF8 (Join-Path $fixture 'Regression.csproj')
dotnet run --project (Join-Path $fixture 'Regression.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Consumable reward regression failed.' }
