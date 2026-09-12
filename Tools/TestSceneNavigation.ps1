$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$fixture=Join-Path $root ('Temp/SceneNavigation-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$hostSource=Get-Content -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModRuntime.cs') -Raw -Encoding UTF8
$moduleSource=Get-Content -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Module.cs') -Raw -Encoding UTF8
$navigate=[regex]::Match($hostSource,'(?ms)^        internal static bool TryNavigateScene\(.*?^        \}')
$native=[regex]::Match($moduleSource,'(?ms)^\tpublic static bool DLOKJOHNDID\(ScreenType .*?^\t\}')
if(!$navigate.Success -or !$native.Success){throw 'Production navigation methods not found.'}
$source=Get-Content -LiteralPath (Join-Path $PSScriptRoot 'ValidateSceneNavigation.cs') -Raw -Encoding UTF8
$source.Replace('/* HOST */',$navigate.Value).Replace('/* NATIVE */',$native.Value) | Set-Content -LiteralPath (Join-Path $fixture 'Program.cs') -Encoding UTF8
$screen=[Security.SecurityElement]::Escape((Join-Path $root 'Assets/Scripts/Assembly-CSharp/ScreenType.cs'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Compile Include="$screen" />
</ItemGroup></Project>
"@ | Set-Content -LiteralPath (Join-Path $fixture 'Fixture.csproj') -Encoding UTF8
dotnet run --project (Join-Path $fixture 'Fixture.csproj')
if($LASTEXITCODE -ne 0){throw 'Scene navigation checks failed.'}
