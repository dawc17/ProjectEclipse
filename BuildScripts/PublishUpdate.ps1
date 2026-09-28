param(
    [Parameter(Mandatory = $true)][string]$PackageDirectory,
    [Parameter(Mandatory = $true)][ValidatePattern('^[A-Za-z0-9._-]+$')][string]$ReleaseTag,
    [ValidateSet('stable','beta')][string]$Channel = 'stable',
    [switch]$Publish,
    [switch]$Plan
)
$ErrorActionPreference = 'Stop'
$repository = 'dawc17/ProjectEclipse'
$PackageDirectory = (Resolve-Path -LiteralPath $PackageDirectory).Path
if ($ReleaseTag -eq 'beta') { throw 'Payloads need a version-specific release tag.' }
$manifestPath = Join-Path $PackageDirectory "$Channel-v2.json"
$legacyPath = Join-Path $PackageDirectory "$Channel.json"
$manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
if ($manifest.format -ne 2) { throw 'Expected an incremental manifest.' }
# Every stable release carries the last bridge manifest so old launchers can upgrade.
if (!(Test-Path -LiteralPath $legacyPath)) { throw 'Provide a bridge manifest using PackageUpdate -IncludeLegacy or -LegacyManifest.' }
$legacy = Get-Content -LiteralPath $legacyPath -Raw | ConvertFrom-Json
if ($legacy.format -ne 1) { throw 'Invalid bridge manifest.' }
# Players refuse to start once this names a newer version than their own.
$versionPath = Join-Path $PackageDirectory "$Channel-version.json"
if (!(Test-Path -LiteralPath $versionPath)) { throw "Missing $Channel-version.json; repackage with the current PackageUpdate.ps1." }
$versionFile = Get-Content -LiteralPath $versionPath -Raw | ConvertFrom-Json
if ($versionFile.format -ne 1 -or $versionFile.version -ne $manifest.version) { throw 'Version file does not match the manifest.' }
$payloads = @{}
$references = @{}
foreach ($part in @($manifest.parts) + @($legacy.parts)) {
    if ($part.url -notmatch '^https://github\.com/dawc17/ProjectEclipse/releases/download/([A-Za-z0-9._-]+)/([A-Za-z0-9._-]+)$') { throw 'Invalid release URL.' }
    $tag = $Matches[1]
    $name = $Matches[2]
    if ($part.sha256 -notmatch '^[a-fA-F0-9]{64}$' -or $part.size -le 0) { throw 'Invalid release reference.' }
    if ($tag -eq $ReleaseTag) {
        $path = Join-Path $PackageDirectory $name
        if (!(Test-Path -LiteralPath $path) -or (Get-Item -LiteralPath $path).Length -ne $part.size -or
            (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash -ne $part.sha256) { throw "Missing or corrupt payload: $name" }
        $payloads[$name] = $path
    } else {
        $key = "$tag/$name"
        if ($references.ContainsKey($key) -and $references[$key].sha256 -ne $part.sha256) { throw "Conflicting reference: $key" }
        $references[$key] = $part
    }
}
$payloads['EclipseLauncher.exe'] = Join-Path $PackageDirectory 'launcher/EclipseLauncher.exe'
$payloads["$Channel-v2.json"] = $manifestPath
$payloads["$Channel.json"] = $legacyPath
$payloads["$Channel-version.json"] = $versionPath
if ($payloads.Count -gt 1000) { throw 'Package exceeds the GitHub 1000 asset limit.' }
foreach ($path in $payloads.Values) { if (!(Test-Path -LiteralPath $path -PathType Leaf)) { throw "Missing file: $path" } }
$bytes = ($payloads.Values | ForEach-Object { (Get-Item -LiteralPath $_).Length } | Measure-Object -Sum).Sum
Write-Output ("Release {0}: {1} local assets, {2:N0} upload bytes; {3} retained-release references." -f $ReleaseTag, $payloads.Count, $bytes, $references.Count)
if ($Plan) {
    Write-Output 'Local hashes checked. No network calls made; hosted references and existing uploads were not checked.'
    return
}
if (!(Get-Command gh -ErrorAction SilentlyContinue)) { throw 'Install GitHub CLI and run gh auth login, or use -Plan for local checks.' }
function Invoke-GhJson([string[]]$Arguments) {
    $result = & gh @Arguments
    if ($LASTEXITCODE -ne 0) { throw ('GitHub command failed: gh ' + ($Arguments -join ' ')) }
    return (($result -join [Environment]::NewLine) | ConvertFrom-Json)
}
$release = Invoke-GhJson @('api', "repos/$repository/releases/tags/$ReleaseTag")
if (!$release.draft) { throw 'Only draft payload releases can be uploaded or published by this script.' }
if (($Channel -eq 'beta') -ne [bool]$release.prerelease) { throw 'Beta payloads require a prerelease; stable payloads require a normal release.' }
$assetCache = @{}
function Get-Assets([string]$Tag, [object]$Release) {
    if ($assetCache.ContainsKey($Tag)) { return $assetCache[$Tag] }
    $found = @{}
    $page = 1
    do {
        $items = @(Invoke-GhJson @('api', "repos/$repository/releases/$($Release.id)/assets?per_page=100&page=$page"))
        foreach ($asset in $items) { $found[$asset.name] = $asset }
        $page++
    } while ($items.Count -eq 100)
    $assetCache[$Tag] = $found
    return $found
}
function Test-Asset([object]$Asset, [long]$Size, [string]$Hash) {
    return $null -ne $Asset -and $Asset.state -eq 'uploaded' -and $Asset.size -eq $Size -and $Asset.digest -eq "sha256:$($Hash.ToLowerInvariant())"
}
# Verify retained references before uploading anything. An old manifest alone is not proof of hosting.
foreach ($key in $references.Keys) {
    $tag, $name = $key.Split('/', 2)
    if (!$assetCache.ContainsKey($tag)) {
        $prior = Invoke-GhJson @('api', "repos/$repository/releases/tags/$tag")
        if ($prior.draft) { throw "Referenced release is still a draft: $tag" }
        $null = Get-Assets $tag $prior
    }
    $part = $references[$key]
    if (!(Test-Asset $assetCache[$tag][$name] $part.size $part.sha256)) {
        throw "Hosted reference missing or digest differs: $key. Retain releases; assets without GitHub digests require separate verification."
    }
}
$assets = Get-Assets $ReleaseTag $release
$newCount = @($payloads.Keys | Where-Object { !$assets.ContainsKey($_) }).Count
if ($assets.Count + $newCount -gt 1000) { throw 'Draft would exceed GitHub asset count limit.' }
# Small immutable assets make reruns cheap. Never clobber payloads or mismatched draft assets.
$pending = @()
foreach ($name in @($payloads.Keys | Sort-Object)) {
    $path = $payloads[$name]
    $hash = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash
    $size = (Get-Item -LiteralPath $path).Length
    if ($assets.ContainsKey($name)) {
        if (!(Test-Asset $assets[$name] $size $hash)) { throw "Existing asset differs: $name. Inspect the draft; nothing was overwritten." }
        Write-Output "Already uploaded: $name"
        continue
    }
    $pending += $path
}
# Batch paths to reduce CLI overhead while staying below Windows command-line limits.
for ($i = 0; $i -lt $pending.Count; $i += 8) {
    $batch = @($pending[$i..([Math]::Min($i + 7, $pending.Count - 1))])
    & gh release upload $ReleaseTag @batch --repo $repository
    if ($LASTEXITCODE -ne 0) { throw 'Upload failed. Rerun to skip verified completed assets.' }
}
# Re-read metadata and verify every upload before exposing a manifest.
$assetCache.Remove($ReleaseTag)
$assets = Get-Assets $ReleaseTag $release
foreach ($name in $payloads.Keys) {
    $path = $payloads[$name]
    if (!(Test-Asset $assets[$name] (Get-Item -LiteralPath $path).Length (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash)) {
        throw "Uploaded asset verification failed: $name"
    }
}
if (!$Publish) {
    Write-Output "Verified draft ready: $($release.html_url). Rerun with -Publish to make it available."
    return
}
if ($Channel -eq 'stable') {
    & gh release edit $ReleaseTag --repo $repository --draft=false --latest
    if ($LASTEXITCODE -ne 0) { throw 'Publishing failed.' }
} else {
    $index = Invoke-GhJson @('api', "repos/$repository/releases/tags/beta")
    if ($index.draft -or !$index.prerelease) { throw 'Create a public beta prerelease for channel manifests first.' }
    & gh release edit $ReleaseTag --repo $repository --draft=false --latest=false
    if ($LASTEXITCODE -ne 0) { throw 'Publishing beta payload failed.' }
    # Payloads are public before replacing the two independently readable channel manifests.
    & gh release upload beta $legacyPath $manifestPath --repo $repository --clobber
    if ($LASTEXITCODE -ne 0) { throw 'Beta channel update failed; payload release is public. Update the beta manifests before announcing it.' }
    # Last, so games only demand the update once the launcher can install it.
    & gh release upload beta $versionPath --repo $repository --clobber
    if ($LASTEXITCODE -ne 0) { throw 'Beta version file update failed; launchers offer the update, but games do not require it yet. Upload beta-version.json to beta.' }
}
Write-Output "Published $ReleaseTag."
