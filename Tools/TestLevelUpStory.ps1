$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$fixture=Join-Path $root ('Temp/LevelUpStory-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$roster=Get-Content -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Roster.cs') -Raw -Encoding UTF8
$hostSource=Get-Content -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModRuntime.cs') -Raw -Encoding UTF8
$gain=[regex]::Match($roster,'(?ms)^\tpublic bool DBPBGBNHAIP\(.*?^\t\}')
$threshold=[regex]::Match($roster,'(?ms)^\tpublic uint HEOHJNFGEDH\(.*?^\t\}')
$publish=[regex]::Match($hostSource,'(?ms)^        internal static void PublishLevelUp\(.*?^        \}')
if(!$gain.Success -or !$threshold.Success -or !$publish.Success){throw 'Production level methods not found.'}
$code=Get-Content -LiteralPath (Join-Path $PSScriptRoot 'ValidateLevelUpStory.cs') -Raw -Encoding UTF8
$code.Replace('/* ROSTER METHODS */',$gain.Value+[Environment]::NewLine+$threshold.Value).Replace('/* HOST METHOD */',$publish.Value) | Set-Content -LiteralPath (Join-Path $fixture 'Program.cs') -Encoding UTF8
$sources=@('ModId.cs','DefinitionId.cs','ModStoryEvents.cs') | ForEach-Object {
 $path=[Security.SecurityElement]::Escape((Join-Path $root ('Assets/Scripts/Eclipse/Runtime/Modding/'+$_)))
 '<Compile Include="'+$path+'" />'
}
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
$($sources -join [Environment]::NewLine)
</ItemGroup></Project>
"@ | Set-Content -LiteralPath (Join-Path $fixture 'Fixture.csproj') -Encoding UTF8
dotnet run --project (Join-Path $fixture 'Fixture.csproj')
if($LASTEXITCODE -ne 0){throw 'Level-up story checks failed.'}
