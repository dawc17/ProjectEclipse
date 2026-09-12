$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
& (Join-Path $PSScriptRoot 'TestPhase1ShowcaseRuntime.ps1')
$fixture=Join-Path $root ('Temp/RewardNative-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$aliases="using ObscuredLong = System.Int64;`nusing ObscuredUInt = System.UInt32;`nusing ObscuredFloat = System.Single;`n"
foreach($name in @('Reward','RewardStruct','RewardPrize','RewardChoice','RewardLottery','Rewardable','LogMessage','MANJCIGJPMK')) {
 $source=Get-Content -LiteralPath (Join-Path $root ('Assets/Scripts/Assembly-CSharp/'+$name+'.cs')) -Raw -Encoding UTF8
 ($aliases+$source.Replace('using CodeStage.AntiCheat.ObscuredTypes;','')) | Set-Content -LiteralPath (Join-Path $fixture ($name+'.cs')) -Encoding UTF8
}
$adapter=Get-Content -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/LegacyContentAdapter.cs') -Raw -Encoding UTF8
$methods=@('BuildRewardNode','BuildRewardItemNode','LegacyItemName') | ForEach-Object {
 $m=[regex]::Match($adapter,'(?ms)^        private (?:XmlElement|string) '+$_+'\(.*?^        \}')
 if(!$m.Success){throw "Missing production method $_"};$m.Value
}
$item=Get-Content -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/RewardItem.cs') -Raw -Encoding UTF8
$constructor=[regex]::Match($item,'(?ms)^\tpublic RewardItem\(.*?^\t\}')
if(!$constructor.Success){throw 'Missing native item constructor'}
$resultSource=Get-Content -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/FightResult.cs') -Raw -Encoding UTF8
$selectItem=[regex]::Match($resultSource,'(?ms)^\t\tpublic void KFJABAMAKOD\(RewardItem .*?^\t\t\}')
if(!$selectItem.Success){throw 'Missing native result item selector'}
$selectLottery=[regex]::Match($resultSource,'(?ms)^\t\tpublic void KFJABAMAKOD\(RewardLottery .*?^\t\t\}')
if(!$selectLottery.Success){throw 'Missing native result lottery selector'}
$runtime=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModRuntime.cs')
$buildLottery=[regex]::Match($runtime,'(?ms)^        internal static FightResult.ResultPrizeStruct BuildLotteryPrize\(.*?^        \}')
if(!$buildLottery.Success){throw 'Missing lottery prize builder'}
$program=Get-Content -LiteralPath (Join-Path $PSScriptRoot 'ValidateRewardNative.cs') -Raw -Encoding UTF8
$program.Replace('/* BUILDERS */',($methods -join "`n")).Replace('/* ITEM CONSTRUCTOR */',$constructor.Value).Replace('/* ITEM SELECTION */',$selectItem.Value).Replace('/* LOTTERY SELECTION */',$selectLottery.Value).Replace('/* LOTTERY BUILDER */',$buildLottery.Value.Replace('FightResult.ResultPrizeStruct','Result')) | Set-Content -LiteralPath (Join-Path $fixture 'Program.cs') -Encoding UTF8
$production=[Security.SecurityElement]::Escape((Join-Path $root 'Temp/Phase1ShowcaseRuntime/bin/Debug/net10.0/Phase1ShowcaseRuntime.dll'))
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
<Reference Include="Phase1ShowcaseRuntime"><HintPath>$production</HintPath></Reference>
</ItemGroup></Project>
"@ | Set-Content -LiteralPath (Join-Path $fixture 'Fixture.csproj') -Encoding UTF8
dotnet run --project (Join-Path $fixture 'Fixture.csproj') -- (Join-Path $root 'Mods')
if($LASTEXITCODE -ne 0){throw 'Native reward checks failed.'}
