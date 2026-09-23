# Underworld modding API: Lua registration through the production bindings and the
# production LegacyContentAdapter projection (reflection), no Unity scene.
. (Join-Path $PSScriptRoot 'TestWarriorPerkLoadouts.ps1')
$script:checks = 0
function Build($catalog, [string]$method, $definition) {
    $adapter = [Eclipse.Modding.LegacyContentAdapter]::new($catalog)
    $flags = [Reflection.BindingFlags]'Instance,NonPublic,Public'
    $info = [Eclipse.Modding.LegacyContentAdapter].GetMethod($method, $flags)
    if ($definition -is [Management.Automation.PSObject]) { $definition = $definition.PSObject.BaseObject }
    if ($info.GetParameters().Count -eq 1) { return $info.Invoke($adapter, @($definition)) }
    return $info.Invoke($adapter, @([Xml.XmlDocument]::new(), $definition))
}
function Fails([string]$lua, [string]$fragment) {
    try { $null = Load-Lua $lua; return $false } catch { return $_.ToString().Contains($fragment) }
}
$base = @'
local zone=sf2.zones.register{id="tier",file="Raid1.1",underworld=true}
local story=sf2.zones.register{id="story"}
local face=sf2.assets.sprite("sprites/sensei/boss_hermit_young")
local sword=sf2.items.get("core:items/weapon/WEAPON_KATANA")
local default=sf2.warriors.get_template("core:warrior-templates/default")
local parent=sf2.warriors.register_template{id="boss",template=default,first_name="RAIDBOSS_FIRE",avatar="boss_fire",voice="Male",
  health_bars=15,attributes={ShieldStack=15,EnchantmentResistance=100},items={sword},skeleton="SkeletonHeavy"}
local child=sf2.warriors.register_template{id="boss_twin",template=parent,avatar=face}
local p=sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_TIME_BOMB_WEAPON")
local w=sf2.warriors.register{id="fighter",template=child,tactic="Aggressive",attributes={MagicInitialCharge=5000,WarriorPower=51},
  health_bars=20,perks={{perk=p,chance=0.3,parameters={DamageFactor=0.5,Health=2}}}}
local hot=sf2.rules.hot_ground{id="hot",frames=720,nodes={{name="NPivot",axis="Y",max=30}},animations={"Jump"}}
local jump=sf2.rules.no_animation{id="nojump",name="Jump"}
local area=sf2.rules.random_area{id="area",image="ra_bleed",icon="ra_bleed_icon",width=500,fade_in=60,frames_on=180,fade_out=60,frames_off=240}
local label=sf2.localization.register{id="hot_label",language="eng",value="Hot ground"}
local group=sf2.rules.group{id="group",description=label,rules={hot,area}}
local bleed=sf2.rules.perk{id="bleed",perk=p,target=sf2.rules.OPPONENT,aspect=100000,parameters={AttributeBleeding=0.2}}
local pick=sf2.rules.random{id="pick",refresh="each_round",rules={jump,bleed}}
local flag=sf2.rules.no_health_bar{id="nohp"}
local invert=sf2.rules.invert_joystick{id="invert",target=sf2.rules.PLAYER}
local reward=sf2.rewards.register{id="win",prize_base=1,experience=1,gems=17,currencies={{currency="ForgeMaterial1",expected=50},{currency="ForgeMaterial3",expected=9,show=false}}}
local b=sf2.battles.register{id="boss_1",zone=zone,type=sf2.battles.FINAL,power_mode="normal",icons={base=face,active=face}}
local hard=sf2.battles.register{id="boss_1_hard",zone=zone,type=sf2.battles.FINAL,power_mode="power"}
local always=sf2.battles.register{id="gauntlet",zone=zone,type=sf2.battles.SURVIVAL}
sf2.fights.register{id="f",battle=b,rounds=1,round_time=999,warriors={w},rules={group,pick,flag,invert},rewards={reward}}
'@
$catalog = Load-Lua $base
function One($collection, [string]$local) { return @($collection | Where-Object { $_.Id.LocalId -eq $local })[0] }

