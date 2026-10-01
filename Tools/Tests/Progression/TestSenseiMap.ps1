# Reuse the actual Lua fixture and verify its bindings before exercising DE logic.
. (Join-Path $PSScriptRoot 'TestBattleLocks.ps1')
Copy-Item (Join-Path $root 'Mods/de128/scripts/content/sensei_map.lua') (Join-Path $package 'scripts/content/sensei_map.lua')
$script:checks=0
$script:events=[Collections.Generic.List[string]]::new()
$script:entries=@{}
$script:failAt=0
function Visit([string]$action) {
    $script:events.Add($action)
    return ($script:failAt -eq 0 -or $script:events.Count -ne $script:failAt)
}
[Eclipse.Modding.ModBattleAccess]::Reveal=[Func[Eclipse.Modding.DefinitionId,bool,bool]] {
    param($id,$locked)
    $act=[int]$id.ToString().Substring($id.ToString().Length-1)
    if(!(Visit "reveal:$act`:$locked")){return $false}
    if(!$script:entries.ContainsKey($act)){$script:entries[$act]=$locked}
    return $true
}
[Eclipse.Modding.ModBattleAccess]::SetLocked=[Func[Eclipse.Modding.DefinitionId,bool,bool]] {
    param($id,$locked)
    $act=[int]$id.ToString().Substring($id.ToString().Length-1)
    if(!(Visit "lock:$act`:$locked") -or !$script:entries.ContainsKey($act)){return $false}
    $script:entries[$act]=$locked
    return $true
}
[Eclipse.Modding.ModBattleAccess]::Focus=[Func[Eclipse.Modding.DefinitionId,bool]] {
    param($id)
    $act=[int]$id.ToString().Substring($id.ToString().Length-1)
    return ((Visit "focus:$act") -and $script:entries.ContainsKey($act))
}
$prefix=@'
local zone=sf2.zones.register{id="test"}
local battles={}
for act=1,6 do battles[act]=sf2.battles.register{id="act"..act,zone=zone,type=sf2.battles.STORY} end
local map=require("content.sensei_map")
'@
[xml]$archive=Get-Content -Raw (Join-Path $root 'Assets/DExml/quests.xml')
try {
    for($act=1;$act -le 6;$act++){
        $quest=$archive.SelectSingleNode("//Quest[@Name='SenseiZone${act}Notify']")
        $actions=$quest.Actions
        $shows=@($actions.ShowBattle)
        Check ($actions.Place -eq 'Map') 'Archived map placement differs.'
        Check ($shows.Count -eq $(if($act -eq 1){6}else{1})) 'Archived reveal/unlock count differs.'
        $expected=[Collections.Generic.List[string]]::new()
        foreach($show in $shows){
            $index=[int]([regex]::Match($show.Name,'^ZONE_([1-6])\|SENSEI_MEMORIES$').Groups[1].Value)
            $locked=$show.Locked -eq '1'
            if($act -eq 1){$expected.Add("reveal:$index`:$locked")}
            else {Check ($index -eq $act -and !$locked) 'Archived later-act unlock differs.'}
        }
        $expected.Add("lock:$act`:False");$expected.Add("focus:$act")
        Check ($actions.SetMapFocus.Battle -eq "ZONE_$act|SENSEI_MEMORIES|") 'Archived focus differs.'
        $script:events.Clear();$script:failAt=0
        $null=Load-Lua ($prefix+"assert(map.open_act($act,battles))")
        Check (($script:events -join ',') -ceq ($expected -join ',')) "Act $act map ordering differs."
        for($i=1;$i -le 6;$i++){Check ($script:entries[$i] -eq ($i -gt $act)) "Act $act lock state $i differs."}
    }
    # Failure at every Act I operation stops the sequence; retry preserves entries.
    for($stop=1;$stop -le 8;$stop++){
        $script:entries=@{};$script:events.Clear();$script:failAt=$stop
        $null=Load-Lua ($prefix+'assert(not map.open_act(1,battles))')
        Check ($script:events.Count -eq $stop) 'Sequence continued after failed operation.'
        $script:events.Clear();$script:failAt=0
        $null=Load-Lua ($prefix+'assert(map.open_act(1,battles))')
        Check ($script:entries.Count -eq 6 -and !$script:entries[1]) 'Partial retry did not finish first act.'
        for($i=2;$i -le 6;$i++){Check $script:entries[$i] 'Partial retry unlocked future act.'}
    }
    $script:entries=@{};$script:events.Clear()
    $null=Load-Lua ($prefix+'assert(not map.open_act(2,battles))')
    Check (($script:events -join ',') -eq 'lock:2:False') 'Later act fabricated initial entries or focused after refusal.'
    foreach($value in @('0','7','1.5','"1"','nil')){
        $script:events.Clear();$failed=$false
        try{$null=Load-Lua ($prefix+"map.open_act($value,battles)")}catch{$failed=$true}
        Check ($failed -and $script:events.Count -eq 0) 'Invalid act changed map.'
    }
} finally {[Eclipse.Modding.ModBattleAccess]::Clear()}
Write-Output "PASS: $script:checks Sensei map sequence checks against historical notification actions. Controlled host; no story activation. Evidence: $fixture"
