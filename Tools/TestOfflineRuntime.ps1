using namespace Eclipse.Content

param(
    # Source-only checks may run before Unity regenerates the gameplay resource.
    # The default still requires byte-for-byte packaged-content validation.
    [switch]$SkipPackagedContent
)

# Build Assembly-CSharp.csproj first. Uses only Temp fixtures, never live saves.
$ErrorActionPreference = 'Stop'
$projectPath = Split-Path $PSScriptRoot -Parent
. (Join-Path $PSScriptRoot 'LoadUnityManagedAssemblies.ps1')
$null = Import-SF2ManagedRuntime $projectPath
$testRoot = Join-Path $projectPath ('Temp/offline-tests/' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $testRoot -Force | Out-Null
$checks = 0
function Assert-True($condition, [string]$label) {
    if (!$condition) { throw $label }
    $script:checks++
}
function Assert-SameBytes([byte[]]$left, [byte[]]$right, [string]$label) {
    Assert-True ([Convert]::ToBase64String($left) -ceq [Convert]::ToBase64String($right)) $label
}
function New-TestArchive($entries) {
    $output = New-Object IO.MemoryStream
    $gzip = New-Object IO.Compression.GZipStream($output, [IO.Compression.CompressionMode]::Compress, $true)
    $writer = New-Object IO.BinaryWriter($gzip, [Text.Encoding]::UTF8)
    $writer.Write('SF2XML1')
    $writer.Write([int]$entries.Count)
    foreach ($entry in $entries) {
        $writer.Write([string]$entry)
        $writer.Write([int]1)
        $writer.Write([byte]65)
    }
    $writer.Dispose()
    $result = $output.ToArray()
    $output.Dispose()
    return ,$result
}

$source = Join-Path $projectPath 'Assets/vanillaXml'
$archive = [GameplayContentArchive]::CreateArchive($source)
$cache = Join-Path $testRoot 'content'
$extracted = [GameplayContentArchive]::ExtractArchive($archive, $cache)
$files = @(Get-ChildItem -LiteralPath $source -Recurse -File | Where-Object Extension -ne '.meta')
foreach ($file in $files) {
    $relative = $file.FullName.Substring($source.Length + 1)
    $destination = Join-Path $extracted $relative
    Assert-True (Test-Path -LiteralPath $destination) ('Missing packaged file: ' + $relative)
    Assert-SameBytes ([IO.File]::ReadAllBytes($file.FullName)) ([IO.File]::ReadAllBytes($destination)) ('Changed XML bytes: ' + $relative)
}
Assert-True (@(Get-ChildItem -LiteralPath $extracted -Recurse -Filter '*.meta').Count -eq 0) 'Metadata must not be packaged'
Assert-True (([GameplayContentArchive]::ExtractArchive($archive, $cache)) -ceq $extracted) 'Content cache is not stable'
Assert-SameBytes $archive ([GameplayContentArchive]::CreateArchive($source)) 'Content archive is not deterministic'

# Verify the actual resource produced by Unity's build processor as well; its
# Mono gzip output need not match the desktop .NET compressor byte for byte.
if ($SkipPackagedContent) {
    Write-Warning 'SKIPPED: generated Unity gameplay resource comparison; source archive and runtime checks remain enabled.'
} else {
    $resource = Join-Path $projectPath 'Assets/Resources/SF2Content/gameplay.bytes'
    Assert-True (Test-Path -LiteralPath $resource) 'Run SF2 > Package Offline Gameplay XML in Unity before this test'
    $packaged = [GameplayContentArchive]::ExtractArchive([IO.File]::ReadAllBytes($resource), $cache)
    foreach ($file in $files) {
        $relative = $file.FullName.Substring($source.Length + 1)
        Assert-SameBytes ([IO.File]::ReadAllBytes($file.FullName)) ([IO.File]::ReadAllBytes((Join-Path $packaged $relative))) ('Unity resource differs: ' + $relative)
    }
}
foreach ($assembly in @([GameplayContentArchive].Assembly, [OfflineServices].Assembly)) {
    $removed = @($assembly.GetReferencedAssemblies() | Where-Object { $_.Name -match 'Facebook|GooglePlay|Purchasing|SmartFox|P31RestKit|Analytics|Advertisements|MobileNativePopups' })
    Assert-True ($removed.Count -eq 0) ('Removed SDK still referenced by ' + $assembly.GetName().Name)
}

# These bridges previously emitted unresolved iOS symbols in Android IL2CPP.
Assert-True ($null -eq [BPGAOEMIFNN]::OBGMKPLOMJL()) 'Legacy native device-ID bridge is still active'
[LLJPGGEJCPK]::JOIGJOFNIKI($true, $true, $true, $true, $true, $true)
[LLJPGGEJCPK]::HLEIFBABHLB()
foreach ($type in @([BPGAOEMIFNN], [LLJPGGEJCPK])) {
    $imports = @($type.GetMethods([Reflection.BindingFlags]'Public,NonPublic,Static') | Where-Object {
        $_.GetCustomAttributes([Runtime.InteropServices.DllImportAttribute], $false).Count -gt 0
    })
    Assert-True ($imports.Count -eq 0) ('Native imports remain in ' + $type.Name)
}
Assert-True (!(Test-Path (Join-Path $projectPath 'Assets/Plugins/MobileNativePopups.dll'))) 'Native popup DLL still ships'

# Compile the actual locale implementation with only Unity's language property
# substituted. An AndroidJavaClass dependency must not compile in this fixture.
$localeSource = Get-Content -Raw (Join-Path $projectPath 'Assets/Plugins/Assembly-CSharp-firstpass/PreciseLocale.cs')
$languageNames = @([regex]::Matches($localeSource, 'SystemLanguage\.(\w+)') | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique)
$localeSource = $localeSource.Replace('using UnityEngine;', 'using OfflineLocaleUnity;').Replace('public static class PreciseLocale', 'public static class OfflineLocaleFixture')
$localeFixture = @'
namespace OfflineLocaleUnity {
    public enum SystemLanguage { /* LANGUAGES */ }
    public static class Application { public static SystemLanguage systemLanguage; }
}
public static class OfflineLocaleTest {
    public static string[] Read(string language, string cultureName) {
        var previous = System.Threading.Thread.CurrentThread.CurrentCulture;
        try {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            OfflineLocaleUnity.Application.systemLanguage = (OfflineLocaleUnity.SystemLanguage)System.Enum.Parse(typeof(OfflineLocaleUnity.SystemLanguage), language);
            return new[] { OfflineLocaleFixture.BGMAJFGKCEB(), OfflineLocaleFixture.PBPAPAFAMJB(), OfflineLocaleFixture.FBPILFMCNGJ(), OfflineLocaleFixture.OHHPBPBCFPL(), OfflineLocaleFixture.HIMMFECDKCI() };
        } finally { System.Threading.Thread.CurrentThread.CurrentCulture = previous; }
    }
}
'@
Add-Type -TypeDefinition ($localeSource + $localeFixture.Replace('/* LANGUAGES */', ($languageNames -join ', ')))
foreach ($case in @(
    @('English', 'en-US', 'en_US', 'en', 'US', 'USD'),
    @('Polish', 'pl-PL', 'pl_PL', 'pl', 'PL', 'PLN'),
    @('Norwegian', 'nb-NO', 'no_NO', 'no', 'NO', 'NOK'),
    @('ChineseTraditional', 'zh-TW', 'zh_TW', 'zh', 'TW', 'TWD'),
    @('ChineseSimplified', 'zh-CN', 'zh_CN', 'zh', 'CN', 'CNY'),
    @('Unknown', '', 'en', 'en', '', ''),
    @('Polish', 'en-US', 'pl', 'pl', '', ''),
    @('Polish', 'pl', 'pl', 'pl', '', '')
)) {
    $locale = [OfflineLocaleTest]::Read($case[0], $case[1])
    for ($i = 0; $i -lt 4; $i++) { Assert-True ($locale[$i] -ceq $case[$i + 2]) ('Incorrect local locale field ' + $i + ' for ' + $case[0] + '/' + $case[1]) }
    Assert-True (($locale[4].Length -gt 0) -eq ($case[5].Length -gt 0)) 'Currency symbol fallback disagrees with region availability'
}

# The HTTP debug listener and unused mobile/cloud adapters no longer ship.
$gameAssembly = [NetworkController].Assembly
foreach ($removed in @('CUDLR.Server', 'CUDLRConsole', 'InternetUtils', 'DumpController',
    'PurchaseController', 'RemoteLicenseChecker', 'RemoteLicenseCache',
    'Nekki.SF2.Core.Permissions.PermissionsManager', 'Nekki.SF2.Core.Security.SecurityManager')) {
    Assert-True ($null -eq $gameAssembly.GetType($removed)) ('Archived service still compiled: ' + $removed)
}

# Compile the real local-alert adapter against a dialog fake. The callback must
# wait for selection, distinguish cancel, and never run twice. No Unity UI here.
$dialogs = Get-Content -Raw (Join-Path $projectPath 'Assets/Scripts/Assembly-CSharp/DialogsOpener.cs')
$alertMethods = [regex]::Matches($dialogs, '(?ms)^\tpublic static void OpenLocalAlertDialog\([^\r\n]*\)\r?\n\t\{.*?^\t\}')
Assert-True ($alertMethods.Count -eq 2) 'Local alert overloads not found'
$alertFixture = @'
using System;
public static class OfflineAlertFixture {
    public static Action<object> Selection;
    public static bool LiteralText;
    public static void PEDJMOMBJJI(string title, string message, string ok, string cancel, Action<object> selected, bool literalText = false) { Selection = selected; LiteralText = literalText; }
/* METHODS */
}
'@
Add-Type -TypeDefinition $alertFixture.Replace('/* METHODS */', (($alertMethods | ForEach-Object Value) -join "`n"))
$script:alertAccepted = 0
$script:alertCancelled = 0
$accept = [Action]{ $script:alertAccepted++ }
$cancel = [Action]{ $script:alertCancelled++ }
[OfflineAlertFixture]::OpenLocalAlertDialog('Title', 'Message', 'OK', 'Cancel', $accept, $cancel)
Assert-True ([OfflineAlertFixture]::LiteralText) 'Display text would be treated as localization keys'
Assert-True ($alertAccepted -eq 0 -and $alertCancelled -eq 0) 'Alert auto-selected without input'
[OfflineAlertFixture]::Selection.Invoke(1)
[OfflineAlertFixture]::Selection.Invoke(0)
Assert-True ($alertAccepted -eq 1 -and $alertCancelled -eq 0) 'OK callback is not exactly once'
[OfflineAlertFixture]::OpenLocalAlertDialog('Title', 'Message', 'OK', 'Cancel', $accept, $cancel)
[OfflineAlertFixture]::Selection.Invoke(0)
Assert-True ($alertAccepted -eq 1 -and $alertCancelled -eq 1) 'Cancel callback selected wrong action'
[OfflineAlertFixture]::OpenLocalAlertDialog('Title', 'Message', 'Reload', $accept)
[OfflineAlertFixture]::Selection.Invoke(0)
Assert-True ($alertAccepted -eq 2) 'Single-button alert dismiss did not complete'
[OfflineAlertFixture]::OpenLocalAlertDialog('Title', 'Message', 'OK', 'Cancel', $null, $null)
[OfflineAlertFixture]::Selection.Invoke(1)
$checks++

# Compile the settings helpers against UI fakes: desktop prefabs omit credits
# and support. This checks managed control flow, not Unity rendering/import.
$settingsSource = Get-Content -Raw (Join-Path $projectPath 'Assets/Scripts/Assembly-CSharp/Nekki/SF2/GUI/Dialogs/SettingsDialog.cs')
$settingsMethods = foreach ($name in @('OHDFPIADEIG', 'PGMBIJFAEHP', 'BOPCBJJIHNK')) {
    $method = [regex]::Match($settingsSource, '(?ms)^\t\tprotected void ' + $name + '\([^\r\n]*\)\r?\n\t\t\{.*?^\t\t\}')
    Assert-True $method.Success ('Settings helper not found: ' + $name)
    $method.Value.Replace('protected void', 'public void')
}
$settingsEnum = [regex]::Match($settingsSource, '(?m)^\t\tpublic enum AHDEAELNGBD\s*\{[^{}]*\}')
Assert-True $settingsEnum.Success 'Settings button IDs not found'
$settingsFixture = @'
using System;
public class OfflineSettingsFixture {
    public class GameObject { public bool activeSelf; public void SetActive(bool value) { activeSelf = value; } }
    public struct Vector2 { public float x, y; public Vector2(float x, float y) { this.x = x; this.y = y; } }
    public class Transform { public Vector2 localPosition; }
    public class ResolutionButton {
        public GameObject gameObject = new GameObject();
        public Transform transform = new Transform();
        public int ButtonId, Listeners;
        public bool interactable;
        public string Normal, Pressed;
        public void SetNormalSprite(string path, string sprite) { Normal = sprite; }
        public void SetPressedSprite(string path, string sprite) { Pressed = sprite; }
        public void RemoveEventListener(int id, Action<object> callback) { Listeners = 0; }
        public void AddEventListener(int id, Action<object> callback) { Listeners++; }
    }
    public enum TextAnchor { MiddleLeft }
    public class LabelAlias {
        public GameObject gameObject = new GameObject();
        public string Alias; public int FontSize, color; public TextAnchor alignment;
        public void set_Alias(string value) { Alias = value; }
        public void set_LabelFontSize(int value) { FontSize = value; }
    }
    public static class Constants { public const int PJJIMHMJPAL = 1; }
    public static class SystemProperties { public static bool DDIDANINPJE() { return true; } }
    public ResolutionButton btnMusic = new ResolutionButton(), btnSound = new ResolutionButton(),
        btnLanguage = new ResolutionButton(), btnGameCenter = new ResolutionButton(), btnCredits, btnSupport;
    public LabelAlias lblGameCenter = new LabelAlias(), lblItunes = new LabelAlias();
    public bool SelectedSprites;
    private bool PEPADDIALAO() { return SelectedSprites; }
    private void OnClickButton(object data) { }
/* ENUM */
/* METHODS */
}
'@
Add-Type -TypeDefinition $settingsFixture.Replace('/* ENUM */', $settingsEnum.Value).Replace('/* METHODS */', ($settingsMethods -join "`n"))
$settings = New-Object OfflineSettingsFixture
foreach ($buttonId in @('BTN_CREDITS', 'BTN_SUPPORT')) {
    $id = [OfflineSettingsFixture+AHDEAELNGBD][Enum]::Parse([OfflineSettingsFixture+AHDEAELNGBD], $buttonId)
    $settings.OHDFPIADEIG($null, 'normal', 'pressed', 0, 0, $id)
    $checks++
}
$settings.PGMBIJFAEHP($null, 'Settings_Credits')
$settings.PGMBIJFAEHP($null, 'Settings_Support')
$checks += 2
foreach ($selected in @($false, $true)) {
    $settings.SelectedSprites = $selected
    $settings.OHDFPIADEIG($settings.btnMusic, 'normal', 'pressed', -670, 200, [OfflineSettingsFixture+AHDEAELNGBD]::BTN_MUSIC)
    $expectedPressed = if ($selected) { 'pressed' } else { 'normal' }
    Assert-True ($settings.btnMusic.Normal -ceq 'normal' -and $settings.btnMusic.Pressed -ceq $expectedPressed) 'Settings music sprites changed'
    Assert-True ($settings.btnMusic.gameObject.activeSelf -and $settings.btnMusic.Listeners -eq 1 -and $settings.btnMusic.ButtonId -eq 0) 'Settings music activation/events changed'
    Assert-True ($settings.btnMusic.transform.localPosition.x -eq -670 -and $settings.btnMusic.transform.localPosition.y -eq 200) 'Settings music position changed'
}
$settingsLabel = New-Object OfflineSettingsFixture+LabelAlias
$settings.PGMBIJFAEHP($settingsLabel, 'Settings_Music')
Assert-True ($settingsLabel.gameObject.activeSelf -and $settingsLabel.Alias -ceq 'Settings_Music' -and $settingsLabel.FontSize -eq 101) 'Existing settings label no longer initializes'
foreach ($hide in @($true, $false)) {
    $settings.BOPCBJJIHNK($hide)
    Assert-True ($settings.btnMusic.gameObject.activeSelf -eq !$hide -and $settings.btnSound.gameObject.activeSelf -eq !$hide -and $settings.btnLanguage.gameObject.activeSelf -eq !$hide) 'Settings visibility fails without credits/support'
}

# A new content version must not overwrite user edits to the old extracted copy.
$fixture = Join-Path $testRoot 'fixture'
New-Item -ItemType Directory -Path $fixture | Out-Null
[IO.File]::WriteAllText((Join-Path $fixture 'test.xml'), '<Old />')
$first = [GameplayContentArchive]::ExtractArchive([GameplayContentArchive]::CreateArchive($fixture), $cache)
[IO.File]::WriteAllText((Join-Path $first 'test.xml'), '<Modded />')
[IO.File]::WriteAllText((Join-Path $fixture 'test.xml'), '<New />')
$second = [GameplayContentArchive]::ExtractArchive([GameplayContentArchive]::CreateArchive($fixture), $cache)
Assert-True ($first -cne $second) 'Changed XML did not produce a new content version'
Assert-True ([IO.File]::ReadAllText((Join-Path $first 'test.xml')) -ceq '<Modded />') 'Old content edits were overwritten'
Assert-True ([IO.File]::ReadAllText((Join-Path $second 'test.xml')) -ceq '<New />') 'New content version was not extracted'
foreach ($entries in @(@('../escape.xml'), @('/escape.xml'), @('C:/escape.xml'), @('a\escape.xml'), @('.complete'), @('same.xml', 'SAME.xml'))) {
    $rejected = $false
    try { [GameplayContentArchive]::ExtractArchive((New-TestArchive $entries), $cache) | Out-Null } catch { $rejected = $true }
    Assert-True $rejected ('Unsafe archive accepted: ' + ($entries -join ', '))
}
$damaged = [byte[]]$archive[0..([int]($archive.Length / 2))]
$rejected = $false
try { [GameplayContentArchive]::ExtractArchive($damaged, $cache) | Out-Null } catch { $rejected = $true }
Assert-True $rejected 'Truncated archive accepted'

# Hash validation and maintenance remain off before and after player initialization.
$save = Join-Path $testRoot 'users.xml'
[xml]$document = '<Root><Warriors /></Root>'
[IO.File]::WriteAllText($save, $document.OuterXml)
foreach ($initialize in @($false, $true)) {
    if ($initialize) { [GameSettings]::IFBKAJPILOI() }
    Assert-True (![GameSettings]::HCAJHNKLLGB()) 'Save validation enabled'
    Assert-True ([UserDataValidator]::CheckFileHash($document, $save)) 'Hashless local save rejected'
    [UserDataValidator]::UpdateFileHash($document, $save)
    [UserDataValidator]::UpdateFileHash($save)
    Assert-True (!(Test-Path -LiteralPath ($save + '.hash'))) 'Hash sidecar was written'
}
[IO.File]::WriteAllText(($save + '.hash'), 'stale-invalid-hash')
Assert-True ([UserDataValidator]::CheckFileHash($document, $save)) 'Stale save hash rejected'
[UserDataValidator]::KAFMCNCGOJH($save)
[UserDataValidator]::NLIJEIGOALP($save, (Join-Path $testRoot 'copy.xml'))
Assert-True ([IO.File]::ReadAllText(($save + '.hash')) -ceq 'stale-invalid-hash') 'Existing hash sidecar was modified'
Assert-True (!(Test-Path -LiteralPath (Join-Path $testRoot 'copy.xml.hash'))) 'Hash sidecar was copied'

$store = [ICFMIHIKGOD]::OFFDIMCJOIC()
Assert-True (!$store.LCFBJGONPBH()) 'Store must report unavailable'
Assert-True ($store.NABJBCEKEHK().Length -eq 0) 'Offline store exposes products'
$script:purchaseFailed = 0
$script:purchaseFinished = 0
$script:purchaseGranted = 0
$store.ENCIAJBEOEA = [Action[string,SF2.Offline.PurchaseFailureReason]]{ param($id, $reason) if ($id -ceq 'test' -and $reason -eq [SF2.Offline.PurchaseFailureReason]::PurchasingUnavailable) { $script:purchaseFailed++ } }
$store.JOFLHEEPJIB = [Action[string,string]]{ param($id,$receipt) $script:purchaseFinished++ }
$store.JEAJAJMDPNL = [Action[string]]{ param($id) $script:purchaseGranted++ }
$store.BDAAKHOLPOF('test')
Assert-True ($script:purchaseFailed -eq 1 -and $script:purchaseFinished -eq 1 -and $script:purchaseGranted -eq 0) 'Offline purchase callbacks are incorrect'

foreach ($url in @('https://example.com', 'http://127.0.0.1/service', '//server/share', 'file://server/share/file.xml', 'jar:https://example.com/app.apk')) {
    Assert-True (![OfflineServices]::IsLocalContent($url)) ('Network URL allowed: ' + $url)
}
foreach ($url in @('file:///C:/game/data.xml', 'file:///data/user/0/game/files/data.xml', 'jar:file:///data/app/base.apk!/assets/data.xml')) {
    Assert-True ([OfflineServices]::IsLocalContent($url)) ('Local content rejected: ' + $url)
}
Assert-True ([Nekki.SF2.Core.Network.ServerProvider]::OFFLINE) 'Backend is not permanently offline'

# Reproduces the PC dojo startup crash: no remote ledger config is loaded.
Assert-True ($null -eq [GeneralConfig]::ELEBLBJKDBI().IMOKGIDCANG()) 'Ledger regression fixture unexpectedly has server settings'
(New-Object LedgerManager).Check() # Must return without server config, Unity, or a roster.
$checks++

# The HTTP client uses Unity's native Debug API in its static initializer;
# verify that callback path with SF2 > Validate Offline Services in the editor.
Write-Output "PASS: $checks offline runtime assertions; $($files.Count) packaged files, $($archive.Length) compressed bytes."
