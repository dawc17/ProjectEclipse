# Approved projections captured from the migrated content after archive equivalence
# checks. Compare through the compact-only Lua binding and
# adapter. Conditions are compared as sets and scheduled actions by trigger, since
# the native parser evaluates conditions together and fires actions per frame/event;
# actions sharing one trigger keep their order. No game save or live fight.
param(
    [string[]]$Files = @('war_whirl.lua', 'sphere1.lua', 'chinese_swords.lua', 'mind_throw.lua', 'sphere2.lua',
        'hermit_storm.lua', 'gatekeeper_power_field.lua', 'combo_sphere3.lua', 'butcher_earthquake.lua',
        'blackness_grasp.lua', 'widow_teleportation.lua', 'sphere3.lua', 'wasp_fly.lua', 'raid_boss_abilities.lua',
        'shared_moves.lua', 'dandy_lightning_chain.lua'),
    [switch]$ShowXml
)
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
dotnet build (Join-Path $root 'Assembly-CSharp.csproj') -v:q /clp:ErrorsOnly
if ($LASTEXITCODE -ne 0) { throw 'Managed build failed.' }
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$null = [Reflection.Assembly]::LoadFrom((Join-Path $root 'Library/ScriptAssemblies/MoonSharp.Interpreter.dll'))
$null = [MoonSharp.Interpreter.Script]::DefaultOptions
$null = Import-SF2ManagedRuntime $root

$fixture = Join-Path $root ('Temp/DE128ShortForm-' + [Guid]::NewGuid().ToString('N'))
$package = Join-Path $fixture 'Mods/de128'
$null = New-Item -ItemType Directory -Force (Join-Path $package 'scripts/content')
Copy-Item -Recurse -LiteralPath (Join-Path $root 'Mods/de128/assets') -Destination $package
@'
schema = 1
id = "de128"
name = "DE128 short-form equivalence"
version = "1.0.0"
authors = ["Eclipse tests"]
entrypoint = "scripts/main.lua"
capabilities = ["content.register", "content.patch"]

[[dependencies]]
id = "core"
version = ">=1.0 <2.0"
'@ | Set-Content (Join-Path $package 'mod.toml')
'return' | Set-Content (Join-Path $package 'scripts/main.lua')
$mod = [Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $fixture 'Mods')).Mods[0]
if ($null -eq $mod) { throw 'Fixture discovery failed.' }

$perks = New-Object Xml.XmlDocument
$perks.XmlResolver = $null
$perks.Load((Join-Path $root 'Assets/vanillaXml/perks.xml'))
$items = New-Object Xml.XmlDocument
$items.XmlResolver = $null
$items.Load((Join-Path $root 'Assets/vanillaXml/list.xml'))
$flags = [Reflection.BindingFlags]'Instance,NonPublic'
$script:checks = 0
function Check([bool]$ok, [string]$message) { if (!$ok) { throw $message }; $script:checks++ }

function Load-Content([string]$fileName, [string]$source) {
    Set-Content -LiteralPath (Join-Path $package ('scripts/content/' + $fileName)) -Value $source -Encoding UTF8
    $module = 'content.' + [IO.Path]::GetFileNameWithoutExtension($fileName)
    Set-Content -LiteralPath (Join-Path $package 'scripts/main.lua') -Value ('require("' + $module + '")')
    $catalog = [Eclipse.Modding.ModContentCatalog]::new()
    $null = [Eclipse.Modding.CoreContentImporter]::ImportWeapons($catalog, [Xml.XmlNode[]]@($items.SelectNodes('/List/Items/Item')), $null)
    $null = [Eclipse.Modding.CoreContentImporter]::ImportMagic($catalog, [Xml.XmlNode[]]@($items.SelectNodes('/List/Items/Item')), $null)
    $null = [Eclipse.Modding.CoreContentImporter]::ImportPerks($catalog, [Xml.XmlNode[]]@($perks.DocumentElement.ChildNodes))
    $tx = $catalog.BeginRegistration($mod)
    $context = $null
    try {
        $assets = [Eclipse.Modding.AssetResolver]::new([Eclipse.Modding.IAssetProvider[]]@([Eclipse.Modding.LooseModProvider]::new($mod)))
        $api = [Eclipse.Modding.ModApiFacade]::new($mod, $assets, $tx, $null)
        $context = [Eclipse.Modding.MoonSharpScriptRuntime]::new().CreateContext($mod, $api)
        $null = $context.ExecuteEntrypoint(); $null = $tx.Commit()
    } finally { if ($null -ne $context) { $context.Dispose() }; $tx.Dispose() }
    $null = $catalog.Freeze()
    return $catalog
}

