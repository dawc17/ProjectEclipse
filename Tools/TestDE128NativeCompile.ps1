# Compile the isolated editor driver with the project's real assembly references
# before creating/booting a Unity clone. No file is imported into the owner editor.
$ErrorActionPreference='Stop'
$root=Split-Path -Parent $PSScriptRoot
$null=New-Item -ItemType Directory -Force (Join-Path $root 'Temp')
$target=Join-Path $root ('Temp/DE128NativeCompile-'+[Guid]::NewGuid().ToString('N')+'.targets')
$source=[Security.SecurityElement]::Escape((Join-Path $PSScriptRoot 'ValidateDE128CombatNative.cs'))
('<Project><ItemGroup Condition="''$(MSBuildProjectName)'' == ''Assembly-CSharp-Editor''"><Compile Include="{0}" /></ItemGroup></Project>' -f $source) | Set-Content -LiteralPath $target
dotnet build (Join-Path $root 'Assembly-CSharp-Editor.csproj') --nologo -v:q /clp:ErrorsOnly "/p:CustomAfterMicrosoftCommonTargets=$target"
if($LASTEXITCODE -ne 0){throw 'DE128 native driver compile check failed; Unity was not launched.'}