# Templates: owned body, parent-first projection, skeleton and inherited parent names.
$parent = One $catalog.WarriorTemplates 'boss'; $child = One $catalog.WarriorTemplates 'boss_twin'
$parentNode = Build $catalog 'BuildTemplateNode' $parent
Check ($parentNode.LocalName -eq 'Template' -and $parentNode.GetAttribute('Name') -eq 'fixture.warriors:warrior-templates/boss' -and
    $parentNode.GetAttribute('Template') -eq 'Default' -and $parentNode.GetAttribute('ShieldTotal') -eq '15' -and
    $parentNode.GetAttribute('ShieldStack') -eq '15' -and $parentNode.GetAttribute('EnchantmentResistance') -eq '100' -and
    !$parentNode.HasAttribute('EclipseCharacterId')) ('Template node differs: ' + $parentNode.OuterXml)
Check ((@($parentNode.SelectNodes('Items/Item') | ForEach-Object { $_.GetAttribute('Name') }) -join ',') -eq 'WEAPON_KATANA,SkeletonHeavy') 'Template items/skeleton differ.'
$childNode = Build $catalog 'BuildTemplateNode' $child
Check ($childNode.GetAttribute('Template') -eq 'fixture.warriors:warrior-templates/boss' -and $childNode.GetAttribute('Avatar') -eq 'fixture.warriors:sprites/sensei/boss_hermit_young') 'Child template differs.'
$ordered = [Eclipse.Modding.LegacyContentAdapter].GetMethod('OrderedModTemplates', [Reflection.BindingFlags]'Instance,NonPublic').Invoke([Eclipse.Modding.LegacyContentAdapter]::new($catalog), @())
Check (@($ordered | ForEach-Object { $_.Id.LocalId }) -join ',' -eq 'boss,boss_twin') 'Templates are not ordered parent-first.'

# Warrior: owned template reference, native attributes and perk parameters.
$warrior = Build $catalog 'BuildWarriorNode' (One $catalog.Warriors 'fighter')
$set = $warrior.SelectSingleNode('Perks/Perk/Set')
Check ($warrior.GetAttribute('Template') -eq 'fixture.warriors:warrior-templates/boss_twin' -and $warrior.GetAttribute('MagicInitialCharge') -eq '5000' -and
    $warrior.GetAttribute('WarriorPower') -eq '51' -and $warrior.GetAttribute('ShieldTotal') -eq '20') ('Warrior node differs: ' + $warrior.OuterXml)
Check ($set.GetAttribute('Chance') -eq '0.3' -and $set.GetAttribute('DamageFactor') -eq '0.5' -and $set.GetAttribute('Health') -eq '2') ('Perk parameters differ: ' + $set.OuterXml)

# Rules: native wrappers, nested children, descriptions and parameters.
$groupNode = Build $catalog 'BuildRuleNode' (One $catalog.FightRules 'group')
Check ($groupNode.LocalName -eq 'ComplexRule' -and $groupNode.FirstChild.LocalName -eq 'Description' -and
    $groupNode.FirstChild.GetAttribute('Alias') -eq 'fixture.warriors:localization/hot_label' -and
    (@($groupNode.ChildNodes | ForEach-Object LocalName) -join ',') -eq 'Description,HotGround,RandomArea') ('Group node differs: ' + $groupNode.OuterXml)
$areaNode = $groupNode.SelectSingleNode('RandomArea')
Check ($areaNode.GetAttribute('Image') -eq 'ra_bleed' -and $areaNode.GetAttribute('Icon') -eq 'ra_bleed_icon' -and $areaNode.GetAttribute('Width') -eq '500' -and
    $areaNode.GetAttribute('FramesOff') -eq '240' -and !$areaNode.HasAttribute('ApplyTo')) ('Random area differs: ' + $areaNode.OuterXml)
$pickNode = Build $catalog 'BuildRuleNode' (One $catalog.FightRules 'pick')
Check ($pickNode.LocalName -eq 'RandomRule' -and $pickNode.GetAttribute('Refresh') -eq 'EachRound' -and
    $pickNode.SelectSingleNode('Perk/Set').GetAttribute('AttributeBleeding') -eq '0.2' -and
    $pickNode.SelectSingleNode('Perk').GetAttribute('ApplyTo') -eq 'Bot') ('Random rule differs: ' + $pickNode.OuterXml)
Check ((Build $catalog 'BuildRuleNode' (One $catalog.FightRules 'nohp')).OuterXml -eq '<NoHealthBar />') 'No-health-bar node differs.'
Check ((Build $catalog 'BuildRuleNode' (One $catalog.FightRules 'invert')).GetAttribute('ApplyTo') -eq 'Player') 'Invert-joystick target lost.'

