# Headless regression harness. Compiles the actual tutorial action and selected
# Profile/ModelContainer methods against UI fakes; does not launch Unity or touch saves.
$ErrorActionPreference = 'Stop'
$projectPath = Split-Path $PSScriptRoot -Parent
$sourceRoot = Join-Path $projectPath 'Assets/Scripts/Assembly-CSharp'
function Read-Method([string]$source, [string]$name, [int]$indent = 1) {
    $tabs = "`t" * $indent
    $pattern = '(?ms)^' + $tabs + '(?:private|public) [^\r\n]+\b' + [regex]::Escape($name) + '\([^\r\n]*\)\r?\n' + $tabs + '\{.*?^' + $tabs + '\}'
    $match = [regex]::Match($source, $pattern)
    if (!$match.Success) { throw "Cannot find runtime method $name" }
    return $match.Value
}
$profile = Get-Content -Raw (Join-Path $sourceRoot 'ProfileScene.cs')
$methods = foreach ($name in @('CGFOHBFAJBL', 'LDFKBJAHGII', 'OOHJAHDHEAP', 'HPHAOJDPNND', 'LMPGJLLBFPP', 'MONAFDKJKOP', 'PlayAnimation', 'ReleaseTrickPreviewInput')) { Read-Method $profile $name }
$model = Get-Content -Raw (Join-Path $sourceRoot 'Nekki/SF2/Core/Fights/ModelContainer.cs')
$tryPlay = Read-Method $model 'TryPlayAnimation' 2
$action = Get-Content -Raw (Join-Path $sourceRoot 'QuestActionStoryTutorialShowBlock.cs')
$action = [regex]::Replace($action, '(?m)^using [^;]+;\r?\n', '')
$fixture = Get-Content -Raw (Join-Path $PSScriptRoot 'TutorialRuntimeFixture.cs')
$fixture = $fixture.Replace('/* PROFILE_METHODS */', ($methods -join "`n")).Replace('/* TRY_PLAY_METHOD */', $tryPlay).Replace('/* TUTORIAL_ACTION */', $action)
Add-Type -TypeDefinition $fixture
[TutorialRegression]::Run()
