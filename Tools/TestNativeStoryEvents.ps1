$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
& (Join-Path $PSScriptRoot 'TestPhase1ShowcaseRuntime.ps1')
$fixture=Join-Path $root ('Temp/NativeStoryEvents-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$runtime=Get-Content -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModRuntime.cs') -Raw -Encoding UTF8
$list=Get-Content -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ListSF.cs') -Raw -Encoding UTF8
$capture=[regex]::Match($runtime,'(?ms)^        internal static ModStoryEvent CaptureStoryEvent\(.*?^        \}')
$publish=[regex]::Match($runtime,'(?ms)^        internal static void PublishStoryEvent\(.*?^        \}')
$dispatch=[regex]::Match($list,'(?ms)^\tpublic bool FFBAJNGHGGD\(.*?^\t\}')
if(!$capture.Success -or !$publish.Success -or !$dispatch.Success){throw 'Production story methods not found.'}
$code=Get-Content -LiteralPath (Join-Path $PSScriptRoot 'ValidateNativeStoryEvents.cs') -Raw -Encoding UTF8
$code.Replace('/* HOST METHODS */',$capture.Value+[Environment]::NewLine+$publish.Value).Replace('/* NATIVE DISPATCH */',$dispatch.Value) | Set-Content -LiteralPath (Join-Path $fixture 'Program.cs') -Encoding UTF8
$production=[Security.SecurityElement]::Escape((Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content -LiteralPath (Join-Path $fixture 'Fixture.csproj') -Encoding UTF8
dotnet run --project (Join-Path $fixture 'Fixture.csproj') -- $root
if($LASTEXITCODE -ne 0){throw 'Native story checks failed.'}
