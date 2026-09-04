#requires -Version 7.0
# Build Assembly-CSharp.csproj first. Exercises the compiled action dispatcher,
# direction parser, binary clips, alignment and frame interpolation without Unity
# rendering, collision physics, UI or saves.
$ErrorActionPreference = 'Stop'
$projectPath = Split-Path $PSScriptRoot -Parent
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$assembly = Import-SF2ManagedRuntime $projectPath
$references = @(
    $assembly.Location,
    (Join-Path $projectPath 'Temp/Bin/Debug/Assembly-CSharp-firstpass.dll'),
    [UnityEngine.Vector3].Assembly.Location,
    'System.dll', 'System.Core.dll', 'System.Xml.dll'
)
if ($PSVersionTable.PSEdition -eq 'Core') {
    $references += Join-Path $PSHOME 'ref/mscorlib.dll'
    $references += Join-Path $PSHOME 'ref/netstandard.dll'
    $references += Join-Path $PSHOME 'ref/System.Collections.dll'
    $references += Join-Path $PSHOME 'ref/System.Runtime.Serialization.Formatters.dll'
    $references += Join-Path $PSHOME 'ref/System.Xml.ReaderWriter.dll'
    $references += Join-Path $PSHOME 'ref/System.Console.dll'
}
Add-Type -Path (Join-Path $PSScriptRoot 'ThrowRuntimeFixture.cs') -ReferencedAssemblies $references

# Model geometry now ships in the TAR/LZ4 art catalog, not gameplay XML.
# Use the actual packaged skeleton without invoking Unity's native Resources API.
$catalog = Get-Content -Raw (Join-Path $projectPath 'Assets/Resources/SF2Content/Art/catalog.json') | ConvertFrom-Json
$modelAddress = 'gamedata/models/mdl_skeleton'
$modelBundle = @($catalog.bundles | Where-Object { @($_.assets | Where-Object address -eq $modelAddress).Count -gt 0 })
if ($modelBundle.Count -ne 1) { throw 'Expected one packaged skeleton model bundle.' }
$record = New-Object Eclipse.Content.PackagedArtCatalog+BundleRecord
$record.name = $modelBundle[0].name
$record.namespaceId = $modelBundle[0].namespaceId
$testRoot = Join-Path $projectPath ('Temp/throw-tests/' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $testRoot -Force | Out-Null
$tarPath = Join-Path $testRoot 'models.tar'
$source = [IO.File]::OpenRead((Join-Path $projectPath ('Assets/StreamingAssets/SF2Content/ArtBundles/' + $modelBundle[0].file)))
$target = [IO.File]::Create($tarPath)
try {
    $decoder = $assembly.GetType('Eclipse.Content.TarAssets.Lz4FrameDecoder')
    $decoder.GetMethod('Decode').Invoke($null, @($source, $target, [long]$modelBundle[0].unpackedSize)) | Out-Null
} finally {
    $target.Dispose()
    $source.Dispose()
}
$bundle = New-Object Eclipse.Content.TarAssets.TarAssetBundle -ArgumentList $record,$tarPath
try {
    $skeletonText = $bundle.LoadText($modelAddress, 'model')
    if ([string]::IsNullOrEmpty($skeletonText)) { throw 'Packaged skeleton model is empty.' }
    [ThrowRuntimeFixture]::Run($projectPath, $skeletonText)
} finally { $bundle.Dispose() }
