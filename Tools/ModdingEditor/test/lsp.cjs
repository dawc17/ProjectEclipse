// Exercise the actual LuaLS completion/hover/signature/diagnostic protocol.
// Usage: npm test -- /absolute/path/to/lua-language-server[.exe]
const { spawn } = require('node:child_process');
const fs = require('node:fs');
const path = require('node:path');
const { pathToFileURL } = require('node:url');
const assert = require('node:assert/strict');

const root = path.resolve(__dirname, '..');
const binary = process.argv[2] || path.join(root, '.test-runtime/luals-3.18.2/bin/lua-language-server.exe');
const workspace = path.join(root, '.test-runtime/workspace');
fs.mkdirSync(workspace, { recursive: true });
const config = {
    runtime: { version: 'Lua 5.2' },
    workspace: { library: [path.join(root, 'library')], checkThirdParty: false },
    diagnostics: { workspaceDelay: 0 },
    telemetry: { enable: false },
};
fs.writeFileSync(path.join(workspace, '.luarc.json'), JSON.stringify(config));
const server = spawn(binary, ['--logpath', path.join(root, '.test-runtime/log')], { windowsHide: true });
const pending = new Map();
const diagnostics = new Map();
let buffer = Buffer.alloc(0);
let sequence = 0;
let stderr = '';
server.stderr.on('data', chunk => { stderr += chunk; });
const send = message => {
    const body = Buffer.from(JSON.stringify({ jsonrpc: '2.0', ...message }));
    server.stdin.write(`Content-Length: ${body.length}\r\n\r\n`);
    server.stdin.write(body);
};
const notify = (method, params) => send({ method, params });
function request(method, params) {
    return new Promise((resolve, reject) => {
        const id = ++sequence;
        const timer = setTimeout(() => { pending.delete(id); reject(new Error(`Timed out: ${method}\n${stderr}`)); }, 20000);
        pending.set(id, { resolve, reject, timer });
        send({ id, method, params });
    });
}
server.stdout.on('data', chunk => {
    buffer = Buffer.concat([buffer, chunk]);
    while (true) {
        const end = buffer.indexOf('\r\n\r\n');
        if (end < 0) break;
        const length = Number(/Content-Length:\s*(\d+)/i.exec(buffer.subarray(0, end).toString())[1]);
        if (buffer.length < end + 4 + length) break;
        const message = JSON.parse(buffer.subarray(end + 4, end + 4 + length));
        buffer = buffer.subarray(end + 4 + length);
        if (message.method && message.id !== undefined) {
            let result = null;
            if (message.method === 'workspace/configuration') {
                result = message.params.items.map(({ section }) =>
                    !section || section === 'Lua' ? config : section.replace(/^Lua\./, '').split('.').reduce((v, k) => v?.[k], config));
            }
            send({ id: message.id, result });
        } else if (message.method === 'textDocument/publishDiagnostics') {
            diagnostics.set(decodeURIComponent(message.params.uri).toLowerCase(), message.params.diagnostics);
        } else if (pending.has(message.id)) {
            const entry = pending.get(message.id);
            pending.delete(message.id);
            clearTimeout(entry.timer);
            message.error ? entry.reject(new Error(JSON.stringify(message.error))) : entry.resolve(message.result);
        }
    }
});
const sleep = ms => new Promise(resolve => setTimeout(resolve, ms));
async function until(check, label) {
    const deadline = Date.now() + 20000;
    while (Date.now() < deadline) {
        const result = await check();
        if (result) return result;
        await sleep(250);
    }
    throw new Error(`Timed out waiting for ${label}`);
}
function open(name, text) {
    const uri = pathToFileURL(path.join(workspace, name)).href;
    fs.writeFileSync(path.join(workspace, name), text);
    notify('textDocument/didOpen', { textDocument: { uri, languageId: 'lua', version: 1, text } });
    return uri;
}
function probe(name, text) {
    const offset = text.indexOf('|');
    assert(offset >= 0);
    const before = text.slice(0, offset).split('\n');
    return { textDocument: { uri: open(name, text.replace('|', '')) }, position: { line: before.length - 1, character: before.at(-1).length } };
}
const labels = result => (result?.items ?? result ?? []).map(item => typeof item.label === 'string' ? item.label : item.label.label);

