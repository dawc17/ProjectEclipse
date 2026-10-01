param(
    [Parameter(Mandatory = $true)]
    [string] $UserDataDirectory,

    [Parameter(Mandatory = $true)]
    [string] $TemplatePath
)

$ErrorActionPreference = 'Stop'

$usersPath = Join-Path $UserDataDirectory 'users.xml'
$usersBackupPath = Join-Path $UserDataDirectory 'users_backup.xml'

if (-not (Test-Path -LiteralPath $usersPath)) {
    throw "Active save not found: $usersPath"
}
if (-not (Test-Path -LiteralPath $TemplatePath)) {
    throw "Max-progress template not found: $TemplatePath"
}

$hashPaths = @("$usersPath.hash", "$usersBackupPath.hash") | Where-Object {
    Test-Path -LiteralPath $_
}
if ($hashPaths.Count -ne 0) {
    throw "Hash-protected save detected; refusing to install an invalid unsigned save: $($hashPaths -join ', ')"
}

$timestamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$backupDirectory = Join-Path $UserDataDirectory "codex-save-backup-$timestamp"
New-Item -ItemType Directory -Path $backupDirectory | Out-Null
Copy-Item -LiteralPath $usersPath -Destination (Join-Path $backupDirectory 'users.xml')
if (Test-Path -LiteralPath $usersBackupPath) {
    Copy-Item -LiteralPath $usersBackupPath -Destination (Join-Path $backupDirectory 'users_backup.xml')
}

$currentDocument = New-Object System.Xml.XmlDocument
$currentDocument.PreserveWhitespace = $false
$currentDocument.Load($usersPath)
$currentWarrior = $currentDocument.SelectSingleNode('/Root/Warriors/Warrior[@ID="1"]')
if ($null -eq $currentWarrior) {
    throw 'Current warrior ID 1 was not found.'
}

$templateText = Get-Content -Raw -LiteralPath $TemplatePath
$templateText = $templateText -replace '^<\?xml[^>]*\?>\s*', ''
$templateDocument = New-Object System.Xml.XmlDocument
$templateDocument.LoadXml('<Root>' + $templateText + '</Root>')
$templateWarrior = $templateDocument.SelectSingleNode('/Root/Warriors/Warrior[not(@IsFake)]')
if ($null -eq $templateWarrior) {
    throw 'The completed warrior was not found in the template.'
}

$preservedAttributes = @(
    'ID',
    'FirstName',
    'Voice',
    'Avatar',
    'InstallID',
    'ServerUserID',
    'Language'
)
$preservedValues = @{}
foreach ($attributeName in $preservedAttributes) {
    $attribute = $currentWarrior.Attributes[$attributeName]
    if ($null -ne $attribute) {
        $preservedValues[$attributeName] = $attribute.Value
    }
}

$replacement = $currentDocument.ImportNode($templateWarrior, $true)
foreach ($entry in $preservedValues.GetEnumerator()) {
    $attribute = $replacement.Attributes[$entry.Key]
    if ($null -eq $attribute) {
        $attribute = $currentDocument.CreateAttribute($entry.Key)
        [void] $replacement.Attributes.Append($attribute)
    }
    $attribute.Value = $entry.Value
}

foreach ($sectionName in @('Sounds', 'SessionSettings')) {
    $currentSection = $currentWarrior.SelectSingleNode($sectionName)
    if ($null -eq $currentSection) {
        continue
    }
    $templateSection = $replacement.SelectSingleNode($sectionName)
    $preservedSection = $currentDocument.ImportNode($currentSection, $true)
    if ($null -eq $templateSection) {
        [void] $replacement.AppendChild($preservedSection)
    }
    else {
        [void] $replacement.ReplaceChild($preservedSection, $templateSection)
    }
}

[void] $currentWarrior.ParentNode.ReplaceChild($replacement, $currentWarrior)

$writerSettings = New-Object System.Xml.XmlWriterSettings
$writerSettings.Encoding = New-Object System.Text.UTF8Encoding($false)
$writerSettings.Indent = $true
$writerSettings.NewLineChars = "`r`n"
$writerSettings.NewLineHandling = [System.Xml.NewLineHandling]::Replace

$temporaryPath = Join-Path $UserDataDirectory "users.codex-$timestamp.tmp"
$writer = [System.Xml.XmlWriter]::Create($temporaryPath, $writerSettings)
try {
    $currentDocument.Save($writer)
}
finally {
    $writer.Dispose()
}

$validationDocument = New-Object System.Xml.XmlDocument
$validationDocument.Load($temporaryPath)
$validationWarrior = $validationDocument.SelectSingleNode('/Root/Warriors/Warrior[@ID="1"]')
if ($null -eq $validationWarrior -or $validationWarrior.Attributes['Level'].Value -ne '52') {
    throw 'Generated save failed validation.'
}

Copy-Item -LiteralPath $temporaryPath -Destination $usersPath -Force
Copy-Item -LiteralPath $temporaryPath -Destination $usersBackupPath -Force
Remove-Item -LiteralPath $temporaryPath

[pscustomobject]@{
    BackupDirectory = $backupDirectory
    Level = $validationWarrior.Attributes['Level'].Value
    CurrentZone = $validationWarrior.Attributes['CurrentZone'].Value
    Items = $validationWarrior.SelectNodes('Items/Item').Count
    Battles = $validationWarrior.SelectNodes('Battles/Battle').Count
    Fights = $validationWarrior.SelectNodes('Fights/Fight').Count
    Perks = $validationWarrior.SelectNodes('Perks/Perk').Count
    OpenTricks = $validationWarrior.SelectNodes('OpenTricks/Trick').Count
    Quests = $validationWarrior.SelectNodes('Quests/Quests/Quest').Count
}
