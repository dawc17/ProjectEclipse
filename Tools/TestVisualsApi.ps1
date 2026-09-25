$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
& (Join-Path $PSScriptRoot 'TestPhase1ShowcaseRuntime.ps1')
$fixture=Join-Path $root ('Temp/VisualsApi-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$marker=Join-Path $fixture 'eclipse-visuals-api-fixture.marker'
'Owned Eclipse visuals API fixture' | Set-Content -Encoding UTF8 -LiteralPath $marker
try {
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidateVisualsApi.cs') -Destination (Join-Path $fixture 'Program.cs')
$production=[Security.SecurityElement]::Escape((Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference>
<Reference Include="MoonSharp.Interpreter"><HintPath>$root/Library/ScriptAssemblies/MoonSharp.Interpreter.dll</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj') -- (Join-Path $fixture 'Mods') (Join-Path $root 'Mods')
if ($LASTEXITCODE -ne 0) { throw 'Visuals API checks failed.' }
} finally {
    $workspace=[System.IO.Path]::GetFullPath($root)
    $target=[System.IO.Path]::GetFullPath($fixture)
    $fixtureRoot=[System.IO.Path]::GetFullPath((Join-Path $workspace 'Temp'))
    if ([System.IO.Path]::GetDirectoryName($target) -ne $fixtureRoot -or
        $fixtureRoot -notlike ($workspace.TrimEnd([System.IO.Path]::DirectorySeparatorChar) + [System.IO.Path]::DirectorySeparatorChar + '*') -or
        [System.IO.Path]::GetFileName($target) -notmatch '^VisualsApi-[0-9a-f]{32}$' -or
        !(Test-Path -LiteralPath $marker -PathType Leaf) -or
        (Get-Content -Raw -LiteralPath $marker).Trim() -ne 'Owned Eclipse visuals API fixture') {
        throw "Refusing to clean an unverified visuals API fixture: $target"
    }
    Remove-Item -LiteralPath $target -Recurse -Force
}
