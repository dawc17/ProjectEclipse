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
[ThrowRuntimeFixture]::Run($projectPath)
