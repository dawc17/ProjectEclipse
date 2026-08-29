# Build Assembly-CSharp.csproj first. Exercises the compiled replay progression
# with real stage definitions and in-memory roster XML. The extracted Eclipse
# action uses fake scene/quest/save services; no Unity UI or player saves.
$ErrorActionPreference = 'Stop'
$projectPath = Split-Path $PSScriptRoot -Parent
$assembly = [Reflection.Assembly]::LoadFrom((Join-Path $projectPath 'Temp/Bin/Debug/Assembly-CSharp.dll'))
$checks = 0
function Assert-True($condition, [string]$label) {
    if (!$condition) { throw $label }
    $script:checks++
}
$flags = [Reflection.BindingFlags]'Instance,NonPublic'
function Set-BattleField($battle, [string]$field, $value) {
    [Battle].GetField($field, $flags).SetValue($battle, $value.PSObject.BaseObject)
}
function New-BattleFixture([System.Xml.XmlElement]$definition, [int]$wins = 0, [int]$cycle = 0) {
    $type = switch ($definition.GetAttribute('Type')) {
        'REPLAYABLE' { [BattleType]::FightReplayable }
        'BOSSES_REPLAYABLE' { [BattleType]::FightBossesReplayable }
        'FINAL_BATTLE_REPLAYABLE' { [BattleType]::FightFinalReplayable }
        default { [BattleType]::FightBosses }
    }
    $runtimeType = if ($type -in @([BattleType]::FightReplayable, [BattleType]::FightBossesReplayable, [BattleType]::FightFinalReplayable)) { [BattleReplayable] } else { [Battle] }
    $battle = [Runtime.Serialization.FormatterServices]::GetUninitializedObject($runtimeType)
    Set-BattleField $battle '_name' $definition.GetAttribute('Name')
    Set-BattleField $battle '_type' $type
    $data = New-Object DeflatedString
    $data.Set($definition)
    Set-BattleField $battle 'CMDDPMAAJOF' $data
    [xml]$save = '<Battle Name="ZONE_2|Test|" ReplayCount="0" Locked="0" Hidden="0" />'
    $roster = New-Object RosterBattle -ArgumentList $save.DocumentElement
    $roster.FHCHCHPPMEI($cycle)
    $battle.FOMHAGJJCLJ($roster)
    $fights = New-Object 'System.Collections.Generic.List[FightList]'
    foreach ($node in $definition.SelectNodes('Fight')) {
        $fight = [Runtime.Serialization.FormatterServices]::GetUninitializedObject([FightList])
        $fight.Name = $node.GetAttribute('Name')
        $fight.Index = $fights.Count
        $fight.CNAOMDMIGLJ = $battle
        $fight.EJGGHHEOGPG = [int]$node.GetAttribute('Replays')
        $fight.set_Type($type)
        [xml]$fightSave = '<Fight CompletedCount="0" EclipseCompletedCount="0" LossCount="3" EclipseLossCount="2" StoryCount="4" RandomGroupSeed="123" RandomRuleSeed="456" />'
        $rosterFight = New-Object RosterFight -ArgumentList $fightSave.DocumentElement
        $rosterFight.OBFNFKPHJIN($wins * $fight.EJGGHHEOGPG)
        $rosterFight.BIINCAKDHLP($wins * $fight.EJGGHHEOGPG)
        $fight.HOCFLEMFFKC($rosterFight)
        if ($battle -is [BattleReplayable]) { $battle.MJJFFAOLCCK($fight) }
        else { $fight.PGBKNLAEANJ = [ConditionStatus]::StatusComplete }
        $fights.Add($fight)
    }
    Set-BattleField $battle 'JNPMCNMEOLE' $fights
    Set-BattleField $battle 'AIKIOPMGCEG' ([ushort]$fights.Count)
    Set-BattleField $battle 'NLLECKHLMAN' $true
    return $battle
}

