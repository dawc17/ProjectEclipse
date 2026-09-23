$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
& (Join-Path $PSScriptRoot 'TestPhase1ShowcaseRuntime.ps1')
$fixture = Join-Path $root ('Temp/SenseiNotifications-' + [Guid]::NewGuid().ToString('N'))
$content = Join-Path $fixture 'Mods/fixture.notify/scripts/content'
New-Item -ItemType Directory -Force $content | Out-Null
foreach ($name in @('sensei_progression','sensei_map','sensei_notification_text','sensei_notifications','sensei_state','sensei_art','sensei_dialog','sensei_defeat_text')) {
    Copy-Item (Join-Path $root "Mods/de128/scripts/content/$name.lua") $content
}
# The notification fallback portrait is a shipped DE128 sprite (absent from the core catalog).
Copy-Item -Recurse (Join-Path $root 'Mods/de128/assets') (Join-Path (Split-Path -Parent $content | Split-Path -Parent) 'assets')
Copy-Item (Join-Path $PSScriptRoot 'ValidateSenseiNotifications.cs') (Join-Path $fixture 'Program.cs')
Copy-Item (Join-Path $PSScriptRoot 'SenseiDialogFixture.cs') (Join-Path $fixture 'SenseiDialogFixture.cs')
$production = [Security.SecurityElement]::Escape((Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'))
$moon = [Security.SecurityElement]::Escape((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference>
<Reference Include="MoonSharp.Interpreter"><HintPath>$moon</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content (Join-Path $fixture 'Notify.csproj')
dotnet run --project (Join-Path $fixture 'Notify.csproj') -- (Join-Path $fixture 'Mods') $root
if ($LASTEXITCODE -ne 0) { throw 'Sensei notification fixture failed.' }
