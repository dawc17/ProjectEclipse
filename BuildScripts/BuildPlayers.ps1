param(
    [ValidateSet('All', 'Windows', 'Android')][string]$Target = 'All',
    [string]$Unity = 'F:\UnityInstalls\2022.3.62f3\Editor\Unity.exe',
    [string]$ProjectPath = (Split-Path -Parent $PSScriptRoot),
    [string]$OutputDirectory = ''
)

$ErrorActionPreference = 'Stop'
$ProjectPath = (Resolve-Path -LiteralPath $ProjectPath).Path
if (!(Test-Path -LiteralPath $Unity -PathType Leaf)) { throw "Unity editor not found: $Unity" }
if (!(Test-Path -LiteralPath (Join-Path $ProjectPath 'ProjectSettings/ProjectVersion.txt'))) {
    throw "Not a Unity project: $ProjectPath"
}
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
    @{ Name = 'Windows'; Platform = 'Win64'; Method = 'BuildWindows'; File = 'Windows/Eclipse.exe' },
    @{ Name = 'Android'; Platform = 'Android'; Method = 'BuildAndroid'; File = 'Android/Eclipse.apk' }
)
$previousWindowsOutput = $env:ECLIPSE_WINDOWS_OUTPUT
$previousAndroidOutput = $env:ECLIPSE_ANDROID_OUTPUT
try {
    $env:ECLIPSE_WINDOWS_OUTPUT = Join-Path $OutputDirectory 'Windows/Eclipse.exe'
    $env:ECLIPSE_ANDROID_OUTPUT = Join-Path $OutputDirectory 'Android/Eclipse.apk'
    foreach ($plan in $plans) {
        if ($Target -ne 'All' -and $Target -ne $plan.Name) { continue }
        $log = Join-Path $OutputDirectory ('build-' + $plan.Name.ToLowerInvariant() + '.log')
        $arguments = @(
            '-batchmode', '-quit', '-projectPath', ('"' + $ProjectPath + '"'),
            '-buildTarget', $plan.Platform,
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
}
