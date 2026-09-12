param(
    [Parameter(Mandatory=$true)][string]$Blender,
    [Parameter(Mandatory=$true)][string]$Suite,
    [Parameter(Mandatory=$true)][string]$Rig,
    [Parameter(Mandatory=$true)][string]$Blend
)
$ErrorActionPreference='Stop'
$bridge=Join-Path $PSScriptRoot 'GymnastBridge.py'
foreach ($required in @($Blender,$Suite,$Rig)) {
    if (-not (Test-Path -LiteralPath $required)) { throw "Required authoring path not found: $required" }
}
if (-not (Test-Path -LiteralPath $Blend)) {
    & $Blender --background --factory-startup --python-exit-code 1 --python $bridge -- prepare --suite $Suite --rig $Rig --output $Blend
    if ($LASTEXITCODE -ne 0) { throw 'Gymnast scene preparation failed.' }
}
# Launching an interactive Blender window is the explicit purpose of this command.
# The upstream addon is registered for this process without editing user preferences.
& $Blender --factory-startup $Blend --python-exit-code 1 --python $bridge -- open --suite $Suite --rig $Rig
if ($LASTEXITCODE -ne 0) { throw 'Gymnast authoring session failed.' }
