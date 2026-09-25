# Includes the attack/input regression and uses its isolated Lua fixture/helpers.
. (Join-Path $PSScriptRoot 'TestMoveAttackAuthoring.ps1')
$manifest=Join-Path $package 'mod.toml'
(Get-Content $manifest -Raw).Replace('["content.register"]','["content.register", "content.patch"]') | Set-Content $manifest
$mod=[Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $fixture 'Mods')).Mods[0]
$null=New-Item -ItemType Directory -Force (Join-Path $package 'assets/animations')
Copy-Item (Join-Path $root 'Assets/Resources/gamedata/animations/binary/chinese_swords_super_slash_old.bytes') (Join-Path $package 'assets/animations/chinese.bytes')
$graphLua=@'
local data=require("content.chinese_swords_data")
local animation=sf2.assets.binary("animations/chinese")
for _,move in ipairs(data.item_lock_extensions) do
    sf2.moves.extend_item_lock {move=move,item_type="Weapon",source_subtype="Sai",subtype="ChineseSwords"}
end
sf2.moves.register {id="slash",animation=animation,conditions=data.conditions,intervals=data.intervals,
    locks=data.locks,transitions=data.transitions,align=data.align,direction=data.direction}
sf2.moves.register {id="preview",animation=animation,locks=data.preview.locks,align=data.preview.align,direction=data.direction}
'@
$graphCatalog=Load-Lua $graphLua
$graphDoc=Project $graphCatalog
$graphDoc.Save((Join-Path $fixture 'graph.xml'))
$slash=$graphDoc.SelectSingleNode('//Move[contains(@Name,"slash")]')
$preview=$graphDoc.SelectSingleNode('//Move[contains(@Name,"preview")]')
$expectedPreview=$archive.SelectSingleNode('//Move[@Name="ShopChineseSwordsSuperSlash"]')
foreach($section in @('Conditions','Intervals','Locks','Transitions','Align','SetDirection')) {
    Check ((Shape $slash[$section]) -ceq (Shape $expected[$section])) ('Archive graph mismatch: '+$section)
}
foreach($section in @('Locks','Align','SetDirection')) {Check ((Shape $preview[$section]) -ceq (Shape $expectedPreview[$section])) ('Preview graph mismatch: '+$section)}

# Invoke real native graph parsers without loading animation/rig resources.
$staticFlags=[Reflection.BindingFlags]'Static,NonPublic'
foreach($case in @(@('KFPHEGLHEMM','Transitions'),@('PLMHJKGIHLB','Align'))) {
    $method=[MovesParser].GetMethod($case[0],$staticFlags)
    $a=$method.Invoke($null,@($slash[$case[1]]));$b=$method.Invoke($null,@($expected[$case[1]]))
    Check (($a | ConvertTo-Json -Depth 9 -Compress) -ceq ($b | ConvertTo-Json -Depth 9 -Compress)) ('Native graph parse differs: '+$case[1])
}
$dirParser=[MovesParser].GetMethod('JOLJIHDPADK',[Reflection.BindingFlags]'Static,Public')
Check (($dirParser.Invoke($null,@($slash.SetDirection)) | ConvertTo-Json -Depth 9 -Compress) -ceq ($dirParser.Invoke($null,@($expected.SetDirection)) | ConvertTo-Json -Depth 9 -Compress)) 'Native direction differs.'
$alignParser=[MovesParser].GetMethod('PLMHJKGIHLB',$staticFlags)
Check (($alignParser.Invoke($null,@($preview.Align)) | ConvertTo-Json -Depth 9 -Compress) -ceq ($alignParser.Invoke($null,@($expectedPreview.Align)) | ConvertTo-Json -Depth 9 -Compress)) 'Native preview shift differs.'

