param([string]$Unity = 'F:\UnityInstalls\6000.6.0f1\Editor\Unity.exe')

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root ('Temp/DE128ShopArt-' + [Guid]::NewGuid().ToString('N'))
$folders = @(
    'Assets/Editor', 'Assets/Resources/SF2Content/Art',
    'Assets/StreamingAssets/SF2Content/ArtBundles',
    'Assets/Scripts/Eclipse/Content/TarAssets',
    'Assets/vanillaXml', 'Assets/DExml', 'Packages', 'ProjectSettings'
)
foreach ($folder in $folders) { New-Item -ItemType Directory -Path (Join-Path $fixture $folder) -Force | Out-Null }

Set-Content -LiteralPath (Join-Path $fixture 'ProjectSettings/ProjectVersion.txt') -Value 'm_EditorVersion: 6000.6.0f1'
Set-Content -LiteralPath (Join-Path $fixture 'Packages/manifest.json') -Value '{"dependencies":{"com.unity.modules.audio":"1.0.0","com.unity.modules.imageconversion":"1.0.0","com.unity.modules.jsonserialize":"1.0.0","com.unity.modules.unitywebrequest":"1.0.0"}}'
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Content/PackagedArtCatalog.cs') -Destination (Join-Path $fixture 'Assets/PackagedArtCatalog.cs')
Copy-Item -Path (Join-Path $root 'Assets/Scripts/Eclipse/Content/TarAssets/*.cs') -Destination (Join-Path $fixture 'Assets/Scripts/Eclipse/Content/TarAssets')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'DE128ShopArtFixtureStubs.cs') -Destination (Join-Path $fixture 'Assets/ArtFixtureStubs.cs')
Copy-Item -LiteralPath (Join-Path $root 'Assets/Editor/ValidateDE128ShopArt.cs') -Destination (Join-Path $fixture 'Assets/Editor/ValidateDE128ShopArt.cs')
Copy-Item -LiteralPath (Join-Path $root 'Assets/Resources/SF2Content/Art/catalog.json') -Destination (Join-Path $fixture 'Assets/Resources/SF2Content/Art/catalog.json')
Copy-Item -LiteralPath (Join-Path $root 'Assets/vanillaXml/list.xml') -Destination (Join-Path $fixture 'Assets/vanillaXml/list.xml')
Copy-Item -LiteralPath (Join-Path $root 'Assets/DExml/list.xml') -Destination (Join-Path $fixture 'Assets/DExml/list.xml')
foreach ($bundle in Get-ChildItem -LiteralPath (Join-Path $root 'Assets/StreamingAssets/SF2Content/ArtBundles') -Filter '*.tar.lz4' -File) {
    New-Item -ItemType HardLink -Path (Join-Path $fixture "Assets/StreamingAssets/SF2Content/ArtBundles/$($bundle.Name)") -Target $bundle.FullName | Out-Null
}

$log = Join-Path $fixture 'unity.log'
$arguments = @('-batchmode', '-nographics', '-quit', '-projectPath', ('"' + $fixture + '"'),
    '-executeMethod', 'ValidateDE128ShopArt.RunBatch', '-logFile', ('"' + $log + '"'))
$process = Start-Process -FilePath $Unity -ArgumentList $arguments -WindowStyle Hidden -Wait -PassThru
if ($process.ExitCode -ne 0) {
    Select-String -LiteralPath $log -Pattern 'error CS|Exception|DE128 shop|Failed' -Context 0,2 | Select-Object -Last 30
    throw "DE128 Unity art validation failed: $log"
}
$passed = Select-String -LiteralPath $log -Pattern 'PASS DE128 shop art: 20 packaged sprites and 3 packaged models.'
if (!$passed) { throw "DE128 Unity art validator did not finish: $log" }
$passed.Line
