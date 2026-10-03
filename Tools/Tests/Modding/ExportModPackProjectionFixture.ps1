function Export-ModPackProjectionFixture([string]$Root, [string]$Destination) {
    $source = [IO.File]::ReadAllText((Join-Path $Root 'Assets/Scripts/Eclipse/Modding/LegacyContentAdapter.cs'))
    $blocks = foreach ($signature in @('private void ApplyCoreFightPatches(', 'private void RemoveStages(', 'private sealed class BattleSourceBinding', 'private static XmlElement FindFightNode(')) {
        $start = $source.IndexOf($signature)
        if ($start -lt 0) { throw "Missing production adapter block: $signature" }
        $open = $source.IndexOf('{', $start); $depth = 1; $end = $open + 1
        while ($depth -gt 0) { if ($source[$end] -eq '{') { $depth++ }; if ($source[$end] -eq '}') { $depth-- }; $end++ }
        $source.Substring($start, $end - $start)
    }
    ('using System; using System.Collections.Generic; using System.Globalization; using System.Xml; using Eclipse.Modding; public sealed partial class PackAdapter {' + ($blocks -join "`n") + '}') | Set-Content -LiteralPath (Join-Path $Destination 'ProductionPackAdapter.cs')
    # Reuse the current production builders extracted by the showcase prerequisite.
    $projection = [IO.File]::ReadAllText((Join-Path $Root 'Temp/Phase1ShowcaseRuntime/Projection.cs'))
    $projection = $projection.Replace('internal sealed class Projection {','internal sealed partial class Projection {')
    # The unused reward wrapper constructs an internal definition; the builders
    # themselves use the public catalog contracts and compile in this assembly.
    $projection = [regex]::Replace($projection, 'public XmlElement Reward\(.*?(?=public XmlDocument Location)', '')
    $projection | Set-Content -LiteralPath (Join-Path $Destination 'Projection.cs')
}
