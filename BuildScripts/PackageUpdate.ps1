param(
    [Parameter(Mandatory = $true)][string]$Version,
    [Parameter(Mandatory = $true)][string]$GameDirectory,
    [ValidateSet('stable','beta')][string]$Channel = 'stable',
    [string]$ReleaseTag = '',
    [string]$Notes = '',
    [string]$OutputDirectory = '',
    [string]$PreviousManifest = '',
    [string]$LegacyManifest = '',
    [switch]$IncludeLegacy
)
$ErrorActionPreference = 'Stop'
if ($Version -notmatch '^\d{1,6}\.\d{1,6}\.\d{1,6}$') { throw 'Version must be major.minor.patch.' }
if (!$ReleaseTag) { $ReleaseTag = if ($Channel -eq 'beta') { "v$Version-beta" } else { "v$Version" } }
if ($ReleaseTag -notmatch '^[A-Za-z0-9._-]+$') { throw 'Invalid release tag.' }
if ($ReleaseTag -eq 'beta') { throw 'Use a version-specific release tag; the beta release only holds channel manifests.' }
if ($PreviousManifest) { $PreviousManifest = (Resolve-Path -LiteralPath $PreviousManifest).Path }
if ($LegacyManifest) {
    if ($IncludeLegacy) { throw 'Choose IncludeLegacy or LegacyManifest, not both.' }
    $LegacyManifest = (Resolve-Path -LiteralPath $LegacyManifest).Path
    $legacy = Get-Content -LiteralPath $LegacyManifest -Raw | ConvertFrom-Json
    if ($legacy.format -ne 1 -or !$legacy.parts) { throw 'LegacyManifest must be a format 1 bridge manifest.' }
}
$GameDirectory = (Resolve-Path -LiteralPath $GameDirectory).Path
foreach ($file in @('Eclipse.exe','UnityPlayer.dll','Eclipse_Data')) {
    if (!(Test-Path -LiteralPath (Join-Path $GameDirectory $file))) { throw "Missing $file" }
}
# The player compares this stamp with the published version file; a mismatch would block every install.
$stampPath = Join-Path $GameDirectory 'Eclipse_Data/eclipse-version.txt'
if (!(Test-Path -LiteralPath $stampPath -PathType Leaf)) { throw "Unversioned build. Rebuild with BuildPlayers.ps1 -Target Windows -Version $Version." }
$stamped = (Get-Content -LiteralPath $stampPath -Raw).Trim()
if ($stamped -ne $Version) { throw "Build is stamped $stamped, not $Version. Rebuild with BuildPlayers.ps1 -Target Windows -Version $Version." }
if (!$OutputDirectory) { $OutputDirectory = Join-Path $PSScriptRoot "out/Releases/$Channel-$Version" }
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
if (Test-Path -LiteralPath $OutputDirectory) { throw "Output already exists: $OutputDirectory" }
if ($OutputDirectory.StartsWith($GameDirectory.TrimEnd('\') + '\', [StringComparison]::OrdinalIgnoreCase)) { throw 'Output must be outside the game directory.' }
New-Item -ItemType Directory -Path $OutputDirectory | Out-Null
& "$PSScriptRoot/BuildLauncher.ps1" -OutputDirectory (Join-Path $OutputDirectory 'launcher')
$compiler = Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
$source = Join-Path (Split-Path -Parent $PSScriptRoot) 'Launcher'
$builder = Join-Path $OutputDirectory 'PackageBuilder.exe'
& $compiler /nologo /target:exe "/out:$builder" /r:System.Web.Extensions.dll /r:System.IO.Compression.dll /r:System.IO.Compression.FileSystem.dll (Join-Path $source 'PackageBuilder.cs') (Join-Path $source 'UpdateCore.cs') (Join-Path $source 'IncrementalUpdate.cs')
if ($LASTEXITCODE -ne 0) { throw 'Package builder compilation failed.' }
# Windows PowerShell drops empty native arguments; pass paths/notes via quoted arguments.
$builderArgs = @($GameDirectory, $OutputDirectory, $Version, $Channel, $ReleaseTag, $Notes, $PreviousManifest)
$argumentLine = ($builderArgs | ForEach-Object { '"' + ($_ -replace '(\\*)"', '$1$1\"' -replace '(\\+)$', '$1$1') + '"' }) -join ' '
$process = Start-Process -FilePath $builder -ArgumentList $argumentLine -Wait -PassThru -NoNewWindow
if ($process.ExitCode -ne 0) { throw 'Incremental packaging failed.' }
Remove-Item -LiteralPath $builder
# Players poll this small file instead of the manifest to decide whether their build is outdated.
$versionFile = [ordered]@{ format = 1; version = $Version } | ConvertTo-Json -Compress
[IO.File]::WriteAllText((Join-Path $OutputDirectory "$Channel-version.json"), $versionFile, (New-Object Text.UTF8Encoding($false)))
if (!$IncludeLegacy) {
    if ($LegacyManifest) { Copy-Item -LiteralPath $LegacyManifest -Destination (Join-Path $OutputDirectory "$Channel.json") }
    Write-Output "Ready in $OutputDirectory. Upload the new .gz objects, launcher/EclipseLauncher.exe, $Channel-v2.json and $Channel-version.json. Nothing was published."
    return
}
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
$zipPath = Join-Path $OutputDirectory 'game.zip'
$zip = [IO.Compression.ZipFile]::Open($zipPath, [IO.Compression.ZipArchiveMode]::Create)
$unpacked = 0L
try {
    foreach ($file in Get-ChildItem -LiteralPath $GameDirectory -Recurse -File) {
        $relative = $file.FullName.Substring($GameDirectory.Length).TrimStart('\','/').Replace('\','/')
        if ($relative -match '^(Mods|versions|staging|launcher|download-cache)(/|$)' -or $relative -match '^launcher-state\.' -or
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
Write-Output "Bridge package ready in $OutputDirectory. Publish both manifests, $Channel-version.json and all new .gz/.partNNN assets. Do not upload game.zip. Nothing was published."
