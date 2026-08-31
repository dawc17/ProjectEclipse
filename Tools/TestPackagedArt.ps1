param(
    [string]$Unity = 'F:\UnityInstalls\2022.3.62f3\Editor\Unity.exe',
    [ValidatePattern('^[A-Za-z0-9_-]+$')][string]$FixtureName = 'TarArtSmokeProject',
    [switch]$BuildPlayer
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root ('Temp/' + $FixtureName)

if (Test-Path -LiteralPath $fixture) {
    $resolvedFixture = (Resolve-Path -LiteralPath $fixture).Path
    $resolvedTemp = (Resolve-Path -LiteralPath (Join-Path $root 'Temp')).Path
    if (!$resolvedFixture.StartsWith($resolvedTemp + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to delete fixture outside Temp: $resolvedFixture"
    }
    Remove-Item -LiteralPath $resolvedFixture -Recurse -Force
}

foreach ($directory in @(
    'Assets/Resources/SF2Content/Art',
    'Assets/Resources/SF2Content/Fonts',
    'Assets/StreamingAssets/SF2Content/ArtBundles',
    'Assets/Scripts/Eclipse/Content/TarAssets',
    'Assets/Scripts/Eclipse/Runtime/Modding',
    'Assets/Scripts/Eclipse/Modding',
    'Packages',
    'ProjectSettings')) {
    New-Item -ItemType Directory -Path (Join-Path $fixture $directory) -Force | Out-Null
}

[IO.File]::WriteAllText((Join-Path $fixture 'Packages/manifest.json'),
    '{"dependencies":{"com.unity.modules.audio":"1.0.0","com.unity.modules.imageconversion":"1.0.0","com.unity.modules.jsonserialize":"1.0.0","com.unity.modules.unitywebrequest":"1.0.0","com.unity.modules.ui":"1.0.0","org.moonsharp.moonsharp":"https://github.com/moonsharp-devs/moonsharp.git?path=/interpreter#0fb8ba9106c44b140b8f56cb44cb1b50b358897c"}}')
[IO.File]::WriteAllText((Join-Path $fixture 'ProjectSettings/ProjectVersion.txt'), "m_EditorVersion: 2022.3.62f3`n")

Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Content/PackagedArtCatalog.cs') -Destination (Join-Path $fixture 'Assets/PackagedArtCatalog.cs')
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ResourcesAndBundles.cs') -Destination (Join-Path $fixture 'Assets/ResourcesAndBundles.cs')
Copy-Item -Path (Join-Path $root 'Assets/Scripts/Eclipse/Modding/*.cs') -Destination (Join-Path $fixture 'Assets/Scripts/Eclipse/Modding') -Force
Copy-Item -Path (Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/*.cs') -Destination (Join-Path $fixture 'Assets/Scripts/Eclipse/Runtime/Modding') -Force
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidatePackagedArt.cs') -Destination (Join-Path $fixture 'Assets/ValidatePackagedArt.cs')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'LegacyModdingStubs.cs') -Destination (Join-Path $fixture 'Assets/LegacyModdingStubs.cs')
Copy-Item -Path (Join-Path $root 'Assets/Scripts/Eclipse/Content/TarAssets/*') -Destination (Join-Path $fixture 'Assets/Scripts/Eclipse/Content/TarAssets') -Recurse -Force
Copy-Item -Path (Join-Path $root 'Assets/Resources/SF2Content/Art/*') -Destination (Join-Path $fixture 'Assets/Resources/SF2Content/Art') -Recurse -Force
Copy-Item -Path (Join-Path $root 'Assets/Resources/SF2Content/Fonts/*') -Destination (Join-Path $fixture 'Assets/Resources/SF2Content/Fonts') -Recurse -Force
Copy-Item -Path (Join-Path $root 'Assets/StreamingAssets/SF2Content/ArtBundles/*') -Destination (Join-Path $fixture 'Assets/StreamingAssets/SF2Content/ArtBundles') -Recurse -Force

$log = Join-Path $root ('Temp/packaged-art-' + [Guid]::NewGuid().ToString('N') + '.log')
$arguments = @('-batchmode', '-nographics', '-projectPath', ('"' + $fixture + '"'),
    '-executeMethod', 'ValidatePackagedArt.RunEditor', '-logFile', ('"' + $log + '"'))
if ($BuildPlayer) { $arguments += '-buildContentSmoke' }
$process = Start-Process -FilePath $Unity -ArgumentList $arguments -WindowStyle Hidden -Wait -PassThru
if ($process.ExitCode -ne 0) {
    Select-String -LiteralPath $log -Pattern 'error CS|Exception|\[PackagedArtTest\]|BuildFailed' -Context 0,3
    throw "Unity validation failed: $log"
}
$passed = Select-String -LiteralPath $log -Pattern '\[PackagedArtTest\] PASS'
if (!$passed) { throw "Unity did not finish the validator: $log" }
$passed.Line

if ($BuildPlayer) {
    $player = Join-Path $fixture 'Build/ContentSmoke.exe'
    $playerLog = Join-Path $root ('Temp/packaged-art-player-' + [Guid]::NewGuid().ToString('N') + '.log')
    $process = Start-Process -FilePath $player -WorkingDirectory (Split-Path $player) -ArgumentList @(
        '-batchmode', '-nographics', '-logFile', ('"' + $playerLog + '"')) -WindowStyle Hidden -Wait -PassThru
    $passed = Select-String -LiteralPath $playerLog -Pattern '\[PackagedArtTest\] PASS'
    if ($process.ExitCode -ne 0 -or !$passed) {
        Get-Content -LiteralPath $playerLog -Tail 60
        throw "Standalone content smoke failed: $playerLog"
    }
    $passed.Line
}
