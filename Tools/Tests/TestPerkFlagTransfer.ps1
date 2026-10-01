$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$sourceRoot = Join-Path $root 'Assets/Scripts/Assembly-CSharp'
$stageSource = Get-Content -Raw -LiteralPath (Join-Path $sourceRoot 'PerksStage.cs')
$infoSource = Get-Content -Raw -LiteralPath (Join-Path $sourceRoot 'InfoPerk.cs')
$stagePatterns = @(
    '(?ms)^\tpublic class ActionPerk\r?\n\t\{.*?^\t\}',
    '(?ms)^    internal System.Action TransferFormEffects\(.*?^    \}',
    '(?ms)^    private static bool IsFormBodyModifier\(.*?^    \}',
    '(?ms)^    internal System.Action RebindQueuedFormActions\(.*?^    \}',
    '(?ms)^    internal System.Action ReplaceFormRegistration\(.*?^    \}',
    '(?ms)^    internal PerkModelStruct PrepareModelRegistration\(.*?^    \}',
    '(?ms)^    internal void RequireFormReferencesTransferred\(.*?^    \}',
    '(?ms)^\tpublic static void HKMMGCLNJCN\(.*?^\t\}',
    '(?ms)^\tpublic static void AEMBNMFGDBN\(.*?^\t\}',
    '(?ms)^\tpublic static void EHFKNCOOCAA\(.*?^\t\}',
    '(?ms)^\tpublic static bool CheckModNameInNamespace\(.*?^\t\}',
    '(?ms)^\tpublic static ActionPerk AFAGHKFHHIF\(.*?^\t\}',
    '(?ms)^\tpublic static List<ActionPerk> DOAECFNPKIO\(.*?^\t\}',
    '(?ms)^\tpublic void CLBPEANCNOA\(.*?^\t\}'
)
$infoPatterns = @(
    '(?ms)^\tpublic List<PerksStage.ActionPerk> MNLNLKOJPHO\(.*?^\t\}',
    '(?ms)^\tpublic List<PerksStage.ActionPerk> HIPOGANEPMI\(.*?^\t\}',
    '(?ms)^\tpublic List<string> BFKDLIMHGFA\(.*?^\t\}',
    '(?ms)^\tpublic List<string> BKIMFEIMHCF\(.*?^\t\}',
    '(?ms)^\tpublic void Run\(.*?^\t\}',
    '(?ms)^\tpublic void MHHNIPBJNAD\(.*?^\t\}',
    '(?ms)^\tpublic void Render\(.*?^\t\}',
    '(?ms)^\tprivate void CAIPNAAJICO\(.*?^\t\}',
    '(?ms)^\tpublic void ClearActions\(.*?^\t\}',
    '(?ms)^\tprivate void ACKKGAAPLDG\(.*?^\t\}',
    '(?ms)^\tprivate void BBEMBELMEGP\(.*?^\t\}',
    '(?ms)^\tprivate void KKODDGMCDBC\(.*?^\t\}'
)
function Extract-Methods([string]$source, [string[]]$patterns) {
    $methods = foreach ($pattern in $patterns) {
        $methodMatches = [regex]::Matches($source, $pattern)
        if ($methodMatches.Count -ne 1) { throw "Expected one production method for $pattern; found $($methodMatches.Count)." }
        $methodMatches[0].Value
    }
    return $methods -join "`n"
}
$fixture = Join-Path $root ('Temp/PerkFlagTransfer-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixture | Out-Null
$code = Get-Content -Raw -LiteralPath (Join-Path $PSScriptRoot 'ValidatePerkFlagTransfer.cs')
$code = $code.Replace('/* STAGE_METHODS */', (Extract-Methods $stageSource $stagePatterns))
$code = $code.Replace('/* INFO_METHODS */', (Extract-Methods $infoSource $infoPatterns))
[IO.File]::WriteAllText((Join-Path $fixture 'Program.cs'), $code)
foreach ($name in @('ActionType.cs', 'PerkActionFlag.cs', 'PerkActionModificator.cs', 'PerkActionVariable.cs', 'PerkConditionModExists.cs')) {
    Copy-Item -LiteralPath (Join-Path $sourceRoot $name) -Destination $fixture
}
[IO.File]::WriteAllText((Join-Path $fixture 'Test.csproj'), '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>')
$arguments = @('run', '--project', (Join-Path $fixture 'Test.csproj'), '--verbosity', 'quiet')
dotnet @arguments
if ($LASTEXITCODE -ne 0) { throw 'Perk flag transfer checks failed.' }