[xml]$baseMoves=Get-Content -Raw (Join-Path $root 'Assets/vanillaXml/animations/moves.xml')
$live=[Collections.Generic.List[InfoAnimation]]::new()
$snapshots=@{}
function Native-Move([string]$name,[Xml.XmlNode]$locks) {
    $move=[InfoAnimation]::new();$move.Name=$name
    [ConditionsParser]::ParseInside($move.MoveData.Locks,$locks)
    return $move
}
foreach($extension in $graphCatalog.MoveItemLockExtensions) {
    $name=$extension.MoveName
    $native=Native-Move $name $baseMoves.SelectSingleNode('//Move[@Name="'+$name+'"]/Locks')
    $snapshots[$name]=@($native.MoveData.Locks)
    $live.Add($native)
}
Check ($live.Count -eq 10) 'Expected ten archived lock extensions.'
$nativeRuntime=[Eclipse.Modding.LegacyContentAdapter].Assembly.GetType('Eclipse.Modding.ExternalCombatContentRuntime')
$apply=$nativeRuntime.GetMethod('ApplyItemLockExtensions',$staticFlags)
function Apply-Extensions($moves,$entries) { return $apply.Invoke($null,[object[]]@($moves,$entries)) }
function Eligible($move,[string]$subtype,[string]$skeleton='Skeleton',[SceneTypes]$scene=[SceneTypes]::SceneFight) {
    $state=[ModelConditions]::new();$state.OJIAKDDCGLB=[Collections.Generic.List[ItemInfo]]::new();$state.IBBALIJOJMC=$scene
    foreach($entry in @(@('Weapon',$subtype),@('Skeleton',$skeleton))) {$item=[ItemInfo]::new($null);$item.Type=$entry[0];$item.SubType=$entry[1];$state.OJIAKDDCGLB.Add($item)}
    foreach($condition in $move.MoveData.Locks) { if(!$condition.IsEqual($state)) {return $false} }
    return $true
}
foreach($move in $live) {Check (!(Eligible $move 'ChineseSwords')) 'Base unexpectedly accepts Chinese swords.'}
$scope=Apply-Extensions $live $graphCatalog.MoveItemLockExtensions
foreach($move in $live) {
    $archived=Native-Move $move.Name $archive.SelectSingleNode('//Move[@Name="'+$move.Name+'"]/Locks')
    foreach($subtype in @('Sai','HermitSwords','ChineseSwords','Katana')) {
        foreach($skeleton in @('Skeleton','Titan')) {
            Check ((Eligible $move $subtype $skeleton) -eq (Eligible $archived $subtype $skeleton)) ('Lock truth table differs: '+$move.Name+'/'+$subtype+'/'+$skeleton)
        }
    }
    for($i=1;$i -lt $move.MoveData.Locks.Count;$i++) {Check ([object]::ReferenceEquals($move.MoveData.Locks[$i],$snapshots[$move.Name][$i])) 'Sibling lock replaced.'}
}
$left=$live | Where-Object Name -eq 'SaiStartStance-Left'
Check (!(Eligible $left 'ChineseSwords' 'Skeleton' ([SceneTypes]::SceneShopArmor))) 'Screen restriction lost.'
$scope.Dispose();$scope.Dispose()
foreach($move in $live) {
    for($i=0;$i -lt $move.MoveData.Locks.Count;$i++) {Check ([object]::ReferenceEquals($move.MoveData.Locks[$i],$snapshots[$move.Name][$i])) 'Rollback did not restore original condition identity.'}
}
# A later invalid target must not leave earlier native edits behind.
$broken=[Collections.Generic.List[Eclipse.Modding.MoveItemLockExtension]]::new()
$broken.Add($graphCatalog.MoveItemLockExtensions[0]);$broken.Add([Eclipse.Modding.MoveItemLockExtension]::new('missing','Weapon','Sai','ChineseSwords'))
$failure=$null;try {$null=Apply-Extensions $live $broken}catch{$failure=$_}
Check ($null -ne $failure) 'Missing live move accepted.'
Check ([object]::ReferenceEquals($live[0].MoveData.Locks[0],$snapshots[$live[0].Name][0])) 'Partial native failure changed a lock.'
foreach($lockText in @(
    '<Locks><Item Type="Weapon" SubType="Sai" Not="1"/></Locks>',
    '<Locks><Operator Type="And"><Item Type="Weapon" SubType="Sai"/></Operator></Locks>',
    '<Locks><Item Type="Weapon" SubType="Sai"/><Item Type="Weapon" SubType="Sai"/></Locks>',
    '<Locks><Operator Type="Or"><Item Type="Weapon" SubType="Sai"/><Item Type="Weapon" SubType="ChineseSwords"/></Operator></Locks>',
    '<Locks><Item Type="Weapon" SubType="Sai" Name="specific"/></Locks>'
)) {
    [xml]$doc=$lockText;$one=Native-Move 'test' $doc.DocumentElement
    $before=@($one.MoveData.Locks)
    $failure=$null
    try {$null=Apply-Extensions ([InfoAnimation[]]@($one)) ([Eclipse.Modding.MoveItemLockExtension[]]@([Eclipse.Modding.MoveItemLockExtension]::new('test','Weapon','Sai','ChineseSwords')))}catch{$failure=$_}
    Check ($null -ne $failure) 'Ambiguous/negative/named/pre-existing lock accepted.'
    Check ([object]::ReferenceEquals($before[0],$one.MoveData.Locks[0])) 'Rejected lock was mutated.'
}
[xml]$singleLock='<Locks><Item Type="Weapon" SubType="Sai"/></Locks>'
$singleMove=Native-Move 'test' $singleLock.DocumentElement
$chain=[Eclipse.Modding.MoveItemLockExtension[]]@(
    [Eclipse.Modding.MoveItemLockExtension]::new('test','Weapon','Sai','ChineseSwords'),
    [Eclipse.Modding.MoveItemLockExtension]::new('test','Weapon','Sai','OtherSwords'))
