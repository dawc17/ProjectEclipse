param(
    [Parameter(Mandatory = $true)][string]$Version,
    [Parameter(Mandatory = $true)][string]$GameDirectory,
    [ValidateSet('stable','beta')][string]$Channel = 'stable',
    [string]$ReleaseTag = '',
    [string]$Notes = '',
    [string]$OutputDirectory = ''
)
$ErrorActionPreference = 'Stop'
if ($Version -notmatch '^\d{1,6}\.\d{1,6}\.\d{1,6}$') { throw 'Version must be major.minor.patch.' }
if (!$ReleaseTag) { $ReleaseTag = if ($Channel -eq 'beta') { 'beta' } else { "v$Version" } }
if ($ReleaseTag -notmatch '^[A-Za-z0-9._-]+$') { throw 'Invalid release tag.' }
$GameDirectory = (Resolve-Path -LiteralPath $GameDirectory).Path
foreach ($file in @('Eclipse.exe','UnityPlayer.dll','Eclipse_Data')) {
    if (!(Test-Path -LiteralPath (Join-Path $GameDirectory $file))) { throw "Missing $file" }
}
if (!$OutputDirectory) { $OutputDirectory = Join-Path $PSScriptRoot "out/Releases/$Channel-$Version" }
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
if (Test-Path -LiteralPath $OutputDirectory) { throw "Output already exists: $OutputDirectory" }
if ($OutputDirectory.StartsWith($GameDirectory.TrimEnd('\') + '\', [StringComparison]::OrdinalIgnoreCase)) { throw 'Output must be outside the game directory.' }
New-Item -ItemType Directory -Path $OutputDirectory | Out-Null
& "$PSScriptRoot/BuildLauncher.ps1" -OutputDirectory (Join-Path $OutputDirectory 'launcher')
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
$zipPath = Join-Path $OutputDirectory 'game.zip'
$zip = [IO.Compression.ZipFile]::Open($zipPath, [IO.Compression.ZipArchiveMode]::Create)
$unpacked = 0L
try {
    foreach ($file in Get-ChildItem -LiteralPath $GameDirectory -Recurse -File) {
        $relative = $file.FullName.Substring($GameDirectory.Length).TrimStart('\','/').Replace('\','/')
        if ($relative -match '^(Mods|versions|staging|launcher)(/|$)' -or $relative -match '^launcher-state\.' -or
            $relative -match '(^|/)(EclipseLauncher\.exe|launcher\.lock)$' -or $relative -match '(BackUpThisFolder_ButDontShipItWithYourGame|BurstDebugInformation_DoNotShip)') { continue }
        if ($file.Attributes -band [IO.FileAttributes]::ReparsePoint) { throw "Do not package links: $relative" }
        [IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $file.FullName, $relative, [IO.Compression.CompressionLevel]::Optimal) | Out-Null
        $unpacked += $file.Length
    }
    $launcher = Join-Path $OutputDirectory 'launcher/EclipseLauncher.exe'
    [IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $launcher, 'EclipseLauncher.exe', [IO.Compression.CompressionLevel]::Optimal) | Out-Null
    $unpacked += (Get-Item -LiteralPath $launcher).Length
} finally { $zip.Dispose() }
# Split the ZIP byte stream below GitHub's per-asset limit; the launcher rejoins it.
$parts = @()
$inputStream = [IO.File]::OpenRead($zipPath)
try {
    $buffer = New-Object byte[] (1MB)
    $index = 0
    while ($inputStream.Position -lt $inputStream.Length) {
        $name = 'Eclipse-{0}-win64.zip.part{1:D3}' -f $Version, $index
        $path = Join-Path $OutputDirectory $name
        $partStream = [IO.File]::Create($path)
        try {
            $remaining = 1900000000L
            while ($remaining -gt 0) {
                $read = $inputStream.Read($buffer, 0, [int][Math]::Min($buffer.Length, $remaining))
                if (!$read) { break }
                $partStream.Write($buffer, 0, $read)
                $remaining -= $read
            }
        } finally { $partStream.Dispose() }
        $parts += @{ url = "https://github.com/dawc17/ProjectEclipse/releases/download/$ReleaseTag/$name"; sha256 = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant(); size = (Get-Item -LiteralPath $path).Length }
        $index++
    }
} finally { $inputStream.Dispose() }
$manifest = @{ format = 1; version = $Version; notes = $Notes; unpackedSize = $unpacked; parts = $parts }
[IO.File]::WriteAllText((Join-Path $OutputDirectory "$Channel.json"), ($manifest | ConvertTo-Json -Depth 5), (New-Object Text.UTF8Encoding($false)))
Write-Output "Ready in $OutputDirectory. Upload $Channel.json and every .part file to release $ReleaseTag. Distribute launcher/EclipseLauncher.exe to players. No release was published."