# Rewards: forge-material currency drops.
$rewardNode = Build $catalog 'BuildRewardNode' (One $catalog.Rewards 'win')
$currencies = @($rewardNode.SelectNodes('Currency'))
Check ($rewardNode.GetAttribute('Bonus') -eq '17' -and $currencies.Count -eq 2 -and $currencies[0].GetAttribute('Name') -eq 'ForgeMaterial1' -and
    $currencies[0].GetAttribute('ExpectedValue') -eq '50' -and $currencies[0].GetAttribute('Drop') -eq '1' -and $currencies[0].GetAttribute('ShowReward') -eq '1' -and
    $currencies[1].GetAttribute('ShowReward') -eq '0') ('Reward node differs: ' + $rewardNode.OuterXml)

# Underworld pages, Power Mode visibility and map-button sprites through the shared policy.
[Eclipse.Modding.ModPolicies]::Content = $catalog
$zoneName = (One $catalog.Zones 'tier').LegacyName
Check ([Eclipse.Modding.ModPolicies]::IsRaidZone($zoneName) -and ![Eclipse.Modding.ModPolicies]::IsRaidZone((One $catalog.Zones 'story').LegacyName)) 'Underworld page classification differs.'
$mode = [Eclipse.Modding.ModPowerMode]::Always
foreach ($pair in @(@('boss_1','Normal'),@('boss_1_hard','Power'),@('gauntlet','Always'))) {
    Check ([Eclipse.Modding.ModPolicies]::TryUnderworldBattle((One $catalog.Battles $pair[0]).LegacyName, [ref]$mode) -and $mode.ToString() -eq $pair[1]) ('Power Mode differs: ' + $pair[0])
}
$icons = $null
Check ([Eclipse.Modding.ModPolicies]::TryBattleIcons((One $catalog.Battles 'boss_1').LegacyName, [ref]$icons) -and
    $icons.Base -eq 'fixture.warriors:sprites/sensei/boss_hermit_young' -and $icons.Active -eq $icons.Base -and $icons.Locked -eq '') 'Battle icons differ.'
Check (![Eclipse.Modding.ModPolicies]::TryBattleIcons((One $catalog.Battles 'boss_1_hard').LegacyName, [ref]$icons)) 'Battle without icons reported icons.'
[Eclipse.Modding.ModPolicies]::Content = $null

# Fingerprints: each new field is part of content identity; omitting them keeps old encodings.
$print = Fingerprint $catalog
foreach ($edit in @(@('expected=50','expected=51'), @('show=false',''), @('refresh="each_round"','refresh="each_fight"'),
                    @('DamageFactor=0.5','DamageFactor=0.6'), @('skeleton="SkeletonHeavy"','skeleton="Skeleton"'),
                    @('power_mode="power"','power_mode="normal"'), @('frames_off=240','frames_off=241'))) {
    $changed = $base.Replace($edit[0], $edit[1]).Replace(',}','}')
    Check ($print -ne (Fingerprint (Load-Lua $changed))) ('Fingerprint ignored: ' + $edit[0])
}

