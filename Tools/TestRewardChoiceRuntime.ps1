$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$testRoot = Join-Path $root 'Temp/RewardChoiceRuntime'
New-Item -ItemType Directory -Force -Path $testRoot | Out-Null
$source = Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/RewardChoice.cs')
$source = [regex]::Replace($source, '(?m)^using .*?\r?\n', '')
$code = @'
using System;
using System.Collections.Generic;
using System.Xml;
public static class XmlCompat
{
    public static float ParseFloat(this XmlAttribute attribute, float fallback = 0f)
    {
        if (attribute == null) return fallback;
        float value;
        return float.TryParse(attribute.Value, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out value) ? value : fallback;
    }
}
public abstract class Rewardable { public readonly string Kind; protected Rewardable(string kind) { Kind = kind; } }
public sealed class RewardItem : Rewardable { public RewardItem(XmlNode node) : base("Item") {} }
public sealed class RewardMoney : Rewardable { public RewardMoney(XmlNode node) : base("Money") {} }
public sealed class RewardCurrency : Rewardable { public RewardCurrency(XmlNode node) : base("Currency") {} }
public sealed class RewardResistance : Rewardable { public RewardResistance(XmlNode node) : base("Resistance") {} }
public sealed class RewardLottery : Rewardable { public RewardLottery(XmlNode node, ushort a, ushort b) : base("Lottery") {} }
public static class NekkiMath { public static float randomFloat(float min, float max) { return min; } }
'@ + "`r`n" + $source + @'

public static class Program
{
    public static int Main()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<Choice><Item Weight='2'/><Money Weight='1'/></Choice>");
        var choice = new RewardChoice(doc.DocumentElement);
        Rewardable selected = choice.OOOBLJIHBEP();
        if (selected == null || selected.Kind != "Item") throw new Exception("RewardChoice did not parse/select its first weighted child.");
        Console.WriteLine("RewardChoice runtime: PASS (backing list initialized; weighted child parse/select works).");
        return 0;
    }
}
'@
$harness = Join-Path $testRoot 'Program.cs'
[IO.File]::WriteAllText($harness, $code, [Text.UTF8Encoding]::new($false))
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$csc = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\Roslyn\csc.exe' | Select-Object -First 1
if (!$csc) { throw 'Could not locate Roslyn.' }
$exe = Join-Path $testRoot 'RewardChoiceRuntime.exe'
& $csc /nologo /langversion:9.0 /target:exe "/out:$exe" $harness
if ($LASTEXITCODE -ne 0) { throw 'RewardChoice regression compilation failed.' }
& $exe
if ($LASTEXITCODE -ne 0) { throw 'RewardChoice regression failed.' }