[xml]$stages = Get-Content -Raw (Join-Path $projectPath 'Assets/xml/stages.xml')
$definitions = $stages.SelectNodes('//Battle[contains(@Name,"ECLIPSEMODE") and (@Type="REPLAYABLE" or @Type="BOSSES_REPLAYABLE" or @Type="FINAL_BATTLE_REPLAYABLE")]')
foreach ($definition in $definitions) {
    $battle = New-BattleFixture $definition
    Assert-True (!$battle.TryStartNextReplay()) ($battle.get_Name() + ': untouched battle advanced')
    for ($cycle = 1; $cycle -le 3; $cycle++) {
        $fights = $battle.ANNHMNIHKCC()
        foreach ($fight in $fights) {
            for ($win = 0; $win -lt $fight.EJGGHHEOGPG; $win++) {
                $fight.FLKFFDLLBKA().GICDABHEMML()
                $fight.FLKFFDLLBKA().LOEBHEODPAH()
            }
            $battle.MJJFFAOLCCK($fight)
            if ($fight -ne $fights[$fights.Count - 1]) {
                Assert-True (!$battle.TryStartNextReplay()) ($battle.get_Name() + ': partial segment reset')
            }
        }
        $before = @($fights | ForEach-Object { $_.FLKFFDLLBKA().LIGMHKEOJBB().OuterXml }) -join "`n"
        Assert-True ($battle.TryStartNextReplay()) ($battle.get_Name() + ': completed segment did not reopen')
        Assert-True ($battle.HLBOMMKJAAO() -eq $cycle) 'Wrong replay cycle'
        Assert-True ($battle.FBFHBKPFLJC() -eq $fights[0]) 'Replay did not restart at first opponent'
        Assert-True ($battle.MNHLGELMOEJ() -eq [ConditionStatus]::StatusOpen) 'Battle remains completed'
        $after = @($fights | ForEach-Object { $_.FLKFFDLLBKA().LIGMHKEOJBB().OuterXml }) -join "`n"
        Assert-True ($before -ceq $after) 'Replay changed lifetime wins/losses, seeds or story counters'
        Assert-True (!$battle.TryStartNextReplay()) 'Replay update was not idempotent'
    }
}

$hermit = $stages.SelectSingleNode('//Zone[@Name="ZONE_2"]/Battle[@Name="BOSS_HERMIT_ECLIPSEMODE"]')
$battle = New-BattleFixture $hermit 4 0
Assert-True ($battle.TryStartNextReplay()) 'Stale completed save did not recover'
Assert-True ($battle.HLBOMMKJAAO() -eq 4) 'Stale cycle did not catch up to saved wins'
$rosterNode = [RosterBattle].GetField('_node', $flags).GetValue($battle.NNPNEABKHPP())
$reloaded = New-Object RosterBattle -ArgumentList $rosterNode.CloneNode($true)
Assert-True ($reloaded.ODCFKCJJDKN() -eq 4) 'ReplayCount was not serialized'
$battle.FOMHAGJJCLJ($reloaded)
Assert-True (!$battle.TryStartNextReplay()) 'Reload advanced the cycle again'
$battle.ANNHMNIHKCC()[0].FLKFFDLLBKA().GICDABHEMML()
$battle.MJJFFAOLCCK($battle.ANNHMNIHKCC()[0])
Assert-True (!$battle.TryStartNextReplay()) 'Partially replayed bodyguards reset'
Assert-True ($battle.FBFHBKPFLJC().Index -eq 1) 'Partial replay lost its next opponent'
$battle = New-BattleFixture $hermit 1
$battle.NNPNEABKHPP().HLNEICNJDCF($true)
Assert-True (!$battle.TryStartNextReplay()) 'Explicitly locked battle was reopened'
$battle = New-BattleFixture $hermit 1
Set-BattleField $battle 'MEOMPEEPCJJ' $null
Assert-True (!$battle.TryStartNextReplay()) 'Missing roster was reopened'
$battle = New-BattleFixture $hermit 1
[FightList].GetField('ECHMHCODAFA', $flags).SetValue($battle.ANNHMNIHKCC()[0], $null)
Assert-True (!$battle.TryStartNextReplay()) 'Missing fight progress was treated as complete'
$battle = New-BattleFixture $hermit 1
$battle.ANNHMNIHKCC()[0].EJGGHHEOGPG = 0
Assert-True (!$battle.TryStartNextReplay()) 'Unlimited fight was treated as a finite segment'