# Validation: typed contracts reject malformed or unsafe input.
$prefix = 'local z=sf2.zones.register{id="u",underworld=true}; local s=sf2.zones.register{id="s"}; local p=sf2.perks.get("core:perks/PERK_ITEM_SPECIAL_TIME_BOMB_WEAPON"); '
$cases = @(
    @('sf2.zones.register{id="x",start=true,underworld=true}', 'cannot be the story start zone'),
    @('sf2.battles.register{id="b",zone=s,type=sf2.battles.FINAL,power_mode="power"}', 'Power Mode visibility requires an Underworld zone'),
    @('sf2.battles.register{id="b",zone=z,type=sf2.battles.FINAL,power_mode="hard"}', 'must be "normal" or "power"'),
    @('sf2.battles.register{id="b",zone=z,type=sf2.battles.FINAL,icons={active=sf2.assets.sprite("sprites/sensei/boss_hermit_young")}}', "field 'base' is required"),
    @('sf2.battles.register{id="b",zone=z,type=sf2.battles.FINAL,icons={base="boss"}}', 'must be a sprite handle'),
    @('sf2.rewards.register{id="r",currencies={{currency="Gems",expected=5}}}', 'ForgeMaterial1, ForgeMaterial2 or ForgeMaterial3'),
    @('sf2.rewards.register{id="r",currencies={{currency="ForgeMaterial1",expected=0}}}', 'expected value must be finite'),
    @('sf2.rewards.register{id="r",currencies={{currency="ForgeMaterial1",expected=1},{currency="ForgeMaterial1",expected=2}}}', 'must be unique'),
    @('sf2.warriors.register{id="w",skeleton="Katana"}', 'recovered skeleton item'),
    @('sf2.warriors.register{id="w",perks={{perk=p,parameters={Aspect=1}}}}', 'other than the dedicated fields'),
    @('sf2.warriors.register{id="w",perks={{perk=p,parameters={DamageFactor=1/0}}}}', 'must be finite'),
    @('sf2.warriors.register_template{id="t",template=sf2.warriors.get_template("core:warrior-templates/missing")}', 'not registered'),
    @('local b=sf2.behaviors.register{id="beh",on_tick=function() end}; local r=sf2.rules.behavior{id="rb",behavior=b}; sf2.rules.group{id="g",rules={r}}', 'Behavior rules cannot be placed inside'),
    @('sf2.rules.group{id="g",rules={}}', '1..64 rule handles'),
    @('sf2.rules.random{id="g",refresh="hourly",rules={sf2.rules.no_health_bar{id="n"}}}', 'must be "each_round" or "each_fight"'),
    @('sf2.rules.random_area{id="a",image="../x",width=5,fade_in=1,frames_on=1,fade_out=1,frames_off=1}', 'plain texture name'),
    @('sf2.rules.random_area{id="a",image="x",width=5,fade_in=0,frames_on=1,fade_out=1,frames_off=1}', 'fades must be 1..36000'),
    @('sf2.rules.perk{id="pr",perk=p,parameters={Frames_quake="5"}}', 'map native parameter names to numbers')
)
foreach ($case in $cases) { Check (Fails ($prefix + $case[0]) $case[1]) ('Invalid input accepted or misreported: ' + $case[0]) }

# sf2.underworld: story.progression capability, typed arguments and the host seam.
$battle = 'local z=sf2.zones.register{id="u",underworld=true}; local b=sf2.battles.register{id="b",zone=z,type=sf2.battles.FINAL}; '
Check (Fails 'sf2.underworld.set_toggle_visible(false)' 'story.progression') 'Toggle visibility ignored its capability.'
Check (Fails ($battle + 'sf2.underworld.set_focus(b)') 'story.progression') 'Focus ignored its capability.'
$manifest = Join-Path $mod.RootPath 'mod.toml'
$original = Get-Content -Raw $manifest
Set-Content $manifest ($original.Replace('["content.register"]', '["content.register", "story.progression"]'))
$mod = [Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $fixture 'Mods')).Mods[0]
try {
    [Eclipse.Modding.ModUnderworldAccess]::Clear()
    Check (Fails 'sf2.underworld.set_toggle_visible(true)' 'unavailable in this host') 'Toggle without a host did not report it.'
    $script:toggles = [Collections.Generic.List[bool]]::new(); $script:focused = [Collections.Generic.List[string]]::new()
    [Eclipse.Modding.ModUnderworldAccess]::SetToggleVisible = [Func[bool,bool]]{ param($visible) $script:toggles.Add($visible); return $true }
    [Eclipse.Modding.ModUnderworldAccess]::SetFocus = [Func[Eclipse.Modding.DefinitionId,bool]]{ param($id) $script:focused.Add($id.ToString()); return $false }
    $null = Load-Lua ($battle + 'assert(sf2.underworld.set_toggle_visible(false)==true); assert(sf2.underworld.set_focus(b)==false)')
    Check (($script:toggles -join ',') -eq 'False' -and ($script:focused -join ',') -eq 'fixture.warriors:battles/b') 'Underworld host calls differ.'
    Check (Fails 'sf2.underworld.set_toggle_visible("yes")' 'requires a boolean') 'Non-boolean toggle accepted.'
    Check (Fails 'sf2.underworld.set_focus("fixture.warriors:battles/b")' 'sf2.underworld.set_focus') 'A string focus argument was accepted.'
} finally {
    [Eclipse.Modding.ModUnderworldAccess]::Clear()
    Set-Content $manifest $original
}
Write-Output "PASS: $script:checks Underworld API checks (templates, perk parameters, rule groups, random areas, currency drops, Power Mode, map-button sprites, fingerprints, validation, sf2.underworld capability/host seam). Production bindings and adapter projection; no Unity scene."
