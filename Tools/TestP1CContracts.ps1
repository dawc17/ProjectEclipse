param(
    [string]$Root = (Split-Path -Parent $PSScriptRoot)
)

$ErrorActionPreference = 'Stop'

function Require-Text([string]$Path, [string]$Pattern, [string]$Message) {
    $text = Get-Content -LiteralPath $Path -Raw
    if ($text -notmatch $Pattern) { throw $Message }
}

function Reject-Text([string]$Path, [string]$Pattern, [string]$Message) {
    $text = Get-Content -LiteralPath $Path -Raw
    if ($text -match $Pattern) { throw $Message }
}

$content = Join-Path $Root 'Assets/Scripts/Eclipse/Runtime/Modding/ModContentP1C.cs'
$adapter = Join-Path $Root 'Assets/Scripts/Eclipse/Modding/LegacyContentAdapter.cs'
$lua = Join-Path $Root 'Assets/Scripts/Eclipse/Modding/MoonSharpScriptRuntime.cs'
$items = Join-Path $Root 'Assets/Scripts/Assembly-CSharp/Items.cs'
$sets = Join-Path $Root 'Assets/Scripts/Assembly-CSharp/ItemSets.cs'
$tree = Join-Path $Root 'Assets/Scripts/Assembly-CSharp/PerkTree.cs'
$recipe = Join-Path $Root 'Assets/Scripts/Assembly-CSharp/Recipe.cs'
$shop = Join-Path $Root 'Assets/Scripts/Assembly-CSharp/Nekki/SF2/GUI/Shop/ShopScene.cs'
$quest = Join-Path $Root 'Assets/Scripts/Assembly-CSharp/QuestCondition.cs'

Require-Text $items '(?s)item\.Type != "Consumable".*item\.Type != "Free".*item\.Type != "Seal"' `
    'Recovered Items.AddExternalItem does not admit the three supported non-equipment categories.'
Require-Text $sets 'External item set requires at least one member' `
    'External item sets must reject empty pseudo-sets whose recovered completion semantics are undefined.'
Require-Text $tree 'ReplaceExternalBranch\(' 'Perk-tree replacement seam is missing.'
Require-Text $tree 'RestoreExternalBranch\(' 'Perk-tree replacement is not reversible.'

Require-Text $content 'ForgeEconomicProfileDefinition' 'Immutable forge profile handle model is missing.'
Require-Text $content 'RegisterForgeRecipeFamily' 'Typed forge recipe-family registration is missing.'
Require-Text $content 'ModForgeRecipeCandidate' 'Forge candidate eligibility model is missing.'
Reject-Text $content '(?i)deliverytime|bonusdeliveryprice|materialprice|currencystruct|modprice' `
    'P1C forge public model contains an economic value surface.'
Require-Text $recipe '_prices\.AddRange\(economicProfile\._prices\)' `
    'External forge family is not bound to a host-owned economic profile.'
Require-Text $recipe 'candidate\.Contains\(itemLevel\)' 'Forge candidate level eligibility is not enforced.'

Require-Text $shop 'ShopAvailabilityPolicy\.IsAvailable' 'Shop UI does not use unified availability policy.'
Require-Text $quest 'ShopAvailabilityPolicy\.IsAvailable' 'Quest Availability does not share the shop policy evaluator.'

foreach ($symbol in @(
    'register_consumable', 'register_free', 'register_seal', 'itemsets',
    'replace_perk_branch', 'forge', 'register_recipe', 'set_availability',
    'FORCE_VISIBLE', 'FORCE_HIDDEN')) {
    Require-Text $lua ([regex]::Escape($symbol)) "Lua P1C surface is missing '$symbol'."
}

Require-Text $adapter 'AddExternalRecipeFamily' 'Legacy adapter does not materialize typed forge families.'
Require-Text $adapter 'ReplaceExternalBranch' 'Legacy adapter does not materialize progression overlays.'
Require-Text $adapter 'AddExternalSet' 'Legacy adapter does not materialize item sets.'

[xml]$forgeXml = Get-Content -LiteralPath (Join-Path $Root 'Assets/vanillaXml/forge.xml') -Raw
$baseProfiles = @($forgeXml.Forge.Recipes.Recipe | ForEach-Object { $_.Name })
foreach ($required in @('Simple', 'Medium', 'Complex')) {
    if ($baseProfiles -notcontains $required) { throw "Authoritative forge fixture '$required' is missing." }
}

[xml]$listXml = Get-Content -LiteralPath (Join-Path $Root 'Assets/vanillaXml/list.xml') -Raw
$allItems = @($listXml.List.Items.Item)
$supportedNonEquipment = @($allItems | Where-Object { $_.Type -in @('Consumable', 'Free', 'Seal') })
if ($supportedNonEquipment.Count -eq 0) { throw 'Authoritative list.xml has no P1C non-equipment fixture.' }

Write-Host ("P1C contract checks passed: {0} host forge profiles, {1} supported non-equipment fixtures." -f `
    $baseProfiles.Count, $supportedNonEquipment.Count)
