function Export-RoundOutcomeFixture([string]$Root, [string]$Destination) {
    $source = [IO.File]::ReadAllText((Join-Path $Root 'Assets/Scripts/Assembly-CSharp/Fight.cs'))
    $blocks = foreach ($signature in @('private bool TryQueueRoundOutcome(', 'private void RenderRound()', 'private void EndRound(', 'private ModelParameters GetWinner(', 'private void FinishRound()', 'private void FinishStance(', 'private void HCNDAFDHACI(')) {
        $start = $source.IndexOf($signature)
        if ($start -lt 0) { throw "Missing production method: $signature" }
        $open = $source.IndexOf('{', $start); $depth = 1; $end = $open + 1
        while ($depth -gt 0) { if ($source[$end] -eq '{') { $depth++ }; if ($source[$end] -eq '}') { $depth-- }; $end++ }
        $source.Substring($start, $end - $start)
    }
    ('using System; using System.Collections.Generic; using Eclipse.Modding; using CodeStage.AntiCheat.ObscuredTypes; public sealed partial class Fight {' + ($blocks -join "`n") + '}') | Set-Content -LiteralPath (Join-Path $Destination 'ProductionFight.cs')
    foreach ($file in @('EndRoundType.cs','BattleType.cs','RuleAppliance.cs','GameOverTypes.cs')) {
        Copy-Item -LiteralPath (Join-Path $Root ('Assets/Scripts/Assembly-CSharp/'+$file)) -Destination $Destination
    }
}
