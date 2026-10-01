$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$fixture = Join-Path $root 'Temp/FloorStainChecks'
New-Item -ItemType Directory -Force $fixture | Out-Null
$source = [Security.SecurityElement]::Escape((Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Presentation/FloorStainMath.cs'))
$tests = [Security.SecurityElement]::Escape((Join-Path $root 'Tools/Tests/ValidateFloorStains.cs'))
@"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup>
  <ItemGroup><Compile Include="$source" /><Compile Include="$tests" /></ItemGroup>
</Project>
"@ | Set-Content -Encoding utf8 (Join-Path $fixture 'Checks.csproj')
dotnet run --project (Join-Path $fixture 'Checks.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Floor stain math checks failed.' }
