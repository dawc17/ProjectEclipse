[CmdletBinding()]
param(
    [string]$SourceProfileDirectory,
    [string]$OutputProfileDirectory,
    [switch]$Install
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$projectPath = Split-Path $PSScriptRoot -Parent
$vanillaRoot = Join-Path $projectPath 'Assets\vanillaXml'
$saveDirectory = Join-Path $env:USERPROFILE 'AppData\LocalLow\Nekki\Shadow Fight 2\userdata'
$profileRoot = Join-Path $saveDirectory 'SaveProfiles'
if ([string]::IsNullOrEmpty($SourceProfileDirectory)) {
    $SourceProfileDirectory = Join-Path $profileRoot 'Complete'
}
if ([string]::IsNullOrEmpty($OutputProfileDirectory)) {
    $OutputProfileDirectory = Join-Path $profileRoot 'VanillaMax'
}

function Load-Xml([string]$Path) {
    $document = New-Object System.Xml.XmlDocument
    $document.PreserveWhitespace = $false
    $document.XmlResolver = $null
    $document.Load($Path)
    return $document
}

function Save-Xml([System.Xml.XmlDocument]$Document, [string]$Path) {
    $settings = New-Object System.Xml.XmlWriterSettings
    $settings.Encoding = New-Object System.Text.UTF8Encoding($false)
    $settings.Indent = $true
    $settings.NewLineChars = "`r`n"
    $settings.NewLineHandling = [System.Xml.NewLineHandling]::Replace
    $parent = [System.IO.Path]::GetDirectoryName($Path)
    New-Item -ItemType Directory -Path $parent -Force | Out-Null
    $writer = [System.Xml.XmlWriter]::Create($Path, $settings)
    try { $Document.Save($writer) } finally { $writer.Dispose() }
}

function New-StringSet {
    return New-Object 'System.Collections.Generic.HashSet[string]' ([System.StringComparer]::Ordinal)
}

function Assert-GameIsClosed {
    $processes = Get-Process -Name 'Shadow Fight 2', 'ShadowFight2', 'Eclipse' -ErrorAction SilentlyContinue
    if ($null -ne $processes) {
        throw 'Close Shadow Fight 2 before installing the vanilla max save.'
    }
}

if (-not (Test-Path -LiteralPath $vanillaRoot -PathType Container)) {
    throw "Vanilla XML root not found: $vanillaRoot"
}
$sourceUsers = Join-Path $SourceProfileDirectory 'users.xml'
if (-not (Test-Path -LiteralPath $sourceUsers -PathType Leaf)) {
    throw "Source max profile not found: $sourceUsers"
}

$list = Load-Xml (Join-Path $vanillaRoot 'list.xml')
$validItems = New-StringSet
foreach ($node in $list.SelectNodes('//Item[@Name]')) { [void]$validItems.Add($node.GetAttribute('Name')) }

$validBattles = New-StringSet
$validFights = New-StringSet
foreach ($stageName in @('stages.xml', 'raid_stages_default.xml')) {
    $stages = Load-Xml (Join-Path $vanillaRoot $stageName)
    foreach ($zone in $stages.SelectNodes('//Zone[@Name]')) {
        $zoneName = $zone.GetAttribute('Name')
        foreach ($battle in $zone.SelectNodes('./Battle[@Name]')) {
            $battleName = $battle.GetAttribute('Name')
            [void]$validBattles.Add("$zoneName|$battleName|")
            $index = 0
            foreach ($fight in $battle.SelectNodes('./Fight')) {
                $index++
                $fightName = $fight.GetAttribute('Name')
                if ([string]::IsNullOrEmpty($fightName)) { $fightName = [string]$index }
                [void]$validFights.Add("$zoneName|$battleName|$fightName")
            }
        }
    }
}

$validQuests = New-StringSet
$questFiles = @{}
$vanillaRootFull = [System.IO.Path]::GetFullPath($vanillaRoot).TrimEnd('\')
foreach ($file in Get-ChildItem -LiteralPath $vanillaRoot -Recurse -File -Filter '*.xml') {
    try {
        $document = Load-Xml $file.FullName
        $relative = $file.FullName.Substring($vanillaRootFull.Length + 1).Replace('\', '/')
        foreach ($quest in $document.SelectNodes('//Quest[@Name]')) {
            $name = $quest.GetAttribute('Name')
            [void]$validQuests.Add($name)
            if (-not $questFiles.ContainsKey($name)) { $questFiles[$name] = 'assets/' + $relative }
        }
    }
    catch {
        Write-Warning "Skipping quest scan for $($file.FullName): $($_.Exception.Message)"
    }
}

$achievements = Load-Xml (Join-Path $vanillaRoot 'Achievements.xml')
$validAchievements = New-StringSet
foreach ($node in $achievements.SelectNodes('//Achievement[@Name]')) { [void]$validAchievements.Add($node.GetAttribute('Name')) }

$perks = Load-Xml (Join-Path $vanillaRoot 'perks.xml')
$validPerks = New-StringSet
foreach ($node in $perks.SelectNodes('//*[@Name]')) { [void]$validPerks.Add($node.GetAttribute('Name')) }

$settings = Load-Xml (Join-Path $vanillaRoot 'internalSettings.xml')
$validCounters = New-StringSet
foreach ($node in $settings.SelectNodes('/Settings/AchievementCounter/Counter[@Name]')) { [void]$validCounters.Add($node.GetAttribute('Name')) }
foreach ($node in $achievements.SelectNodes('/Achievements/Counter[@Name]')) { [void]$validCounters.Add($node.GetAttribute('Name')) }

$validMapButtons = New-StringSet
foreach ($file in Get-ChildItem -LiteralPath $vanillaRoot -Recurse -File -Filter '*.xml') {
    try {
        $document = Load-Xml $file.FullName
        foreach ($node in $document.SelectNodes('//ShowMapButton[@Name]')) { [void]$validMapButtons.Add($node.GetAttribute('Name')) }
    }
    catch { }
}

$document = Load-Xml $sourceUsers
$warrior = $document.SelectSingleNode('/Root/Warriors/Warrior[@ID="1"]')
if ($null -eq $warrior) { throw 'Warrior ID 1 was not found in the source max profile.' }

$removed = [ordered]@{
    Items = New-Object System.Collections.Generic.List[string]
    Battles = New-Object System.Collections.Generic.List[string]
    Fights = New-Object System.Collections.Generic.List[string]
    Quests = New-Object System.Collections.Generic.List[string]
    Achievements = New-Object System.Collections.Generic.List[string]
    RepostAchievements = New-Object System.Collections.Generic.List[string]
    Perks = New-Object System.Collections.Generic.List[string]
    PerkHistory = New-Object System.Collections.Generic.List[string]
    Counters = New-Object System.Collections.Generic.List[string]
    MapButtons = New-Object System.Collections.Generic.List[string]
    Variables = New-Object System.Collections.Generic.List[string]
    Attributes = New-Object System.Collections.Generic.List[string]
}
$invalidReferences = New-StringSet

foreach ($node in @($warrior.SelectNodes('Items/Item[@Name]'))) {
    $name = $node.GetAttribute('Name')
    if (-not $validItems.Contains($name)) {
        $removed.Items.Add($name)
        [void]$invalidReferences.Add($name)
        [void]$node.ParentNode.RemoveChild($node)
    }
}
foreach ($node in @($warrior.SelectNodes('Battles/Battle[@Name]'))) {
    $name = $node.GetAttribute('Name')
    if (-not $validBattles.Contains($name)) {
        $removed.Battles.Add($name)
        [void]$invalidReferences.Add($name)
        [void]$node.ParentNode.RemoveChild($node)
    }
}
foreach ($node in @($warrior.SelectNodes('Fights/Fight[@IDS]'))) {
    $name = $node.GetAttribute('IDS')
    if (-not $validFights.Contains($name)) {
        $removed.Fights.Add($name)
        [void]$invalidReferences.Add($name)
        [void]$node.ParentNode.RemoveChild($node)
    }
}
foreach ($node in @($warrior.SelectNodes('Quests/Quests/Quest[@Name]'))) {
    $name = $node.GetAttribute('Name')
    if (-not $validQuests.Contains($name)) {
        $removed.Quests.Add($name)
        [void]$node.ParentNode.RemoveChild($node)
    }
    elseif ($questFiles.ContainsKey($name)) {
        $node.SetAttribute('FileName', $questFiles[$name])
    }
}
foreach ($section in @('Achievements', 'RepostAchievements')) {
    foreach ($node in @($warrior.SelectNodes("$section/*[@Name]"))) {
        $name = $node.GetAttribute('Name')
        if (-not $validAchievements.Contains($name)) {
            $removed[$section].Add($name)
            [void]$node.ParentNode.RemoveChild($node)
        }
    }
}
foreach ($section in @('Perks', 'PerkHistory')) {
    foreach ($node in @($warrior.SelectNodes("$section/*[@Name]"))) {
        $name = $node.GetAttribute('Name')
        if (-not $validPerks.Contains($name)) {
            $removed[$section].Add($name)
            [void]$node.ParentNode.RemoveChild($node)
        }
    }
}
foreach ($node in @($warrior.SelectNodes('Counters/*[@Name]'))) {
    $name = $node.GetAttribute('Name')
    if (-not $validCounters.Contains($name)) {
        $removed.Counters.Add($name)
        [void]$node.ParentNode.RemoveChild($node)
    }
}
foreach ($node in @($warrior.SelectNodes('MapButtons/*[@Name]'))) {
    $name = $node.GetAttribute('Name')
    if (-not $validMapButtons.Contains($name)) {
        $removed.MapButtons.Add($name)
        [void]$node.ParentNode.RemoveChild($node)
    }
}

# Remove variables when their value is an exact content reference that was
# removed above. Also drop a narrow set of old content-state flags whose owning
# Ascension/challenger/extended-raid content no longer exists in vanilla 2.41.9.
$obsoleteVariables = New-StringSet
foreach ($name in @(
    'asc_tutorial_first_ticket', 'asc_new_zone_index', 'CurrentAscensionZoneIntro',
    'Asc_free_pass_ZONE_2', 'AscensionRunning', 'asc_tutorial_second_ticket',
    'PrizeSuperItem', 'Asc_tutorial_set_item_shown', 'Asc_tutorial_duel_shown',
    'ChallengerBattlesUnlocked', 'ShownTier4Raids', 'InPowerMode', 'RaidIntroShown',
    'RaidChargeButton', 'PuppeteerGreetings', 'PuppeteerWin', 'America23Greetings',
    'VortexLoss', 'VulcanLambGreetings'
)) { [void]$obsoleteVariables.Add($name) }

foreach ($node in @($warrior.SelectNodes('Quests/Variables/Variable[@Name]'))) {
    $name = $node.GetAttribute('Name')
    $value = $node.GetAttribute('Value')
    if ($invalidReferences.Contains($value) -or $obsoleteVariables.Contains($name)) {
        $removed.Variables.Add($name + '=' + $value)
        [void]$node.ParentNode.RemoveChild($node)
    }
}

$obsoleteAttributes = @(
    'RaidToggleIsVisible', 'RaidMapFocus', 'RaidTutorialStep', 'RaidTutorialGemsTaken',
    'RaidTop100Focus', 'RaidChargeName', 'RaidBuffName', 'RaidTeamBuffName', 'WatchedVideo'
)
foreach ($name in $obsoleteAttributes) {
    if ($null -ne $warrior.Attributes[$name]) {
        $removed.Attributes.Add($name + '=' + $warrior.GetAttribute($name))
        $warrior.RemoveAttribute($name)
    }
}

# Keep the maxed profile at the vanilla endgame instead of pointing back into a
# removed DE raid/Ascension state.
$warrior.SetAttribute('Level', '52')
$warrior.SetAttribute('Tutorial', 'END')
$warrior.SetAttribute('CurrentZone', 'ZONE_7')
$warrior.SetAttribute('FightIDS', 'ZONE_7|C3_BOSS_TITAN|6')
$warrior.SetAttribute('MapFocus', 'ZONE_7|C3_BOSS_TITAN|')
if ($null -ne $warrior.Attributes['EclipseMode']) { $warrior.SetAttribute('EclipseMode', 'Off') }

foreach ($currentUser in @($document.SelectNodes('/Root/CurrentUser'))) {
    foreach ($watched in @($currentUser.SelectNodes('WatchedVideos'))) {
        [void]$currentUser.RemoveChild($watched)
    }
}

$versions = $document.SelectSingleNode('/Root/Versions')
if ($null -eq $versions) {
    $versions = $document.CreateElement('Versions')
    [void]$document.DocumentElement.AppendChild($versions)
}
$version = $versions.SelectSingleNode('Version')
if ($null -eq $version) { $version = $document.CreateElement('Version'); [void]$versions.AppendChild($version) }
$version.SetAttribute('Value', '2.41.9')
$dataVersion = $versions.SelectSingleNode('DataVersion')
if ($null -eq $dataVersion) { $dataVersion = $document.CreateElement('DataVersion'); [void]$versions.AppendChild($dataVersion) }
$dataVersion.SetAttribute('Value', '2.41.9.0')

foreach ($slot in @('Armor', 'Helm', 'Weapon', 'Ranged', 'Magic')) {
    if (-not $validItems.Contains($warrior.GetAttribute($slot))) {
        throw "Equipped $slot is not defined by vanilla list.xml: $($warrior.GetAttribute($slot))"
    }
}

# Validation: no content-bearing save nodes may point outside the imported
# vanilla 2.41.9 XML set.
foreach ($node in $warrior.SelectNodes('Items/Item[@Name]')) {
    if (-not $validItems.Contains($node.GetAttribute('Name'))) { throw 'Invalid item survived sanitization.' }
}
foreach ($node in $warrior.SelectNodes('Battles/Battle[@Name]')) {
    if (-not $validBattles.Contains($node.GetAttribute('Name'))) { throw 'Invalid battle survived sanitization.' }
}
foreach ($node in $warrior.SelectNodes('Fights/Fight[@IDS]')) {
    if (-not $validFights.Contains($node.GetAttribute('IDS'))) { throw 'Invalid fight survived sanitization.' }
}
foreach ($node in $warrior.SelectNodes('Quests/Quests/Quest[@Name]')) {
    if (-not $validQuests.Contains($node.GetAttribute('Name'))) { throw 'Invalid quest survived sanitization.' }
}
if ($warrior.GetAttribute('Level') -ne '52') { throw 'Sanitized profile is not level 52.' }

if (Test-Path -LiteralPath $OutputProfileDirectory) {
    $resolvedOutput = [System.IO.Path]::GetFullPath($OutputProfileDirectory)
    $resolvedProfiles = [System.IO.Path]::GetFullPath($profileRoot).TrimEnd('\') + '\'
    if (-not $resolvedOutput.StartsWith($resolvedProfiles, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to replace output directory outside SaveProfiles: $resolvedOutput"
    }
    Remove-Item -LiteralPath $OutputProfileDirectory -Recurse -Force
}
New-Item -ItemType Directory -Path $OutputProfileDirectory -Force | Out-Null
$outputUsers = Join-Path $OutputProfileDirectory 'users.xml'
$outputBackup = Join-Path $OutputProfileDirectory 'users_backup.xml'
Save-Xml $document $outputUsers
Copy-Item -LiteralPath $outputUsers -Destination $outputBackup -Force

$reportLines = New-Object System.Collections.Generic.List[string]
$reportLines.Add('Vanilla max save sanitization')
$reportLines.Add('Source: ' + $sourceUsers)
$reportLines.Add('Vanilla root: ' + $vanillaRoot)
$reportLines.Add('Level: ' + $warrior.GetAttribute('Level'))
$reportLines.Add('Money: ' + $warrior.GetAttribute('Money'))
$reportLines.Add('Gems: ' + $warrior.GetAttribute('Bonus'))
$reportLines.Add('Items retained: ' + $warrior.SelectNodes('Items/Item').Count)
$reportLines.Add('Battles retained: ' + $warrior.SelectNodes('Battles/Battle').Count)
$reportLines.Add('Fights retained: ' + $warrior.SelectNodes('Fights/Fight').Count)
foreach ($entry in $removed.GetEnumerator()) {
    $reportLines.Add('')
    $reportLines.Add($entry.Key + ' removed: ' + $entry.Value.Count)
    foreach ($value in $entry.Value) { $reportLines.Add('  ' + $value) }
}
$reportPath = Join-Path $OutputProfileDirectory 'sanitization-report.txt'
[IO.File]::WriteAllLines($reportPath, $reportLines, [Text.UTF8Encoding]::new($false))

if ($Install) {
    Assert-GameIsClosed
    foreach ($hashName in @('users.xml.hash', 'users_backup.xml.hash')) {
        if (Test-Path -LiteralPath (Join-Path $saveDirectory $hashName)) {
            throw "Hash-protected active save detected; refusing unsigned install: $hashName"
        }
    }

    $activeUsers = Join-Path $saveDirectory 'users.xml'
    if (-not (Test-Path -LiteralPath $activeUsers)) { throw "Active save not found: $activeUsers" }
    $active = Load-Xml $activeUsers
    $activeWarrior = $active.SelectSingleNode('/Root/Warriors/Warrior[@ID="1"]')
    $installDocument = Load-Xml $outputUsers
    $installWarrior = $installDocument.SelectSingleNode('/Root/Warriors/Warrior[@ID="1"]')

    foreach ($attributeName in @('ID', 'FirstName', 'Voice', 'Avatar', 'InstallID', 'ServerUserID', 'Language')) {
        if ($null -ne $activeWarrior.Attributes[$attributeName]) {
            $installWarrior.SetAttribute($attributeName, $activeWarrior.GetAttribute($attributeName))
        }
    }
    $activeSounds = $active.SelectSingleNode('/Root/CurrentUser/Sounds')
    $installCurrentUser = $installDocument.SelectSingleNode('/Root/CurrentUser')
    if ($null -ne $activeSounds -and $null -ne $installCurrentUser) {
        $existingSounds = $installCurrentUser.SelectSingleNode('Sounds')
        $replacement = $installDocument.ImportNode($activeSounds, $true)
        if ($null -eq $existingSounds) { [void]$installCurrentUser.AppendChild($replacement) }
        else { [void]$installCurrentUser.ReplaceChild($replacement, $existingSounds) }
    }

    $timestamp = Get-Date -Format 'yyyyMMdd-HHmmss'
    $safety = Join-Path $profileRoot ('Safety\' + $timestamp + '-pre-vanilla-max')
    New-Item -ItemType Directory -Path $safety -Force | Out-Null
    foreach ($relative in @('users.xml', 'users_backup.xml', 'assets\localSettings.bin', 'assets\packs.xml')) {
        $source = Join-Path $saveDirectory $relative
        if (Test-Path -LiteralPath $source -PathType Leaf) {
            $destination = Join-Path $safety $relative
            New-Item -ItemType Directory -Path ([IO.Path]::GetDirectoryName($destination)) -Force | Out-Null
            Copy-Item -LiteralPath $source -Destination $destination -Force
        }
    }

    Save-Xml $installDocument (Join-Path $saveDirectory 'users.xml')
    Copy-Item -LiteralPath (Join-Path $saveDirectory 'users.xml') -Destination (Join-Path $saveDirectory 'users_backup.xml') -Force
    Write-Host "Active save safety copy: $safety"
}

Write-Host "VanillaMax profile: $OutputProfileDirectory"
Write-Host "Level $($warrior.GetAttribute('Level')), money $($warrior.GetAttribute('Money')), gems $($warrior.GetAttribute('Bonus'))"
Write-Host "Retained $($warrior.SelectNodes('Items/Item').Count) items, $($warrior.SelectNodes('Battles/Battle').Count) battles, $($warrior.SelectNodes('Fights/Fight').Count) fights."
foreach ($entry in $removed.GetEnumerator()) {
    if ($entry.Value.Count -ne 0) { Write-Host ("Removed {0} {1}." -f $entry.Value.Count, $entry.Key) }
}
Write-Host "Report: $reportPath"
