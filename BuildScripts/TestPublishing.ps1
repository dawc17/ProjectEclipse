$ErrorActionPreference = 'Stop'
$testRoot = Join-Path $PSScriptRoot ('out/Launcher/publishing-' + [Guid]::NewGuid().ToString('N'))
$game = Join-Path $testRoot 'game'
New-Item -ItemType Directory -Path (Join-Path $game 'Eclipse_Data') -Force | Out-Null
foreach ($name in @('Eclipse.exe', 'UnityPlayer.dll', 'Eclipse_Data/data')) {
    [IO.File]::WriteAllText((Join-Path $game $name), $name)
}
function Assert([bool]$Condition, [string]$Label) {
    if (!$Condition) { throw "FAIL $Label" }
    Write-Output "PASS $Label"
}
$stamp = Join-Path $game 'Eclipse_Data/eclipse-version.txt'
$package = Join-Path $testRoot 'first'
$rejected = $false
try { & "$PSScriptRoot/PackageUpdate.ps1" -Version 9.1.0 -GameDirectory $game -OutputDirectory $package -IncludeLegacy } catch { $rejected = $true }
Assert ($rejected -and !(Test-Path -LiteralPath $package)) 'unversioned build cannot be packaged'
[IO.File]::WriteAllText($stamp, '9.0.9')
$rejected = $false
try { & "$PSScriptRoot/PackageUpdate.ps1" -Version 9.1.0 -GameDirectory $game -OutputDirectory $package -IncludeLegacy } catch { $rejected = $true }
Assert ($rejected -and !(Test-Path -LiteralPath $package)) 'mismatched version stamp cannot be packaged'
[IO.File]::WriteAllText($stamp, '9.1.0')
& "$PSScriptRoot/PackageUpdate.ps1" -Version 9.1.0 -GameDirectory $game -OutputDirectory $package -IncludeLegacy
$versionFile = Get-Content -LiteralPath (Join-Path $package 'stable-version.json') -Raw | ConvertFrom-Json
Assert ($versionFile.format -eq 1 -and $versionFile.version -eq '9.1.0') 'package writes channel version file'
$publishingMock = @{}
$publishingMock.releases = @{
    'v9.1.0' = @{ id = 1; draft = $true; prerelease = $false; html_url = 'https://example.invalid/draft' }
    'v9.1.1' = @{ id = 2; draft = $true; prerelease = $false }
    'beta' = @{ id = 3; draft = $false; prerelease = $true }
}
$publishingMock.remoteAssets = @{ 1 = @{}; 2 = @{}; 3 = @{} }
$publishingMock.uploads = 0
$publishingMock.publishes = 0
# This function shadows gh for every publisher invocation; no network or real release writes.
function gh {
    $arguments = @($args)
    $global:LASTEXITCODE = 0
    if ($arguments[0] -eq 'api') {
        $endpoint = $arguments[1]
        if ($endpoint -match '/releases/tags/(.+)$') {
            return ($publishingMock.releases[$Matches[1]] | ConvertTo-Json -Compress)
        }
        if ($endpoint -match '/releases/(\d+)/assets\?per_page=100&page=(\d+)$') {
            $id = [int]$Matches[1]
            $page = [int]$Matches[2]
            $assets = @($publishingMock.remoteAssets[$id].Values | Select-Object -Skip (($page - 1) * 100) -First 100)
            return (ConvertTo-Json -InputObject $assets -Compress -Depth 5)
        }
    }
    if ($arguments[0] -eq 'release' -and $arguments[1] -eq 'upload') {
        $release = $publishingMock.releases[$arguments[2]]
        for ($i = 3; $i -lt $arguments.Length -and $arguments[$i] -ne '--repo'; $i++) {
            $path = $arguments[$i]
            $name = [IO.Path]::GetFileName($path)
            $publishingMock.remoteAssets[[int]$release.id][$name] = @{
                name = $name; size = (Get-Item -LiteralPath $path).Length; state = 'uploaded'
                digest = 'sha256:' + (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant()
            }
            $publishingMock.uploads++
        }
        return
    }
    if ($arguments[0] -eq 'release' -and $arguments[1] -eq 'edit') {
        $publishingMock.releases[$arguments[2]].draft = $false
        $publishingMock.publishes++
        return
    }
    throw ('Unexpected mocked gh command: ' + ($arguments -join ' '))
}
& "$PSScriptRoot/PublishUpdate.ps1" -PackageDirectory $package -ReleaseTag v9.1.0
$firstCount = $publishingMock.uploads
Assert ($firstCount -gt 3 -and $publishingMock.publishes -eq 0) 'upload verifies assets and keeps draft'
& "$PSScriptRoot/PublishUpdate.ps1" -PackageDirectory $package -ReleaseTag v9.1.0
Assert ($publishingMock.uploads -eq $firstCount) 'rerun skips verified uploads'
$original = $publishingMock.remoteAssets[1]['stable-v2.json'].digest
$publishingMock.remoteAssets[1]['stable-v2.json'].digest = 'sha256:wrong'
$rejected = $false
try { & "$PSScriptRoot/PublishUpdate.ps1" -PackageDirectory $package -ReleaseTag v9.1.0 } catch { $rejected = $true }
Assert ($rejected -and $publishingMock.publishes -eq 0) 'conflicting remote asset cannot publish'
$publishingMock.remoteAssets[1]['stable-v2.json'].digest = $original
& "$PSScriptRoot/PublishUpdate.ps1" -PackageDirectory $package -ReleaseTag v9.1.0 -Publish
Assert ($publishingMock.publishes -eq 1 -and $publishingMock.remoteAssets[1].ContainsKey('stable-version.json')) 'explicit publication after verification'

[IO.File]::WriteAllText((Join-Path $game 'Eclipse_Data/data'), 'changed')
[IO.File]::WriteAllText($stamp, '9.1.1')
$second = Join-Path $testRoot 'second'
& "$PSScriptRoot/PackageUpdate.ps1" -Version 9.1.1 -GameDirectory $game -OutputDirectory $second -PreviousManifest (Join-Path $package 'stable-v2.json') -LegacyManifest (Join-Path $package 'stable.json')
$before = $publishingMock.uploads
& "$PSScriptRoot/PublishUpdate.ps1" -PackageDirectory $second -ReleaseTag v9.1.1
Assert (($publishingMock.uploads - $before) -lt $firstCount) 'incremental publication uploads fewer assets'
$savedAssets = $publishingMock.remoteAssets[1]
$publishingMock.remoteAssets[1] = @{}
$rejected = $false
try { & "$PSScriptRoot/PublishUpdate.ps1" -PackageDirectory $second -ReleaseTag v9.1.1 -Publish } catch { $rejected = $true }
Assert ($rejected -and $publishingMock.publishes -eq 1) 'missing retained release blocks publication'
$publishingMock.remoteAssets[1] = $savedAssets
# Beta uses immutable payloads and updates the channel only after publication.
Copy-Item -LiteralPath (Join-Path $second 'stable-v2.json') -Destination (Join-Path $second 'beta-v2.json')
Copy-Item -LiteralPath (Join-Path $second 'stable.json') -Destination (Join-Path $second 'beta.json')
Copy-Item -LiteralPath (Join-Path $second 'stable-version.json') -Destination (Join-Path $second 'beta-version.json')
$publishingMock.releases['v9.1.1'].prerelease = $true
& "$PSScriptRoot/PublishUpdate.ps1" -PackageDirectory $second -ReleaseTag v9.1.1 -Channel beta -Publish
Assert ($publishingMock.publishes -eq 2 -and $publishingMock.remoteAssets[3].ContainsKey('beta-v2.json') -and
    $publishingMock.remoteAssets[3].ContainsKey('beta-version.json')) 'beta payload publishes before channel update'
Write-Output "Publishing tests passed with mocked GitHub; fixtures at $testRoot"
