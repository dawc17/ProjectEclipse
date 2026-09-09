$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$runtime = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModRuntime.cs')
$adapter = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Eclipse/Modding/LegacyContentAdapter.cs')
function Extract-Method($source, $signature) {
    $start = $source.IndexOf($signature)
    if ($start -lt 0) { throw "Missing production method: $signature" }
    $brace = $source.IndexOf('{', $start)
    $depth = 1
    for ($end = $brace + 1; $depth -gt 0; $end++) {
        if ($source[$end] -eq '{') { $depth++ }
        if ($source[$end] -eq '}') { $depth-- }
    }
    return $source.Substring($start, $end - $start)
}
$stage = Extract-Method $runtime 'public static void ApplyStageContent()'
$quest = Extract-Method $runtime 'public static void ApplyQuestContent()'
$kind = Extract-Method $adapter 'private static string BattleKindName('
$kinds = ([regex]::Matches($kind, 'case ModBattleKind\.(\w+)') | ForEach-Object { $_.Groups[1].Value }) -join ','
$winner = Extract-Method (Get-Content -Raw (Join-Path $root "Assets/Scripts/Assembly-CSharp/Fight.cs")) "private ModelParameters GetWinner("
$code = @'
using System;
using ObscuredFloat = System.Single;
using System.Collections.Generic;
public class LoadingModule { protected bool CHIHBINEGFL; public virtual void JLPMOKPFECK() {} }
public static class Debug { public static void Log(object x) {} public static void LogError(object x) {} }
public static class GameUtils { public static void InitVariables() {} public static void OEKOKKCILAG() {} }
public static class GameSettings { public static void OCIPKAONMOP() {} public static void LNNLDPLDABI() {} }
public static class GameLoader { public static void BJLLJHDFMOO() {} public static void POLKDKOOACO() {} public static void SetSound() {} }
public static class LocalizationManager { public static void Init() {} }
public class PerkTree { public static PerkTree GBPBIPFIOJH() => new PerkTree(); public void LJHPGKAOIAE() {} }
public class ListSF {
    public static int Parses;
    public static ListSF ELEBLBJKDBI() => new ListSF();
    public static ListSF CCDKHLAMKKO() => new ListSF();
    public void AFAKCAMAACM() {}
    public void IIKDNMBIHCM() { Parses++; Eclipse.Modding.ModRuntime.ApplyStageContent(); Eclipse.Modding.ModRuntime.ApplyQuestContent(); }
}
public enum EndRoundType { EndRoundTypeZeroHealth, Timeout }
public enum RuleAppliance { AppliancePlayer, ApplianceOpponent }
public class EndRule { public RuleAppliance IMINMDOFHMG() => RuleAppliance.AppliancePlayer; }
public class ModelParameters { public float Life; public float KKMCHCNOHMB() => Life; }
public class WinnerProbe {
    object KGKDKENMAOA;
    public ModelParameters NMNCKBPFCCP = new ModelParameters { Life = 1 };
    public ModelParameters AKBNKDBHCEO = new ModelParameters { Life = 0.1f };
    EndRoundType _endRoundType = EndRoundType.Timeout;
    EndRule _endFightRule = null;
    __WINNER__
    public ModelParameters Winner(bool winner) => GetWinner(winner);
}
namespace Eclipse.Modding {
    public static class ModModeRuntime { public static bool Enabled; public static bool IsRaid(object fight) => Enabled; }

    public enum ModBattleKind { __KINDS__ }
    public class ModContentException : Exception { public ModContentException(string s) : base(s) {} }
    public class Adapter {
        public bool FailStage, FailQuest;
        public void ApplyP3Content() {}
        public void ApplyStages(ListSF list) { if (FailStage) throw new Exception("stage failure"); }
        public void ApplyQuests(ListSF list) { if (FailQuest) throw new Exception("quest failure"); }
        __KIND__
        public static string RaidKind() => BattleKindName(ModBattleKind.Raid);
    }
    public class Catalog {
        public List<int> Zones = new List<int>(), Battles = new List<int>(), Fights = new List<int>(), Quests = new List<int>();
    }
    public class Session { public Catalog Content = new Catalog(); }
    public static class ModRuntime {
        public static Adapter _legacyContent;
        public static Session Scripts = new Session();
        public static int Shutdowns;
        public static void Shutdown() { Shutdowns++; _legacyContent = null; }
        public static void ApplyLocaleMetadata() {} public static void ApplyLegacyLocalization() {}
        __STAGE__
        __QUEST__
    }
}
public static class Program {
    public static int Main() {
        if (Eclipse.Modding.Adapter.RaidKind() != "RAID") throw new Exception("Raid XML mapping missing");
        var probe = new WinnerProbe();
        Eclipse.Modding.ModModeRuntime.Enabled = true;
        if (probe.Winner(true) != probe.AKBNKDBHCEO || probe.Winner(false) != probe.NMNCKBPFCCP) throw new Exception("Raid timeout granted a health-percentage victory");
        probe.AKBNKDBHCEO.Life = 0;
        if (probe.Winner(true) != probe.NMNCKBPFCCP || probe.Winner(false) != probe.AKBNKDBHCEO) throw new Exception("Exhausted raid boss did not lose");
        Eclipse.Modding.ModModeRuntime.Enabled = false; probe.AKBNKDBHCEO.Life = 0.1f;
        if (probe.Winner(true) != probe.NMNCKBPFCCP) throw new Exception("Ordinary timeout semantics changed");
        for (int failure = 0; failure < 3; failure++) {
            ListSF.Parses = 0;
            Eclipse.Modding.ModRuntime.Shutdowns = 0;
            Eclipse.Modding.ModRuntime._legacyContent = new Eclipse.Modding.Adapter { FailStage = failure == 1, FailQuest = failure == 2 };
            var parser = new ParseModule();
            parser.JLPMOKPFECK(); parser.JLPMOKPFECK();
            if (ListSF.Parses != 1) throw new Exception("Loader reparsed base content after mod failure");
            if (Eclipse.Modding.ModRuntime.Shutdowns != (failure == 0 ? 0 : 1)) throw new Exception("Incorrect mod shutdown");
        }
        Console.WriteLine("PASS: production raid mapping and ParseModule success/stage-failure/quest-failure complete once without reparsing base content.");
        return 0;
    }
}
'@
$code = $code.Replace('__KINDS__', $kinds).Replace('__KIND__', $kind).Replace('__STAGE__', $stage).Replace('__QUEST__', $quest).Replace('__WINNER__', $winner)
$testRoot = Join-Path $root 'Temp/ModStartupRuntime'
New-Item -ItemType Directory -Force -Path $testRoot | Out-Null
$harness = Join-Path $testRoot 'Program.cs'
[IO.File]::WriteAllText($harness, $code)
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$csc = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\Roslyn\csc.exe' | Select-Object -First 1
if (!$csc) { throw 'Could not locate Roslyn.' }
$exe = Join-Path $testRoot 'ModStartupRuntime.exe'
& $csc /nologo /langversion:9.0 /target:exe "/out:$exe" $harness (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ParseModule.cs')
if ($LASTEXITCODE -ne 0) { throw 'Startup regression compilation failed.' }
& $exe
if ($LASTEXITCODE -ne 0) { throw 'Startup regression failed.' }
