param(
    [ValidateSet('All', 'Windows', 'Android', 'WindowsEditableXml', 'WindowsDevelopment')][string]$Target = 'All',
    [string]$Unity = '',
    [string]$ProjectPath = (Split-Path -Parent $PSScriptRoot),
    [string]$OutputDirectory = '',
    # Release version for PackageUpdate.ps1. Omit for a dev build, which never checks for updates.
    [ValidatePattern('^(\d{1,6}\.\d{1,6}\.\d{1,6})?$')][string]$Version = ''
)

$ErrorActionPreference = 'Stop'
$ProjectPath = (Resolve-Path -LiteralPath $ProjectPath).Path
if (!(Test-Path -LiteralPath (Join-Path $ProjectPath 'ProjectSettings/ProjectVersion.txt'))) {
    throw "Not a Unity project: $ProjectPath"
}
$editorVersion = (Select-String -LiteralPath (Join-Path $ProjectPath 'ProjectSettings/ProjectVersion.txt') -Pattern '^m_EditorVersion: (.+)$').Matches[0].Groups[1].Value
if ([string]::IsNullOrWhiteSpace($Unity)) {
    $candidates = @("F:\UnityInstalls\$editorVersion\Editor\Unity.exe", "$env:ProgramFiles\Unity\Hub\Editor\$editorVersion\Editor\Unity.exe")
    $Unity = $candidates | Where-Object { Test-Path -LiteralPath $_ -PathType Leaf } | Select-Object -First 1
}
if (!$Unity -or !(Test-Path -LiteralPath $Unity -PathType Leaf)) { throw "Unity $editorVersion editor not found; pass -Unity with its executable path." }
$lockPath = Join-Path $ProjectPath 'Temp/UnityLockfile'
if (Test-Path -LiteralPath $lockPath) {
    $lockProbe = $null
    try {
        $lockProbe = [IO.File]::Open($lockPath, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::None)
    } catch {
        throw "Close the Unity editor for this project before building: $ProjectPath"
    } finally {
        if ($null -ne $lockProbe) { $lockProbe.Dispose() }
    }
}
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $ProjectPath 'Builds'
}
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null

$plans = @(
    @{ Name = 'Windows'; Profile = 'Eclipse Windows'; Method = 'BuildWindows'; File = 'Windows/Eclipse.exe' },
    @{ Name = 'Android'; Profile = 'Eclipse Android'; Method = 'BuildAndroid'; File = 'Android/Eclipse.apk' },
    @{ Name = 'WindowsEditableXml'; Profile = 'Eclipse Windows Editable XML'; Method = 'BuildWindowsEditableXml'; File = 'WindowsEditableXml/Eclipse.exe' },
    @{ Name = 'WindowsDevelopment'; Profile = 'Eclipse Windows Development'; Method = 'BuildWindowsDevelopment'; File = 'WindowsDevelopment/Eclipse.exe' }
)
$previousWindowsOutput = $env:ECLIPSE_WINDOWS_OUTPUT
$previousAndroidOutput = $env:ECLIPSE_ANDROID_OUTPUT
$previousEditableXmlOutput = $env:ECLIPSE_WINDOWS_EDITABLE_XML_OUTPUT
$previousDevelopmentOutput = $env:ECLIPSE_WINDOWS_DEVELOPMENT_OUTPUT
$previousVersion = $env:ECLIPSE_BUILD_VERSION
try {
    $env:ECLIPSE_BUILD_VERSION = $Version
    $env:ECLIPSE_WINDOWS_OUTPUT = Join-Path $OutputDirectory 'Windows/Eclipse.exe'
    $env:ECLIPSE_ANDROID_OUTPUT = Join-Path $OutputDirectory 'Android/Eclipse.apk'
    $env:ECLIPSE_WINDOWS_EDITABLE_XML_OUTPUT = Join-Path $OutputDirectory 'WindowsEditableXml/Eclipse.exe'
    $env:ECLIPSE_WINDOWS_DEVELOPMENT_OUTPUT = Join-Path $OutputDirectory 'WindowsDevelopment/Eclipse.exe'
    foreach ($plan in $plans) {
        # Tester/development builds are produced only on request.
        if ($Target -ne $plan.Name -and ($Target -ne 'All' -or $plan.Name -notin @('Windows', 'Android'))) { continue }
        $profile = 'Assets/Settings/Build Profiles/' + $plan.Profile + '.asset'
        if (!(Test-Path -LiteralPath (Join-Path $ProjectPath $profile))) { throw "Build Profile missing: $profile. Run SF2 > Unity 6 > Set Up Workflows in Unity." }
        $log = Join-Path $OutputDirectory ('build-' + $plan.Name.ToLowerInvariant() + '.log')
        $arguments = @(
            '-batchmode', '-quit', '-projectPath', ('"' + $ProjectPath + '"'),
            '-activeBuildProfile', ('"' + $profile + '"'),
            '-executeMethod', ('EclipsePlayerBuild.' + $plan.Method),
            '-logFile', ('"' + $log + '"')
        )
        Write-Host ('Building ' + $plan.Name + '; log: ' + $log)
        $process = Start-Process -FilePath $Unity -ArgumentList $arguments -WindowStyle Hidden -Wait -PassThru
        if ($process.ExitCode -ne 0 -or
            !(Test-Path -LiteralPath (Join-Path $OutputDirectory $plan.File)) -or
            !(Select-String -LiteralPath $log -Pattern '\[EclipseBuild\] PASS:')) {
            throw ($plan.Name + ' build did not succeed (exit ' + $process.ExitCode + '). See ' + $log)
        }
        (Select-String -LiteralPath $log -Pattern '\[EclipseBuild\] PASS:').Line
        if ($plan.Name -eq 'Windows') {
            & (Join-Path $PSScriptRoot 'BuildLauncher.ps1') -OutputDirectory (Join-Path $OutputDirectory 'Windows')
        }
    }
} finally {
    $env:ECLIPSE_WINDOWS_OUTPUT = $previousWindowsOutput
    $env:ECLIPSE_ANDROID_OUTPUT = $previousAndroidOutput
    $env:ECLIPSE_WINDOWS_EDITABLE_XML_OUTPUT = $previousEditableXmlOutput
    $env:ECLIPSE_WINDOWS_DEVELOPMENT_OUTPUT = $previousDevelopmentOutput
    $env:ECLIPSE_BUILD_VERSION = $previousVersion
}
