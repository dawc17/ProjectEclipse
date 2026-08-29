[CmdletBinding()]
param(
    [ValidateSet('Status', 'CaptureComplete', 'UseComplete', 'UseVanillaMax', 'NewGame')]
    [string]$Action = 'Status',

    [switch]$Force
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

# This is Unity's persistentDataPath for the current Windows build
# (company: Nekki, product: Shadow Fight 2).
$saveDirectory = Join-Path $env:USERPROFILE 'AppData\LocalLow\Nekki\Shadow Fight 2\userdata'
$profileDirectory = Join-Path $saveDirectory 'SaveProfiles'
$completeDirectory = Join-Path $profileDirectory 'Complete'
$vanillaMaxDirectory = Join-Path $profileDirectory 'VanillaMax'
$safetyDirectory = Join-Path $profileDirectory 'Safety'

# These are the files ListSF.CELGPFFHLIM clears when the game resets a user.
# Keep their hash companions with each profile too, for builds where user-data
# validation is enabled.
$saveFiles = @(
    'users.xml',
    'users.xml.hash',
    'users_backup.xml',
    'users_backup.xml.hash',
    'assets\localSettings.bin',
    'assets\localSettings.bin.hash',
    'assets\packs.xml',
    'assets\packs.xml.hash'
)

function Get-ProfileSummary {
    param([Parameter(Mandatory = $true)][string]$Directory)

    $usersFile = Join-Path $Directory 'users.xml'
    if (-not (Test-Path -LiteralPath $usersFile)) {
        return 'not present'
    }

    try {
        [xml]$document = Get-Content -LiteralPath $usersFile -Raw
        $warrior = $document.Root.Warriors.Warrior | Select-Object -First 1
        if ($null -eq $warrior) {
            return 'users.xml has no warrior'
        }

        return 'level {0}, tutorial {1}, money {2}, gems {3}' -f `
            $warrior.Level, $warrior.Tutorial, $warrior.Money, $warrior.Bonus
    }
    catch {
        return 'users.xml could not be read: ' + $_.Exception.Message
    }
}

function Copy-SF2SaveFiles {
    param(
        [Parameter(Mandatory = $true)][string]$FromDirectory,
        [Parameter(Mandatory = $true)][string]$ToDirectory
    )

    foreach ($relativeFile in $saveFiles) {
        $sourceFile = Join-Path $FromDirectory $relativeFile
        if (-not (Test-Path -LiteralPath $sourceFile -PathType Leaf)) {
            continue
        }

        $destinationFile = Join-Path $ToDirectory $relativeFile
        $destinationParent = [System.IO.Path]::GetDirectoryName($destinationFile)
        New-Item -ItemType Directory -Path $destinationParent -Force | Out-Null
        Copy-Item -LiteralPath $sourceFile -Destination $destinationFile -Force
    }
}

function Clear-SaveFiles {
    param([Parameter(Mandatory = $true)][string]$Directory)

    foreach ($relativeFile in $saveFiles) {
        $saveFile = Join-Path $Directory $relativeFile
        if (Test-Path -LiteralPath $saveFile -PathType Leaf) {
            Remove-Item -LiteralPath $saveFile -Force
        }
    }
}

function Assert-GameIsClosed {
    $processes = Get-Process -Name 'Shadow Fight 2', 'ShadowFight2', 'Eclipse' -ErrorAction SilentlyContinue
    if ($null -ne $processes) {
        throw 'Close Shadow Fight 2 before switching saves. Running it now could overwrite the selected profile.'
    }
}

function Save-SafetyCopy {
    if (-not (Test-Path -LiteralPath (Join-Path $saveDirectory 'users.xml') -PathType Leaf)) {
        return
    }

    $timestamp = Get-Date -Format 'yyyyMMdd-HHmmss'
    $safetyCopy = Join-Path $safetyDirectory $timestamp
    New-Item -ItemType Directory -Path $safetyCopy -Force | Out-Null
    Copy-SF2SaveFiles -FromDirectory $saveDirectory -ToDirectory $safetyCopy
    Write-Host "Safety copy created: $safetyCopy"
}

if ($Action -eq 'Status') {
    Write-Host "Active save:   $(Get-ProfileSummary -Directory $saveDirectory)"
    Write-Host "Complete save: $(Get-ProfileSummary -Directory $completeDirectory)"
    Write-Host "Vanilla max:   $(Get-ProfileSummary -Directory $vanillaMaxDirectory)"
    Write-Host "Profile folder: $profileDirectory"
    exit 0
}

Assert-GameIsClosed
New-Item -ItemType Directory -Path $saveDirectory -Force | Out-Null

switch ($Action) {
    'CaptureComplete' {
        $activeUsersFile = Join-Path $saveDirectory 'users.xml'
        if (-not (Test-Path -LiteralPath $activeUsersFile -PathType Leaf)) {
            throw "No active users.xml exists at $saveDirectory. Start the game once before capturing a complete profile."
        }

        $stagingDirectory = Join-Path $profileDirectory 'Complete.staging'
        Remove-Item -LiteralPath $stagingDirectory -Recurse -Force -ErrorAction SilentlyContinue
        New-Item -ItemType Directory -Path $stagingDirectory -Force | Out-Null
        Copy-SF2SaveFiles -FromDirectory $saveDirectory -ToDirectory $stagingDirectory

        if (Test-Path -LiteralPath $completeDirectory) {
            if (-not $Force) {
                throw "A complete profile already exists at $completeDirectory. Re-run with -Force to replace it."
            }
            Remove-Item -LiteralPath $completeDirectory -Recurse -Force
        }

        Move-Item -LiteralPath $stagingDirectory -Destination $completeDirectory
        Write-Host "Complete profile captured: $(Get-ProfileSummary -Directory $completeDirectory)"
    }

    'UseComplete' {
        if (-not (Test-Path -LiteralPath (Join-Path $completeDirectory 'users.xml') -PathType Leaf)) {
            throw "The complete profile is missing. Capture it first with: .\Tools\SwitchSaveProfile.ps1 -Action CaptureComplete"
        }

        Save-SafetyCopy
        Clear-SaveFiles -Directory $saveDirectory
        Copy-SF2SaveFiles -FromDirectory $completeDirectory -ToDirectory $saveDirectory
        Write-Host "Complete profile restored: $(Get-ProfileSummary -Directory $saveDirectory)"
    }

    'UseVanillaMax' {
        if (-not (Test-Path -LiteralPath (Join-Path $vanillaMaxDirectory 'users.xml') -PathType Leaf)) {
            throw "The vanilla max profile is missing. Build it with: .\Tools\PrepareVanillaMaxSave.ps1"
        }

        Save-SafetyCopy
        Clear-SaveFiles -Directory $saveDirectory
        Copy-SF2SaveFiles -FromDirectory $vanillaMaxDirectory -ToDirectory $saveDirectory
        Write-Host "Vanilla max profile restored: $(Get-ProfileSummary -Directory $saveDirectory)"
    }

    'NewGame' {
        Save-SafetyCopy
        Clear-SaveFiles -Directory $saveDirectory
        Write-Host 'Active save cleared. Start the game now; it will create a fresh first-playthrough save.'
    }
}
