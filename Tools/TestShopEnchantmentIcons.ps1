param([string]$Unity = 'F:\UnityInstalls\2022.3.62f3\Editor\Unity.exe')
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$fixture = Join-Path $root 'Temp/ShopEnchantmentIconsProject'
foreach ($directory in @('Assets/Resources/ui/Enchantments', 'Packages', 'ProjectSettings')) {
    New-Item -ItemType Directory -Path (Join-Path $fixture $directory) -Force | Out-Null
}
[IO.File]::WriteAllText((Join-Path $fixture 'Packages/manifest.json'), '{"dependencies":{"com.unity.ugui":"1.0.0"}}')
[IO.File]::WriteAllText((Join-Path $fixture 'ProjectSettings/ProjectVersion.txt'), "m_EditorVersion: 2022.3.62f3`n")
Copy-Item -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/Nekki/SF2/GUI/ResolutionImage.cs') -Destination (Join-Path $fixture 'Assets/ResolutionImage.cs')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'ValidateShopEnchantmentIcons.cs') -Destination (Join-Path $fixture 'Assets/ValidateShopEnchantmentIcons.cs')
foreach ($png in Get-ChildItem -LiteralPath (Join-Path $root 'Assets/Resources/ui/Enchantments') -Filter '*.png') {
    Copy-Item -LiteralPath $png.FullName, ($png.FullName + '.meta') -Destination (Join-Path $fixture 'Assets/Resources/ui/Enchantments')
}
$log = Join-Path $root ('Temp/shop-enchantment-icons-' + [Guid]::NewGuid().ToString('N') + '.log')
$arguments = @('-batchmode', '-nographics', '-projectPath', ('"' + $fixture + '"'),
    '-sourceProject', ('"' + $root + '"'), '-executeMethod', 'ValidateShopEnchantmentIcons.Run',
    '-logFile', ('"' + $log + '"'))
$process = Start-Process -FilePath $Unity -ArgumentList $arguments -WindowStyle Hidden -Wait -PassThru
if ($process.ExitCode -ne 0) {
    Select-String -LiteralPath $log -Pattern 'error CS|Exception:|\[ShopEnchantmentIcons\]' -Context 0,4
    throw "Unity validation failed: $log"
}
$passed = Select-String -LiteralPath $log -Pattern '\[ShopEnchantmentIcons\] PASS'
if (!$passed) { throw "Unity did not finish the validator: $log" }
$passed.Line
