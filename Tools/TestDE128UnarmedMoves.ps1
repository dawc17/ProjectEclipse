# Archive equivalence for DE128's restored default-moveset additions
# (Mods/de128/scripts/content/unarmed_moves.lua). Projects the Lua through the
# real binding and legacy adapter, then compares each move with the archived
# Definitive Edition moves.xml. No game save or live fight.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
dotnet build (Join-Path $root 'Assembly-CSharp.csproj') -v:q /clp:ErrorsOnly
if ($LASTEXITCODE -ne 0) { throw 'Managed build failed.' }
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$null = [Reflection.Assembly]::LoadFrom((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
$null = [MoonSharp.Interpreter.Script]::DefaultOptions
$null = Import-SF2ManagedRuntime $root

$fixture = Join-Path $root ('Temp/DE128UnarmedMoves-' + [Guid]::NewGuid().ToString('N'))
$package = Join-Path $fixture 'Mods/de128'
$null = New-Item -ItemType Directory -Force (Join-Path $package 'scripts/content')
Copy-Item -Recurse -LiteralPath (Join-Path $root 'Mods/de128/assets') -Destination $package
Copy-Item -Recurse -LiteralPath (Join-Path $root 'Mods/de128/localizations') -Destination $package
@'
schema = 1
id = "de128"
name = "DE128 unarmed move archive check"
version = "1.0.0"
authors = ["Eclipse tests"]
entrypoint = "scripts/main.lua"
capabilities = ["content.register", "content.patch"]

[[dependencies]]
id = "core"
version = ">=1.0 <2.0"
'@ | Set-Content (Join-Path $package 'mod.toml')
Copy-Item (Join-Path $root 'Mods/de128/scripts/content/unarmed_moves.lua') (Join-Path $package 'scripts/content/unarmed_moves.lua')
'require("content.unarmed_moves")' | Set-Content (Join-Path $package 'scripts/main.lua')

try {
    $mod = [Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $fixture 'Mods')).Mods[0]
    if ($null -eq $mod) { throw 'Fixture discovery failed.' }
    $catalog = [Eclipse.Modding.ModContentCatalog]::new()
    $tx = $catalog.BeginRegistration($mod)
    $context = $null
    try {
        $assets = [Eclipse.Modding.AssetResolver]::new([Eclipse.Modding.IAssetProvider[]]@([Eclipse.Modding.LooseModProvider]::new($mod)))
        $api = [Eclipse.Modding.ModApiFacade]::new($mod, $assets, $tx, $null)
        [Eclipse.Modding.ModLocalizationLoader]::Load($mod, $assets, $tx)
        $context = [Eclipse.Modding.MoonSharpScriptRuntime]::new().CreateContext($mod, $api)
        $null = $context.ExecuteEntrypoint(); $null = $tx.Commit()
    } finally { if ($null -ne $context) { $context.Dispose() }; $tx.Dispose() }
    $null = $catalog.Freeze()
    $flags = [Reflection.BindingFlags]'Instance,NonPublic'
    $adapter = [Eclipse.Modding.LegacyContentAdapter]::new($catalog)
    $document = [Eclipse.Modding.LegacyContentAdapter].GetMethod('BuildMovesDocument', $flags).Invoke($adapter, @())
    $projected = Join-Path $fixture 'projected.xml'
    $document.Save($projected)
    python (Join-Path $PSScriptRoot 'CompareDE128UnarmedMoves.py') $projected (Join-Path $root 'Assets/DExml/animations/moves.xml')
    if ($LASTEXITCODE -ne 0) { throw 'Restored unarmed moves differ from the archive.' }
} finally {
    Remove-Item -LiteralPath $fixture -Recurse -Force -ErrorAction SilentlyContinue
}
