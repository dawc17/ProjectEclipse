$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
& (Join-Path $PSScriptRoot 'TestDE128Foundation.ps1')
$production = Get-ChildItem (Join-Path $root 'Temp') -Directory -Filter 'DE128Foundation-*' | Sort-Object LastWriteTime -Descending | Select-Object -First 1
$assembly = Join-Path $production.FullName 'bin/Debug/net10.0/DE128Foundation.dll'
$fixture = Join-Path $root ('Temp/ForgePending-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $fixture | Out-Null
function Method([string]$file,[string]$signature) {
    $s=[IO.File]::ReadAllText((Join-Path $root ('Assets/Scripts/Assembly-CSharp/'+$file+'.cs')))
    $start=$s.IndexOf($signature); if($start -lt 0){throw "Missing $signature"}
    $open=$s.IndexOf('{',$start);$end=$open+1;$depth=1
    while($depth -gt 0){if($s[$end] -eq '{'){$depth++};if($s[$end] -eq '}'){$depth--};$end++}
    $s.Substring($start,$end-$start)
}
$test=[IO.File]::ReadAllText((Join-Path $PSScriptRoot 'ForgePendingTimerTests.cs'))
$user=@('public RecipeItemInfo PHDBCIHJKON()', 'public bool SetRecipeDelivery(', 'public void ClearRecipeDelivery()', 'private static void SetNodeAttribute(') | ForEach-Object {Method 'UserItem' $_}
$test=$test.Replace('// INSERT_USERITEMS',(Method 'UserItems' 'public bool FinishDeliveryRecipe('))
$test=$test.Replace('// INSERT_USERITEM',($user -join "`n"))
$test=$test.Replace('// INSERT_LISTSF',(Method 'ListSF' 'private void GHDNJMDEALP('))
$test=$test.Replace('// INSERT_FORGE',(Method 'ForgeManager' 'public bool FinishEnchant('))
[IO.File]::WriteAllText((Join-Path $fixture 'Program.cs'),$test)
Copy-Item (Join-Path $root 'Assets/Scripts/Assembly-CSharp/RecipeItemInfo.cs') $fixture
$assembly=[Security.SecurityElement]::Escape($assembly)
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup><Reference Include="DE128Foundation"><HintPath>$assembly</HintPath></Reference></ItemGroup></Project>
"@ | Set-Content (Join-Path $fixture 'ForgePending.csproj')
dotnet run --project (Join-Path $fixture 'ForgePending.csproj') -- (Join-Path $root 'Mods')
if($LASTEXITCODE -ne 0){throw 'Pending forge lifecycle regression failed.'}
