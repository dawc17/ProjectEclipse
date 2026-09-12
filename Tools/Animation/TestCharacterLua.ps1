param([Parameter(Mandatory=$true)][string]$Package, [switch]$Packaged)
$ErrorActionPreference='Stop'
$root=Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
& (Join-Path $root 'Tools/TestPhase1ShowcaseRuntime.ps1')
$fixture=Join-Path $root ('Temp/CharacterLua-'+[Guid]::NewGuid().ToString('N'))
$mod=Join-Path $fixture 'Mods/example.authored'
New-Item -ItemType Directory -Path (Join-Path $mod 'scripts') -Force | Out-Null
Copy-Item -LiteralPath (Join-Path $Package 'assets') -Destination $mod -Recurse
$module = if ($Packaged) { 'scripts/character.lua' } else { 'character.generated.lua' }
Copy-Item -LiteralPath (Join-Path $Package $module) -Destination (Join-Path $mod 'scripts/character.lua')
if ($Packaged) { Copy-Item -LiteralPath (Join-Path $Package 'localizations') -Destination $mod -Recurse }
@'
schema = 1
id = "example.authored"
name = "Authored character fixture"
authors = ["Eclipse"]
version = "1.0.0"
api = ">=0.22 <1.0"
entrypoint = "scripts/main.lua"
capabilities = ["content.register"]
[[dependencies]]
id = "core"
version = ">=1.0 <2.0"
'@ | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $mod 'mod.toml')
@'
local sf2=require('sf2')
local authored=require('character')
sf2.moves.register {
    id='attack',animation=sf2.assets.binary('animations/authored'),type='ATTACK',end_frame=59,
    events={'key_pressed'},
    conditions={{type='character',warrior=authored.warrior},{type='keys',keys={{key='Kick',press='Tap'}}}},
    intervals={{type='Attack',start=6,['end']=8,attack={edges={'ECalf_2'},damage=0.12,impulse={x=245,z=350}}}},
}
'@ | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $mod 'scripts/main.lua')
if ($Packaged) { Copy-Item -LiteralPath (Join-Path $Package 'scripts/main.lua') -Destination (Join-Path $mod 'scripts/main.lua') -Force }
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidateCharacterLua.cs') -Destination (Join-Path $fixture 'Program.cs')
$production=[Security.SecurityElement]::Escape((Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'))
$moon=[Security.SecurityElement]::Escape((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference>
<Reference Include="MoonSharp.Interpreter"><HintPath>$moon</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'CharacterLua.csproj')
dotnet run --project (Join-Path $fixture 'CharacterLua.csproj') -- (Join-Path $fixture 'Mods') (Join-Path $root 'Assets/vanillaXml/stages.xml') $Packaged.IsPresent
if ($LASTEXITCODE -ne 0) { throw 'Authored character Lua test failed.' }