# Condition containers are order-free; action containers are grouped by trigger.
$conditionContainers = @('Conditions', 'Condition', 'Locks', 'TacticConditions', 'And', 'Or', 'All', 'Any')
function Canonical([Xml.XmlNode]$node) {
    if ($node.NodeType -ne [Xml.XmlNodeType]::Element) { return $node.OuterXml }
    $attributes = @($node.Attributes | Sort-Object Name | ForEach-Object { $_.Name + '="' + $_.Value + '"' }) -join ' '
    $children = @($node.ChildNodes | ForEach-Object { Canonical $_ })
    if ($conditionContainers -contains $node.Name) { $children = @($children | Sort-Object) }
    elseif ($node.Name -eq 'Actions') {
        $index = 0
        $children = @($node.ChildNodes | ForEach-Object {
            $trigger = if ($_.Attributes -and $_.Attributes['Frame']) { 'F' + ([int]$_.Attributes['Frame'].Value).ToString('D6') }
                elseif ($_.Attributes -and $_.Attributes['Event']) { 'E' + $_.Attributes['Event'].Value } else { 'Z' }
            [pscustomobject]@{ Key = $trigger; Index = $index++; Text = (Canonical $_) }
        } | Sort-Object Key, Index | ForEach-Object Text)
    }
    return '<' + $node.Name + ($(if ($attributes) { ' ' + $attributes } else { '' })) + '>' + ($children -join '') + '</' + $node.Name + '>'
}

function Describe($catalog) {
    $adapter = [Eclipse.Modding.LegacyContentAdapter]::new($catalog)
    $document = [Eclipse.Modding.LegacyContentAdapter].GetMethod('BuildMovesDocument', $flags).Invoke($adapter, @())
    $parts = @('moves:' + (Canonical $document.DocumentElement))
    $parts += 'patches:' + (@($catalog.MoveCombatPatches | ForEach-Object { $_.MoveName + '|' + $_.Disable }) -join ',')
    $parts += 'tactics:' + (@($catalog.Tactics | ForEach-Object { $_.RuntimeName + '|' + $_.CoreTemplate }) -join ',')
    $parts += 'moves-registered:' + (@($catalog.Moves | ForEach-Object { $_.Id.ToString() + '|' + $_.Animation }) -join ',')
    return @{ Text = ($parts -join "`n"); Xml = $document.OuterXml }
}

try {
    $approved = Get-Content (Join-Path $PSScriptRoot 'DE128MoveProjections.json') -Raw | ConvertFrom-Json -AsHashtable
    foreach ($file in $Files) {
        if ($file -eq 'chinese_swords.lua') {
            Copy-Item (Join-Path $root 'Mods/de128/scripts/content/chinese_swords_data.lua') (Join-Path $package 'scripts/content/chinese_swords_data.lua')
        }
        $source = Get-Content -Raw (Join-Path $root ('Mods/de128/scripts/content/' + $file))
        $after = Describe (Load-Content $file $source)
        if ($ShowXml) { Write-Output $after.Xml }
        Check ($approved.ContainsKey($file) -and $approved[$file] -ceq $after.Text) ("Native projection changed: " + $file)
    }
    Write-Output "PASS: $script:checks DE128 move projections match approved fixtures."
} finally {
    Remove-Item -LiteralPath $fixture -Recurse -Force -ErrorAction SilentlyContinue
}