async function main() {
    const rootUri = pathToFileURL(workspace).href;
    await request('initialize', {
        processId: process.pid, rootUri, workspaceFolders: [{ uri: rootUri, name: 'Eclipse preview test' }],
        capabilities: { workspace: { configuration: true }, textDocument: { completion: { completionItem: { snippetSupport: true } }, hover: { contentFormat: ['markdown', 'plaintext'] } } },
    });
    notify('initialized', {});
    notify('workspace/didChangeConfiguration', { settings: { Lua: config } });

    const moduleProbe = probe('modules.lua', 'local sf2 = require("sf2")\nsf2.|');
    await until(async () => {
        const found = labels(await request('textDocument/completion', moduleProbe));
        return ['items', 'assets', 'localization', 'price', 'shop', 'log'].every(label => found.includes(label));
    }, 'module completion');
    fs.writeFileSync(path.join(root, '.test-runtime/modules.json'), JSON.stringify(await request('textDocument/completion', moduleProbe), null, 2));
    console.log('PASS: require("sf2") resolves and completes API modules');

    const functionProbe = probe('functions.lua', 'local sf2 = require("sf2")\nsf2.items.|');
    await until(async () => {
        const result = await request('textDocument/completion', functionProbe);
        fs.writeFileSync(path.join(root, '.test-runtime/completion.json'), JSON.stringify(result, null, 2));
        fs.writeFileSync(path.join(root, '.test-runtime/hover.json'), JSON.stringify(await request('textDocument/hover', { textDocument: functionProbe.textDocument, position: { line: 1, character: 5 } }), null, 2));
        return labels(result).some(label => label.startsWith('register_weapon'));
    }, 'function completion');
    console.log('PASS: weapon function completion');

    const fieldsProbe = probe('fields.lua', 'local sf2 = require("sf2")\nsf2.items.register_weapon {\n    |\n}');
    await until(async () => {
        const result = await request('textDocument/completion', fieldsProbe);
        fs.writeFileSync(path.join(root, '.test-runtime/fields.json'), JSON.stringify(result, null, 2));
        const fields = labels(result);
        return ['id', 'display_name', 'icon', 'model', 'subtype'].every(field => fields.some(label => label.replace(/\?$/, '') === field || label.startsWith(`${field} `)));
    }, 'weapon field completion');
    console.log('PASS: all five weapon table fields complete without manual type annotations');

    const hoverProbe = probe('hover.lua', 'local sf2 = require("sf2")\nlocal register = sf2.items.register_wea|pon');
    await until(async () => {
        const hover = JSON.stringify(await request('textDocument/hover', hoverProbe));
        return hover?.includes('content.register') && hover.includes('https://dawc17.github.io/ProjectEclipse/');
    }, 'hover documentation');
    console.log('PASS: hover includes capability guidance and a wiki link');

    const signatureProbe = probe('signature.lua', 'local sf2 = require("sf2")\nsf2.price.coins(|)');
    await until(async () => {
        const signature = JSON.stringify(await request('textDocument/signatureHelp', signatureProbe));
        return signature?.includes('amount') && signature.includes('integer');
    }, 'signature help');
    console.log('PASS: price signature help');

    const api = require('../data/api.json');
    const modules = [...new Set(Object.keys(api.functions).map(n => n.split('.')[1]))];
    for (const module of modules) {
        const query = probe(`module-${module}.lua`, `local sf2 = require("sf2")\nsf2.${module}.|`);
        const expected = [...Object.keys(api.functions), ...Object.keys(api.aliases), ...Object.keys(api.constants)]
            .filter(n => n.startsWith(`sf2.${module}.`)).map(n => n.split('.')[2]);
        await until(async () => {
            const found = labels(await request('textDocument/completion', query));
            return expected.every(name => found.some(label => label === name || label.startsWith(name + '(')));
        }, `all ${module} functions, aliases, and constants`);
    }
    console.log(`PASS: every function, alias, and constant completes across ${modules.length} API modules`);
    const ruleFields = probe('rule-fields.lua', 'local sf2=require("sf2")\nsf2.rules.behavior { | }');
    await until(async () => {
        const found = labels(await request('textDocument/completion', ruleFields));
        return ['behavior', 'parameters', 'target', 'rounds', 'mode'].every(key => found.some(value => value.startsWith(key)));
    }, 'battle behavior rule fields');
    const patchFields = probe('fight-patch-fields.lua', 'local sf2=require("sf2")\nsf2.fights.patch { | }');
    await until(async () => {
        const found = labels(await request('textDocument/completion', patchFields));
        return ['rules', 'append_rules', 'location', 'music'].every(key => found.some(value => value.startsWith(key)));
    }, 'fight patch rule and presentation fields');
    const callback = probe('callback.lua', 'local sf2=require("sf2")\nsf2.behaviors.register { id="test", on_damage_resolving=function(params, fighter, event)\n fighter:|\nend }');
    await until(async () => labels(await request('textDocument/completion', callback)).some(n => n.startsWith('scale_incoming_damage')), 'resolving fighter callback inference');
    const event = probe('event.lua', 'local sf2=require("sf2")\nsf2.behaviors.register { id="test", on_damage_received=function(params, fighter, event)\n local value=event.|\nend }');
    await until(async () => labels(await request('textDocument/completion', event)).includes('health_before'), 'damage event inference');
    const stateful = probe('stateful.lua', 'local sf2=require("sf2")\nsf2.behaviors.register { id="test", state={fields={hits={type="integer",default=0}}}, on_damage_received=function(self, fighter, event)\n local value=self.|\nend }');
    await until(async () => labels(await request('textDocument/completion', stateful)).includes('state'), 'stateful callback inference');
    console.log('PASS: inline callbacks infer fighter methods, damage events, and stateful self');
    const snapshot = probe('snapshot.lua', 'local sf2=require("sf2")\nsf2.behaviors.register { id="test", on_round_begin=function(_, fighter)\n local combat=fighter:snapshot()\n if combat then local value=combat.self.| end\nend }');
    await until(async () => {
        const found=labels(await request('textDocument/completion',snapshot));
        return ['health','max_health','health_bars','position'].every(name=>found.includes(name));
    }, 'snapshot return type inference');
    console.log('PASS: combat snapshot return type and fighter fields complete');
    const flagMethods=probe('flag-methods.lua','local sf2=require("sf2")\nsf2.behaviors.register { id="flags",on_animation_start=function(_,fighter)\n fighter:|\nend }');
    await until(async()=>{const found=labels(await request('textDocument/completion',flagMethods));return ['set_flag','has_flag','clear_flag','set_control_blocked'].every(key=>found.some(name=>name.startsWith(key)));},'scoped combat flag methods');
    const formReceipt = probe('form-receipt.lua', 'local sf2=require("sf2")\nlocal form=sf2.warriors.register { id="form" }\nsf2.behaviors.register { id="shift", on_tick=function(_, fighter)\n local result=fighter:change_form(form)\n local value=result.|\nend }');
    await until(async () => {
        const found=labels(await request('textDocument/completion',formReceipt));
        return ['status','error'].every(key=>found.includes(key));
    }, 'form request result fields');
    console.log('PASS: form request completion fields infer from fighter method');

    const validText = fs.readFileSync(path.join(root, 'templates/weapon/scripts/main.lua'), 'utf8');
    // LuaLS does not publish an initial empty report. Introduce an error, then
    // fix it, so a cleared report positively confirms that diagnostics ran.
    const validUri = open('valid.lua', validText + '\nsf2.price.coins("temporary test error")\n');
    const validKey = decodeURIComponent(validUri).toLowerCase();
    await until(() => (diagnostics.get(validKey)?.length ?? 0) > 0, 'temporary diagnostic');
    notify('textDocument/didChange', { textDocument: { uri: validUri, version: 2 }, contentChanges: [{ text: validText }] });
    await until(() => diagnostics.get(validKey)?.length === 0, 'cleared sample diagnostics');
    console.log('PASS: complete first-weapon script has no diagnostics');
    const ruleText = fs.readFileSync(path.join(root, 'templates/battle-rules/scripts/main.lua'), 'utf8');
    const ruleUri = open('valid-rule.lua', ruleText + '\nsf2.price.coins("temporary test error")\n');
    const ruleKey = decodeURIComponent(ruleUri).toLowerCase();
    await until(() => (diagnostics.get(ruleKey)?.length ?? 0) > 0, 'temporary rule diagnostic');
    notify('textDocument/didChange', { textDocument: { uri: ruleUri, version: 2 }, contentChanges: [{ text: ruleText }] });
    await until(() => diagnostics.get(ruleKey)?.length === 0, 'cleared battle-rule diagnostics');
    console.log('PASS: complete snapshot-based battle rule has no diagnostics');
    const upgradeFields=probe('upgrade-fields.lua','local sf2=require("sf2")\nsf2.perks.register { upgrades = { { | } } }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',upgradeFields));
        return ['level','description','parameters'].every(name=>found.some(value=>value.startsWith(name)));
    },'perk upgrade entry fields');
    console.log('PASS: perk upgrade entries complete');
    const timerFields=probe('timer-fields.lua','local sf2=require("sf2")\nsf2.timers.set { subsystem="forge", seconds=0, | }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',timerFields));
        return ['skip_enabled','complete_pending'].every(name=>found.some(value=>value.startsWith(name)));
    },'pending forge timer policy fields');
      console.log('PASS: pending forge timer policy fields complete');
      const shopFields=probe('shop-fields.lua','local sf2=require("sf2")\nsf2.shop.set_availability { item=sf2.items.get("core:items/weapon/WEAPON_KNIVES"), | }');
      await until(async()=>{
          const found=labels(await request('textDocument/completion',shopFields));
          return ['minimum_level','required_group','visibility'].every(name=>found.some(value=>value.startsWith(name)));
      },'shop availability level fields');
      console.log('PASS: shop availability level fields complete');
      const priceFields=probe('shop-price-fields.lua','local sf2=require("sf2")\nsf2.shop.set_price { | }');
      await until(async()=>{
          const found=labels(await request('textDocument/completion',priceFields));
          return ['item','price','secondary_price'].every(name=>found.some(value=>value.startsWith(name)));
      },'shop price fields');
      console.log('PASS: shop price fields complete');
      const presentationFields=probe('presentation-fields.lua','local sf2=require("sf2")\nsf2.items.set_presentation { | }');
      await until(async()=>{
          const found=labels(await request('textDocument/completion',presentationFields));
          return ['item','icon','model'].every(name=>found.some(value=>value.startsWith(name)));
      },'item presentation fields');
      console.log('PASS: item presentation fields complete');
      const subtypeFields=probe('subtype-fields.lua','local sf2=require("sf2")\nsf2.items.set_subtype { | }');
      await until(async()=>{
          const found=labels(await request('textDocument/completion',subtypeFields));
          return ['item','subtype'].every(name=>found.some(value=>value.startsWith(name)));
      },'combat subtype fields');
      console.log('PASS: equipment combat subtype fields complete');
      const profileFields=probe('initial-profile-fields.lua','local sf2=require("sf2")\nsf2.items.set_initial_profile { | }');
      await until(async()=>{
          const found=labels(await request('textDocument/completion',profileFields));
          return ['item','level','upgrade_level','initial_stats','upgrade_template','legacy_paid_item','clear_local_upgrades'].every(name=>found.some(value=>value.startsWith(name)));
      },'item initial profile fields');
      console.log('PASS: item initial profile fields complete');
    for (const [category,fields] of Object.entries({weapon:['weapon_damage'],armor:['body_defense','head_defense','unarmed_damage'],helm:['head_defense'],ranged:['ranged_damage','weapon_damage'],magic:['magic_damage']})) {
        const initial=probe('initial-'+category+'.lua','local sf2=require("sf2")\nsf2.items.register_'+category+' { initial_stats={ | } }');
        await until(async()=>{
            const found=labels(await request('textDocument/completion',initial));
            return fields.every(name=>found.some(value=>value.startsWith(name)));
        },category+' initial stat completion');
    }
    console.log('PASS: category-specific initial equipment stats complete');
    const damageFields=probe('move-damage-fields.lua','local sf2=require("sf2")\nsf2.moves.register_template { id="test",intervals={{type="Attack",attack={edges={"Edge"}, | }}} }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',damageFields));
        return ['damage_terms','hit','impulse'].every(name=>found.some(value=>value.startsWith(name)));
    },'move attack fields');
    const termFields=probe('move-term-fields.lua','local sf2=require("sf2")\nsf2.moves.register_template { id="test",intervals={{type="Attack",attack={edges={"Edge"},damage_terms={{ | }}}}} }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',termFields));
        return ['type','shift'].every(name=>found.some(value=>value.startsWith(name)));
    },'move damage term fields');
    console.log('PASS: mixed move damage fields complete');
    const moveLockFields=probe('move-lock-fields.lua','local sf2=require("sf2")\nsf2.moves.extend_item_lock { | }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',moveLockFields));
        return ['move','item_type','source_subtype','subtype'].every(name=>found.some(value=>value.startsWith(name)));
    },'move item lock extension fields');
    const movePatchFields=probe('move-patch-fields.lua','local sf2=require("sf2")\nsf2.moves.patch { | }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',movePatchFields));
        return ['move','conditions','interval_end','hit','sound_frame'].every(name=>found.some(value=>value.startsWith(name)));
    },'move patch fields');
    const movePatchFrame=probe('move-patch-frame.lua','local sf2=require("sf2")\nsf2.moves.patch { move="Test", interval_end={ | } }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',movePatchFrame));
        return ['name','expected','value'].every(name=>found.some(value=>value.startsWith(name)));
    },'move patch selector fields');
    console.log('PASS: scoped native move patches and expected-value selectors complete');
    const moveGraphFields=probe('move-graph-fields.lua','local sf2=require("sf2")\nsf2.moves.register { id="move",animation=sf2.assets.binary("animations/test"), | }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',moveGraphFields));
        return ['locks','transitions','align','direction','actions','profile','tactic_distance','tactic_conditions','no_wall_repulsion','no_interpolation_frames'].every(name=>found.some(value=>value.startsWith(name)));
    },'move graph fields');
    const alignFields=probe('move-align-fields.lua','local sf2=require("sf2")\nsf2.moves.register { id="move",animation=sf2.assets.binary("animations/test"),align={axes={"X"},pivot={ | }} }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',alignFields));
        return ['object','part','player'].every(name=>found.some(value=>value.startsWith(name)));
    },'move alignment point fields');
    console.log('PASS: move graph and item lock extension fields complete');
    const moveActionFields=probe('move-action-fields.lua','local sf2=require("sf2")\nsf2.moves.register { id="move",animation=sf2.assets.binary("animations/test"),actions={{ | }} }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',moveActionFields));
        return ['type','frame','event','core_sounds'].every(name=>found.some(value=>value.startsWith(name)));
    },'scheduled move action fields');
    const stopSound=probe('move-stop-sound.lua','local sf2=require("sf2")\nsf2.moves.register { id="move",animation=sf2.assets.binary("animations/test"),actions={{type="stop_sound",event="Hit", | }} }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',stopSound));
        return found.some(value=>value.startsWith('core_sound'));
    },'native sound stop field');
    const playAnimation=probe('move-play-animation.lua','local sf2=require("sf2")\nsf2.moves.register { id="move",animation=sf2.assets.binary("animations/test"),actions={{type="play_animation",frame=17, | }} }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',playAnimation));
        return ['move','core_animation','player','child_name'].every(name=>found.some(value=>value.startsWith(name)));
    },'child animation action fields');
    const moveTacticFields=probe('move-tactic-fields.lua','local sf2=require("sf2")\nsf2.moves.register { id="move",animation=sf2.assets.binary("animations/test"),tactic_distance={ | } }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',moveTacticFields));
        return ['axis','minimum','maximum','from','to'].every(name=>found.some(value=>value.startsWith(name)));
    },'move tactic distance fields');
    const tacticCondition=probe('move-tactic-conditions.lua','local sf2=require("sf2")\nsf2.moves.register { id="move",animation=sf2.assets.binary("animations/test"),tactic_conditions={{ | }} }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',tacticCondition));
        return ['type','name','player','conditions','minimum'].every(name=>found.some(value=>value.startsWith(name)));
    },'move tactic condition fields');
    const roundResult=probe('move-round-result.lua','local sf2=require("sf2")\nsf2.moves.register { id="move",animation=sf2.assets.binary("animations/test"),conditions={{type="round_result", | }} }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',roundResult));
        return ['name','player'].every(name=>found.some(value=>value.startsWith(name)));
    },'round result condition fields');
    const backgroundEffect=probe('move-background-effect.lua','local sf2=require("sf2")\nsf2.moves.register { id="move",animation=sf2.assets.binary("animations/test"),actions={{type="effect",frame=1,effect={name="storm",core_sequence="storm", | }}} }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',backgroundEffect));
        return ['on_background','attach'].every(name=>found.some(value=>value.startsWith(name)));
    },'background effect field');
    const moveProfileFields=probe('move-profile-fields.lua','local sf2=require("sf2")\nsf2.moves.register { id="move",animation=sf2.assets.binary("animations/test"),profile={ | } }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',moveProfileFields));
        return ['rank','core_icon','display_name'].every(name=>found.some(value=>value.startsWith(name)));
    },'move profile localization fields');
    const rewardContext=probe('reward-configure-context.lua','local sf2=require("sf2")\nsf2.rewards.register { id="reward",items={{item=sf2.items.get("core:items/weapon/WEAPON_KNIVES"),configure=function(context)\n local value=context.|\n return {}\nend}} }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',rewardContext));
        return ['player_level','item_id'].every(name=>found.includes(name));
    },'reward configure context fields');
    const rewardGrant=probe('reward-configure-grant.lua','local sf2=require("sf2")\nsf2.rewards.register { id="reward",items={{ | }} }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',rewardGrant));
        return ['item','upgrade','configure'].every(name=>found.some(value=>value===name||value===name+'?'||value.startsWith(name+' ')));
    },'reward grant fields');
    console.log('PASS: reward grant configure completes and callback context is inferred');
    const outgoing=probe('outgoing.lua','local sf2=require("sf2")\nsf2.behaviors.register { id="test",on_damage_dealing=function(_,fighter,event)\n fighter:|\nend }');
    await until(async()=>{const found=labels(await request('textDocument/completion',outgoing));return ['scale_outgoing_damage','add_outgoing_damage'].every(method=>found.some(name=>name.startsWith(method)));},'outgoing fighter method inference');
    console.log('PASS: outgoing damage callbacks infer the scoped modifier');
    for (const callback of ['on_hit_post_crit','on_post_hit']) {
        const hit=probe(callback+'-event.lua',`local sf2=require("sf2")\nsf2.behaviors.register { id="test",${callback}=function(_,fighter,event)\n local value=event.|\nend }`);
        await until(async()=>{const found=labels(await request('textDocument/completion',hit));return ['damage','blocked','critical','target','weapon','unarmed','ranged','magic'].every(field=>found.includes(field));},callback+' event fields');
    }
    const postHit=probe('post-hit-fighter.lua','local sf2=require("sf2")\nsf2.behaviors.register { id="test",on_post_hit=function(_,fighter,event)\n fighter:|\nend }');
    await until(async()=>{const found=labels(await request('textDocument/completion',postHit));return ['scale_outgoing_damage','add_outgoing_damage','show_status_icon','clear_status_icon'].every(method=>found.some(name=>name.startsWith(method)));},'post-hit fighter methods');
    const postCrit=probe('post-crit-fighter.lua','local sf2=require("sf2")\nsf2.behaviors.register { id="test",on_hit_post_crit=function(_,fighter,event)\n fighter:|\nend }');
    await until(async()=>{const found=labels(await request('textDocument/completion',postCrit));return !found.some(name=>name.startsWith('add_outgoing_damage'));},'post-crit remains read-only for pending hit');
    console.log('PASS: native hit-phase callbacks and scoped outgoing operations complete');
    for (const [callback,field] of [['on_combo_changed','last_combo'],['on_style_changed','style_rank'],['on_tick','delta_frames'],['on_animation_start','animation_name'],['on_animation_end','target']]) {
        const position=probe(callback+'.lua',`local sf2=require("sf2")\nsf2.behaviors.register { id="test",${callback}=function(_,fighter,event)\n local value=event.|\nend }`);
        await until(async()=>labels(await request('textDocument/completion',position)).includes(field),callback+' event inference');
    }
    console.log('PASS: native combo, style and tick callback fields complete');
    const aiAction=probe('ai-action.lua','local sf2=require("sf2")\nsf2.tactics.register { id="brain",on_decide=function(memory,event)\n local action=event.actions[1]\n local value=action.|\nend }');
    for (const field of ['name','type','priority','timing','inputs'])
        await until(async()=>labels(await request('textDocument/completion',aiAction)).includes(field),'AI action '+field+' completion');
    const aiTiming=probe('ai-timing.lua','local sf2=require("sf2")\nsf2.tactics.register { id="brain",on_decide=function(memory,event)\n local timing=event.actions[1].timing\n if timing then local value=timing.| end\nend }');
    for (const field of ['first_sample','last_sample','mid_frames','nominal_frames','nominal_seconds','looped'])
        await until(async()=>labels(await request('textDocument/completion',aiTiming)).includes(field),'AI timing '+field+' completion');
    const aiInput=probe('ai-input.lua','local sf2=require("sf2")\nsf2.tactics.register { id="brain",on_decide=function(memory,event)\n local input=event.actions[1].inputs[1]\n local value=input.|\nend }');
    for (const field of ['control','press'])
        await until(async()=>labels(await request('textDocument/completion',aiInput)).includes(field),'AI input '+field+' completion');
    const aiAnimation=probe('ai-animation.lua','local sf2=require("sf2")\nsf2.tactics.register { id="brain",on_decide=function(memory,event)\n local animation=event.opponent and event.opponent.animation\n if animation then local value=animation.| end\nend }');
    for (const field of ['name','type','facing','intervals'])
        await until(async()=>labels(await request('textDocument/completion',aiAnimation)).includes(field),'AI animation '+field+' completion');
    const interval=probe('combat-interval.lua','local sf2=require("sf2")\nsf2.behaviors.register { id="observer",on_round_begin=function(_,fighter)\n local combat=fighter:snapshot()\n local animation=combat and combat.self.animation\n if animation then local interval=animation.intervals[1]; local value=interval.| end\nend }');
    for (const field of ['name','type'])
        await until(async()=>labels(await request('textDocument/completion',interval)).includes(field),'Combat interval '+field+' completion');
    const uiNode=probe('ui-node.lua','local sf2=require("sf2")\nsf2.ui.open { id="menu",mount="menu",root={ | } }');
    await until(async()=>labels(await request('textDocument/completion',uiNode)).includes('kind'),'recursive UI node completion');
    const uiImage=probe('ui-image.lua','local sf2=require("sf2")\nsf2.ui.open { id="menu",mount="menu",root={ id="art",kind="image",width=64,height=64, | } }');
    await until(async()=>labels(await request('textDocument/completion',uiImage)).some(label=>label==='sprite'||label==='sprite?'),'image sprite completion');
    await until(async()=>labels(await request('textDocument/completion',uiImage)).some(label=>label==='mirrored'||label==='mirrored?'),'image mirroring completion');
    const fightEntry=probe('fight-entry.lua','local sf2=require("sf2")\nsf2.story.before_fight(first,function(request) local id=request.| end)');
    await until(async()=>labels(await request('textDocument/completion',fightEntry)).includes('fight'),'fight-entry request identity completion');
    const actScreen=probe('act-screen.lua','local sf2=require("sf2")\nsf2.ui.act_screen { | }');
    for (const field of ['lines','on_complete'])
        await until(async()=>labels(await request('textDocument/completion',actScreen)).some(label=>label===field||label===field+'?'),'act-screen '+field+' completion');
    const actLine=probe('act-line.lua','local sf2=require("sf2")\nsf2.ui.act_screen { lines={{ | }} }');
    for (const field of ['text','frames'])
        await until(async()=>labels(await request('textDocument/completion',actLine)).includes(field),'act-line '+field+' completion');
    const uiGrid=probe('ui-grid.lua','local sf2=require("sf2")\nsf2.ui.open { id="menu",mount="menu",root={ id="grid",kind="grid",width=300,height=200, | } }');
    for (const field of ['columns','cell_width','cell_height'])
        await until(async()=>labels(await request('textDocument/completion',uiGrid)).some(label=>label===field||label===field+'?'),'grid '+field+' completion');
    const uiPlacement=probe('ui-placement.lua','local sf2=require("sf2")\nsf2.ui.open { id="hud",mount="hud",placement={ | } }');
    await until(async()=>{
        const result=await request('textDocument/completion',uiPlacement);
        fs.writeFileSync(path.join(root,'.test-runtime/ui-placement.json'),JSON.stringify(result,null,2));
        return labels(result).some(name=>name.startsWith('anchor'));
    },'UI placement completion');
    console.log('PASS: UI layout nodes complete from the open definition');
    const modeResult=probe('mode-result.lua','local sf2=require("sf2")\nsf2.modes.register { id="trial",fights={},on_result=function(result)\n local won=result.|\nend }');
    await until(async()=>labels(await request('textDocument/completion',modeResult)).includes('completions'),'mode result callback completion');
    const uiStyle=probe('ui-style.lua','local sf2=require("sf2")\nsf2.ui.open { id="menu",mount="menu",root={ id="root",kind="text",style={ | } } }');
    await until(async()=>labels(await request('textDocument/completion',uiStyle)).some(name=>name.startsWith('font_size')),'UI style completion');
    const closeDefinition=probe('ui-close.lua','local sf2=require("sf2")\nsf2.ui.open { id="menu",mount="menu", | }');
    await until(async()=>labels(await request('textDocument/completion',closeDefinition)).some(name=>name.startsWith('on_close')),'UI close callback completion');
    await until(async()=>labels(await request('textDocument/completion',closeDefinition)).some(name=>name.startsWith('on_change')),'UI change callback completion');
    await until(async()=>labels(await request('textDocument/completion',closeDefinition)).some(name=>name.startsWith('on_back')),'UI Back callback completion');
    const checkedSetter=probe('ui-checked.lua','local sf2=require("sf2")\nsf2.ui.|');
    await until(async()=>labels(await request('textDocument/completion',checkedSetter)).some(name=>name.startsWith('set_checked')),'UI checked setter completion');
    await until(async()=>labels(await request('textDocument/completion',checkedSetter)).some(name=>name.startsWith('set_sprite')),'UI sprite setter completion');
    const chargeText=fs.readFileSync(path.join(root,'templates/charge-ui/scripts/main.lua'),'utf8');
    const chargeUri=open('charge-close.lua',chargeText+'\nsf2.ui.is_open("bad handle")');
    const chargeKey=decodeURIComponent(chargeUri).toLowerCase();
    await until(()=>(diagnostics.get(chargeKey)?.length??0)>0,'temporary UI handle diagnostic');
    notify('textDocument/didChange',{textDocument:{uri:chargeUri,version:2},contentChanges:[{text:chargeText}]});
    await until(()=>diagnostics.get(chargeKey)?.length===0,'cleared Charged Strike close callback diagnostics')
        .catch(error=>{throw new Error(error.message+'\n'+JSON.stringify(diagnostics.get(chargeKey)));});
    console.log('PASS: UI close callback completes and Charged Strike has no diagnostics');

    const random=probe('random.lua','local sf2=require("sf2")\nsf2.random.|');
    await until(async()=>{const found=labels(await request('textDocument/completion',random));return ['integer','number'].every(name=>found.some(label=>label.startsWith(name)));},'random stream functions');
    const seededText=fs.readFileSync(path.join(root,'templates/seeded-trial/scripts/main.lua'),'utf8');
    const seededUri=open('seeded.lua',seededText+'\nsf2.random.integer("route","invalid",3)');
    const seededKey=decodeURIComponent(seededUri).toLowerCase();
    await until(()=>(diagnostics.get(seededKey)?.length??0)>0,'random bound type diagnostic');
    notify('textDocument/didChange',{textDocument:{uri:seededUri,version:2},contentChanges:[{text:seededText}]});
    await until(()=>diagnostics.get(seededKey)?.length===0,'cleared seeded mode diagnostics');
    console.log('PASS: random functions complete and seeded trial has no diagnostics');

    for (const name of ['programmable-ai','generated-expedition','animated-arena','dojo-selector']) {
        const source=fs.readFileSync((name==='animated-arena'||name==='dojo-selector')
            ? path.join(root,'../../Mods/example.'+name+'/scripts/main.lua')
            : path.join(root,'templates',name,'scripts/main.lua'),'utf8');
        const uri=open(name+'.lua',source+'\nsf2.price.coins("temporary error")');
        const key=decodeURIComponent(uri).toLowerCase();
        await until(()=>(diagnostics.get(key)?.length??0)>0,name+' temporary diagnostic');
        notify('textDocument/didChange',{textDocument:{uri,version:2},contentChanges:[{text:source}]});
        await until(()=>diagnostics.get(key)?.length===0,name+' cleared diagnostics')
            .catch(error=>{throw new Error(error.message+'\n'+JSON.stringify(diagnostics.get(key)));});
    }
    const aiDecision=probe('ai-decision.lua','local sf2=require("sf2")\nsf2.tactics.register { id="brain",on_decide=function(memory,event)\n local value=event.|\nend }');
    await until(async()=>labels(await request('textDocument/completion',aiDecision)).includes('actions'),'AI decision completion');
    const prepare=probe('prepare.lua','local sf2=require("sf2")\nsf2.modes.register { id="mode",fights={},on_prepare=function(request,event)\n local value=event.|\nend }');
    await until(async()=>labels(await request('textDocument/completion',prepare)).includes('step'),'mode preparation completion');
    const encounterPlan=probe('encounter-plan.lua','local sf2=require("sf2")\nsf2.modes.register { id="mode",fights={},on_prepare=function(request)\n sf2.modes.resolve(request, { | })\nend }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',encounterPlan));
        return ['rules','description'].every(field=>found.some(name=>name===field||name===field+'?'||name.startsWith(field+' ')));
    },'encounter plan dynamic rule fields');
    const hotGround=probe('hot-ground.lua','local sf2=require("sf2")\nsf2.rules.hot_ground { | }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',hotGround));
        return ['id','frames','nodes','animations','target','mode','rounds'].every(field=>found.some(name=>name===field||name===field+'?'||name.startsWith(field+' ')));
    },'hot ground rule fields');
    const hotGroundNode=probe('hot-ground-node.lua','local sf2=require("sf2")\nsf2.rules.hot_ground { id="floor",frames=300,nodes={{ | }} }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',hotGroundNode));
        return ['name','axis','min','max'].every(field=>found.some(name=>name===field||name===field+'?'||name.startsWith(field+' ')));
    },'hot ground node fields');
    const nativeRule=probe('native-rule.lua','local sf2=require("sf2")\nsf2.rules.|');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',nativeRule));
        return ['hot_ground','ring_out','regeneration','no_animation','remove_interval'].every(field=>found.some(name=>name.startsWith(field)));
    },'native challenge rule functions');
    const movePatch=probe('move-perk-lock.lua','local sf2=require("sf2")\nsf2.moves.remove_perk_lock { | }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',movePatch));
        return ['move','perk'].every(field=>found.some(name=>name===field||name.startsWith(field+' ')));
    },'move perk-lock removal fields');
    const body=probe('body.lua','local sf2=require("sf2")\nsf2.warriors.register { | }');
    await until(async()=>labels(await request('textDocument/completion',body)).some(name=>name.startsWith('body_model')),'character model completion');
    const attack=probe('attack.lua','local sf2=require("sf2")\nsf2.moves.register { intervals={{type="Attack",attack={ | }}} }');
    await until(async()=>labels(await request('textDocument/completion',attack)).some(name=>name.startsWith('edges')),'attack interval completion');
    const profileProbe=probe('profile-query.lua','local sf2=require("sf2")\nlocal item=sf2.items.get("core:items/weapon/weapon_nunchaku")\nlocal snapshot=sf2.profile.item("core:items/weapon/weapon_nunchaku")\nlocal value=snapshot.|');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',profileProbe));
        return ['present','owned','count','equipped','type','subtype'].every(field=>result.includes(field));
    },'profile item snapshot fields');
    const perkProbe=probe('profile-perk.lua','local sf2=require("sf2")\nlocal perk=sf2.perks.get("core:perks/PERK_COBRA")\nlocal snapshot=sf2.profile.perk("core:perks/PERK_COBRA")\nlocal value=snapshot.|');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',perkProbe));
        return ['learned','upgrade'].every(field=>result.includes(field));
    },'learned perk snapshot fields');
    const equipmentProbe=probe('profile-equipment.lua','local sf2=require("sf2")\nlocal items=sf2.profile.equipment()\nlocal value=items[1].|');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',equipmentProbe));
        return ['item','owned','count','type','subtype','upgrade'].every(field=>result.includes(field));
    },'equipment array snapshot fields');
    const storyProbe=probe('story-event.lua','local sf2=require("sf2")\nsf2.story.on("purchase",function(event)\nlocal value=event.|\nend)');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',storyProbe));
        return ['kind','item','recipe','previous_level','level','scene','previous_count','count','fight','outcome','eclipse','equipment'].every(field=>result.includes(field));
    },'story event callback fields');
    const groupPatchProbe=probe('group-patch.lua','local sf2=require("sf2")\nsf2.items.set_tactic_subtype { | }');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',groupPatchProbe));
        return ['item','group'].every(field=>result.some(name=>name.startsWith(field)));
    },'weapon group override fields');
    const weaponGroupProbe=probe('weapon-group.lua','local sf2=require("sf2")\nsf2.items.register_weapon { | }');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',weaponGroupProbe));
        return result.some(name=>name.startsWith('tactic_subtype'));
    },'weapon tactic group field');
    const innateProbe=probe('innate-perks.lua','local sf2=require("sf2")\nsf2.items.set_innate_perks { entries={{ | }} }');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',innateProbe));
        return ['perk','parameters'].every(field=>result.some(name=>name === field || name === field+'?' || name.startsWith(field + ' ')));
    },'innate perk entry fields');
    const loadoutProbe=probe('default-enchantments.lua','local sf2=require("sf2")\nsf2.items.set_default_enchantments { entries={{ | }} }');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',loadoutProbe));
        return ['perk','aspect'].every(field=>result.some(name=>name === field || name === field+'?' || name.startsWith(field + ' ')));
    },'default enchantment entry fields');
    const deviationProbe=probe('forge-deviation.lua','local sf2=require("sf2")\nsf2.forge.override_deviation { | }');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',deviationProbe));
        return ['profile','equipment','minimum','maximum'].every(field=>result.some(name=>name === field || name.startsWith(field + ' ')));
    },'forge deviation fields');
    const forgeProbe=probe('forge-exclusion.lua','local sf2=require("sf2")\nsf2.forge.exclude_candidate { | }');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',forgeProbe));
        return ['profile','perk','equipment'].every(field=>result.some(name=>name === field || name.startsWith(field + ' ')));
    },'forge exclusion fields');
    const curveProbe=probe('location-curve.lua','local sf2=require("sf2")\nsf2.locations.register { layers = { { images = { { motion_y = { points = { { | } } } } } } } }');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',curveProbe));
        return ['period','value','ease'].every(field=>result.some(name=>name === field || name === field + '?' || name.startsWith(field + ' ')));
    },'location curve point inference');
    console.log('PASS: procedural workflow and AI examples have no diagnostics; character, attack, location curve and callback fields complete');

    const moveEffectProbe=probe('move-effect.lua','local sf2=require("sf2")\nsf2.moves.register { id="effect", actions={{ type="effect", effect={ | } }} }');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',moveEffectProbe));
        return ['name','core_sequence','scale','time_scale','looped','position','follow','attach'].every(field=>result.some(value=>value.startsWith(field)));
    },'scheduled move effect fields');
    const moveAttachProbe=probe('move-effect-attach.lua','local sf2=require("sf2")\nsf2.moves.register { id="effect", actions={{ type="effect", effect={name="shock",core_sequence="shock",attach={ | } } }} }');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',moveAttachProbe));
        return ['player','root_point','attach_point','offset_x','offset_y','start_rotation'].every(field=>result.some(value=>value.startsWith(field)));
    },'scheduled effect attachment fields');

    const projectileProbe=probe('move-projectile.lua','local sf2=require("sf2")\nsf2.moves.register { id="projectile", actions={{ type="create_projectile", projectile={ | } }} }');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',projectileProbe));
        return ['name','core_skeleton','copy_parent_type','item','core_start_animation','start_move'].every(field=>result.some(value=>value.startsWith(field)));
    },'scheduled projectile fields');
    const bulletsProbe=probe('move-bullets.lua','local sf2=require("sf2")\nsf2.moves.register { id="charge", actions={{ type="add_bullets", bullets={ | } }} }');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',bulletsProbe));
        return ['type','value'].every(field=>result.some(value=>value.startsWith(field)));
    },'scheduled charge fields');

    const spellMotionProbe=probe('spell-motion.lua','local sf2=require("sf2")\nsf2.moves.register { id="spell", velocity={ | } }');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',spellMotionProbe));
        return ['x','y','z','ax','ay','az','save_velocity'].every(field=>result.some(value=>value.startsWith(field)));
    },'spell velocity fields');
    const chargeConditionProbe=probe('spell-condition.lua','local sf2=require("sf2")\nsf2.moves.register { id="spell", conditions={{ type="bullets", | }} }');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',chargeConditionProbe));
        return ['bullet_type','minimum','maximum','player','not'].every(field=>result.some(value=>value.startsWith(field)));
    },'spell charge condition fields');

    const spellAttackProbe=probe('spell-attack.lua','local sf2=require("sf2")\nsf2.moves.register { id="spell", intervals={{ type="Attack", attack={options={ | }} }} }');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',spellAttackProbe));
        return ['no_effect','no_critical','ignores_block','body_part','defense_types','ignores_invulnerable'].every(field=>result.some(value=>value.startsWith(field)));
    },'spell attack options');
    const physicalHitProbe=probe('physical-hit.lua','local sf2=require("sf2")\nsf2.moves.register_template { id="hit",intervals={{type="Attack",attack={edges={"Edge"},hit=|}}} }');
    await until(async()=>{ const found=labels(await request('textDocument/completion',physicalHitProbe)); return ['Physycal','HighLong','NoReaction'].every(name=>found.some(value=>value.includes(name))); },'native physical and long high-hit reaction completion');
    for (const [kind,field,expected] of [['sound','sound',['core_sound','voice']],['shake_screen','shake',['pause_time','effect_time','amplitude_x','amplitude_y','frequency_x','frequency_y']]]) {
        const point=probe('move-'+kind+'.lua',`local sf2=require("sf2")\nsf2.moves.register { id="move",animation=sf2.assets.binary("animations/test"),actions={{type="${kind}",frame=1,${field}={ | }}} }`);
        await until(async()=>{ const found=labels(await request('textDocument/completion',point)); return expected.every(name=>found.some(value=>value.startsWith(name))); },kind+' nested fields');
    }
    const hitMoveProbe=probe('owned-hit.lua','local sf2=require("sf2")\nsf2.moves.register_template { id="attack",intervals={{type="Attack",attack={ | }}} }');
    await until(async()=>labels(await request('textDocument/completion',hitMoveProbe)).some(value=>value.startsWith('hit_move')),'owned hit move field');
    const impulseDirectionProbe=probe('impulse-direction.lua','local sf2=require("sf2")\nsf2.moves.register { id="reaction",animation=sf2.assets.binary("animations/test"),direction={impulse={ | }} }');
    await until(async()=>labels(await request('textDocument/completion',impulseDirectionProbe)).some(value=>value.startsWith('reverse')),'impulse direction reverse field');
    const distanceConditionProbe=probe('spell-distance.lua','local sf2=require("sf2")\nsf2.moves.register { id="spell", conditions={{ type="distance", | }} }');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',distanceConditionProbe));
        return ['axis','from','to','minimum','maximum','not'].every(field=>result.some(value=>value.startsWith(field)));
    },'spell distance condition fields');

    const warriorPerkProbe=probe('warrior-perk.lua','local sf2=require("sf2")\nsf2.warriors.register {id="opponent",perks={{ | }}}');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',warriorPerkProbe));
        return ['perk','aspect','chance_factor','chance','frames'].every(name=>found.some(value=>value.startsWith(name)));
    },'warrior perk settings fields');
    const rewardEconomyProbe=probe('reward-economy.lua','local sf2=require("sf2")\nsf2.rewards.register {id="victory", | }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',rewardEconomyProbe));
        return ['experience','prize_base'].every(name=>found.some(value=>value.startsWith(name)));
    },'reward experience and performance base fields');
    const progressProbe=probe('fight-progress.lua','local sf2=require("sf2")\nlocal progress=sf2.profile.fight("core:fights/zone_1/tournament/3")\nprogress.|');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',progressProbe));
        return ['present','wins','losses'].every(name=>found.some(value=>value.startsWith(name)));
    },'saved fight progress snapshot fields');

    const invalidUri = open('invalid.lua', [
        'local sf2 = require("sf2")',
        'sf2.items.register_weapon {',
        '    id = "bad",',
        '    display_name = sf2.localization.key("weapon.training_blade"),',
        '    icon = sf2.assets.model("wrong_kind"),',
        '    model = sf2.assets.model("core:gamedata/models/mdl_weapon_katana_ritual"),',
        '}',
        'sf2.price.coins("five")',
        'sf2.items.register_weapon { id = "missing_fields" }',
        'sf2.items.register_wepon {}',
    ].join('\n'));
    await until(() => (diagnostics.get(decodeURIComponent(invalidUri).toLowerCase())?.length ?? 0) >= 4, 'invalid sample diagnostics');
    const errors = diagnostics.get(decodeURIComponent(invalidUri).toLowerCase());
    fs.writeFileSync(path.join(root, '.test-runtime/diagnostics.json'), JSON.stringify(errors, null, 2));
    for (const line of [4, 7, 8, 9]) assert(errors.some(d => d.range.start.line === line), `Missing diagnostic at line ${line + 1}: ${JSON.stringify(errors)}`);
    console.log('PASS: wrong handle, wrong scalar type, missing fields, and misspelled function are diagnosed');
    console.log('All LuaLS integration checks passed. This verifies editor behavior, not game execution.');
    await request('shutdown', null);
    notify('exit');
}
server.on('error', error => { console.error(error); process.exitCode = 1; });
main().catch(error => { console.error(error); process.exitCode = 1; }).finally(() => {
    for (const entry of pending.values()) clearTimeout(entry.timer);
    server.kill();
});
