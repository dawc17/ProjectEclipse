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
type('ComboEvent',{combo:'integer',last_combo:'integer'},'CombatEvent');
type('StyleEvent',{style_rank:'integer',style_name:'string',style_gain:'number',is_hit:'boolean'},'CombatEvent');
type('TickEvent',{frame:'integer',seconds:'number',delta_frames:'integer',delta_seconds:'number'},'CombatEvent');
type('FightEndEvent',{won:'boolean',player_result:enumOf('win','loss','surrender','timeout')},'CombatEvent');
const callbacks = ['on_fight_begin','on_round_begin','on_tick','on_damage_resolving','on_damage_dealing','on_damage_received','on_damage_dealt','on_block','on_critical','on_combo_changed','on_style_changed','on_round_end','on_fight_end'];
for (const stateful of [false,true]) {
    const fields = { id:'string', 'parameters?':schema, ...(stateful ? { state:E('BehaviorState') } : { 'state?':'nil' }) };
    for (const name of callbacks) {
        const event=name==='on_tick'?'TickEvent':name==='on_combo_changed'?'ComboEvent':name==='on_style_changed'?'StyleEvent':['on_damage_resolving','on_damage_dealing'].includes(name)?'IncomingDamageEvent':name==='on_fight_end'?'FightEndEvent':['on_damage_received','on_damage_dealt','on_block','on_critical'].includes(name)?'DamageEvent':'CombatEvent';
        fields[`${name}?`] = `fun(${stateful ? 'self:'+E('BehaviorSelf') : 'parameters:'+values}, fighter:${E(name==='on_damage_resolving'?'ResolvingFighter':name==='on_damage_dealing'?'OutgoingFighter':'Fighter')}, event:${E(event)})`;
    }
    type(stateful ? 'StatefulBehavior' : 'BehaviorDefinition', fields);
}
type('Fighter', { 'health?':'number', 'side?':'string', 'source?':'string', 'rule_id?':'string', 'opponent?':E('Opponent') });
type('Opponent', {'health?':'number'});
type('CombatPosition', {x:'number',y:'number',z:'number'});
type('FighterSnapshot', {health:'number',max_health:'number',health_bars:'integer',position:E('CombatPosition')});
type('CombatSnapshot', {self:E('FighterSnapshot'),'opponent?':E('FighterSnapshot'),frame:'integer',seconds:'number',round_active:'boolean'});
type('ResolvingFighter',{},'Fighter');
type('OutgoingFighter',{},'Fighter');
const fighterMethods = {
    snapshot:{params:{},returns:`${E('CombatSnapshot')}|nil`,capability:null},
    change_health:{params:{amount:'number'},capability:'combat.change_life'},
    add_magic_charge:{params:{amount:'number'},capability:'combat.magic_charge'},
    scale_outgoing_damage:{params:{multiplier:'number'},capability:'combat.modify_outgoing_hit'},
    scale_incoming_damage:{params:{multiplier:'number'},capability:'combat.modify_hit'},
    add_damage_shield:{params:{key:'string',fraction:'number',frames:'integer'},capability:'combat.effects'},
    remove_damage_shield:{params:{key:'string'},capability:'combat.effects'},
};
const equipment = { id:'string', display_name:H('Localization'), icon:H('Sprite'), model:H('Model') };
for (const name of ['Weapon','Armor','Helm','Ranged','Magic']) {
    type(`${name}Definition`, { ...equipment, ...(name === 'Weapon' ? {'subtype?':['string','Defaults to Katana. Match the model and move family.']} : ['Ranged','Magic'].includes(name) ? {subtype:'string'} : {}) });
    reg(`items.register_${name.toLowerCase()}`,`${name}Definition`,name);
}
type('NonEquipmentDefinition', {id:'string',display_name:H('Localization'),'icon?':H('Sprite'),'model?':H('Model'),'subtype?':'string','pack_label?':'string','silent_receive?':'boolean','spend_after_use?':'boolean'});
for (const name of ['Consumable','Free','Seal']) reg(`items.register_${name.toLowerCase()}`,'NonEquipmentDefinition',name);
lookup('items.get','Item');
type('ItemAlias',{from:'string',to:H('Item')}); reg('items.alias','ItemAlias');
type('IdDefinition',{id:'string'}); reg('items.tombstone','IdDefinition');
type('ShopListing',{section:enumOf('weapons','armor','helmets','ranged','magic'),item:H('Item'),level:['integer','Weapon: 1-52; armor/helm: 2-52; ranged/magic: 6-52.'],price:H('Price')});
fn('shop.addItem',{definition:E('ShopListing')},'string'); aliases['sf2.shop.add']='sf2.shop.addItem';
type('Availability',{item:H('Item'),'visibility?':enumOf('inherit','force_visible','force_hidden'),'required_group?':'string'}); reg('shop.set_availability','Availability',null,'content.patch');
for (const name of ['coins','gems']) fn(`price.${name}`,{amount:'integer'},H('Price'),null,{bounds:{amount:[0,2147483647]}});
for (const name of ['sprite','model','audio','binary']) fn(`assets.${name}`,{reference:'string'},H(name[0].toUpperCase()+name.slice(1)),null,{referenceKind:name});
fn('assets.qualify',{reference:'string'},'string',null);
fn('assets.exists',{reference:'string'},'boolean',null);
type('AssetReplacement',{target:'string',replacement:'string'}); reg('assets.replace','AssetReplacement',null,'assets.replace');
fn('localization.key',{key:'string'},H('Localization'),'content.register',{referenceKind:'localization'});
type('LocalizationPatch',{target:'string',language:'string',value:'string'});reg('localization.patch','LocalizationPatch',null,'content.patch');
reg('state.register','StateDefinition',null,'state.write');
fn('state.get',{name:'string'},`${primitive}|nil`,'state.read');
fn('state.set',{values},'nil','state.write');
fn('state.unset',{name:'string'},'nil','state.write');
reg('behaviors.register','BehaviorDefinition','Behavior');
functions['sf2.behaviors.register'].overload = `fun(definition:${E('StatefulBehavior')}):${H('Behavior')}`;
lookup('perks.get','Perk');
type('PerkUpgrade',{level:'integer','description?':H('Localization'),'parameters?':values});
type('PerkDefinition',{id:'string',display_name:H('Localization'),description:H('Localization'),'icon?':H('Sprite'),behavior:H('Behavior'),kind:enumOf('single','combo'),'parameters?':values,'upgrades?':E('PerkUpgrade')+'[]'});
type('TemplatePerk',{id:'string',display_name:H('Localization'),description:H('Localization'),'icon?':H('Sprite'),template:H('Perk'),'parameters?':values,'upgrades?':E('PerkUpgrade')+'[]'});
fn('perks.register',{definition:`${E('PerkDefinition')}|${E('TemplatePerk')}`},H('Perk'));
const equipmentKinds=enumOf('weapon','armor','helm','ranged','magic');
type('EnchantmentDefinition',{id:'string',recipe:enumOf('simple','medium','complex'),item_types:`(${equipmentKinds})[]`,behavior:H('Behavior'),display_name:H('Localization'),description:H('Localization'),'icon?':H('Sprite'),'parameters?':values});
type('LegacyEnchantment',{id:'string',recipe:enumOf('simple','medium','complex'),item_types:`(${equipmentKinds})[]`,perk:H('Perk')});
fn('enchantments.register',{definition:`${E('EnchantmentDefinition')}|${E('LegacyEnchantment')}`},H('Enchantment'));
type('ZoneDefinition',{id:'string','file?':'string','start?':'boolean'});reg('zones.register','ZoneDefinition','Zone');lookup('zones.get','Zone');
type('BattleDefinition',{id:'string',zone:H('Zone'),type:'string','x?':'integer','y?':'integer',...Object.fromEntries(['alias','title','description','icon','icon_atlas','preview','eclipse_toggle_name','location','music','reward_image'].map(k=>[k+'?','string'])),'show_resistance?':'boolean'});reg('battles.register','BattleDefinition','Battle');
type('AttributeAlignment',{factor:'number',shift:'number','priority?':'integer','mode?':enumOf('all','normal','eclipse')});
type('WarriorDefinition',{id:'string','template?':H('WarriorTemplate'),...Object.fromEntries(['first_name','last_name','avatar','voice','group'].map(k=>[k+'?','string'])),'level?':'integer','tactic?':`${H('Tactic')}|string`,'random?':'integer','items?':H('Item')+'[]','perks?':H('Perk')+'[]','attributes?':'table<string,number>','attribute_alignments?':E('AttributeAlignment')+'[]','health_bars?':['integer','0 inherits the template; 1-10000 is the total number of health bars.']});
lookup('warriors.get_template','WarriorTemplate');reg('warriors.register','WarriorDefinition','Warrior');
type('ItemGrant',{item:H('Item'),'upgrade?':'integer'});type('RewardCandidate',{item:H('Item'),'upgrade?':'integer','weight?':'number'});type('RewardChoice',{items:E('RewardCandidate')+'[]'});
type('RewardDefinition',{id:'string','items?':E('ItemGrant')+'[]','choices?':E('RewardChoice')+'[]','gems?':'integer'});reg('rewards.register','RewardDefinition','Reward');
type('FightDefinition',{id:'string',battle:H('Battle'),'warriors?':H('Warrior')+'[]','rules?':H('Rule')+'[]','rewards?':[H('Reward')+'[]','First slot is the zero-win result; second slot is one win.'],...Object.fromEntries(['rounds','round_time','replays','replay_interval','power'].map(k=>[k+'?','integer'])),...Object.fromEntries(['location','music','description','reward_image'].map(k=>[k+'?','string'])),'evaluated_rating?':'number','health_recovery?':'number','locked?':'boolean'});reg('fights.register','FightDefinition','Fight');
type('FightPatch',{target:'string','description?':'string','rounds?':'integer','round_time?':'integer','location?':'string','music?':'string','rules?':H('Rule')+'[]','append_rules?':H('Rule')+'[]'});reg('fights.patch','FightPatch',null,'content.patch');
const rule={id:'string','target?':enumOf('player','opponent','all'),'mode?':enumOf('normal','eclipse','all'),'rounds?':'integer[]'};
for (const [name,extra] of Object.entries({no_perks:{'name?':'string'},require_item:{item:H('Item'),'minimum_level?':'integer'},equip_item:{item:H('Item'),'minimum_level?':'integer'},avatar:{name:'string'},name:{name:'string'},no_button:{name:'string'},perk:{perk:H('Perk')},behavior:{behavior:H('Behavior'),'parameters?':values},recharge_magic_each_round:{},attributes:{values:'table<string,number>'}})) {
    const fields={...rule,...extra};if(name==='require_item') delete fields['target?'];
    const shape='Rule_'+name;type(shape,fields);reg('rules.'+name,shape,'Rule');
}
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
type('ForgeItem',{equipment:equipmentKinds,'enchantments?':'integer','bar_scale?':'string','min_deviation?':'integer','max_deviation?':'integer','random_aspect?':'boolean'});type('ForgeCandidate',{perk:H('Perk'),equipment:equipmentKinds,'min_level?':'integer','max_level?':'integer'});type('ForgeRecipe',{id:'string','alias?':'string',economic_profile:H('ForgeProfile'),items:E('ForgeItem')+'[]',candidates:E('ForgeCandidate')+'[]'});reg('forge.register_recipe','ForgeRecipe','ForgeRecipe');
type('Fonts',{content:'string',title:'string',button:'string','size_scale?':'number','line_spacing?':'number','custom_line_spacing_scale?':'number'});type('LocaleDefinition',{id:'string',name:'string',locale:'string','alias?':'string','is_asian?':'boolean','file_icon?':'string','file_icon_selected?':'string','loader_image?':'string','preloader_image?':'string','fonts?':E('Fonts')});fn('locales.register',{definition:E('LocaleDefinition')},'string');
type('LocationImage',{sprite:H('Sprite'),'x?':'number','y?':'number','width?':'number','height?':'number','opaque?':'boolean','flip_x?':'boolean','flip_y?':'boolean','mask?':'boolean'});type('FighterPositions',{player_x:'number',player_y:'number',enemy_x:'number',enemy_y:'number'});type('LocationLayer',{'type?':'integer','factor?':'number','scaling?':'boolean','images?':E('LocationImage')+'[]','fighters?':E('FighterPositions')});type('LocationDefinition',{id:'string','color?':'string',...Object.fromEntries(['wall','floor','position_y','width','height','min_width','friction_force','grid_size'].map(k=>[k+'?','number'])),'music?':H('Audio'),layers:E('LocationLayer')+'[]'});reg('locations.register','LocationDefinition','Location');fn('locations.name',{location:H('Location')},'string',null);
const moveEvent=enumOf('animation_end','animation_start','interval_end','interval_start','hit','strike','every_frame','birth','round_stage_start','mod_expires');
type('MoveEvent',{type:moveEvent,'name?':'string','player?':'string'});
const moveCondition=`${E('MovePerkCondition')}|${E('MoveNamedCondition')}|${E('MoveConditionGroup')}`;
type('MovePerkCondition',{type:'"perk"',perk:H('Perk'),'player?':'string','not?':'boolean'});type('MoveNamedCondition',{type:enumOf('current_animation','current_interval','item'),'name?':'string','player?':'string','item_type?':'string','item_subtype?':'string','not?':'boolean'});type('MoveConditionGroup',{type:enumOf('all','any'),conditions:`(${moveCondition})[]`,'not?':'boolean'});type('MoveInterval',{'type?':'string','name?':'string'});
const move={id:'string','templates?':H('MoveTemplate')+'[]','core_templates?':'string[]','events?':`(${moveEvent}|${E('MoveEvent')})[]`,'conditions?':`(${moveCondition})[]`,'intervals?':E('MoveInterval')+'[]',...Object.fromEntries(['type','mirror_node','tactic_equivalent','tactic_weapon'].map(k=>[k+'?','string'])),...Object.fromEntries(['priority','mid_frames','first_frame','end_frame'].map(k=>[k+'?','integer'])),'looped?':'boolean','ends_stage?':'boolean'};
type('MoveTemplateDefinition',move);type('MoveDefinition',{...move,animation:H('Binary')});reg('moves.register_template','MoveTemplateDefinition','MoveTemplate');reg('moves.register','MoveDefinition','Move');
type('SoundAction',{type:'"sound"',audio:H('Audio'),'volume?':'number','looped?':'boolean'});type('HitEffectAction',{type:'"hit_effect"',name:'string'});type('TriggerDefinition',{id:'string','events?':move['events?'],'conditions?':move['conditions?'],'actions?':`(${E('SoundAction')}|${E('HitEffectAction')})[]`});reg('moves.register_trigger','TriggerDefinition','Trigger');
type('TacticValue',{...Object.fromEntries(['base','counter_factor','damage_factor','health_factor','enemy_health_factor','animation_frames_factor','child_frames_factor','magic_bullet_factor','missile_bullet_factor','hit_factor','distance_factor','shift','limit','anti_limit'].map(k=>[k+'?','number'])),'factor_type?':enumOf('linear','exponential')});type('TacticMemory',{'strikes?':'integer','round_factor?':'number'});type('TacticWeight',{'move?':H('Move'),'animation?':'string','value?':E('TacticValue')});type('TacticDefinition',{id:'string','type?':enumOf('tabular','random'),'template?':'string','memory?':E('TacticMemory'),...Object.fromEntries(['counter_attack','dodge','block','safe_attack','table_attack','cautious_movement','dodge_missiles','dodge_magic'].map(k=>[k+'?',E('TacticValue')])),...Object.fromEntries(['animation_weights','quick_attacks','evades','expected_wait'].map(k=>[k+'?',E('TacticWeight')+'[]']))});reg('tactics.register','TacticDefinition','Tactic');fn('tactics.name',{tactic:H('Tactic')},'string',null);
const mode={id:'string',fights:H('Fight')+'[]','repeatable?':'boolean','reset_on_loss?':'boolean','minimum_level?':'integer','starts_at?':'integer','ends_at?':'integer','entry_item?':H('Item'),'entry_count?':'integer'};
type('ModeDefinition',mode);type('RaidDefinition',{...mode,'hard_mode?':'boolean'});for(const name of ['modes','events','raids']) reg(name+'.register',name==='raids'?'RaidDefinition':'ModeDefinition');
type('TimerPolicy',{subsystem:'"forge"',seconds:'integer','skip_enabled?':'boolean'});reg('timers.set','TimerPolicy',null,'policy.timers');fn('services.disable',{name:enumOf('paid_offers','battle_pass','ads','rewarded_video','online_services','payments')},'nil','policy.services');
type('CounterDefinition',{id:'string','maximum?':'integer'});reg('counters.register','CounterDefinition','Counter');fn('counters.get',{counter:H('Counter')},'integer','progression.read');fn('counters.add',{counter:H('Counter'),amount:'integer'},'integer','progression.write');type('AchievementDefinition',{id:'string',counter:H('Counter'),title:H('Localization'),description:H('Localization'),icon:H('Sprite'),threshold:'integer','hidden?':'boolean'});reg('achievements.register','AchievementDefinition');
for(const name of ['debug','info','warn','error']) fn('log.'+name,{message:'string'},'nil',null);
for(const [name,target] of Object.entries({log:'info',warn:'warn',error:'error'})) aliases['sf2.mod.'+name]='sf2.log.'+target;
type('UiHandle', { 'private __eclipseUi': 'true' });
type('UiNode', { id:'string', kind:enumOf('stack','row','column','scroll','text','button','progress'),
    'width?':'number','height?':'number','gap?':'number','text?':'string','value?':'number',
    'visible?':'boolean','enabled?':'boolean','children?':E('UiNode')+'[]' });
type('UiPlacement', { 'anchor?':enumOf('top_left','top','top_right','left','center','right','bottom_left','bottom','bottom_right'),'x?':'number','y?':'number' });
type('UiDefinition', { id:'string',mount:enumOf('menu','modal','hud'),root:E('UiNode'),'placement?':E('UiPlacement'),
    'on_click?':`fun(view:${H('Ui')},widget_id:string)` });
fn('ui.open',{definition:E('UiDefinition')},H('Ui'),'ui.create');
fn('ui.close',{view:H('Ui')},'nil',null);
fn('ui.is_open',{view:H('Ui')},'boolean',null);
fn('ui.set_text',{view:H('Ui'),widget_id:'string',text:'string'},'nil',null);
fn('ui.set_value',{view:H('Ui'),widget_id:'string',value:'number'},'nil',null,{bounds:{value:[0,1]}});
fn('ui.set_visible',{view:H('Ui'),widget_id:'string',visible:'boolean'},'nil',null);
fn('ui.set_enabled',{view:H('Ui'),widget_id:'string',enabled:'boolean'},'nil',null);
module.exports={types,functions,aliases,callbacks,fighterMethods};
