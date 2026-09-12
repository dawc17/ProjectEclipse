$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root ('Temp/InnateLua-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture -Force | Out-Null
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidateModInnateLua.cs') -Destination (Join-Path $fixture 'Program.cs')
[xml]$project = Get-Content -Raw -LiteralPath (Join-Path $root 'Assembly-CSharp.csproj')
$refs = @($project.Project.ItemGroup.Reference | Where-Object { $_.HintPath } | ForEach-Object { [string]$_.HintPath })
$refs += @('Eclipse.Runtime','Assembly-CSharp-firstpass','Assembly-CSharp') | ForEach-Object { Join-Path $root "Temp/Bin/Debug/$_.dll" }
$references = foreach ($path in ($refs | Select-Object -Unique)) {
    if (-not [IO.Path]::IsPathRooted($path)) { $path = Join-Path $root $path }
    if (Test-Path -LiteralPath $path) {
        '<Reference Include="' + [IO.Path]::GetFileNameWithoutExtension($path) + '"><HintPath>' + [Security.SecurityElement]::Escape($path) + '</HintPath></Reference>'
    }
}
'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup>' + ($references -join "`n") + '</ItemGroup></Project>' | Set-Content -Encoding utf8 (Join-Path $fixture 'Innate.csproj')
dotnet run --project (Join-Path $fixture 'Innate.csproj') -- (Join-Path $fixture 'Mods')
if ($LASTEXITCODE -ne 0) { throw 'Real runtime innate Lua invocation failed.' }
