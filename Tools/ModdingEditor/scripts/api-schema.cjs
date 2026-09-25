// Authored public type contracts. Names/constants and hover prose are checked
// against runtime bindings and the public wiki by generate.cjs.
const types = {}, functions = {}, aliases = {};
const E = name => `Eclipse.${name}`;
const H = name => E(`${name}Handle`);
const enumOf = (...values) => values.map(JSON.stringify).join('|');
const type = (name, fields, parent) => { types[name] = { fields, parent }; return E(name); };
const fn = (name, params, returns = 'nil', capability = 'content.register', options = {}) => {
    functions[`sf2.${name}`] = { params, returns, capability, ...options };
};
const reg = (name, shape, result, capability) => fn(name, { definition: E(shape) }, result ? H(result) : 'nil', capability);
const lookup = (name, result) => fn(name, { reference: 'string' }, H(result));
for (const name of ['Sprite','Model','Audio','Binary','Localization','Item','Price','Perk','Behavior','Zone','Battle','WarriorTemplate','Warrior','Reward','Fight','Rule','Quest','ItemSet','ForgeProfile','ForgeRecipe','Location','MoveTemplate','Move','Trigger','Tactic','Counter']) {
    type(`${name}Handle`, { [`private __eclipse${name}`]: 'true' });
}
for (const name of ['Weapon','Armor','Helm','Ranged','Magic','Consumable','Free','Seal']) type(`${name}Handle`, {}, 'ItemHandle');
type('EnchantmentHandle', { 'private __eclipseEnchantment': 'true' });
const primitive = 'number|boolean|string';
type('FieldSchema', { type: enumOf('number','integer','boolean','string'), 'required?': ['boolean','Defaults to true. Required saved fields need defaults.'], 'default?': primitive });
const schema = `table<string,${E('FieldSchema')}|${enumOf('number','integer','boolean','string')}>`;
const values = 'table<string,any>'; // Runtime schemas determine these keys/types; do not invent static types.
const migrations = `table<integer,fun(old:${values}):${values}|nil>`;
type('StateDefinition', { version:'integer', 'fields?':schema, 'aliases?':'table<string,string>', 'tombstones?':'string[]', 'migrations?':migrations });
type('BehaviorState', { 'fields?':schema, 'lifetime?':enumOf('round','fight','saved'), 'version?':'integer', 'migrations?':migrations });
type('BehaviorSelf', { params:values, state:values });
type('CombatEvent', { type:'string', 'round?':'integer', 'damage?':'number', 'health_before?':'number', 'health_after?':'number', 'blocked?':'boolean', 'critical?':'boolean', 'won?':'boolean' });
type('DamageEvent',{damage:'number',health_before:'number',health_after:'number',blocked:'boolean',critical:'boolean',round:'integer'},'CombatEvent');
type('IncomingDamageEvent',{damage:'number',blocked:'boolean',critical:'boolean'},'CombatEvent');
type('HitPhaseEvent',{damage:'number',blocked:'boolean',critical:'boolean',target:enumOf('self','opponent'),weapon:'boolean',unarmed:'boolean',ranged:'boolean',magic:'boolean'},'CombatEvent');
type('ComboEvent',{combo:'integer',last_combo:'integer'},'CombatEvent');
type('StyleEvent',{style_rank:'integer',style_name:'string',style_gain:'number',is_hit:'boolean'},'CombatEvent');
type('AnimationLifecycleEvent',{animation_name:'string',target:enumOf('self','opponent','other'),frame:'integer'},'CombatEvent');
type('TickEvent',{frame:'integer',seconds:'number',delta_frames:'integer',delta_seconds:'number'},'CombatEvent');
type('FightEndEvent',{won:'boolean',player_result:enumOf('win','loss','surrender','timeout')},'CombatEvent');
const callbacks = ['on_animation_start','on_animation_end','on_fight_begin','on_round_begin','on_tick','on_damage_resolving','on_damage_dealing','on_hit_post_crit','on_post_hit','on_damage_received','on_damage_dealt','on_block','on_critical','on_combo_changed','on_style_changed','on_round_end','on_fight_end'];
for (const stateful of [false,true]) {
    const fields = { id:'string', 'parameters?':schema, ...(stateful ? { state:E('BehaviorState') } : { 'state?':'nil' }) };
    for (const name of callbacks) {
        const event=['on_animation_start','on_animation_end'].includes(name)?'AnimationLifecycleEvent':name==='on_tick'?'TickEvent':name==='on_combo_changed'?'ComboEvent':name==='on_style_changed'?'StyleEvent':['on_hit_post_crit','on_post_hit'].includes(name)?'HitPhaseEvent':['on_damage_resolving','on_damage_dealing'].includes(name)?'IncomingDamageEvent':name==='on_fight_end'?'FightEndEvent':['on_damage_received','on_damage_dealt','on_block','on_critical'].includes(name)?'DamageEvent':'CombatEvent';
        fields[`${name}?`] = `fun(${stateful ? 'self:'+E('BehaviorSelf') : 'parameters:'+values}, fighter:${E(name==='on_damage_resolving'?'ResolvingFighter':['on_damage_dealing','on_post_hit'].includes(name)?'OutgoingFighter':'Fighter')}, event:${E(event)})`;
    }
    type(stateful ? 'StatefulBehavior' : 'BehaviorDefinition', fields);
}
type('Fighter', { 'health?':'number', 'side?':'string', 'source?':'string', 'rule_id?':'string', 'opponent?':E('Opponent') });
type('Opponent', {'health?':'number'});
type('CombatPosition', {x:'number',y:'number',z:'number'});
type('AnimationIntervalSnapshot',{name:'string',type:enumOf('none','unstable','uninterrupt','self_uninterrupt','attack','block','invulnerable','invisible')});
type('AnimationSnapshot',{name:'string',type:enumOf('none','move','attack'),facing:'integer',intervals:E('AnimationIntervalSnapshot')+'[]'});
type('FighterSnapshot', {health:'number',max_health:'number',health_bars:'integer',position:E('CombatPosition'),'animation?':E('AnimationSnapshot')});
type('CombatSnapshot', {self:E('FighterSnapshot'),'opponent?':E('FighterSnapshot'),frame:'integer',seconds:'number',round_active:'boolean'});
type('ResolvingFighter',{},'Fighter');
type('OutgoingFighter',{},'Fighter');
const fighterMethods = {
    change_form:{params:{character:H('Warrior')},returns:E('FormRequest'),capability:'combat.transform'},
    snapshot:{params:{},returns:`${E('CombatSnapshot')}|nil`,capability:null},
    change_health:{params:{amount:'number'},capability:'combat.change_life'},
    add_magic_charge:{params:{amount:'number'},capability:'combat.magic_charge'},
    scale_outgoing_damage:{params:{multiplier:'number'},capability:'combat.modify_outgoing_hit'},
    add_outgoing_damage:{params:{amount:'number'},capability:'combat.modify_outgoing_hit'},
    scale_incoming_damage:{params:{multiplier:'number'},capability:'combat.modify_hit'},
    add_damage_shield:{params:{key:'string',fraction:'number',frames:'integer'},capability:'combat.effects'},
    remove_damage_shield:{params:{key:'string'},capability:'combat.effects'},
    set_control_blocked:{params:{control:enumOf('punch','kick','ranged','magic','raid_charge'),blocked:'boolean'},capability:'combat.effects'},
    set_flag:{params:{key:'string'},returns:'string',capability:'combat.effects'},
    clear_flag:{params:{key:'string'},capability:'combat.effects'},
    has_flag:{params:{key:'string'},returns:'boolean',capability:'combat.effects'},
    show_status_icon:{params:{key:'string',sprite:H('Sprite'),frames:'integer','stacks?':'integer'},capability:'combat.effects'},
    clear_status_icon:{params:{key:'string'},capability:'combat.effects'},
};
type('FormRequest',{status:enumOf('queued','applied','failed'),'error?':'string'});
const equipment = { id:'string', display_name:H('Localization'), icon:H('Sprite'), model:H('Model') };
const initialStatFields = {Weapon:['weapon_damage'],Armor:['body_defense','head_defense','unarmed_damage'],Helm:['head_defense'],Ranged:['ranged_damage','weapon_damage'],Magic:['magic_damage']};
for (const name of ['Weapon','Armor','Helm','Ranged','Magic']) {
    type(`${name}InitialStats`, Object.fromEntries(initialStatFields[name].map(field=>[field+'?',['integer','0-1000000. Unspecified stats remain absent in an explicit snapshot.']])));
    type(`${name}Definition`, { ...equipment, 'initial_stats?':[E(`${name}InitialStats`),'Exact initial snapshot. Omit for normal level-derived stats; {} leaves all initial stats absent. Native upgrades remain unchanged.'], ...(name === 'Weapon' ? {'subtype?':['string','Defaults to Katana. Match the model and move family.'], 'tactic_subtype?':['string','Optional AI table group, defaults to subtype. 1-128 ASCII letters, digits or underscores.']} : ['Ranged','Magic'].includes(name) ? {subtype:'string'} : {}) });
    reg(`items.register_${name.toLowerCase()}`,`${name}Definition`,name);
}
type('NonEquipmentDefinition', {id:'string',display_name:H('Localization'),'icon?':H('Sprite'),'model?':H('Model'),'subtype?':'string','pack_label?':'string','silent_receive?':'boolean','spend_after_use?':'boolean'});
for (const name of ['Consumable','Free','Seal']) reg(`items.register_${name.toLowerCase()}`,'NonEquipmentDefinition',name);
lookup('items.get','Item');
type('ItemAlias',{from:'string',to:H('Item')}); reg('items.alias','ItemAlias');
type('IdDefinition',{id:'string'}); reg('items.tombstone','IdDefinition');
type('ShopListing',{section:enumOf('weapons','armor','helmets','ranged','magic'),item:H('Item'),level:['integer','Weapon: 1-52; armor/helm: 2-52; ranged/magic: 6-52.'],price:H('Price')});
fn('shop.addItem',{definition:E('ShopListing')},'string');
type('Availability',{item:H('Item'),'visibility?':enumOf('inherit','force_visible','force_hidden'),'required_group?':['string','Player-group prerequisite; also the native unlock-notification pack label for mod-owned equipment. Core labels stay unchanged.'],'minimum_level?':'integer'}); reg('shop.set_availability','Availability',null,'content.patch');
type('ShopPricePatch',{item:H('Item'),price:H('Price'),'secondary_price?':[H('Price'),'Optional positive price in the other currency.']}); reg('shop.set_price','ShopPricePatch',null,'content.patch');
for (const name of ['coins','gems']) fn(`price.${name}`,{amount:'integer'},H('Price'),null,{bounds:{amount:[0,2147483647]}});
for (const name of ['sprite','model','audio','binary']) fn(`assets.${name}`,{reference:'string'},H(name[0].toUpperCase()+name.slice(1)),null,{referenceKind:name});
fn('assets.qualify',{reference:'string'},'string',null);
fn('assets.exists',{reference:'string'},'boolean',null);
type('AssetReplacement',{target:'string',replacement:'string'}); reg('assets.replace','AssetReplacement',null,'assets.replace');
fn('localization.key',{key:'string'},H('Localization'),'content.register',{referenceKind:'localization'});
fn('localization.text',{key:H('Localization'),'language?':'string'},'string',null);
type('LocalizationDefinition',{id:'string',language:'string',value:'string'});reg('localization.register','LocalizationDefinition','Localization');
type('LocalizationPatch',{target:'string',language:'string',value:'string'});reg('localization.patch','LocalizationPatch',null,'content.patch');
reg('state.register','StateDefinition',null,'state.write');
fn('state.get',{name:'string'},`${primitive}|nil`,'state.read');
fn('state.set',{values},'nil','state.write');
fn('state.unset',{name:'string'},'nil','state.write');
fn('random.integer',{field:'string',minimum:'integer',maximum:'integer'},'integer',['state.read','state.write']);
fn('random.number',{field:'string'},'number',['state.read','state.write']);
reg('behaviors.register','BehaviorDefinition','Behavior');
functions['sf2.behaviors.register'].overload = `fun(definition:${E('StatefulBehavior')}):${H('Behavior')}`;
lookup('perks.get','Perk');
type('PerkUpgrade',{level:'integer','description?':H('Localization'),'parameters?':values});
type('PerkDefinition',{id:'string',display_name:H('Localization'),description:H('Localization'),'icon?':H('Sprite'),behavior:H('Behavior'),kind:enumOf('single','combo'),'parameters?':values,'upgrades?':E('PerkUpgrade')+'[]','initial_upgrade?':'integer'});
type('TemplatePerk',{id:'string',display_name:H('Localization'),description:H('Localization'),'icon?':H('Sprite'),template:H('Perk'),'parameters?':values,'upgrades?':E('PerkUpgrade')+'[]','initial_upgrade?':'integer'});
fn('perks.register',{definition:`${E('PerkDefinition')}|${E('TemplatePerk')}`},H('Perk'));
const equipmentKinds=enumOf('weapon','armor','helm','ranged','magic');
type('EnchantmentDefinition',{id:'string',recipe:enumOf('simple','medium','complex'),item_types:`(${equipmentKinds})[]`,behavior:H('Behavior'),display_name:H('Localization'),description:H('Localization'),'icon?':H('Sprite'),'parameters?':values});
type('LegacyEnchantment',{id:'string',recipe:enumOf('simple','medium','complex'),item_types:`(${equipmentKinds})[]`,perk:H('Perk')});
fn('enchantments.register',{definition:`${E('EnchantmentDefinition')}|${E('LegacyEnchantment')}`},H('Enchantment'));
type('BattleIcons',{base:[H('Sprite'),'Unlocked, not selected.'],'active?':[H('Sprite'),'Selected state; supply it for complete art.'],'locked?':[H('Sprite'),'Locked state; native lock art when omitted.'],'locked_active?':[H('Sprite'),'Selected while locked.']});
type('ZoneDefinition',{id:'string','file?':'string','start?':'boolean','underworld?':['boolean','Place the page on the Underworld (raid) map; cannot be the start zone.']});reg('zones.register','ZoneDefinition','Zone');lookup('zones.get','Zone');
type('BattleDefinition',{id:'string',zone:H('Zone'),type:'string','x?':'integer','y?':'integer',...Object.fromEntries(['alias','title','description','icon','icon_atlas','eclipse_toggle_name','location','reward_image'].map(k=>[k+'?','string'])),'music?':[`${H('Audio')}|string`,'Audio handle for music the mod ships, or a native track name.'],'preview?':[`${H('Sprite')}|string`,'Sprite handle for mod-supplied art, or an existing native preview name.'],'show_resistance?':'boolean','power_mode?':[enumOf('normal','power'),'Underworld pages only: shown only while Power Mode is off (normal) or on (power). Omit to always show.'],'icons?':[E('BattleIcons'),'Mod-supplied map-button sprites.']});reg('battles.register','BattleDefinition','Battle');
type('AttributeAlignment',{factor:'number',shift:'number','priority?':'integer','mode?':enumOf('all','normal','eclipse')});
fn('battles.set_locked',{battle:H('Battle'),locked:'boolean'},'boolean','story.progression');
fn('battles.reveal',{battle:H('Battle'),locked:'boolean'},'boolean','story.progression');
fn('battles.focus',{battle:H('Battle')},'boolean','story.progression');
type('WarriorPerk',{perk:H('Perk'),'aspect?':['number','Core perks only; finite 0..2147483647. Omit to inherit.'],'chance_factor?':['number','Core perks only; finite 0..10000 native multiplier, not a probability. Omit to inherit.'],'chance?':['number','Core perks only; finite probability 0..1. Omit to inherit.'],'frames?':['integer','Core perks only; 0..2147483647 native frame duration. Omit to inherit.'],'parameters?':['table<string,number>','Other native perk parameters: up to 32 names to finite numbers; dedicated names such as Aspect/Chance/Frames are rejected.']});
type('WarriorDefinition',{id:'string','template?':H('WarriorTemplate'),...Object.fromEntries(['first_name','last_name','voice','group'].map(k=>[k+'?','string'])),'avatar?':[`${H('Sprite')}|string`,'Sprite handle for mod-supplied art, or an existing native portrait name.'],'level?':'integer','tactic?':`${H('Tactic')}|string`,'random?':'integer','items?':H('Item')+'[]','perks?':`(${H('Perk')}|${E('WarriorPerk')})[]`,'attributes?':'table<string,number>','attribute_alignments?':[E('AttributeAlignment')+'[]','Rows append once to inherited template alignments. Omit to keep the parent rows.'],'body_model?':H('Model'),'skin_models?':H('Model')+'[]','health_bars?':['integer','0 inherits the template; 1-10000 is the total number of health bars.'],'skeleton?':['string','Recovered body item such as Skeleton or SkeletonHeavy, added to the loadout.']});
{ const t={...types['WarriorDefinition'].fields};for(const k of ['group?','random?','body_model?','skin_models?']) delete t[k];type('WarriorTemplateDefinition',t); }
lookup('warriors.get_template','WarriorTemplate');reg('warriors.register','WarriorDefinition','Warrior');reg('warriors.register_template','WarriorTemplateDefinition','WarriorTemplate');
type('RewardGrantContext',{player_level:'integer',item_id:'string'});
type('RewardGrantEnchantment',{perk:H('Perk'),'aspect?':'number'});
type('RewardGrantConfiguration',{'level?':'integer','enchantments?':E('RewardGrantEnchantment')+'[]'});
const rewardConfigure=`fun(context:${E('RewardGrantContext')}):${E('RewardGrantConfiguration')}`;
type('ItemGrant',{item:H('Item'),'upgrade?':'integer','configure?':rewardConfigure});type('RewardCandidate',{item:H('Item'),'upgrade?':'integer','weight?':'number','configure?':rewardConfigure});type('RewardChoice',{items:E('RewardCandidate')+'[]'});
type('RewardCurrency',{currency:enumOf('ForgeMaterial1','ForgeMaterial2','ForgeMaterial3'),expected:['number','Average amount rolled by the game; finite, >0 and <=10000000.'],'show?':['boolean','List the drop on the result screen; default true.']});
type('RewardDefinition',{id:'string','items?':E('ItemGrant')+'[]','choices?':E('RewardChoice')+'[]','gems?':'integer','experience?':['integer','0..1000000 experience points; default 0.'],'prize_base?':['number','Finite 0..1000000 native performance-bonus base; omitted keeps fallback. Not a fixed coin award.'],'currencies?':[E('RewardCurrency')+'[]','Up to 16 forge-material drops, each currency at most once.']});reg('rewards.register','RewardDefinition','Reward');
type('FightDefinition',{id:'string',battle:H('Battle'),'warriors?':H('Warrior')+'[]','rules?':H('Rule')+'[]','rewards?':[H('Reward')+'[]','First slot is the zero-win result; second slot is one win.'],...Object.fromEntries(['rounds','round_time','replays','replay_interval','power'].map(k=>[k+'?','integer'])),...Object.fromEntries(['location','description','reward_image'].map(k=>[k+'?','string'])),'music?':[`${H('Audio')}|string`,'Audio handle for music the mod ships, or a native track name.'],'evaluated_rating?':'number','health_recovery?':'number','locked?':'boolean'});reg('fights.register','FightDefinition','Fight');
type('RewardDropPatch',{wins:'integer',reward:H('Reward'),'mode?':enumOf('all','normal','eclipse'),'min_level?':'integer','max_level?':'integer'});
type('FightPatch',{target:'string','description?':'string','rounds?':'integer','round_time?':'integer','location?':'string','music?':'string','warriors?':H('Warrior')+'[]','reward_drops?':E('RewardDropPatch')+'[]','rules?':H('Rule')+'[]','append_rules?':H('Rule')+'[]'});reg('fights.patch','FightPatch',null,'content.patch');
const rule={id:'string','target?':enumOf('player','opponent','all'),'mode?':enumOf('normal','eclipse','all'),'rounds?':'integer[]'};
type('HotGroundNode',{name:'string',axis:enumOf('X','Y'),'min?':'number','max?':'number'});
for (const [name,extra] of Object.entries({no_perks:{'name?':'string'},require_item:{item:H('Item'),'minimum_level?':'integer'},equip_item:{item:H('Item'),'minimum_level?':'integer'},avatar:{name:'string'},name:{name:'string'},no_button:{name:'string'},perk:{perk:H('Perk'),'aspect?':'number','parameters?':'table<string,number>'},no_health_bar:{},invert_joystick:{},random_area:{image:'string','icon?':'string',width:'number',fade_in:'integer',frames_on:'integer',fade_out:'integer',frames_off:'integer'},light_in_the_darkness:{radius:['number','Normalized light radius in (0, 1].'],'shape?':['number','0 is square, 1 is circular; default 1.']},behavior:{behavior:H('Behavior'),'parameters?':values},recharge_magic_each_round:{},attributes:{values:'table<string,number>'},hot_ground:{frames:'integer',nodes:E('HotGroundNode')+'[]','animations?':'string[]'},ring_out:{node:'string',axis:enumOf('X','Y'),min:'number',max:'number'},regeneration:{rate:'number',frames_after_hit:'integer'},remove_interval:{type:enumOf('Attack','Block','Invulnerable','SelfUninterrupt','Uninterrupt','Unstable')}})) {
    const fields={...rule,...extra};if(name==='require_item') delete fields['target?'];
    if(name==='hot_ground') fields['target?']=enumOf('player','opponent');
    const shape='Rule_'+name;type(shape,fields);reg('rules.'+name,shape,'Rule');
}
type('Rule_no_animation',{id:'string',name:'string','mode?':enumOf('normal','eclipse','all'),'rounds?':'integer[]'});reg('rules.no_animation','Rule_no_animation','Rule');
type('Rule_group',{id:'string',rules:[H('Rule')+'[]','1..64 distinct rule handles; behavior rules are rejected.'],'description?':H('Localization'),'mode?':enumOf('normal','eclipse','all'),'rounds?':'integer[]'});reg('rules.group','Rule_group','Rule');
type('Rule_random',{id:'string',rules:[H('Rule')+'[]','1..64 distinct rule handles; behavior rules are rejected.'],'refresh?':[enumOf('each_fight','each_round'),'Default each_fight.'],'no_doubles?':'boolean','mode?':enumOf('normal','eclipse','all'),'rounds?':'integer[]'});reg('rules.random','Rule_random','Rule');
fn('underworld.set_toggle_visible',{visible:'boolean'},'boolean','story.progression');
fn('underworld.set_focus',{battle:H('Battle')},'boolean','story.progression');
const questEvents=['session','activate','fight_enter','fight_end','raid_fight_enter','raid_fight_end','raid_enter','raid_end','reset_mode','raid_map_enter','raid_floor_changed','show_raid_loot','level_up','got_item','set_item_acquired','purchase','delivery','timer_end','enchantment','activate_perk','deactivate_perk','dialog','map_button','scene_loaded','shop_enter'];
type('VariableOperand',{kind:'"variable"',name:'string'});type('EventOperand',{kind:enumOf('event_fight','fight_result','current_battle')});type('FightOperand',{kind:enumOf('fight_wins','fight_id'),fight:H('Fight')});
const operand=`${primitive}|${E('VariableOperand')}|${E('EventOperand')}|${E('FightOperand')}`;
type('Comparison',{op:enumOf('eq','gt','gte','lt','lte'),left:operand,right:operand,'not?':'boolean'});
const condition=`${E('Comparison')}|${E('ConditionGroup')}`;
type('ConditionGroup',{op:enumOf('all','any'),conditions:`(${condition})[]`,'not?':'boolean'});
type('DialogLine',{text:'string','button?':'string','frames?':'integer'});
const actionNames=['show_battle','toggle_battle','map_focus','fight','current_fight','eclipse','update_eclipse_battles','give_item','set_variable','dialog','story'];
const actionUnion=actionNames.map(n=>E('Action_'+n)).join('|');
type('DialogButton',{text:'string','color?':'string',actions:`(${actionUnion})[]`});
for (const [name,fields] of Object.entries({show_battle:{battle:H('Battle'),'locked?':'boolean'},toggle_battle:{battle:H('Battle'),'visible?':'boolean'},map_focus:{battle:H('Battle')},fight:{fight:H('Fight')},current_fight:{},eclipse:{'enabled?':'boolean'},update_eclipse_battles:{},give_item:{item:H('Item')},set_variable:{name:'string',value:'string'},dialog:{'title?':'string','image?':'string',lines:`(string|${E('DialogLine')})[]`,'button?':E('DialogButton')},story:{lines:`(string|${E('DialogLine')})[]`}})) type('Action_'+name,{type:JSON.stringify(name),...fields});
type('QuestDefinition',{id:'string','priority?':'integer','unresumable?':'boolean','allow_doubles?':'boolean','place?':enumOf('map','fight','dojo'),'groups?':'string[]','marks?':'string[]',events:`(${enumOf(...questEvents)})[]`,'conditions?':`(${condition})[]`,actions:`(${actionUnion})[]`});reg('quests.register','QuestDefinition','Quest');
type('SetMember',{item:H('Item'),'scale?':'number','rotate?':'number','x?':'number','y?':'number','icons_y?':'number'});type('ItemSet',{id:'string',title:H('Localization'),text:H('Localization'),brief:H('Localization'),members:E('SetMember')+'[]'});reg('itemsets.register','ItemSet','ItemSet');
type('PerkChoice',{perk:H('Perk'),'action?':enumOf('unlock','upgrade')});type('PerkBranch',{level:'integer',entries:E('PerkChoice')+'[]'});reg('progression.replace_perk_branch','PerkBranch',null,'content.patch');
lookup('forge.profile','ForgeProfile');
type('InnatePerk',{perk:H('Perk'),'parameters?':'table<string, number>'});type('ItemInnatePerks',{item:H('Item'),entries:E('InnatePerk')+'[]'});reg('items.set_innate_perks','ItemInnatePerks',null,'content.patch');
type('ItemTacticSubtype',{item:H('Item'),group:['string','Native AI table group; empty selects physical subtype fallback.']});reg('items.set_tactic_subtype','ItemTacticSubtype',null,'content.patch');
type('ItemCombatSubtype',{item:H('Item'),subtype:['string','Case-sensitive native combat family; requires matching moves/projectile support.']});reg('items.set_subtype','ItemCombatSubtype',null,'content.patch');
type('ItemInitialProfileStats',Object.fromEntries(['weapon_damage','body_defense','head_defense','unarmed_damage','ranged_damage','magic_damage'].map(field=>[field+'?',['integer','0-1000000; only fields valid for the item category are accepted.']])));
type('ItemPresentation',{item:H('Item'),'icon?':[H('Sprite'),'Optional replacement shop icon.'],'model?':[H('Model'),'Optional replacement fighter model. At least one of icon or model is required.']});reg('items.set_presentation','ItemPresentation',null,'content.patch');
type('ItemInitialProfile',{item:H('Item'),level:['integer','Starting equipment level, 1-52.'],upgrade_level:['integer','Starting upgrade tier, 0-5200.'],initial_stats:[E('ItemInitialProfileStats'),'Required complete initial stat snapshot; omitted fields remain absent.'],'upgrade_template?':[enumOf(...['Weapon','Armor','Helm','Ranged','Magic'].flatMap(category=>[category+'_Bonus','Paid_'+category+'_Bonus'])),'Existing native template matching the equipment category; omitted keeps the current template.'],'legacy_paid_item?':[enumOf('none','paid','super_paid'),'Optional legacy PaidItem metadata. Omitted keeps the current marker; none clears it. Does not change price or currency.'],'clear_local_upgrades?':['boolean','Optional; remove the item’s own upgrade rows so its shared template supplies progression.']});reg('items.set_initial_profile','ItemInitialProfile',null,'content.patch');
type('DefaultEnchantment',{perk:H('Perk'),'aspect?':'integer'});type('ItemDefaultEnchantments',{item:H('Item'),entries:E('DefaultEnchantment')+'[]'});reg('items.set_default_enchantments','ItemDefaultEnchantments',null,'content.patch');
type('ForgeDeviation',{profile:H('ForgeProfile'),equipment:equipmentKinds,minimum:'integer',maximum:'integer'});reg('forge.override_deviation','ForgeDeviation',null,'content.patch');
type('ForgeCandidateExclusion',{profile:H('ForgeProfile'),perk:H('Perk'),equipment:equipmentKinds});reg('forge.exclude_candidate','ForgeCandidateExclusion',null,'content.patch');
type('ForgeItem',{equipment:equipmentKinds,'enchantments?':'integer','bar_scale?':'string','min_deviation?':'integer','max_deviation?':'integer','random_aspect?':'boolean'});type('ForgeCandidate',{perk:H('Perk'),equipment:equipmentKinds,'min_level?':'integer','max_level?':'integer'});type('ForgeRecipe',{id:'string','alias?':'string',economic_profile:H('ForgeProfile'),items:E('ForgeItem')+'[]',candidates:E('ForgeCandidate')+'[]'});reg('forge.register_recipe','ForgeRecipe','ForgeRecipe');
type('Fonts',{content:'string',title:'string',button:'string','size_scale?':'number','line_spacing?':'number','custom_line_spacing_scale?':'number'});type('LocaleDefinition',{id:'string',name:'string',locale:'string','alias?':'string','is_asian?':'boolean','file_icon?':'string','file_icon_selected?':'string','loader_image?':'string','preloader_image?':'string','fonts?':E('Fonts')});fn('locales.register',{definition:E('LocaleDefinition')},'string');
type('LocationCurvePoint',{period:'number',value:'number','ease?':'number'});type('LocationCurve',{'offset?':'number',points:E('LocationCurvePoint')+'[]'});
type('ProfilePerkSnapshot',{learned:'boolean','upgrade?':'integer'});fn('profile.perk',{perk:H('Perk')+'|string'},E('ProfilePerkSnapshot'),'profile.read');
type('ProfileItemSnapshot',{'type?':'string','subtype?':'string',present:'boolean',owned:'boolean',count:'integer',equipped:'boolean','upgrade?':'integer'});fn('profile.level',{},'integer','profile.read');fn('profile.item',{item:H('Item')+'|string'},E('ProfileItemSnapshot'),'profile.read');
type('ProfileFightSnapshot',{present:'boolean',wins:'integer',losses:'integer'});fn('profile.fight',{fight:H('Fight')+'|string'},E('ProfileFightSnapshot'),'profile.read');
fn('profile.set_eclipse_mode',{enabled:'boolean'},'boolean','story.progression');
type('BattleEquipmentSnapshot',{'item?':'string','type?':'string','subtype?':'string'});
type('StorySubscription',{'private __eclipseStorySubscription':'true'});type('StoryEvent',{kind:'"purchase"|"enchantment"|"level_up"|"scene_enter"|"item_acquired"|"battle_result"','fight?':'string','outcome?':'"win"|"loss"|"surrender"|"raid_timeout"|"raid_round_timeout"','eclipse?':'boolean','equipment?':E('BattleEquipmentSnapshot')+'[]','item?':'string','recipe?':'string','previous_count?':'integer','count?':'integer','previous_level?':'integer','level?':'integer','scene?':'"map"|"shop"|"profile"|"dojo"|"fight"'});
type('FightEntryRequest', {'private __eclipseFightEntry':'true',fight:'string'});
fn('story.before_fight',{fight:H('Fight'),on_before_fight:`fun(request:${E('FightEntryRequest')}):boolean|nil`},'nil','story.progression');
fn('story.resume_fight',{request:E('FightEntryRequest')},'boolean','story.progression');
fn('story.cancel_fight',{request:E('FightEntryRequest')},'nil','story.progression');
fn('story.fight_pending',{request:E('FightEntryRequest')},'boolean','story.progression');
fn('story.on',{event:'"purchase"|"enchantment"|"level_up"|"scene_enter"|"item_acquired"|"battle_result"',callback:'fun(event: Eclipse.StoryEvent)'},E('StorySubscription'),'story.events');
fn('story.off',{subscription:E('StorySubscription')},'nil','story.events');
fn('story.is_active',{subscription:E('StorySubscription')},'boolean','story.events');
fn('scenes.open',{destination:'"map"|"shop"|"profile"|"dojo"'},'boolean','presentation.navigate');
type('LocationImage',{sprite:H('Sprite'),'x?':'number','y?':'number','width?':'number','height?':'number','opaque?':'boolean','flip_x?':'boolean','flip_y?':'boolean','mask?':'boolean',...Object.fromEntries(['motion_x','motion_y','rotation','opacity'].map(k=>[k+'?',E('LocationCurve')]))});type('FighterPositions',{player_x:'number',player_y:'number',enemy_x:'number',enemy_y:'number'});type('LocationLayer',{'type?':'integer','factor?':'number','scaling?':'boolean','images?':E('LocationImage')+'[]','fighters?':E('FighterPositions')});type('LocationDefinition',{id:'string','color?':'string',...Object.fromEntries(['wall','floor','position_y','width','height','min_width','friction_force','grid_size'].map(k=>[k+'?','number'])),'music?':H('Audio'),'music_choices?':H('Audio')+'[]','dojo?':'boolean',layers:E('LocationLayer')+'[]'});reg('locations.register','LocationDefinition','Location');fn('locations.name',{location:H('Location')},'string',null);fn('locations.select_dojo',{location:H('Location')},'nil','presentation.dojo');fn('locations.reset_dojo',{},'nil','presentation.dojo');fn('locations.selected_dojo',{},'string|nil','presentation.dojo');
const moveEvent=enumOf('animation_end','animation_start','interval_end','interval_start','hit','strike','every_frame','birth','round_stage_start','mod_expires','key_pressed');
type('MoveEvent',{type:moveEvent,'name?':'string','player?':'string'});
const moveCondition=`${E('MovePerkCondition')}|${E('MoveNamedCondition')}|${E('MoveConditionGroup')}|${E('MoveCharacterCondition')}|${E('MoveKeysCondition')}|${E('MoveStageCondition')}|${E('MoveScreenCondition')}|${E('MoveModCondition')}|${E('MoveActorCondition')}|${E('MoveBulletsCondition')}|${E('MoveDistanceCondition')}`;
type('MoveDistanceCondition',{type:'"distance"',axis:enumOf('X','Y','Full'),from:E('MovePoint'),to:E('MovePoint'),'minimum?':'number','maximum?':'number','not?':'boolean'});
type('MoveActorCondition',{type:'"actor_name"',name:'string','player?':enumOf('Me','Enemy','Parent','Child','EnemyChild','Both'),'not?':'boolean'});
type('MoveBulletsCondition',{type:'"bullets"',bullet_type:enumOf('MagicBullet','RaidChargeBullet'),'minimum?':'integer','maximum?':'integer','player?':enumOf('Me','Enemy','Parent','Child','EnemyChild','Both'),'not?':'boolean'});
type('MoveVelocity',{'x?':'number','y?':'number','z?':'number','ax?':'number','ay?':'number','az?':'number','save_velocity?':'boolean'});
type('MovePerkCondition',{type:'"perk"',perk:H('Perk'),'player?':'string','not?':'boolean'});type('MoveNamedCondition',{type:enumOf('current_animation','current_interval','item'),'name?':'string','player?':'string','item_type?':'string','item_subtype?':'string','not?':'boolean'});type('MoveConditionGroup',{type:enumOf('all','any'),conditions:`(${moveCondition})[]`,'not?':'boolean'});
type('MoveCharacterCondition',{type:'"character"',warrior:H('Warrior'),'not?':'boolean'});
type('MoveKey',{key:enumOf('Up','Up-Forward','Forward','Down-Forward','Down','Down-Back','Back','Up-Back','Punch','Kick','Ranged','Magic','RaidCharge','Super'),'press?':enumOf('Tap','Hold','Release')});
type('MoveKeysCondition',{type:'"keys"',keys:E('MoveKey')+'[]','not?':'boolean'});
type('MoveStageCondition',{type:'"round_stage"',name:enumOf('StartStance','Fight','EndStance','TryOn'),'player?':enumOf('Me','Enemy','Both'),'not?':'boolean'});
type('MoveScreenCondition',{type:'"screen"',name:enumOf('ShopArmor','ShopWeapon','ShopHelm','ShopMissile','ShopMagic','ShopRuby','ShopFree','ShopRaidItemPack','Profile','Fight'),'player?':enumOf('Me','Enemy','Both'),'not?':'boolean'});
type('MoveModCondition',{type:'"mod_exists"',name:'string','player?':enumOf('Me','Enemy','Both'),'not?':'boolean'});
type('MoveDamageTerm',{type:enumOf('UnarmedDamage','WeaponDamage','RangedDamage','MagicDamage'),'shift?':'number'});
type('MoveImpulse',{'x?':'number','y?':'number','z?':'number'});
type('MoveAttackOptions',{'no_effect?':'boolean','no_critical?':'boolean','ignores_block?':'boolean','ignores_all_invulnerable?':'boolean','body_part?':enumOf('Body','Head'),'defense_types?':'('+enumOf('BodyDefense','HeadDefense')+')[]','ignores_invulnerable?':'string[]'});
type('MoveAttack',{'edges?':'string[]','direct?':'boolean','hit_move?':H('Move'),'damage?':'number','damage_type?':enumOf('UnarmedDamage','WeaponDamage','RangedDamage','MagicDamage'),'damage_terms?':E('MoveDamageTerm')+'[]','hit?':enumOf('High','Middle','Low','Spinning','HighHeavy','MiddleShortPlus','Physycal','HighLong','NoReaction','WaspFly','Earthquake'),'id?':'integer','impulse?':E('MoveImpulse'),'options?':E('MoveAttackOptions')});
type('MoveInterval',{'type?':'string','name?':'string','start?':'integer','end?':'integer','attack?':E('MoveAttack')});
type('MovePoint',{object:enumOf('Nodes','Pivot','Wall','Animation','Floor','COM'),'player?':enumOf('Me','Enemy','Parent','Child','EnemyChild'),'part?':'string','shift_x?':'number','shift_y?':'number'});
type('MoveAlignment',{axes:'('+enumOf('X','Y','Z')+')[]',pivot:E('MovePoint'),position:E('MovePoint')});
type('MoveImpulseDirection',{'reverse?':'boolean'});
type('MoveDirection',{'from?':E('MovePoint'),'to?':E('MovePoint'),'impulse?':E('MoveImpulseDirection')});
type('MoveTransition',{conditions:`(${moveCondition})[]`,'frame_shift?':'integer','first_frame?':'integer'});
const move={id:'string','templates?':H('MoveTemplate')+'[]','core_templates?':'string[]','events?':`(${moveEvent}|${E('MoveEvent')})[]`,'conditions?':`(${moveCondition})[]`,'intervals?':E('MoveInterval')+'[]','locks?':`(${moveCondition})[]`,'align?':E('MoveAlignment'),'direction?':E('MoveDirection'),...Object.fromEntries(['type','mirror_node','tactic_equivalent','tactic_weapon'].map(k=>[k+'?','string'])),...Object.fromEntries(['priority','mid_frames','first_frame','end_frame'].map(k=>[k+'?','integer'])),'looped?':'boolean','ends_stage?':'boolean'};
type('MoveEffect',{name:'string',core_sequence:'string','scale?':'number','time_scale?':'number','looped?':'boolean','position?':E('MovePoint'),'follow?':'boolean'});
type('MoveProjectile',{name:'string',core_skeleton:'string','copy_parent_type?':enumOf('Weapon','Ranged','Magic'),'item?':H('Item'),'core_start_animation?':'string','start_move?':H('Move')});
type('MoveBulletChange',{type:enumOf('MagicBullet','RaidChargeBullet'),value:'integer'});
type('MoveSound',{core_sound:'string','voice?':enumOf('Male','MaleLow','Female')});
type('MoveShake',{'pause_time?':'integer','effect_time?':'integer','amplitude_x?':'number','amplitude_y?':'number','frequency_x?':'number','frequency_y?':'number'});
type('MoveScheduledAction',{type:enumOf('random_sound','try_on_end','effect','stop_effect','stop_follow_effect','create_projectile','add_bullets','delete_actor','sound','shake_screen'),'frame?':'integer','event?':enumOf('RoundStage','KeyPressed','KeyReleased','RoundStart','RoundEnd','Hit','Strike','WallHit','AnimationStart','AnimationEnd','IntervalStart','IntervalEnd','EveryFrame','Birth','ModExpires'),'core_sounds?':'string[]','sound?':E('MoveSound'),'shake?':E('MoveShake'),'effect?':E('MoveEffect'),'effect_name?':'string','projectile?':E('MoveProjectile'),'bullets?':E('MoveBulletChange'),'player?':enumOf('Me','Enemy','Parent','Child','EnemyChild')});
type('MoveProfile',{rank:'integer',core_icon:'string','display_name?':H('Localization')});
type('MoveTacticDistance',{axis:enumOf('X','Y','Full'),'minimum?':'number','maximum?':'number',from:E('MovePoint'),to:E('MovePoint')});
type('MoveTemplateDefinition',move);type('MoveDefinition',{...move,animation:H('Binary'),'transitions?':E('MoveTransition')+'[]','actions?':E('MoveScheduledAction')+'[]','profile?':E('MoveProfile'),'tactic_distance?':E('MoveTacticDistance'),'tactic_conditions?':`(${moveCondition})[]`,'no_wall_repulsion?':'boolean','no_interpolation_frames?':'boolean','no_magic_recharge?':'boolean','velocity?':E('MoveVelocity')});reg('moves.register_template','MoveTemplateDefinition','MoveTemplate');reg('moves.register','MoveDefinition','Move');
type('MoveItemLockExtension',{move:'string',item_type:enumOf('Weapon','Ranged','Magic','Armor','Helm','Skeleton'),source_subtype:'string',subtype:'string'});reg('moves.extend_item_lock','MoveItemLockExtension',null,'content.patch');
type('MoveIntervalEndPatch',{name:enumOf('Uninterrupt','SelfUninterrupt','Unstable'),expected:'integer',value:'integer'});
type('MoveHitPatch',{expected:enumOf('High','Middle','Low','Spinning','HighHeavy','MiddleShortPlus','Physycal','HighLong','NoReaction'),value:enumOf('High','Middle','Low','Spinning','HighHeavy','MiddleShortPlus','Physycal','HighLong','NoReaction')});
type('MoveSoundFramePatch',{name:'string',expected:'integer',value:'integer'});
type('MovePatch',{move:'string','disable?':'boolean','conditions?':move['conditions?'],'interval_end?':E('MoveIntervalEndPatch'),'hit?':E('MoveHitPatch'),'sound_frame?':E('MoveSoundFramePatch')});reg('moves.patch','MovePatch',null,'content.patch');
type('MovePerkLockRemoval',{move:'string',perk:H('Perk')});reg('moves.remove_perk_lock','MovePerkLockRemoval',null,'content.patch');
type('SoundAction',{type:'"sound"',audio:H('Audio'),'volume?':'number','looped?':'boolean'});type('HitEffectAction',{type:'"hit_effect"',name:'string'});type('TriggerDefinition',{id:'string','events?':move['events?'],'conditions?':move['conditions?'],'actions?':`(${E('SoundAction')}|${E('HitEffectAction')})[]`});reg('moves.register_trigger','TriggerDefinition','Trigger');
type('TacticValue',{...Object.fromEntries(['base','counter_factor','damage_factor','health_factor','enemy_health_factor','animation_frames_factor','child_frames_factor','magic_bullet_factor','missile_bullet_factor','hit_factor','distance_factor','shift','limit','anti_limit'].map(k=>[k+'?','number'])),'factor_type?':enumOf('linear','exponential')});type('TacticMemory',{'strikes?':'integer','round_factor?':'number'});type('TacticWeight',{'move?':H('Move'),'animation?':'string','value?':E('TacticValue')});type('AiActionTiming',{first_sample:'integer',last_sample:'integer',mid_frames:'integer',nominal_frames:'integer',nominal_seconds:'number',looped:'boolean'});type('AiActionInput',{control:enumOf('Up','Up-Forward','Forward','Down-Forward','Down','Down-Back','Back','Up-Back','Punch','Kick','Ranged','Magic','RaidCharge','Super','Unknown'),press:enumOf('tap','hold','release')});type('AiAction',{name:'string',type:enumOf('none','move','attack'),priority:'integer','timing?':E('AiActionTiming'),inputs:E('AiActionInput')+'[]'});type('AiDecision',{self:E('FighterSnapshot'),opponent:E('FighterSnapshot')+'?',frame:'integer',seconds:'number',actions:E('AiAction')+'[]'});type('TacticDefinition',{id:'string','on_decide?':`fun(memory:table,event:${E('AiDecision')}): ${E('AiAction')}|"wait"|nil`,'type?':enumOf('tabular','random'),'template?':'string','memory?':E('TacticMemory'),...Object.fromEntries(['counter_attack','dodge','block','safe_attack','table_attack','cautious_movement','dodge_missiles','dodge_magic'].map(k=>[k+'?',E('TacticValue')])),...Object.fromEntries(['animation_weights','quick_attacks','evades','expected_wait'].map(k=>[k+'?',E('TacticWeight')+'[]']))});reg('tactics.register','TacticDefinition','Tactic');fn('tactics.name',{tactic:H('Tactic')},'string',null);
type('ModeResult',{won:'boolean',step:'integer',total:'integer',completions:'integer',fight_id:'string'});
type('ModeRequest',{'private __eclipseModeRequest':'true'});type('EncounterPlan',{'warriors?':H('Warrior')+'[]','level?':'integer','rounds?':'integer','round_time?':'integer','rules?':H('Rule')+'[]','description?':'string'});type('ModePreparation',{step:'integer',total:'integer',completions:'integer',fight_id:'string'});
const mode={'on_prepare?':`fun(request:${E('ModeRequest')},event:${E('ModePreparation')}):${E('EncounterPlan')}|nil`,id:'string',fights:H('Fight')+'[]','repeatable?':'boolean','reset_on_loss?':'boolean','minimum_level?':'integer','starts_at?':'integer','ends_at?':'integer','entry_item?':H('Item'),'entry_count?':'integer','on_result?':`fun(result:${E('ModeResult')}):${H('Fight')}|"complete"|nil`};
type('ModeDefinition',mode);type('RaidDefinition',{...mode,'hard_mode?':'boolean'});for(const name of ['modes','events','raids']) reg(name+'.register',name==='raids'?'RaidDefinition':'ModeDefinition');
fn('modes.resolve',{request:E('ModeRequest'),plan:E('EncounterPlan')},'nil',null);fn('modes.cancel',{request:E('ModeRequest')},'nil',null);fn('modes.is_pending',{request:E('ModeRequest')},'boolean',null);
type('TimerPolicy',{subsystem:'"forge"',seconds:'integer','skip_enabled?':'boolean','complete_pending?':'boolean'});reg('timers.set','TimerPolicy',null,'policy.timers');fn('services.disable',{name:enumOf('paid_offers','battle_pass','ads','rewarded_video','online_services','payments')},'nil','policy.services');
type('CounterDefinition',{id:'string','maximum?':'integer'});reg('counters.register','CounterDefinition','Counter');fn('counters.get',{counter:H('Counter')},'integer','progression.read');fn('counters.add',{counter:H('Counter'),amount:'integer'},'integer','progression.write');type('AchievementDefinition',{id:'string',counter:H('Counter'),title:H('Localization'),description:H('Localization'),icon:H('Sprite'),threshold:'integer','hidden?':'boolean'});reg('achievements.register','AchievementDefinition');
for(const name of ['debug','info','warn','error']) fn('log.'+name,{message:'string'},'nil',null);
type('UiHandle', { 'private __eclipseUi': 'true' });
type('UiStyle', { 'font_size?':'integer','text_align?':enumOf('left','center','right'),
    'text_color?':'string','background_color?':'string','fill_color?':'string' });
