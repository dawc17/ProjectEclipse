# Exercises the pure interpolation clock from the compiled runtime and audits
# that interpolation remains confined to presentation code.
$ErrorActionPreference = 'Stop'
$projectPath = Split-Path $PSScriptRoot -Parent
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$assembly = Import-SF2ManagedRuntime $projectPath
$checks = 0
function Assert-True($condition, [string]$label) {
    if (!$condition) { throw $label }
    $script:checks++
}

$type = $assembly.GetType('Eclipse.Rendering.Interpolation.FightInterpolation', $true)
$method = $type.GetMethod('CalculateAlpha')
$cases = @(
    @(10.0, 10.0, 0.02, 0.0),
    @(10.01, 10.0, 0.02, 0.5),
    @(10.02, 10.0, 0.02, 1.0),
    @(9.99, 10.0, 0.02, 0.0),
    @(10.03, 10.0, 0.02, 1.0),
    @(10.0, 10.0, 0.0, 1.0)
)
foreach ($case in $cases) {
    $actual = [single]$method.Invoke($null, @([double]$case[0], [double]$case[1], [single]$case[2]))
    Assert-True ([Math]::Abs($actual - [single]$case[3]) -lt 0.0001) "Interpolation alpha mismatch: $actual"
}

$mesh = Get-Content -Raw (Join-Path $projectPath 'Assets/Scripts/Assembly-CSharp/MeshNode.cs')
$interpolation = Get-Content -Raw (Join-Path $projectPath 'Assets/Scripts/Eclipse/Rendering/Interpolation/FightInterpolation.cs')
$physics = Get-Content -Raw (Join-Path $projectPath 'Assets/Scripts/Assembly-CSharp/ModelPhysics.cs')
$collision = Get-Content -Raw (Join-Path $projectPath 'Assets/Scripts/Assembly-CSharp/ModelCollision.cs')
$frameRateFiles = @(
    'Assets/Plugins/Assembly-CSharp-firstpass/GameInit.cs',
    'Assets/Plugins/Assembly-CSharp-firstpass/SceneLoader.cs',
    'Assets/Scripts/Assembly-CSharp/Nekki/SF2/Core/ApplicationController.cs'
)

Assert-True ($mesh.Contains('FightInterpolation.SamplePosition')) 'Visible mesh does not use interpolation'
Assert-True ($interpolation.Contains('node.ICLEOFDKDIF()')) 'Interpolation does not read the authoritative current pose'
Assert-True ($interpolation.Contains('node.FOGHEPNAPLC()')) 'Interpolation does not read the previous pose'
Assert-True (!$physics.Contains('FightInterpolation')) 'Model physics depends on render interpolation'
Assert-True (!$collision.Contains('FightInterpolation')) 'Collision depends on render interpolation'
Assert-True (!$interpolation.Contains('AMPCKAIPIHH(')) 'Interpolation writes the authoritative current pose'
Assert-True (!$interpolation.Contains('LAHLFIKENPP(')) 'Interpolation writes the authoritative previous pose'
foreach ($relative in $frameRateFiles) {
    $source = Get-Content -Raw (Join-Path $projectPath $relative)
    Assert-True ($source.Contains('SF2DisplayFrameRate.Apply()')) "Frame cap remains in $relative"
    Assert-True (!$source.Contains('targetFrameRate = 60')) "Hard 60 FPS cap remains in $relative"
}

Write-Output "PASS: $checks frame interpolation assertions (clock, presentation-only reads, no simulation writes, no 60 FPS cap)."
