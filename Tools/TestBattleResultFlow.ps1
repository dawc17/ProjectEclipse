$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$fixture=Join-Path $root ('Temp/BattleResultFlow-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$source=Get-Content -Raw -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/GameUtils.cs')
$method=[regex]::Match($source,'(?ms)^\tpublic static void EndFight\(.*?^\t\}')
if(!$method.Success){throw 'Native EndFight method not found.'}
$program=Get-Content -Raw -LiteralPath (Join-Path $PSScriptRoot 'ValidateBattleResultFlow.cs')
$program.Replace('/* END FIGHT */',$method.Value) | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Program.cs')
$sources=@('ModId.cs','DefinitionId.cs','ModStoryEvents.cs') | ForEach-Object {
 $path=[Security.SecurityElement]::Escape((Join-Path $root ('Assets/Scripts/Eclipse/Runtime/Modding/'+$_)))
 '<Compile Include="'+$path+'" />'
}
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
$($sources -join [Environment]::NewLine)
</ItemGroup></Project>
"@ | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $fixture 'Fixture.csproj')
dotnet run --project (Join-Path $fixture 'Fixture.csproj')
if($LASTEXITCODE -ne 0){throw 'Native battle result flow checks failed.'}
