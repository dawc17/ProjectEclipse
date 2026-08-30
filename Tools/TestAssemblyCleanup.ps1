$ErrorActionPreference = 'Stop'
$projectPath = Split-Path -Parent $PSScriptRoot
$sourcePath = Join-Path $projectPath 'Assets/Scripts/Assembly-CSharp'
# Compile the complete retained classes against minimal local dependencies.
# The fixture cannot construct HTTP forms, inspect devices, or invoke native Unity.
$session = Get-Content -Raw (Join-Path $sourcePath 'NetworkController.cs')
$backend = Get-Content -Raw (Join-Path $sourcePath 'Nekki/SF2/Core/Network/ServerProvider.cs')
$backend = $backend.Replace('namespace Nekki.SF2.Core.Network', 'namespace Backend')
$fakes = Get-Content -Raw (Join-Path $PSScriptRoot 'AssemblyCleanupFixture.cs')
Add-Type -TypeDefinition ($fakes + "`nnamespace AssemblyCleanupFixture.Session {`n" + $session + "`n}`n" +
    "namespace AssemblyCleanupFixture {`n" + $backend + "`n}`n")
[AssemblyCleanupFixture.Tests]::Run()
