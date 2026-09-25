param([switch]$KeepFixture)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$modSource = Join-Path $root 'Mods/de128'
$moon = Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'
if (!(Test-Path -LiteralPath $moon -PathType Leaf)) {
    throw "Missing project MoonSharp assembly: $moon. Import the project in Unity first."
}
if (!(Test-Path -LiteralPath (Join-Path $modSource 'mod.toml') -PathType Leaf)) {
    throw "Missing DE128 package: $modSource"
}

# Each run owns a fresh directory. Allow a separate disk when the repository
# volume cannot hold the package copies made by the foundation matrix.
$fixtureRoot = if ($env:DE128_FOUNDATION_FIXTURE_ROOT) {
    [System.IO.Path]::GetFullPath($env:DE128_FOUNDATION_FIXTURE_ROOT)
} else {
    Join-Path $root 'Temp'
}
New-Item -ItemType Directory -Force -Path $fixtureRoot | Out-Null
$fixture = Join-Path $fixtureRoot ('DE128Foundation-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
try {
$runtimeSources = Get-ChildItem -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Runtime/Modding') -Filter '*.cs' -File
$bindingSources = Get-ChildItem -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding') -Filter 'MoonSharpScriptRuntime*.cs' -File
$compileFiles = @($runtimeSources.FullName) + @($bindingSources.FullName) + @(
    (Join-Path $PSScriptRoot 'DE128FoundationTests.cs'),
    (Join-Path $PSScriptRoot 'DE128ShopTests.cs'),
    (Join-Path $PSScriptRoot 'DE128EquipmentTests.cs'),
    (Join-Path $PSScriptRoot 'DE128UnderworldTests.cs'),
    (Join-Path $PSScriptRoot 'DE128UnderworldStoryTests.cs'),
    (Join-Path $PSScriptRoot 'SenseiDialogFixture.cs'),
    (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ShopAvailabilityPolicy.cs'),
    (Join-Path $root 'Assets/Scripts/Eclipse/Content/ItemListCompatibility.cs'),
    (Join-Path $PSScriptRoot 'DECombatPerksTests.cs'))
$compileXml = ($compileFiles | Sort-Object | ForEach-Object {
    '    <Compile Include="' + [Security.SecurityElement]::Escape($_) + '" />'
}) -join "`n"
$moonXml = [Security.SecurityElement]::Escape($moon)
$project = Join-Path $fixture 'DE128Foundation.csproj'
@"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <Nullable>disable</Nullable>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>
  <ItemGroup>
$compileXml
    <Reference Include="MoonSharp.Interpreter">
      <HintPath>$moonXml</HintPath>
      <Private>true</Private>
    </Reference>
  </ItemGroup>
</Project>
"@ | Set-Content -Encoding UTF8 -LiteralPath $project

Write-Output "DE128 fixture: $fixture"
dotnet build $project --nologo --verbosity quiet
if ($LASTEXITCODE -ne 0) { throw "DE128 fixture compilation failed: $LASTEXITCODE" }
dotnet (Join-Path $fixture 'bin/Debug/net10.0/DE128Foundation.dll') $modSource $fixture $root
if ($LASTEXITCODE -ne 0) { throw "DE128 foundation checks failed: $LASTEXITCODE" }
} finally {
    if (-not $KeepFixture) {
        $ownedRoot = [System.IO.Path]::TrimEndingDirectorySeparator([System.IO.Path]::GetFullPath($fixtureRoot))
        $ownedFixture = [System.IO.Path]::GetFullPath($fixture)
        if ([System.IO.Path]::GetDirectoryName($ownedFixture) -ine $ownedRoot -or
            [System.IO.Path]::GetFileName($ownedFixture) -notmatch '^DE128Foundation-[0-9a-f]{32}$') {
            throw "Refusing to clean fixture outside its owned root: $ownedFixture"
        }
        if (Test-Path -LiteralPath $ownedFixture) {
            Remove-Item -LiteralPath $ownedFixture -Recurse -Force
        }
    }
}