type('UiNode', { id:'string', kind:enumOf('stack','row','column','scroll','text','button','progress','toggle','slider','image','grid'),
    'width?':'number','height?':'number','gap?':'number','text?':'string','value?':'number','checked?':'boolean',
    'visible?':'boolean','enabled?':'boolean','children?':E('UiNode')+'[]','style?':E('UiStyle'),'sprite?':H('Sprite'),'mirrored?':['boolean','Image only. Defaults to false; horizontally reflects the artwork without changing layout size.'],
    'columns?':'integer','cell_width?':'number','cell_height?':'number' });
type('UiPlacement', { 'anchor?':enumOf('top_left','top','top_right','left','center','right','bottom_left','bottom','bottom_right'),'x?':'number','y?':'number' });
type('UiDefinition', { id:'string',mount:enumOf('menu','modal','hud'),root:E('UiNode'),'placement?':E('UiPlacement'),
    'on_back?':`fun(view:${H('Ui')})`,
    'on_change?':`fun(view:${H('Ui')},widget_id:string,value:boolean|number)`,
    'on_click?':`fun(view:${H('Ui')},widget_id:string)`,
    'on_close?':`fun(view:${H('Ui')},reason:${enumOf('script','back','scene','error','destroyed')})` });
type('ActScreenLine', {text:H('Localization'),frames:['integer','Required duration at 60 frames per second, 1..3600.']});
type('ActScreenDefinition', {lines:E('ActScreenLine')+'[]','on_complete?':'fun()'});
fn('ui.act_screen',{definition:E('ActScreenDefinition')},'boolean','ui.create');
type('StoryDialogLine', {text:H('Localization'),'button?':[H('Localization'),'Caption of the more button for this page.']});
type('StoryDialogDefinition', {'portrait?':[H('Sprite'),'Omit to hide the portrait.'],lines:[E('StoryDialogLine')+'[]','1..16 dense pages.'],button:[H('Localization'),'Final button caption.'],'title?':H('Localization'),'mirrored?':'boolean','ignore_back?':'boolean','on_complete?':'fun()','on_cancel?':'fun()'});
fn('ui.story_dialog',{definition:E('StoryDialogDefinition')},'boolean','ui.create');
fn('ui.open',{definition:E('UiDefinition')},H('Ui'),'ui.create');
fn('ui.close',{view:H('Ui')},'nil',null);
fn('ui.is_open',{view:H('Ui')},'boolean',null);
fn('ui.set_text',{view:H('Ui'),widget_id:'string',text:'string'},'nil',null);
fn('ui.set_sprite',{view:H('Ui'),widget_id:'string',sprite:H('Sprite')},'nil',null);
fn('ui.set_value',{view:H('Ui'),widget_id:'string',value:'number'},'nil',null,{bounds:{value:[0,1]}});
fn('ui.set_checked',{view:H('Ui'),widget_id:'string',checked:'boolean'},'nil',null);
fn('ui.set_visible',{view:H('Ui'),widget_id:'string',visible:'boolean'},'nil',null);
fn('ui.set_enabled',{view:H('Ui'),widget_id:'string',enabled:'boolean'},'nil',null);
type("QuestSuppression",{target:"string"});reg("quests.suppress","QuestSuppression",null,"content.patch");
type('ProfileEquipmentSnapshot',{'item?':'string','type?':'string','subtype?':'string',owned:'boolean',count:'integer','upgrade?':'integer',enchantments:['string[]','Qualified lower-case perk IDs of the current enchantments, in native order; unknown perks are omitted.']});fn('profile.equipment',{},E('ProfileEquipmentSnapshot')+'[]','profile.read');
module.exports={types,functions,aliases,callbacks,storyCallbacks:['on_before_fight'],modeCallbacks:['on_result','on_prepare'],uiCallbacks:['on_complete','on_cancel','on_click','on_close','on_change','on_back'],aiCallbacks:['on_decide'],fighterMethods};