$scope=Apply-Extensions ([InfoAnimation[]]@($singleMove)) $chain
Check ((Eligible $singleMove 'ChineseSwords') -and (Eligible $singleMove 'OtherSwords') -and (Eligible $singleMove 'Sai')) 'Multiple extensions failed to compose.'
[xml]$siblingXml='<Screen Name="Fight"/>'
$sibling=Parse-Condition $siblingXml.DocumentElement;$singleMove.MoveData.Locks.Add($sibling)
$scope.Dispose();Check (!(Eligible $singleMove 'ChineseSwords')) 'Direct item lock did not restore.'
Check ([object]::ReferenceEquals($singleMove.MoveData.Locks[1],$sibling)) 'Teardown removed a later unrelated sibling.'
$chain[1]=[Eclipse.Modding.MoveItemLockExtension]::new('test','Weapon','ChineseSwords','OtherSwords')
$failure=$null;try {$null=Apply-Extensions ([InfoAnimation[]]@($singleMove)) $chain}catch{$failure=$_}
Check ($null -ne $failure -and !(Eligible $singleMove 'ChineseSwords')) 'Order-dependent source selector accepted or partially applied.'

$baseFingerprint=Fingerprint $graphCatalog
Check ($baseFingerprint -ceq (Fingerprint (Load-Lua $graphLua))) 'Graph reload fingerprint changed.'
Check ($baseFingerprint -cne (Fingerprint (Load-Lua $graphLua.Replace('source_subtype="Sai"','source_subtype="HermitSwords"')))) 'Extension selector omitted from fingerprint.'
$absoluteDoc=Project (Load-Lua $graphLua.Replace('local animation=','data.transitions[1].frame_shift=nil; data.transitions[1].first_frame=4' + "`n" + 'local animation='))
$absoluteTransitions=[MovesParser].GetMethod('KFPHEGLHEMM',$staticFlags).Invoke($null,@($absoluteDoc.SelectSingleNode('//Move[contains(@Name,"slash")]/Transitions')))
Check (!$absoluteTransitions[0].IsFrameShift -and $absoluteTransitions[0].FrameShift -eq 4) 'Absolute transition frame became a relative shift.'
$emptyGraph='sf2.moves.register_template {id="empty"}'
Check ((Fingerprint (Load-Lua $emptyGraph)) -ceq (Fingerprint (Load-Lua $emptyGraph.Replace('id="empty"','id="empty",locks={}')))) 'Empty graph changed existing content fingerprint.'
foreach($mutation in @('data.transitions[1].frame_shift=3','data.align.position.x=1','data.direction.to.node="NHeel_1"','data.locks[1].subtype="Sai"','data.transitions[1].conditions[1].animation="OtherMove"')) {
    $changed=Load-Lua $graphLua.Replace('local animation=',($mutation+"`n"+'local animation='))
    Check ($baseFingerprint -cne (Fingerprint $changed)) ('Graph field omitted from fingerprint: '+$mutation)
}
foreach($mutation in @(
    'data.transitions[1].first_frame=2', 'data.transitions[1].frame_shift=0.5',
    'data.transitions[1].frame_shift=100001', 'data.transitions[1].conditions={}',
    'data.align.axes={"X","X"}', 'data.align.axes={"W"}', 'data.align.pivot.node=nil',
    'data.align.pivot.x=1', 'data.align.position={floor="Me"}',
    'data.direction.to.player=nil', 'data.direction.to={animation="Enemy"}',
    'data.align.position.x=1/0', 'data.align.position.shift_z=1'
)) {
    $failure=$null;try {$null=Load-Lua $graphLua.Replace('local animation=',($mutation+"`n"+'local animation='))}catch{$failure=$_}
    Check ($null -ne $failure) ('Invalid graph accepted: '+$mutation)
}
$failure=$null
try {$null=Load-Lua 'sf2.moves.register_template {id="bad",transitions={{frame_shift=2,conditions={{stage="Fight"}}}}}'}catch{$failure=$_}
Check ($null -ne $failure) 'Template transition accepted although native parser does not inherit it.'
$duplicate='sf2.moves.extend_item_lock {move="SaiSpit",item_type="Weapon",source_subtype="Sai",subtype="ChineseSwords"}'
$failure=$null;try {$null=Load-Lua ($duplicate+"`n"+$duplicate)}catch{$failure=$_}
Check ($null -ne $failure) 'Duplicate extension accepted.'
$txCatalog=[Eclipse.Modding.ModContentCatalog]::new()
foreach($attempt in 1..2) {
    $tx=$txCatalog.BeginRegistration($mod);$failure=$null
    try {$tx.ExtendMoveItemLock('SaiSpit','Weapon','Sai','ChineseSwords');$tx.Commit()}catch{$failure=$_}finally{$tx.Dispose()}
    Check (($attempt -eq 1 -and $null -eq $failure) -or ($attempt -eq 2 -and $null -ne $failure)) 'Catalog conflict handling failed.'
}
Check ($txCatalog.MoveItemLockExtensions.Count -eq 1) 'Conflict leaked a registration.'
(Get-Content $manifest -Raw).Replace(', "content.patch"','') | Set-Content $manifest
$mod=[Eclipse.Modding.ModDiscovery]::DiscoverLoose((Join-Path $fixture 'Mods')).Mods[0]
$failure=$null;try {$null=Load-Lua $duplicate}catch{$failure=$_}
Check ($null -ne $failure -and $failure.ToString().Contains('content.patch')) 'Missing patch capability accepted.'
Write-Output "PASS $script:checks combined Lua/native attack and graph checks. Ten archived lock extensions, native graph parsing, conflicts and rollback; no live fight."
