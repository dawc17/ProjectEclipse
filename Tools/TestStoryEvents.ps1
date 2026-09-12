$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$fixture=Join-Path $root ('Temp/StoryEvents-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidateStoryEvents.cs') -Destination (Join-Path $fixture 'Program.cs')
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
if ($LASTEXITCODE -ne 0) { throw 'Story event checks failed.' }
