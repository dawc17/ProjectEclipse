$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$fixture=Join-Path $root ('Temp/SceneStory-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$sceneSource=Get-Content -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Nekki/SF2/GUI/Scene.cs') -Raw -Encoding UTF8
$hostSource=Get-Content -LiteralPath (Join-Path $root 'Assets/Scripts/Eclipse/Modding/ModRuntime.cs') -Raw -Encoding UTF8
$awake=[regex]::Match($sceneSource,'(?ms)^\t\tprotected override void Awake\(\).*?^\t\t\}')
$publish=[regex]::Match($hostSource,'(?ms)^        internal static void PublishSceneEntry\(.*?^        \}')
if(!$awake.Success -or !$publish.Success){throw 'Scene hook methods not found.'}
$code=Get-Content -LiteralPath (Join-Path $PSScriptRoot 'ValidateSceneStory.cs') -Raw -Encoding UTF8
$code.Replace('/* AWAKE */',$awake.Value).Replace('/* PUBLISH */',$publish.Value) | Set-Content -LiteralPath (Join-Path $fixture 'Program.cs') -Encoding UTF8
$paths=@('Assets/Scripts/Eclipse/Runtime/Modding/ModId.cs','Assets/Scripts/Eclipse/Runtime/Modding/DefinitionId.cs','Assets/Scripts/Eclipse/Runtime/Modding/ModStoryEvents.cs','Assets/Scripts/Eclipse/Modding/ModSceneEntry.cs','Assets/Scripts/Assembly-CSharp/ScreenType.cs')
$sources=$paths | ForEach-Object { '<Compile Include="'+[Security.SecurityElement]::Escape((Join-Path $root $_))+'" />' }
@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>
$($sources -join [Environment]::NewLine)
</ItemGroup></Project>
"@ | Set-Content -LiteralPath (Join-Path $fixture 'Fixture.csproj') -Encoding UTF8
dotnet run --project (Join-Path $fixture 'Fixture.csproj')
if($LASTEXITCODE -ne 0){throw 'Scene story checks failed.'}