# Run the production Eclipse action with actual Battle/RosterFight objects.
# Only its external services are replaced, allowing map-refresh/save assertions.
$actionSource = Get-Content -Raw (Join-Path $projectPath 'Assets/Scripts/Assembly-CSharp/QuestAction.cs')
$action = [regex]::Match($actionSource, '(?ms)^public class QuestActionUpdateEclipseBattles : QuestAction\r?\n\{.*?^\}')
Assert-True $action.Success 'Cannot find Eclipse update action'
$shim = @'
using System;
using System.Collections.Generic;
using System.Xml;
namespace EclipseRuntimeTest {
    public class QuestParameters { }
    public class QuestAction {
        public virtual void DEJMHFMLKIC(QuestParameters parameters) { }
        protected void OGIJONMKABB() { }
    }
    public class Roster {
        public bool EclipseMode = true;
        public bool JPMPIDFGCJL() { return EclipseMode; }
    }
    public class ListSF {
        public static ListSF Instance = new ListSF();
        public static Roster Roster = new Roster();
        public List<Battle> Battles = new List<Battle>();
        public int Saves;
        public static ListSF ELEBLBJKDBI() { return Instance; }
        public static Roster CCDKHLAMKKO() { return Roster; }
        public List<Battle> MMCHMBIKIEP() { return Battles; }
        public void EJANJEEGOOE() { Saves++; }
    }
    public class Scene<T> where T : new() {
        public static T Current = new T();
        public static T get_Current() { return Current; }
    }
    public class ZoneScrollItem {
        public Battle Selected;
        public Battle get_LastBattle() { return Selected; }
    }
    public class MapScene {
        public ZoneScrollItem Zone = new ZoneScrollItem();
        public int Reselections;
        public ZoneScrollItem GetCurrentZone() { return Zone; }
        public void UpdateBattleButtonHidden(Battle battle) { }
        public void UpdateCurrentZone() { }
        public void SelectBattle(Battle battle, float duration) { Zone.Selected = battle; Reselections++; }
    }
/* ACTION */
}
'@
$refs = @($assembly.Location)
if ($PSVersionTable.PSEdition -eq 'Core') {
    foreach ($name in @('mscorlib', 'System.Collections', 'System.Xml', 'System.Xml.ReaderWriter', 'System.Xml.XmlDocument')) {
        $refs += Join-Path $PSHOME ('ref/' + $name + '.dll')
    }
} else { $refs += 'System.Xml' }
Add-Type -TypeDefinition $shim.Replace('/* ACTION */', $action.Value) -ReferencedAssemblies $refs
$normal = New-BattleFixture $stages.SelectSingleNode('//Zone[@Name="ZONE_2"]/Battle[@Name="BOSS_HERMIT"]') 1
$eclipse = New-BattleFixture $hermit 1
$zone = New-Object Zone -ArgumentList @('ZONE_2','test')
foreach ($entry in @($normal, $eclipse)) {
    $entry.EENNGGIMMMI($zone)
    $zone.LGIIBNJFADA.Add($entry)
    [EclipseRuntimeTest.ListSF]::Instance.Battles.Add($entry)
}
$normal.NNPNEABKHPP().HCEOCBOFIGC($true)
$map = [EclipseRuntimeTest.Scene[EclipseRuntimeTest.MapScene]]::Current
$map.Zone.Selected = $eclipse
$update = New-Object EclipseRuntimeTest.QuestActionUpdateEclipseBattles
$update.DEJMHFMLKIC($null)
Assert-True ($eclipse.HLBOMMKJAAO() -eq 1) 'Eclipse action did not advance completed Hermit'
Assert-True ([EclipseRuntimeTest.ListSF]::Instance.Saves -eq 1) 'Progress-only change was not saved'
Assert-True ($map.Reselections -eq 1) 'Progress-only change did not rebuild selected preview'
Assert-True ($normal.MNHLGELMOEJ() -eq [ConditionStatus]::StatusComplete) 'Normal story completion changed'
$update.DEJMHFMLKIC($null)
Assert-True ([EclipseRuntimeTest.ListSF]::Instance.Saves -eq 1) 'Repeated update rewrote progress'
$eclipse.ANNHMNIHKCC()[0].FLKFFDLLBKA().GICDABHEMML()
$eclipse.MJJFFAOLCCK($eclipse.ANNHMNIHKCC()[0])
foreach ($mode in @($false, $true, $false, $true)) {
    [EclipseRuntimeTest.ListSF]::Roster.EclipseMode = $mode
    $update.DEJMHFMLKIC($null)
    Assert-True ($eclipse.HLBOMMKJAAO() -eq 1) 'Mode toggle reset a partial replay'
    Assert-True ($eclipse.FBFHBKPFLJC().Index -eq 1) 'Mode toggle lost bodyguard progress'
    Assert-True ($eclipse.KBPNDJPMCCG() -eq !$mode) 'Eclipse visibility did not follow mode'
}
Write-Output "PASS: $checks Eclipse assertions across $($definitions.Count) replay segments (three cycles each, saved progress, action integration and map refresh)."
