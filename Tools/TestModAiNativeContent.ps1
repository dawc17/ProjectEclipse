# Real compiled parser/reader/adapter with shipped vanilla XML and clip bytes.
# Prewarming the native cache bypasses Unity resource I/O only. No combat model is run.
param([string]$OutputPath)
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
msbuild (Join-Path $root 'Assembly-CSharp.csproj') /nologo /v:quiet /clp:ErrorsOnly
if ($LASTEXITCODE -ne 0) { throw 'Native AI content test requires a current managed build.' }
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$assembly = Import-SF2ManagedRuntime $root
[MovesMaps]::Init()
$source = [xml](Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/vanillaXml/animations/moves.xml'))
$document = [xml]'<Movesxml><Moves/><Triggers/></Movesxml>'
$null = $document.DocumentElement.PrependChild($document.ImportNode($source.SelectSingleNode('/Movesxml/Templates'), $true))
$nativeType = $assembly.GetType('InfoAnimation', $true)
$read = $nativeType.GetMethod('ReadAnimation', [Reflection.BindingFlags]'NonPublic,Instance')
$cache = $nativeType.GetMethod('DDPBDPEDIGC', [Reflection.BindingFlags]'NonPublic,Instance')
$adapter = $assembly.GetType('Eclipse.Modding.ModRuntime', $true).GetMethod('AiActionSnapshot', [Reflection.BindingFlags]'NonPublic,Static')
$checks = 0
function Check([bool]$value, [string]$message) { $script:checks++; if (!$value) { throw $message } }
$names = @('StepBack','StaffStepBack','HighKick','LowKick')
foreach ($name in $names) {
    $node = $source.SelectSingleNode("/Movesxml/Moves/Move[@Name='$name']")
    Check ($null -ne $node) "Missing vanilla move: $name"
    $null = $document.SelectSingleNode('/Movesxml/Moves').AppendChild($document.ImportNode($node, $true))
    $clip = [InfoAnimation]::new()
    $clip.FileName = $node.GetAttribute('FileName')
    $bytes = [IO.File]::ReadAllBytes((Join-Path $root ('Assets/Resources/gamedata/animations/binary/' + $clip.FileName)))
    $null = $read.Invoke($clip, [object[]]@(,$bytes))
    $null = $cache.Invoke($clip, $null)
    Check ($clip.DIHJOPGKGFO().Length -gt 0) "Clip reader returned no frames: $name"
}
$moves = [Collections.Generic.List[InfoAnimation]]::new()
$templates = [Collections.Generic.Dictionary[string,TemplateAnimation]]::new()
$tricks = [Collections.Generic.List[Trick]]::new()
$triggers = [Collections.Generic.List[Trigger]]::new()
$parse = $assembly.GetType('MovesParser', $true).GetMethod('ParseAdditional', [Reflection.BindingFlags]'NonPublic,Static')
$added = $parse.Invoke($null, [object[]]@($document,$moves,$templates,$tricks,$triggers))
Check ($added -eq 4 -and $moves.Count -eq 4) 'Native parser did not produce all selected moves'
$snapshots = @()
foreach ($move in $moves) {
    $snapshot = $adapter.Invoke($null, [object[]]@($move))
    Check ($snapshot.Name -eq $move.Name) 'Native adapter changed move identity'
    Check ($snapshot.Timing.LastSample -ge $snapshot.Timing.FirstSample) "Unresolved clip bounds: $($move.Name)"
    Check ($snapshot.Timing.NominalFrames -eq $move.ONLKMFOENEH()) "Native duration mismatch: $($move.Name)"
    Check ($move.DFKIHADCFKG() -eq 67) "Unexpected native rig size: $($move.Name)"
    if ($move.Name -like '*StepBack') {
        Check ($snapshot.Type -eq 'move') "Retreat is not classified as a move: $($move.Name)"
        Check (@($snapshot.Inputs | Where-Object { $_.Control -eq 'Back' -and $_.Press -in @('tap','hold') }).Count -gt 0) "Retreat control unavailable: $($move.Name)"
    } else {
        Check ($snapshot.Type -eq 'attack') "Kick is not classified as an attack: $($move.Name)"
        Check (@($snapshot.Inputs | Where-Object { $_.Control -eq 'Kick' -and $_.Press -eq 'tap' }).Count -gt 0) "Kick tap unavailable: $($move.Name)"
    }
    $snapshots += $snapshot
}
if ($OutputPath) { [IO.File]::WriteAllText([IO.Path]::GetFullPath($OutputPath), (ConvertTo-Json -InputObject $snapshots -Depth 8)) }
Write-Output "PASS: $checks real native parser/clip/AI metadata checks across four shipped 67-node moves. Resource I/O bypassed via native cache; combat eligibility and playback not exercised."
