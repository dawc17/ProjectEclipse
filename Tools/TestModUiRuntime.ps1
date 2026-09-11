$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root ('Temp/ModUi-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force -Path $fixture | Out-Null
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/ModUiRuntime.cs') -Destination $fixture
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/ModId.cs') -Destination $fixture
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidateModUiRuntime.cs') -Destination (Join-Path $fixture 'Program.cs')
'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>' | Set-Content -Encoding UTF8 (Join-Path $fixture 'Ui.csproj')
dotnet run --project (Join-Path $fixture 'Ui.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Mod UI lifecycle fixture failed.' }
$controller = Get-Content -Raw -Encoding UTF8 (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Nekki/SF2/Core/Fights/Controller/GameController.cs')
if ([regex]::Matches($controller, '\bCallEvent\(').Count -ne 2 -or
    $controller -notmatch '(?s)private void Update\(\).*?SyncModUiCapture\(\);.*?NBMONJPAMHI.Render\(\);' -or
    $controller -notmatch '_modUiControls.SetCaptured\(Eclipse.UI.Modding.ModUiGameBridge.BlocksGameplayInput\)' -or
    $controller -notmatch 'eventType == 0 \? _modUiControls.Press\(key\) : _modUiControls.Release\(key\)') {
    throw 'Native fight inputs must route through the capture gate while continuing to observe physical release edges.'
}
$back = Get-Content -Raw -Encoding UTF8 (Join-Path $root 'Assets/Scripts/Assembly-CSharp/BackKeyManager.cs')
if ($back -notmatch 'public void OnBackKeyClicked\(\)\s*\{\s*if \(Eclipse.UI.Modding.ModUiGameBridge.TryHandleBack\(\)\) return;') {
    throw 'Mod UI must receive Back before the native screen stack.'
}
$dialogs = Get-Content -Raw -Encoding UTF8 (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Nekki/SF2/GUI/Dialogs/DialogCanvasController.cs')
if ($dialogs -notmatch 'public void BlockTouches\(\)\s*\{\s*Eclipse.UI.Modding.ModUiGameBridge.SetNativeBlocked\(true\);' -or
    $dialogs -notmatch 'public void UnBlockTouches\(\)\s*\{\s*Eclipse.UI.Modding.ModUiGameBridge.SetNativeBlocked\(false\);') {
    throw 'Native dialog input ownership must be signaled before raycaster changes.'
}
Write-Host 'PASS: native control, Back and dialog bridge source contracts (not a physical-device playtest).'
