<#
.SYNOPSIS
    Packs the files that differ between two player builds into a small patch zip.

.DESCRIPTION
    Compares every file in -New against -Old by SHA-256 and zips the new or changed
    ones, keeping their relative paths, so a player who has the old build extracts the
    zip over their game folder. Files that exist only in the old build are listed in
    REMOVED.txt inside the zip; leftovers are usually harmless but are named so they
    can be deleted by hand.

    The IL2CPP backup folder (*_BackUpThisFolder_ButDontShipItWithYourGame) is skipped.

.EXAMPLE
    .\Tools\MakeBuildPatch.ps1 -Old Builds\EclipseMP_old -New Builds\EclipseMP -Out Builds\EclipseMP-patch.zip
#>
param(
    [Parameter(Mandatory = $true)][string]$Old,
    [Parameter(Mandatory = $true)][string]$New,
    [Parameter(Mandatory = $true)][string]$Out
)

$ErrorActionPreference = 'Stop'
# Get-Item gives the same long-form path the file listing uses (Resolve-Path can keep 8.3 names).
$Old = (Get-Item -LiteralPath $Old).FullName.TrimEnd('\')
$New = (Get-Item -LiteralPath $New).FullName.TrimEnd('\')

function Get-BuildFiles([string]$root) {
    $table = @{}
    Get-ChildItem -LiteralPath $root -Recurse -File |
        Where-Object { $_.FullName -notmatch '_BackUpThisFolder_ButDontShipItWithYourGame' } |
        ForEach-Object { $table[$_.FullName.Substring($root.Length + 1)] = $_ }
    return $table
}

Write-Host "Scanning builds..."
$oldFiles = Get-BuildFiles $Old
$newFiles = Get-BuildFiles $New

$changed = New-Object System.Collections.Generic.List[string]
foreach ($path in $newFiles.Keys) {
    $file = $newFiles[$path]
    if (-not $oldFiles.ContainsKey($path)) { $changed.Add($path); continue }
    $before = $oldFiles[$path]
    # Different sizes differ; equal sizes need a hash.
    if ($before.Length -ne $file.Length) { $changed.Add($path); continue }
    if ((Get-FileHash -LiteralPath $before.FullName -Algorithm SHA256).Hash -ne
        (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash) { $changed.Add($path) }
}
$removed = @($oldFiles.Keys | Where-Object { -not $newFiles.ContainsKey($_) } | Sort-Object)

if ($changed.Count -eq 0 -and $removed.Count -eq 0) { Write-Host "The builds are identical; no patch written."; return }

$stage = Join-Path ([System.IO.Path]::GetTempPath()) ("eclipse-patch-" + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $stage | Out-Null
try {
    $bytes = 0
    foreach ($path in ($changed | Sort-Object)) {
        $target = Join-Path $stage $path
        New-Item -ItemType Directory -Force -Path (Split-Path $target) | Out-Null
        Copy-Item -LiteralPath $newFiles[$path].FullName -Destination $target
        $bytes += $newFiles[$path].Length
        Write-Host ("  changed  {0}  ({1:N1} MB)" -f $path, ($newFiles[$path].Length / 1MB))
    }
    if ($removed.Count -gt 0) {
        $removed | Set-Content -LiteralPath (Join-Path $stage 'REMOVED.txt') -Encoding utf8
        $removed | ForEach-Object { Write-Host "  removed  $_" }
    }
    if (Test-Path -LiteralPath $Out) { Remove-Item -LiteralPath $Out -Confirm:$false }
    Compress-Archive -Path (Join-Path $stage '*') -DestinationPath $Out -CompressionLevel Optimal
    $zip = Get-Item -LiteralPath $Out
    Write-Host ("{0} changed files ({1:N1} MB), {2} removed -> {3} ({4:N1} MB)" -f $changed.Count, ($bytes / 1MB), $removed.Count, $zip.FullName, ($zip.Length / 1MB))
}
finally {
    Remove-Item -LiteralPath $stage -Recurse -Force -Confirm:$false
}
