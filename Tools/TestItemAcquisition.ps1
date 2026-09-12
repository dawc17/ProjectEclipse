$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
& (Join-Path $PSScriptRoot 'TestPhase1ShowcaseRuntime.ps1')
$fixture=Join-Path $root ('Temp/ItemAcquisition-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$runtime=Get-Content -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModRuntime.cs') -Raw -Encoding UTF8
$list=Get-Content -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/ListSF.cs') -Raw -Encoding UTF8
$publish=[regex]::Match($runtime,'(?ms)^        internal static void PublishItemAcquired\(.*?^        \}')
$acquire=[regex]::Match($list,'(?ms)^\tpublic static UserItem GEFDJDIINND\(ItemInfo .*?^\t\}')
if(!$publish.Success -or !$acquire.Success){throw 'Production acquisition methods missing'}
$inventory=Get-Content -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/UserItems.cs') -Raw -Encoding UTF8
$delivery=[regex]::Match($inventory,'(?ms)^\tpublic void GBLHFNGPIOF\(.*?^\t\}')
if(!$delivery.Success){throw 'Production delivery method missing'}
$code=Get-Content -LiteralPath (Join-Path $PSScriptRoot 'ValidateItemAcquisition.cs') -Raw -Encoding UTF8
$code.Replace('/* PUBLISH */',$publish.Value).Replace('/* ACQUIRE */',$acquire.Value).Replace('/* DELIVERY */',$delivery.Value) | Set-Content -LiteralPath (Join-Path $fixture 'Program.cs') -Encoding UTF8
$production=[Security.SecurityElement]::Escape((Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup><Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference></ItemGroup></Project>
"@ | Set-Content -LiteralPath (Join-Path $fixture 'Fixture.csproj') -Encoding UTF8
dotnet run --project (Join-Path $fixture 'Fixture.csproj')
if($LASTEXITCODE -ne 0){throw 'Item acquisition checks failed'}
