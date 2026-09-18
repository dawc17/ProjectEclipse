$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root 'Temp/MovePerkLocks'
New-Item -ItemType Directory -Force $fixture | Out-Null

function Extract-Block([string]$source, [string]$signature) {
    $start = $source.IndexOf($signature, [StringComparison]::Ordinal)
    if ($start -lt 0) { throw "Cannot find production member: $signature" }
    $lineStart = $source.LastIndexOf("`n", $start); if ($lineStart -lt 0) { $lineStart = 0 } else { $lineStart++ }
    $brace = $source.IndexOf('{', $start); if ($brace -lt 0) { throw "Cannot find opening brace: $signature" }
    $depth = 0
    for ($i = $brace; $i -lt $source.Length; $i++) {
        if ($source[$i] -eq '{') { $depth++ }
        elseif ($source[$i] -eq '}') { $depth--; if ($depth -eq 0) { return $source.Substring($lineStart, $i - $lineStart + 1) } }
    }
    throw "Cannot find closing brace: $signature"
}

$runtime = Get-Content -Raw (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModAssetLoader.cs')
$rollback = Extract-Block $runtime 'internal sealed class MovePerkLockRollback'
$apply = Extract-Block $runtime 'internal static MovePerkLockRollback ApplyMovePerkLocks'
$remove = Extract-Block $runtime 'internal static void RemoveMovePerkLocks'
$projection = Join-Path $fixture 'ExternalCombatContentRuntime.cs'
@"
using System; using System.Collections.Generic; using System.Xml; using Eclipse.Modding;
internal static class ExternalCombatContentRuntime {
$rollback
$apply
$remove
}
"@ | Set-Content -Encoding UTF8 $projection

$project = Join-Path $fixture 'MovePerkLocks.csproj'
$test = Join-Path $root 'Tools/MovePerkLockTests.cs'
$content = Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding/ModMovePerkLocks.cs'
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><EnableDefaultCompileItems>false</EnableDefaultCompileItems><Nullable>disable</Nullable><LangVersion>latest</LangVersion></PropertyGroup><ItemGroup>
  <Compile Include="$([Security.SecurityElement]::Escape($content))" />
  <Compile Include="$([Security.SecurityElement]::Escape($projection))" />
  <Compile Include="$([Security.SecurityElement]::Escape($test))" />
</ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 $project

dotnet build $project -nologo --verbosity quiet
if ($LASTEXITCODE -ne 0) { throw "Move perk-lock fixture compile failed: $LASTEXITCODE" }
dotnet (Join-Path $fixture 'bin/Debug/net10.0/MovePerkLocks.dll') $root
if ($LASTEXITCODE -ne 0) { throw "Move perk-lock fixture failed: $LASTEXITCODE" }
